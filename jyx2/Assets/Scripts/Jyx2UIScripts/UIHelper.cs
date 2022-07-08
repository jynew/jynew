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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using i18n.TranslatorDef;
using Jyx2Configs;
using UnityEngine;

public class UIHelper
{
    /// <summary>
    /// 获取物品的效果 //ChangeMPType
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static Dictionary<int, int> GetItemEffect(Jyx2ConfigItem item) 
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
        if (item.Zuoyouhubo != 0)//左右互搏
            result.Add(24, item.Zuoyouhubo);
        if (item.AttackPoison != 0)//攻击带毒
            result.Add(23, item.AttackPoison);
        if (item.ChangePoisonLevel != 0)//中毒解毒
            result.Add(26, item.ChangePoisonLevel);
        if (item.AddTili != 0) //体力
            result.Add(14, item.AddTili);


        return result;
    }

    /// <summary>
    /// 获取使用物品的需求 //NeedMPType; 
    /// </summary>
    /// <param name="item"></param>
    public static Dictionary<int, int> GetUseItemRequire(Jyx2ConfigItem item) 
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
        if (item.ConditionHeal > 0)
            result.Add(6, item.ConditionHeal);
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
        if (item.ConditionAnqi > 0)
            result.Add(12, item.ConditionAnqi);
        if (item.ConditionIQ > 0)
            result.Add(25, item.ConditionIQ);

        return result;

    }

    //使用人
    static string GetItemUser(Jyx2ConfigItem item)
    {
        StringBuilder sb = new StringBuilder();

        RoleInstance user = GameRuntimeData.Instance.GetRoleInTeam(GameRuntimeData.Instance.GetItemUser(item.Id));
        if (user != null)
        {
            sb.Append($"{user.Name}\n");
        }

        return sb.ToString();
    }

    //效果
    static string GetEffectText(Jyx2ConfigItem item)
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
    static string GetUseRquire(Jyx2ConfigItem item)
    {
        Dictionary<int, int> effects = UIHelper.GetUseItemRequire(item);
        StringBuilder sb = new StringBuilder();
        if (item.NeedExp > 0)
        {
            //sb.Append($"经验:  {item.NeedExp}\n");
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
    static string GetOutPut(Jyx2ConfigItem item)
    {
        if (item.GenerateItems == "")
            return "";
        
        StringBuilder sb = new StringBuilder();
        
        var GenerateItemList = new List<Jyx2ConfigCharacterItem>();
        var GenerateItemArr = item.GenerateItems.Split('|');
        foreach (var GenerateItem in GenerateItemArr)
        {
            var GenerateItemArr2 = GenerateItem.Split(',');
            if (GenerateItemArr2.Length != 2) continue;
            var characterItem = new Jyx2ConfigCharacterItem();
            characterItem.Id = int.Parse(GenerateItemArr2[0]);
            characterItem.Count = int.Parse(GenerateItemArr2[1]);
            GenerateItemList.Add(characterItem);
        }
        foreach (var tempItem in GenerateItemList)
        {
            var cfg = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(tempItem.Id);
            if (cfg == null)
                continue;
            sb.Append($"{cfg.Name}:  {tempItem.Count}\n");
        }

        return sb.ToString();
    }

    //需要物品
    static string GetNeedItem(Jyx2ConfigItem item)
    {
        StringBuilder sb = new StringBuilder();
        if (item.GenerateItemNeedExp > 0)
        {
           /* sb.Append($"练出物品需经验:  {item.GenerateItemNeedExp}\n");*/
        }
        
        if (item.GenerateItemNeedCost != -1)
        {
            sb.Append($"材料:  {GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(item.GenerateItemNeedCost).Name}\n");
        }

        return sb.ToString();
    }

    public static string GetItemDesText(Jyx2ConfigItem item)
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.Append($"<size=35><color=#FFDB00>{item.Name}</color></size>\n");
        strBuilder.Append($"{item.Desc}");

        string user = GetItemUser(item);
        if (!string.IsNullOrEmpty(user))
        {
            strBuilder.Append($"\n\n");
            strBuilder.Append("<size=28><color=#FFDB00>使用人</color></size>\n");
            strBuilder.Append(user);
        }
        string effect = GetEffectText(item);
        if (!string.IsNullOrEmpty(effect))
        {
            strBuilder.Append($"\n\n");
            //---------------------------------------------------------------------------
            //strBuilder.Append("<size=28><color=#FFDB00>效果</color></size>\n");
            //---------------------------------------------------------------------------
            //特定位置的翻译【MainMenu右下角当前版本的翻译】
            //---------------------------------------------------------------------------
            strBuilder.Append("<size=28><color=#FFDB00>效果</color></size>\n".GetContent(nameof(UIHelper)));
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
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
}
 