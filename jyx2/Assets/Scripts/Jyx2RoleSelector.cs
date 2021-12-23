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
using Jyx2.Middleware;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jyx2RoleSelector : MonoBehaviour
{

    public static Jyx2RoleSelector Create(IEnumerable<RoleInstance> roles, Func<RoleInstance, bool> mustRoles, Action<List<RoleInstance>> callback)
    {
        var prefab = Jyx2ResourceHelper.GetCachedPrefab("Jyx2RoleSelector");
        var obj = Instantiate(prefab);
        var parent = GameObject.Find("MainUI").transform;
        obj.transform.SetParent(parent);

        var rt = obj.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;

        var panel = obj.GetComponent<Jyx2RoleSelector>();
        panel.Show(roles, mustRoles, callback);
        return panel;
    }

    public Text info;
    public Button confirmButton;
    public Button cancelButton;
    public Transform m_Container;

    Action<List<RoleInstance>> _callback;
    Func<RoleInstance, bool> _mustRolesFunc;

    public void Show(IEnumerable<RoleInstance> roles, Func<RoleInstance, bool> mustRoles, Action<List<RoleInstance>> callback)
    {
        HSUnityTools.DestroyChildren(m_Container);
        _mustRolesFunc = mustRoles;
        _callback = callback;
        foreach (var role in roles)
        {
            AddRole(role);
        }
    }

    void AddRole(RoleInstance role)
    {
        bool isMust = (_mustRolesFunc != null && _mustRolesFunc(role));

        var roleItemUI = Jyx2RoleHeadUI.Create(role, isMust, ()=> { info.text = role.Name; });
        roleItemUI.transform.SetParent(m_Container);

    }

    void Start()
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(OnCancel);
    }

    void OnConfirm()
    {
        List<RoleInstance> rst = new List<RoleInstance>();
        for(int i = 0; i < m_Container.childCount; ++i)
        {
            var child = m_Container.GetChild(i);
            Jyx2RoleHeadUI roleHeadUI = child.GetComponent<Jyx2RoleHeadUI>();
            if (roleHeadUI == null) continue;
            if (roleHeadUI.IsChecked)
            {
                var role = roleHeadUI.GetRole();
                rst.Add(role);
                Debug.Log(role.Key);
            }
            
        }
        _callback(rst);

        Destroy(this.gameObject);
    }

    void OnCancel()
    {
        GameUtil.DisplayPopinfo("不能取消");
    }
}
