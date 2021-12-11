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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jyx2RoleHeadUI : MonoBehaviour
{

    public static Jyx2RoleHeadUI Create(RoleInstance role, bool forceChecked, Action clickCallback)
    {
        var prefab = Jyx2ResourceHelper.GetCachedPrefab("Jyx2RoleHeadUI");

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

        headImage.LoadAsyncForget(role.Data.GetPic());
        
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
