using HanSquirrel.ResourceManager;
using Jyx2;
using System;
using UnityEngine;

public static class RoleHelper
{
    /// <summary>
    /// 寻找主角
    /// </summary>
    /// <returns></returns>
    public static MapRole FindPlayer()
    {
        var obj = GameObject.Find("Level/Player");
        if (obj != null) return obj.GetComponent<MapRole>();
        obj = GameObject.FindWithTag("Player");
        if (obj == null)
        {
            //Debug.LogError("找不到主角，请设置层级为Level/Player。或者Tag设置为Player");
            return null;
        }
        return obj.GetComponent<MapRole>();
    }

    /// <summary>
    /// 通过已有数据实例在地图中创建角色
    /// </summary>
    /// <param name="role"></param>
    public static MapRole CreateRoleView(this RoleInstance role, string tag = "NPC")
    {
        var roleViewPre = Jyx2ResourceHelper.GetCachedPrefab("Assets/Prefabs/MapRole.prefab");
        var roleView = GameObject.Instantiate(roleViewPre).GetComponent<MapRole>();
        role.View = roleView;
        roleView.DataInstance = role;
        roleView.m_RoleKey = role.Key;
        roleView.name = role.Key;
        roleView.tag = tag;
        if (MapRuntimeData.Instance != null) MapRuntimeData.Instance.AddMapRole(role);
        return roleView;
    }

    /// <summary>
    /// 通过角色Key创建角色数据示例
    /// </summary>
    public static RoleInstance CreateRoleInstance(string roleKey)
    {
        RoleInstance role = new RoleInstance(roleKey);
        if (MapRuntimeData.Instance != null) MapRuntimeData.Instance.AddMapRole(role);
        return role;
    }

    /// <summary>
    /// 地图角色绑定新的数据实例
    /// </summary>
    /// <param name="roleKey"></param>
    /// <param name="roleView"></param>
    public static void CreateRoleInstance(this MapRole roleView, string roleKey)
    {
        roleView.BindRoleInstance(new RoleInstance(roleKey));
        roleView.DataInstance.Hp = roleView.DataInstance.MaxHp; //默认满血
    }

    /// <summary>
    /// 地图角色绑定已有数据实例
    /// </summary>
    /// <param name="roleView"></param>
    /// <param name="role"></param>
    public static void BindRoleInstance(this MapRole roleView, RoleInstance role, Action callback = null)
    {
        if (role == null || roleView == null)
            return;

        //已经绑定过了，不需要再行绑定了。
        if (role.View == roleView && roleView.DataInstance == role)
            return;
        
        role.View = roleView;
        roleView.ForceSetAnimator(null);
        roleView.DataInstance = role;

        //JYX2 不刷新临时NPC外观
        if(roleView.m_RoleKey != "testman")
        {
            roleView.RefreshModel(callback);
        }
        else
        {
            if(callback != null)
                callback();
        }
    }
}

