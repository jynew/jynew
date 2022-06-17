using ProtoBuf;

namespace Jyx2Configs
{
    [ProtoContract]
    public class Jyx2ConfigSkill : Jyx2ConfigBase
    {
        //伤害类型
        //普通 = 0, 吸内 = 1, 用毒 = 2, 解毒 = 3, 医疗 = 4
        [ProtoMember(1)]
        public int DamageType; //伤害类型
        
        //攻击范围类型
        //点攻击 = 0, 线攻击 = 1, 十字攻击 = 2, 面攻击 = 3,
        [ProtoMember(2)]
        public int SkillCoverType;
        
        //消耗内力点数
        [ProtoMember(3)]
        public int MpCost; 
        
        //带毒点数
        [ProtoMember(4)]
        public int Poison;
        
        //技能等级配置
        [ProtoMember(5)]
        public string Levels ;
        
        //技能外观
        public Jyx2SkillDisplayAsset Display
        {
            get
            {
                return Jyx2SkillDisplayAsset.Get(Name);
            }
        }
    }
    
    public class Jyx2ConfigSkillLevel 
    {
        //攻击力
        public int Attack;
        
        //移动范围
        public int SelectRange;

        //杀伤范围
        public int AttackRange;

        //加内力
        public int AddMp; 

        //杀伤内力
        public int KillMp;
    }
}
