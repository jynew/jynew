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
using HSFrameWork.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

/// <summary>
/// 统一管理所有的事件触发
/// </summary>
public class GameEventManager : MonoBehaviour
{
    GameEvent curEvent = null;
    const int NO_EVENT = -1;

    bool isEmptyEvent(GameEvent evt)
    {
        return IsNoEvent(evt.m_EnterEventId) && IsNoEvent(evt.m_InteractiveEventId) && IsNoEvent(evt.m_UseItemEventId);
    }

    bool isInteractiveOrUseItemEvent(GameEvent evt)
    {
        if (evt == null) return false;
        return !IsNoEvent(evt.m_InteractiveEventId) || !IsNoEvent(evt.m_UseItemEventId);
    }

    bool isEnterEvent(GameEvent evt)
    {
        if (evt == null)
            return false;
        return IsNoEvent(evt.m_EnterEventId);
    }

    public bool OnTriggerEvent(GameEvent evt)
    {
        if (isEmptyEvent(evt))
            return false;

        if (evt == curEvent)
            return false;

        //如果已经有一个交互事件占位了，并且自己事件不是立刻触发事件，则让一下优先级
        if (!isEnterEvent(evt) && isInteractiveOrUseItemEvent(curEvent))
            return false;


        //关闭之前的事件
        if (curEvent != null && curEvent != evt)
        {
            OnExitEvent(curEvent);
        }

        //设置当前事件
        curEvent = evt;
        return TryTrigger(evt);
    }

    public void OnExitEvent(GameEvent evt)
    {
        if (evt == curEvent)
        {
            curEvent = null;
        }

        if (isEmptyEvent(evt))
        {
            return;
        }

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
    void ShowInteractUIPanel(GameEvent evt)
    {
        var uiParams = new List<object>();
        int buttonCount = 0;
        
        if (!IsNoEvent(evt.m_InteractiveEventId))
        {
            uiParams.Add(curEvent.m_InteractiveInfo);
            uiParams.Add(new Action(() =>
            {
                ExecuteJyx2Event(curEvent.m_InteractiveEventId);
            }));
            buttonCount++;
        }

        //使用道具
        if (!IsNoEvent(evt.m_UseItemEventId))
        {
            uiParams.Add(curEvent.m_UseItemInfo);
            uiParams.Add(new Action(() =>
            {
                OnClickedUseItemButton();
            }));
            buttonCount++;
        }

        if (buttonCount == 1)
        {
            Jyx2_UIManager.Instance.ShowUI(nameof(InteractUIPanel), uiParams[0], uiParams[1]);
        }
        else if (buttonCount == 2)
        {
            Jyx2_UIManager.Instance.ShowUI(nameof(InteractUIPanel), uiParams[0], uiParams[1], uiParams[2], uiParams[3]);
        }
    }

    Button GetUseItemButton()
    {
        var root = GameObject.Find("LevelMaster/UI");
        var btn = root.transform.Find("UseItemButton").GetComponent<Button>();
        return btn;
    }


    void OnClickedUseItemButton()
    {
        if (curEvent.m_UseItemEventId == NO_EVENT) return;

        Jyx2_UIManager.Instance.ShowUI(nameof(BagUIPanel), GameRuntimeData.Instance.Items, new Action<int>((itemId) =>
        {
            if (itemId == -1) //取消使用
                return;

            //使用道具
            ExecuteJyx2Event(curEvent.m_UseItemEventId, new JYX2EventContext() { currentItemId = itemId });
        }));
    }

    public void OnClicked(GameEvent evt)
    {
        if (evt.m_InteractiveEventId == NO_EVENT) return;

        curEvent = evt;
        ExecuteJyx2Event(evt.m_InteractiveEventId);
    }


    bool IsNoEvent(int eventId)
    {
        if (eventId == NO_EVENT) return true;
        if (eventId < 0)
            return true;
        return false;
    }


    bool TryTrigger(GameEvent evt)
    {
        //直接触发
        if (!IsNoEvent(evt.m_EnterEventId) && !LuaExecutor.isExcutling())
        {
            ExecuteJyx2Event(evt.m_EnterEventId);
            return true;
        }

        //既没有交互事件，也不是使用道具事件的情况
        if (IsNoEvent(evt.m_InteractiveEventId) && IsNoEvent(evt.m_UseItemEventId)) return false;

        if (evt.m_EventTargets == null || evt.m_EventTargets.Length == 0) return false;

        //显示交互面板
        ShowInteractUIPanel(evt);

        UnityTools.HighLightObjects(evt.m_EventTargets, Color.red);

        return true;
    }



    private void ExecuteJyx2Event(int eventId, JYX2EventContext context = null)
    {
        if (eventId < 0)
        {
            //Debug.LogError("执行错误的luaEvent，id=" + eventId);
            return;
        }

        //停止导航
        var levelMaster = LevelMaster.Instance;
        if (levelMaster != null)
        {
            // fix drag motion continuous move the player when scene is playing
            // modified by eaphone at 2021/05/31
            levelMaster.SetPlayerCanController(false);
            levelMaster.StopPlayerNavigation();
        }
        
        SetCurrentGameEvent(curEvent);
        
        //设置运行环境上下文
        JYX2EventContext.current = context;

        void ExecuteCurEvent()
        {
            //先判断是否有蓝图类
            //如果有则执行蓝图，否则执行lua
            Jyx2ResourceHelper.LoadEventGraph(eventId, (graph) => { graph.Run(OnFinishEvent); }, () =>
            {
                //执行lua
                var eventLuaPath = "jygame/ka" + eventId;
                Jyx2.LuaExecutor.Execute(eventLuaPath, OnFinishEvent);
            });
        }

        if (curEvent != null)
        {
            StartCoroutine(curEvent.MarkChest(ExecuteCurEvent));
        }
        else
        {
            ExecuteCurEvent();
        }
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
            levelMaster.SetPlayerCanController(true);
        }

        if (curEvent != null)
        {
            UnityTools.DisHighLightObjects(curEvent.m_EventTargets);
        }

        curEvent = null;
    }

    static string _currentEvt;
    static public void SetCurrentGameEvent(GameEvent evt)
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
    static public GameEvent GetCurrentGameEvent()
    {
        return GetGameEventByID(_currentEvt);
    }
	
	static public GameEvent GetGameEventByID(string id)
	{
        if (string.IsNullOrEmpty(id))
            return null;

        foreach (var evt in FindObjectsOfType<GameEvent>())
        {
            if (evt.name == id)
                return evt;
        }

        return null;
	}
}
