using Cinemachine;
using HSUI;
using System;
using UniRx;
using UnityEngine;

public class CameraHelper : BaseUI
{
    public static CameraHelper Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<CameraHelper>();
            return _instance;
        }
    }
    private static CameraHelper _instance;

    public CinemachineStateDrivenCamera m_StateDrivenCam;
    private CinemachineFreeLook _freeLook;

    public Transform m_BattleCam;
    private CinemachineVirtualCamera _battleVCam;

    public float m_RrotateSpeed = 50f;//旋转速度
    public float smoothing = 3;//平滑系数
    private float zoomSpeed = 3f;//缩放速度

    private LevelMaster _levelMaster;
    //private BattleHelper _battleHelper;
    private Transform m_CameraFollow;

    bool isBattleFieldLockRole = true;

    //记录玩家上一次两个手指的距离
    private float lastTouchDistance = 0;

    /// <summary>
    /// 设置战场相机锁定当前人物
    /// </summary>
    public void BattleFieldLockRole()
    {
        isBattleFieldLockRole = true;
    }

    private void Start()
    {
        if (_levelMaster == null) _levelMaster = GameObject.FindObjectOfType<LevelMaster>();
        //if (_battleHelper == null) _battleHelper = GameObject.FindObjectOfType<BattleHelper>();
        if (m_CameraFollow == null)
        {
            var player = GameObject.Find("Player");
            if(player != null)
            {
                m_CameraFollow = player.transform;
            }
        }
        _freeLook = m_StateDrivenCam.transform.Find("CM FreeLook").GetComponent<CinemachineFreeLook>();
        _battleVCam = m_BattleCam.transform.Find("CM VCam").GetComponent<CinemachineVirtualCamera>();

        if(m_CameraFollow != null)
        {
            ChangeFollow(m_CameraFollow);
        }
        
        if (_levelMaster.m_TouchPad != null)
        {
            Observable.EveryUpdate()
                .Where(_ => m_CameraFollow != null)
                .Subscribe(ms =>
                {
                    //移动平台直接滑动
                    float nor = _levelMaster.m_TouchPad.axisX.axisValue;
                    if (nor != 0)
                    {
                        if (BattleManager.Instance.IsInBattle)
                        {
                            m_BattleCam.transform.RotateAround(m_BattleCam.position, Vector3.up, 0.005f * m_RrotateSpeed * nor);//每帧旋转空物体，相机也跟随旋转
                        }
                        else
                        {
                            if (_freeLook.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
                                _freeLook.m_XAxis.Value = 0.005f * m_RrotateSpeed * nor;
                            if (_freeLook.m_BindingMode == CinemachineTransposer.BindingMode.WorldSpace)
                                _freeLook.m_Heading.m_Bias = (_freeLook.m_Heading.m_Bias + 0.005f * m_RrotateSpeed * nor) % 360;
                        }
                    }

                    if(Input.touchCount == 2)
                    {
                        if(Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                        {
                            //移动后记录新手指位置
                            var newPosition1 = Input.GetTouch(0).position;
                            var newPosition2 = Input.GetTouch(1).position;

                            float currentTouchDistance = Vector3.Distance(newPosition1, newPosition2);
                            if (lastTouchDistance == 0)
                                lastTouchDistance = currentTouchDistance;

                            float distance = (currentTouchDistance - lastTouchDistance) * Time.deltaTime/2;

                            Debug.Log(distance);

                            Vector3 tmpPos = _battleVCam.transform.position;
                            tmpPos += _battleVCam.transform.forward * distance * zoomSpeed;
                            tmpPos.y = Mathf.Clamp(tmpPos.y, 5, 10);//限制缩放范围
                            if ((tmpPos.y <= 5 && distance > 0) || (tmpPos.y >= 10 && distance < 0))
                                //到缩放极限后，防止镜头向其他方向移动
                                tmpPos = _battleVCam.transform.position;
                            _battleVCam.transform.position = tmpPos;

                            lastTouchDistance = currentTouchDistance;
                        }
                    }
                    else
                    {
                        lastTouchDistance = 0;
                    }
                });
        }

        if (!Application.isMobilePlatform)
        {
            Observable.EveryUpdate()
                .Where(_ => m_CameraFollow != null)
                .Subscribe(ms =>
                {
                    //使用鼠标右键
                    if (Input.GetMouseButton(1))
                    {
                        float nor = Input.GetAxis("Mouse X");//获取鼠标的偏移量

                        //每帧旋转空物体，相机也跟随旋转
                        if (BattleManager.Instance.IsInBattle)
                        {
                            m_BattleCam.transform.RotateAround(m_BattleCam.position, Vector3.up, Time.deltaTime * m_RrotateSpeed * nor * 2);//每帧旋转空物体，相机也跟随旋转
                        }
                        else
                        {
                            if (_freeLook.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
                                _freeLook.m_XAxis.Value = Time.deltaTime * m_RrotateSpeed * nor;
                            if (_freeLook.m_BindingMode == CinemachineTransposer.BindingMode.WorldSpace)
                                _freeLook.m_Heading.m_Bias = (_freeLook.m_Heading.m_Bias + Time.deltaTime * m_RrotateSpeed * nor) % 360;
                        }
                    }

                    
                    //平移
                    var v = Input.GetAxis("Vertical");
                    var h = Input.GetAxis("Horizontal");
                    if(v!=0 || h != 0)
                    {
                        isBattleFieldLockRole = false;
                        
                        Vector3 movement = new Vector3(h * 30 * Time.deltaTime, 0f, v * 30 * Time.deltaTime);
                        
                        //BY CG:不知道哪里旋转了45度，总之这样就对了……
                        //movement = Quaternion.Euler(0,45,0) * movement;

                        m_BattleCam.Translate(movement);
                    }

                    //滚轮缩放
                    var scroll = Input.GetAxis("Mouse ScrollWheel");
                    if(scroll != 0)
                    {
                        Vector3 tmpPos = _battleVCam.transform.position;
                        tmpPos += _battleVCam.transform.forward * scroll * zoomSpeed;
                        tmpPos.y = Mathf.Clamp(tmpPos.y, 5, 10);//限制缩放范围
                        if ((tmpPos.y <= 5 && scroll > 0) || (tmpPos.y >= 10 && scroll < 0))
                            //到缩放极限后，防止镜头向其他方向移动
                            tmpPos = _battleVCam.transform.position;
                        _battleVCam.transform.position = tmpPos;
                    }
                });
        }

        Observable.EveryLateUpdate()
            .Where(_ => m_CameraFollow != null && m_BattleCam != null)
            .Subscribe(ms =>
            {
                if (isBattleFieldLockRole)
                {
                    if (BattleManager.Instance.IsInBattle)
                    {
                        if (!m_CameraInit) //首次初始化，瞬间切换相机到角色位置
                        {
                            m_BattleCam.transform.position = m_CameraFollow.position;
                            m_CameraInit = true;
                        }
                        else //否则缓慢移动
                        {
                            //目标物体要到达的目标位置
                            Vector3 targetPos = m_CameraFollow.position;

                            //使用线性插值计算让摄像机用smoothing * Time.deltaTime时间从当前位置到移动到目标位置
                            m_BattleCam.transform.position = Vector3.Lerp(m_BattleCam.transform.position, targetPos, smoothing * Time.deltaTime);
                        }
                    }
                    else
                    {
                        m_BattleCam.transform.position = m_CameraFollow.position;
                    }

                    //Camera.main.transform.position = m_BattleCam.transform.position;
                }
                
            });
    }

    bool m_CameraInit = false;

    public void ChangeFollow(Transform follow)
    {
        m_CameraFollow = follow;

        //战斗摄像机
        if (BattleManager.Instance.IsInBattle)
        {
            m_StateDrivenCam.gameObject.SetActive(false);

            //让VCam位置方向和现有摄像机保持一致
            m_BattleCam.gameObject.SetActive(true);
            //_battleVCam.transform.position = Camera.main.transform.position;
            //_battleVCam.transform.rotation = Camera.main.transform.rotation;
            //战斗开始重置震动参数
            //NoiseSettings noise = Resources.Load("CinemachineNoise/6D Shake") as NoiseSettings;
            //_battleVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = noise;
            //_battleVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
            //_battleVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;

            isBattleFieldLockRole = true;
        }
        //通用摄像机
        else
        {
            //StateDrivenCam摄像机以CinemationCamera自带的方式跟踪角色
            m_StateDrivenCam.Follow = follow;
            m_StateDrivenCam.LookAt = follow;
            m_StateDrivenCam.gameObject.SetActive(true);

            //战斗摄像机位置和目标角色保持同步
            m_BattleCam.gameObject.SetActive(false);
        }
    }

    private CompositeDisposable _disposables = new CompositeDisposable();
    public void ShakeCamera(float duration, int amplitude, int frequency)
    {
        _disposables.Clear();
        NoiseSettings noise = Resources.Load("CinemachineNoise/6D Shake") as NoiseSettings;
        _freeLook.GetRig(1).AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _freeLook.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = noise;
        _freeLook.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        _freeLook.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
        Observable.TimerFrame(Convert.ToInt32(duration * 60), FrameCountType.FixedUpdate).Subscribe(ms =>
        {
            _freeLook.GetRig(1).DestroyCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }).AddTo(_disposables);
    }

    /// <summary>
    /// 改变FOV
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="finalFov"></param>
    /// <param name="duration"></param>
    public void ChangeFOV(float finalFov, float duration)
    {
        var frame = Convert.ToInt32(duration * 60);
        float frameFov = (finalFov - _freeLook.m_Lens.FieldOfView) / frame;
        Debug.Log(frame);
        Debug.Log(frameFov);
        var start = Observable.EveryFixedUpdate()
            .Where(_ => _freeLook.m_Lens.FieldOfView < finalFov)
            .Subscribe(ms =>
            {
                _freeLook.m_Lens.FieldOfView += frameFov;
            });
        Observable.TimerFrame(frame + 60, FrameCountType.FixedUpdate)
            .Subscribe(ms =>
            {
                start.Dispose();
            });
    }

    /// <summary>
    /// 改变战斗相机的FOV
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="finalFov"></param>
    /// <param name="duration"></param>
    public void ChangeBattleCameraFOV(float finalFov, float duration)
    {
        var frame = Convert.ToInt32(duration * 60);
        float frameFov = (finalFov - _battleVCam.m_Lens.FieldOfView) / frame;
        //Debug.Log(frame);
        //Debug.Log(frameFov);
        var start = Observable.EveryFixedUpdate()
            .Where(_ => _battleVCam.m_Lens.FieldOfView != finalFov)
            .Subscribe(ms =>
            {
                _battleVCam.m_Lens.FieldOfView += frameFov;
                _battleVCam.m_Lens.FieldOfView = Mathf.Clamp(_battleVCam.m_Lens.FieldOfView, 20, 70);
            });
        Observable.TimerFrame(frame + 60, FrameCountType.FixedUpdate)
            .Subscribe(ms =>
            {
                start.Dispose();
            });
    }

    /// <summary>
    /// 还原FOV
    /// </summary>
    /// <param name="duration"></param>
    public void ResetFOV(float duration)
    {
        var frame = Convert.ToInt32(duration * 60);
        float frameFov = (60 - _freeLook.m_Lens.FieldOfView) / frame;
        var end = Observable.EveryFixedUpdate()
            .Where(_ => _freeLook.m_Lens.FieldOfView > 60)
            .Subscribe(ms =>
            {
                _freeLook.m_Lens.FieldOfView += frameFov;
            });
        Observable.TimerFrame(frame + 60, FrameCountType.FixedUpdate)
            .Subscribe(ms =>
            {
                _freeLook.m_Lens.FieldOfView = 60;
                end.Dispose();
            });
    }

    public void ChangeFreeLookHeight(float finalHeight, float duration)
    {
        var frame = Convert.ToInt32(duration * 60);
        float frameHeight = (finalHeight - _freeLook.m_Orbits[1].m_Height) / frame;
        Debug.Log(frame);
        Debug.Log(frameHeight);
        var start = Observable.EveryFixedUpdate()
            .Where(_ => _freeLook.m_Orbits[1].m_Height > finalHeight)
            .Subscribe(ms =>
            {
                _freeLook.m_Orbits[1].m_Height += frameHeight;
            });
        Observable.TimerFrame(frame + 60, FrameCountType.FixedUpdate)
            .Subscribe(ms =>
            {
                start.Dispose();
            });
    }

    public void ResetFreeLookHeight(float duration)
    {
        var frame = Convert.ToInt32(duration * 60);
        float frameHeight = (5.5f - _freeLook.m_Orbits[1].m_Height) / frame;
        Debug.Log(frame);
        Debug.Log(frameHeight);
        var start = Observable.EveryFixedUpdate()
            .Where(_ => _freeLook.m_Orbits[1].m_Height < 5.5f)
            .Subscribe(ms =>
            {
                _freeLook.m_Orbits[1].m_Height += frameHeight;
            });
        Observable.TimerFrame(frame + 60, FrameCountType.FixedUpdate)
            .Subscribe(ms =>
            {
                start.Dispose();
            });
    }

    public void ChangeFreeLookRadius(float finalRadius, float duration)
    {
        var frame = Convert.ToInt32(duration * 60);
        float framefinalRadius = (finalRadius - _freeLook.m_Orbits[1].m_Radius) / frame;
        Debug.Log(frame);
        Debug.Log(framefinalRadius);
        var start = Observable.EveryFixedUpdate()
            .Where(_ => _freeLook.m_Orbits[1].m_Radius > finalRadius)
            .Subscribe(ms =>
            {
                _freeLook.m_Orbits[1].m_Radius += framefinalRadius;
            });
        Observable.TimerFrame(frame + 60, FrameCountType.FixedUpdate)
            .Subscribe(ms =>
            {
                start.Dispose();
            });
    }

    public void ResetFreeLookRadius(float duration)
    {
        var frame = Convert.ToInt32(duration * 60);
        float frameRadius = (5.0f - _freeLook.m_Orbits[1].m_Radius) / frame;
        Debug.Log(frame);
        Debug.Log(frameRadius);
        var start = Observable.EveryFixedUpdate()
            .Where(_ => _freeLook.m_Orbits[1].m_Radius < 5.0f)
            .Subscribe(ms =>
            {
                _freeLook.m_Orbits[1].m_Radius += frameRadius;
            });
        Observable.TimerFrame(frame + 60, FrameCountType.FixedUpdate)
            .Subscribe(ms =>
            {
                start.Dispose();
            });
    }
}