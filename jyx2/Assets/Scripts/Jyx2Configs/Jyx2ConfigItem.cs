using Cysharp.Threading.Tasks;
using Jyx2.MOD;
using Jyx2.ResourceManagement;
using ProtoBuf;
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
    [ProtoContract]
    public class Jyx2ConfigItem : Jyx2ConfigBase
    {
        public async UniTask<Sprite> GetPic()
        {
            var _sprite = await ResLoader.LoadAsset<Sprite>($"BuildSource/Items/{Id}.png");
            return _sprite;
        }
        
        //物品说明
        [ProtoMember(1)]
        public string Desc; 
        
        //物品类型
        //道具 = 0, 装备 = 1, 经书 = 2, 消耗品 = 3, 暗器 = 4, 
        [ProtoMember(2)]
        public int ItemType; 
        
        public Jyx2ItemType GetItemType()
        {
            return (Jyx2ItemType) ItemType;
        }
        
        //装备类型
        //不是装备 = -1, 武器 = 0, 防具 = 1
        [ProtoMember(3)]
        public int EquipmentType;
        
        //练出武功
        [ProtoMember(4)]
        public int Skill;
        
        //加生命
        [ProtoMember(5)]
        public int AddHp; 
        
        //加生命最大值
        [ProtoMember(6)]
        public int AddMaxHp; 

        //加中毒解毒
        [ProtoMember(7)]
        public int ChangePoisonLevel; 

        //加体力
        [ProtoMember(8)]
        public int AddTili; 

        //改变内力性质
        [ProtoMember(9)]
        public int ChangeMPType; 

        //加内力
        [ProtoMember(10)]
        public int AddMp;

        //加内力最大值
        [ProtoMember(11)]
        public int AddMaxMp;

        //加攻击力
        [ProtoMember(12)]
        public int Attack;

        //加轻功
        [ProtoMember(13)]
        public int Qinggong;

        //加防御力
        [ProtoMember(14)]
        public int Defence;

        //加医疗
        [ProtoMember(15)]
        public int Heal;

        //加使毒
        [ProtoMember(16)]
        public int UsePoison;

        //加解毒
        [ProtoMember(17)]
        public int DePoison;

        //加抗毒
        [ProtoMember(18)]
        public int AntiPoison;
        
        //加拳掌
        [ProtoMember(19)]
        public int Quanzhang;

        //加御剑
        [ProtoMember(20)]
        public int Yujian;

        //加耍刀
        [ProtoMember(21)]
        public int Shuadao;

        //加特殊兵器
        [ProtoMember(22)]
        public int Qimen;

        //加暗器技巧
        [ProtoMember(23)]
        public int Anqi;

        //加武学常识
        [ProtoMember(24)]
        public int Wuxuechangshi;

        //加品德
        [ProtoMember(25)]
        public int AddPinde;

        //左右互搏
        [ProtoMember(26)]
        public int Zuoyouhubo;

        //加功夫带毒
        [ProtoMember(27)]
        public int AttackPoison;

        //仅修炼人物
        [ProtoMember(28)]
        public int OnlySuitableRole;

        //需内力性质
        [ProtoMember(29)]
        public int NeedMPType;
        
        //需内力
        [ProtoMember(30)]
        public int ConditionMp;

        //需攻击力
        [ProtoMember(31)]
        public int ConditionAttack;

        //需轻功
        [ProtoMember(32)]
        public int ConditionQinggong;

        //需用毒
        [ProtoMember(33)]
        public int ConditionPoison;

        //需医疗
        [ProtoMember(34)]
        public int ConditionHeal;

        //需解毒
        [ProtoMember(35)]
        public int ConditionDePoison;

        //需拳掌
        [ProtoMember(36)]
        public int ConditionQuanzhang;

        //需御剑
        [ProtoMember(37)]
        public int ConditionYujian;

        //需耍刀
        [ProtoMember(38)]
        public int ConditionShuadao;

        //需特殊兵器
        [ProtoMember(39)]
        public int ConditionQimen;

        //需暗器
        [ProtoMember(40)]
        public int ConditionAnqi;

        //需资质
        [ProtoMember(41)]
        public int ConditionIQ;

        //需经验
        [ProtoMember(42)]
        public int NeedExp;

        //需自宫
        //-1不需要, 1需要
        [ProtoMember(43)]
        public int NeedCastration;

        //练出物品需经验
        [ProtoMember(44)]
        public int GenerateItemNeedExp;

        //需材料
        [ProtoMember(45)]
        public int GenerateItemNeedCost;

        //练出物品
        [ProtoMember(46)]
        public string GenerateItems;
    }
}
