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
using Jyx2Configs;
using UnityEngine;

namespace Jyx2
{
    /// <summary>
    /// JYX2的武功实例
    /// </summary>
    [Serializable]
    public class SkillInstance
    {
        #region 存档数据定义
        [SerializeField] public int Key;
        [SerializeField] public int Level;
        #endregion

        public SkillInstance()
        {
        }

        public SkillInstance(Jyx2ConfigCharacterSkill s)
        {
            Key = s.Skill.Id;
            Level = s.Level;
        }
        
        public SkillInstance(int magicId)
        {
            Key = magicId;
            Level = 0;
            GetSkill();
        }

        public SkillInstance(Jyx2ConfigItem item, int magicId)
        {
            Key = magicId;
            Level = 0;
            GetSkill(item);
        }

        public int GetLevel()
        {
            return Level / 100 + 1;
        }

        public Jyx2ConfigSkillLevel GetSkillLevelInfo(int level = -1)
        {
            if(level < 1)
            {
                level = GetLevel();
            }
            if(level > _skill.Levels.Count)
            {
                Debug.LogError("skill level error");
                return null;
            }
            return _skill.Levels[level - 1];
        }

        public string Name
        {
            get
            {
                return GetSkill().Name;
            }
        }

        public Jyx2ConfigSkill GetSkill(Jyx2ConfigItem _anqi = null)
        {
            var skillT = GameConfigDatabase.Instance.Get<Jyx2ConfigSkill>(Key);

			//暗器
			if (_anqi != null)
			{
                skillT.Poison = _anqi.ChangePoisonLevel;
                
				foreach (var sl in _skill.Levels)
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

        Jyx2ConfigSkill skill;
        Jyx2ConfigSkill _skill{
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
                switch ((int)_skill.SkillCoverType)
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
                if ((int)_skill.SkillCoverType == 1) //直线
                    return 1;

                if ((int)_skill.SkillCoverType == 2) //中心星型
                    return 0;

                return GetSkillLevelInfo().SelectRange;
            }
        }

        public int CoverSize
        {
            get
            {
                if ((int)_skill.SkillCoverType == 1 || (int)_skill.SkillCoverType == 2)
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
