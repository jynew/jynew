using Jyx2;
using HSFrameWork.ConfigTable;
using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopUIPanel:Jyx2_UIBase
{
    ChildGoComponent childMgr;
    int curShopId;
    Jyx2Shop curShopData;
    int curSelectIndex = 0;
    ShopUIItem curSelectItem;

    Dictionary<int, int> currentBuyCount = new Dictionary<int, int>();//当前已经购买的物品数量
    protected override void OnCreate()
    {
        InitTrans();
        childMgr = GameUtil.GetOrAddComponent<ChildGoComponent>(ItemRoot_RectTransform);
        childMgr.Init(ScrollItem_RectTransform, (trans) => 
        {
            ShopUIItem item = GameUtil.GetOrAddComponent<ShopUIItem>(trans);
            item.Init();
            BindListener(trans.GetComponent<Button>(), () =>
            {
                OnItemClick(item);
            });
        });

        BindListener(CloseBtn_Button, OnCloseClick);
        BindListener(ConfirmBtn_Button, OnConfirmClick);
    }

    int GetHasBuyNum(int id) 
    {
        if (!currentBuyCount.ContainsKey(id))
            return 0;
        return currentBuyCount[id];
    }

    void AddBuyCount(int itemId, int num) 
    {
        if (!currentBuyCount.ContainsKey(itemId))
            currentBuyCount[itemId] = 0;
        currentBuyCount[itemId] += num;
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        curShopId = (int)allParams[0];
        curShopData = ConfigTable.Get<Jyx2Shop>(curShopId);

        curSelectIndex = 0;
        currentBuyCount.Clear();
        RefreshChild();
        RefreshProperty();
        RefreshMoney();
    }

    void RefreshMoney() 
    {
        int num = GameRuntimeData.Instance.GetMoney();
        MoneyNum_Text.text = $"持有银两:{num}";
    }

    void RefreshChild() 
    {
        childMgr.RefreshChildCount(curShopData.ShopItems.Count);
        List<Transform> childList = childMgr.GetUsingTransList();
        for (int i = 0; i < childList.Count; i++)
        {
            Transform trans = childList[i];
            Jyx2ShopItem data = curShopData.ShopItems[i];
            ShopUIItem uiItem = trans.GetComponent<ShopUIItem>();
            int currentNum = GetHasBuyNum(data.Id);
            uiItem.Refresh(data,i,currentNum);
            uiItem.SetSelect(curSelectIndex == i);
            if (curSelectIndex == i)
                curSelectItem = uiItem;
        }
    }

    void RefreshProperty() 
    {
        if (curSelectIndex < 0 || curSelectIndex >= curShopData.ShopItems.Count) 
        {
            ItemDes_RectTransform.gameObject.SetActive(false);
            return;
        }
        ItemDes_RectTransform.gameObject.SetActive(true);
        string mainText = UIHelper.GetItemDesText(curShopData.ShopItems[curSelectIndex].Id);
        DesText_Text.text = mainText;
    }

    void OnItemClick(ShopUIItem item)
    {
        int index = item.GetIndex();
        if (index == curSelectIndex)
            return;
        curSelectIndex = index;
        if (curSelectItem) 
        {
            curSelectItem.SetSelect(false);
        }
        curSelectItem = item;
        curSelectItem.SetSelect(true);
        RefreshProperty();
    }

    void OnCloseClick() 
    {
        Jyx2_UIManager.Instance.HideUI("ShopUIPanel");
    }

    void OnConfirmClick() 
    {
        if (curSelectItem == null)
            return;
        int count = curSelectItem.GetBuyCount();
        if (count <= 0)
            return;
        Jyx2ShopItem item = curShopData.ShopItems[curSelectItem.GetIndex()];
        Jyx2Item itemCfg = ConfigTable.Get<Jyx2Item>(item.Id);
        if (itemCfg == null)
            return;
        int moneyCost = count * item.Price;
        if (GameRuntimeData.Instance.GetMoney() < moneyCost) 
        {
            GameUtil.DisplayPopinfo("银两不够");
            return;
        }
        GameRuntimeData.Instance.AddItem(item.Id, count);
        AddBuyCount(item.Id, count);
        GameUtil.DisplayPopinfo($"获得物品{itemCfg.Name},数量{count}");
        GameRuntimeData.Instance.AddItem(Jyx2Consts.MONEY_ID, -moneyCost);

        RefreshChild();
        RefreshMoney();
    }
}
