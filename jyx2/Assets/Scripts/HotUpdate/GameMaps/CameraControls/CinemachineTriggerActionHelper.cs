/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Cinemachine;

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


    void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (enterLockDirection)
        {
            //开启方向锁
            LockPlayerDirection();
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
            //开启方向锁
            LockPlayerDirection();
        }
        if (m_UnlockTarget)
        {
            if (old_followTarget != null)
            {
                vcam.Follow = old_followTarget;
            }
        }
    }

    private void LockPlayerDirection()
    {
        var playerMovement = LevelMaster.Instance?.GetPlayer().GetComponent<Jyx2_PlayerMovement>();
        if (playerMovement != null)
            playerMovement.IsLockingDirection = true;
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
