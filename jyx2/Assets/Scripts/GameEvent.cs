using System.Collections;
using System.Collections.Generic;
using HSFrameWork.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 游戏的驱动事件
/// </summary>
public class GameEvent : MonoBehaviour
{
    static public GameEvent GetCurrentGameEvent()
    {
        return GameEventManager.GetCurrentGameEvent();
    }

    public const int NO_EVENT = -1;

    /// <summary>
    /// 交互对象
    /// </summary>
    public GameObject[] m_EventTargets;

    /// <summary>
    /// 交互事件
    /// </summary>
    public int m_InteractiveEventId = NO_EVENT;

    /// <summary>
    /// 使用物品事件
    /// </summary>
    public int m_UseItemEventId = NO_EVENT;

    /// <summary>
    /// 经过事件
    /// </summary>
    public int m_EnterEventId = NO_EVENT;

    /// <summary>
    /// 交互提示按钮文字
    /// </summary>
    public string m_InteractiveInfo = "交互";

    /// <summary>
    /// 使用物品按钮文字
    /// </summary>
    public string m_UseItemInfo = "使用物品";

    /// <summary>
    /// 交互物体的最小距离
    /// </summary>
    const float EVENT_TRIGGER_DISTANCE = 4;

    GameEventManager evtManager
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


    public void Init()
    {
        //如果有可交互事件，并且有绑定可交互物体。把这些物体设置为交互对象
        if(m_UseItemEventId != NO_EVENT || m_InteractiveEventId != NO_EVENT)
        {
            if(m_EventTargets != null && m_EventTargets.Length > 0)
            {
                foreach(var obj in m_EventTargets)
                {
                    if(obj != null && obj.GetComponent<InteractiveObj>() == null)
                    {
                        var interactiveObject = obj.AddComponent<InteractiveObj>();
                        interactiveObject.SetMouseClickCallback(OnClickTarget);
                    }
                }
            }
        }

        //否则清空该物体的可交互属性
        if(m_UseItemEventId == NO_EVENT && m_InteractiveEventId == NO_EVENT)
        {
            foreach (var obj in m_EventTargets)
            {
                if (obj == null) continue;
                var o = obj.GetComponent<InteractiveObj>();
                if(o != null)
                {
                    GameObject.Destroy(o);
                }
            }
        }
    }

    void OnClickTarget(InteractiveObj target)
    {
        //BY CGGG 2021/6/9，已经修改为面朝向射线触发，不会再需要鼠标点击
        //DO NOTHING

        return;
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Jyx2Player.GetPlayer().IsOnBoat)
            return;

        //先判断角色是否已经足够近了
        var levelMaster = LevelMaster.Instance;
        if (levelMaster == null)
        {
            Debug.LogError("LevelMaster is NULL! but click target triggered.");
            return;
        }

        if((levelMaster.GetPlayerPosition() - target.transform.position).magnitude > EVENT_TRIGGER_DISTANCE)
        {
            StoryEngine.Instance.DisplayPopInfo("我需要离得更近一些");
            return;
        }

        evtManager.OnClicked(this);
    }


    void OnTriggerStay(Collider other)
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (LevelMaster.Instance == null || LevelMaster.Instance.IsInited == false)
            return;
        
        //只保留进入触发事件
        if (this.m_EnterEventId == NO_EVENT)
            return;

        var player = Jyx2Player.GetPlayer();
        if (player == null || other.gameObject != player.gameObject)
            return;
        
        evtManager.OnTriggerEvent(this);
    }

    
    /*
    void OnTriggerStay(Collider other)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Jyx2Player.GetPlayer().IsOnBoat)
            return;

        //这里只触发非交互类事件
        if (this.m_EnterEventId != NO_EVENT)
        {
            evtManager.OnTriggerEvent(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Jyx2Player.GetPlayer().IsOnBoat)
            return;

        //这里只触发非交互类事件
        if (this.m_EnterEventId != NO_EVENT)
        {
            evtManager.OnTriggerEvent(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        evtManager.OnExitEvent(this);
    }*/


    public void MarkChest()
    {
        foreach (var target in m_EventTargets)
        {
            if (target == null) continue;
            var chest = target.GetComponent<MapChest>();
            if (chest != null)
            {
				chest.ChangeLockStatus(m_UseItemEventId>0);
                chest.MarkAsOpened();
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
