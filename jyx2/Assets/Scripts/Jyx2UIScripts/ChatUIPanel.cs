/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2;
using HSFrameWork.ConfigTable;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public enum ChatType 
{
    None = -1,
    RoleId = 1,
    Selection = 2,
}
public partial class ChatUIPanel : Jyx2_UIBase,IUIAnimator
{
    public override UILayer Layer => UILayer.NormalUI;
    public override bool IsOnly => true;

    Action _callback;
    ChatType _currentShowType = ChatType.None;
    string _currentText;//存一下要显示的文字 当文字要显示的时候 用一个指针显示当前显示到的索引 分多次显示，点击显示接下来的
    int _currentShowIndex = 0;
    protected override void OnCreate()
    {
        InitTrans();

        StorySelectionItem_Button.gameObject.SetActive(false);
        MainBg_Button.onClick.AddListener(OnMainBgClick);

        InitPanelTrigger();
    }

    void OnMainBgClick() 
    {
        if (_currentShowType == ChatType.None)
            return;
        if (_currentShowType == ChatType.Selection)
            return;
        ShowText();
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        ChatType _type = (ChatType)allParams[0];
        _currentShowType = _type;
        _currentShowIndex = 0;
        switch (_type) 
        {
            case ChatType.RoleId:
                Show((int)allParams[1], (string)allParams[2], (int)allParams[3], (Action)allParams[4]);
                break;
            case ChatType.Selection:
                ShowSelection((string)allParams[1], (string)allParams[2], (List<string>)allParams[3], (Action<int>)allParams[4]);
                break;
        }

        //临时将触发按钮隐藏
        var panel = FindObjectOfType<InteractUIPanel>();
        if (panel != null && panel.gameObject.activeSelf)
        {
            _interactivePanel = panel.gameObject;
            panel.gameObject.SetActive(false);
        }

        //Jyx2_UIManager.Instance.SetMainUIActive(false);
    }


    private GameObject _interactivePanel = null;

    private void ShowCharacter(string roleHeadPath,int roleId)
    {
        ChangePosition(roleId);
        if (string.IsNullOrEmpty((roleHeadPath)))
        {
            RoleHeadImage_Image.gameObject.SetActive(false);
        }
        else
        {
            RoleHeadImage_Image.gameObject.SetActive(false);
            Jyx2ResourceHelper.GetRoleHeadSprite(roleHeadPath, (head) =>
            {
                RoleHeadImage_Image.gameObject.SetActive(true);
                RoleHeadImage_Image.sprite = head;
            });
        }
    }

    //根据对话框最大显示字符以及标点断句分段显示对话 by eaphone at 2021/6/12
    void ShowText() 
    {
        if (_currentShowIndex >= _currentText.Length - 1) 
        {
            Jyx2_UIManager.Instance.HideUI(nameof(ChatUIPanel));
            _callback?.Invoke();
            _callback = null;

            if (_interactivePanel)
            {
                _interactivePanel.SetActive(true);
                _interactivePanel = null;
            }
            
            return;
        }
		var finalS=_currentText;
		if(_currentText.Length>GameConst.MAX_CHAT_CHART_NUM){
			int preIndex = _currentShowIndex;
			string[] sList=_currentText.Substring(preIndex,_currentText.Length - preIndex).Split(new char[]{'！','？','，','　'},StringSplitOptions.RemoveEmptyEntries);//暂时不对,'．'进行分割，不然对话中...都会被去除掉
			var tempIndex=0;
			foreach(var i in sList){
				var tempNum=i.Length+1;//包含分隔符
				if(tempIndex+tempNum<GameConst.MAX_CHAT_CHART_NUM){
					tempIndex+=tempNum;
					_currentShowIndex+=tempNum;
					continue;
				}
				break;
			}
			_currentShowIndex = Mathf.Clamp(_currentShowIndex, 0, _currentText.Length);
			finalS=_currentText.Substring(preIndex, _currentShowIndex - preIndex);
		}else{
			_currentShowIndex = _currentText.Length;
		}
		MainContent_Text.text = finalS;
    }

