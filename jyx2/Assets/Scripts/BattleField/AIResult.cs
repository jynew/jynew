/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using HSFrameWork.Common;
using ProtoBuf;
using System.Xml.Serialization;

namespace Jyx2
{
    [XmlType]
    public class AIResult
    {
        #region 行为结果
        //移动到的位置
        [ProtoMember(1)]
        [XmlAttribute]
        public int MoveX;

        [ProtoMember(2)]
        [XmlAttribute]
        public int MoveY;

        //使用的招式
        [XmlIgnore]
        public BattleZhaoshiInstance Zhaoshi;

        [ProtoMember(3)]
        [XmlAttribute("skill")]
        public string zhaoshiPK
        {
            get
            {
                if (Zhaoshi == null) return string.Empty;
                return Zhaoshi.Data.PK;
            }
            set { }
        }

        //攻击坐标
        [ProtoMember(4)]
        [XmlAttribute]
        public int AttackX;
        [ProtoMember(5)]
        [XmlAttribute]
        public int AttackY;

        //是否休息
        [XmlAttribute]
        public bool IsRest;

        //使用的道具
        [ProtoMember(6)]
        [XmlAttribute]
        public Jyx2Item Item;
        #endregion


    }

    public class SkillCastResult
    {
        public SkillCastResult() { }

        public SkillCastResult(RoleInstance sprite, RoleInstance target, BattleZhaoshiInstance tzhaoshi, int targetx, int targety)
        {
            //self = new SkillCastRoleEffect(sprite);
            r1 = sprite;
            r2 = target;
            zhaoshi = tzhaoshi;
            skilltarget_x = targetx;
            skilltarget_y = targety;
        }

        public int skilltarget_x;
        public int skilltarget_y;

        [XmlIgnore]
        public BattleZhaoshiInstance zhaoshi;

        [XmlIgnore]
        public RoleInstance r1;

        [XmlIgnore]
        public RoleInstance r2;

        public int damage; //伤害
        public int damageMp;
        public int addMp; //增加内力
        public int poison;
        public int depoison;
        public int heal;

        public double GetTotalScore()
        {
            if(r1.team != r2.team)
            {
                float scale = 1;
                if (damage >= r2.Hp)
                    scale = 1.25f;
                float attackTwiceScale = 1;
                if (r1.Zuoyouhubo == 1)
                    attackTwiceScale = 2;

                return attackTwiceScale * scale * damage + attackTwiceScale * damageMp / 5 + poison;
            }else if(r1.team == r2.team)
            {
                return depoison + heal;
            }
            return 0;
        }

        public bool IsDamage()
        {
            return damage > 0 || damageMp > 0;
        }

        /// <summary>
        /// 具体执行改逻辑
        /// </summary>
        public void Run()
        {
            var rst = this;
            if (rst.damage > 0)
            {
                if (rst.damage > r2.Hp) rst.damage = r2.Hp;
                r2.Hp -= rst.damage;

                if (r2.View != null)
                {
                    r2.View.SetDamage(rst.damage, r2.Hp);
                }

                r1.ExpGot += rst.damage;
                if (r2.Hp <= 0)
                    r1.ExpGot += rst.damage / 2;

                //无敌
                if(BattleManager.Whosyourdad && r2.team == 0)
                {
                    r2.Hp = r2.MaxHp;
                }
            }

            if (rst.damageMp > 0)
            {
                int damageMp = Tools.Limit(rst.damageMp, 0, r2.Mp);
                r2.Mp -= damageMp;
                if (r2.View != null)
                {
                    r2.View.ShowAttackInfo($"<color=blue>内力-{damageMp}</color>");
                }

                //吸取内力逻辑
                if (rst.addMp > 0)
                {
                    int finalMp = Tools.Limit(r1.Mp + rst.addMp, 0, r1.MaxMp);
                    int deltaMp = finalMp - r1.Mp;
                    if (deltaMp >= 0)
                    {
                        r1.View.ShowAttackInfo($"<color=blue>内力+{deltaMp}</color>");
                        r1.Mp = finalMp;
                    }
                }

                r1.ExpGot += damageMp / 2;
            }

            if (rst.poison > 0)
            {
                r2.Poison += rst.poison;
                if (r2.View != null)
                {
                    r2.View.ShowAttackInfo($"<color=green>中毒+{rst.poison}</color>");
                }

                r1.ExpGot += rst.poison;
            }

            if (rst.depoison > 0)
            {
                r2.Poison -= rst.depoison;
                if (r2.View != null)
                {
                    r2.View.ShowAttackInfo($"<color=green>中毒-{rst.depoison}</color>");
                }

                r1.ExpGot += rst.depoison;
            }

            if (rst.heal > 0)
            {
                int tmp = r2.Hp;
                r2.Hp += rst.heal;
                r2.Hp = Tools.Limit(r2.Hp, 0, r2.MaxHp);
                int addHp = r2.Hp - tmp;
                if (r2.View != null)
                {
                    r2.View.ShowAttackInfo($"<color=white>医疗+{addHp}</color>");
                }

                r1.ExpGot += rst.heal;
            }
        }
    }
}
