using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HSFrameWork.ConfigTable;
using Jyx2;
using System;
using UnityEngine.AI;
using DG.Tweening;
using HanSquirrel.ResourceManager;
using UnityEngine.Playables;
using HSFrameWork.Common;
using static Jyx2.BattleFieldModel;
using Jyx2.Setup;
using System.Linq;
using UnityEngine.UI;
using Jyx2;

public class StoryEngine : MonoBehaviour
{
    public static StoryEngine Instance;

    public DialogPanelUI dialogPanel;
    //public FullSuggestPanel fullSuggestPanel;
    //public MiddleTopMessageSuggest middleTopMessageSuggestPanel;
    public Transform popinfoContainer;

    //public FollowCamera2 followCamera;

    public bl_HUDText HUDRoot;
    public AudioSource m_AudioSource;
    public AudioSource m_SoundAudioSource;

    [HideInInspector]
    public bool BlockPlayerControl
    {
        get
        {
            return _blockPlayerControl;
        }
        set
        {
            _blockPlayerControl = value;
        }
    }



    private bool _blockPlayerControl;

    GameRuntimeData runtime
    {
        get
        {
            return GameRuntimeData.Instance;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
    }

    //当前指令指定参数
    List<GameObject> m_ParaGameObjects;

    //执行指令
    public void ExecuteCommand(string command, List<GameObject> paraGameObjects)
    {
        if (string.IsNullOrEmpty(command))
            return;

        m_ParaGameObjects = paraGameObjects;

        string cmd = command.Split('#')[0].ToLower();
        string value = command.Substring(cmd.Length + 1); //command.Split('#')[1];

        if (cmd == "dialog")
        {
            //dialogPanel.Show("", value, null);
            Jyx2_UIManager.Instance.ShowUI("ChatUIPanel", ChatType.RoleKey, "", value);
        }
        else if (cmd == "selfsay")
        {
            GameRuntimeData.Instance.Player.View.Say(value);
        }
        else if (cmd == "loadlevel")
        {
            SceneManager.LoadScene(value);
        }
        else if (cmd == "loadmap")
        {
            var loadPara = new LevelMaster.LevelLoadPara() { loadType = LevelMaster.LevelLoadPara.LevelLoadType.Load };
            LevelLoader.LoadGameMap(value, loadPara);
        }
        else if (cmd == "mapevt")
        {
            MapEvt mapEvt = ConfigTable.Get<MapEvt>(value);

            if (mapEvt != null)
            {
                //如果已经执行过，返回
                if (mapEvt.IsFinished(runtime))
                    return;

                //标记为正在执行
                mapEvt.MarkAsExecuting(runtime);

                m_CurrentMapEvt = mapEvt;
                PlayStory(mapEvt.ExecuteCode.Split('\n'), mapEvt.Result);
            }
            else
            {
                Debug.LogError("载入了错误的mapevt:" + value);
            }
        }
        else if (cmd == "timeline")
        {
            PlayTimeline(value, null);
        }
        else if (cmd == "runtimestory")
        {
            PlayStory(new string[] { value }, "");
        }
        else if (cmd == "transport")
        {
            var levelMaster = FindObjectOfType<LevelMaster>();
            levelMaster.Transport(value);
        }
        else if(cmd == "win")
        {
            BattleHelper battleHelper = FindObjectOfType<BattleHelper>();
            var model = battleHelper.GetModel();
            foreach(var role in model.Roles)
            {
                if (role.team != 0) role.SetHPAndRefreshHudBar(0);
                role.CheckDeath();
            }

            HSUtilsEx.CallWithDelay(this, () => {
                battleHelper.SwitchStatesTo(BattleHelper.BattleViewStates.WaitingForNextActiveBattleRole);
            }, 1f);
            
        }
        else if(cmd == "lose")
        {
            BattleHelper battleHelper = FindObjectOfType<BattleHelper>();
            var model = battleHelper.GetModel();
            foreach (var role in model.Roles)
            {
                if (role.team == 0) role.SetHPAndRefreshHudBar(0);
                role.CheckDeath();
            }
            HSUtilsEx.CallWithDelay(this, () =>
            {
                battleHelper.SwitchStatesTo(BattleHelper.BattleViewStates.WaitingForNextActiveBattleRole);
            }, 1f);
        }
        else if(cmd == "testlua")
        {
            LuaExecutor.Execute(value);
        }else if(cmd == "jyx2event")
        {
            LuaExecutor.Execute("jygame/ka" + value);
        }else if(cmd == "battle")
        {
            LevelLoader.LoadBattle(int.Parse(value), null);
        }
    }

    private void PlayableDiretor_stopped(PlayableDirector obj)
    {
        Debug.Log("on playable director stopped.");
        obj.gameObject.SetActive(false);
        obj.stopped -= PlayableDiretor_stopped;

        BlockPlayerControl = false;
        if (__timeLineCallback != null)
        {
            __timeLineCallback();
            __timeLineCallback = null;
        }
    }
    Action __timeLineCallback;

    void PlayTimeline(string timeLineName, Action callback)
    {

        Debug.Log("timeline command called. value = " + timeLineName);
        GameObject root = GameObject.Find("Level/Timeline");
        if (root != null)
        {
            Debug.Log("do playing");
            var timeLineObj = root.transform.Find(timeLineName).gameObject;
            var playableDiretor = timeLineObj.GetComponent<PlayableDirector>();

            playableDiretor.stopped += PlayableDiretor_stopped;

            __timeLineCallback = callback;

            //以UNBLOCK为开头的timeline不会阻塞角色行动
            BlockPlayerControl = !timeLineName.StartsWith("[UNBLOCK]");
            playableDiretor.Play();

            timeLineObj.SetActive(true);
        }
        else
        {
            Debug.LogError("can not find timeline root object: Level/Timeline");
        }
    }

    string[] m_CurrentStorys;
    string m_CurrentResult;
    int m_CurrentIndex = 0;
    MapEvt m_CurrentMapEvt;

    void PlayStory(string[] storys, string result)
    {
        m_CurrentStorys = storys;
        for(int i = 0; i < m_CurrentStorys.Length; ++i) //移除掉空格
        {
            m_CurrentStorys[i] = m_CurrentStorys[i].Trim();
        }
        m_CurrentIndex = 0;
        m_CurrentResult = result;

        PlayNextStory();
    }

    GameObject GetGameObject(string path)
    {
        if (path.StartsWith("$"))
        {
            int index = int.Parse(path.Replace("$", ""));
            if (m_ParaGameObjects == null || index >= m_ParaGameObjects.Count)
            {
                Debug.LogError("invalid para, 包含了$，但是没传GameObject参数");
                return null;
            }
            else
            {
                return m_ParaGameObjects[index];
            }
        }
        else
        {
            return GameObject.Find(path);
        }
    }

    void FinishCurrentMapEvt(int result = 0)
    {
        if (m_CurrentMapEvt != null)
        {
            m_CurrentMapEvt.MarkAsFinished(runtime);
        }

        //play result
        PlayResult(result);
    }

    void PlayNextStory()
    {
        //剧情结束
        if (m_CurrentIndex >= m_CurrentStorys.Length)
        {
            FinishCurrentMapEvt();
            return;
        }

        string currentStroy = m_CurrentStorys[m_CurrentIndex].Trim();
        m_CurrentIndex++;

        //跳过注释
        if (currentStroy.StartsWith("//"))
        {
            PlayNextStory();
            return;
        }

        var split = currentStroy.Split('*');
        string cmd = split[0];
        string value = split.Length > 1 ? split[1] : string.Empty;

        if (string.IsNullOrEmpty(cmd))
        {
            var tmp = value.Split('#');
            string roleKey = tmp[0];
            string content = tmp[1];
            BlockPlayerControl = true;
            //dialogPanel.Show(roleKey, content, () =>
            //{
            //    BlockPlayerControl = false;
            //    PlayNextStory();
            //});
            Jyx2_UIManager.Instance.ShowUI("ChatUIPanel", ChatType.RoleKey, roleKey, content,new Action(()=> 
            {
                BlockPlayerControl = false;
                PlayNextStory();
            }));
        }
        else if (cmd == "Select")
        {
            var tmp = value.Split('#');
            string roleKey = tmp[0];
            string content = tmp[1];
            List<string> selectionContent = new List<string>();
            for (int i = 2; i < tmp.Length; i++)
            {
                selectionContent.Add(tmp[i]);
            }
            BlockPlayerControl = true;
            //dialogPanel.ShowSelection(roleKey, content, selectionContent, delegate (int index)
            //{
            //    dialogPanel.gameObject.SetActive(false);
            //    BlockPlayerControl = false;
            //    PlayResult(index);
            //});
            Jyx2_UIManager.Instance.ShowUI("ChatUIPanel", ChatType.Selection, roleKey, content, selectionContent, new Action<int>((index) =>
            {
                BlockPlayerControl = false;
                PlayResult(index);
            }));
        }
        else if (cmd == "SelectJump")
        {
            var tmp = value.Split('#');
            string roleKey = tmp[0];
            string content = tmp[1];
            List<string> selectionContent = new List<string>();
            List<string> jumpTo = new List<string>();
            for (int i = 2; i < tmp.Length; i++)
            {
                var tip = tmp[i].Split(':');
                selectionContent.Add(tip[0]);
                jumpTo.Add(tip[1]);
            }
            BlockPlayerControl = true;
            //dialogPanel.ShowSelection(roleKey, content, selectionContent, delegate (int index)
            //{
            //    dialogPanel.gameObject.SetActive(false);
            //    BlockPlayerControl = false;
            //    JumpToTag(jumpTo[index]);
            //});
            Jyx2_UIManager.Instance.ShowUI("ChatUIPanel", ChatType.Selection, roleKey, content, selectionContent, new Action<int>((index) =>
            {
                BlockPlayerControl = false;
                JumpToTag(jumpTo[index]);
            }));
        }
        else if (cmd == "NpcMove")
        {
            string npcKey = value.Split('#')[0];
            GameObject npc = FindNpc(npcKey);
            if (npc == null)
                return;
            string navPointPath = value.Split('#')[1];
            GameObject navTarget = FindGameObjectOrNext(navPointPath);
            if (navTarget == null)
                return;

            var agent = npc.GetComponent<NavMeshAgent>();
            agent.destination = navTarget.transform.position;
            agent.isStopped = false;
            PlayNextStory();
        }
        else if (cmd == "LookAt")
        {
            string npcKey = value.Split('#')[0];
            GameObject npc = FindNpc(npcKey);
            if (npc == null)
                return;

            string lookToNpcKey = value.Split('#')[1];
            GameObject lookToNpc = FindNpc(lookToNpcKey);
            if (lookToNpc == null)
                return;

            var lookAtPos = new Vector3(lookToNpc.transform.position.x, npc.transform.position.y, lookToNpc.transform.position.z);
            npc.transform.DOLookAt(lookAtPos, 0.5f);
            PlayNextStory();
        }
        else if (cmd == "LookAtObject")
        {
            string npcKey = value.Split('#')[0];
            GameObject npc = FindNpc(npcKey);
            if (npc == null)
                return;

            GameObject lookAt = GetGameObject(value.Split('#')[1]);
            if (lookAt == null)
                return;

            var lookAtPos = new Vector3(lookAt.transform.position.x, npc.transform.position.y, lookAt.transform.position.z);
            npc.transform.DOLookAt(lookAtPos, 0.5f);
            PlayNextStory();
        }
        else if (cmd == "Music")
        {
            Jyx2ResourceHelper.LoadAsset<AudioClip>(value, audioClip=> {
                if (audioClip != null)
                {
                    m_AudioSource.clip = audioClip;
                    m_AudioSource.Play();
                }
                PlayNextStory();
            });
        }
        else if (cmd == "Sound")
        {
            Jyx2ResourceHelper.LoadAsset<AudioClip>(value, audioClip=> {
                if (audioClip != null)
                {
                    m_SoundAudioSource.clip = audioClip;
                    m_SoundAudioSource.Play();
                    //m_AudioSource.PlayOneShot(audioClip);
                }
                PlayNextStory();
            });
        }
        else if (cmd == "NpcGuide")
        {
            string npcKey = value.Split('#')[0];
            GameObject npc = FindNpc(npcKey);
            if (npc == null)
                return;

            var mapRole = npc.GetComponent<MapRole>();
            mapRole.SetBehavior(MapRoleBehavior.Guide);
            mapRole.m_BehaviorParas.Clear();
            mapRole.m_BehaviorParas.Add(float.Parse(value.Split('#')[1]));
            mapRole.m_BehaviorParas.Add(float.Parse(value.Split('#')[2]));
            PlayNextStory();
        }
        else if (cmd == "BlockSuggest")
        {
            //fullSuggestPanel.Show(value, PlayNextStory);
            GameUtil.ShowFullSuggest(value, "", PlayNextStory);
        }
        //else if (cmd == "CameraLocalMove")
        //{
        //    var tmp = value.Split(',');
        //    followCamera.deltaPos = new Vector3(float.Parse(tmp[0]), float.Parse(tmp[1]), float.Parse(tmp[2]));
        //    PlayNextStory();
        //}
        else if (cmd == "Timeline")
        {
            PlayTimeline(value, PlayNextStory);
        }
        else if (cmd == "Say")
        {
            string npcKey = value.Split('#')[0];
            GameObject npc = FindNpc(npcKey);
            if (npc == null)
                return;

            var mapRole = npc.GetComponent<MapRole>();
            var duration = float.Parse(value.Split('#')[2]);
            mapRole.Say(value.Split('#')[1], duration);
            HSUtilsEx.CallWithDelay(this, PlayNextStory, duration);
        }
        else if (cmd == "EnableTrigger")
        {
            GameObject triggerRoot = GameObject.Find("Level/Triggers");
            var t = triggerRoot.transform.Find(value);
            if (t != null)
            {
                t.gameObject.SetActive(true);
            }
        }
        else if (cmd == "DisableTrigger")
        {
            GameObject triggerRoot = GameObject.Find("Level/Triggers");
            var t = triggerRoot.transform.Find(value);
            if (t != null)
            {
                t.gameObject.SetActive(false);
            }
        }
        else if (cmd == "ShowBattleRole")
        {
            foreach (var roleKey in value.Split('#'))
            {
                FindNpc(roleKey).SetActive(true);
            }
            HSUtilsEx.CallWithDelay(this, PlayNextStory, 0.01f);
        }
        else if (cmd == "HideNPC")
        {
            foreach (var roleKey in value.Split('#'))
            {
                FindNpc(roleKey).SetActive(false);
            }
            HSUtilsEx.CallWithDelay(this, PlayNextStory, 0.01f);
        }
        else if (cmd == "CreateEnemy")
        {
            foreach (var data in value.Split('#'))
            {
                var roleKey = data.Split('@')[0];
                var pointKey = data.Split('@')[1];
                RoleInstance roleInstance = new RoleInstance(roleKey);
                roleInstance.CreateRoleView();
                var pointObj = FindGameObjectOrNext("Level/Dynamic/NPC/" + pointKey);
                roleInstance.View.SetBehavior(MapRoleBehavior.Enemy);
                roleInstance.View.transform.SetParent(pointObj.transform, false);
                roleInstance.View.transform.position = Vector3.zero;
                roleInstance.View.RefreshModel();
            }
            HSUtilsEx.CallWithDelay(this, PlayNextStory, 0.01f);
        }
        else if (cmd == "Battle")
        {
            //var m_BattleHelper = FindObjectOfType<BattleHelper>();
            int range = 16;
            bool playerJoin = true;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("#"))
                {
                    range = int.Parse(value.Split('#')[0]);
                    playerJoin = (value.Split('#')[1] == "true") ? true : false;
                }
                else
                {
                    range = int.Parse(value);
                }
            }
            BattleStartParams pa = new BattleStartParams()
            {
                range = range,
                playerJoin = playerJoin,
                backToBigMap = false,
                callback = new Action<BattleResult>((result) =>
                {
                    if (result == BattleResult.Win)
                    {
                        PlayResult(0);
                    }
                    else if (result == BattleResult.Lose)
                    {
                        PlayResult(1);
                    }
                }),
            };
            BattleManager.Instance.StartBattle(pa);
            //m_BattleHelper.StartBattle(delegate (BattleResult result)
            //{
            //    if (result == BattleResult.Win)
            //    {
            //        PlayResult(0);
            //    }
            //    else if (result == BattleResult.Lose)
            //    {
            //        PlayResult(1);
            //    }
            //}, range, false, playerJoin);
        }
        //else if (cmd == "ReplaceSkill")
        //{
        //    GameRuntimeData.Instance.Player.Skill = new BattleSkillInstance(value, GameRuntimeData.Instance.Player);
        //    PlayNextStory();
        //}
        else if (cmd == "SetLevel")
        {
            GameRuntimeData.Instance.TeamLevel = Convert.ToInt32(value);
            PlayNextStory();
        }
        else if (cmd == "ForceChangePlayerWeapon")
        {
            //GameRuntimeData.Instance.Player.View.ChangeWeaponTo(int.Parse(value));
            PlayNextStory();
        }
        else if (cmd == "Select")
        {
            PlayResult(0);
        }
        else if (cmd == "Join")
        {
            if (GameRuntimeData.Instance == null || GameRuntimeData.Instance.Team == null || GameRuntimeData.Instance.Team.Find(r => r.Key == value) != null)
            {
                PlayNextStory();
                return;
            }

            var role = MapRuntimeData.Instance.Roles.Find(r => { return r.Key == value; });
            role.View.SetBehavior(MapRoleBehavior.Teammate);
            role.View.m_BehaviorParas.Add(32);
            GameRuntimeData.Instance.Team.Add(role);
            
            //加入到当前队伍
            if (GameRuntimeData.Instance.CurrentTeam.Count < 4 && GameRuntimeData.Instance.Team.Contains(role) && !GameRuntimeData.Instance.CurrentTeam.Contains(role))
            {
                GameRuntimeData.Instance.CurrentTeam.Add(role);
            }
            Debug.Log($"队伍人数：{GameRuntimeData.Instance.Team.Count}");
            //fullSuggestPanel.Show(value + "加入队伍！", PlayNextStory);
            GameUtil.ShowFullSuggest(value, "", PlayNextStory);
            PlayNextStory();
        }
        else if (cmd == "ExploreJoin")
        {
            if (GameRuntimeData.Instance == null || GameRuntimeData.Instance.Team == null || GameRuntimeData.Instance.Team.Find(r => r.Key == value) != null)
            {
                PlayNextStory();
                return;
            }

            var role = MapRuntimeData.Instance.Roles.Find(r => { return r.Key == value; });
            role.View.SetBehavior(MapRoleBehavior.Teammate);
            role.View.m_BehaviorParas.Add(32);
            MapRuntimeData.Instance.ExploreTeam.Add(role);

            //fullSuggestPanel.Show(value + "临时加入队伍！", PlayNextStory);
            GameUtil.ShowFullSuggest(value, "", PlayNextStory);
            PlayNextStory();
        }
        else if (cmd == "ClearTeammate")
        {
            if (GameRuntimeData.Instance == null || GameRuntimeData.Instance.Team == null)
            {
                PlayNextStory();
                return;
            }
            if (GameRuntimeData.Instance.Team.Count > 1)
                GameRuntimeData.Instance.Team.RemoveRange(1, GameRuntimeData.Instance.Team.Count - 1);
            Debug.Log($"队伍人数：{GameRuntimeData.Instance.Team.Count}");
            PlayNextStory();
        }
        else if (cmd == "Popinfo")
        {
            DisplayPopInfo(value);
            PlayNextStory();
        }
        else if (cmd == "MiddleTopSuggest")
        {
            //middleTopMessageSuggestPanel.Show(value);
            Jyx2_UIManager.Instance.ShowUI("CommonTipsUIPanel", TipsType.MiddleTop, value);
            PlayNextStory();
        }
        else if (cmd == "Judge") //跳转指令
        {
            //string conditionJudgeContent = value.Split('#')[0];
            //string jumpTag = value.Split('#')[1];

            ////如果满足条件则跳转，否则向下执行
            //if (TriggerManager.IsTrue(conditionJudgeContent))
            //{
            //    if (IsJumpTag(jumpTag))
            //    {
            //        JumpToTag(jumpTag);
            //    }
            //    else
            //    {
            //        Debug.LogError("剧本配置错误，跳转标签格式不对，应该用[]括起来");
            //    }
            //}
            //else
            //{
            //    PlayNextStory();
            //}
        }
        else if (cmd == "Goto" || cmd == "Jump")
        {
            JumpToTag(value);
        }
        else if (cmd == "Money")
        {
            int add = int.Parse(value);
            if (add != 0)
            {
                //RuntimeHelper.Instance.AddMoney(add);
                if (add > 0)
                {
                    DisplayPopInfo($"获得银两 {add}");
                }
                else
                {
                    DisplayPopInfo($"减少银两 {-add}");
                }
            }
            PlayNextStory();
        }
        else if (cmd == "SetKey") //设置一个KeyValue
        {
            runtime.SetKeyValues(value.Split('#')[0], value.Split('#')[1]);
            PlayNextStory();
        }
        else if (cmd == "ResetTeamStatus") //恢复探索队伍血量，复活死亡队友
        {
            foreach(var role in MapRuntimeData.Instance.ExploreTeam)
            {
                role.Resurrect();
            }
            PlayNextStory();
        }
        else if (cmd == "Transport")
        {
            var levelMaster = FindObjectOfType<LevelMaster>();
            levelMaster.Transport(value);
            PlayNextStory();
        }
        else if (IsJumpTag(cmd)) //这是一个跳转标签，直接往下播放
        {
            PlayNextStory();
        }
        else if (cmd.ToLower() == "end") //直接结束当前剧情
        {
            if (!string.IsNullOrEmpty(value)) //如果有指定结果，则直接跳转
            {
                FinishCurrentMapEvt(int.Parse(value));
            }
            else//直接指针移到队尾
            {
                m_CurrentIndex = m_CurrentStorys.Length;
                PlayNextStory();
            }
        }
    }

    //判断一个标签是否是跳转标签
    bool IsJumpTag(string content)
    {
        if (content.StartsWith("[") && content.EndsWith("]")) return true;
        return false;
    }

    void JumpToTag(string tag)
    {
        for (int i=0;i< m_CurrentStorys.Length; ++i)
        {
            string content = m_CurrentStorys[i].Trim();
            if(content == tag)
            {
                m_CurrentIndex = i;
                PlayNextStory();
                return;
            }
        }

        Debug.LogError($"错误：跳转tag {tag}未在剧本中定义");
    }

    void PlayResult(int index)
    {
        List<string> results = new List<string>();
        if (m_CurrentResult.Contains("\n"))
        {
            results = m_CurrentResult.Split('\n').ToList();
        }
        else
        {
            results.Add(m_CurrentResult);
        }
        ExecuteCommand(results[index], m_ParaGameObjects);
    }


    const float POPINFO_FADEOUT_TIME = 1f;
    public void DisplayPopInfo(string msg, float duration=2f)
    {
        Jyx2_UIManager.Instance.ShowUI("CommonTipsUIPanel",TipsType.Common, msg, duration);
    }

    GameObject FindNpc(string npcKey)
    {
        if (npcKey == "Player")
        {
            return GameRuntimeData.Instance.Player.View.gameObject;
        }
        return FindGameObjectOrNext("Level/Dynamic/NPC/" + npcKey);
    }

    GameObject FindGameObjectOrNext(string path)
    {
        GameObject obj = GetGameObject(path);
        if (obj == null)
        {
            Debug.LogError("gameObject not find:" + path);
            PlayNextStory();
            return null;
        }
        else
        {
            return obj;
        }
    }

    public static bool DoLoadGame(int index)
    {
        //加载存档
        var r = GameRuntimeData.LoadArchive(index);
        if (r==null)
        {
            return false;
        }

        //初始化角色
        foreach (var role in r.Team)
        {
            role.BindKey();
        }

        var loadPara = new LevelMaster.LevelLoadPara() { loadType = LevelMaster.LevelLoadPara.LevelLoadType.Load };

        //加载地图
        LevelLoader.LoadGameMap(r.CurrentMap, loadPara);
        return true;
    }

}
