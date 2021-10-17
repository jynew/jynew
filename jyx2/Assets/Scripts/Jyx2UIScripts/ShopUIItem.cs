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
using HSFrameWork.Common;
using HSFrameWork.ConfigTable;
using Jyx2;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIItem : MonoBehaviour
{
    Image iconImg;
    Text desText;
    Transform numContent;
    Button addBtn;
    Button reduceBtn;
    Text numText;
    Transform select;
    Text itemNum;
    Text totalCost;

    Jyx2ShopItem shopItem;
    int buyCount;
    int index;
    int leftNum;
    public void Init() 
    {
        iconImg = transform.Find("Icon").GetComponent<Image>();
        desText = transform.Find("DesText").GetComponent<Text>();
        numContent = transform.Find("NumContent");
        addBtn = numContent.Find("AddBtn").GetComponent<Button>();
        reduceBtn = numContent.Find("ReduceBtn").GetComponent<Button>();
        numText = numContent.Find("NumText").GetComponent<Text>();
        select = transform.Find("Select");
        itemNum = transform.Find("PriceText").GetComponent<Text>();
        totalCost = numContent.Find("TotalCost").GetComponent<Text>();

        addBtn.onClick.AddListener(OnAddBtnClick);
        reduceBtn.onClick.AddListener(OnReduceBtnClick);
    }

    public async UniTaskVoid Refresh(Jyx2ShopItem shopItem, int index, int hasNum, int hasBuyNum) 
    {
        this.index = index;
        this.shopItem = shopItem;
        Jyx2Item item = ConfigTable.Get<Jyx2Item>(shopItem.Id);
        if (item == null) 
        {
            GameUtil.LogError("查询不到物品配置，id =" + shopItem.Id);
            return;
        }
        
        desText.text = $"{item.Name}\n价格：{shopItem.Price}";
        leftNum = shopItem.Count - hasNum - hasBuyNum;
        leftNum = Tools.Limit(leftNum,0,shopItem.Count);
        itemNum.text = leftNum.ToString();
        
        var sprite = await Jyx2ResourceHelper.LoadItemSprite(int.Parse(item.Id));
        if (sprite != null)
        {
            iconImg.sprite = sprite;
        }
    }

    void RefreshCount() 
    {
        numText.text = buyCount.ToString();
        int moneyCount = GameRuntimeData.Instance.GetMoney();
        int needCount = shopItem.Price * buyCount;
        Color textColor = moneyCount >= needCount ? Color.white : Color.red;
        totalCost.text = "花费："+needCount.ToString();
        totalCost.color = textColor;
    }

    public void SetSelect(bool active) 
    {
        numContent.gameObject.SetActive(active);
        select.gameObject.SetActive(active);
        if (active) 
        {
            buyCount = leftNum > 0 ? 1 : 0;
            RefreshCount();
        }
    }

    void OnAddBtnClick() 
    {
        if (buyCount >= leftNum)
            return;
        buyCount++;
        RefreshCount();
    }

    void OnReduceBtnClick() 
    {
        if (buyCount <= 0)
            return;
        buyCount--;
        RefreshCount();
    }

    public int GetIndex() 
    {
        return index;
    }

    public int GetBuyCount() 
    {
        return buyCount;
    }
}
