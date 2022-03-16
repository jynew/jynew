/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

#if JYX2_USE_HSFRAMEWORK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using UnityEngine;

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


        public Jyx2SkillDisplayAsset Display
        {
            get
            {
                return Jyx2SkillDisplayAsset.Get(Name);
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
}
#endif