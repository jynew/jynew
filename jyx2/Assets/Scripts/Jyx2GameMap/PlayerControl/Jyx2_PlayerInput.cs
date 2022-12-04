using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Jyx2.InputCore
{
    [DisallowMultipleComponent]
    public class Jyx2_PlayerInput : MonoBehaviour,IJyx2_InputContext
    {

        private int m_EnableFrame = int.MaxValue;

        public bool CanUpdate => Jyx2_Input.IsPlayerContext;

        private bool IsValidFrame => m_EnableFrame <= Time.frameCount;


        //寻路终点图标
        public GameObject navPointerPrefab; //寻路图标prefab
        private GameObject navPointer
        {
            get
            {
                if (_navPointer == null)
                    _navPointer = GameObject.Instantiate(navPointerPrefab);
                return _navPointer;
            }
        }
        private GameObject _navPointer;
        private float _navDisplayTime;

        private Jyx2Player m_MapPlayer;
        private Jyx2_PlayerMovement m_PlayerMovement;
        private Jyx2_PlayerAutoWalk m_AutoWalker;

        private void Awake()
        {
            m_MapPlayer = GetComponent<Jyx2Player>();
            m_PlayerMovement = GetComponent<Jyx2_PlayerMovement>();
            m_AutoWalker = GetComponent<Jyx2_PlayerAutoWalk>();
        }

        void OnEnable()
        {
            InputContextManager.Instance.AddInputContext(this, true);
            m_EnableFrame = Time.frameCount + 1;
        }

        void OnDisable()
        {
            InputContextManager.Instance.RemoveInputContext(this);
        }


        public void OnUpdate()
        {
            UpdatePlayerInput();
            TryClearNavPointer();
            UpdateUIInput();
        }

        private void UpdatePlayerInput()
        {
            if (!m_MapPlayer.CanControlPlayer)
                return;

            if (GameViewPortManager.Instance.GetViewportType() != GameViewPortManager.ViewportType.Follow || LevelMaster.IsInWorldMap)
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

        private void UpdateUIInput()
        {
            if (!m_MapPlayer.CanControlPlayer)
                return;
            if(Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UI_SystemMenu))
            {
                var ui = Jyx2_UIManager.Instance.GetUI<MainUIPanel>();
                if (ui != null)
                    ui.OnSystemBtnClick();
            }
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UI_Xiake))
            {
                var ui = Jyx2_UIManager.Instance.GetUI<MainUIPanel>();
                if (ui != null)
                    ui.OnXiakeBtnClick();
            }
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UI_Bag))
            {
                var ui = Jyx2_UIManager.Instance.GetUI<MainUIPanel>();
                if (ui != null)
                    ui.OnBagBtnClick();
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
            if (!m_PlayerMovement.IsNavAgentAvailable) 
                return;

            //在editor上可以寻路
            if (IsClickControlEnable())
            {
                //点击寻路
                if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !UnityTools.IsPointerOverUIObject())
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    //NPC层
                    if (Physics.Raycast(ray, out RaycastHit hitInfo, 500, LayerMask.GetMask("NPC")))
                    {
                        if (GameRuntimeData.Instance.Player.View != null)
                        {
                            var dist = Vector3.Distance(GameRuntimeData.Instance.Player.View.transform.position, hitInfo.transform.position);
                            Debug.Log("on npc clicked, dist = " + dist);

                            //现在没有直接地图上点击NPC的实现	
                        }
                    }
                    //BY CG: MASK：15:Ground层
                    else if (Physics.Raycast(ray, out hitInfo, 500, LayerMask.GetMask("Ground")))
                    {
                        if (LevelMaster.GetCurrentGameMap().Tags.Contains("NONAVAGENT"))
                        {
                            var dest = hitInfo.point;
                            var sourcePos = m_MapPlayer.transform.position;
                            if (Vector3.Distance(m_MapPlayer.transform.position, dest) < 0.1f) return;
                            transform.LookAt(new Vector3(dest.x, m_MapPlayer.transform.position.y, dest.z));
                            //设置位移
                            transform.position = Vector3.Lerp(m_MapPlayer.transform.position, dest, Time.deltaTime);
                            //计算当前速度
                            var speed = (transform.position - sourcePos).magnitude / Time.deltaTime;
                            m_PlayerMovement.SetNavAgentUpdateRotation(true);
                            m_PlayerMovement.SetManualMoveSpeed(speed);
                        }
                        else
                        {
                            m_PlayerMovement.MoveToDestination(hitInfo.point);
                        }

                        DisplayNavPointer(hitInfo.point);
                    }
                }
                else
                {
                    m_PlayerMovement.SetManualMoveSpeed(0);
                }
            }
        }

        private bool IsAxisMoved()
        {
            return Jyx2_Input.GetAxis2DRaw(Jyx2PlayerAction.MoveHorizontal, Jyx2PlayerAction.MoveVertical) != Vector2.zero;
        }

        private bool IsETCJoyStickMoved()
        {
            return LevelMaster.Instance.m_Joystick.axisX.axisValue != 0 || LevelMaster.Instance.m_Joystick.axisY.axisValue != 0;
        }

        void OnManualControlPlayer()
        {
            if (IsAxisControlEnable() && IsAxisMoved())
            {
                var input = Jyx2_Input.GetAxis2DRaw(Jyx2PlayerAction.MoveHorizontal, Jyx2PlayerAction.MoveVertical);
                m_PlayerMovement.UpdateMovement(input.normalized);
            }
            else if (IsJoystickControlEnable() && IsETCJoyStickMoved())
            {
                var input = new Vector2(-LevelMaster.Instance.m_Joystick.axisX.axisValue, 
                                        LevelMaster.Instance.m_Joystick.axisY.axisValue);
                m_PlayerMovement.UpdateMovement(input.normalized);
            }
            else
            {
                if (!m_PlayerMovement.IsNavAgentUpdateRotation)
                {
                    m_PlayerMovement.SetManualMoveSpeed(0);
                }
                //如果被锁方向，在这解锁
                if (m_PlayerMovement.IsLockingDirection)
                {
                    m_PlayerMovement.IsLockingDirection = false;
                }
            }
        }

        void OnManualControlPlayerFollowViewport()
        {
            m_PlayerMovement.IsLockingDirection = true;
            m_PlayerMovement.SetNavAgentUpdateRotation(false);
            if (IsAxisControlEnable() && IsAxisMoved())
            {
                var input = Jyx2_Input.GetAxis2DRaw(Jyx2PlayerAction.MoveHorizontal, Jyx2PlayerAction.MoveVertical);
                m_PlayerMovement.UpdateMovement(input.normalized);
            }
            else if (IsJoystickControlEnable() && IsETCJoyStickMoved())
            {
                var input = new Vector2(-LevelMaster.Instance.m_Joystick.axisX.axisValue,
                                        LevelMaster.Instance.m_Joystick.axisY.axisValue);
                m_PlayerMovement.UpdateMovement(input.normalized);
            }
            else
            {
                m_PlayerMovement.SetManualMoveSpeed(0);
            }

            if (Jyx2_Input.GetButton(Jyx2PlayerAction.RotateLeft))
            {
                transform.RotateAround(transform.position, Vector3.up, -5);
            }

            if (Jyx2_Input.GetButton(Jyx2PlayerAction.RotateRight))
            {
                transform.RotateAround(transform.position, Vector3.up, 5);
            }
            //鼠标滑屏
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !UnityTools.IsPointerOverUIObject())
            {
                transform.RotateAround(transform.position, Vector3.up, 15 * Input.GetAxis("Mouse X"));
            }

            //鼠标滚轮缩放视角
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

        private void DisplayNavPointer(Vector3 pos)
        {
            navPointer.transform.position = pos + Vector3.up * 0.2f;
            _navDisplayTime = Time.time;
            navPointer.BetterSetActive(true);
        }

        void TryClearNavPointer()
        {
            if (navPointer.activeSelf && Time.time - _navDisplayTime > 1)
            {
                navPointer.BetterSetActive(false);
            }
        }

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

        public bool IsJoystickControlEnable()
        {
            return LevelMaster.Instance.IsMobilePlatform() && !IsMobileClickControl();
        }
    }
}
