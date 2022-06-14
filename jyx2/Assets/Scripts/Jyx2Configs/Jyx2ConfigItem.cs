using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Jyx2.MOD;
using UnityEngine;

namespace Jyx2Configs
{
    public enum Jyx2ItemType
    {
        TaskItem = 0, //道具
        Equipment = 1, //装备
        Book = 2, //经书
        Costa = 3, //消耗品
        Anqi = 4, //暗器
    }
    public class Jyx2ConfigItem : Jyx2ConfigBase
    {
        // [ShowIf(nameof(IsWeapon))]
        // [BoxGroup(EXTEND_GROUP)][LabelText("武器武功配合加攻击力")]
        // public int ExtraAttack;
        //
        // [ShowIf(nameof(IsWeapon))]
        // [BoxGroup(EXTEND_GROUP)][LabelText("配合武功")][SerializeReference]
        // public Jyx2ConfigSkill PairedWugong;
        //
        // bool IsWeapon()
        // {
        //     return (int)this.EquipmentType == 0;
        // }

        public async UniTask<Sprite> GetPic()
        {
            var _sprite = await MODLoader.LoadAsset<Sprite>($"Assets/BuildSource/Jyx2Items/{Id}.png");
            return _sprite;
        }
        
        //物品说明
        public string Desc; 
        
        //物品类型
        //道具 = 0, 装备 = 1, 经书 = 2, 消耗品 = 3, 暗器 = 4, 
        public int ItemType; 
        
        public Jyx2ItemType GetItemType()
        {
            return (Jyx2ItemType) ItemType;
        }
        
        //装备类型
        //不是装备 = -1, 武器 = 0, 防具 = 1
        public int EquipmentType;
        
        //练出武功
        public int Skill;
        
        //加生命
        public int AddHp; 
        
        //加生命最大值
        public int AddMaxHp; 

        //加中毒解毒
        public int ChangePoisonLevel; 

        //加体力
        public int AddTili; 

        //改变内力性质
        public int ChangeMPType; 

        //加内力
        public int AddMp;

        //加内力最大值
        public int AddMaxMp;

        //加攻击力
        public int Attack;

        //加轻功
        public int Qinggong;

        //加防御力
        public int Defence;

        //加医疗
        public int Heal;

        //加使毒
        public int UsePoison;

        //加解毒
        public int DePoison;

        //加抗毒
        public int AntiPoison;
        
        //加拳掌
        public int Quanzhang;

        //加御剑
        public int Yujian;

        //加耍刀
        public int Shuadao;

        //加特殊兵器
        public int Qimen;

        //加暗器技巧
        public int Anqi;

        //加武学常识
        public int Wuxuechangshi;

        //加品德
        public int AddPinde;

        //左右互搏
        public int Zuoyouhubo;

        //加功夫带毒
        public int AttackPoison;

        //仅修炼人物
        public int OnlySuitableRole;

        //需内力性质
        public int NeedMPType;
        
        //需内力
        public int ConditionMp;

        //需攻击力
        public int ConditionAttack;

        //需轻功
        public int ConditionQinggong;

        //需用毒
        public int ConditionPoison;

        //需医疗
        public int ConditionHeal;

        //需解毒
        public int ConditionDePoison;

        //需拳掌
        public int ConditionQuanzhang;

        //需御剑
        public int ConditionYujian;

        //需耍刀
        public int ConditionShuadao;

        //需特殊兵器
        public int ConditionQimen;

        //需暗器
        public int ConditionAnqi;

        //需资质
        public int ConditionIQ;

        //需经验
        public int NeedExp;

        //需自宫
        //-1不需要, 1需要
        public int NeedCastration;

        //练出物品需经验
        public int GenerateItemNeedExp;

        //需材料
        public int GenerateItemNeedCost;

        //练出物品
        public string GenerateItems;
    }
}
