/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 统一管理所有的事件触发
/// </summary>
public class GameEventManager : MonoBehaviour
{
    GameEvent curEvent = null;

    public bool OnTriggerEvent(GameEvent newEvent)
    {
        if (newEvent == null)
            return false;

        if (newEvent.IsEmptyEvent)
            return false;

        if (newEvent == curEvent)
            return false;

        //新来的事件优先级更高才触发
        if (curEvent != null && curEvent.PriorityOrder >= newEvent.PriorityOrder)
            return false;

        //关闭之前的事件
        if (curEvent != null)
        {
            OnExitEvent(curEvent);
        }

        //设置当前事件
        curEvent = newEvent;
        return TryTrigger(newEvent);
    }

    public void OnExitEvent(GameEvent evt)
    {
        if (evt == curEvent)
        {
            curEvent = null;
        }

        if (evt.IsEmptyEvent)
            return;

        if(evt.HasEventTargets)
            UnityTools.DisHighLightObjects(evt.m_EventTargets);

        Jyx2_UIManager.Instance.HideUI(nameof(InteractUIPanel));
    }

    public void OnExitAllEvents()
    {
        if (curEvent == null)
            return;
        
        UnityTools.DisHighLightObjects(curEvent.m_EventTargets);
        Jyx2_UIManager.Instance.HideUI(nameof(InteractUIPanel));
        curEvent = null;
    }
    

    /// <summary>
    /// 显示交互面板
    /// </summary>
    async void ShowInteractUIPanel(GameEvent evt)
    {
        if (evt.IsEmptyEvent)
            return;

        Action OnInteract = () => ExecuteJyx2Event(curEvent.m_InteractiveEventId);
        Action OnUseItem = () => OnClickedUseItemButton();

        if (evt.IsInteractiveEvent && evt.IsUseItemEvent)
        {
            await Jyx2_UIManager.Instance.ShowUIAsync(nameof(InteractUIPanel), GameEvent.InteractText, OnInteract, GameEvent.UseItemText, OnUseItem);
        }
        else if (evt.IsInteractiveEvent)
        {
            await Jyx2_UIManager.Instance.ShowUIAsync(nameof(InteractUIPanel), GameEvent.InteractText, OnInteract);
        }
        else if(evt.IsUseItemEvent)
        {
            await Jyx2_UIManager.Instance.ShowUIAsync(nameof(InteractUIPanel), GameEvent.UseItemText, OnUseItem);
        }
    }


    async void OnClickedUseItemButton()
    {
        if (!curEvent.IsUseItemEvent) return;

        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(BagUIPanel), GameRuntimeData.Instance.Items, new Action<int>((itemId) =>
        {
            if (itemId == -1) //取消使用
                return;

            //使用道具
            ExecuteJyx2Event(curEvent.m_UseItemEventId, new JYX2EventContext() { currentItemId = itemId });
        }));
    }

    bool TryTrigger(GameEvent evt)
    {
        //直接触发
        if (evt.IsTriggerEnterEvent && !LuaExecutor.IsExecuting())
        {
            ExecuteJyx2Event(evt.m_EnterEventId);
            return true;
        }

        //既没有交互事件，也不是使用道具事件的情况
        if (!evt.IsInteractiveOrUseItemEvent) return false;

        if (!evt.HasEventTargets) return false;

        //显示交互面板
        ShowInteractUIPanel(evt);

        UnityTools.HighLightObjects(evt.m_EventTargets, Color.red);

        return true;
    }



    public void ExecuteJyx2Event(int eventId, JYX2EventContext context = null)
    {
        if (eventId < 0)
        {
            //Debug.LogError("执行错误的luaEvent，id=" + eventId);
            return;
        }

        //停止导航
        var levelMaster = LevelMaster.Instance;

        //fix player stop moving after interaction UI confirm
        if (levelMaster != null && eventId != 911)
        {
            // fix drag motion continuous move the player when scene is playing
            // modified by eaphone at 2021/05/31
            levelMaster.GetPlayer().locomotionController.playerControllable = false;
            levelMaster.GetPlayer().locomotionController.StopPlayerNavigation();
        }
        
        SetCurrentGameEvent(curEvent);
        
        //设置运行环境上下文
        JYX2EventContext.current = context;

        async UniTask ExecuteCurEvent()
        {
            //先判断是否有蓝图类
            //如果有则执行蓝图，否则执行lua
            var graph = await Jyx2ResourceHelper.LoadEventGraph(eventId);
            if (graph != null)
            {
                graph.Run(OnFinishEvent);
            }
            else
            {
                var eventLuaPath = string.Format(RuntimeEnvSetup.CurrentModConfig.LuaFilePatten, eventId);
                await Jyx2.LuaExecutor.Execute(eventLuaPath);
                OnFinishEvent();
            }
        }

        ExecuteCurEvent().Forget();
    }

    void OnFinishEvent()
    {
        JYX2EventContext.current = null;

        SetCurrentGameEvent(null);
        // fix drag motion continuous move the player when scene is playing
        // modified by eaphone at 2021/05/31
        var levelMaster = LevelMaster.Instance;
        if (levelMaster != null)
        {
            levelMaster.GetPlayer().locomotionController.playerControllable = true;
        }

        if (curEvent != null)
        {
            UnityTools.DisHighLightObjects(curEvent.m_EventTargets);
        }

        curEvent = null;
    }

    static string _currentEvt;
    public static void SetCurrentGameEvent(GameEvent evt)
    {
        if (evt == null)
        {
            _currentEvt = "";
        }
        else
        {
            _currentEvt = evt.name;
        }
    }

    public static GameEvent GetCurrentGameEvent()
    {
        return GetGameEventByID(_currentEvt);
    }
	
	public static GameEvent GetGameEventByID(string id)
	{
        if (string.IsNullOrEmpty(id))
            return null;

        var allEvents = FindObjectsOfType<GameEvent>();
        var result = Array.Find(allEvents, element => element.name == id);
        return result;
	}
}
