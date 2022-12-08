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
using Animancer;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Jyx2.InputCore;


[RequireComponent(typeof(Jyx2_PlayerInput))]
[RequireComponent(typeof(Jyx2_PlayerMovement))]
[RequireComponent(typeof(Jyx2_PlayerAutoWalk))]
[DisallowMultipleComponent]
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

    public Animator m_Animator;

    public bool IsOnBoat;

    NavMeshAgent _navMeshAgent;
    Jyx2_PlayerAutoWalk _autoWalker;
    Jyx2_PlayerMovement m_PlayerMovement;
    Jyx2Boat _boat;

    private float m_InteractDelayTime;

    private bool m_IsInTimeline = false;

    public bool IsInTimeline
    {
        get => m_IsInTimeline;
        set => m_IsInTimeline = value;
    }

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
        if (NavMesh.SamplePosition(transform.position, out myNavHit, 2.5f, GetNormalNavAreaMask()))
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

    public void GetInSecret()
    {
        _navMeshAgent.areaMask = GetSecretNavAreaMask();
    }

    public void GetOutSecret()
    {
        _navMeshAgent.areaMask = GetNormalNavAreaMask();
    }

    /// <summary>
    /// 获取水路行走mask
    /// </summary>
    /// <returns></returns>
    int GetWaterNavAreaMask()
    {
        return (0 << 0) + (0 << 1) + (1 << 2) + (1 << 3) + (0 << 4);
    }

    /// <summary>
    /// 获取普通的陆地行走mask
    /// </summary>
    /// <returns></returns>
    int GetNormalNavAreaMask()
    {
        return (1 << 0) + (0 << 1) + (1 << 2) + (0 << 3) + (0 << 4);
    }

    // 获取隐秘区域行走mask
    int GetSecretNavAreaMask()
    {
        return (1 << 0) + (0 << 1) + (1 << 2) + (0 << 3) + (1 << 4);
    }

    public void Init()
    {
        m_PlayerMovement = GetComponent<Jyx2_PlayerMovement>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _boat = FindObjectOfType<Jyx2Boat>();
        _autoWalker = GetComponent<Jyx2_PlayerAutoWalk>();
        _gameEventLayerMask = LayerMask.GetMask("GameEvent");
        m_InteractDelayTime = Time.time + 0.1f;
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

    public bool CanControlPlayer
    {
        get
        {
            if (!Jyx2_Input.IsPlayerContext)
                return false;
            if (m_IsInTimeline)
                return false;
            if (_autoWalker.IsAutoWalking)
                return false;
            if (Jyx2_UIManager.Instance.IsUIOpen(nameof(GameOver)))
                return false;
            if (StoryEngine.BlockPlayerControl)
                return false;
            if (LevelMaster.Instance != null && LevelMaster.Instance.IsFadingScene)
                return false;
            return true;
        }
    }


    void Update()
    {
        //在船上
        if (IsOnBoat)
        {
            _boat.transform.position = this.transform.position;
            _boat.transform.rotation = this.transform.rotation;
        }

        if (!CanControlPlayer)
            return;
		
	    //尝试解决战斗场景中出现交互按钮导致游戏卡死的问题
        if (LevelMaster.IsInBattle)
	        return;

        //延迟下交互触发 不然加载后的第一帧 交互和对话会同时触发
        if (m_InteractDelayTime >= Time.time)
            return;
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

            if (CanSee(target) && TryGetInteractiveGameEvent(target, out GameEvent evt))
            {
                //找到第一个可交互的物体，则结束
                return evt;
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
#if UNITY_EDITOR
                Debug.DrawLine(transform.position, target.transform.position, Color.green);
                //Debug.Log("Inside trigger: " + target.transform.name);
#endif
                return true;
            }
            else
            {
                //判断主角与目标之间有无其他collider遮挡。忽略trigger。
                RaycastHit hit;
                if(Physics.Raycast(transform.position, targetDirection, out hit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore)) 
                {
                    //Debug.Log("Hit. hit: " + hit.transform.name + " target:" + target.transform.name);
                    if(hit.transform.GetInstanceID() != target.transform.GetInstanceID())
                    {
#if UNITY_EDITOR
                        Debug.DrawLine(transform.position, hit.point, Color.red);
#endif
                        return false;
                    }
                }
#if UNITY_EDITOR
                //Debug.Log("Outside trigger: " + target.transform.name);
                Debug.DrawLine(transform.position, target.transform.position, Color.green);
#endif
                return true;

            }
        }


        return false;
    }

    bool TryGetInteractiveGameEvent(Collider collider, out GameEvent evt)
    {
        evt = collider.GetComponent<GameEvent>(); 
        if (evt == null)
            return false;

        //有进入触发事件
        if (evt.IsTriggerEnterEvent)
            return false;

        //没有交互触发事件
        if (!evt.IsInteractiveOrUseItemEvent)
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
        if (_boat == null) return;
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
        if (_boat == null) return;
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

    public void StopPlayerMovement()
    {
        m_PlayerMovement?.StopMovement();
    }
}
