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
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 游戏的驱动事件
/// </summary>
public class GameEvent : MonoBehaviour
{
    public enum EventPriority
    {
        Lowest = 1 << 1,

        //以后如果有其他的在中间拓展

        Highest = 1 << 30,
    }

    static public GameEvent GetCurrentGameEvent()
    {
        return GameEventManager.GetCurrentGameEvent();
    }

    private const string NO_EVENT = "-1";

    /// <summary>
    /// 交互对象
    /// </summary>
    public GameObject[] m_EventTargets;

    /// <summary>
    /// 交互事件
    /// </summary>
    public string m_InteractiveEventId = NO_EVENT;

    /// <summary>
    /// 使用物品事件
    /// </summary>
    public string m_UseItemEventId = NO_EVENT;

    /// <summary>
    /// 经过事件
    /// </summary>
    public string m_EnterEventId = NO_EVENT;

    /// <summary>
    /// 交互提示按钮文字
    /// </summary>
    //---------------------------------------------------------------------------
    //public string m_InteractiveInfo = "交互";
    //---------------------------------------------------------------------------
    //特定位置的翻译【交互提示按钮文字】
    //---------------------------------------------------------------------------
    public static string InteractText => "交互".GetContent(nameof(GameEvent));
    //---------------------------------------------------------------------------
    //---------------------------------------------------------------------------

    /// <summary>
    /// 使用物品按钮文字
    /// </summary>
    //---------------------------------------------------------------------------
    //public string m_UseItemInfo = "使用物品";
    //---------------------------------------------------------------------------
    //特定位置的翻译【使用物品按钮文字】
    //---------------------------------------------------------------------------
    public static string UseItemText => "使用物品".GetContent(nameof(GameEvent));
    //---------------------------------------------------------------------------
    //---------------------------------------------------------------------------

    /// <summary>
    /// 交互物体的最小距离
    /// </summary>
    const float EVENT_TRIGGER_DISTANCE = 4;

    GameEventManager gameEventManager
    {
        get
        {
            if(_evtManager == null)
            {
                _evtManager = FindObjectOfType<GameEventManager>();
            }
            return _evtManager;
        }
    }

    GameEventManager _evtManager;

    public bool IsTriggerEnterEvent => IsValidEvent(m_EnterEventId);

    public bool IsUseItemEvent => IsValidEvent(m_UseItemEventId);

    public bool IsInteractiveEvent => IsValidEvent(m_InteractiveEventId);

    public bool IsInteractiveOrUseItemEvent => IsInteractiveEvent || IsUseItemEvent;

    public bool IsEmptyEvent => !IsUseItemEvent && !IsInteractiveEvent && !IsTriggerEnterEvent;

    public int PriorityOrder
    {
        get
        {
            int result = 0;
            if (IsTriggerEnterEvent)
                result |= (int)EventPriority.Highest;
            if (IsInteractiveOrUseItemEvent)
                result |= (int)EventPriority.Lowest;
            return result;
        }
    }


    public bool HasEventTargets => m_EventTargets != null && m_EventTargets.Length > 0;


    private bool IsValidEvent(string eventId)
    {
        if (eventId == NO_EVENT)
            return false;
        if (int.TryParse(eventId, out var v))
        {
            if (v < 0) return false;
        }
        return true;
    }


    public void Init()
    {
        //不再需要直接点击物体交互了，因此InteractiveObj就没必要初始化了 by 0kk470
    }

    private bool IsPlayerEntered(Collider collider)
    {
        var player = Jyx2Player.GetPlayer();
        if (player == null || collider.gameObject != player.gameObject)
            return false;
        return true;
    }


    void OnTriggerEnter(Collider other)
    {
        if (LevelMaster.Instance == null)
            return;

        if (!LevelMaster.Instance.IsInited)
            return;
        
        //只保留进入触发事件
        if (!IsTriggerEnterEvent)
            return;

        if (!IsPlayerEntered(other))
            return;

        if (gameEventManager == null)
            return;
        gameEventManager.OnTriggerEvent(this);
    }


    public async UniTask MarkChest()
    {
        foreach (var target in m_EventTargets)
        {
            if (target == null) continue;
            var chest = target.GetComponent<MapChest>();
            if (chest != null)
            {
				//使用物品事件为-1时可以直接打开。>0时候需要对应钥匙才能解开。-2时不能打开，参考南贤居宝箱一开始不能打开，交谈后可以直接打开
                chest.ChangeLockStatus(m_UseItemEventId != NO_EVENT);
                await chest.MarkAsOpened();
            }
        }
    }


    public static GameEvent GetCurrentSceneEvent(string id)
    {
        var evtGameObj = GameObject.Find("Level/Triggers/" + id);
        if (evtGameObj == null)
            return null;

        return evtGameObj.GetComponent<GameEvent>();
    }

}
