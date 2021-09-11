/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSFrameWork.ConfigTable;
using HSFrameWork.SPojo;
using Jyx2;
using UnityEngine;

namespace Jyx2
{
    /// <summary>
    /// JYX2的武功实例
    /// </summary>
    public class WugongInstance : SaveablePojo
    {
        public WugongInstance() { }
        public WugongInstance(Jyx2RoleWugong w)
        {
            Key = w.Id;
            Level = w.Level;
            GetSkill(); //初始化
        }
        public WugongInstance(int magicId)
        {
            Key = magicId;
            Level = 0;
            GetSkill();
        }

        public WugongInstance(Jyx2Item item, int magicId)
        {
            Key = magicId;
            Level = 0;
            GetSkill(item);
        }

        public int Key
        {
            get { return Get("Key", 0); }
            set {
                Save("Key", value);
                _skill = null;
                //GetSkill();
            }
        }

        public int Level
        {
            get { return Get("Level", 0); }
            set { Save("Level", value); }
        }

        public int GetLevel()
        {
            return Level / 100 + 1;
        }

        public Jyx2SkillLevel GetSkillLevelInfo(int level = -1)
        {
            if(level < 1)
            {
                level = GetLevel();
            }
            if(level > _skill.SkillLevels.Count)
            {
                Debug.LogError("skill level error");
                return null;
            }
            return _skill.SkillLevels[level - 1];
        }

        public string Name
        {
            get
            {
                return GetSkill().Name;
            }
        }

        public Jyx2Skill GetSkill(Jyx2Item _anqi = null)
        {
            var skillT = ConfigTable.Get<Jyx2Skill>(Key);

			//暗器
			if (_anqi != null)
			{
				skillT.Animation = _anqi.AnqiAnimation;
				skillT.Poison = _anqi.ChangePoisonLevel;
                
				foreach (Jyx2SkillLevel sl in _skill.SkillLevels)
				{
					sl.Attack = Mathf.Abs(_anqi.AddHp);
				}
			}
            return skillT;
        }

        public void ResetSkill()
        {
            _skill = null;
        }

		Jyx2Skill skill;
        Jyx2Skill _skill{
			get {
				if(skill==null) skill=GetSkill();
				return skill;
			}
			set {
				skill=value;
			}
		}

        public SkillCoverType CoverType
        {
            get
            {
                switch (_skill.SkillCoverType)
                {
                    case 0:
                        return SkillCoverType.POINT;
                    case 1:
                        return SkillCoverType.LINE;
                    case 2:
                        return SkillCoverType.CROSS;
                    case 3:
                        return SkillCoverType.FACE;
                    default:
                        Debug.LogError("invalid skill cover type:" + _skill.SkillCoverType);
                        return SkillCoverType.INVALID;
                }
            }
        }

        public int CastSize
        {
            get
            {
                if (_skill.SkillCoverType == 1) //直线
                    return 1;

                if (_skill.SkillCoverType == 2) //中心星型
                    return 0;

                return GetSkillLevelInfo().SelectRange;
            }
        }

        public int CoverSize
        {
            get
            {
                if (_skill.SkillCoverType == 1 || _skill.SkillCoverType == 2)
                    return GetSkillLevelInfo().SelectRange;
                return GetSkillLevelInfo().AttackRange + 1;
            }
        }


        public Jyx2SkillDisplayAsset GetDisplay()
        {
			return _skill.Display;
        }

        public int GetCoolDown()
        {
            return 0;
        }
    }

}
