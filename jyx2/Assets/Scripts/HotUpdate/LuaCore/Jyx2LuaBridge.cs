/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using XLua;
using UnityEngine.Playables;
using Sirenix.Utilities;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2.Middleware;
using UnityEngine.Rendering.PostProcessing;
using Rewired;

namespace Jyx2
{
    public class JYX2EventContext
    {
        public int currentItemId;
        
        public static JYX2EventContext current = null;
    }
    

    /// <summary>
    /// lua的桥接函数
    ///
    /// 都是在Unity主线程中被调用，放心食用
    /// </summary>
    [LuaCallCSharp]
    public  static partial class Jyx2LuaBridge
    {
        //static StoryEngine storyEngine => StoryEngine.Instance;
        private static GameRuntimeData runtime => GameRuntimeData.Instance;
        public static bool isQuickBattle = false;


        #region 交互&UI

        //---UIPanel
        /// <summary>
        /// 对话
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="content"></param>
        /// <param name="callback"></param>
        public static void Talk(int roleId, string content, Action callback)
        {
            Talk(roleId, content, "", 0, callback);
        }
        /// <summary>
        /// 对话
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="content"></param>
        /// <param name="talkName"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public static void Talk(int roleId, string content, string talkName, int type, Action callback)
        {
            StoryEngine.BlockPlayerControl = true;
            Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.RoleId, roleId, content, type, new Action(() =>
            {
                StoryEngine.BlockPlayerControl = false;
                callback?.Invoke();
            })).Forget();
        }
        /// <summary>
        /// 询问是否战斗
        /// </summary>
        /// <param name="callback"></param>
        public static void AskBattle(Action<bool> callback)
        {
            ShowYesOrNoSelectPanel("是否与之过招？", callback);
        }
        /// <summary>
        /// 询问是否邀入队伍
        /// </summary>
        /// <param name="callback"></param>
        public static void AskJoin(Action<bool> callback)
        {
            ShowYesOrNoSelectPanel("是否要求加入？", callback);
        }
        /// <summary>
        /// 询问是否休息
        /// </summary>
        /// <param name="callback"></param>
        public static void AskRest(Action<bool> callback)
        {
            ShowYesOrNoSelectPanel("是否休息？<color=red>（温馨提示：受伤太重或中毒不回复）</color>", callback);
        }
        /// <summary>
        /// 打开韦小宝商店
        /// </summary>
        /// <remarks></remarks>
        /// <param name="callback"></param>
        public static void WeiShop(Action callback)
        {
            if (LevelMaster.IsInWorldMap)
            {
                callback();
                return;
            }

            int mapId = LevelMaster.GetCurrentGameMap().Id;
            var hasData = LuaToCsBridge.ShopTable[mapId]; // mapId和shopId对应
            if (hasData == null)
            {
                callback();
                return;
            }

            Jyx2_UIManager.Instance.ShowUIAsync(nameof(ShopUIPanel), "", new Action(() => { callback(); })).Forget();
        }
        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public static void ShowMessage(string message, Action callback)
        {
            MessageBox.ShowMessage(message, callback);
        }
        /// <summary>
        /// 显示品德值
        /// </summary>
        /// <remarks>
        /// 使用消息框进行显示
        /// </remarks>
        /// <param name="callback"></param>
        public static void ShowEthics(Action callback)
        {
            MessageBox.ShowMessage("你现在的品德指数为" + runtime.Player.Pinde, callback);
        }
        /// <summary>
        /// 显示声望值值
        /// </summary>
        /// <remarks>
        /// 使用消息框进行显示
        /// </remarks>
        /// <param name="callback"></param>
        public static void ShowRepute(Action callback)
        {
            MessageBox.ShowMessage("你现在的个人声望指数为" + runtime.Player.Shengwang, callback);
        }
        public static void ShowYesOrNoSelectPanel(string selectMessage, Action<bool> callback)
        {
            UniTask.Void(async () =>
            {
                List<string> selectionContent = new List<string>() { "是", "否" };
                StoryEngine.BlockPlayerControl = true;
                await Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.Selection, "0", selectMessage,
                    selectionContent, new Action<int>((index) =>
                    {
                        _selectResult = index;
                        StoryEngine.BlockPlayerControl = false;
                        callback(_selectResult == 0);
                    }));
            });
        }
        public static void ShowSelectPanel(int roleId, string selectMessage, LuaTable content, Action<int> callback)
        {
            UniTask.Void(async () =>
            {
                StoryEngine.BlockPlayerControl = true;
                List<string> selections = content.Cast<List<string>>();

                await Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.Selection, roleId.ToString(), selectMessage, selections, new Action<int>((index) =>
                {
                    StoryEngine.BlockPlayerControl = false;
                    callback(index);
                }));
            });
        }
        public static void ShowToast(string msg)
        {
            StoryEngine.DisplayPopInfo(msg);
        }
        /// <summary>
        /// 显示二选一框
        /// </summary>
        /// <remarks>
        /// --by citydream
        /// </remarks>
        /// <param name="selectMessage"></param>
        /// <param name="YesMessage"></param>
        /// <param name="NoMessage"></param>
        /// <param name="callback"></param>
        public static void ShowMessageSelectPanel(string selectMessage, string YesMessage, string NoMessage, Action<int> callback)
        {
            UniTask.Void(async () =>
            {
                StoryEngine.BlockPlayerControl = true;
                List<string> selections = new List<string>() { YesMessage, NoMessage };

                await Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.Selection, "0", selectMessage, selections, new Action<int>((index) =>
                {
                    StoryEngine.BlockPlayerControl = false;
                    callback(index);
                }));
            });
        }

        //---音乐
        /// <summary>
        /// 替换当前的出门音乐
        /// </summary>
        /// <param name="musicId"></param>
        public static void ChangeMMapMusic(int musicId)
        {
            LevelMaster.GetCurrentGameMap().ForceSetLeaveMusicId = musicId;
        }
        /// <summary>
        /// 播放指定id的音乐
        /// </summary>
        /// <param name="id"></param>
        public static void PlayMusic(int id)
        {
            AudioManager.PlayMusic(id);
        }
        /// <summary>
        /// 播放指定id的音效
        /// </summary>
        /// <param name="waveIndex"></param>
        public static void PlayWave(int waveIndex)
        {
            string path = "Assets/BuildSource/sound/e" + (waveIndex < 10 ? ("0" + waveIndex.ToString()) : waveIndex.ToString()) + ".wav";
            AudioManager.PlayClipAtPoint(path, Camera.main.transform.position).Forget();
        }

        //---流程控制
        /// <summary>
        /// 尝试开启一场战斗
        /// </summary>
        /// <param name="battleId"></param>
        /// <param name="callback"></param>
        public static void TryBattle(int battleId, Action<bool> callback)
        {
            var battle = LuaToCsBridge.BattleTable[battleId];
            if (battle == null)
            {
                Debug.LogError($"战斗id={battleId}未定义");
                callback(false);
                return;
            }

            TryBattleWithConfig(battle, callback);
        }
        /// <summary>
        /// 显示结束场景
        /// </summary>
        public static void jyx2_ShowEndScene()
        {
            UniTask.Void(async () =>
            {
                await DarkSceneAsync();
                await Jyx2_UIManager.Instance.ShowUIAsync(nameof(TheEnd));
                await jyx2_WaitAsync(1);
                await LightSceneAsync();
            });
        }
        /// <summary>
        /// 退出到主菜单
        /// </summary>
        public static void BackToMainMenu()
        {
            LoadingPanel.Create(null).Forget();
        }

        #endregion


        #region 场景

        /// <summary>
        /// 设置一个场景为可进入
        /// </summary>
        /// <remarks>
        /// 将指定场景的进入条件置为0
        /// </remarks>
        /// <param name="sceneId"></param>
        public static void OpenScene(int sceneId)
        {
            runtime.SetSceneEntraceCondition(sceneId, 0);
        }
        /// <summary>
        /// 开启暗场景
        /// </summary>
        /// <remarks>
        /// 至全黑有一秒的缓冲时间
        /// </remarks>
        /// <param name="callback"></param>
        public static void DarkScence(Action callback)
        {
            var blackCover = LevelMaster.Instance.BlackCover;
            if (blackCover == null)
            {
                Debug.LogError("DarkScence error，找不到LevelMaster/UI/BlackCover");
                callback();
                return;
            }

            blackCover.gameObject.SetActive(true);
            blackCover.DOFade(1, 1).OnComplete(() => callback());
        }
        /// <summary>
        /// 关闭暗场景
        /// </summary>
        /// <remarks>
        /// 关闭黑色图像遮罩同样有1秒的缓冲时间
        /// </remarks>
        /// <param name="callback"></param>
        public static void LightScence(Action callback)
        {
            var blackCover = LevelMaster.Instance.BlackCover;
            if (blackCover == null)
            {
                Debug.LogError("LightScene error，找不到LevelMaster/UI/BlackCover");
                callback();
                return;
            }

            if (!blackCover.gameObject.activeSelf)
            {
                Debug.Log("已经是SceneLight状态，不必再延迟逻辑了");
                callback();
                return;
            }

            blackCover.DOFade(0, 1).OnComplete(() =>
            {
                blackCover.gameObject.SetActive(false);
                callback();
            });
        }
        /// <summary>
        /// 开启/关闭屏幕后处理中的边缘阴影
        /// </summary>
        /// <remarks>
        /// 要求场景中必须已正确配置 PostProcessVolumn，并具有预先配置好的Vignette组件
        /// </remarks>
        /// <param name="isOn"></param>
        public static void ScreenVignette(bool isOn)
        {
            var postProcess = GameObject.FindObjectOfType<PostProcessVolume>();
            if (postProcess == null)
            {
                Debug.LogError("错误：调用ScreenVignette的场景必须包含PostProcessVolumn组件");
                return;
            }

            if (postProcess.profile.TryGetSettings<Vignette>(out var vignette))
            {
                vignette.active = isOn;
            }
        }
        /// <summary>
        /// 获取当前地图编号
        /// </summary>
        /// <remarks>
        /// --by citydream
        /// </remarks>
        /// <returns></returns>
        public static int GetCurrentGameMapid()
        {
            return LevelMaster.GetCurrentGameMap().Id;
        }
        /// <summary>
        /// 替换场景对象
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="path"></param>
        /// <param name="replace"></param>
        public static void jyx2_ReplaceSceneObject(string scene, string path, string replace)
        {
            LevelMasterBooster level = GameObject.FindObjectOfType<LevelMasterBooster>();
            level.ReplaceSceneObject(scene, path, replace);
        }
        /// <summary>
        /// 移动对象
        /// </summary>
        /// <remarks>
        /// add to handle indoor transport object
        /// path: name of destination transform
        /// parent: parent path of destination transform
        /// target: "" mean transport player. otherwise, need the full path of transport object.
        /// eahphone at 2021/6/5
        /// </remarks>
        /// <param name="path">移动目标点的节点名</param>
        /// <param name="parent">移动目标点的节点路径</param>
        /// <param name="target">移动目标的节点全路径,当为空字符串时,目标为玩家</param>
        public static void jyx2_MovePlayer(string path, string parent = "Level/Triggers", string target = "")
        {
            var levelMaster = GameObject.FindObjectOfType<LevelMaster>();
            levelMaster.TransportToTransform(parent, path, target);
        }
        /// <summary>
        /// 相机跟随目标节点
        /// </summary>
        /// <param name="path">要跟随的节点名称或全路径</param>
        public static void jyx2_CameraFollow(string path)
        {
            var followObj = GameObject.Find(path);
            if (followObj == null)
            {
                Debug.LogError("jyx2_CameraFollow 找不到物体,path=" + path);
                return;
            }
            var cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
            if (cameraBrain != null)
            {
                var vcam = cameraBrain.ActiveVirtualCamera;
                if (vcam != null)
                {
                    vcam.Follow = followObj.transform;
                }
            }
        }
        /// <summary>
        /// 相机跟随玩家
        /// </summary>
        public static void jyx2_CameraFollowPlayer()
        {
            jyx2_CameraFollow("Level/Player");
        }
        /// <summary>
        /// 主角行走到目标点
        /// </summary>
        /// <remarks>
        /// 起始与目标节点需要在"Level/NavigateObjs/"下定义
        /// </remarks>
        /// <param name="fromName">起始节点名,为-1时,主角当前位置作为起始点</param>
        /// <param name="toName">目标节点名</param>
        /// <param name="callback"></param>
        public static void jyx2_WalkFromTo(int fromName, int toName, Action callback)
        {
            var fromObj = GameObject.Find("Level/Player");
            if (fromName != -1)
            {
                fromObj = GameObject.Find($"Level/NavigateObjs/{fromName}");
            }
            var toObj = GameObject.Find($"Level/NavigateObjs/{toName}");
            if (fromObj == null || toObj == null)
            {
                GameUtil.LogError("jyx2_CameraFollow 找不到navigate物体,name=" + fromName + "/" + toName);
                callback();
                return;
            }
            var autoWalker = LevelMaster.Instance.GetPlayer().GetComponent<Jyx2_PlayerAutoWalk>();
            if (autoWalker == null)
            {
                GameUtil.LogError("找不到自动行走控件");
                callback();
                return;
            }
            autoWalker.PlayerWarkFromTo(fromObj.transform.position, toObj.transform.position, callback);
        }
        /// <summary>
        /// 切换角色动作
        /// </summary>
        /// <remarks>
        /// 调用样例（胡斐居）
        /// jyx2_SwitchRoleAnimation("Level/NPC/胡斐", "Assets/BuildSource/AnimationControllers/打坐.controller")
        /// </remarks>
        /// <param name="rolePath"></param>
        /// <param name="animationControllerPath"></param>
        public static void jyx2_SwitchRoleAnimation(string rolePath, string animationControllerPath, string scene = "")
        {
            Debug.Log("jyx2_SwitchRoleAnimation called");
            LevelMasterBooster level = GameObject.FindObjectOfType<LevelMasterBooster>();
            if (level == null)
            {
                Debug.LogError("jyx2_SwitchRoleAnimation调用错误，找不到LevelMaster");
                return;
            }

            level.ReplaceNpcAnimatorController(scene, rolePath, animationControllerPath);
        }

        //---Timeline
        /// <summary>
        /// 等待Timeline播放
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="callback"></param>
        public static void jyx2_Wait(float duration, Action callback)
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() => callback())
                .SetDelay(duration / _timelineSpeed);
        }
        /// <summary>
        /// 停止播放Timeline
        /// </summary>
        /// <param name="timelineName"></param>
        public static void jyx2_StopTimeline(string timelineName)
        {
            //UI恢复
            var mainCanvas = GameObject.Find("MainCanvas");
            if (mainCanvas != null)
                mainCanvas.transform.Find("MainUI").gameObject.SetActive(true);

            var timeLineRoot = GameObject.Find("Timeline");
            var timeLineObj = timeLineRoot.transform.Find(timelineName);

            if (timeLineObj == null)
            {
                Debug.LogError("jyx2_PlayTimeline 找不到Timeline,path=" + timelineName);
                return;
            }

            var playableDiretor = timeLineObj.GetComponent<PlayableDirector>();
            playableDiretor.stopped -= TimeLineNext;
            timeLineObj.gameObject.SetActive(false);

            var player = Jyx2Player.GetPlayer();
            player.IsInTimeline = false;
            player.gameObject.SetActive(true);
            player.m_Animator.transform.localPosition = Vector3.zero;
            player.m_Animator.transform.localRotation = Quaternion.Euler(Vector3.zero);
            if (clonePlayer != null)
            {
                GameObject.Destroy(clonePlayer.gameObject);
            }
            clonePlayer = null;

            playableDiretor.GetComponent<PlayableDirectorHelper>().ClearTempObjects();
        }
        /// <summary>
        /// 设置Timeline的播放速度
        /// </summary>
        /// <param name="speed"></param>
        public static void jyx2_SetTimelineSpeed(float speed)
        {
            _timelineSpeed = speed;
        }
        /// <summary>
        /// 简单模式播放timeline，播放完毕后直接关闭
        /// </summary>
        /// <param name="timelineName"></param>
        public static void jyx2_PlayTimelineSimple(string timelineName, bool hidePlayer = false, string hideGameObject = "", bool hideUI = false)
        {
            var timeLineRoot = GameObject.Find("Timeline");
            var timeLineObj = timeLineRoot.transform.Find(timelineName);

            if (hidePlayer)
            {
                var player = LevelMaster.Instance.GetPlayer();
                player.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(hideGameObject))
            {
                string path = "Level/" + hideGameObject;
                GameObject obj = GameObject.Find(path);
                obj.gameObject.SetActive(false);
            }

            if (hideUI)
            {
                GameObject.Find("MainCanvas").transform.Find("MainUI").gameObject.SetActive(false);
            }

            if (timeLineObj == null)
            {
                Debug.LogError("jyx2_PlayTimeline 找不到Timeline,path=" + timelineName);
                return;
            }
            timeLineObj.gameObject.SetActive(true);

            var playableDirector = timeLineObj.GetComponent<PlayableDirector>();
            GameUtil.CallWithDelay(playableDirector.duration, () =>
            {
                timeLineObj.gameObject.SetActive(false);
                if (hidePlayer)
                {
                    var player = LevelMaster.Instance.GetPlayer();
                    player.gameObject.SetActive(true);
                }
                if (!string.IsNullOrEmpty(hideGameObject))
                {
                    string path = "Level/" + hideGameObject;
                    GameObject obj = GameObject.Find(path);
                    obj.gameObject.SetActive(true);
                }
                if (hideUI)
                {
                    GameObject.Find("MainCanvas").transform.Find("MainUI").gameObject.SetActive(true);
                }
            });
        }
        /// <summary>
        /// 播放Timeline
        /// </summary>
        /// <param name="timelineName"></param>
        /// <param name="playMode"></param>
        /// <param name="isMovePlayer"></param>
        /// <param name="tagRole"></param>
        public static void jyx2_PlayTimeline(string timelineName, int playMode, bool isMovePlayer, string tagRole)
        {
            var timeLineRoot = GameObject.Find("Timeline");
            var timeLineObj = timeLineRoot.transform.Find(timelineName);

            if (timeLineObj == null)
            {
                Debug.LogError("jyx2_PlayTimeline 找不到Timeline,path=" + timelineName);

                return;
            }

            timeLineObj.gameObject.SetActive(true);
            var playableDirector = timeLineObj.GetComponent<PlayableDirector>();

            if (playMode == (int)TimeLinePlayMode.ExecuteNextEventOnEnd)
            {
                playableDirector.stopped += TimeLineNext;
            }
            else if (playMode == (int)TimeLinePlayMode.ExecuteNextEventOnPlaying)
            {

            }

            playableDirector.Play();

            //timeline播放速度
            if (_timelineSpeed != 1 && _timelineSpeed > 0)
            {
                playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(_timelineSpeed);
            }

            //UI隐藏
            var mainCanvas = GameObject.Find("MainCanvas");
            if (mainCanvas != null)
                mainCanvas.transform.Find("MainUI").gameObject.SetActive(false);


            //没有指定对象，则默认为主角播放
            if (string.IsNullOrEmpty(tagRole) || tagRole == "PLAYER")
            {
                var bindPlayerObj = Jyx2Player.GetPlayer().m_Animator.gameObject;
                DoPlayTimeline(playableDirector, bindPlayerObj, isMovePlayer);
            }
            else
            {
                string objPath = "Level/" + tagRole;
                GameObject obj = GameObject.Find(objPath);
                DoPlayTimeline(playableDirector, obj.gameObject);
            }
            var player = Jyx2Player.GetPlayer();
            if (player != null)
                player.IsInTimeline = true;
        }

        //---Flag
        public static void jyx2_SetFlag(string flagKey, string value)
        {
            runtime.SetKeyValues(GetCustomerFlagPrefix(flagKey), value);
        }
        public static string jyx2_GetFlag(string flagKey)
        {
            if (runtime.KeyExist(GetCustomerFlagPrefix(flagKey)))
                return runtime.GetKeyValues(GetCustomerFlagPrefix(flagKey));
            return "";
        }
        public static void jyx2_SetFlagInt(string flagKey, int value)
        {
            runtime.SetKeyValues(GetCustomerFlagPrefix(flagKey), value.ToString());
        }
        public static int jyx2_GetFlagInt(string flagKey)
        {
            if (runtime.KeyExist(GetCustomerFlagPrefix(flagKey)))
                return int.Parse(runtime.GetKeyValues(GetCustomerFlagPrefix(flagKey)));
            return 0;
        }

        #endregion


        #region 角色

        /// <summary>
        /// 使指定角色习得武功
        /// </summary>
        /// <remarks>
        /// 未学会武功则习得武功,否则武功等级+1
        /// </remarks>
        /// <param name="roleId"></param>
        /// <param name="magicId"></param>
        /// <param name="noDisplay">不为0且角色在队伍中时,显示提示</param>
        public static void LearnMagic2(int roleId, int magicId, int noDisplay)
        {
            var role = runtime.GetRoleInTeam(roleId);

            if (role == null)
            {
                TryFindRole(roleId, out role);
            }

            if (role == null)
            {
                Debug.LogError("调用了不存在的角色,roleId =" + roleId);
                return;
            }

            role.LearnMagic(magicId);

            //只有设置了显示，并且角色在队伍的时候才显示
            if (noDisplay != 0 && runtime.IsRoleInTeam(roleId))
            {
                var skill = LuaToCsBridge.SkillTable[magicId];
                StoryEngine.DisplayPopInfo(role.Name + "习得武学" + skill.Name);
            }
        }
        /// <summary>
        /// 设置指定角色的用毒属性
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="v"></param>
        public static void SetOneUsePoi(int roleId, int v)
        {
            if (!TryFindRole(roleId, out var role))
                return;
            role.UsePoison = v;
        }
        /// <summary>
        /// 获取指定角色等级
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static int GetRoleLevel(int roleId)
        {
            if (!TryFindRole(roleId, out var role))
                return 0;
            return role.Level;
        }
        /// <summary>
        /// 指定角色使用物品
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        public static void RoleUseItem(int roleId, int itemId)
        {
            var item = LuaToCsBridge.ItemTable[itemId];
            if (!TryFindRole(roleId, out var role))
                return;
            //武器
            if ((int)item.EquipmentType == 0)
            {
                role.Weapon = itemId;
            }
            //防具
            else if ((int)item.EquipmentType == 1)
            {
                role.Armor = itemId;
            }
            role.UseItem(item);
            runtime.SetItemUser(itemId, roleId);

            Jyx2.Jyx2LuaBridge.DispatchLuaEvent("OnRoleUseItem", roleId, itemId);
        }
        /// <summary>
        /// 指定角色卸下物品（装备）
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        public static void RoleUnequipItem(int roleId, int itemId)
        {
            if (!TryFindRole(roleId, out var role))
                return;
            var item = LuaToCsBridge.ItemTable[itemId];
            //武器
            if ((int)item.EquipmentType == 0)
            {
                role.Weapon = -1;
            }
            //防具
            else if ((int)item.EquipmentType == 1)
            {
                role.Armor = -1;
            }
            role.UnequipItem(item);
        }
        /// <summary>
        /// 设置角色身上的一个武功
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="magicIndexRole">要设置的角色武功数组的索引</param>
        /// <param name="magicId">设置为指定武功的ID</param>
        /// <param name="level">设置为指定武功经验</param>
        public static void SetOneMagic(int roleId, int magicIndexRole, int magicId, int level)
        {
            var role = runtime.GetRoleInTeam(roleId);

            if (role == null)
            {
                TryFindRole(roleId, out role);
            }

            if (role == null)
            {
                Debug.LogError("调用了不存在的角色,roleId =" + roleId);
                return;
            }

            if (magicIndexRole >= role.Wugongs.Count)
            {
                Debug.LogError("SetOneMagic调用错误，index越界");
                return;
            }

            role.Wugongs[magicIndexRole].Key = magicId;
            role.Wugongs[magicIndexRole].Level = level;
            //重置下缓存的配置
            role.Wugongs[magicIndexRole].ResetSkill();
        }
        /// <summary>
        /// 设置角色内力属性
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void SetPersonMPPro(int roleId, int value)
        {
            if (!TryFindRole(roleId, out var role))
                return;
            role.MpType = value;
        }
        /// <summary>
        /// 设置角色性别
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="sexual"></param>
        public static void SetSexual(int roleId, int sexual)
        {
            if (!TryFindRole(roleId, out var role))
                return;
            role.Sex = sexual;
        }
        /// <summary>
        /// 为角色添加物品
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        public static void NPCGetItem(int roleId, int itemId, int count)
        {
            runtime.RoleGetItem(roleId, itemId, count);
        }

        //---属性判断
        /// <summary>
        /// 判断角色的道德属性是否在区间内
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static bool JudgeEthics(int roleId, int low, int high)
        {
            return JudgeRoleValue(roleId, (r) => { return r.Pinde >= low && r.Pinde <= high; });
        }
        /// <summary>
        /// 判断角色的基础攻击属性是否在区间内
        /// </summary>
        /// <remarks>
        /// 基础攻击不包含装备的攻击加成
        /// </remarks>
        /// <param name="roleId"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static bool JudgeAttack(int roleId, int low, int high)
        {
            bool ret = JudgeRoleValue(roleId, (r) => {
                int originAttack = r.Attack - r.GetWeaponProperty("Attack") - r.GetArmorProperty("Attack");

                return originAttack >= low && originAttack <= high;
            });
            return ret;
        }
        /// <summary>
        /// 判断武学常识
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static bool JudgeWCH(int roleId, int low, int high)
        {
            return JudgeRoleValue(roleId, (r) => { return r.Wuxuechangshi >= low && r.Wuxuechangshi <= high; });
        }
        /// <summary>
        /// 判断医疗
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static bool JudgeHeal(int roleId, int low, int high)
        {
            return JudgeRoleValue(roleId, (r) => { return r.Heal >= low && r.Heal <= high; });
        }
        /// <summary>
        /// 判断拳掌
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static bool JudgeQuanzhang(int roleId, int low, int high)
        {
            return JudgeRoleValue(roleId, (r) => { return r.Quanzhang >= low && r.Quanzhang <= high; });
        }
        /// <summary>
        /// 判断御剑
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static bool JudgeYujian(int roleId, int low, int high)
        {
            return JudgeRoleValue(roleId, (r) => { return r.Yujian >= low && r.Yujian <= high; });
        }
        /// <summary>
        /// 判断攻击带毒
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static bool JudgeAttackPoison(int roleId, int low, int high)
        {
            return JudgeRoleValue(roleId, (r) => { return r.AttackPoison >= low && r.AttackPoison <= high; });
        }
        /// <summary>
        /// 判断奇门
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static bool JudgeQimen(int roleId, int low, int high)
        {
            return JudgeRoleValue(roleId, (r) => { return r.Qimen >= low && r.Qimen <= high; });
        }
        /// <summary>
        /// 判断防御
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static bool JudgeDefence(int roleId, int low, int high)
        {
            return JudgeRoleValue(roleId, (r) => { return r.Defence >= low && r.Defence <= high; });
        }
        /// <summary>
        /// 判断资质
        /// </summary>
        /// <remarks>
        /// 新增函数 by citydream
        /// </remarks>
        /// <param name="roleId"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static bool JudgeIQ(int roleId, int low, int high)
        {
            return JudgeRoleValue(roleId, (r) => { return r.IQ >= low && r.IQ <= high; });
        }

        //---属性增加

        /// <summary>
        /// 增加角色属性的通用方法
        /// </summary>
        /// <param name="roleId">需要改变属性角色的Id</param>
        /// <param name="attrName">需要改变的属性</param>
        /// <param name="v">属性增加值（可以是负数）</param>
        /// <param name="dispName">弹窗显示时属性名称（留空则不弹窗）</param>
        public static void AddAttr(int roleId, string attrName, int v, string dispName)
        {
            if (!TryFindRole(roleId, out var role))
                return;
            int delta = role.AddAttr(attrName, v);
            if (dispName != null && dispName != "")
            {
            StoryEngine.DisplayPopInfo(role.Name + dispName + (delta > 0 ? "增加" : "减少") + Math.Abs(delta));
            }
        }

        /// <summary>
        /// 增加角色的资质属性
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="v">变化量</param>
        public static void AddAptitude(int roleId, int v)
        {
            AddAttr(roleId, "IQ", v, "资质");
        }
        /// <summary>
        /// 增加医疗
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddHeal(int roleId, int value)
        {
            AddAttr(roleId, "Heal", value, "医疗");
        }
        /// <summary>
        /// 增加防御
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddDefence(int roleId, int value)
        {
            AddAttr(roleId, "Defence", value, "防御");
        }
        /// <summary>
        /// 增加拳掌
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddQuanzhang(int roleId, int value)
        {
            AddAttr(roleId, "Quanzhang", value, "拳掌");
        }
        /// <summary>
        /// 增加耍刀
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddShuadao(int roleId, int value)
        {
            AddAttr(roleId, "Shuadao", value, "耍刀");
        }
        /// <summary>
        /// 增加御剑
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddYujian(int roleId, int value)
        {
            AddAttr(roleId, "Yujian", value, "御剑");
        }
        /// <summary>
        /// 增加暗器
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddAnqi(int roleId, int value)
        {
            AddAttr(roleId, "Anqi", value, "暗器");
        }
        /// <summary>
        /// 增加奇门
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddQimen(int roleId, int value)
        {
            AddAttr(roleId, "Qimen", value, "奇门");
        }
        /// <summary>
        /// 增加武学常识
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddWuchang(int roleId, int value)
        {
            AddAttr(roleId, "Wuxuechangshi", value, "武学常识");
        }
        /// <summary>
        /// 增加功夫带毒
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddAttackPoison(int roleId, int value)
        {
            AddAttr(roleId, "AttackPoison", value, "功夫带毒");
        }
        /// <summary>
        /// 增加抗毒
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddAntiPoison(int roleId, int value)
        {
            AddAttr(roleId, "AntiPoison", value, "抗毒");
        }
        /// <summary>
        /// 增加经验
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddExp(int roleId, int value)
        {
            AddAttr(roleId, "Exp", value, "经验");
        }
        /// <summary>
        /// 增加经验，不提示
        /// </summary>
        /// <remarks>
        /// --by citydream
        /// </remarks>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddExpWithoutHint(int roleId, int value)
        {
            AddAttr(roleId, "Exp", value, null);
        }
        /// <summary>
        /// 加等级并返回实际增加的值
        /// </summary>
        /// <remarks>
        /// --by citydream
        /// </remarks>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int AddLevelreturnUper(int roleId, int value)
        {
            if (!TryFindRole(roleId, out var r))
                return 0;
            var v0 = r.Level;
            r.Level = Tools.Limit(r.Level + value, 0, GameConst.MAX_ROLE_LEVEL);
            //StoryEngine.DisplayPopInfo(r.Name + "等级增加" + (r.Level - v0));
            int Uper = r.Level - v0;
            return Uper;
        }
        /// <summary>
        /// 增加轻功
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddSpeed(int roleId, int value)
        {
            AddAttr(roleId, "Qinggong", value, "轻功");
        }
        /// <summary>
        /// 增加内力
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddMp(int roleId, int value)
        {
            if (!TryFindRole(roleId, out var r))
                return;
            var v0 = r.MaxMp;
            r.MaxMp = Tools.Limit(v0 + value, 0, GameConst.MAX_ROLE_MP);
            r.Mp = Tools.Limit(r.Mp + value, 0, GameConst.MAX_ROLE_MP);
            StoryEngine.DisplayPopInfo(r.Name + "内力增加" + (r.MaxMp - v0));
        }
        /// <summary>
        /// 加内力不提示
        /// </summary>
        /// <remarks>
        /// --by citydream
        /// </remarks>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddMpWithoutHint(int roleId, int value)
        {
            if (!TryFindRole(roleId, out var r))
                return;
            var v0 = r.MaxMp;
            r.MaxMp = Tools.Limit(v0 + value, 0, GameConst.MAX_ROLE_MP);
            r.Mp = Tools.Limit(r.Mp + value, 0, GameConst.MAX_ROLE_MP);
            // StoryEngine.DisplayPopInfo(r.Name + "内力增加" + (r.MaxMp - v0));
        }
        /// <summary>
        /// 增加基础攻击力
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddAttack(int roleId, int value)
        {
            AddAttr(roleId, "Attack", value, "武力");
        }
        /// <summary>
        /// 增加最大生命
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddHp(int roleId, int value)
        {
            if (!TryFindRole(roleId, out var r))
                return;
            var v0 = r.MaxHp;
            r.MaxHp = Tools.Limit(v0 + value, 0, GameConst.MAX_ROLE_HP);
            r.Hp = Tools.Limit(r.Hp + value, 0, GameConst.MAX_ROLE_HP);
            StoryEngine.DisplayPopInfo(r.Name + "生命增加" + (r.MaxHp - v0));
        }
        /// <summary>
        /// 加生命不提示
        /// </summary>
        /// <remarks>
        /// --by citydream
        /// </remarks>
        /// <param name="roleId"></param>
        /// <param name="value"></param>
        public static void AddHpWithoutHint(int roleId, int value)
        {
            if (!TryFindRole(roleId, out var r))
                return;
            var v0 = r.MaxHp;
            r.MaxHp = Tools.Limit(v0 + value, 0, GameConst.MAX_ROLE_HP);
            r.Hp = Tools.Limit(r.Hp + value, 0, GameConst.MAX_ROLE_HP);
            // StoryEngine.DisplayPopInfo(r.Name + "生命增加" + (r.MaxHp - v0));
        }

        #endregion


        #region 玩家

        /// <summary>
        /// 添加（减少）物品，并显示提示
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count">可为负数</param>
        public static void AddItem(int itemId, int count)
        {
            var item = LuaToCsBridge.ItemTable[itemId];
            if (item == null)
            {
                Debug.LogError("调用了未定义的物品:" + itemId);
                return;
            }

            var stringBuilder = new System.Text.StringBuilder();
            var token = nameof(Jyx2LuaBridge);
            int displayCount = Mathf.Abs(count);
            if (count < 0)
            {
                displayCount = Mathf.Min(displayCount, runtime.GetItemCount(itemId));
                //---------------------------------------------------------------------------
                //StoryEngine.DisplayPopInfo("失去物品:" + item.Name + "×" + Math.Abs(count));
                //---------------------------------------------------------------------------
                //特定位置的翻译【得到物品提示】
                //---------------------------------------------------------------------------
                stringBuilder.Append("失去物品：".GetContent(token));
                //---------------------------------------------------------------------------
                //---------------------------------------------------------------------------
            }
            else
            {
                //---------------------------------------------------------------------------
                //StoryEngine.DisplayPopInfo("得到物品:" + item.Name + "×" + Math.Abs(count));
                //---------------------------------------------------------------------------
                //特定位置的翻译【得到物品提示】
                //---------------------------------------------------------------------------
                stringBuilder.Append("得到物品：".GetContent(token));
                //---------------------------------------------------------------------------
                //---------------------------------------------------------------------------
            }

            stringBuilder.Append(item.Name);
            stringBuilder.Append("×");
            stringBuilder.Append(displayCount);
            StoryEngine.DisplayPopInfo(stringBuilder.ToString());

            runtime.AddItem(itemId, count);

        }
        /// <summary>
        /// 添加（减少）物品，不显示提示
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        public static void AddItemWithoutHint(int itemId, int count)
        {
            var item = LuaToCsBridge.ItemTable[itemId];
            if (item == null)
            {
                Debug.LogError("调用了未定义的物品:" + itemId);
                return;
            }

            runtime.AddItem(itemId, count);
        }
        /// <summary>
        /// 判断玩家是否拥有指定物品
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool HaveItem(int itemId)
        {
            return runtime.HaveItemBool(itemId);
        }
        /// <summary>
        /// 判断当前使用的物品是不是指定的物品
        /// </summary>
        /// <remarks>
        /// 当前使用的物品是玩家点击"使用物品"按钮后选择的物品
        /// </remarks>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool UseItem(int itemId)
        {
            if (JYX2EventContext.current == null)
                return false;

            return itemId == JYX2EventContext.current.currentItemId;
        }
        /// <summary>
        /// 增加玩家的品德
        /// </summary>
        /// <param name="value"></param>
        public static void AddEthics(int value)
        {
            runtime.Player.Pinde = Tools.Limit(runtime.Player.Pinde + value, 0, 100);
            /* StoryEngine.DisplayPopInfo((value > 0 ? "增加" : "减少") + "品德:" + Math.Abs(value));*/
        }
        /// <summary>
        /// 增加玩家的声望
        /// </summary>
        /// <param name="value"></param>
        public static void AddRepute(int value)
        {
            runtime.Player.Shengwang = Tools.Limit(runtime.Player.Shengwang + value, 0, GameConst.MAX_ROLE_SHENGWANG);
            /*    StoryEngine.DisplayPopInfo("增加声望:" + value);*/
        }
        /// <summary>
        /// 玩家死亡/游戏结束
        /// </summary>
        public static void Dead()
        {
            //防止死亡后传送到enterTrigger再次触发事件。临时处理办法
            ModifyEvent(-2, -2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
            Jyx2_UIManager.Instance.ShowUIAsync(nameof(GameOver)).Forget();
        }
        /// <summary>
        /// 判断玩家银两是否达到指定值
        /// </summary>
        /// <remarks>
        /// 大于或等于
        /// </remarks>
        /// <param name="money"></param>
        /// <returns></returns>
        public static bool JudgeMoney(int money)
        {
            return (runtime.GetItemCount(GameConst.MONEY_ID) >= money);
        }
        /// <summary>
        /// 判断玩家的性别
        /// </summary>
        /// <param name="sexual">0->男,1->女</param>
        /// <returns></returns>
        public static bool JudgeSexual(int sexual)
        {
            return JudgeRoleValue(0, r => r.Sex == sexual);
        }
        /// <summary>
        /// 获取玩家的银两数
        /// </summary>
        /// <returns></returns>
        public static int GetMoneyCount()
        {
            return runtime.GetItemCount(GameConst.MONEY_ID);
        }
        /// <summary>
        /// 设置玩家朝向
        /// </summary>
        /// <remarks>
        /// modify by eaphone at 2021/6/5
        /// </remarks>
        /// <param name="dir">0->-90,1->0,2->180,3->90</param>
        public static void SetRoleFace(int dir)
        {
            var player = Jyx2Player.GetPlayer();
            if (player == null)
                return;
            var movementComp = player.GetComponent<Jyx2_PlayerMovement>();
            if (movementComp == null)
                return;
            movementComp.SetRotation(dir);
        }
        #endregion


        #region 队伍
        
        /// <summary>
        /// 角色加入，同时获得对方身上的物品
        /// </summary>
        /// <param name="roleId"></param>
        public static void Join(int roleId)
        {
            if (runtime.JoinRoleToTeam(roleId, true))
            {
                if (!TryFindRole(roleId, out var role))
                    return;
                StoryEngine.DisplayPopInfo(role.Name + "加入队伍！");
            }
        }
        /// <summary>
        /// 角色加入，同时获得对方身上的物品。不提示--by citydream
        /// </summary>
        /// <param name="roleId"></param>
        public static void JoinWithoutHint(int roleId)
        {
            if (runtime.JoinRoleToTeam(roleId, true))
            {
                if (!TryFindRole(roleId, out var role))
                    return;
                if (role.Hp <= 0)
                {
                    role.Hp = 1;
                }
                //  StoryEngine.DisplayPopInfo(role.Name + "加入队伍！");
            }
        }
        /// <summary>
        /// 角色离队
        /// </summary>
        /// <param name="roleId"></param>
        public static void Leave(int roleId)
        {
            if (runtime.LeaveTeam(roleId))
            {
                if (!TryFindRole(roleId, out var role))
                    return;
                StoryEngine.DisplayPopInfo(role.Name + "离队。");
            }
        }
        /// <summary>
        /// 离队，不提示--by citydream
        /// </summary>
        /// <param name="roleId"></param>
        public static void LeaveWithoutHint(int roleId)
        {
            runtime.LeaveTeam(roleId);
        }
        /// <summary>
        /// 队伍中所有人的内力清零
        /// </summary>
        public static void ZeroAllMP()
        {
            foreach (var r in runtime.GetTeam())
            {
                r.Mp = 0;
            }
        }
        /// <summary>
        /// 判断队伍中是否存在指定角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static bool InTeam(int roleId)
        {
            return runtime.GetRoleInTeam(roleId) != null;
        }
        /// <summary>
        /// 判断队伍是否已满(>=6)
        /// </summary>
        /// <remarks>
        /// modify the logicc, when count>=6, team is full ---by eaphone at 2021/6/5
        /// </remarks>
        /// <returns></returns>
        public static bool TeamIsFull()
        {
            return runtime.GetTeamMembersCount() > GameConst.MAX_TEAMCOUNT - 1;
        }
        /// <summary>
        /// 全队恢复
        /// </summary>
        /// <remarks>
        /// --by citydream
        /// </remarks>
        public static void RestTeam()
        {
            foreach (var role in runtime.GetTeam())
            {
                role.Recover();
            }
        }
        /// <summary>
        /// 全队休息
        /// </summary>
        /// <remarks>
        /// 受伤太重(受伤程度>=33)或中毒不回复
        /// </remarks>
        public static void Rest()
        {
            foreach (var role in runtime.GetTeam())
            {
                if (role.Hurt < 33 && role.Poison <= 0)
                {
                    role.Recover();
                }
            }
        }
        /// <summary>
        /// 全队休息
        /// </summary>
        /// <remarks>
        /// 受伤太重(受伤程度>=50)或中毒不回复
        /// </remarks>
        public static void RestFight()
        {
            foreach (var role in runtime.GetTeam())
            {
                if (role.Hurt < 50 && role.Poison <= 0)
                {
                    role.Recover();
                }
            }
        }
        /// <summary>
        /// 判断队伍中是否有女性
        /// </summary>
        /// <returns></returns>
        public static bool JudgeFemaleInTeam()
        {
            foreach (var r in runtime.GetTeam())
            {
                if (r.Sex == 1)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取队伍人数
        /// </summary>
        /// <returns></returns>
        public static int GetTeamMembersCount()
        {
            return runtime.GetTeamMembersCount();
        }
        /// <summary>
        /// 获取队伍生命总和
        /// </summary>
        /// <returns></returns>
        public static int GetTeamTotalHp()
        {
            int totalHp = 0;
            foreach (var role in runtime.GetTeam())
            {
                totalHp += role.Hp;
            }
            return totalHp;
        }
        /// <summary>
        /// 获取队伍等级总和
        /// </summary>
        /// <remarks>
        /// --by citydream
        /// </remarks>
        /// <returns></returns>
        public static int GetTeamTotalLevel()
        {
            int totalLevel = 0;
            foreach (var role in runtime.GetTeam())
            {
                totalLevel += role.Level;
            }
            return totalLevel;
        }
        /// <summary>
        /// 所有人离队(不含主角)
        /// </summary>
        public static void AllLeave()
        {
            Debug.Log("call AllLeave()");
            Debug.Log(runtime.GetTeamMembersCount());

            var roleList = runtime.GetTeam().ToList();
            foreach (var role in roleList)
            {
                if (role.Key != 0)
                {
                    runtime.LeaveTeam(role.Key);
                }
            }
        }
        /// <summary>
        /// 获取队伍中的最大等级
        /// </summary>
        /// <remarks>
        /// --by citydream
        /// </remarks>
        /// <returns></returns>
        public static int GetTeamMaxLevel()
        {
            int TeamMaxLevel = 0;

            foreach (var role in runtime.GetTeam())
            {
                if (role.Level > TeamMaxLevel)
                {
                    TeamMaxLevel = role.Level;
                }
            }
            return TeamMaxLevel;
        }
        /// <summary>
        /// 获取队伍角色Id列表
        /// </summary>
        /// <returns></returns>
        public static List<int> GetTeamId()
        {
            return runtime.GetTeamId();
        }

        #endregion


        #region 事件相关

        public static void LuaExecFinished(string ret)
        {
            var s = LuaExecutor.CurrentEventSourceStack.Pop();
            s.TrySetResult(ret);
        }
        /// <summary>
        /// 修改事件
        /// </summary>
        /// <param name="scene">场景，-2为当前场景</param>
        /// <param name="eventId">事件ID，-2为保留</param>
        /// <param name="canPass">是否能够经过，-2为保留，在本作中没有作用</param>
        /// <param name="changeToEventId">修改为的编号，-2为保留，在本作中没有作用</param>
        /// <param name="interactiveEventId">交互事件ID</param>
        /// <param name="useItemEventId">使用道具事件ID</param>
        /// <param name="enterEventId">进入事件ID</param>
        /// <param name="p7">开始贴图</param>
        /// <param name="p8">结束贴图</param>
        /// <param name="p9">起始贴图</param>
        /// <param name="p10">动画延迟</param>
        /// <param name="p11">X坐标</param>
        /// <param name="p12">Y坐标</param>
        public static void ModifyEvent(int scene,int eventId,int canPass,
            int changeToEventId,int interactiveEventId,int useItemEventId,
            int enterEventId,int p7, int p8, int p9, int p10, int p11, int p12)
        {
            bool isCurrentScene = false;
            //场景ID
            if (scene == -2) //当前场景
            {
                scene = LevelMaster.GetCurrentGameMap().Id;
                isCurrentScene = true;
            }

            var evt = GameEvent.GetCurrentGameEvent();
            //事件ID
            if (eventId == -2)
            {
                if (evt == null)
                {
                    Debug.LogError("内部错误：当前的eventId为空，但是指定修改当前event");
                    return;
                }

                eventId = int.Parse(evt.name); //当前事件
            }
            else
            {
                evt = GameEventManager.GetGameEventByID(eventId.ToString());
            }

            if (isCurrentScene && evt != null) //当前场景事件如何获取
            {
                if (interactiveEventId == -2)
                {
                    interactiveEventId = int.Parse(evt.m_InteractiveEventId);
                }

                if (useItemEventId == -2)
                {
                    useItemEventId = int.Parse(evt.m_UseItemEventId);
                }

                if (enterEventId == -2)
                {
                    enterEventId = int.Parse(evt.m_EnterEventId);
                }
            }
            // 非当前场景事件如何获取
            else
            {
                if (interactiveEventId == -2)
                {
                    interactiveEventId = -1;
                }

                if (useItemEventId == -2)
                {
                    useItemEventId = -1;
                }

                if (enterEventId == -2)
                {
                    enterEventId = -1;
                }
            }

            //更新全局记录
            runtime.ModifyEvent(scene, eventId, interactiveEventId, useItemEventId, enterEventId);

            if (p7 != -2)
            {
                runtime.SetMapPic(scene, eventId, p7);
            }

            //刷新当前场景中的事件
            LevelMaster.Instance.RefreshGameEvents();
            if (interactiveEventId == -1 && evt != null)
            {
                async UniTask ExecuteCurEvent()
                {
                    await evt.MarkChest();
                }

                ExecuteCurEvent().Forget();
            }
        }
        /// <summary>
        /// 判断场景贴图
        /// </summary>
        /// <remarks>
        /// ModifyEvent里如果p7!=-2时，会更新对应{场景}_{事件}的贴图信息，可以用此方法JudegeScenePic检查对应的贴图信息
        /// </remarks>
        /// <param name="scene"></param>
        /// <param name="eventId"></param>
        /// <param name="pic"></param>
        /// <returns></returns>
        public static bool JudgeScenePic(int scene, int eventId, int pic)
        {
            bool result = false;
            //场景ID
            if (scene == -2) //当前场景
            {
                scene = LevelMaster.GetCurrentGameMap().Id;
            }

            //事件ID
            if (eventId == -2)
            {
                var evt = GameEvent.GetCurrentGameEvent();
                if (evt != null)
                {
                    eventId = int.Parse(evt.name); //当前事件
                }
            }
            var _target = runtime.GetMapPic(scene, eventId);
            //Debug.LogError(_target);
            result = _target == pic;
            return result;
        }
        /// <summary>
        /// 获取当前事件ID--by citydream
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentEventID()
        {
            int eventId = 0;
            var evt = GameEvent.GetCurrentGameEvent();
            if (evt == null)
            {
                Debug.LogError("内部错误：当前的eventId为空，但是指定修改当前event");
                return -1;
            }
            eventId = int.Parse(evt.name); //当前事件
            return eventId;
        }
        /// <summary>
        /// 修改这个接口逻辑为在当前trigger对应事件序号基础上加上v1,v2,v3
        /// </summary>
        /// <remarks>
        /// (只对大于0的进行相加，-2保留原事件序号，-1为直接设置) ---modified by eaphone at 2021/6/12
        /// </remarks>
        /// <param name="scene"></param>
        /// <param name="eventId"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        public static void Add3EventNum(int scene, int eventId, int v1, int v2, int v3)
        {
            bool isCurrentScene = false;
            //场景ID
            if (scene == -2) //当前场景
            {
                scene = LevelMaster.GetCurrentGameMap().Id;
                isCurrentScene = true;
            }

            var evt = GameEvent.GetCurrentGameEvent();
            //事件ID
            if (eventId == -2)
            {
                if (evt == null)
                {
                    Debug.LogError("内部错误：当前的eventId为空，但是指定修改当前event");
                    return;
                }

                eventId = int.Parse(evt.name); //当前事件
            }
            else
            {
                evt = GameEventManager.GetGameEventByID(eventId.ToString());
            }

            if (isCurrentScene && evt != null) //非当前场景事件如何获取
            {
                if (v1 == -2)
                {
                    //值为-2时，取当前值
                    v1 = int.Parse(evt.m_InteractiveEventId);
                }
                else if (v1 > -1)
                {
                    v1 += int.Parse(evt.m_InteractiveEventId);
                }

                if (v2 == -2)
                {
                    v2 = int.Parse(evt.m_UseItemEventId);
                }
                else if (v2 > -1)
                {
                    v2 += int.Parse(evt.m_UseItemEventId);
                }

                if (v3 == -2)
                {
                    v3 = int.Parse(evt.m_EnterEventId);
                }
                else if (v3 > -1)
                {
                    v3 += int.Parse(evt.m_EnterEventId);
                }

                runtime.ModifyEvent(scene, eventId, v1, v2, v3);

                //刷新当前场景中的事件
                LevelMaster.Instance.RefreshGameEvents();
            }
            else
            {
                if (v1 > 0)
                {
                    runtime.AddEventCount(scene, eventId, 0, v1);
                }

                if (v2 > 0)
                {
                    runtime.AddEventCount(scene, eventId, 1, v2);
                }

                if (v3 > 0)
                {
                    runtime.AddEventCount(scene, eventId, 2, v3);
                }
            }
        }
        
        /// <summary>
        /// 判断事件触发器的交互事件id是否为指定值
        /// </summary>
        /// <param name="eventIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool JudgeEventNum(int eventIndex, int value)
        {
            bool result = false;
            var evt = GameEvent.GetCurrentSceneEvent(eventIndex.ToString());
            if (evt != null)
            {
                result = (int.Parse(evt.m_InteractiveEventId) == value);
            }
            return result;
        }
        /// <summary>
        /// 判断事件触发器的指定事件id是否为指定值
        /// </summary>
        /// <remarks>
        /// 判断指定触发器的交互事件--by citydream
        /// </remarks>
        /// <param name="eventIndex"></param>
        /// <param name="EventId">0->交互事件,1->使用物品事件,2->进入事件</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool JudgePointEventNum(int eventIndex, int EventId, int value)
        {
            bool result = false;
            var evt = GameEvent.GetCurrentSceneEvent(eventIndex.ToString());
            if (evt != null)
            {
                if (EventId == 0)
                {
                    result = (int.Parse(evt.m_InteractiveEventId) == value);
                }
                else if (EventId == 1)
                {
                    result = (int.Parse(evt.m_UseItemEventId) == value);
                }
                else if (EventId == 2)
                {
                    result = (int.Parse(evt.m_EnterEventId) == value);
                }
            }
            return result;
        }
        /// <summary>
        /// 重新加载热更新
        /// </summary>
        /// <remarks>
        /// 一般是用于开发时调试，供MOD开发人员调用的指令
        /// </remarks>
        public static void PreloadLua()
        {
            LuaManager.PreloadLua();
        }
        /// <summary>
        /// 生成a到b之间不均匀分布的随机数
        /// </summary>
        /// <remarks>
        /// 靠近a的部分概率大，靠近b的部分概率小
        /// </remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetImbalancedRandomInt(int a, int b)
        {
            return Tools.GetImbalancedRandomInt(a, b);
        }

        #endregion


        #region Lua侧事件派发

        private static LuaFunction GetDispatchFunc()
        {
            var func = LuaManager.GetLuaEnv().Global.Get<LuaFunction>("LuaEventDispatcher_DispatchEvent");
            if (func == null)
            {
                Debug.LogWarning("无法获取Lua侧事件派发函数, 可能是因为初始化尚未完成");
            }
            return func;
        }

        //泛型避免装箱
        public static void DispatchLuaEvent<T1>(string Event, T1 p1)
        {
            var func = GetDispatchFunc();
            if (func == null)
                return;
            func.Action(Event, p1);
        }

        public static void DispatchLuaEvent<T1, T2>(string Event, T1 p1, T2 p2)
        {
            var func = GetDispatchFunc();
            if (func == null)
                return;
            func.Action(Event, p1, p2);
        }

        public static void DispatchLuaEvent<T1, T2, T3>(string Event, T1 p1, T2 p2, T3 p3)
        {
            var func = GetDispatchFunc();
            if (func == null)
                return;
            func.Action(Event, p1, p2, p3);
        }

        #endregion


        #region Private

        private enum TimeLinePlayMode
        {
            ExecuteNextEventOnPlaying = 0,
            ExecuteNextEventOnEnd = 1,
        }
        private static bool _battleResult = false;
        private static Animator clonePlayer;
        private static float _timelineSpeed = 1;
        private static int _selectResult;
        public static async UniTask TalkAsync(int roleId, string content, string talkName, int type)
        {
            bool finished = false;
            Talk(roleId, content, talkName, type, () =>
            {
                finished = true;
            });
            await UniTask.WaitUntil(() => finished);
        }
        //开始一场战斗
        public static void TryBattleWithConfig(LBattleConfig battle, Action<bool> callback)
        {
            if (isQuickBattle)
            {
                ShowYesOrNoSelectPanel("是否战斗胜利？", callback);
                return;
            }

            bool isWin = false;

            //记录当前地图和位置
            LMapConfig currentMap = LevelMaster.GetCurrentGameMap();
            var pos = LevelMaster.Instance.GetPlayerPosition();
            var rotate = LevelMaster.Instance.GetPlayerOrientation();

            LevelLoader.LoadBattle(battle, (ret) =>
            {
                LevelLoader.LoadGameMap(currentMap, new LevelMaster.LevelLoadPara()
                {
                    //还原当前地图和位置
                    loadType = LevelMaster.LevelLoadPara.LevelLoadType.ReturnFromBattle,
                    Pos = pos,
                    Rotate = rotate,
                }, () =>
                {
                    isWin = (ret == BattleResult.Win);
                    callback(isWin);
                });
            });
        }
        private static async UniTask<bool> TryBattleAsync(int battleId)
        {
            bool finished = false;
            bool rst = false;
            TryBattle(battleId, (ret) =>
            {
                rst = ret;
                finished = true;
            });
            await UniTask.WaitUntil(() => finished);
            return rst;
        }
        private static async UniTask DarkSceneAsync()
        {
            bool finished = false;
            DarkScence(() => finished = true);
            await UniTask.WaitUntil(() => finished);
        }
        private static async UniTask LightSceneAsync()
        {
            bool finished = false;
            LightScence(() => finished = true);
            await UniTask.WaitUntil(() => finished);
        }
        private static bool TryFindRole(int roleId, out RoleInstance role)
        {
            role = runtime.GetRole(roleId);
            bool isSuccess = role != null;
            if (!isSuccess)
            {
                Debug.LogWarningFormat("找不到ID为{0}的目标角色", roleId);
            }
            return isSuccess;
        }
        private static string GetCustomerFlagPrefix(string flag)
        {
            return "CustomerFlag_" + flag;
        }
        private static bool JudgeRoleValue(int roleId, Predicate<RoleInstance> judge)
        {
            var role = runtime.GetRoleInTeam(roleId);
            if (role == null)
            {
                TryFindRole(roleId, out role);
            }
            if (role == null)
            {
                Debug.LogError("调用了不存在的role，roleid=" + roleId);
                return false;
            }

            bool result = judge(role);
            return result;
        }
        public static async UniTask jyx2_WalkFromToAsync(int fromName, int toName)
        {
            bool finished = false;
            jyx2_WalkFromTo(fromName, toName, () =>
            {
                finished = true;
            });
            await UniTask.WaitUntil(() => finished);
        }
        private static void DoPlayTimeline(PlayableDirector playableDirector, GameObject player, bool isMovePlayer = false)
        {
            var bindingDic = playableDirector.playableAsset.outputs;
            bindingDic.ForEach(delegate (PlayableBinding playableBinding)
            {
                if (playableBinding.outputTargetType == typeof(Animator))
                {
                    //移动主角来播放特殊剧情
                    if (isMovePlayer)
                    {
                        playableDirector.GetComponent<PlayableDirectorHelper>().BindPlayer(player);
                    }
                    playableDirector.SetGenericBinding(playableBinding.sourceObject, player);
                }
            });
        }
        private static async UniTask jyx2_WaitAsync(float duration)
        {
            bool finished = false;
            jyx2_Wait(duration, () => finished = true);
            await UniTask.WaitUntil(() => finished);
        }


        #endregion


        #region 非通用
        //打开所有场景
        public static void OpenAllScene()
        {
            foreach(var map in LuaToCsBridge.MapTable.Values)
            {
                runtime.SetSceneEntraceCondition(map.Id, 0);
            }
            runtime.SetSceneEntraceCondition(2, 2); //云鹤崖 需要轻功大于75
            runtime.SetSceneEntraceCondition(38, 2); //摩天崖 需要轻功大于75
            runtime.SetSceneEntraceCondition(75, 1); //桃花岛
            runtime.SetSceneEntraceCondition(80, 1); //绝情谷底
        }
        public static void FightForTop()
        {
            FightForTopAsync().Forget();
        }
        //武林大会
        public static async UniTask FightForTopAsync()
        {
            Dictionary<int, string> heads = new Dictionary<int, string>();
            heads.Add(8, "唐文亮来领教阁下的高招。");
            heads.Add(21, "贫尼定闲愿领教阁下高招。");
            heads.Add(23, "贫道天门领教阁下高招。");
            heads.Add(31, "小兄弟，我们再来玩玩。");
            heads.Add(32, "小兄弟，秃笔翁陪你玩玩。");
            heads.Add(43, "白某愿领教阁下高招。");
            heads.Add(7, "何太冲来领教阁下的高招。");
            heads.Add(11, "杨逍技痒，和少侠玩玩。");
            heads.Add(14, "韦一笑技痒，和少侠玩玩。");
            heads.Add(20, "莫某再次领教阁下高招。");
            heads.Add(33, "小兄弟，黑白子向你讨教。");
            heads.Add(34, "小兄弟，黄钟公向你讨教。");
            heads.Add(10, "范某技痒，和少侠玩玩。");
            heads.Add(12, "老朽技痒，和少侠玩玩。");
            heads.Add(19, "岳某不才，向少侠挑战。");
            heads.Add(22, "左冷禅愿领教阁下高招。");
            heads.Add(56, "黄蓉愿领教阁下高招。");
            heads.Add(68, "丘处机领教阁下高招。");
            heads.Add(13, "谢某技痒，和少侠玩玩。");
            heads.Add(55, "郭靖愿领教阁下高招。");
            heads.Add(62, "老夫领教少侠高招！");
            heads.Add(67, "裘千仞来领教阁下的高招。");
            heads.Add(70, "阿弥陀佛，贫僧愿向少侠挑战。");
            heads.Add(71, "洪某拜教！");
            heads.Add(26, "任某来领教阁下的高招。");
            heads.Add(57, "少侠的确武功高强，我黄老邪来领教领教。");
            heads.Add(60, "让我老毒物来会会你。");
            heads.Add(64, "哇！你又学了这么多新奇的功夫。来，来，老顽童陪你玩玩。");
            heads.Add(3, "苗某向少侠讨教。");
            heads.Add(69, "不错不错，七公我来领教领教。");
            var ran = new System.Random();
            var keys = heads.Keys.ToList();
            var values = heads.Values.ToList();
            for (int i = 0; i < 5; i++)
            {
                var tempList = new List<int>();
                for (int i2 = 0; i2 < 3; i2++)
                {
                    int j = ran.Next(0, 6);
                    while (tempList.Contains(j))
                    {
                        j = ran.Next(0, 6);
                    }
                    tempList.Add(j);
                    await TalkAsync(keys[i * 6 + j], values[i * 6 + j], "", 0);
                    if (!await TryBattleAsync(102 + i * 6 + j))
                    {
                        Dead();
                        return;
                    }
                }
                if (i != 4)
                {
                    await TalkAsync(70, "少侠已连战三场，可先休息再战。", "talkname0", 0);
                    RestFight();
                    await DarkSceneAsync();
                    await LightSceneAsync();
                }
            }

            await TalkAsync(0, "接下来换谁？", "talkname0", 1);
            await TalkAsync(0, "…………", "talkname0", 1);
            await TalkAsync(0, "没有人了吗？", "talkname0", 1);
            await TalkAsync(70, "如果还没有人要出来向这位少侠挑战，那么这武功天下第一之名，武林盟主之位，就由这位少侠夺得。", "talkname0", 0);
            await TalkAsync(70, "………………", "talkname0", 0);
            await TalkAsync(70, "好，恭喜少侠，这武林盟主之位就由少侠获得，而这把“武林神杖”也由你保管。", "talkname0", 0);
            await TalkAsync(12, "恭喜少侠！", "talkname0", 0);
            await TalkAsync(64, "小兄弟，恭喜你！", "talkname0", 0);
            await TalkAsync(19, "好，今年的武林大会到此已圆满结束，希望明年各位武林同道能再到我华山一游。", "talkname0", 0);
            await DarkSceneAsync();
            jyx2_ReplaceSceneObject("", "NPC/华山弟子", "");
            jyx2_ReplaceSceneObject("", "NPC/battleNPC", "");
            await LightSceneAsync();
            await TalkAsync(0, "历经千辛万苦，我终于打败群雄，得到这武林盟主之位及神杖。但是“圣堂”在哪呢？为什么没人告诉我，难道大家都不知道。这会儿又有的找了。", "talkname0", 1);
            AddItem(143, 1);
        }
        ///进黑龙潭--by citydream
        public static void EnterPond()
        {
            var rad = new System.Random();
            int j = rad.Next(1, 5);
            jyx2_WalkFromToAsync(-1, j).Forget();
        }
        ///出黑龙潭--by citydream
        public static void LeavePond()
        {
            jyx2_WalkFromToAsync(-1, 0).Forget();
        }
        public static bool Judge14BooksPlaced()
        {
            return jyx2_CheckEventCount(82, 999, 0) == 14;
        }
        public static void AskSoftStar(Action callback)
        {
            UniTask.Void(async () =>
            {
                var eventLuaPath = string.Format(RuntimeEnvSetup.CurrentModConfig.LuaFilePatten, UnityEngine.Random.Range(801, 820).ToString());
                await Jyx2.LuaExecutor.Execute(eventLuaPath);
                callback();
            });
        }
        public static bool jyx2_CheckBookAndRepute()
        {
            if (runtime.Player.Shengwang < 200)
            {
                return false;
            }
            for (var i = 144; i < 158; i++)
            {
                if (!HaveItem(i))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion


        #region 空定义
        /// <summary>
        /// 修改地图
        /// </summary>
        /// <param name="sceneId">场景ID,-2为当前场景</param>
        /// <param name="layer">层级</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="v">贴图编号（需要除以2）</param>
        public static void SetScenceMap(int sceneId, int layer, int x, int y, int v)
        {
            //这个函数已经不需要实现，具体2D和3D版差异解决的方式可以参考
            //https://github.com/jynew/jynew/wiki/1.5%E6%90%AD%E5%BB%BA%E6%B8%B8%E6%88%8F%E4%B8%96%E7%95%8C%E5%B7%AE%E5%BC%82%E8%A7%A3%E5%86%B3%E5%8A%9E%E6%B3%95

        }
        public static void ChangeScencePic(int p1, int p2, int p3, int p4)
        {
            //这个函数已经不需要实现，具体2D和3D版差异解决的方式可以参考
            //https://github.com/jynew/jynew/wiki/1.5%E6%90%AD%E5%BB%BA%E6%B8%B8%E6%88%8F%E4%B8%96%E7%95%8C%E5%B7%AE%E5%BC%82%E8%A7%A3%E5%86%B3%E5%8A%9E%E6%B3%95

        }
        //播放动画
        public static void PlayAnimation(int p1, int p2, int p3)
        {
            //这个函数已经不需要实现，使用jyx2_PlayTimeline来解决
        }
        public static void WalkFromTo(int x1, int y1, int x2, int y2)
        {
            //这个函数已经不需要实现，使用jyx2_WalkFromTo来解决
        }
        public static void ScenceFromTo(int x, int y, int x2, int y2)
        {
            //重制版不需要再实现，使用  jyx2_CameraFollow、jyx2_CameraFollowPlayer

        }
        public static void Play2Amination(int eventIndex1, int beginPic1, int endPic1, int eventIndex2, int beginPic2, int endPic2)
        {
            //这个函数已经不需要实现，使用jyx2_PlayTimeline来解决
        }
        public static void instruct_50(int p1, int p2, int p3, int p4, int p5, int p6, int p7)
        {
        }
        public static void EndAmination(int p1, int p2, int p3, int p4, int p5, int p6, int p7)
        {

        }
        public static void SetScencePosition2(int x, int y)
        {
            //设置位置，没用了，调用jyx2_MovePlayer替代
        }
        public static void instruct_57()
        {

        }
        private static void TimeLineNext(PlayableDirector playableDirector)
        {

        }
        #endregion


        //targetEvent:0-interactiveEvent, 1-useItemEvent, 2-enterEvent
        public static int jyx2_CheckEventCount(int scene, int eventId, int targetEvent)
        {
            int result = 0;
            //场景ID
            if (scene == -2) //当前场景
            {
                scene = LevelMaster.GetCurrentGameMap().Id;
            }

            //事件ID
            if (eventId == -2)
            {
                var evt = GameEvent.GetCurrentGameEvent();
                if (evt == null)
                {
                    Debug.LogError("内部错误：当前的eventId为空，但是指定修改当前event");
                    return -1;
                }
                eventId = int.Parse(evt.name); //当前事件
            }

            result = runtime.GetEventCount(scene, eventId, targetEvent);
            return result;
        }
        public static void jyx2_FixMapObject(string key, string value)
        {
            runtime.KeyValues[key] = value;
            var objs = GameObject.FindObjectsOfType<FixWithGameRuntime>();
            if (objs != null)
            {
                foreach(var obj in objs)
                {
                    if(key==obj.Flag)
                        obj.Reload();
                    else continue;
                }
            }
            LevelMasterBooster level = GameObject.FindObjectOfType<LevelMasterBooster>();
            level.RefreshSceneObjects().Forget();
        }
    }
}
