using Jyx2;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Jyx2Player : MonoBehaviour
{
    /// <summary>
    /// 交互的视野范围
    /// </summary>
    const float PLAYER_INTERACTIVE_RANGE = 1f;

    /// <summary>
    /// 交互的视野角度
    /// </summary>
    const float PLAYER_INTERACTIVE_ANGLE = 120f;

    /// <summary>
    /// 是否激活交互选项
    /// </summary>
    [HideInInspector]
    public bool EnableInteractive { get; set; }

    private bool canControl = true;
    
    public static Jyx2Player GetPlayer()
    {
        if (LevelMaster.Instance == null)
            return null;
        
        return LevelMaster.Instance.GetPlayer();
    }

    
    public bool IsOnBoat;

    NavMeshAgent _navMeshAgent;
    Jyx2Boat _boat;

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

    public void GetInBoat(Jyx2Boat boat)
    {
        IsOnBoat = true;
        _boat = boat;

        _navMeshAgent.updatePosition = false;
        transform.position = boat.transform.position;
        transform.rotation = boat.transform.rotation;

        SetHide(true);
        _navMeshAgent.areaMask = GetWaterNavAreaMask();
        _navMeshAgent.updatePosition = true;
    }

    public bool GetOutBoat()
    {
        NavMeshHit myNavHit;
        if (NavMesh.SamplePosition(transform.position, out myNavHit, 3.5f, GetNormalNavAreaMask()))
        {
            //比水平面还低
            if (myNavHit.position.y < 5f)
            {
                return false;
            }

            SetHide(false);
            _navMeshAgent.areaMask = GetNormalNavAreaMask();
            IsOnBoat = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    void SetHide(bool isHide)
    {
        foreach (var r in transform.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            r.enabled = !isHide;
        }
    }

    /// <summary>
    /// 获取水路行走mask
    /// </summary>
    /// <returns></returns>
    int GetWaterNavAreaMask()
    {
        return (0 << 0) + (0 << 1) + (1 << 2) + (1 << 3);
    }

    /// <summary>
    /// 获取普通的陆地行走mask
    /// </summary>
    /// <returns></returns>
    int GetNormalNavAreaMask()
    {
        return (1 << 0) + (0 << 1) + (1 << 2) + (0 << 3);
    }


    public void Init()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _boat = FindObjectOfType<Jyx2Boat>();

        EnableInteractive = true;
    }

    void Start()
    {
        Init();
    }

    public void CanControl(bool isOn)
    {
        canControl = isOn;
    }

    void Update()
    {
        //在船上
        if (IsOnBoat)
        {
            _boat.transform.position = this.transform.position;
            _boat.transform.rotation = this.transform.rotation;
        }

        if (!canControl)
            return;
        
        //判断交互范围
        Debug.DrawRay(transform.position, transform.forward, Color.yellow);
        
        //获得当前可以触发的交互物体
        var gameEvent = DetectInteractiveGameEvent();
        if (gameEvent == null)
        {
            evtManager.OnExitAllEvents();
        }
        else
        {
            //Debug.Log("find interactive trigger:" + gameEvent.name);
            evtManager.OnTriggerEvent(gameEvent);
        }
    }
    #region 事件交互
    
    private Collider[] targets = new Collider[10];
    
    /// <summary>
    /// 在交互视野范围内寻找第一个可被交互物体
    /// </summary>
    /// <returns></returns>
    GameEvent DetectInteractiveGameEvent()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, PLAYER_INTERACTIVE_RANGE, targets, LayerMask.GetMask("GameEvent"));
        //添加
        for (int i = 0; i < count; i++)
        {
            var target = targets[i];

            //判断是否在视野内
            if (Vector3.Angle(transform.forward, target.transform.position - transform.position) <= PLAYER_INTERACTIVE_ANGLE / 2)
            {
                //找到第一个可交互的物体，则结束
                if (SetInteractiveGameEvent(target))
                {
                    return target.GetComponent<GameEvent>();
                }
            }
        }

        return null;
    }

    bool SetInteractiveGameEvent(Collider c)
    {
        var evt = c.GetComponent<GameEvent>();
        if (evt == null)
            return false;

        //有进入触发事件
        if (evt.m_EnterEventId != GameEvent.NO_EVENT)
            return false;

        //没有交互触发事件
        if (evt.m_InteractiveEventId == GameEvent.NO_EVENT && evt.m_UseItemEventId == GameEvent.NO_EVENT)
            return false;


        return true;
    }
    


    #endregion
    //保存世界信息
    public void RecordWorldInfo()
    {
        var runtime = GameRuntimeData.Instance;
        runtime.WorldPosition = UnityTools.Vector3ToString(this.transform.position);
        runtime.BoatWorldPos = UnityTools.Vector3ToString(_boat.transform.position);
        runtime.BoatRotate = UnityTools.QuaternionToString(_boat.transform.rotation);
        runtime.OnBoat = IsOnBoat ? 1 : 0;
    }

    public void LoadWorldInfo()
    {
        var runtime = GameRuntimeData.Instance;
        var pos = UnityTools.StringToVector3(runtime.WorldPosition); //大地图读取当前位置
        PlayerSpawnAt(pos);

        if (!string.IsNullOrEmpty(runtime.BoatWorldPos))
        {
            _boat.transform.position = UnityTools.StringToVector3(runtime.BoatWorldPos);
            _boat.transform.rotation = UnityTools.StringToQuaternion(runtime.BoatRotate);

            if (runtime.OnBoat == 1)
            {
                _boat.GetInBoat();
            }
        }
    }

    void PlayerSpawnAt(Vector3 spawnPos)
    {
        _navMeshAgent.enabled = false;
        Debug.Log("load pos = " + spawnPos);
        transform.position = spawnPos;
        _navMeshAgent.enabled = true;
    }
}