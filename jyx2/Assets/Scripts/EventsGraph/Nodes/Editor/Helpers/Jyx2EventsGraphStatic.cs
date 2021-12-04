using System;
using System.Collections;
using System.Collections.Generic;

using Jyx2;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;


static public class Jyx2EventsGraphStatic 
{
    static Jyx2EventsGraphStatic()
    {
        var tmp = new List<string>();

        var roles = AssetDatabase.LoadAllAssetsAtPath("Assets/BuildSource/Configs/Characters");
        foreach (var r in roles)
        {
            tmp.Add(r.name);
        }
        s_roleList = tmp.ToArray();
        tmp.Clear();
        
        foreach (var item in AssetDatabase.LoadAllAssetsAtPath("Assets/BuildSource/Configs/Items"))
        {
            tmp.Add(item.name);
        }
        s_itemList = tmp.ToArray();

        tmp.Clear();
        
        foreach (var item in AssetDatabase.LoadAllAssetsAtPath("Assets/BuildSource/Configs/Skills"))
        {
            tmp.Add(item.name);
        }
        s_skillList = tmp.ToArray();
        tmp.Clear();
        foreach (var item in AssetDatabase.LoadAllAssetsAtPath("Assets/BuildSource/Configs/Maps"))
        {
            tmp.Add(item.name);
        }
        s_sceneList = tmp.ToArray();
    }

    public static string[] s_roleList;
    public static string[] s_itemList;
    public static string[] s_skillList;
    public static string[] s_sceneList;
}
