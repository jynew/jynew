using HanSquirrel.ResourceManager;
using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jyx2RoleHeadUI : MonoBehaviour
{

    public static Jyx2RoleHeadUI Create(RoleInstance role, bool forceChecked, Action clickCallback)
    {
        var prefab = Jyx2ResourceHelper.GetCachedPrefab("Assets/Prefabs/Jyx2RoleHeadUI.prefab");

        var obj = Instantiate(prefab);
        var itemUI = obj.GetComponent<Jyx2RoleHeadUI>();
        itemUI.Show(role, forceChecked, clickCallback);
        return itemUI;
    }

    public bool ForceChecked = false;


    public void Show(RoleInstance role, bool forceChecked, Action clickCallback)
    {
        ForceChecked = forceChecked;
        _role = role;
        _clickCallback = clickCallback;

        Jyx2ResourceHelper.GetRoleHeadSprite(role, headImage);

        IsChecked = forceChecked;
    }

    //选中和反选
    public void OnClick()
    {
        if(_clickCallback != null)
        {
            _clickCallback();
        }
        if (ForceChecked)
        {
            GameUtil.DisplayPopinfo("此角色强制上场");
            return;
        }
        
        IsChecked = !IsChecked;
    }

    Action _clickCallback;
    public Image headImage;
    public Image checkImage;
    public Button btn;
    RoleInstance _role;

    public RoleInstance GetRole() { return _role; }
    
    public bool IsChecked
    {
        get { return _isChecked; }
        set
        {
            _isChecked = value;
            checkImage.color = value ? Color.red : Color.white;
        }
    }
    bool _isChecked = false;

    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(OnClick);
    }

}
