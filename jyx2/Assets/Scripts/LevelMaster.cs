/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using UnityEngine;
using UnityEngine.AI;
using Jyx2;
using System;
using System.Collections;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Jyx2Configs;
using Application = UnityEngine.Application;
using UnityEngine.UI;

public class LevelMaster : MonoBehaviour
{

	//载入参数
	public class LevelLoadPara
	{
		public enum LevelLoadType
		{
			Load, //读档
			Entrance, //从外部进入
			StartAtTrigger, //从指定Trigger开始
			ReturnFromBattle,
		}

		public LevelLoadType loadType = LevelLoadType.Entrance;
		public string triggerName = "";
		public Vector3 Pos;
		public Quaternion Rotate;
	}

	public static LevelLoadPara loadPara = new LevelLoadPara();

	public static LevelMaster Instance
	{
		get
		{
			if (_instance == null) _instance = FindObjectOfType<LevelMaster>();
			return _instance;
		}
	}
	private static LevelMaster _instance;

	public bool MobileSimulate = false;
	public GameObject m_MobileRotateSlider;
	public bl_HUDText HUDRoot;
	public GameObject navPointerPrefab; //寻路图标prefab
	public ETCTouchPad m_TouchPad;
	public ETCJoystick m_Joystick;

	private Jyx2Player _gameMapPlayer;
	
	NavMeshAgent _playerNavAgent;
	NavMeshPath _cachePath;
	GameObject _navPointer;

	//寻路终点图标
	GameObject navPointer
	{
		get
		{
			var result = _navPointer;
			if (result == null)
				result = Instantiate(navPointerPrefab);
			return result;
		}
		set { _navPointer = value; }
	}

	CameraHelper m_CameraHelper;

	public static Jyx2ConfigMap LastGameMap = null; //前一个地图
	private static Jyx2ConfigMap _currentMap;

	public static void SetCurrentMap(Jyx2ConfigMap map)
	{
		_currentMap = map;
	}

	[HideInInspector]
	public bool IsInited = false;

	[Header("Lock Direction")]
	public float unlockDegee = 10f;

	//BattleHelper m_BattleHelper;

	bool IsMobilePlatform()
	{
		return MobileSimulate || Application.isMobilePlatform;
	}

	GameRuntimeData runtime
	{
		get { return GameRuntimeData.Instance; }
	}

	/// <summary>
	/// 获取当前所在地图
	/// </summary>
	/// <returns></returns>
	public static Jyx2ConfigMap GetCurrentGameMap()
	{
		return _currentMap;
	}

	/// <summary>
	/// 当前是否在战斗中
	/// </summary>
	public static bool IsInBattle = false;

	/// <summary>
	/// 当前是否在大地图，统一判断方式
	/// </summary>
	public bool IsInWorldMap
	{
		get { return _currentMap?.Tags.Contains("WORLDMAP") ?? false; }
	}

