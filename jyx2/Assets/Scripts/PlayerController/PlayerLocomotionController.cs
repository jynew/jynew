using System;
using Cinemachine;
using Jyx2;
using UnityEngine;
using UnityEngine.AI;


public class PlayerLocomotionController:MonoBehaviour
{
	public GameObject navPointerPrefab; //寻路图标prefab
	public float unlockDegree = 10f;
	
	//寻路终点图标
	private GameObject navPointer
	{
		get
		{
			if (_navPointer == null)
				_navPointer = GameObject.Instantiate(navPointerPrefab);
			return _navPointer;
		}
		set { _navPointer = value; }
	}
	private GameObject _navPointer;
	
	public bool isLockingDirection { get; set; }

    public bool forceDisable
    {
        get => _forceDisable;
        set => _forceDisable = value;
    }
    private bool _forceDisable = false;
    
    /// <summary>
    /// 玩家是否拥有角色控制权
    /// </summary>
    /// <returns></returns>
    public bool playerControllable
    {
	    get => _playerControllable;
	    set
	    {
		    _playerControllable = value;
		    gameMapPlayer.SetPlayerControlEnable(_playerControllable);

		    var interactUI = GameObject.FindObjectOfType<InteractUIPanel>();
		    if (interactUI != null)
		    {
			    interactUI.gameObject.SetActive(_playerControllable);
		    }
	    }
    }
    private bool _playerControllable = true;

    private NavMeshAgent _playerNavAgent;
    
    private NavMeshPath _cachePath;
    private float _navDisplayTime;
    private Vector3 _tempDestH = Vector3.zero;
    private Vector3 _tempDestV = Vector3.zero;
    private float _tempH = 0;
    private float _tempV = 0;
    private Action _OnArriveDestination;

    private Jyx2Player gameMapPlayer
    {
	    get
	    {
		    if (_gameMapPlayer == null)
		    {
			    if (LevelMaster.Instance == null)
			    {
				    _gameMapPlayer = null;
			    }
			    else
			    {
				    _gameMapPlayer = LevelMaster.Instance.GetPlayer();
			    }
		    }
		    return _gameMapPlayer;
	    }
    }
    private Jyx2Player _gameMapPlayer;

    public void Init(NavMeshAgent agent)
    {
	    _playerNavAgent = agent;
	    
	    navPointer = GameObject.Instantiate(navPointerPrefab);
	    navPointer.SetActive(false);
    }

	// 放Jyx2Player里OnUpdate会在触发剧情的情况下停止更新，还是先独立出来
	void Update()
    {
        UpdatePlayerControl();
        TryClearNavPointer();
    }
    
    private void UpdatePlayerControl()
    {
        if (BattleManager.Instance.IsInBattle)
            return;

        //timeline不允许角色移动
        if (StoryEngine.Instance != null && StoryEngine.Instance.BlockPlayerControl)
        {
            SetPlayerSpeed(0);
            return;
        }

        if (gameMapPlayer == null)
            return;
		
        if (GameViewPortManager.Instance.GetViewportType() != GameViewPortManager.ViewportType.Follow || LevelMaster.Instance.IsInWorldMap)
        {
            //鼠标点击控制
            OnClickControlPlayer();

            //手动操作
            OnManualControlPlayer();
        }
        else
        {
            //手动操作跟随视角
            OnManualControlPlayerFollowViewport();
        }
    }
    
    /// <summary>
    /// 是否可以标准输入移动
    /// </summary>
    /// <returns></returns>
    private bool IsAxisControlEnable()
    {
        return true;
    }
    
