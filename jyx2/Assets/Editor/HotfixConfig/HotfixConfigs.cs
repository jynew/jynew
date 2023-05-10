
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jyx2;
using XLua;


/// <summary>
/// 这里标记所有的可被热更新的类
/// </summary>
public static class HotfixConfigs
{
    [Hotfix]
    public static List<Type> by_field = new List<Type>()
    {
        typeof(RoleInstance),
        typeof(Jyx2LuaBridge)
    };

    [Hotfix]
    public static List<Type> by_property
    {
        get
        {
            return (from type in Assembly.Load("Assembly-CSharp").GetTypes()
                where !type.IsGenericTypeDefinition && !type.IsNested && !InBlackList(type)
                select type).ToList();
        }
    }


    /// <summary>
    /// 不上xlua修复的黑名单
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static bool InBlackList(Type type)
    {
        //这里需要屏蔽一些不支持xlua修复的第三方库或者类
        
        if (type.Namespace != null)
        {
            if (type.Namespace.StartsWith("UnityEngine")) return true;
            if (type.Namespace.StartsWith("UnityEditor")) return true;
            if (type.Namespace.StartsWith("FastWater")) return true;
            if (type.Namespace.StartsWith("StylizedWater")) return true;
            if (type.Namespace.StartsWith("Microsoft")) return true;
        }
        
        if (type.Name.StartsWith("FastWater")) return true;
        if (type.Name.StartsWith("StylizedWater")) return true;
        

        return false;
    }
}

