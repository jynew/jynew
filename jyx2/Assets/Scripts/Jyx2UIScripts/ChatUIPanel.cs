using Jyx2;
using HSFrameWork.ConfigTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.AddressableAssets;

public enum ChatType 
{
    None = -1,
    RoleKey = 0,
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
            case ChatType.RoleKey:
                Show((string)allParams[1], (string)allParams[2], (Action)allParams[3]);
                break;
            case ChatType.Selection:
                ShowSelection((string)allParams[1], (string)allParams[2], (List<string>)allParams[3], (Action<int>)allParams[4]);
                break;
        }
        //Jyx2_UIManager.Instance.SetMainUIActive(false);
    }

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

    void ShowText() 
    {
        if (_currentShowIndex >= _currentText.Length - 1) 
        {
            Jyx2_UIManager.Instance.HideUI("ChatUIPanel");
            _callback?.Invoke();
            _callback = null;
            return;
        }
        int preIndex = _currentShowIndex;
        _currentShowIndex += 10000;
        _currentShowIndex = Mathf.Clamp(_currentShowIndex, 0, _currentText.Length - 1);
        MainContent_Text.text = _currentText.Substring(preIndex, _currentShowIndex - preIndex);
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
        



        Content_RectTransform.anchoredPosition = roleId == 0 ? Vector3.zero : new Vector3(450, 0, 0);
        Content_RectTransform.sizeDelta = new Vector2(-450, 280);


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



    public void Show(string roleKey, string msg, Action callback)
    {
        Role role = Role.Get(roleKey);
        //没有定义Role或者HeadAvata
        if (role == null || string.IsNullOrEmpty(role.HeadAvata))
        {
            ChangePosition(1);
            _currentText = $"{roleKey}：{msg}";
            HeadAvataPre_RectTransform.gameObject.SetActive(false);
        }
        else
        {
            //没有Player
            if (roleKey == "主角" && GameRuntimeData.Instance.Player != null)
            {
             
                ShowCharacter(GameRuntimeData.Instance.Player.HeadAvata,0);
                _currentText = $"{GameRuntimeData.Instance.Player.Name}:{msg}";
            }
            else
            {
                ChangePosition(1);
                ShowCharacter(role.HeadAvata,1);
                _currentText = $"{role.Name}：{msg}";
            }
        }
        SelectionPanel_RectTransform.gameObject.SetActive(false);
        _callback = callback;
        ShowText();
    }

    public void ShowSelection(string roleKey, string msg, List<string> selectionContent, Action<int> callback)
    {

        //没有Player
        if (roleKey == "主角" && GameRuntimeData.Instance.Player != null)
        {
            ShowCharacter(GameRuntimeData.Instance.Player.HeadAvata,0);
            MainContent_Text.text = $"{msg}";
        }
        else
        {
            Role role = Role.Get(roleKey);
            //没有定义Role或者HeadAvata
            if (role == null || string.IsNullOrEmpty(role.HeadAvata))
            {
                MainContent_Text.text = $"{roleKey}：{msg}";
                RoleHeadImage_Image.gameObject.SetActive(false);
            }
            else
            {
                ShowCharacter(role.HeadAvata,1);
                MainContent_Text.text = $"{role.Name}：{msg}";
            }
        }

        //Role role = Role.Get(roleKey);
        ////没有定义Role或者HeadAvata
        //if (role == null || string.IsNullOrEmpty(role.HeadAvata))
        //{
        //    MainContent_Text.text = $"{roleKey}：{msg}";
        //    RoleHeadImage_Image.gameObject.SetActive(false);
        //}
        //else
        //{
        //    //没有Player
        //    if (roleKey == "主角" && GameRuntimeData.Instance.Player != null)
        //    {
        //        ShowCharacter(GameRuntimeData.Instance.Player.HeadAvata);
        //        MainContent_Text.text = $"{GameRuntimeData.Instance.Player.Name}:{msg}";
        //    }
        //    else
        //    {
        //        ShowCharacter(role.HeadAvata);
        //        MainContent_Text.text = $"{role.Name}：{msg}";
        //    }
        //}
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
                Jyx2_UIManager.Instance.HideUI("ChatUIPanel");
                callback?.Invoke(currentIndex);
            });
        }
        SelectionPanel_RectTransform.gameObject.SetActive(true);
    }

    List<Tweener> allTweenList = new List<Tweener>();
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

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        //Jyx2_UIManager.Instance.SetMainUIActive(true);
        foreach (var item in allTweenList)
        {
            item.Complete();
            item.Kill();
        }
        allTweenList.Clear();
    }
}
