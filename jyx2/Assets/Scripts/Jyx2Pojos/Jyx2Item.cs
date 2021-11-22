/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


using UnityEngine;

namespace Jyx2
{

#if JYX2_USE_HSFRAMEWORK
    
    [XmlType("jyx2item")]
    public class Jyx2Item : BaseBean
    {
        public override string PK { get { return Id; } }

        [XmlAttribute]
        public string Id; //ID

        [XmlAttribute]
        public string Name; //物品名

        [XmlAttribute]
        public string Brief; //物品缩写

        [XmlAttribute]
        public string Desc; //物品说明

        [XmlAttribute]
        public int Wugong; //练出武功

        [XmlAttribute]
        public int AnqiAnimation; //暗器动画编号


        //使用人
        public int User
        {
            get
            {
                if (!GameRuntimeData.Instance.ItemUser.ContainsKey(Id))
                    return -1;
                return GameRuntimeData.Instance.ItemUser[Id];
            }
            set
            {
                GameRuntimeData.Instance.ItemUser[Id] = value;
            }
        }

        [XmlAttribute]
        public int EquipmentType; //装备类型

        [XmlAttribute]
        public int IsShowDesc; //显示物品说明

        [XmlAttribute]
        public int ItemType; //物品类型

        public Jyx2ItemType GetItemType()
        {
            return (Jyx2ItemType) ItemType;
        }

        [XmlAttribute]
        public int AddHp; //加生命

        [XmlAttribute]
        public int AddMaxHp; //加生命最大值

        [XmlAttribute("Jiedu")]
        public int ChangePoisonLevel; //加中毒解毒

        [XmlAttribute]
        public int AddTili; //加体力

        [XmlAttribute("ChangeNeigongType")]
        public int ChangeMPType; //改变内力性质

        [XmlAttribute]
        public int AddMp;//加内力

        [XmlAttribute]
        public int AddMaxMp; //加内力最大值

        [XmlAttribute]
        public int Attack; //加攻击力

        [XmlAttribute]
        public int Qinggong; //加轻功

        [XmlAttribute]
        public int Defence; //加防御力

        [XmlAttribute]
        public int Heal; //加医疗

        [XmlAttribute("Poison")]
        public int UsePoison;//加使毒

        [XmlAttribute]
        public int DePoison; //加解毒

        [XmlAttribute]
        public int AntiPoison; //加抗毒

        [XmlAttribute]
        public int Quanzhang; //加拳掌

        [XmlAttribute]
        public int Yujian; //加御剑

        [XmlAttribute]
        public int Shuadao; //加耍刀

        [XmlAttribute]
        public int Qimen; //加特殊兵器

        [XmlAttribute]
        public int Anqi;//加暗器技巧

        [XmlAttribute]
        public int Wuxuechangshi; //加武学常识

        [XmlAttribute]
        public int AddPinde; //加品德

        [XmlAttribute]
        public int AttackFreq; //加攻击次数

        [XmlAttribute]
        public int AttackPoison; //加功夫带毒

        [XmlAttribute("UserLearnLimit")]
        public int OnlySuitableRole; //仅修炼人物

        [XmlAttribute("ConditionNeili")]
        public int NeedMPType; //需内力性质

        [XmlAttribute]
        public int ConditionMp; //需内力

        [XmlAttribute]
        public int ConditionAttack; //需攻击力

        [XmlAttribute]
        public int ConditionQinggong; //需轻功

        [XmlAttribute]
        public int ConditionPoison;//需用毒

        [XmlAttribute]
        public int ConditionHeal; //需医疗

        [XmlAttribute]
        public int ConditionDePoison; //需解毒

        [XmlAttribute]
        public int ConditionQuanzhang; //需拳掌

        [XmlAttribute]
        public int ConditionYujian; //需御剑

        [XmlAttribute]
        public int ConditionShuadao; //需耍刀

        [XmlAttribute]
        public int ConditionQimen;//需特殊兵器

        [XmlAttribute]
        public int ConditionAnqi;//需暗器

        [XmlAttribute("ConditionZizhi")]
        public int ConditionIQ; //需资质

        [XmlAttribute("ConditionExp")]
        public int NeedExp; //需经验

        [XmlAttribute]
        public int GenerateItemNeedExp;//练出物品需经验

        [XmlAttribute]
        public int GenerateItemNeedCost; //需材料

        [XmlElement]
        public List<Jyx2RoleItem> GenerateItems; //练出物品
        
    }
#endif
}
