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
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Animancer;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using XLua;

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

    private bool canControl = true;

    public static Jyx2Player GetPlayer()
    {
        if (LevelMaster.Instance == null)
            return null;
        
        return LevelMaster.Instance.GetPlayer();
    }

    public async UniTask OnSceneLoad()
    {
        //transform.rotation = Quaternion.Euler(Vector3.zero);
        
        //fix bug:无法正确触发开场的剧情，似乎异步加载scene的时候，触发器碰撞没有被激活
        var c = GetComponent<Collider>();
        await UniTask.WaitForEndOfFrame();
        c.enabled = false;
        await UniTask.WaitForEndOfFrame();
        c.enabled = true;
    }

    public HybridAnimancerComponent m_Animancer;
    public Animator m_Animator;

    
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
		_navMeshAgent.Warp(boat.transform.position);
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

        _gameEventLayerMask = LayerMask.GetMask("GameEvent");
    }

    void Start()
    {
        Init();
        
        //修复一些场景里有主角贴图丢失导致紫色的情况
        if (GetComponent<SkinnedMeshRenderer>() != null)
        {
            GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
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

        //BigMapIdleJudge();
        
        //判断交互范围
        Debug.DrawRay(transform.position, transform.forward, Color.yellow);

        if (evtManager != null)
        {
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
    }


    private float _bigmapIdleTimeCount = 0;
    private const float BIG_MAP_IDLE_TIME = 5f;
    private bool _playingbigMapIdle = false;

    
    private HybridAnimancerComponent GetPlayerAnimancer()
    {
        return m_Animancer;
    }
    
    //在大地图上判断是否需要展示待机动作
    void BigMapIdleJudge()
    {
        if(_boat == null) return; //暂实现：判断是否是大地图，有船才是大地图

        var animator = m_Animator;
        
        if (_playingbigMapIdle)
        {
            //判断是否有移动速度，有的话立刻打断目前IDLE动作
            if (animator!=null && animator.GetFloat("speed") > 0)
            {
                var animancer = GetPlayerAnimancer();
                animancer.Stop();
                animancer.PlayController();
                _playingbigMapIdle = false;
            }
            return;
        }

        //一旦开始移动，则重新计时
        if (animator!=null && animator.GetFloat("speed") > 0)
        {
            _bigmapIdleTimeCount = 0;
            return;
        }
        
        _bigmapIdleTimeCount += Time.deltaTime;
        if (_bigmapIdleTimeCount > BIG_MAP_IDLE_TIME)
        {
            //展示IDLE动作
            _bigmapIdleTimeCount = 0;
            var animancer = GetPlayerAnimancer();
            var clip = Jyx2.Middleware.Tools.GetRandomElement(GlobalAssetConfig.Instance.bigMapIdleClips);
            animancer.Play(clip, 0.25f);
            _playingbigMapIdle = true;
        }
    }
    
    #region 事件交互
    
    private Collider[] targets = new Collider[10];

    private int _gameEventLayerMask = -1;
    
    /// <summary>
    /// 在交互视野范围内寻找第一个可被交互物体
    /// </summary>
    /// <returns></returns>
    GameEvent DetectInteractiveGameEvent()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, PLAYER_INTERACTIVE_RANGE, targets, _gameEventLayerMask);
        //添加
        for (int i = 0; i < count; i++)
        {
            var target = targets[i];

            if (CanSee(target) && SetInteractiveGameEvent(target))
            {
                //找到第一个可交互的物体，则结束
                return target.GetComponent<GameEvent>();
            }
        }

        return null;
    }

    bool CanSee(Collider target)
    {
        //判断是否在视野角度内
        var isInViewField = Vector3.Angle(transform.forward, target.transform.position - transform.position) <= PLAYER_INTERACTIVE_ANGLE / 2;
        if(isInViewField) {

            var targetDirection = (target.transform.position - transform.position).normalized;

            //判断主角的NavMesh Agent是否在目标trigger范围內
            var isInTrigger = target.bounds.Contains(transform.position + targetDirection * _navMeshAgent.radius);
            if(isInTrigger) 
            {
                Debug.DrawLine(transform.position, target.transform.position, Color.green);
                //Debug.Log("Inside trigger: " + target.transform.name);

                return true;
            }
            else
            {
                //判断主角与目标之间有无其他collider遮挡。忽略trigger。
                RaycastHit hit;
                if(Physics.Raycast(transform.position, targetDirection, out hit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                    //Debug.Log("Hit. hit: " + hit.transform.name + " target:" + target.transform.name);
                    if(hit.transform.GetInstanceID() != target.transform.GetInstanceID())
                    {
                        Debug.DrawLine(transform.position, hit.point, Color.red);
                        return false;
                    }
                }
                //Debug.Log("Outside trigger: " + target.transform.name);
                Debug.DrawLine(transform.position, target.transform.position, Color.green);
                return true;

            }
        }


        return false;
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

        if (runtime.WorldData == null)
        {
            runtime.WorldData = new WorldMapSaveData();
        }

        WorldMapSaveData worldData = runtime.WorldData;
        worldData.WorldPosition = this.transform.position;
        worldData.WorldRotation = this.transform.rotation;
        worldData.BoatWorldPos = _boat.transform.position;
        worldData.BoatRotate = _boat.transform.rotation;
        worldData.OnBoat = IsOnBoat ? 1 : 0;
    }

    public void LoadWorldInfo()
    {
        var runtime = GameRuntimeData.Instance;
        if (runtime.WorldData == null) return;
        
        PlayerSpawnAt(runtime.WorldData.WorldPosition, runtime.WorldData.WorldRotation);

        LoadBoat();

        if (runtime.WorldData.OnBoat == 1)
        {
            _boat.GetInBoat();
        }
    }

    public void LoadBoat()
    {
        var runtime = GameRuntimeData.Instance;
        if (runtime.WorldData == null)
            return; //首次进入
        
        _boat.transform.position = runtime.WorldData.BoatWorldPos;
        _boat.transform.rotation = runtime.WorldData.BoatRotate;
    }

    public Vector3 GetBoatPosition()
    {
        return _boat == null ? new Vector3() : _boat.transform.position;
    }

    void PlayerSpawnAt(Vector3 spawnPos,Quaternion ori)
    {
        _navMeshAgent.enabled = false;
        Debug.Log("load pos = " + spawnPos);
        transform.position = spawnPos;
		transform.rotation = ori;
        _navMeshAgent.enabled = true;
    }
}