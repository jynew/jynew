using Jyx2.InputCore;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Jyx2
{
    [DisallowMultipleComponent]
    public class Jyx2_PlayerMovement : MonoBehaviour
    {
        private float m_ManualMoveSpeed = 0;

        public bool IsLockingDirection { get; set; }

        public bool IsNavAgentUpdateRotation => _playerNavAgent != null && _playerNavAgent.updateRotation;

        public bool IsNavAgentAvailable => _playerNavAgent != null && _playerNavAgent.enabled && _playerNavAgent.isOnNavMesh;

        private NavMeshAgent _playerNavAgent;

        private NavMeshPath _cachePath;

        private Animator m_Animator;

        private Jyx2_PlayerAutoWalk m_AutoWalker;

        private void Awake()
        {
            _playerNavAgent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponentInChildren<Animator>();
            m_AutoWalker = GetComponent<Jyx2_PlayerAutoWalk>();
            Jyx2_Input.OnPlayerInputStateChange += OnPlayerInputStateChange;
        }

        private void OnDestroy()
        {
            Jyx2_Input.OnPlayerInputStateChange -= OnPlayerInputStateChange;
        }

        void OnPlayerInputStateChange(bool isInputEnabled)
        {
            if(!isInputEnabled && !m_AutoWalker.IsAutoWalking)
            {
                //失去玩家控制且不是自动寻路
                StopMovement();
            }
        }

        private Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
        {
            Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
            Vector3 resultVec3 = center + point;
            return resultVec3;
        }

        public void SetRotation(int ro)
        {
            int[] roationSet = { -90, 0, 180, 90 };
            transform.rotation = Quaternion.Euler(Vector3.up * roationSet[ro]);
        }

        public void SetManualMoveSpeed(float speed)
        {
            m_ManualMoveSpeed = speed;
        }


        public void UpdateMovement(Vector2 input)
        {
            float h = input.x;
            float v = input.y;
            //Debug.Log($"h={h},v={v}");
            _playerNavAgent.updateRotation = false;
            _playerNavAgent.isStopped = true;
            
            Vector3 forward = Vector3.zero;
            //尝试只使用摄像机的朝向来操作角色移动
            forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = RotateRound(forward, Vector3.zero, Vector3.up, 90);
            right.y = 0;
            right.Normalize();

            var dest = transform.position + right * h + forward * v;
            if (!IsLockingDirection)
            { 
                transform.LookAt(new Vector3(dest.x, transform.position.y, dest.z));
            }
            var sourcePos = transform.position;
            var maxSpeed = _playerNavAgent.speed;

            //设置位移
            transform.position = Vector3.Lerp(transform.position, dest, Time.deltaTime * maxSpeed);

            //计算当前速度
            var speed = (transform.position - sourcePos).magnitude / Time.deltaTime;
            SetManualMoveSpeed(speed);

            if (IsNavAgentAvailable)
            {
                _playerNavAgent.isStopped = true;
                _playerNavAgent.ResetPath();
            }
        }

        public void UpdateMovement(float h, float v)
        {
            var input = new Vector2(h, v);
            UpdateMovement(input);
        }

        public void StopMovement()
        {
            SetManualMoveSpeed(0);
            _playerNavAgent.isStopped = true;
        }

        public void MoveToDestination(Vector3 target)
        {
            _playerNavAgent.isStopped = false;
            _playerNavAgent.updateRotation = true;
            if (_playerNavAgent.SetDestination(target))
            {
                if (_cachePath == null)
                    _cachePath = new NavMeshPath();

                bool isPathValid = _playerNavAgent.CalculatePath(_playerNavAgent.destination, _cachePath);
                if (isPathValid)
                    _playerNavAgent.SetPath(_cachePath);
            }
        }
        
        public void SetNavAgentUpdateRotation(bool updateRotation)
        {
            if (_playerNavAgent == null)
                return;
            _playerNavAgent.updateRotation = updateRotation;
        }

        private void Update()
        {
            UpdateAnimSpeed();
        }

        void UpdateAnimSpeed()
        {
            float navAgentSpeed = IsNavAgentAvailable ? _playerNavAgent.velocity.magnitude : 0;
            float manualMoveSpeed = m_ManualMoveSpeed;
            float finalSpeed = Math.Max(manualMoveSpeed, navAgentSpeed);
            m_Animator.SetFloat("speed", Mathf.Clamp(finalSpeed, 0, 20));
        }
    }
}
