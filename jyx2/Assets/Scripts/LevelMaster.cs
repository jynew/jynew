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
using Application = UnityEngine.Application;
using UnityEngine.UI;
using Jyx2.InputCore;
using Jyx2.MOD.ModV2;
using Sirenix.Utilities;

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
	public ETCTouchPad m_TouchPad;
	public ETCJoystick m_Joystick;

	private Jyx2Player _gameMapPlayer;
	private Image m_BlackCover;
	
	NavMeshAgent _playerNavAgent;

	CameraHelper m_CameraHelper;

	public static LMapConfig LastGameMap = null; //前一个地图
	private static LMapConfig _currentMap;

	public static void SetCurrentMap(LMapConfig map)
	{
		_currentMap = map;
	}

	[HideInInspector]
	public bool IsInited = false;

	//BattleHelper m_BattleHelper;

	public bool IsMobilePlatform()
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
	public static LMapConfig GetCurrentGameMap()
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
	public static bool IsInWorldMap => _currentMap != null && _currentMap.Tags.Contains("WORLDMAP");

	public Image BlackCover
    {
		get
        {
			if(m_BlackCover == null)
            {
				m_BlackCover = transform.Find("UI/BlackCover")?.GetComponent<Image>();
            }
			return m_BlackCover;
        }
    }

	public bool IsFadingScene => BlackCover != null && BlackCover.gameObject.activeSelf;

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
		
		
		//判断是否有进入触发的事件，如果有则触发
		if (_currentMap != null && !_currentMap.BindScript.IsNullOrWhitespace())
		{
			if (!FindObjectOfType<LevelMasterBooster>().m_IsBattleMap)
			{
				FindObjectOfType<GameEventManager>().ExecuteJyx2Event(_currentMap.BindScript + ".Start");	
			}
		}
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

			var modConfig = RuntimeEnvSetup.CurrentModConfig;
			
			//高度
			if (viewPortType == GameViewPortManager.ViewportType.Topdown)
			{
				
				body.m_FollowOffset = modConfig.CameraOffsetFar != Vector3.zero ? modConfig.CameraOffsetFar : GlobalAssetConfig.Instance.defaultVcamOffset;	
			}
			else if(viewPortType == GameViewPortManager.ViewportType.TopdownClose)
			{
				body.m_FollowOffset = modConfig.CameraOffsetNear != Vector3.zero ? modConfig.CameraOffsetNear : GlobalAssetConfig.Instance.vcamOffsetClose;
			}


			if (_gameMapPlayer != null)
			{
				//跟随对象
				vcam.Follow = _gameMapPlayer.transform;	
			}
		}
	}

	private void PlayMusic(LMapConfig currentMap)
	{
		if (currentMap == null) return;
		
		//有上一张图的出门音乐就放该音乐
		if (LastGameMap != null)
		{
			var music = LastGameMap.GetOutMusic();
                        if (music != -1)
                        {
                            AudioManager.PlayMusic(music);
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
			m_Joystick.gameObject.SetActive(IsMobilePlatform() && !(GameSettingManager.MobileMoveMode == GameSettingManager.MobileMoveModeType.Click));
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
			GetPlayer()?.StopPlayerMovement();

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

		var playerMovement = _gameMapPlayer.GetComponent<Jyx2_PlayerMovement>();
		if (playerMovement != null)
		{
			playerMovement.StopMovement();
		}
	}

	private async UniTask SetPlayer(Jyx2Player playerRoleView)
	{
		_playerNavAgent = playerRoleView.GetComponent<NavMeshAgent>();

		SetPlayerSpeed(0);
		var gameMap = GetCurrentGameMap();
		if (gameMap != null && gameMap.Tags.Contains("WORLDMAP"))
		{
			_playerNavAgent.speed = GameSettings.GetFloat("PLAYER_MOVE_SPEED_WORLD_MAP");
		}
		else
		{
			_playerNavAgent.speed = GameSettings.GetFloat("PLAYER_MOVE_SPEED");
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
		GamePadUpdate();
	}

	private Vector3 _tempCameraPosition;

	public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
	{
		Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
		Vector3 resultVec3 = center + point;
		return resultVec3;
	}

	#region 导航标志
	private bool gamepadConnected;
	private Button interactiveButton;
	
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

	//手动存档
	public void OnManuelSave(int index = -1)
	{
		if (runtime == null)
		{
			StoryEngine.DisplayPopInfo("<color=red>存档失败！</color>");
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
		StoryEngine.DisplayPopInfo("存档成功！");
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
			string eventId = obj.name.Trim();

			try
			{
				string modify = runtime.GetModifiedEvent(gameMap.Id, eventId);
				if (!string.IsNullOrEmpty(modify))
				{
					string[] tmp = modify.Split('_');
					evt.m_InteractiveEventId = tmp[0];
					evt.m_UseItemEventId = tmp[1];
					evt.m_EnterEventId = tmp[2];
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
		if (button == null)
			return;
		if (!button.gameObject.activeInHierarchy)
			return;
        if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.Interact1))
        {
            button.onClick?.Invoke();
        }
    }
}

