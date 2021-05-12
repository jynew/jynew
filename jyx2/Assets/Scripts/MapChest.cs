using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图宝箱
/// </summary>
public class MapChest : MonoBehaviour
{
    [Header("用来标识宝箱的唯一ID，全局唯一")]
    public string ID = System.Guid.NewGuid().ToString();

    [Header("宝箱的打开状态，是直接消失还是变为打开")]
    public MapChestOpenDisplayType displayType = MapChestOpenDisplayType.Hide;

    public enum MapChestOpenDisplayType
    {
        Hide, //直接消失
        SetOpened, //变为打开状态
    }


    GameRuntimeData runtime { get { return GameRuntimeData.Instance; } }

    public string GetRuntimeKey()
    {
        return "chest." + ID;
    }

    public void Init()
    {
        RefreshOpenStates();
    }

    public void MarkAsOpened()
    {
        runtime.SetKeyValues(GetRuntimeKey(), "1");
        RefreshOpenStates();
    }

    void RefreshOpenStates()
    {
        int state = GetState();
        //没打开
        if (state == 0)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }else if(state == 1)
        {
            if(displayType == MapChestOpenDisplayType.Hide)
            {
                GetComponent<MeshRenderer>().enabled = false;
            }else if(displayType == MapChestOpenDisplayType.SetOpened)
            {

            }
        }
    }

    //获得宝箱状态
    int GetState()
    {
        string pk = GetRuntimeKey();
        if (!runtime.KeyExist(pk)) return 0;
        return int.Parse(runtime.GetKeyValues(pk));
    }
}
