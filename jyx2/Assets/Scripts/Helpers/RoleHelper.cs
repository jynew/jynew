/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using HanSquirrel.ResourceManager;
using Jyx2;
using System;
using Cysharp.Threading.Tasks;
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
        return roleView;
    }
    
    /// <summary>
    /// 地图角色绑定新的数据实例
    /// </summary>
    /// <param name="roleKey"></param>
    /// <param name="roleView"></param>
    public static void CreateRoleInstance(this MapRole roleView, string roleKey)
    {
        roleView.BindRoleInstance(new RoleInstance(roleKey)).Forget();
        roleView.DataInstance.Hp = roleView.DataInstance.MaxHp; //默认满血
    }

    /// <summary>
    /// 地图角色绑定已有数据实例
    /// </summary>
    /// <param name="roleView"></param>
    /// <param name="role"></param>
    public static async UniTask BindRoleInstance(this MapRole roleView, RoleInstance role)
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
            await roleView.RefreshModel();
        }
    }
}

