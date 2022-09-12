
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
                where type.Namespace == "Jyx2"
                select type).ToList();
        }
    }
}

