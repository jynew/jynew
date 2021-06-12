using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class ColliderTrigger : UIBehaviour
{
    /// <summary>
    /// 只触发一次
    /// </summary>
    [Header("只触发一次")]
    public bool Once = false;

    /// <summary>
    /// 触发后消失
    /// </summary>
    [Header("触发后消失")]
    public bool AutoDestory = false;

    /// <summary>
    /// 目标进入时触发
    /// </summary>
    [Header("目标进入时触发")]
    public bool CallEnter = true;

    /// <summary>
    /// 目标停留时触发
    /// </summary>
    [Header("目标停留时触发")]
    public bool CallStay = false;

    /// <summary>
    /// 目标离开时触发
    /// </summary>
    [Header("目标离开时触发")]
    public bool CallLeavel = false;

    /// <summary>
    /// 可被主角吸收
    /// </summary>
    [Header("可被主角吸收")]
    public bool CanSuck = false;

    /// <summary>
    /// 触发指令
    /// </summary>
    [Header("触发指令")]
    public string Command;

    /// <summary>
    /// 可触发Trigger的对象名
    /// </summary>
    public List<string> m_AcceptColliderNames;

    /// <summary>
    /// 可触发Trigger的对象Tag
    /// </summary>
    public List<MapRoleBehavior> m_AcceptColliderBehavior;

    //传参，可以在指令中使用${index}来进行调用
    public List<GameObject> m_ParamGameObjects;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        ExecuteTrigger(other, CallEnter);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!enabled) return;
        ExecuteTrigger(other, CallStay);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enabled) return;
        ExecuteTrigger(other, CallLeavel);
    }

    private void ExecuteTrigger(Collider other, bool triggerState)
    {
        if (triggerState && !string.IsNullOrEmpty(Command) && IsGameObjectAccept(other.gameObject))
        {
            if (CanSuck)
            {
                float s = 0.1f;
                int frame = Convert.ToInt32(s * 60);
                float frameScale = (1f - 0.1f) / frame;
                Vector3 framePosition = (GameRuntimeData.Instance.Player.View.transform.position - transform.position + new Vector3(0, 1f, 0)) / frame;
                var anim = Observable.EveryFixedUpdate()
                    .Where(_=> transform.localScale.x > 0.1f)
                    .Subscribe(ms =>
                    {
                        transform.localScale -= new Vector3(frameScale, frameScale, frameScale);
                        transform.position += framePosition;
                    });
                Observable.TimerFrame(frame, FrameCountType.FixedUpdate)
                    .Subscribe(ms =>
                    {
                        anim.Dispose();
                        ExecuteCommand(other);
                    });
            }
            else
            {
                ExecuteCommand(other);
            }
        }
    }

    private void ExecuteCommand(Collider other = null)
    {

        StoryEngine.Instance.ExecuteCommand(Command, m_ParamGameObjects);
        
        if (Once) enabled = false;
        Destroy();
    }

    private bool IsGameObjectAccept(GameObject obj)
    {
        if (!enabled) return false;
        if (obj == null) return false;

        if (m_AcceptColliderNames != null && m_AcceptColliderNames.Count > 0)
        {
            return m_AcceptColliderNames.Contains(obj.name);
        }
        else if (m_AcceptColliderBehavior != null && m_AcceptColliderBehavior.Count > 0)
        {
            return m_AcceptColliderBehavior.Contains(obj.GetComponent<MapRole>().m_Behavior);
        }
        else
        {
            return obj == GameRuntimeData.Instance.Player.View.gameObject;
        }
    }

    private void Destroy()
    {
        if (AutoDestory) GameObject.Destroy(gameObject);
    }
}