	// Use this for initialization
	void Start()
	{
		//先关闭触发事件
		GameObject triggers = GameObject.Find("Level/Triggers");
		if (triggers != null)
		{
			foreach (Transform trigger in triggers.transform)
			{
				trigger.gameObject.layer = LayerMask.NameToLayer("GameEvent");
				var c = trigger.gameObject.GetComponent<Collider>();
				if (c != null)
				{
					c.enabled = false;
				}
			}
		}

		if (Camera.main != null)
		{
			var brain = Camera.main.GetComponent<CinemachineBrain>();
			if (brain != null)
			{
				brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
			}
		}

		var gameMap = GetCurrentGameMap();
		if (gameMap != null && !IsInBattle)
		{
			if (gameMap.Tags.Contains("WORLDMAP"))//JYX2 临时测试
			{
				var btn = transform.Find("UI/MainUI/BackButton");
				if (btn != null)
				{
					btn.gameObject.SetActive(false);
				}
			}

			//播放音乐
			PlayMusic(gameMap);
		}

		navPointer = Instantiate(navPointerPrefab);
		navPointer.SetActive(false);

		//刷新界面控制器
		UpdateMobileControllerUI();

		//尝试绑定主角
		TryBindPlayer().Forget();
		
		//大地图不能使用跟随相机（目前好像比较卡？）
		if (gameMap != null && !gameMap.Tags.Contains("WORLDMAP") && _gameMapPlayer != null)
		{
			//初始化跟随相机
			GameViewPortManager.Instance.InitForLevel(_gameMapPlayer.transform);
		}
		
		//刷新游戏事件
		RefreshGameEvents();

		//全部初始化完以后，激活trigger的触发 
		triggers = GameObject.Find("Level/Triggers");
		if (triggers != null)
		{
			foreach (Transform trigger in triggers.transform)
			{
				var c = trigger.gameObject.GetComponent<Collider>();
				if (c != null)
					c.enabled = true;
			}
		}

		//修复所有没有绑定controller的角色
		foreach (var animator in FindObjectsOfType<Animator>())
		{
			if (animator.runtimeAnimatorController != null) continue;
			if (animator.transform.parent.name == "NPC")
			{
				animator.runtimeAnimatorController = GlobalAssetConfig.Instance.defaultNPCAnimatorController;
			}
		}

		if (gameMap != null && !gameMap.Tags.Contains("WORLDMAP"))
		{
			//调整摄像机参数
			UpdateCameraParams();

			if (!IsInBattle)
			{
				//显示当前地图名，大地图不用显示
				Jyx2_UIManager.Instance.ShowUIAsync(nameof(CommonTipsUIPanel), TipsType.MiddleTop, gameMap.GetShowName()).Forget();
			}
		}

		interactiveButton = Jyx2InteractiveButton.GetInteractiveButton();

		IsInited = true;
		
	}

	public void UpdateCameraParams()
	{
		//世界地图取默认场景的设置
		if (_currentMap.Tags.Contains("WORLDMAP"))
			return;
		
		//调整摄像机参数
		var vcamObj = GameObject.Find("CameraGroup/CM vcam1");

		if (vcamObj != null)
		{
			var vcam = vcamObj.GetComponent<CinemachineVirtualCamera>();
			var body = vcam.GetCinemachineComponent<CinemachineTransposer>();

			var viewPortType = GameViewPortManager.Instance.GetViewportType();

			//高度
			if (viewPortType == GameViewPortManager.ViewportType.Topdown)
			{
				body.m_FollowOffset = GlobalAssetConfig.Instance.defaultVcamOffset;	
			}
			else if(viewPortType == GameViewPortManager.ViewportType.TopdownClose)
			{
				body.m_FollowOffset = GlobalAssetConfig.Instance.vcamOffsetClose;
			}


			if (_gameMapPlayer != null)
			{
				//跟随对象
				vcam.Follow = _gameMapPlayer.transform;	
			}
		}
	}

	private void PlayMusic(Jyx2ConfigMap currentMap)
	{
		if (currentMap == null) return;
		
		//有上一张图的出门音乐就放该音乐
		if (LastGameMap != null)
		{
			if (LastGameMap.ForceSetLeaveMusicId != -1)
			{
				AudioManager.PlayMusic(LastGameMap.ForceSetLeaveMusicId);
				return;
			}
			if (LastGameMap.OutMusic != -1)
			{
				AudioManager.PlayMusic(LastGameMap.OutMusic);
				return;
			}
		}
		//没有就放进门的
		AudioManager.PlayMusic(currentMap.InMusic);

	}

	public void PlayMusicAtPath(string musicPath)
	{
		AudioManager.PlayMusicAtPath(musicPath).Forget();
	}

	public void UpdateMobileControllerUI()
	{
		if (BattleManager.Instance.IsInBattle)
		{
			m_Joystick.gameObject.SetActive(false);
		}
		else
		{
			m_Joystick.gameObject.SetActive(IsJoystickControlEnable());
		}
		
		m_TouchPad.gameObject.SetActive(BattleManager.Instance.IsInBattle && IsMobilePlatform()); //移动平台显示战斗旋转
	}

