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

        //存储Level各等级数据，在运行时修改不会影响原配置表
        List<LSkillLevel> _levelInfo;
        public List<LSkillLevel> LevelInfo
        {
            get{
                if (_levelInfo == null)
                    _levelInfo = GetLevelInfo();
                return _levelInfo;
            }
            set{
                _levelInfo = value;
            }
        }

        public void ResetSkill()
        {
            _data = null;
        }

        public string Name
        {
            get
            {
                return Data.Name;
            }
        }

        public SkillInstance()
        {
        }

        public SkillInstance(LRoleSkill s)
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

        public SkillInstance(LItemConfig item, int magicId)
        {
            Key = magicId;
            Level = 0;
            GetSkill(item);
        }

        public int GetLevel()
        {
            return Level / 100 + 1;
        }

        public LSkillLevel GetSkillLevelInfo(int level = -1)
        {
            if(level < 1)
            {
                level = GetLevel();
            }

            if(level > LevelInfo.Count)
            {
                Debug.LogError("skill level error");
                return null;
            }
            return LevelInfo[level - 1];
        }

        //实际上这个函数里针对暗器的操作没什么用，暗器的相关数据都在AnqiSkillCastInstance里处理了
        public LSkillConfig GetSkill(LItemConfig _anqi = null)
        {
            var skillT = LuaToCsBridge.SkillTable[Key];

            //暗器
            if (_anqi != null)
            {
                skillT.Poison = _anqi.ChangePoisonLevel;

                foreach (var sl in LevelInfo)
                {
                    sl.Attack = Mathf.Abs(_anqi.AddHp);
                }
            }
            return skillT;
        }

        public List<LSkillLevel> GetLevelInfo()
        {
            var _levels = new List<LSkillLevel>();
            foreach (var lvl in Data.Levels)
            {
                var skillLevel = new CsSkillLevel(lvl);
                _levels.Add(skillLevel);
            }
            return _levels;
        }

        //施放范围类型
        public SkillCoverType CoverType
        {
            get
            {
                switch ((int)Data.SkillCoverType)
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

        //技能施放范围
        public int CastSize
        {
            get
            {
                if ((int)Data.SkillCoverType == 1) //直线
                    return 1;

                if ((int)Data.SkillCoverType == 2) //中心星型
                    return 0;

                return GetSkillLevelInfo().SelectRange;
            }
        }

        //杀伤范围
        public int CoverSize
        {
            get
            {
                if ((int)Data.SkillCoverType == 1 || (int)Data.SkillCoverType == 2)
                    return GetSkillLevelInfo().SelectRange;
                return GetSkillLevelInfo().AttackRange;
            }
        }

        //获取技能外观
        public Jyx2SkillDisplayAsset GetDisplay()
        {
            if(!string.IsNullOrWhiteSpace(Data.DisplayFileName))
                return Jyx2SkillDisplayAsset.Get(Data.DisplayFileName);
            else
                return Jyx2SkillDisplayAsset.Get(Data.Name);
        }

        public int GetCoolDown()
        {
            return 0;
        }
    }

}
