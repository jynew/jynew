using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jyx2Configs
{
    [CreateAssetMenu(menuName = "金庸重制版/配置文件/技能", fileName = "技能ID_技能名")]
    public class Jyx2ConfigSkill : Jyx2ConfigBase
    {
        public enum Jyx2ConfigSkillDamageType
        {
            普通 = 0,
            吸内 = 1,
            用毒 = 2,
            解毒 = 3,
            医疗 = 4
        }

        public enum Jyx2ConfigSkillCoverType
        {
            点攻击 = 0,
            线攻击 = 1,
            十字攻击 = 2,
            面攻击 = 3,
        }
        
        private const string CGroup1 = "基本配置";
        private const string CGroup2 = "战斗属性";
        private const string CGroupLevels = "等级配置";
        
        [BoxGroup(CGroup2)][LabelText("伤害类型")][EnumPaging]
        public Jyx2ConfigSkillDamageType DamageType; //伤害类型
        
        [BoxGroup(CGroup2)][LabelText("攻击范围类型")][EnumPaging]
        public Jyx2ConfigSkillCoverType SkillCoverType; //攻击范围
        
        [BoxGroup(CGroup2)][LabelText("消耗内力点数")]
        public int MpCost; 
        
        [BoxGroup(CGroup2)][LabelText("带毒点数")]
        public int Poison;
        
        [InfoBox("错误：必须设置10个等级信息", InfoMessageType.Error, 
            "@this.Levels == null || this.Levels.Count != 10")]
        [BoxGroup(CGroupLevels)] [LabelText("技能等级配置")] [SerializeReference][TableList(ShowIndexLabels = true)]
        [InfoBox("[Attack]攻击力  [SelectRange]选择范围  [AttackRange]杀伤范围  [AddMp]加内力  [KillMp]杀内力")]
        public List<Jyx2ConfigSkillLevel> Levels ;

        [InlineEditor] [BoxGroup("技能外观")] [SerializeReference]
        public Jyx2SkillDisplayAsset Display;

        public override async UniTask WarmUp()
        {
            
        }
    }

    [Serializable]
    public class Jyx2ConfigSkillLevel 
    {
        //[LabelText("攻击力")]
        public int Attack; //攻击力

        //[LabelText("选择范围")]
        public int SelectRange; //移动范围

        //[LabelText("杀伤范围")]
        public int AttackRange; //杀伤范围

        //[LabelText("加内力")]
        public int AddMp; //加内力

        //[LabelText("杀伤内力")]
        public int KillMp; //杀伤内力
    }
}
