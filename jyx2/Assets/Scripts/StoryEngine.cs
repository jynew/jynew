/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HSFrameWork.ConfigTable;
using Jyx2;
using System;
using UnityEngine.Playables;

public class StoryEngine : MonoBehaviour
{
    public static StoryEngine Instance;


    public Transform popinfoContainer;
    public bl_HUDText HUDRoot;
    public AudioSource m_AudioSource;
    public AudioSource m_SoundAudioSource;

    [HideInInspector]
    public bool BlockPlayerControl
    {
        get { return _blockPlayerControl; }
        set { _blockPlayerControl = value; }
    }


    private bool _blockPlayerControl;

    private static GameRuntimeData runtime
    {
        get { return GameRuntimeData.Instance; }
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

        if (cmd == "selfsay")
        {
            GameRuntimeData.Instance.Player.View.Say(value);
        }
        else if (cmd == "loadlevel")
        {
            SceneManager.LoadScene(value);
        }
        else if (cmd == "loadmap")
        {
            var loadPara = new LevelMaster.LevelLoadPara() {loadType = LevelMaster.LevelLoadPara.LevelLoadType.Load};
            LevelLoader.LoadGameMap(value, loadPara);
        }
        else if (cmd == "timeline")
        {
            PlayTimeline(value, null);
        }
        else if (cmd == "transport")
        {
            var levelMaster = FindObjectOfType<LevelMaster>();
            levelMaster.Transport(value);
        }
        else if (cmd == "win")
        {
            //TODO
        }
        else if (cmd == "lose")
        {
            //TODO
        }
        else if (cmd == "testlua")
        {
            LuaExecutor.Execute(value);
        }
        else if (cmd == "jyx2event")
        {
            LuaExecutor.Execute("jygame/ka" + value);
        }
        else if (cmd == "battle")
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


    public void DisplayPopInfo(string msg, float duration = 2f)
    {
        Jyx2_UIManager.Instance.ShowUI(nameof(CommonTipsUIPanel), TipsType.Common, msg, duration);
    }

    public static bool DoLoadGame(int index)
    {
        //加载存档
        var r = GameRuntimeData.LoadArchive(index);
        if (r == null)
        {
            return false;
        }

        //初始化角色
        foreach (var role in r.Team)
        {
            role.BindKey();
            
            //因为更改了存储的数据结构，需要检查存档的数据
            if (!runtime.HaveItemBool(role.Weapon) && role.Weapon != -1) runtime.AddItem(role.Weapon, 1);
            if (!runtime.HaveItemBool(role.Armor) && role.Armor != -1) runtime.AddItem(role.Armor, 1);
            if (!runtime.HaveItemBool(role.Xiulianwupin) && role.Xiulianwupin != -1)
                runtime.AddItem(role.Xiulianwupin, 1);
        }

        var loadPara = new LevelMaster.LevelLoadPara() {loadType = LevelMaster.LevelLoadPara.LevelLoadType.Load};

        //加载地图
        // fix load game from Main menu will not transport player to last time indoor position 
        // modified by eaphone at 2021/06/01
        LevelLoader.LoadGameMap(ConfigTable.Get<GameMap>(r.CurrentMap), loadPara, "",
            () => { LevelMaster.Instance.TryBindPlayer(); });
        return true;
    }
}