	void LoadSpawnPosition()
	{
		if (runtime == null || _gameMapPlayer == null)
			return;

		var map = GetCurrentGameMap();
		if (map == null)
			return;

		if (loadPara.loadType == LevelLoadPara.LevelLoadType.Load)
		{
			if (map.Tags.Contains("WORLDMAP"))
			{
				GetPlayer().LoadWorldInfo();
			}
			else
			{
				PlayerSpawnAt(loadPara.Pos);
				PlayerSpawnRotate(loadPara.Rotate);
			}
		}
		else if (loadPara.loadType == LevelLoadPara.LevelLoadType.Entrance)
		{
			if (map.Tags.Contains("WORLDMAP")) //大地图
			{
				GetPlayer().LoadWorldInfo();
			}
			else
			{
				var entranceObj = GameObject.FindGameObjectWithTag("Entrance"); //找入口
				if (entranceObj != null)
				{
					PlayerSpawnAt(entranceObj.transform.position);
				}
			}
		}
		else if (loadPara.loadType == LevelLoadPara.LevelLoadType.StartAtTrigger)
		{
			Transport(loadPara.triggerName);

			if (_currentMap.Tags.Contains("WORLDMAP"))
				GetPlayer().LoadBoat();
		}
		else if (loadPara.loadType == LevelLoadPara.LevelLoadType.ReturnFromBattle)
		{
			//从战斗回来的，先不能触发对话逻辑
			SetPlayerCanController(false);
			StopPlayerNavigation();

			PlayerSpawnAt(loadPara.Pos);
			PlayerSpawnRotate(loadPara.Rotate);
		}
	}

	void PlayerSpawnAt(Vector3 spawnPos)
	{
		_playerNavAgent.enabled = false;
		Debug.Log("load pos = " + spawnPos);
		_gameMapPlayer.transform.position = spawnPos;
		_playerNavAgent.enabled = true;
	}
	void PlayerSpawnRotate(Quaternion ori)
	{
		_playerNavAgent.enabled = false;
		Debug.Log("load ori = " + ori);
		_gameMapPlayer.transform.rotation = ori;
		_playerNavAgent.enabled = true;
	}


	private void SetPlayerSpeed(float speed)
	{
		if (_gameMapPlayer == null)
			return;

		var animator = _gameMapPlayer.m_Animator;
		if (animator != null)
		{
			animator.SetFloat("speed", Math.Min(speed, 20));
		}
	}

	private async UniTask SetPlayer(Jyx2Player playerRoleView)
	{
		_playerNavAgent = playerRoleView.GetComponent<NavMeshAgent>();

		SetPlayerSpeed(0);
		var gameMap = GetCurrentGameMap();
		if (gameMap != null && gameMap.Tags.Contains("WORLDMAP"))
		{
			_playerNavAgent.speed = GlobalAssetConfig.Instance.playerMoveSpeedWorldMap;
		}
		else
		{
			_playerNavAgent.speed = GlobalAssetConfig.Instance.playerMoveSpeed;
		}

		_playerNavAgent.angularSpeed = GameConst.MapAngularSpeed;
		_playerNavAgent.acceleration = GameConst.MapAcceleration;
		_playerNavAgent.autoBraking = false;
		_playerNavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;


		playerRoleView.Init();
		LoadSpawnPosition();
	}

	// fix bind player failed error when select player before start battle
	// modified by eaphone at 2021/05/31
	public async UniTask TryBindPlayer()
	{
		if (_gameMapPlayer != null)
			return;

		_gameMapPlayer = RoleHelper.FindPlayer();

		if (_gameMapPlayer != null)
		{
			//设置主角
			await SetPlayer(_gameMapPlayer);

			var gameMap = GetCurrentGameMap();
			if (gameMap != null && gameMap.Tags.Contains("POINTLIGHT")) //点光源
			{
				var obj = Jyx2ResourceHelper.CreatePrefabInstance(ConStr.PlayerPointLight);
				obj.transform.SetParent(_gameMapPlayer.transform);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
			}
		}
	}

