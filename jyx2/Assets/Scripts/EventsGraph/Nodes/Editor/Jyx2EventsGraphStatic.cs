using System.Collections;
using System.Collections.Generic;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEngine;

static public class Jyx2EventsGraphStatic 
{
    static Jyx2EventsGraphStatic()
    {
        if (!ConfigTable.IsInited())
        {
            ConfigTable.InitSync();
        }
            
        var tmp = new List<string>();
        foreach (var role in ConfigTable.GetAll<Jyx2Role>())
        {
            tmp.Add(role.Name);
        }
        s_roleList = tmp.ToArray();

        tmp.Clear();
        foreach (var item in ConfigTable.GetAll<Jyx2Item>())
        {
            tmp.Add(item.Name);
        }
        s_itemList = tmp.ToArray();
    }

    public static string[] s_roleList;
    public static string[] s_itemList;
}
