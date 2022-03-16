using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Jyx2;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;


static public class Jyx2EventsGraphStatic 
{
    static Jyx2EventsGraphStatic()
    {
        var tmp = new List<string>();

        string tempPath = "Assets/BuildSource/Configs/Characters/";
        DirectoryInfo direction = new DirectoryInfo(tempPath);
        if (direction.Exists)
        {
            FileInfo[] fileInfos = direction.GetFiles("*.asset", SearchOption.TopDirectoryOnly);
            foreach (var item in fileInfos)
            {
                
                tmp.Add(item.Name.Replace(".asset",""));
            }
        }
        s_roleList = tmp.ToArray();
        tmp.Clear();

        tempPath = "Assets/BuildSource/Configs/Items/";
        direction = new DirectoryInfo(tempPath);
        if (direction.Exists)
        {
            FileInfo[] fileInfos = direction.GetFiles("*.asset", SearchOption.TopDirectoryOnly);
            foreach (var item in fileInfos)
            {
                tmp.Add(item.Name.Replace(".asset", ""));
            }
        }
        s_itemList = tmp.ToArray();
        tmp.Clear();
        
        tempPath = "Assets/BuildSource/Configs/Skills/";
        direction = new DirectoryInfo(tempPath);
        if (direction.Exists)
        {
            FileInfo[] fileInfos = direction.GetFiles("*.asset", SearchOption.TopDirectoryOnly);
            foreach (var item in fileInfos)
            {
                tmp.Add(item.Name.Replace(".asset", ""));
            }
        }
        s_skillList = tmp.ToArray();
        tmp.Clear();


        tempPath = "Assets/BuildSource/Configs/Maps/";
        direction = new DirectoryInfo(tempPath);
        if (direction.Exists)
        {
            FileInfo[] fileInfos = direction.GetFiles("*.asset", SearchOption.TopDirectoryOnly);
            foreach (var item in fileInfos)
            {
                tmp.Add(item.Name.Replace(".asset", ""));
            }
        }
        s_sceneList = tmp.ToArray();

    }

    public static string[] s_roleList;
    public static string[] s_itemList;
    public static string[] s_skillList;
    public static string[] s_sceneList;
}
