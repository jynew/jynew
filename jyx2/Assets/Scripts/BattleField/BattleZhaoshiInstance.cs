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
using Jyx2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jyx2
{
    public class BattleZhaoshiInstance
    {
        public const int MAX_MAGIC_LEVEL_INDEX = 9;

        protected BattleZhaoshiInstance()
        {
        }

        public BattleZhaoshiInstance(WugongInstance wugong)
        {
            Data = wugong;
            level = wugong.GetLevel();
            Key = wugong.Key.ToString();
        }

        public enum ZhaoshiStatus
        {
            OK, //正常
            CD, //CD中
        }

        public WugongInstance Data;

        public Jyx2Item Anqi;

        public string Key;

        public int level;

        public int CurrentCooldown = 0;

        public static int TimeTickCoolDown = GameConst.ACTION_SP;

        public void CastCD()
        {
            CurrentCooldown += Data.GetCoolDown() * TimeTickCoolDown;
        }

        /// <summary>
        /// JYX2使用技能消耗的逻辑
        /// </summary>
        public void CastCost(RoleInstance role)
        {
            int damageType = GetDamageType();
            if(damageType == 0 || damageType == 1)//普通攻击、吸内
            {
                int level_index = this.level;
                role.Tili = Tools.Limit(role.Tili - 3, 0, 100);
                role.Mp = Tools.Limit(role.Mp - this.calNeedMP(level_index), 0, role.MaxMp);

                int levelAdd = Tools.Limit(1 + Tools.GetRandomInt(0, 2), 0, 100 * 10);
                //levelAdd += Tools.GetRandomInt(50, 100);//for test

                //空挥升级
                //己方队员，并且武功等级提升了
                if (role.team == 0 && (Data.Level / 100) < ((Data.Level + levelAdd) / 100))
                {
                    StoryEngine.Instance.DisplayPopInfo(
                        $"{role.Name}的{this.Data.Name}升到{((Data.Level + levelAdd) / 100) + 1}级!");
                }
                
                //JYX2:最多10级，每级100
                this.Data.Level += levelAdd;
                this.Data.Level = Tools.Limit(this.Data.Level, 0, GameConst.MAX_SKILL_LEVEL);

            }else if(damageType ==2)//用毒
            {
                role.Tili = Tools.Limit(role.Tili - 3, 0, 100);
            }else if(damageType == 3)//解毒
            {
                role.Tili = Tools.Limit(role.Tili - 5, 0, 100);
            }else if(damageType == 4)//医疗
            {
                role.Tili = Tools.Limit(role.Tili - 5, 0, 100);
            }
            
            //暗器，扣除道具
            if (this is AnqiZhaoshiInstance)
            {
                if (!role.isAI)
                {
                    GameRuntimeData.Instance.AddItem(Anqi.Id, -1);
                }
                else
                {
                    role.AddItem(int.Parse(Anqi.Id), -1);
                }
            }
        }

        /// <summary>
        /// JYX2:Magic int calNeedMP(int level_index) { return NeedMP * ((level_index + 2) / 2); }
        /// </summary>
        /// <param name="level_index"></param>
        /// <returns></returns>
        int calNeedMP(int level_index) { return Data.GetSkill().MpCost * ((level_index + 2) / 2); }

        public void TimeRun()
        {
            if (CurrentCooldown > 0)
                CurrentCooldown -= 1;
        }

        public ZhaoshiStatus GetStatus()
        {
            if (CurrentCooldown > 0)
                return ZhaoshiStatus.CD;
            return ZhaoshiStatus.OK;
        }


        public virtual bool IsCastToEnemy()
        {
            return true;
        }


        public int calMaxLevelIndexByMP(int mp, int max_level)
        {
            max_level = limit(max_level, 0, MAX_MAGIC_LEVEL_INDEX);
            int needMp = Data.GetSkill().MpCost;
            if(needMp <= 0)
            {
                return max_level;
            }
            int level = limit(mp / (needMp * 2) * 2 - 1, 0, max_level);
            return level;
        }

        public int getMpCost()
        {
            int needMp = Data.GetSkill().MpCost;
            return needMp * (level + 2) / 2; // 内力消耗计算公式
        }

        private int limit(int v,int v1,int v2)
        {
            if (v < v1) v = v1;
            if (v > v2) v = v2;
            return v;
        }

        public virtual bool IsAttack()
        {
            return true;
        }

        public virtual int GetCastSize()
        {
            return Data.CastSize;
        }

        public virtual SkillCoverType GetCoverType()
        {
            return Data.CoverType;
        }

        public virtual int GetCoverSize()
        {
            return Data.CoverSize;
        }

        public virtual int GetDamageType()
        {
            return Data.GetSkill().DamageType;
        }
    }

    /// <summary>
    /// JYX2:用毒
    /// </summary>
    public class PoisonZhaoshiInstance : BattleZhaoshiInstance
    {
        public PoisonZhaoshiInstance(int lv)
        {
            _level = lv;
            Data = new WugongInstance(93);
        }

        int _level;

        public override bool IsAttack()
        {
            return false;
        }

        public override SkillCoverType GetCoverType()
        {
            return SkillCoverType.POINT;
        }

        public override int GetCoverSize()
        {
            return 1;
        }

        public override int GetCastSize()
        {
            //JYX2 BattleScene.h:virtual int calActionStep(int ability) { return ability / 15 + 1; }     //依据能力值计算行动的范围步数
            return _level / 15 + 1;
        }
    }

    /// <summary>
    /// JYX2：解毒
    /// </summary>
    public class DePoisonZhaoshiInstance : BattleZhaoshiInstance
    {
        public DePoisonZhaoshiInstance(int lv)
        {
            _level = lv;
            Data = new WugongInstance(94);
        }

        int _level;

        public override bool IsAttack()
        {
            return false;
        }

        public override bool IsCastToEnemy()
        {
            return false;
        }

        public override SkillCoverType GetCoverType()
        {
            return SkillCoverType.POINT;
        }

        public override int GetCoverSize()
        {
            return 1;
        }

        public override int GetCastSize()
        {
            //JYX2 BattleScene.h:virtual int calActionStep(int ability) { return ability / 15 + 1; }     //依据能力值计算行动的范围步数
            return _level / 15 + 1;
        }
    }

    /// <summary>
    /// JYX2：医疗
    /// </summary>
    public class HealZhaoshiInstance : BattleZhaoshiInstance
    {
        public HealZhaoshiInstance(int lv)
        {
            _level = lv;
            Data = new WugongInstance(95);
        }

        int _level;

        public override bool IsAttack()
        {
            return false;
        }

        public override bool IsCastToEnemy()
        {
            return false;
        }

        public override SkillCoverType GetCoverType()
        {
            return SkillCoverType.POINT;
        }

        public override int GetCoverSize()
        {
            return 1;
        }

        public override int GetCastSize()
        {
            //JYX2 BattleScene.h:virtual int calActionStep(int ability) { return ability / 15 + 1; }     //依据能力值计算行动的范围步数
            return _level / 15 + 1;  
        }
    }

    /// <summary>
    /// JYX2:暗器
    /// </summary>
    public class AnqiZhaoshiInstance : BattleZhaoshiInstance
    {

        public AnqiZhaoshiInstance(int lv, Jyx2Item item)
        {
            _level = lv;
            Anqi = item;
            Data = new WugongInstance(item);
        }

        int _level;

        public override bool IsAttack()
        {
            return false;
        }

        public override SkillCoverType GetCoverType()
        {
            return SkillCoverType.POINT;
        }

        public override int GetCoverSize()
        {
            return 1;
        }

        public override int GetCastSize()
        {
            //JYX2 BattleScene.h:virtual int calActionStep(int ability) { return ability / 15 + 1; }     //依据能力值计算行动的范围步数
            return _level / 15 + 1;
        }
    }
}
