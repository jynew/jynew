/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using System;

/************************************************************************************************
    *  配置
    *  1、IFix、Interpret、ReverseWrapper须放到一个打了Configure标签的类里；
    *  2、IFix、Interpret、ReverseWrapper均用打了相应标签的属性来表示；
    *  3、IFix、Interpret、ReverseWrapper配置须放到Editor目录下；
*************************************************************************************************/

namespace IFix
{
    //放置配置的
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigureAttribute : Attribute
    {

    }

    //默认执行原生代码，能切换到解析执行，必须放在标记了Configure的类里
    [AttributeUsage(AttributeTargets.Property)]
    public class IFixAttribute : Attribute
    {
    }

    //生成反向（解析调用原生）封装器，加速调用性能
    [AttributeUsage(AttributeTargets.Property)]
    public class ReverseWrapperAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class FilterAttribute : Attribute
    {
    }

    public static class Configure
    {
        //
        public static Dictionary<string, List<KeyValuePair<object, int>>> GetConfigureByTags(List<string> tags)
        {
            var types = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                        from type in assembly.GetTypes()
                        where type.IsDefined(typeof(ConfigureAttribute), false)
                        select type;
            var tagsMap = tags.ToDictionary(t => t, t => new List<KeyValuePair<object, int>>());

            foreach(var type in types)
            {
                foreach (var prop in type.GetProperties(BindingFlags.Static | BindingFlags.Public
                    | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                    {
                        foreach (var ca in prop.GetCustomAttributes(false))
                        {
                            int flag = 0;
                            var fp = ca.GetType().GetProperty("Flag");
                            if (fp != null)
                            {
                                flag = (int)fp.GetValue(ca, null);
                            }
                            List<KeyValuePair<object, int>> infos;
                            if (tagsMap.TryGetValue(ca.GetType().ToString(), out infos))
                            {
                                foreach (var applyTo in prop.GetValue(null, null) as IEnumerable)
                                {
                                    infos.Add(new KeyValuePair<object, int>(applyTo, flag));
                                }
                            }
                        }
                    }
                }
            }
            return tagsMap;
        }

        public static List<MethodInfo> GetFilters()
        {
            var types = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                        from type in assembly.GetTypes()
                        where type.IsDefined(typeof(ConfigureAttribute), false)
                        select type;

            List<MethodInfo> filters = new List<MethodInfo>();
            foreach (var type in types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public
                    | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if(method.IsDefined(typeof(IFix.FilterAttribute), false))
                    {
                        filters.Add(method);
                    }
                }
            }
            return filters;
        }

        public static IEnumerable<MethodInfo> GetTagMethods(Type tagType, string searchAssembly)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                        && (assembly.GetName().Name == searchAssembly)
                    where assembly.CodeBase.IndexOf("ScriptAssemblies") != -1
                    from type in assembly.GetTypes()
                    from method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                        | BindingFlags.NonPublic)
                    where method.IsDefined(tagType, false)
                    select method);
        }

        public static IEnumerable<FieldInfo> GetTagFields(Type tagType, string searchAssembly)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                        && (assembly.GetName().Name == searchAssembly)
                    where assembly.CodeBase.IndexOf("ScriptAssemblies") != -1
                    from type in assembly.GetTypes()
                    from field in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                        | BindingFlags.NonPublic)
                    where field.IsDefined(tagType, false)
                    select field);
        }

        public static IEnumerable<PropertyInfo> GetTagProperties(Type tagType, string searchAssembly)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                        && (assembly.GetName().Name == searchAssembly)
                    where assembly.CodeBase.IndexOf("ScriptAssemblies") != -1
                    from type in assembly.GetTypes()
                    from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                        | BindingFlags.NonPublic)
                    where property.IsDefined(tagType, false)
                    select property);
        }

        public static IEnumerable<Type> GetTagClasses(Type tagType, string searchAssembly)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                        && (assembly.GetName().Name == searchAssembly)
                    where assembly.CodeBase.IndexOf("ScriptAssemblies") != -1
                    from type in assembly.GetTypes()
                    where type.IsDefined(tagType, false)
                    select type
                    );

        }
    }
}
