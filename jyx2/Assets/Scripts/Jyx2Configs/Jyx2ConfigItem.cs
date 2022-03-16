using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Jyx2;
using Jyx2.MOD;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

    [CreateAssetMenu(menuName = "金庸重制版/配置文件/道具", fileName = "道具ID_道具名")]
    public class Jyx2ConfigItem : Jyx2ConfigBase
    {
        public enum Jyx2ConfigItemEquipmentType
        {
            不是装备 = -1,
            武器 = 0,
            防具 = 1
        }
        
        public enum Jyx2ConfigItemType
        {
            道具 = 0, 
            装备 = 1, 
            经书 = 2, 
            消耗品 = 3, 
            暗器 = 4, 
        }
        
        private const string EXTEND_GROUP = "扩展属性";
        private const string EFFECT_GROUP = "使用效果";
        private const string CONDITION_GROUP = "使用条件";

        [ShowIf(nameof(IsWeapon))]
        [BoxGroup(EXTEND_GROUP)][LabelText("武器武功配合加攻击力")]
        public int ExtraAttack;

        [ShowIf(nameof(IsWeapon))]
        [BoxGroup(EXTEND_GROUP)][LabelText("配合武功")][SerializeReference]
        public Jyx2ConfigSkill PairedWugong;

        bool IsWeapon()
        {
            return (int)this.EquipmentType == 0;
        }



        [BoxGroup(DEFAULT_GROUP_NAME)][LabelText("图标")]
        public AssetReferenceTexture2D Pic;

        private Sprite _sprite;
        public async UniTask<Sprite> GetPic()
        {
            if (Pic == null || string.IsNullOrEmpty(Pic.AssetGUID)) return null;
            if (_sprite == null)
            {
                _sprite = await MODLoader.LoadAsset<Sprite>(Jyx2ResourceHelper.GetAssetRefAddress(Pic, typeof(Texture2D)));
            }
            return _sprite;
        }
        
        [BoxGroup(DEFAULT_GROUP_NAME)][LabelText("物品说明")]
        public string Desc; 
        
        [BoxGroup(DEFAULT_GROUP_NAME)][LabelText("物品类型")][EnumPaging]
        public Jyx2ConfigItemType ItemType; 
        
        public Jyx2ItemType GetItemType()
        {
            return (Jyx2ItemType) ((int)ItemType);
        }
        
        [ShowIf(nameof(ShowEquipmentType))]
        [BoxGroup(DEFAULT_GROUP_NAME)] [LabelText("装备类型")][EnumToggleButtons] 
        public Jyx2ConfigItemEquipmentType EquipmentType;
        
        bool ShowEquipmentType()
        {
            return (int)ItemType == 1;
        }
        
        [ShowIf(nameof(ShowSkill))]
        [BoxGroup(DEFAULT_GROUP_NAME)][LabelText("练出武功")][SerializeReference]
        public Jyx2ConfigSkill Skill;

        bool ShowSkill()
        {
            return (int)ItemType == 2;
        }


        [BoxGroup(EFFECT_GROUP)][LabelText("加生命")]
        public int AddHp; 

        [BoxGroup(EFFECT_GROUP)][LabelText("加生命最大值")]
        public int AddMaxHp; 

        [BoxGroup(EFFECT_GROUP)][LabelText("加中毒解毒")]
        public int ChangePoisonLevel; 

        [BoxGroup(EFFECT_GROUP)][LabelText("加体力")]
        public int AddTili; 

        [BoxGroup(EFFECT_GROUP)][LabelText("改变内力性质")]
        public int ChangeMPType; 

        [BoxGroup(EFFECT_GROUP)][LabelText("加内力")]
        public int AddMp;

        [BoxGroup(EFFECT_GROUP)][LabelText("加内力最大值")]
        public int AddMaxMp;

        [BoxGroup(EFFECT_GROUP)][LabelText("加攻击力")]
        public int Attack;

        [BoxGroup(EFFECT_GROUP)][LabelText("加轻功")]
        public int Qinggong;

        [BoxGroup(EFFECT_GROUP)][LabelText("加防御力")]
        public int Defence;

        [BoxGroup(EFFECT_GROUP)][LabelText("加医疗")]
        public int Heal;

        [BoxGroup(EFFECT_GROUP)][LabelText("加使毒")]
        public int UsePoison;

        [BoxGroup(EFFECT_GROUP)][LabelText("加解毒")]
        public int DePoison;

        [BoxGroup(EFFECT_GROUP)][LabelText("加抗毒")]
        public int AntiPoison;

        [BoxGroup(EFFECT_GROUP)][LabelText("加拳掌")]
        public int Quanzhang;

        [BoxGroup(EFFECT_GROUP)][LabelText("加御剑")]
        public int Yujian;

        [BoxGroup(EFFECT_GROUP)][LabelText("加耍刀")]
        public int Shuadao;

        [BoxGroup(EFFECT_GROUP)][LabelText("加特殊兵器")]
        public int Qimen;

        [BoxGroup(EFFECT_GROUP)][LabelText("加暗器技巧")]
        public int Anqi;

        [BoxGroup(EFFECT_GROUP)][LabelText("加武学常识")]
        public int Wuxuechangshi;

        [BoxGroup(EFFECT_GROUP)][LabelText("加品德")]
        public int AddPinde;

        [BoxGroup(EFFECT_GROUP)][LabelText("左右互搏")]
        public int Zuoyouhubo;

        [BoxGroup(EFFECT_GROUP)][LabelText("加功夫带毒")]
        public int AttackPoison;

        [ShowIf(nameof(IsItemBook))]
        [BoxGroup(CONDITION_GROUP)][LabelText("仅修炼人物")]
        public int OnlySuitableRole;

        [ShowIf(nameof(IsItemBook))]
        [BoxGroup(CONDITION_GROUP)][LabelText("需内力性质")][EnumToggleButtons]
        public Jyx2ConfigCharacter.MpTypeEnum NeedMPType;

        bool IsItemBook()
        {
            return (int)this.ItemType == 2;
        }

        [BoxGroup(CONDITION_GROUP)][LabelText("需内力")]
        public int ConditionMp;

        [BoxGroup(CONDITION_GROUP)][LabelText("需攻击力")]
        public int ConditionAttack;

        [BoxGroup(CONDITION_GROUP)][LabelText("需轻功")]
        public int ConditionQinggong;

        [BoxGroup(CONDITION_GROUP)][LabelText("需用毒")]
        public int ConditionPoison;

        [BoxGroup(CONDITION_GROUP)][LabelText("需医疗")]
        public int ConditionHeal;

        [BoxGroup(CONDITION_GROUP)][LabelText("需解毒")]
        public int ConditionDePoison;

        [BoxGroup(CONDITION_GROUP)][LabelText("需拳掌")]
        public int ConditionQuanzhang;

        [BoxGroup(CONDITION_GROUP)][LabelText("需御剑")]
        public int ConditionYujian;

        [BoxGroup(CONDITION_GROUP)][LabelText("需耍刀")]
        public int ConditionShuadao;

        [BoxGroup(CONDITION_GROUP)][LabelText("需特殊兵器")]
        public int ConditionQimen;

        [BoxGroup(CONDITION_GROUP)][LabelText("需暗器")]
        public int ConditionAnqi;

        [BoxGroup(CONDITION_GROUP)][LabelText("需资质")]
        public int ConditionIQ;

        [BoxGroup(CONDITION_GROUP)][LabelText("需经验")]
        public int NeedExp;

        [BoxGroup(CONDITION_GROUP)][LabelText("需自宫")]
        public bool NeedCastration;

        [ShowIf(nameof(IsItemBook))]
        [BoxGroup(EFFECT_GROUP)][LabelText("练出物品需经验")]
        public int GenerateItemNeedExp;

        [ShowIf(nameof(IsItemBook))]
        [BoxGroup(EFFECT_GROUP)][LabelText("需材料")][SerializeReference]
        public Jyx2ConfigItem GenerateItemNeedCost;

        [ShowIf(nameof(IsItemBook))]
        [BoxGroup(EFFECT_GROUP)][LabelText("练出物品")]
        [TableList]
        public List<Jyx2ConfigCharacterItem> GenerateItems;

        public override async UniTask WarmUp()
        {
            //GetPic().Forget();
            
            //清理缓存
            if (Application.isEditor)
            {
                _sprite = null;
            }
        }
    }
}
