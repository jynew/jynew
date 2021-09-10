/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2;
using HSFrameWork.ConfigTable;
using Jyx2;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopUIPanel:Jyx2_UIBase
{
    ChildGoComponent childMgr;
    int curShopId;
    Jyx2Shop curShopData;
    int curSelectIndex = 0;
    ShopUIItem curSelectItem;
	Action callback;

    Dictionary<int, int> currentBuyCount = new Dictionary<int, int>();
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
        //curShopId = (int)allParams[0];
		curShopId=int.Parse(LevelMaster.Instance.GetCurrentGameMap().Jyx2MapId);
        curShopData = ConfigTable.Get<Jyx2Shop>(curShopId);

        curSelectIndex = 0;
        currentBuyCount.Clear();
        RefreshChild();
        RefreshProperty();
        RefreshMoney();
		if(allParams.Length>1){
			callback=(Action)allParams[1];
		}
    }
	
    protected override void OnHidePanel(){
		base.OnHidePanel();
		callback?.Invoke();
		callback=null;
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
        Jyx2_UIManager.Instance.HideUI(nameof(ShopUIPanel));
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
            GameUtil.DisplayPopinfo("持有银两不足");
            return;
        }
        GameRuntimeData.Instance.AddItem(item.Id, count);
        AddBuyCount(item.Id, count);
        GameUtil.DisplayPopinfo($"购买{itemCfg.Name},数量{count}");
        GameRuntimeData.Instance.AddItem(GameConst.MONEY_ID, -moneyCost);

        RefreshChild();
        RefreshMoney();
    }
}
