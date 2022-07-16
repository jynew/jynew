/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using ProtoBuf;
using System.Xml.Serialization;
using Jyx2.Middleware;
using Jyx2Configs;

namespace Jyx2
{
    [XmlType]
    public class AIResult
    {
        #region 行为结果
        //移动到的位置
        [XmlAttribute]
        public int MoveX;
        
        [XmlAttribute]
        public int MoveY;

        //使用的招式
        [XmlIgnore]
        public SkillCastInstance SkillCast;
        
        [XmlAttribute("skill")]
        public string skillCastPK
        {
            get
            {
                if (SkillCast == null) return string.Empty;
                return SkillCast.Data.Key.ToString();
            }
            set { }
        }

        //攻击坐标
        [XmlAttribute]
        public int AttackX;

        [XmlAttribute]
        public int AttackY;

        //是否休息
        [XmlAttribute]
        public bool IsRest;

        //使用的道具
        [XmlAttribute]
        public Jyx2ConfigItem Item;
        #endregion


    }

    public class SkillCastResult
    {
        public SkillCastResult() { }

        public SkillCastResult(RoleInstance sprite, RoleInstance target, SkillCastInstance tSkillCast, int targetx, int targety)
        {
            //self = new SkillCastRoleEffect(sprite);
            r1 = sprite;
            r2 = target;
            skillCast = tSkillCast;
            skilltarget_x = targetx;
            skilltarget_y = targety;
        }

        public int skilltarget_x;
        public int skilltarget_y;

        [XmlIgnore]
        public SkillCastInstance skillCast;

        [XmlIgnore]
        public RoleInstance r1;

        [XmlIgnore]
        public RoleInstance r2;

        public int damage; //伤害
        public int damageMp;
        public int addMp; //增加内力
        public int addMaxMp;
        public int poison;
        public int depoison;
        public int heal;
        public int hurt;

        public double GetTotalScore()
        {
            float scale = 1;
            if (damage >= r2.Hp)
                scale = 1.25f;
            float attackTwiceScale = 1;
            if (r1.Zuoyouhubo == 1)
                attackTwiceScale = 2;

            return attackTwiceScale * scale * damage + attackTwiceScale * damageMp / 5;
        }

        public bool IsDamage()
        {
            return damage > 0 || damageMp > 0;
        }

        /// <summary>
        /// 具体执行改逻辑
        /// 战斗经验计算公式可以参考：https://github.com/ZhanruiLiang/jinyong-legend
        /// </summary>
        /// <returns></returns>
        public void Run()
        {
            var rst = this;
            if (rst.damage > 0)
            {
                r2.Hp -= rst.damage;

                if (r2.View != null)
                {
                    r2.View.SetDamage(rst.damage);
                }

                r1.ExpGot += 2 + rst.damage / 5;
                //打死敌人获得额外经验
                if (r2.Hp <= 0)
                    r1.ExpGot += r2.Level * 10;

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
                    r1.MaxMp = Tools.Limit(r1.MaxMp + rst.addMaxMp, 0, GameConst.MAX_ROLE_MP);
                    int finalMp = Tools.Limit(r1.Mp + rst.addMp, 0, r1.MaxMp);
                    int deltaMp = finalMp - r1.Mp;
                    if (deltaMp >= 0)
                    {
                        r1.View.ShowAttackInfo($"<color=blue>内力+{deltaMp}</color>");
                        r1.Mp = finalMp;
                    }
                }
            }

            if (rst.poison > 0)
            {
                r2.Poison += rst.poison;
                if (r2.View != null)
                {
                    r2.View.ShowAttackInfo($"<color=green>中毒+{rst.poison}</color>");
                }

                r1.ExpGot += 1;
            }

            if (rst.depoison > 0)
            {
                r2.Poison -= rst.depoison;
                if (r2.View != null)
                {
                    r2.View.ShowAttackInfo($"<color=green>中毒-{rst.depoison}</color>");
                }

                r1.ExpGot += 1;
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

                r1.ExpGot += 1;
            }

            r2.Hurt += rst.hurt;
            r2.Hurt = Tools.Limit(r2.Hurt, 0, GameConst.MAX_HURT);
        }
    }
}