    public void Show(int roleId, string msg, int type, Action callback)
    {
        _currentText = $"{msg}";
        _callback = callback;
        SelectionPanel_RectTransform.gameObject.SetActive(false);

        HeadAvataPre_RectTransform.gameObject.SetActive(!(type == 2 || type == 3));

        //不显示人物
        if (type == 2 || type == 3)
        {
            ChangePosition(1,false);
            RoleHeadImage_Image.gameObject.SetActive(false);
        }
        else
        {
            var headMapping = ConfigTable.Get<Jyx2RoleHeadMapping>(roleId);
            if (headMapping != null && !string.IsNullOrEmpty(headMapping.HeadAvata))
            {
                ShowCharacter(headMapping.HeadAvata, roleId);
                //RoleHeadImage_Image.gameObject.SetActive(true);
            }
            else
            {
                RoleHeadImage_Image.gameObject.SetActive(false);
            }
        }
        ShowText();
    }
    //根据角色ID修改左右位置
    public void ChangePosition(int roleId, bool ShowName = true)
    {
        Name_RectTransform.gameObject.SetActive(ShowName);
        kuang_RectTransform.gameObject.SetActive(ShowName);
        if (ShowName)
        {
            var role = ConfigTable.Get<Jyx2RoleHeadMapping>(roleId);
            if (roleId == 0 && GameRuntimeData.Instance.Player != null)
            {
                NameTxt_Text.text = GameRuntimeData.Instance.Player.Name;
            }
            else
            {
                NameTxt_Text.text = role.ModelAsset;
            }
        }


        Content_RectTransform.anchoredPosition = roleId == 0 || !ShowName ? Vector3.zero : new Vector3(450, 0, 0);

        Content_RectTransform.sizeDelta = ShowName ? new Vector2(-450, 280) : new Vector2(0, 280);


        HeadAvataPre_RectTransform.anchorMax = roleId == 0 ? Vector2.right : Vector2.zero;
        HeadAvataPre_RectTransform.anchorMin = roleId == 0 ? Vector2.right : Vector2.zero;
        HeadAvataPre_RectTransform.pivot = roleId == 0 ? Vector2.right : Vector2.zero;
        HeadAvataPre_RectTransform.anchoredPosition = Vector3.zero;

        kuang_RectTransform.anchorMax = roleId == 0 ? Vector2.right : Vector2.zero;
        kuang_RectTransform.anchorMin = roleId == 0 ? Vector2.right : Vector2.zero;
        kuang_RectTransform.pivot = roleId == 0 ? Vector2.right : Vector2.zero;
        kuang_RectTransform.anchoredPosition = Vector3.zero;

        Name_RectTransform.anchorMax = roleId == 0 ? Vector2.right : Vector2.zero;
        Name_RectTransform.anchorMin = roleId == 0 ? Vector2.right : Vector2.zero;
        Name_RectTransform.pivot = roleId == 0 ? Vector2.right : Vector2.zero;
        Name_RectTransform.anchoredPosition = new Vector2(roleId == 0 ? -450 : 450 , 280); 
    }
    
    public void ShowSelection(string roleName, string msg, List<string> selectionContent, Action<int> callback)
    {

        //没有Player
        if (roleName == "主角" && GameRuntimeData.Instance.Player != null)
        {
            ShowCharacter(GameRuntimeData.Instance.Player.HeadAvata,0);
            MainContent_Text.text = $"{msg}";
        }
        else
        {
            Jyx2Role role = ConfigTable.GetAll<Jyx2Role>().First(r => r.Name == roleName);
            
            //没有定义Role或者HeadAvata
            if (role == null )
            {
                MainContent_Text.text = $"{roleName}：{msg}";
                RoleHeadImage_Image.gameObject.SetActive(false);
            }
            else
            {
                var headMapping = ConfigTable.Get<Jyx2RoleHeadMapping>(role.Id);
                ShowCharacter(headMapping.HeadAvata,1);
                MainContent_Text.text = $"{role.Name}：{msg}";
            }
        }

        ClearChildren(Container_RectTransform.transform);
        for (int i = 0; i < selectionContent.Count; i++)
        {
            int currentIndex = i;
            Button selectionItem = Instantiate(StorySelectionItem_Button);
            selectionItem.gameObject.SetActive(true);
            selectionItem.transform.Find("Text").GetComponent<Text>().text = selectionContent[i];
            selectionItem.transform.SetParent(Container_RectTransform, false);
            BindListener(selectionItem, delegate
            {
                Jyx2_UIManager.Instance.HideUI(nameof(ChatUIPanel));
                callback?.Invoke(currentIndex);
            });
        }
        SelectionPanel_RectTransform.gameObject.SetActive(true);
    }

    public void DoShowAnimator()
    {
        //Content_RectTransform.anchoredPosition = Vector2.zero;
        //allTweenList.Add(Content_RectTransform.DOAnchorPosY(130, 0.5f));
        //HeadAvataPre_RectTransform.anchoredPosition = new Vector2(-300, 300);
        //allTweenList.Add(HeadAvataPre_RectTransform.DOAnchorPosX(300, 0.5f));
    }

    public void DoHideAnimator()
    {
        
    }

    private void InitPanelTrigger()
    {
        List<EventTrigger.Entry> entries = Panel_Trigger.triggers;
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].eventID == EventTriggerType.PointerClick)
            {
                entries[i].callback = new EventTrigger.TriggerEvent();
                entries[i].callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>((BaseEventData) =>
                {
                    OnMainBgClick();
                }));
                break;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) OnMainBgClick();
    }
}
