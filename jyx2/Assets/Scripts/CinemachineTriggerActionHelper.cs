using Cinemachine;
using HanSquirrel.ResourceManager;
using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class CinemachineTriggerActionHelper : UIBehaviour
{
    public bool CallEnter = false;
    public bool CallStay = false;
    public bool CallLeavel = false;
    [Header("Lock Direction")]
    public bool enterLockDirection = false;
    public bool exitLockDirection = false;

    public bool m_UnlockTarget = false;
    public CinemachineVirtualCamera vcam;
    private Transform old_followTarget;

    //接受的trigger名字
    public List<string> m_AcceptColliderNames;

    bool IsGameObjectAccept(GameObject obj)
    {
        if (!enabled) return false;
        if (obj == null) return false;

        if (m_AcceptColliderNames == null || m_AcceptColliderNames.Count == 0)
        {
            return obj == GameRuntimeData.Instance.Player.View.gameObject;
        }
        else
        {
            return m_AcceptColliderNames.Contains(obj.name);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (enterLockDirection)
        {
            LevelMaster levelMaster = GameObject.FindObjectOfType<LevelMaster>();
            //开启方向锁
            levelMaster.m_IsLockingDirection = true;
        }
        if(m_UnlockTarget)
        {
            if (vcam.Follow != null)
            {
                old_followTarget = vcam.Follow;
                vcam.Follow = null;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!enabled) return;
    }

    void OnTriggerExit(Collider other)
    {
        if (!enabled) return;
        if (exitLockDirection)
        {
            LevelMaster levelMaster = GameObject.FindObjectOfType<LevelMaster>();
            //开启方向锁
            levelMaster.m_IsLockingDirection = true;
        }
        if (m_UnlockTarget)
        {
            if (old_followTarget != null)
            {
                vcam.Follow = old_followTarget;
            }
        }
    }

    IEnumerator CallWithDelay(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    public void LockDirection()
    {
        //LevelMaster levelMaster = GameObject.FindObjectOfType<LevelMaster>();
        //开启方向锁
        //levelMaster.m_IsLockingDirection = true;
        /*
        //释放WASD任一键时解除方向锁
        var keyStream = Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D));
        keyStream.Subscribe(xs => { levelMaster.m_IsLockingDirection = false; });
        */
    }
}
