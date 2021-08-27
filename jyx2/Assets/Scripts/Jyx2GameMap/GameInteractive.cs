/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Jyx2;
using HSFrameWork.Common;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[DisallowMultipleComponent]
/// <summary>
/// 可交互场景物体
/// </summary>
public class GameInteractive : MonoBehaviour {
    
    //目标交互类型
    public enum TargetAction
    {
        DeActive,
    }

    //目标对象
    public List<GameObject> m_Targets;

    //目标对象设置
    public TargetAction m_TargetAction = TargetAction.DeActive;

    //指令
    public string m_Command;

    //交互按钮上写的字
    public string m_EnteractiveText = "交互";

    //是否交互一次
    public bool m_IsOnce = true;

    //接受的trigger名字
    public List<string> m_AcceptColliderNames;

    //传参，可以在指令中使用${index}来进行调用
    public List<GameObject> m_ParamGameObjects;

    [Header("Level Design")]
    //推物体
    public GameObject MoveObject;
    public Vector3 m_AddMove;
    public float m_MoveTime;
    private Vector3 finalPosition;

    public Vector3 m_AddRotate;
    private Quaternion finalRotation;

    //踩机关
    private Vector3 m_OriginalPosition;
    public bool m_TriggerObject = false;

    public GameObject m_button;
    private Vector3 m_OriginalButtonPosition;
    public Vector3 m_AddButtonMove;
    public float m_ButtonMoveTime;
    private Vector3 finalButtonPosition;

    public GameObject m_TimelineObject;
    public bool BlockPlayerControl = true;
    public GameObject m_TranspotTrigger;

    public GameObject m_activeObject;

    private void Start()
    {
        if(MoveObject != null)
            m_OriginalPosition = MoveObject.transform.position;
        if(m_button != null)
            m_OriginalButtonPosition = m_button.transform.position;
    }

    bool IsGameObjectAccept(GameObject obj)
    {
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

    Button GetInteractiveButton()
    {
        var root = GameObject.Find("LevelMaster/UI");
        var btn = root.transform.Find("InteractiveButton").GetComponent<Button>();
        return btn;
    }

    void OnTriggerStay(Collider other)
    {
        if (!IsGameObjectAccept(other.gameObject))
            return;

        ShowInteractiveButton(m_EnteractiveText);
        //踩机关
        if (m_TriggerObject)
        {
            finalPosition = m_OriginalPosition + m_AddMove;
            //if((finalPosition - MoveObject.transform.position).magnitude > 0.1)
            MoveObject.transform.DOMove(finalPosition, m_MoveTime);
            if (m_button != null)
            {
                finalButtonPosition = m_OriginalButtonPosition + m_AddButtonMove;
                m_button.transform.DOMove(finalButtonPosition, m_ButtonMoveTime);
            }
        }
    }

    void ShowInteractiveButton(string text)
    {
        //Debug.Log("on GameInteractive enter");
        var btn = GetInteractiveButton();
        btn.GetComponentInChildren<Text>().text = text;
        btn.gameObject.SetActive(true);
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);

        //anim
        btn.transform.Find("FocusImage").gameObject.SetActive(true);
        //HSUtilsEx.CallWithDelay(btn, () =>
        //{
        //    btn.transform.Find("FocusImage").gameObject.SetActive(false);
        //}, 0.5f);
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsGameObjectAccept(other.gameObject))
            return;
        //Debug.Log("on GameInteractive leave");
        var btn = GetInteractiveButton();

        HSUtilsEx.CallWithDelay(this, () => {
            btn.gameObject.SetActive(false);
        }, 0.5f);
        //踩机关
        if (m_TriggerObject)
        {
            finalPosition = m_OriginalPosition;
            MoveObject.transform.DOMove(finalPosition, m_MoveTime);
            if (m_button != null)
            {
                finalButtonPosition = m_OriginalButtonPosition;
                m_button.transform.DOMove(finalButtonPosition, m_ButtonMoveTime);
            }
        }
    }

    void OnClicked()
    {
        if (m_IsOnce)
        {
            this.gameObject.SetActive(false);
        }

        var btn = GetInteractiveButton();
        btn.gameObject.SetActive(false);

        if (m_Targets != null)
        {
            if (m_TargetAction == TargetAction.DeActive)
            {
                foreach(var t in m_Targets)
                {
                    t.SetActive(false);
                }
            }
        }
        //推物体
        if (MoveObject != null)
        {
            if (!m_IsOnce)
            {
                finalPosition = MoveObject.transform.position + (MoveObject.transform.position - GameRuntimeData.Instance.Player.View.transform.position).normalized * 10;
                finalPosition = new Vector3(finalPosition.x, MoveObject.transform.position.y, finalPosition.z);
            }
            else
            {
                finalPosition = MoveObject.transform.position + m_AddMove;
            }
            MoveObject.transform.DOMove(finalPosition, m_MoveTime);
        }

        if (m_TimelineObject != null)
        {
            PlayTimeline(m_TimelineObject, null);
        }

        if (m_activeObject != null)
        {
            m_activeObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(m_Command))
        {
            StoryEngine.Instance.ExecuteCommand(m_Command, m_ParamGameObjects);
        }
    }

    private void PlayableDiretor_stopped(PlayableDirector obj)
    {
        obj.gameObject.SetActive(false);
        obj.stopped -= PlayableDiretor_stopped;

        StoryEngine.Instance.BlockPlayerControl = false;
        //播放结束后传送
        if (m_TranspotTrigger != null)
        {
            var levelMaster = FindObjectOfType<LevelMaster>();
            levelMaster.Transport(m_TranspotTrigger.name);
            GameRuntimeData.Instance.Player.View.gameObject.SetActive(true);
        }
        
        //UI恢复
        var mainUI = GameObject.Find("MainUI");
        if(mainUI != null )
            mainUI.gameObject.SetActive(true);

        if (__timeLineCallback != null)
        {
            __timeLineCallback();
            __timeLineCallback = null;
        }
    }
    Action __timeLineCallback;

    void PlayTimeline(GameObject timeObject, Action callback)
    {
        var playableDiretor = timeObject.GetComponent<PlayableDirector>();

        playableDiretor.stopped += PlayableDiretor_stopped;

        __timeLineCallback = callback;

        //阻塞角色行动
        StoryEngine.Instance.BlockPlayerControl = BlockPlayerControl;
        playableDiretor.Play();
        
        //UI隐藏
        var mainUI = GameObject.Find("MainUI");
        if(mainUI != null )
            mainUI.gameObject.SetActive(false);

        if (m_TranspotTrigger != null)
        {
            GameRuntimeData.Instance.Player.View.gameObject.SetActive(false);
        }

        timeObject.SetActive(true);
    }
}
