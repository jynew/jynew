using Jyx2;
using HSFrameWork.ConfigTable;
using HSUI;
using Jyx2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class DialogPanelUI : BaseUI {
    
    public Image roleHeadImage;
    public Text message;
    public GameObject SelectionPanel;
    public Transform SelectionPanelContainer;
    public Button SelectionItem;

    Action _callback;

    public void Show(int roleId, string msg, int type, Action callback)
    {
        message.text = $"{msg}";
        _callback = callback;
        SelectionPanel.SetActive(false);
        gameObject.SetActive(true);
        
        //不显示人物
        if (type == 2 || type == 3)
        {
            roleHeadImage.gameObject.SetActive(false);
        }
        else
        {
            var headMapping = ConfigTable.Get<Jyx2RoleHeadMapping>(roleId);
            if (headMapping != null && !string.IsNullOrEmpty(headMapping.HeadAvata))
            {
                ShowCharacter(headMapping.HeadAvata);
                roleHeadImage.gameObject.SetActive(true);
            }
            else
            {
                roleHeadImage.gameObject.SetActive(false);
            }
        }
    }

    public void Show(string roleKey, string msg, Action callback)
    {
        Role role = Role.Get(roleKey);
        //没有定义Role或者HeadAvata
        if (role == null || string.IsNullOrEmpty(role.HeadAvata))
        {
            message.text = $"{roleKey}：{msg}";
            roleHeadImage.gameObject.SetActive(false);
        }
        else
        {
            //没有Player
            if (roleKey == "主角" && GameRuntimeData.Instance.Player != null)
            {
                ShowCharacter(GameRuntimeData.Instance.Player.HeadAvata);
                message.text = $"{GameRuntimeData.Instance.Player.Name}:{msg}";
            }
            else
            {
                ShowCharacter(role.HeadAvata);
                message.text = $"{role.Name}：{msg}";
            }
            roleHeadImage.gameObject.SetActive(true);
        }
        gameObject.SetActive(true);
        SelectionPanel.SetActive(false);
        _callback = callback;
    }

    public void ShowSelection(string roleKey, string msg, List<string> selectionContent, Action<int> callback)
    {
        Role role = Role.Get(roleKey);
        //没有定义Role或者HeadAvata
        if (role == null || string.IsNullOrEmpty(role.HeadAvata))
        {
            message.text = $"{roleKey}：{msg}";
            roleHeadImage.gameObject.SetActive(false);
        }
        else
        {
            //没有Player
            if (roleKey == "主角" && GameRuntimeData.Instance.Player != null)
            {
                ShowCharacter(GameRuntimeData.Instance.Player.HeadAvata);
                message.text = $"{GameRuntimeData.Instance.Player.Name}:{msg}";
            }
            else
            {
                ShowCharacter(role.HeadAvata);
                message.text = $"{role.Name}：{msg}";
            }
            roleHeadImage.gameObject.SetActive(true);
        }
        ClearChildren(SelectionPanelContainer);
        for(int i = 0; i < selectionContent.Count; i++)
        {
            int currentIndex = i;
            Button selectionItem = Instantiate(SelectionItem);
            selectionItem.transform.Find("Text").GetComponent<Text>().text = selectionContent[i];
            selectionItem.transform.SetParent(SelectionPanelContainer, false);
            BindListener(selectionItem, delegate
            {
                this.gameObject.SetActive(false);
                callback?.Invoke(currentIndex);
            });
        }
        gameObject.SetActive(true);
        SelectionPanel.SetActive(true);
    }

    public void OnCallback()
    {
        gameObject.SetActive(false);
        _callback?.Invoke();
    }
    
    private void ShowCharacter(string roleHeadPath)
    {
        if (string.IsNullOrEmpty((roleHeadPath)))
        {
            roleHeadImage.gameObject.SetActive(false);
        }
        else
        {
            roleHeadImage.gameObject.SetActive(true);
            Jyx2ResourceHelper.GetRoleHeadSprite(roleHeadPath, roleHeadImage);
        }
    }

}
