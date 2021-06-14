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

	public bool isLock;

    public enum MapChestOpenDisplayType
    {
        Hide, //直接消失
        SetOpened, //变为打开状态
    }


    GameRuntimeData runtime
    {
        get { return GameRuntimeData.Instance; }
    }

    private MapChestInteract m_MapChestInteract;

    private string GetRuntimeKey()
    {
        return "chest." + ID;
    }

    public void Init()
    {
        m_MapChestInteract = this.gameObject.GetComponent<MapChestInteract>();
        RefreshOpenStates();
    }

    public void MarkAsOpened()
    {
		if(!isLock){
			runtime.SetKeyValues(GetRuntimeKey(), "1");
			//播放动画
			m_MapChestInteract.Open(RefreshOpenStates);
		}
    }

    /// <summary>
    /// 刷新宝箱状态
    /// </summary>
    void RefreshOpenStates()
    {
        int state = GetState();
        //没打开
        if (state == 0)
        {
            ShowClosedChest();
        }
        else if (state == 1)
        {
            if (displayType == MapChestOpenDisplayType.Hide)
            {
                ShowHideChest();
            }
            else if (displayType == MapChestOpenDisplayType.SetOpened)
            {
                ShowOpenedChest();
            }
        }
    }

    /// <summary>
    /// 打开过的宝箱显示
    /// </summary>
    private void ShowOpenedChest()
    {
        m_MapChestInteract.SetOpened();
    }

    /// <summary>
    /// 打开之后消失的宝箱
    /// </summary>
    private void ShowHideChest()
    {
        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.enabled = false;
        }

    }

    /// <summary>
    /// 显示未打开的宝箱
    /// </summary>
    private void ShowClosedChest()
    {
        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.enabled = true;
        }
    }

    //获得宝箱状态
    int GetState()
    {
        string pk = GetRuntimeKey();
        if (!runtime.KeyExist(pk)) return 0;
        return int.Parse(runtime.GetKeyValues(pk));
    }
	
	public void ChangeLockStatus(bool status){
		isLock=status;
	}
}