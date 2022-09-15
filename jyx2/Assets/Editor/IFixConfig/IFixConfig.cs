using System.Collections.Generic;
using IFix;
using System;
using System.Reflection;
using System.Linq;
using UnityEditor;
using System.Text;
using UnityEngine;
using System.IO;

[Configure]
public class IFixConfig
{
    private static Assembly _mainAssembly = Assembly.Load("Assembly-CSharp");

    [IFix]
    static IEnumerable<Type> hotfix
    {
        get
        {
            var types = (from type in _mainAssembly.GetTypes()
                    where
                    (
                        type.Namespace != null &&
                        type.Namespace.StartsWith("Jyx2") &&
                        !type.IsGenericType
                    )
                    select type
                );

            return new List<Type>(types);
        }
    }

    [IFix.Filter]
    static bool Filter(System.Reflection.MethodInfo methodInfo)
    {
        return methodInfo.DeclaringType.FullName.Contains("Editor");
    }
    
    
    [MenuItem("InjectFix/Patched")]
    static void InjectfixEnable()
    {
        BuildTargetGroup buildTargetGroup = GetCurBuildTarget();
        string symbolsStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        string[] symbols = symbolsStr.Split(';');
        HashSet<string> symbolSet = new HashSet<string>();
        for(int i = 0; i < symbols.Length; ++i)
        {
            if(!symbolSet.Contains(symbols[i]))
            {
                symbolSet.Add(symbols[i]);
            }
        }
        if(!symbolSet.Contains("INJECTFIX_PATCH_ENABLE"))
        {
            symbolSet.Add("INJECTFIX_PATCH_ENABLE");
        }
        else
        {
            symbolSet.Remove("INJECTFIX_PATCH_ENABLE");
        }
        StringBuilder sb = new StringBuilder();
        foreach(string s in symbolSet)
        {
            sb.Append(s + ";");
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, sb.ToString());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static BuildTargetGroup GetCurBuildTarget()
    {
        BuildTarget buildTarget =  EditorUserBuildSettings.activeBuildTarget;
        switch(buildTarget)
        {
            case BuildTarget.Android:
            {
                return BuildTargetGroup.Android;
            }
            case BuildTarget.iOS:
            {
                return BuildTargetGroup.iOS;
            }
            default:
            {
                return BuildTargetGroup.Standalone;
            }
        }
    }

    [MenuItem("InjectFix/Patched", true)]
    static bool InjectfixEnableChecked()
    {
#if INJECTFIX_PATCH_ENABLE
        Menu.SetChecked("InjectFix/Patched", true);
#else
        Menu.SetChecked("InjectFix/Patched", false);
#endif
        return true;
    }
}
