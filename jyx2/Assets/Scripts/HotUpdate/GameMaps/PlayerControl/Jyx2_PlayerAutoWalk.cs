using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Jyx2
{
    [DisallowMultipleComponent]
    public class Jyx2_PlayerAutoWalk:MonoBehaviour
    {
        private NavMeshAgent m_NavmeshAgent;

        private Jyx2Player m_MapPlayer;

        private bool m_IsWalking = false;

        private Action m_OnArriveDestination;

        public bool IsAutoWalking => m_IsWalking;

        private void Awake()
        {
            m_NavmeshAgent = GetComponent<NavMeshAgent>();
            m_MapPlayer = GetComponent<Jyx2Player>();
        }

        public void PlayerWarkFromTo(Vector3 fromVector, Vector3 toVector, Action callback)
        {
            if (m_NavmeshAgent == null)
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
            m_IsWalking = true;
            m_OnArriveDestination = callback;
            m_NavmeshAgent.Warp(fromVector);
            m_NavmeshAgent.isStopped = false;
            m_NavmeshAgent.updateRotation = true;
            bool isDestinationReachable = m_NavmeshAgent.SetDestination(toVector);
            if (!isDestinationReachable)
            {
                Debug.LogError("SetDestination设置自动寻路终点失败，无法到达的目标点");
                FinishAutoWalking();
                return;
            }
        }

        private void Update()
        {
            if (!m_IsWalking)
                return;
            if (!m_NavmeshAgent.pathPending && m_NavmeshAgent.enabled && !m_NavmeshAgent.isStopped && m_NavmeshAgent.remainingDistance <= m_NavmeshAgent.stoppingDistance)
            {
                FinishAutoWalking();
            }
        }

        private void FinishAutoWalking()
        {
            m_NavmeshAgent.updateRotation = false;
            m_OnArriveDestination?.Invoke();
            m_OnArriveDestination = null;
            m_IsWalking = false;
            m_MapPlayer.StopPlayerMovement();
        }
    }
}
