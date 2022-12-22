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
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : Jyx2_UIBase
{
    [SerializeField]
    private Button m_ConfirmButton;

    [SerializeField]
    private Button m_CancelButton;

    [SerializeField]
    private LayoutGroup m_ButtonLayout;

    [SerializeField]
    private Text m_MessageText;

    private Action _onConfirm;
    private Action _onCancel;
    private Action _onClose;

    public override UILayer Layer => UILayer.Top;


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
        var ui = Jyx2_UIManager.Instance.ShowUI<MessageBox>();
        var layer = Jyx2_UIManager.Instance.GetUIParent(uiLayer);
        if(layer != null)
        {
            ui.transform.SetParent(layer, false);
        }
        return ui;
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

    /// <summary>
    /// Lua那边的协程要通过OnClose回调Resume回去
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="OnClose"></param>
    private void SetMessage(string msg, Action OnClose)
    {
        m_CancelButton.gameObject.SetActive(false);
        m_ButtonLayout.SetLayoutHorizontal();
        SetMessageBoxData(msg, null, null, OnClose);
    }


    private void SetConfirmOrCancel(string msg, Action onConfirm, Action onCancel)
    {
        m_CancelButton.gameObject.SetActive(true);
        m_ButtonLayout.SetLayoutHorizontal();
        SetMessageBoxData(msg, onConfirm, onCancel);
    }

    private void SetMessageBoxData(string msg, Action onConfirm, Action onCancel, Action onClose = null)
    {
        m_MessageText.text = msg;
        _onConfirm = onConfirm;
        _onCancel = onCancel;
        _onClose = onClose;
    }

    private void OnConfirmBtnClick()
	{
        _onConfirm?.Invoke();
        DestroyMessageBox();
    }

    private void OnCancelBtnClick()
    {
        _onCancel?.Invoke();
        DestroyMessageBox();
    }

    private void DestroyMessageBox()
    {
        _onClose?.Invoke();
        _onClose = null;
        _onConfirm = null;
        _onCancel = null;
        Jyx2_UIManager.Instance.HideUI<MessageBox>();
    }
}
