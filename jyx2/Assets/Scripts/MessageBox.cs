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
using System.Text;

using Jyx2;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField]
    private Button m_ConfirmButton;

    [SerializeField]
    private Button m_CancelButton;

    [SerializeField]
    private LayoutGroup m_ButtonLayout;

    [SerializeField]
    private Text m_MessageText;

	private Action _onCofirom;
    private Action _onCancel;

    private void Awake()
    {
        m_ConfirmButton.onClick.AddListener(OnConfirmBtnClick);
        m_CancelButton.onClick.AddListener(OnCancelBtnClick);
    }

    private void OnDestroy()
    {
        m_ConfirmButton.onClick.RemoveListener(OnConfirmBtnClick);
        m_CancelButton.onClick.RemoveListener(OnCancelBtnClick);
    }

    private static MessageBox CreateMessageBox(UILayer uiLayer = UILayer.Top)
    {
        var parent = Jyx2_UIManager.Instance.GetUIParent(uiLayer);
        var obj = Jyx2ResourceHelper.CreatePrefabInstance("MessageBox");
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        return obj.GetComponent<MessageBox>();
    }


    public static void ShowMessage(string msg, Action onConfirm = null, UILayer uiLayer = UILayer.Top)
    {
        var msgBox = CreateMessageBox(uiLayer);
        if(msgBox != null)
        {
            msgBox.SetMessage(msg, onConfirm);
        }
    }

    public static void ConfirmOrCancel(string msg, Action onConfirm = null, Action onCancel = null, UILayer uiLayer = UILayer.Top)
    {
        var msgBox = CreateMessageBox(uiLayer);
        if (msgBox != null)
        {
            msgBox.SetConfirmOrCancel(msg, onConfirm, onCancel);
        }
    }


    private void SetMessage(string msg, Action onConfirm)
    {
        m_CancelButton.gameObject.SetActive(false);
        m_ButtonLayout.SetLayoutHorizontal();
        SetMessageBoxData(msg, onConfirm, null);
    }


    private void SetConfirmOrCancel(string msg, Action onConfrim, Action onCancel)
    {
        m_CancelButton.gameObject.SetActive(true);
        m_ButtonLayout.SetLayoutHorizontal();
        SetMessageBoxData(msg, onConfrim, onCancel);
    }

    private void SetMessageBoxData(string msg, Action onConfrim, Action onCancel)
    {
        m_MessageText.text = msg;
        _onCofirom = onConfrim;
        _onCancel = onCancel;
    }

    private void OnConfirmBtnClick()
	{
        _onCofirom?.Invoke();
        DestroyMessageBox();
    }

    private void OnCancelBtnClick()
    {
        _onCancel?.Invoke();
        DestroyMessageBox();
    }

    private void DestroyMessageBox()
    {
        _onCofirom = null;
        _onCancel = null;
        Jyx2ResourceHelper.ReleasePrefabInstance(gameObject);
    }

    private void Update()
	{
        if (gameObject.activeSelf)
        {
            if (GamepadHelper.IsConfirm())
            {
                OnConfirmBtnClick();
            }
            else if (GamepadHelper.IsCancel())
            {
                OnCancelBtnClick();
            }
        }
	}
}
