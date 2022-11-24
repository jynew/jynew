using Jyx2.InputCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

namespace Jyx2
{
    [DisallowMultipleComponent]
    public class Jyx2_PlayerMovement : MonoBehaviour
    {
        public float unlockDegree = 10f;

        private Vector3 _tempDestH = Vector3.zero;

        private Vector3 _tempDestV = Vector3.zero;

        private float _tempH = 0;

        private float _tempV = 0;

        public bool IsLockingDirection { get; set; }

        public bool IsNavAgentUpdateRotation => _playerNavAgent != null && _playerNavAgent.updateRotation;

        public bool IsNavAgentAvailable => _playerNavAgent != null && _playerNavAgent.enabled && _playerNavAgent.isOnNavMesh;

        private NavMeshAgent _playerNavAgent;

        private NavMeshPath _cachePath;

        private Jyx2Player m_MapPlayer;

        private Jyx2_PlayerAutoWalk m_AutoWalker;

        private void Awake()
        {
            _playerNavAgent = GetComponent<NavMeshAgent>();
            m_MapPlayer = GetComponent<Jyx2Player>();
            m_AutoWalker = GetComponent<Jyx2_PlayerAutoWalk>();
            Jyx2_Input.OnPlayerInputStateChange += OnPlayerInputStateChange;
        }

        private void OnDestroy()
        {
            Jyx2_Input.OnPlayerInputStateChange -= OnPlayerInputStateChange;
        }

        void OnPlayerInputStateChange(bool isInputEnabled)
        {
            if(!isInputEnabled)
            {
                //失去玩家控制且不是自动寻路
                if (!m_AutoWalker.IsAutoWalking)
                {
                    StopMovement();
                }
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


        public void UpdateMovement(Vector2 input)
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

            var dest = transform.position + right * h + forward * v;
            if (_tempDestH == Vector3.zero) _tempDestH = right * h;
            if (_tempDestV == Vector3.zero) _tempDestV = forward * v;
            if (IsLockingDirection)
            {
                dest = transform.position + _tempDestH + _tempDestV;
                Vector3 cur_dir = new Vector3(h, v, 0).normalized;
                Vector3 old_dir = new Vector3(_tempH, _tempV, 0).normalized;
                if (Vector3.Angle(cur_dir, old_dir) > unlockDegree)
                {
                    IsLockingDirection = false;
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
            transform.LookAt(new Vector3(dest.x, transform.position.y, dest.z));
            var sourcePos = transform.position;
            var maxSpeed = _playerNavAgent.speed;

            //设置位移
            transform.position = Vector3.Lerp(transform.position, dest, Time.deltaTime * maxSpeed);

            //计算当前速度
            var speed = (transform.position - sourcePos).magnitude / Time.deltaTime;
            m_MapPlayer.SetAnimSpeed(speed);

            if (_playerNavAgent == null || !_playerNavAgent.enabled || !_playerNavAgent.isOnNavMesh) return;
            _playerNavAgent.isStopped = true;
            _playerNavAgent.ResetPath();
        }

        public void UpdateMovement(float h, float v)
        {
            var input = new Vector2(h, v);
            UpdateMovement(input);
        }

        public void StopMovement()
        {
            _playerNavAgent.isStopped = true;
            m_MapPlayer.SetAnimSpeed(0);
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
    }
}
