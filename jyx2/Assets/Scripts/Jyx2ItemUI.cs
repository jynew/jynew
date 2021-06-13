using System.Collections;
using System.Collections.Generic;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEngine;
using UnityEngine.UI;

public class Jyx2ItemUI : MonoBehaviour
{
    public Image m_Image;
    public Text m_NameText;
    public Text m_CountText;

    public static Jyx2ItemUI Create(int id,int count)
    {
        var prefab = Jyx2ResourceHelper.GetCachedPrefab("Assets/Prefabs/Jyx2ItemUI.prefab");

        
        var obj = Instantiate(prefab); //TODO对象池
        var itemUI = obj.GetComponent<Jyx2ItemUI>();
        itemUI.Show(id, count);
        return itemUI;
    }

    private int _id;

    public Jyx2Item GetItem()
    {
        return ConfigTable.Get<Jyx2Item>(_id);
    }

    public void Show(int id,int count)
    {
        _id = id;
        var item = GetItem();
        m_NameText.text = item.Name;
        m_CountText.text = (count > 1 ? count.ToString() : "");

        Jyx2ResourceHelper.GetItemSprite(id, m_Image);
    }

    public void Select(bool active) 
    {
        Transform select = transform.Find("Select");
        select.gameObject.SetActive(active);
    }

}