    void OnClickControlPlayer()
	{
		if (_playerNavAgent == null)
			return;
		SetPlayerSpeed(_playerNavAgent.velocity.magnitude);
 
 
		if (!_playerNavAgent.enabled || !_playerNavAgent.isOnNavMesh) return;
 
		//到达目的地了
		if (!_playerNavAgent.pathPending && _playerNavAgent.enabled && !_playerNavAgent.isStopped && _playerNavAgent.remainingDistance <= _playerNavAgent.stoppingDistance)
		{
			_playerNavAgent.isStopped = true;
			if (_OnArriveDestination != null)
			{
				_OnArriveDestination.Invoke();
				_OnArriveDestination = null;
				playerControllable = true;
			}
		}
		if (!LevelMaster.Instance.GetPlayer().CanControlPlayer)
			return;
 
		//在editor上可以寻路
		if (IsClickControlEnable())
		{
			//点击寻路
			if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !UnityTools.IsPointerOverUIObject())
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
 
				//NPC层
				if (Physics.Raycast(ray, out RaycastHit hitInfo, 500, 1 << LayerMask.NameToLayer("NPC")))
				{
					if (GameRuntimeData.Instance.Player.View != null)
					{
						var dist = Vector3.Distance(GameRuntimeData.Instance.Player.View.transform.position, hitInfo.transform.position);
						Debug.Log("on npc clicked, dist = " + dist);
					
						//现在没有直接地图上点击NPC的实现	
					}
				}
				//BY CG: MASK：15:Ground层
				else if (Physics.Raycast(ray, out hitInfo, 500, 1 << LayerMask.NameToLayer("Ground")))
				{
					if (LevelMaster.GetCurrentGameMap().Tags.Contains("NONAVAGENT"))
					{
						var dest = hitInfo.point;
						var sourcePos = _gameMapPlayer.transform.position;
						if (Vector3.Distance(_gameMapPlayer.transform.position, dest) < 0.1f) return;
						_gameMapPlayer.transform.LookAt(new Vector3(dest.x, _gameMapPlayer.transform.position.y, dest.z));
						//设置位移
						_gameMapPlayer.transform.position = Vector3.Lerp(_gameMapPlayer.transform.position, dest, Time.deltaTime);
						//计算当前速度
						var speed = (_gameMapPlayer.transform.position - sourcePos).magnitude / Time.deltaTime;
						_playerNavAgent.updateRotation = true;
						SetPlayerSpeed(speed);
					}
					else
					{
						_playerNavAgent.isStopped = false;
						_playerNavAgent.updateRotation = true;
						if (_playerNavAgent.SetDestination(hitInfo.point))
						{
							if (_cachePath == null)
								_cachePath = new NavMeshPath();
 
							bool isPathValid = _playerNavAgent.CalculatePath(_playerNavAgent.destination, _cachePath);
							if (isPathValid)
								_playerNavAgent.SetPath(_cachePath);
						}
 
					}
 
					DisplayNavPointer(hitInfo.point);
				}
			}
		}
	}
    
    void OnManualControlPlayer()
    {
        if (!playerControllable || _forceDisable)//掉本调用自动寻路的时候 不能手动控制
            return;
		
        if (IsAxisControlEnable() && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            OnManuelMove(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized);
        }
        else if (IsJoystickControlEnable() && (LevelMaster.Instance.m_Joystick.axisX.axisValue != 0 || LevelMaster.Instance.m_Joystick.axisY.axisValue != 0))
        {
            OnManuelMove(new Vector2(-LevelMaster.Instance.m_Joystick.axisX.axisValue, LevelMaster.Instance.m_Joystick.axisY.axisValue).normalized);
        }
        else
        {
            if (!_playerNavAgent.updateRotation)
            {
                SetPlayerSpeed(0);
            }

            //如果被锁方向，在这解锁
            if (isLockingDirection)
            {
                isLockingDirection = false;
            }
        }
    }
    
    void OnManualControlPlayerFollowViewport()
    {
	    if (!playerControllable || _forceDisable)//掉本调用自动寻路的时候 不能手动控制
		    return;

	    _playerNavAgent.updateRotation = false;

	    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
	    {
		    OnManuelMove2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	    }
	    else if (LevelMaster.Instance.m_Joystick.axisX.axisValue != 0 || LevelMaster.Instance.m_Joystick.axisY.axisValue != 0)
	    {
		    OnManuelMove2(-LevelMaster.Instance.m_Joystick.axisX.axisValue, LevelMaster.Instance.m_Joystick.axisY.axisValue);
	    }
	    else
	    {
		    SetPlayerSpeed(0);
	    }

	    if (Input.GetKey(KeyCode.Q))
	    {
		    _gameMapPlayer.transform.RotateAround(_gameMapPlayer.transform.position, Vector3.up, -5);
	    }

	    if (Input.GetKey(KeyCode.E))
	    {
		    _gameMapPlayer.transform.RotateAround(_gameMapPlayer.transform.position, Vector3.up, 5);
	    }

	    //鼠标滑屏
	    if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !UnityTools.IsPointerOverUIObject())
	    {
		    _gameMapPlayer.transform.RotateAround(_gameMapPlayer.transform.position, Vector3.up, 15 * Input.GetAxis("Mouse X"));
	    }

	    //鼠标滚轮
	    if (Input.GetAxis("Mouse ScrollWheel") != 0)
	    {
		    var vcam = GameViewPortManager.Instance.GetFollowVCam();
		    if (vcam != null)
		    {
			    var c = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
			    c.ShoulderOffset = new Vector3(c.ShoulderOffset.x, c.ShoulderOffset.y, c.ShoulderOffset.z + Input.GetAxis("Mouse ScrollWheel") * 10);
		    }
	    }
    }
    
    void OnManuelMove(Vector2 input)
    {
    	float h = input.x;
    	float v = input.y;
    	//Debug.Log($"h={h},v={v}");
        _playerNavAgent.updateRotation = false;

    	Vector3 forward = Vector3.zero;
    	//尝试只使用摄像机的朝向来操作角色移动
    	forward = Camera.main.transform.forward;//m_Player.position - Camera.main.transform.position;
    	forward.y = 0;
    	forward.Normalize();

    	Vector3 right = RotateRound(forward, Vector3.zero, Vector3.up, 90);
    	right.y = 0;
    	right.Normalize();

    	var dest = gameMapPlayer.transform.position + right * h + forward * v;
    	if (_tempDestH == Vector3.zero) _tempDestH = right * h;
    	if (_tempDestV == Vector3.zero) _tempDestV = forward * v;
    	if (isLockingDirection)
    	{
    		dest = gameMapPlayer.transform.position + _tempDestH + _tempDestV;
    		Vector3 cur_dir = new Vector3(h, v, 0).normalized;
    		Vector3 old_dir = new Vector3(_tempH, _tempV, 0).normalized;
    		if (Vector3.Angle(cur_dir, old_dir) > unlockDegree)
    		{
    			isLockingDirection = false;
    		}
    		//Debug.Log("LockingDirection");
    	}
    	else
    	{
    		_tempDestH = right * h;
    		_tempDestV = forward * v;
    		_tempH = h;
    		_tempV = v;
    		//Debug.Log("UnLockingDirection");
    	}
        gameMapPlayer.transform.LookAt(new Vector3(dest.x, gameMapPlayer.transform.position.y, dest.z));
    	var sourcePos = gameMapPlayer.transform.position;
    	var maxSpeed = _playerNavAgent.speed;

    	//设置位移
        gameMapPlayer.transform.position = Vector3.Lerp(gameMapPlayer.transform.position, dest, Time.deltaTime * maxSpeed);

    	//计算当前速度
    	var speed = (gameMapPlayer.transform.position - sourcePos).magnitude / Time.deltaTime;
    	SetPlayerSpeed(speed);

    	if (_playerNavAgent == null || !_playerNavAgent.enabled || !_playerNavAgent.isOnNavMesh) return;
        _playerNavAgent.isStopped = true;
        _playerNavAgent.ResetPath();
    }
    
    void OnManuelMove2(float h, float v)
    {
	    _playerNavAgent.updateRotation = false;

	    Vector3 forward = Vector3.zero;
	    //尝试只使用摄像机的朝向来操作角色移动
	    forward = Camera.main.transform.forward;//m_Player.position - Camera.main.transform.position;
	    forward.y = 0;
	    forward.Normalize();

	    Vector3 right = RotateRound(forward, Vector3.zero, Vector3.up, 90);
	    right.y = 0;
	    right.Normalize();

	    var dest = _gameMapPlayer.transform.position + right * h + forward * v;
	    if (_tempDestH == Vector3.zero) _tempDestH = right * h;
	    if (_tempDestV == Vector3.zero) _tempDestV = forward * v;
	    if (isLockingDirection)
	    {
		    dest = _gameMapPlayer.transform.position + _tempDestH + _tempDestV;
		    Vector3 cur_dir = new Vector3(h, v, 0).normalized;
		    Vector3 old_dir = new Vector3(_tempH, _tempV, 0).normalized;
		    if (Vector3.Angle(cur_dir, old_dir) > unlockDegree)
		    {
			    isLockingDirection = false;
		    }
		    //Debug.Log("LockingDirection");
	    }
	    else
	    {
		    _tempDestH = right * h;
		    _tempDestV = forward * v;
		    _tempH = h;
		    _tempV = v;
		    //Debug.Log("UnLockingDirection");
	    }

	    var sourcePos = _gameMapPlayer.transform.position;
	    var maxSpeed = _playerNavAgent.speed;

	    //设置位移
	    _gameMapPlayer.transform.position = Vector3.Lerp(_gameMapPlayer.transform.position, dest, Time.deltaTime * maxSpeed);

	    //计算当前速度
	    var speed = (_gameMapPlayer.transform.position - sourcePos).magnitude / Time.deltaTime;
	    SetPlayerSpeed(speed);

	    if (_playerNavAgent == null || !_playerNavAgent.enabled || !_playerNavAgent.isOnNavMesh) return;
	    _playerNavAgent.isStopped = true;
	    _playerNavAgent.ResetPath();
    }
    
    public void PlayerWarkFromTo(Vector3 fromVector, Vector3 toVector, Action callback)
    {
	    if (_playerNavAgent == null)
	    {
		    callback?.Invoke();
		    return;
	    }
		if (!NavMesh.SamplePosition(toVector, out NavMeshHit hit, 10, 1 << NavMesh.GetAreaFromName("Walkable")))
		{
			Debug.LogError("Navemesh 采样点失败, 目标点不在导航网格上");
			callback?.Invoke();
			return;
		}
		toVector = hit.position;
		playerControllable = false;
		_OnArriveDestination = callback;
		_playerNavAgent.Warp(fromVector);
		_playerNavAgent.isStopped = false;
		_playerNavAgent.updateRotation = true;
		bool isDestinationReachable = _playerNavAgent.SetDestination(toVector);
		if (!isDestinationReachable)
		{
			Debug.LogError("SetDestination设置自动寻路终点失败，无法到达的目标点");
			_playerNavAgent.isStopped = true;
			_OnArriveDestination = null;
			playerControllable = true;
			_playerNavAgent.updateRotation = false;
			callback?.Invoke();
			return;
		}
	}

    private void SetPlayerSpeed(float speed)
    {
        if (gameMapPlayer == null)
            return;

        var animator = gameMapPlayer.m_Animator;
        if (animator != null)
        {
            animator.SetFloat( Animator.StringToHash("speed"), Math.Min(speed, 20));
        }
    }
    
    public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
    {
	    Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
	    Vector3 resultVec3 = center + point;
	    return resultVec3;
    }
    
    // implement change player facing. 0:top-right, 1:down-right, 2:top-left, 3:down-left
    // modify by eaphone at 2021/6/5
    public void SetRotation(int ro)
    {
	    int[] roationSet = { -90, 0, 180, 90 };
	    _gameMapPlayer.transform.rotation = Quaternion.Euler(Vector3.up * roationSet[ro]);
    }
    
    //主角停止导航，停留在原地
    public void StopPlayerNavigation()
    {
	    if (_playerNavAgent == null || !_playerNavAgent.enabled || !_playerNavAgent.isOnNavMesh) return;

	    _playerNavAgent.isStopped = true;

    }
    
    private void DisplayNavPointer(Vector3 pos)
    {
	    navPointer.transform.position = pos + new Vector3(0, 0.2f, 0);
	    _navDisplayTime = Time.time;
	    navPointer.SetActive(true);
    }
    
    void TryClearNavPointer()
    {
	    if (navPointer.activeSelf && Time.time - _navDisplayTime > 1)
	    {
		    navPointer.SetActive(false);
	    }
    }
    
    /// <summary>
    /// 是否可以点击移动
    /// </summary>
    /// <returns></returns>
    public bool IsClickControlEnable()
    {
	    if (!LevelMaster.Instance.IsMobilePlatform()) return true;
	    if (IsMobileClickControl()) return true;
	    return false;
    }
    
    private bool IsMobileClickControl()
    {
	    return LevelMaster.Instance.IsMobilePlatform() && GameSettingManager.MobileMoveMode == GameSettingManager.MobileMoveModeType.Click;
    }
    
    /// <summary>
    /// 是否可以虚拟摇杆移动
    /// </summary>
    /// <returns></returns>
    public bool IsJoystickControlEnable()
    {
	    return LevelMaster.Instance.IsMobilePlatform() && !IsMobileClickControl();
    }
}
