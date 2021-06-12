using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HSFrameWork.ConfigTable;

namespace Jyx2
{
    [XmlType("jyx2skill")]
    public class Jyx2Skill : BaseBean
    {
        public override string PK { get { return Id; } }

        [XmlAttribute]
        public string Id; //ID

        [XmlAttribute]
        public string Name; //名称

        [XmlAttribute]
        public int SoundEffect; //出招音效

        //1-拳，2-剑，3-刀，4-特殊
        [XmlAttribute]
        public int SkillType; //武功类型

        [XmlAttribute]
        public int Animation; //武功动画&音效

        /// <summary>
        /// 0-普通，1-吸取MP 2-用毒 3-解毒 4-医疗
        /// </summary>
        [XmlAttribute]
        public int DamageType; //伤害类型

        //0-点，1-线，2-十字，3-面
        [XmlAttribute]
        public int SkillCoverType; //攻击范围

        [XmlAttribute]
        public int MpCost; //消耗内力点数

        [XmlAttribute]
        public int Poison; //敌人中毒点数

        [XmlElement]
        public List<Jyx2SkillLevel> SkillLevels; //武功等级因素


        public Jyx2SkillDisplay Display
        {
            get
            {
                return ConfigTable.Get<Jyx2SkillDisplay>(Id);
            }
        }
    }

    [XmlType("jyx2skill_level")]
    public class Jyx2SkillLevel
    {
        [XmlAttribute]
        public int Attack; //攻击力

        [XmlAttribute]
        public int SelectRange; //移动范围

        [XmlAttribute]
        public int AttackRange; //杀伤范围

        [XmlAttribute]
        public int AddMp; //加内力

        [XmlAttribute]
        public int KillMp; //杀伤内力
    }

    [XmlType("jyx2skillDisplay")]
    public class Jyx2SkillDisplay : BaseBean
    {
        public override string PK { get { return Id; } }

        public string GetAnimationController()
        {
            if (string.IsNullOrEmpty(AnimationController))
                return "Assets/BuildSource/AnimationControllers/jyx2humanoidController.controller";
            else
                return AnimationController;
        }
        
        /// <summary>
        /// 是否是使用标准的AnimationController
        /// </summary>
        /// <returns></returns>
        public bool IsStandardAnimationController()
        {
            return string.IsNullOrEmpty(AnimationController);
        }

        /// <summary>
        /// 受击动画码
        /// </summary>
        /// <returns></returns>
        public string GetBeHitAnimationCode()
        {
            if(string.IsNullOrEmpty(BehitAnim))
                return "@Assets/BuildSource/Animations/标准受击.anim";
            else
            {
                return BehitAnim;
            }
        }

        [XmlAttribute]
        public string Id;

        [XmlAttribute]
        public string WeaponCode;

        [XmlAttribute]
        public string AnimationController;

        [XmlAttribute]
        public string BehitAnim;

        [XmlAttribute]
        public string RunAnim;
        
        [XmlAttribute]
        public string IdleAnim;
        
        [XmlAttribute]
        public string AttackAnim;

        [XmlAttribute]
        public float AnimaionDelay;

        [XmlAttribute]
        public float Duration;

        [XmlAttribute]
        public float HitDelay;

        [XmlAttribute]
        public string CastEft;

        [XmlAttribute]
        public float CastDelay;

        [XmlAttribute]
        public string CastOffset;

        [XmlAttribute]
        public string BlockEft;

        [XmlAttribute]
        public float BlockDelay;

        [XmlAttribute]
        public string BlockOffset;

        [XmlAttribute]
        public string AudioEft;

        [XmlAttribute]
        public float AudioEftDelay;
    }
}
