using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using HSUI;
using Jyx2;

public class ItemUI : DefaultUISpawnDespawnReceivable, IPointerDownHandler, IPointerUpHandler
{
    private static List<Color> _colorMapping = new List<Color>()
    {
        new Color((float)0x4f/(float)0xff, (float)0x3e/(float)0xff, (float)0x26/(float)0xff),//0x4F3E26
	    new Color((float)0x21/(float)0xff, (float)0x45/(float)0xff, (float)0x52/(float)0xff),//0x214552
	    new Color((float)0x31/(float)0xff, (float)0x51/(float)0xff, (float)0x29/(float)0xff),//0x315129 },
	    new Color((float)0x8C/(float)0xff, (float)0x45/(float)0xff, (float)0x00/(float)0xff),//0x8C4500 },
	    new Color((float)0x52/(float)0xff, (float)0x27/(float)0xff, (float)0x42/(float)0xff),//0x522742 },
	    new Color((float)0x9C/(float)0xff, (float)0x20/(float)0xff, (float)0x00/(float)0xff),//0x9C2000 },
	};

    public Text m_NameText;
    public Text m_CountText;
    public Text m_PriceText;
    public Text m_TagText;
    public Text m_UserCountText;
    public Image m_Icon;
    public GameObject m_DisableSignObj;
    public GameObject m_NewSignObj;
    public GameObject m_LockSignObj;
    public Image m_Frame;
    public Sprite[] m_FrameSprite;
    public Button m_ActionButton;
    public Button m_SelfButton;

    ItemInstance m_Item;
    ItemType m_ItemType = ItemType.Error;
    bool m_IsInteractive = false;
    Action m_Callback;
    Func<ItemInstance, bool> m_IsActiveCallback;
    int m_Price;

    public bool IsShopItem { get; set; }
    public int Count { get; set; }
    public bool IsInteractive
    {
        get
        {
            return m_IsInteractive;
        }
    }
    public ItemType ItemType
    {
        get
        {
            if (m_Item != null) return m_Item.Type;
            return m_ItemType;
        }
        set { m_ItemType = value; }
    }

    void Awake()
    {
        InitInfo(this);
    }

    public ItemInstance GetItem()
    {
        return m_Item;
    }

    public void SetEmpty()
    {
        m_CountText.gameObject.SetActive(false);
        m_DisableSignObj.SetActive(false);
        m_LockSignObj.SetActive(false);
        m_NewSignObj.SetActive(false);
        m_Frame.sprite = m_FrameSprite[0];

        m_NameText.text = "";
        m_Icon.gameObject.SetActive(false);
        m_Item = null;
        m_PriceText.text = "";
    }

    public void Refresh()
    {
        Bind(m_Item, Count, m_Callback, m_IsActiveCallback, m_Price);
    }

    public void SafeDestroy()
    {
        gameObject.SetActive(false);
        transform.SetParent(null);
        Destroy(gameObject, 0.1f);
    }

    public void SetShowTag(string showtag)
    {
        if (string.IsNullOrEmpty(showtag))
        {
            m_TagText.gameObject.SetActive(false);
        }
        else
        {
            m_TagText.text = showtag;
            m_TagText.gameObject.SetActive(true);
        }
    }

    public void Bind(ItemInstance item, Action callback,
        Func<ItemInstance, bool> isActiveCallback = null, int price = -1)
    {
        Bind(item, item.Count, callback, isActiveCallback, price);
    }

    public void Bind(ItemInstance item, int count, Action callback,
        Func<ItemInstance, bool> isActiveCallback = null, int price = -1)
    {
        m_Item = item;
        Count = count;
        m_Callback = callback;
        m_IsActiveCallback = isActiveCallback;
        m_Price = price;
        if (m_TagText != null)
        {
            m_TagText.gameObject.SetActive(false);
        }

        if (isActiveCallback != null)
        {
            if (!isActiveCallback(item))
            {
                m_DisableSignObj.SetActive(true);
                m_IsInteractive = false;
            }
            else
            {
                m_DisableSignObj.SetActive(false);
                m_IsInteractive = true;
            }
        }
        else
        {
            if (m_DisableSignObj != null)
            {
                m_DisableSignObj.SetActive(false);
            }
        }

        // 物品名字
        string nameText = item.ItemData.Name;

        m_NameText.text = nameText;
        if (IsShopItem)
        {
            if (Count == 1 || Count == -1)
            {
                m_CountText.text = "";
                m_CountText.gameObject.SetActive(false);
            }
            else
            {
                m_CountText.text = string.Format("限购 {0}", Count.ToString());
                m_CountText.gameObject.SetActive(true);
            }
        }
        else
        {
            m_NameText.GetComponent<Outline>().effectColor = _colorMapping[(int)item.GetRare()];
            if (Count == 1 || Count == -1)
            {
                m_CountText.text = "";
                m_CountText.gameObject.SetActive(false);
            }
            else
            {
                m_CountText.text = $"×{Count}";
                m_CountText.gameObject.SetActive(true);
            }
        }
        if (callback == null)
        {
            m_SelfButton.enabled = false;
        }
        else
        {
            m_SelfButton.enabled = true;
            BindListener(m_SelfButton, delegate
            {
                callback();
            });
        }
        SetFrame(item);

        
        m_Icon.gameObject.SetActive(true);

        if (m_PriceText != null)
        {
            if (price <= 0)
            {
                m_PriceText.text = "";
            }
            else
            {
                m_PriceText.text = string.Format("{0}", price.ToString());
            }
        }


        //if (item.isNew == true)
        //{
        //    m_NewSignObj.SetActive(true);
        //}
        //else
        //{
        //    m_NewSignObj.SetActive(false);
        //}
        //if (item.IsLocked())
        //{
        //    m_LockSignObj.SetActive(true);
        //}
        //else
        //{
        //    m_LockSignObj.SetActive(false);
        //}
    }

    public void RefreshCountText()
    {
        if (Count == 1 || Count == -1)
        {
            m_CountText.text = "";
            m_CountText.gameObject.SetActive(false);
        }
        else
        {
            m_CountText.text = $"×{Count}";
            m_CountText.gameObject.SetActive(true);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {

    }

    public void OnPointerUp(PointerEventData data)
    {

    }

    void SetFrame(ItemInstance item)
    {
        m_Frame.sprite = m_FrameSprite[(int)item.GetRare()];
    }

    void BindItemCallback(ItemInstance item, int count, Action<ItemInstance, ItemUI> callback)
    {
        Bind(item, count, () =>
        {
            callback(this.GetItem(), this);
        });
    }
    void SetInteractive(bool isInteractive)
    {
        m_DisableSignObj.SetActive(!isInteractive);
        m_IsInteractive = isInteractive;
    }

    public void SetUserCount(int count)
    {
        if (count > 0)
        {
            m_UserCountText.transform.parent.gameObject.SetActive(true);
            m_UserCountText.text = count.ToString();
        }
        else
        {
            m_UserCountText.transform.parent.gameObject.SetActive(false);
        }
    }
}