	public void SwitchToBattleUI(bool isOn)
	{
		GameObject.Find("LevelMaster/UI/MainUI").SetActive(!isOn);
		GameObject.Find("LevelMaster/UI/SystemButton").SetActive(!isOn);
		GameObject.Find("LevelMaster/UI/PlayerStatusPanel").SetActive(!isOn);
	}

	void Update()
	{
		TryClearNavPointer();
		PlayerControl();

		GamePadUpdate();
	}

	void PlayerControl()
	{
		if (BattleManager.Instance.IsInBattle)
			return;

		//timeline不允许角色移动
		if (StoryEngine.Instance != null && StoryEngine.Instance.BlockPlayerControl)
		{
			SetPlayerSpeed(0);
			return;
		}

		if (_gameMapPlayer == null)
			return;
		
		if (GameViewPortManager.Instance.GetViewportType() != GameViewPortManager.ViewportType.Follow || IsInWorldMap)
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

	bool _CanController = true;
	public void SetPlayerCanController(bool CanController)
	{
		_CanController = CanController;
		var player = GetPlayer();
		player.CanControl(CanController);

		var interactUI = FindObjectOfType<InteractUIPanel>();
		if (interactUI != null)
		{
			interactUI.gameObject.SetActive(CanController);
		}
	}

	/// <summary>
	/// 玩家是否拥有角色控制权
	/// </summary>
	/// <returns></returns>
	public bool IsPlayerCanControl()
	{
		return _CanController;
	}

	private Action _OnArriveDestination;
	public void PlayerWarkFromTo(Vector3 fromVector, Vector3 toVector, Action callback)
	{
		if (_playerNavAgent == null)
		{
			callback?.Invoke();
			return;
		}
		SetPlayerCanController(false);
		_OnArriveDestination = callback;
		_playerNavAgent.Warp(fromVector);
		_playerNavAgent.isStopped = false;
		_playerNavAgent.updateRotation = true;
		
		//寻找最近的点
		NavMeshHit hit;
		if (NavMesh.SamplePosition(toVector, out hit, 10, 1 << LayerMask.NameToLayer("Ground")))
		{
			toVector = hit.position;
		}
		
		_playerNavAgent.SetDestination(toVector);
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
				SetPlayerCanController(true);
			}
		}
		if (!_CanController)
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
					if (runtime.Player.View != null)
					{
						var dist = Vector3.Distance(runtime.Player.View.transform.position, hitInfo.transform.position);
						Debug.Log("on npc clicked, dist = " + dist);
					
						//现在没有直接地图上点击NPC的实现	
					}
				}
				//BY CG: MASK：15:Ground层
				else if (Physics.Raycast(ray, out hitInfo, 500, 1 << LayerMask.NameToLayer("Ground")))
				{
					if (_currentMap.Tags.Contains("NONAVAGENT"))
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

	/// <summary>
	/// 是否可以点击移动
	/// </summary>
	/// <returns></returns>
	private bool IsClickControlEnable()
	{
		if (!IsMobilePlatform()) return true;
		if (IsMobileClickControl()) return true;
		return false;
	}

	/// <summary>
	/// 是否可以虚拟摇杆移动
	/// </summary>
	/// <returns></returns>
	private bool IsJoystickControlEnable()
	{
		return IsMobilePlatform() && !IsMobileClickControl();
	}

	/// <summary>
	/// 是否可以标准输入移动
	/// </summary>
	/// <returns></returns>
	private bool IsAxisControlEnable()
	{
		return true;
	}

	private bool IsMobileClickControl()
	{
		return IsMobilePlatform() && GameSettingManager.MobileMoveMode == GameSettingManager.MobileMoveModeType.Click;
	}

	public void ForceSetEnable(bool forceDisable)
	{
		_forceDisable = forceDisable;
	}

	private bool _forceDisable = false;

	void OnManualControlPlayer()
	{
		if (!_CanController || _forceDisable)//掉本调用自动寻路的时候 不能手动控制
			return;
		
		if (IsAxisControlEnable() && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
		{
			OnManuelMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		}
		else if (IsJoystickControlEnable() && (m_Joystick.axisX.axisValue != 0 || m_Joystick.axisY.axisValue != 0))
		{
			OnManuelMove(-m_Joystick.axisX.axisValue, m_Joystick.axisY.axisValue);
		}
		else
		{
			if (!_playerNavAgent.updateRotation)
			{
				SetPlayerSpeed(0);
			}

			//如果被锁方向，在这解锁
			if (m_IsLockingDirection)
			{
				m_IsLockingDirection = false;
			}
		}
	}


	//手动控制跟随相机
	void OnManualControlPlayerFollowViewport()
	{
		if (!_CanController || _forceDisable)//掉本调用自动寻路的时候 不能手动控制
			return;

		_playerNavAgent.updateRotation = false;

		if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
		{
			OnManuelMove2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		}
		else if (m_Joystick.axisX.axisValue != 0 || m_Joystick.axisY.axisValue != 0)
		{
			OnManuelMove2(-m_Joystick.axisX.axisValue, m_Joystick.axisY.axisValue);
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
		if (m_IsLockingDirection)
		{
			dest = _gameMapPlayer.transform.position + _tempDestH + _tempDestV;
			Vector3 cur_dir = new Vector3(h, v, 0).normalized;
			Vector3 old_dir = new Vector3(_tempH, _tempV, 0).normalized;
			if (Vector3.Angle(cur_dir, old_dir) > unlockDegee)
			{
				m_IsLockingDirection = false;
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

	public bool m_IsLockingDirection = false;
	private Vector3 _tempDestH = Vector3.zero;
	private Vector3 _tempDestV = Vector3.zero;
	private float _tempH = 0;
	private float _tempV = 0;
	void OnManuelMove(float h, float v)
	{
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

		var dest = _gameMapPlayer.transform.position + right * h + forward * v;
		if (_tempDestH == Vector3.zero) _tempDestH = right * h;
		if (_tempDestV == Vector3.zero) _tempDestV = forward * v;
		if (m_IsLockingDirection)
		{
			dest = _gameMapPlayer.transform.position + _tempDestH + _tempDestV;
			Vector3 cur_dir = new Vector3(h, v, 0).normalized;
			Vector3 old_dir = new Vector3(_tempH, _tempV, 0).normalized;
			if (Vector3.Angle(cur_dir, old_dir) > unlockDegee)
			{
				m_IsLockingDirection = false;
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
		_gameMapPlayer.transform.LookAt(new Vector3(dest.x, _gameMapPlayer.transform.position.y, dest.z));
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
	private Vector3 _tempCameraPosition;

	public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
	{
		Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
		Vector3 resultVec3 = center + point;
		return resultVec3;
	}

	//主角停止导航，停留在原地
	public void StopPlayerNavigation()
	{
		if (_playerNavAgent == null || !_playerNavAgent.enabled || !_playerNavAgent.isOnNavMesh) return;

		_playerNavAgent.isStopped = true;

	}

	#region 导航标志
	float _navDisplayTime;
	private bool gamepadConnected;
	private Button interactiveButton;

	//显示导航标志
	void DisplayNavPointer(Vector3 pos)
	{
		navPointer.transform.position = pos + new Vector3(0, 0.2f, 0);
		_navDisplayTime = Time.time;
		navPointer.SetActive(true);
	}

	//隐藏导航标志
	void TryClearNavPointer()
	{
		if (navPointer.activeSelf && Time.time - _navDisplayTime > 1)
		{
			navPointer.SetActive(false);
		}
	}
	#endregion


	//传送
	public void Transport(string transportName)
	{
		_playerNavAgent.enabled = false;
		TransportToTransform("Level/Triggers", transportName, "");
		_playerNavAgent.enabled = true;
	}

	public void TransportToTransform(string path, string name, string target)
	{
		var rootObj = GameObject.Find(path);
		var trans = rootObj.transform.Find(name);

		if (trans == null)
		{
			rootObj = GameObject.Find("Level/Dynamic");
			trans = rootObj.transform.Find(name);
		}
		if (trans != null)
		{
			if (target == "")
			{
				Transport(trans.position);
				//增加传送时设置朝向。rotation为0时不作调整，需要朝向0时候，可以使用360.
				if (trans.rotation != Quaternion.identity)
				{
					_gameMapPlayer.transform.rotation = trans.rotation;
				}
			}
			else
			{
				var t = GameObject.Find(target).transform;
				t.position = trans.position;
			}
		}
		else
		{
			Debug.LogError("找不到传送点：" + name);
		}
	}

	//传送
	public void Transport(Vector3 position)
	{
		_playerNavAgent.Warp(position);
		_gameMapPlayer.transform.position = position;
	}

	// implement change player facing. 0:top-right, 1:down-right, 2:top-left, 3:down-left
	// modify by eaphone at 2021/6/5
	public void SetRotation(int ro)
	{
		int[] roationSet = { -90, 0, 180, 90 };
		_gameMapPlayer.transform.rotation = Quaternion.Euler(Vector3.up * roationSet[ro]);
	}

	//手动存档
	public void OnManuelSave(int index = -1)
	{
		if (runtime == null)
		{
			StoryEngine.Instance.DisplayPopInfo("<color=red>存档失败！</color>");
			Debug.LogError("存档失败！请从GameStart中启动游戏！");
			return;
		}

		if (_currentMap.Tags.Contains("WORLDMAP"))
		{
			GetPlayer().RecordWorldInfo();
		}

		runtime.SubMapData = new SubMapSaveData(GetCurrentGameMap().Id);
		runtime.SubMapData.CurrentPos = _gameMapPlayer.transform.position;
		runtime.SubMapData.CurrentOri = _gameMapPlayer.transform.rotation;


		runtime.GameSave(index);
		StoryEngine.Instance.DisplayPopInfo("存档成功！");
	}

	public Vector3 GetPlayerPosition()
	{
		return _gameMapPlayer.transform.position;
	}
	public Quaternion GetPlayerOrientation()
	{
		return _gameMapPlayer.transform.rotation;
	}
	
	public Jyx2Player GetPlayer()
	{
		return _gameMapPlayer;
	}

	//刷新本场景内的所有事件
	//事件执行和更改结果存储在runtime里，需要结合当前场景进行调整
	public void RefreshGameEvents()
	{
		var gameMap = GetCurrentGameMap();
		if (gameMap == null) return;

		//调整所有的触发器
		GameObject eventsParent = GameObject.Find("Level/Triggers");
		if (eventsParent == null) return;
		foreach (Transform obj in eventsParent.transform)
		{
			var evt = obj.GetComponent<GameEvent>();
			if (evt == null) continue;
			string eventId = obj.name;

			try
			{
				string modify = runtime.GetModifiedEvent(gameMap.Id, int.Parse(eventId));
				if (!string.IsNullOrEmpty(modify))
				{
					string[] tmp = modify.Split('_');
					evt.m_InteractiveEventId = int.Parse(tmp[0]);
					evt.m_UseItemEventId = int.Parse(tmp[1]);
					evt.m_EnterEventId = int.Parse(tmp[2]);
				}

				evt.Init();
			}
			catch (Exception e)
			{
				Debug.LogError("事件解析错误,事件ID必须是数字,eventId=" + eventId);
			}
		}
	}

	private void GamePadUpdate()
	{
		Button button = Jyx2InteractiveButton.GetInteractiveButton();

		if (GamepadHelper.GamepadConnected != gamepadConnected)
		{
			gamepadConnected = GamepadHelper.GamepadConnected;

			Transform trans = button?.gameObject.transform;
			if (trans != null)
			{
				var image = trans.GetChild(2).GetComponentInChildren<Image>();
				image.gameObject.SetActive(gamepadConnected);
			}
		}

		if (gamepadConnected)
		{
			if (GamepadHelper.IsConfirm() && button != null && button.gameObject.activeSelf)
			{
				button.onClick?.Invoke();
			}
		}
	}
}

