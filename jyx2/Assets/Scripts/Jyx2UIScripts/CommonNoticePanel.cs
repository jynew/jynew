using i18n.TranslatorDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class CommonNoticePanel:Jyx2_UIBase
{

    private Action OnBtn1Click;

    private Action OnBtn2Click;

    protected override void OnCreate()
    {
        InitTrans();
        BindListener(CloseBtn_Button, OnCloseBtnClick);
        BindListener(FirstBtn_Button, OnFirstBtnClick);
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        OnBtn1Click = null;
        OnBtn2Click = null;
    }

    public string Content => ContentText_Text.text;
    

    public void SetData(string title, string content, Action OnBtn1Action = null, Action OnBtn2Action = null, string btn1Text = "", string btn2Text = "")
    {
        TitleText_Text.text = title;
        ContentText_Text.text = content;
        OnBtn1Click = OnBtn1Action;
        OnBtn2Click = OnBtn2Action;
        if (string.IsNullOrEmpty(btn1Text))
            btn1Text = "确认".GetContent(nameof(CommonNoticePanel));
        if (string.IsNullOrEmpty(btn2Text))
            btn2Text = "关闭".GetContent(nameof(CommonNoticePanel));
        FirstBtn_Text_Text.text = btn1Text;
        CloseBtn_Text_Text.text = btn2Text;
        TextView_ScrollRect.verticalNormalizedPosition = 1;
    }

    private void OnFirstBtnClick()
    {
        OnBtn1Click?.Invoke();
    }

    public void OnCloseBtnClick()
    {
        OnBtn2Click?.Invoke();
        Hide();
    }

    public static async void ShowNotice(string title, string content, Action btn1Action = null, Action btn2Action = null, string btn1Text = "", string btn2Text = "")
    {
        var ui = await Jyx2_UIManager.Instance.ShowUIAsync<CommonNoticePanel>();
        if(ui != null)
        {
            ui.SetData(title, content, btn1Action, btn2Action, btn1Text, btn2Text);
        }
    }
}
