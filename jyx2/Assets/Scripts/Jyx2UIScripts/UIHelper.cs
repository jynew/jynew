using HSFrameWork.ConfigTable;
using Jyx2;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UIHelper
{
    /// <summary>
    /// 获取物品的效果 //ChangeMPType
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static Dictionary<int, int> GetItemEffect(Jyx2Item item) 
    {
        Dictionary<int, int> result = new Dictionary<int, int>();
        if (item.AddHp != 0)//加血
            result.Add(13, item.AddHp);
        if (item.AddMaxHp != 0)//最大血量
            result.Add(16, item.AddMaxHp);
        if (item.AddMp != 0)//加内力
            result.Add(15, item.AddMp);
        if (item.AddMaxMp != 0)//最大内力
            result.Add(17, item.AddMaxMp);
        if (item.Attack != 0)//攻击
            result.Add(18, item.Attack);
        if (item.Qinggong != 0)//轻功
            result.Add(3, item.Qinggong);
        if (item.Defence != 0)//防御
            result.Add(4, item.Defence);
        if (item.Heal != 0)//医疗
            result.Add(6, item.Heal);
        if (item.UsePoison != 0)//用毒
            result.Add(7, item.UsePoison);
        if (item.DePoison != 0)//解毒
            result.Add(8, item.DePoison);
        if (item.AntiPoison != 0)//抗毒
            result.Add(19, item.AntiPoison);
        if (item.Quanzhang != 0)//拳掌
            result.Add(9, item.Quanzhang);
        if (item.Yujian != 0)//御剑
            result.Add(10, item.Yujian);
        if (item.Shuadao != 0)//耍刀
            result.Add(11, item.Shuadao);
        if (item.Qimen != 0)//奇门
            result.Add(20, item.Qimen);
        if (item.Anqi != 0)//暗器
            result.Add(12, item.Anqi);
        if (item.Wuxuechangshi != 0)//武学常识
            result.Add(21, item.Wuxuechangshi);
        if (item.AttackFreq != 0)//左右互搏
            result.Add(24, item.AttackFreq);
        if (item.AttackPoison != 0)//攻击带毒
            result.Add(23, item.AttackPoison);
        if (item.ChangePoisonLevel != 0)//中毒解毒
            result.Add(26, item.ChangePoisonLevel);


        return result;
    }

    /// <summary>
    /// 获取使用物品的需求 //NeedMPType; 
    /// </summary>
    /// <param name="item"></param>
    public static Dictionary<int, int> GetUseItemRequire(Jyx2Item item) 
    {
        Dictionary<int, int> result = new Dictionary<int, int>();
        if (item.ConditionMp > 0)
            result.Add(15, item.ConditionMp);
        if (item.ConditionAttack > 0)
            result.Add(18, item.ConditionAttack);
        if (item.ConditionQinggong > 0)
            result.Add(3, item.ConditionQinggong);
        if (item.ConditionPoison > 0)
            result.Add(7, item.ConditionPoison);
        if (item.ConditionDePoison > 0)
            result.Add(8, item.ConditionDePoison);
        if (item.ConditionQuanzhang > 0)
            result.Add(9, item.ConditionQuanzhang);
        if (item.ConditionYujian > 0)
            result.Add(10, item.ConditionYujian);
        if (item.ConditionShuadao > 0)
            result.Add(11, item.ConditionShuadao);
        if (item.ConditionQimen > 0)
            result.Add(20, item.ConditionQimen);
        if (item.ConditionIQ > 0)
            result.Add(25, item.ConditionIQ);

        return result;

    }

    //效果
    static string GetEffectText(Jyx2Item item)
    {
        Dictionary<int, int> effects = UIHelper.GetItemEffect(item);
        StringBuilder sb = new StringBuilder();
        foreach (var effect in effects)
        {
            if (!GameConst.ProItemDic.ContainsKey(effect.Key.ToString()))
                continue;
            PropertyItem pro = GameConst.ProItemDic[effect.Key.ToString()];
            string valueText = effect.Value > 0 ? $"+{effect.Value}" : effect.Value.ToString();
            sb.Append($"{pro.Name}:  {valueText}\n");
        }
        return sb.ToString();
    }

    //使用要求
    static string GetUseRquire(Jyx2Item item)
    {
        Dictionary<int, int> effects = UIHelper.GetUseItemRequire(item);
        StringBuilder sb = new StringBuilder();
        if (item.NeedExp > 0)
        {
            sb.Append($"经验:  {item.NeedExp}\n");
        }
        foreach (var effect in effects)
        {
            if (!GameConst.ProItemDic.ContainsKey(effect.Key.ToString()))
                continue;
            PropertyItem pro = GameConst.ProItemDic[effect.Key.ToString()];
            sb.Append($"{pro.Name}:  {effect.Value.ToString()}\n");
        }
        return sb.ToString();
    }

    //产出
    static string GetOutPut(Jyx2Item item)
    {
        List<Jyx2RoleItem> items = item.GenerateItems;
        if (items == null || items.Count <= 0)
            return "";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < items.Count; i++)
        {
            Jyx2RoleItem tempItem = items[i];
            Jyx2Item cfg = ConfigTable.Get<Jyx2Item>(tempItem.Id);
            if (cfg == null)
                continue;
            sb.Append($"{cfg.Name}:  {tempItem.Count}\n");
        }

        return sb.ToString();
    }

    //需要物品
    static string GetNeedItem(Jyx2Item item)
    {
        StringBuilder sb = new StringBuilder();
        if (item.GenerateItemNeedExp > 0)
        {
            sb.Append($"练出物品需经验:  {item.GenerateItemNeedExp}\n");
        }
        if (item.GenerateItemNeedCost > 0)
        {
            Jyx2Item cfg = ConfigTable.Get<Jyx2Item>(item.GenerateItemNeedCost);
            if (cfg != null)
                sb.Append($"材料:  {cfg.Name}\n");
        }

        return sb.ToString();
    }

    static string GetItemDesText(Jyx2Item item)
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.Append($"<size=35><color=#FFDB00>{item.Name}</color></size>\n");
        strBuilder.Append($"{item.Desc}");

        string effect = GetEffectText(item);
        if (!string.IsNullOrEmpty(effect))
        {
            strBuilder.Append($"\n\n");
            strBuilder.Append("<size=28><color=#FFDB00>效果</color></size>\n");
            strBuilder.Append(effect);
        }

        string useRequire = GetUseRquire(item);
        if (!string.IsNullOrEmpty(useRequire))
        {
            strBuilder.Append($"\n\n");
            strBuilder.Append("<size=28><color=#FFDB00>使用需求</color></size>\n");
            strBuilder.Append(useRequire);
        }

        string output = GetOutPut(item);
        if (!string.IsNullOrEmpty(output))
        {
            strBuilder.Append($"\n\n");
            strBuilder.Append("<size=28><color=#FFDB00>练出</color></size>\n");
            strBuilder.Append(output);
        }

        string needItem = GetNeedItem(item);
        if (!string.IsNullOrEmpty(needItem))
        {
            strBuilder.Append($"\n\n");
            strBuilder.Append("<size=28><color=#FFDB00>需要物品</color></size>\n");
            strBuilder.Append(needItem);
        }

        return strBuilder.ToString();
    }

    public static string GetItemDesText(int itemId) 
    {
        Jyx2Item item = ConfigTable.Get<Jyx2Item>(itemId);
        if (item == null) 
        {
            GameUtil.LogError("配置表错误，查询不到物品，itemid=" + itemId);
            return "";
        }
        return GetItemDesText(item);
    }
}
 