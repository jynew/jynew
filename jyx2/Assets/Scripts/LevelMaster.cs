/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Jyx2;
using HSFrameWork.ConfigTable;
using HanSquirrel.ResourceManager;
using UniRx;
using System;
using Cinemachine;
using Cysharp.Threading.Tasks;

public class LevelMaster : MonoBehaviour
{
    public static GameMap LastGameMap = null; //前一个地图

    //载入参数
    public class LevelLoadPara
    {
        public enum LevelLoadType
        {
            Load, //读档
            Entrance, //从外部进入
            StartAtTrigger, //从指定Trigger开始
            StartAtPos,
        }

        public LevelLoadType loadType = LevelLoadType.Entrance;
        public string triggerName = "";
        public string CurrentPos;
        public string CurrentOri;
    }

    static public LevelLoadPara loadPara = new LevelLoadPara();

    public static LevelMaster Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<LevelMaster>();
            return _instance;
        }
    }
    private static LevelMaster _instance;

    enum MapType
    {
        BigMap,
        Explore,
    }

    public bool RuntimeDataSimulate = true;
    public bool MobileSimulate = false;

    public ETCTouchPad m_TouchPad;

    Transform _player;
    MapRole _playerView;
    NavMeshAgent _playerNavAgent;

    MapType m_CurrentType = MapType.Explore;


    public GameObject m_MobileRotateSlider;

    public bl_HUDText HUDRoot;

    public GameObject navPointerPrefab; //寻路图标prefab

    GameObject navPointer;//寻路终点图标

    public ETCJoystick m_Joystick;

    [HideInInspector]
    public bool IsInited = false;

    [Header("Lock Direction")]
    public float unlockDegee = 10f;

    //BattleHelper m_BattleHelper;

    CameraHelper m_CameraHelper;

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
    public GameMap GetCurrentGameMap()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        var gameMap = ConfigTable.Get<GameMap>(sceneName);
        return gameMap;
    }


    // Use this for initialization
    async void Start()
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

        if (RuntimeDataSimulate && runtime == null)
        {
            //测试存档位
            var r = GameRuntimeData.CreateNew();  //选一个没有用过的id
        }

        var brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
        }
        

        var gameMap = GetCurrentGameMap();
        if (gameMap != null)
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

            if (gameMap.Tags.Contains("BIGMAP"))
            {
                m_CurrentType = MapType.BigMap;
            }
            else
            {
                //显示当前地图名，大地图不用显示
                Jyx2_UIManager.Instance.ShowUI(nameof(CommonTipsUIPanel), TipsType.MiddleTop, gameMap.GetShowName());
            }

            if (string.IsNullOrEmpty(runtime.CurrentMap))
            {
                runtime.CurrentMap = gameMap.Key;
            }
        }

        navPointer = Instantiate(navPointerPrefab);
        navPointer.SetActive(false);

        //刷新界面控制器
        UpdateMobileControllerUI();

        GraphicSetting.GlobalSetting.Execute();


        //尝试绑定主角
        TryBindPlayer();

        //刷新游戏事件
        RefreshGameEvents();        
        
        //全部初始化完以后，激活trigger的触发 
        triggers = GameObject.Find("Level/Triggers");
        if (triggers != null)
        {
            foreach (Transform trigger in triggers.transform)
            {
                var c = trigger.gameObject.GetComponent<Collider>();
                if(c != null)
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

        IsInited = true;
    }


    private void PlayMusic(GameMap gameMap)
    {
        if (gameMap == null) return;

        var enterMusic = gameMap.GetEnterMusic();
        if (!string.IsNullOrEmpty(enterMusic))
        {
            PlayMusicAtPath(enterMusic);
        }
        else if (LastGameMap != null)
        {
            PlayLeaveMusic(LastGameMap);
        }
    }

    public void PlayMusicAtPath(string musicPath)
    {
        AudioManager.PlayMusicAtPath(musicPath).Forget();
    }

    /// <summary>
    /// 播放离开音乐
    /// </summary>
    public void PlayLeaveMusic(GameMap gameMap)
    {
        if (gameMap == null) return;
        var leaveMusic = gameMap.GetLeaveMusic();
        if (!string.IsNullOrEmpty(leaveMusic))
        {
            PlayMusicAtPath(leaveMusic);
        }
    }

    private void UpdateMobileControllerUI()
    {
        //大地图或editor上都不显示
        m_Joystick.gameObject.SetActive(IsMobilePlatform() && m_CurrentType != MapType.BigMap);
        m_TouchPad.gameObject.SetActive(IsMobilePlatform() && m_CurrentType != MapType.BigMap);
        
        //战斗中移动按钮隐藏
        if (BattleManager.Instance.IsInBattle)
        {
            m_Joystick.gameObject.SetActive(false);
        }
    }

    void LoadSpawnPosition()
    {
        if (runtime == null || _player == null)
            return;
		
        var map = GetCurrentGameMap();
        if (map == null)
            return;

        if (loadPara.loadType == LevelLoadPara.LevelLoadType.Load)
        {
            if (map.Tags.Contains("WORLDMAP"))
            {
                if (string.IsNullOrEmpty(runtime.WorldPosition))
                    return;

                GetPlayer().LoadWorldInfo();
            }
            else
            {
                var pos = UnityTools.StringToVector3(runtime.CurrentPos); //从指定位置读取
                PlayerSpawnAt(pos);
				PlayerSpawnAt(UnityTools.StringToQuaternion(runtime.CurrentOri));
            }

        }
        else if (loadPara.loadType == LevelLoadPara.LevelLoadType.Entrance)
        {
            if (map.Tags.Contains("WORLDMAP")) //大地图
            {
                if (string.IsNullOrEmpty(runtime.WorldPosition))
                    return;

                GetPlayer().LoadWorldInfo();
            }
            else
            {
                var entranceObj = GameObject.FindGameObjectWithTag("Entrance"); //找入口
                if (entranceObj != null)
                {
                    var spawnPos = entranceObj.transform.position;
                    PlayerSpawnAt(spawnPos);
                }
            }
        }
        else if (loadPara.loadType == LevelLoadPara.LevelLoadType.StartAtTrigger)
        {
            GameObject startTrigger = GameObject.Find(loadPara.triggerName);
            if(startTrigger == null)
            {
                Debug.LogError($"地图{map.Name}的指定载入trigger:{loadPara.triggerName} 未定义");
                return;
            }

            var spawnPos = startTrigger.transform.position;
            PlayerSpawnAt(spawnPos);
        }else if(loadPara.loadType == LevelLoadPara.LevelLoadType.StartAtPos)
        {
            PlayerSpawnAt(UnityTools.StringToVector3(loadPara.CurrentPos));
			PlayerSpawnAt(UnityTools.StringToQuaternion(loadPara.CurrentOri));
        }
    }

    void PlayerSpawnAt(Vector3 spawnPos)
    {
        _playerNavAgent.enabled = false;
        Debug.Log("load pos = " + spawnPos);
        _player.position = spawnPos;
        _playerNavAgent.enabled = true;
    }
    void PlayerSpawnAt(Quaternion ori)
    {
        Debug.Log("load ori = " + ori);
        _player.rotation = ori;
    }


    private void SetPlayerSpeed(float speed)
    {
        if (_player == null)
            return;

        var animator = _playerView.GetAnimator();
        if(animator != null)
        {
            animator.SetFloat("speed", speed);
        }
    }

    private async UniTask SetPlayer(MapRole playerRoleView)
    {
		// reverting this change. to fix "reference on null object" error when enter/ exit scene
		// modified by eaphone at 2021/05/30
        _playerView = playerRoleView;
        _player = playerRoleView.transform;
        _playerNavAgent = playerRoleView.GetComponent<NavMeshAgent>();
        
        SetPlayerSpeed(0);
        var gameMap = GetCurrentGameMap();
        if (gameMap != null && gameMap.Tags.Contains("WORLDMAP"))
        {
            _playerNavAgent.speed = 10; //大地图上放大一倍
        }
        else
        {
            _playerNavAgent.speed = GameConst.MapSpeed;
        }
        
        _playerNavAgent.angularSpeed = GameConst.MapAngularSpeed;
        _playerNavAgent.acceleration = GameConst.MapAcceleration;

        var playerCom = _player.GetComponent<Jyx2Player>();
        if(playerCom == null)
        {
            var player = _player.gameObject.AddComponent<Jyx2Player>();
            player.Init();
        }

        await playerRoleView.BindRoleInstance(runtime.Player);
        LoadSpawnPosition();
    }

	// fix bind player failed error when select player before start battle
	// modified by eaphone at 2021/05/31
    public void TryBindPlayer()
    {
        if (_player != null)
            return;

        //寻找主角
        var playerObj = RoleHelper.FindPlayer();
        if (playerObj != null)
        {
            //设置主角
            SetPlayer(playerObj).Forget();
            //添加队友
            //CreateTeammates(gameMap, playerObj.transform);

            var gameMap = GetCurrentGameMap();
            if (gameMap != null && gameMap.Tags.Contains("POINTLIGHT")) //点光源
            {
                var obj = Jyx2ResourceHelper.CreatePrefabInstance(ConStr.PlayerPointLight);
                obj.transform.SetParent(playerObj.transform);
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

    void FixedUpdate()
    {
        TryClearNavPointer();
        PlayerControll();
    }
    
    void PlayerControll()
    {
        if (BattleManager.Instance.IsInBattle)
            return;
        
        //timeline不允许角色移动
        if (StoryEngine.Instance != null && StoryEngine.Instance.BlockPlayerControl)
        {
            SetPlayerSpeed(0);
            return;
        }
        
        if (_player == null)
            return;

        //鼠标点击控制
        OnClickControlPlayer();

        //手动操作
        OnManualControlPlayer();
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
    public void PlayerWarkFromTo(Vector3 fromVector,Vector3 toVector, Action callback) 
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
        _playerNavAgent.SetDestination(toVector);
    }

    void OnClickControlPlayer()
    {

        SetPlayerSpeed(_playerNavAgent.velocity.magnitude);
        

        if (_playerNavAgent == null || !_playerNavAgent.enabled || !_playerNavAgent.isOnNavMesh) return;
 
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
        if (!Application.isMobilePlatform || m_CurrentType == MapType.BigMap)
        {
            //点击寻路
            if (Input.GetMouseButton(1) && !UnityTools.IsPointerOverUIObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //NPC层
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("NPC")))
                {
                    var dist = Vector3.Distance(runtime.Player.View.transform.position, hitInfo.transform.position);
                    Debug.Log("on npc clicked, dist = " + dist);

                    if (dist > 4)
                    {
                        runtime.Player.View.Say("太远了，走近一点才能对话");
                    }
                    else //和NPC聊天
                    {
                        var mapRole = hitInfo.transform.GetComponent<MapRole>();
						if(mapRole!=null){
							mapRole.DoNpcChat();
						}
                    }
                }
                //BY CG: MASK：15:Ground层
                else if (Physics.Raycast(ray, out hitInfo, 100, 1 << LayerMask.NameToLayer("Ground")))
                {
                    _playerNavAgent.isStopped = false;
                    _playerNavAgent.updateRotation = true;
                    _playerNavAgent.SetDestination(hitInfo.point);

                    DisplayNavPointer(hitInfo.point);
                }
            }
        }
    }

    void OnManualControlPlayer()
    {
        if (!_CanController)//掉本调用自动寻路的时候 不能手动控制
            return;
        
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            OnManuelMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        else if(m_Joystick.axisX.axisValue!=0 || m_Joystick.axisY.axisValue != 0)
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

    public bool m_IsLockingDirection = false;
    private Vector3 _tempDestH = Vector3.zero;
    private Vector3 _tempDestV = Vector3.zero;
    //BY HW:用于记录被锁前操作的方向
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
        
        var dest = _player.position + right * h + forward * v;
        if (_tempDestH == Vector3.zero) _tempDestH = right * h;
        if (_tempDestV == Vector3.zero) _tempDestV = forward * v;
        if (m_IsLockingDirection)
        {
            dest = _player.position + _tempDestH + _tempDestV;
            //BY HW:判断解锁方向的逻辑
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
            //BY HW:记录操作方向
            _tempH = h;
            _tempV = v;
            //Debug.Log("UnLockingDirection");
        }
        _player.LookAt(new Vector3(dest.x, _player.position.y, dest.z));
        var sourcePos = _player.transform.position;
        var maxSpeed = _playerNavAgent.speed;

        //设置位移
        _player.transform.position = Vector3.Lerp(_player.transform.position, dest, Time.fixedDeltaTime * maxSpeed);

        //计算当前速度
        var speed = (_player.transform.position - sourcePos).magnitude / Time.fixedDeltaTime;
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
        TransportToTransform("Level/Triggers",transportName,"");
    }
	
	public void TransportToTransform(string path, string name, string target)
	{
		var rootObj = GameObject.Find(path);
        var trans = rootObj.transform.Find(name);

		if(trans == null)
		{
			rootObj = GameObject.Find("Level/Dynamic");
			trans=rootObj.transform.Find(name);
		}
        if(trans != null)
        {
			if(target==""){
				Transport(trans.position);
				//增加传送时设置朝向。rotation为0时不作调整，需要朝向0时候，可以使用360.
				if(trans.rotation!=Quaternion.identity){
					_player.rotation=trans.rotation;
				}
			}else{
				var t=GameObject.Find(target).transform;
				t.position=trans.position;
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
        _player.position = position;
    }
	
	// implement change player facing. 0:top-right, 1:down-right, 2:top-left, 3:down-left
	// modify by eaphone at 2021/6/5
	public void SetRotation(int ro){
		int[] roationSet={-90,0,180,90};
		_player.rotation = Quaternion.Euler(Vector3.up*roationSet[ro]);
	}

    //手动存档
    public void OnManuelSave(int index = -1)
    {
        if(runtime == null)
        {
            StoryEngine.Instance.DisplayPopInfo("<color=red>存档失败！</color>");
            Debug.LogError("存档失败！请从GameStart中启动游戏！");
            return;
        }

        if (GetCurrentGameMap().Tags.Contains("WORLDMAP"))
        {
            GetPlayer().RecordWorldInfo();
        }
        else
        {
            runtime.CurrentPos = UnityTools.Vector3ToString(_player.position);
			runtime.CurrentOri = UnityTools.QuaternionToString(_player.rotation);
        }

        Debug.Log("set current pos = " + runtime.CurrentPos);
        runtime.GameSave(index);
        StoryEngine.Instance.DisplayPopInfo("存档成功！");
    }

    public Vector3 GetPlayerPosition()
    {
        return _player.position;
    }
    public Quaternion GetPlayerOrientation()
    {
        return _player.rotation;
    }

	// handle player null exception
	// modified by eaphone at 2021/05/31
    public Jyx2Player GetPlayer()
    {
		var player=_player.GetComponent<Jyx2Player>();
        if (player == null)
        {
            player = _player.gameObject.AddComponent<Jyx2Player>();
            player.Init();
        }
        return player;
    }

    public void QuitToBigMap()
    {
		// modified by eaphone at 2021/05/30
		//LevelLoader.LoadGameMap("0_BigMap");
        //LevelLoader.LoadGameMap("Level_BigMap");
		LevelLoader.LoadGameMap("0_BigMap&transport#" + GetCurrentGameMap().BigMapTriggerName);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("0_GameStart");
    }

    //刷新本场景内的所有事件
    //事件执行和更改结果存储在runtime里，需要结合当前场景进行调整
    public void RefreshGameEvents()
    {
        var gameMap = GetCurrentGameMap();
        if (gameMap == null) return;

        //场景ID
        string sceneId = gameMap.Jyx2MapId;

        //大地图
        if (string.IsNullOrEmpty(sceneId))
            return;

        //调整所有的触发器
        GameObject eventsParent = GameObject.Find("Level/Triggers");
        foreach(Transform obj in eventsParent.transform)
        {
            var evt = obj.GetComponent<GameEvent>();
            if (evt == null) continue;
            string eventId = obj.name;

            try
            {
                string modify = runtime.GetModifiedEvent(int.Parse(sceneId), int.Parse(eventId));
                if (!string.IsNullOrEmpty(modify))
                {
                    string[] tmp = modify.Split('_');
                    evt.m_InteractiveEventId = int.Parse(tmp[0]);
                    evt.m_UseItemEventId = int.Parse(tmp[1]);
                    evt.m_EnterEventId = int.Parse(tmp[2]);
                }

                evt.Init();
            }catch(Exception e)
            {
                Debug.LogError("事件解析错误,事件ID必须是数字,eventId=" + eventId);
            }
            
        }
    }
	
	//增加接口修改bigmapzone脚本的command。用于和韦小宝对话后，增加传送韦小宝的逻辑
	//added by eaphone at 2021/6/8
	public void ModifyBigmapZoneCmd(string cmd, string target="Leave"){
		var gameMap = GetCurrentGameMap();
        if (gameMap == null) return;

        //场景ID
        string sceneId = gameMap.Jyx2MapId;

        //大地图
        if (string.IsNullOrEmpty(sceneId))
            return;

        GameObject obj = GameObject.Find("Level/Triggers/"+target);
        var evt = obj.GetComponent<BigMapZone>();
		if (evt != null){
			string eventId = obj.name;
			if(target==eventId){
				try
				{
					evt.Command = cmd;
				}catch(Exception e)
				{
					Debug.LogError(e);
				}
			}
		}
	}
}

