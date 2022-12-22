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
using Jyx2Configs;
using NUnit.Framework;
using UnityEngine;
using XLua;

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

        // 与一个Lua配置表中的Skill绑定
        private LSkillConfig _data;
        public LSkillConfig Data
        {
            get{
                if (_data == null)
                    _data = GetSkill();
                return _data;
            }
            set{
                _data = value;
            }
        }
        public SkillInstance()
        {
        }

        public SkillInstance(Jyx2ConfigCharacterSkill s)
        {
            Key = s.Id;
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

            if(level > _levels.Count)
            {
                Debug.LogError("skill level error");
                return null;
            }
            return _levels[level - 1];
        }

        public string Name
        {
            get
            {
                return Data.Name;
            }
        }

        //public Jyx2ConfigSkill GetSkill(Jyx2ConfigItem _anqi = null)
        public LSkillConfig GetSkill(Jyx2ConfigItem _anqi = null)
        {
            //var skillT = GameConfigDatabase.Instance.Get<Jyx2ConfigSkill>(Key);
            var skillT = LuaToCsBridge.SkillTable[Key];

            //暗器
            if (_anqi != null)
            {
                skillT.Poison = _anqi.ChangePoisonLevel;

                foreach (var sl in _levels)
                {
                    sl.Attack = Mathf.Abs(_anqi.AddHp);
                }
            }
            return skillT;
        }

        public List<Jyx2ConfigSkillLevel> GetLevels()
        {
            var _levels = new List<Jyx2ConfigSkillLevel>();
            foreach (var lvl in _data.Levels)
            {
                if (lvl.Count != 5) continue;
                var skillLevel = new Jyx2ConfigSkillLevel();
                skillLevel.Attack = lvl[0];
                skillLevel.SelectRange = lvl[1];
                skillLevel.AttackRange = lvl[2];
                skillLevel.AddMp = lvl[3];
                skillLevel.KillMp = lvl[4];
                _levels.Add(skillLevel);
            }
            /*var _levelArr = _data.Levels.Split('|');
            foreach (var _level in _levelArr)
            {
                var _levelArr2 = _level.Split(',');
                if (_levelArr2.Length != 5) continue;
                var skillLevel = new Jyx2ConfigSkillLevel();
                skillLevel.Attack = int.Parse(_levelArr2[0]);
                skillLevel.SelectRange = int.Parse(_levelArr2[1]);
                skillLevel.AttackRange = int.Parse(_levelArr2[2]);
                skillLevel.AddMp = int.Parse(_levelArr2[3]);
                skillLevel.KillMp = int.Parse(_levelArr2[4]);
                _levels.Add(skillLevel);
            }*/
            return _levels;
        }

        public void ResetSkill()
        {
            _data = null;
        }

        List<Jyx2ConfigSkillLevel> levels;

        List<Jyx2ConfigSkillLevel> _levels
        {
            get
            {
                levels = GetLevels();
                return levels;
            }
            set {
                levels = value;
            }
        }

        public SkillCoverType CoverType
        {
            get
            {
                switch ((int)_data.SkillCoverType)
                {
                    case 0:
                        return SkillCoverType.POINT;
                    case 1:
                        return SkillCoverType.LINE;
                    case 2:
                        return SkillCoverType.CROSS;
                    case 3:
                        return SkillCoverType.RECT;
                    case 4:
                        return SkillCoverType.RHOMBUS;
                    default:
                        Debug.LogError("invalid skill cover type:" + _data.SkillCoverType);
                        return SkillCoverType.INVALID;
                }
            }
        }

        public int CastSize
        {
            get
            {
                if ((int)_data.SkillCoverType == 1) //直线
                    return 1;

                if ((int)_data.SkillCoverType == 2) //中心星型
                    return 0;

                return GetSkillLevelInfo().SelectRange;
            }
        }

        public int CoverSize
        {
            get
            {
                if ((int)_data.SkillCoverType == 1 || (int)_data.SkillCoverType == 2)
                    return GetSkillLevelInfo().SelectRange;
                return GetSkillLevelInfo().AttackRange;
            }
        }


        public Jyx2SkillDisplayAsset GetDisplay()
        {
            var DisplayFileName = _data.DisplayFileName;
            if(!string.IsNullOrWhiteSpace(DisplayFileName))
                return Jyx2SkillDisplayAsset.Get(DisplayFileName);
            else
                return Jyx2SkillDisplayAsset.Get(Name);
        }

        public int GetCoolDown()
        {
            return 0;
        }
    }

}
