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
using XLua;
using UnityEngine;

namespace Jyx2
{
    //用来解读Lua的Battle配置表
    [CSharpCallLua]
    public interface LBattleConfig
    {
        int Id {get;set;}
        string Name {get;set;}
        string MapScene {get;set;}
        int Exp {get;set;}
        int Music {get;set;}
        List<int> TeamMates {get;set;}
        List<int> AutoTeamMates {get;set;}
        List<int> Enemies {get;set;}
        List<RoleInstance> DynamicTeammate {get;set;}
        List<RoleInstance> DynamicEnemies {get;set;}
    }
    //用来解读Lua的Extra配置表
    [CSharpCallLua]
    public interface LExtraConfig
    {
        int Id {get;set;}
        int Weapon {get;set;}
        int Wugong {get;set;}
        int ExtraAttack {get;set;}
    }
    //用来解读Lua的Character配置表
    [CSharpCallLua]
    public interface LRoleSkill
    {
        int Id {get;set;}
        int Level {get;set;}
    }
    [CSharpCallLua]
    public interface LRoleItem
    {
        int Id {get;set;}
        int Count {get;set;}
    }
    [CSharpCallLua]
    public interface LRoleConfig
    {
        int Id {get;set;}
        string Name {get;set;}
        int Sexual {get;set;}
        int Pic {get;set;}
        int Pinde {get;set;}
        int IQ {get;set;}
        int MaxHp {get;set;}
        int MaxMp {get;set;}
        int HpInc {get;set;}
        int Level {get;set;}
        int Exp {get;set;}
        int MpType {get;set;}
        int Attack {get;set;}
        int Qinggong {get;set;}
        int Defence {get;set;}
        int Heal {get;set;}
        int UsePoison {get;set;}
        int DePoison {get;set;}
        int AntiPoison {get;set;}
        int Quanzhang {get;set;}
        int Yujian {get;set;}
        int Shuadao {get;set;}
        int Qimen {get;set;}
        int Anqi {get;set;}
        int Wuxuechangshi {get;set;}
        int AttackPoison {get;set;}
        int Zuoyouhubo {get;set;}
        List<LRoleSkill> Skills {get;set;}
        List<LRoleItem> Items {get;set;}
        int Weapon {get;set;}
        int Armor {get;set;}
        string LeaveStoryId {get;set;}
        string ModelFileKey {get;set;}
    }

    //用来解读Lua的Skill配置表
    [CSharpCallLua]
    public interface LSkillLevel
    {
        int Attack {get;set;}
        int SelectRange {get;set;}
        int AttackRange {get;set;}
        int AddMp {get;set;}
        int KillMp {get;set;}
    }
    [CSharpCallLua]
    public interface LSkillConfig
    {
        int Id {get;set;}
        string Name {get;set;}
        int DamageType {get;set;}
        int SkillCoverType {get;set;}
        int MpCost {get;set;}
        int Poison {get;set;}
        List<LSkillLevel> Levels {get;set;}
        string DisplayFileName {get;set;}
    }
    //用来解读Lua的Item配置表
    [CSharpCallLua]
    public interface LItemConfig
    {
        int Id {get;set;}
        string Name {get;set;}
    }

    public static class Jyx2LuaFunRef
    {
        [CSharpCallLua]
        public delegate int CallConfigInt(string configName, int Key, string FieldName);
        [CSharpCallLua]
        public delegate string CallConfigStr(string configName, int Key, string FieldName);
    }

    public class LuaToCsBridge
    {
        static private LuaEnv _luaEnv;
        static private LuaEnv LEnv
        {
            get
            {
                if (_luaEnv == null) _luaEnv = LuaManager.GetLuaEnv();
                return _luaEnv;
            }
        }

        [CSharpCallLua]
        public delegate void LuaInitAllRole(Dictionary<int,RoleInstance> AllRoles);

        // 用来读取Lua的配置文件
        public static Dictionary<int, LRoleConfig> CharacterTable;
        public static Dictionary<int, LSkillConfig> SkillTable;
        public static Dictionary<int, LItemConfig> ItemTable;
        public static Dictionary<int, LBattleConfig> BattleTable;
        public static Dictionary<int, LExtraConfig> ExtraTable;

        public static Jyx2LuaFunRef.CallConfigInt GetConfigValue;
        public static Jyx2LuaFunRef.CallConfigStr GetConfigString;

        public static void LuaToCsBridgeInit()
        {
            LuaConfRef();
        }

        public static void LuaToCsBridgeDispose()
        {
            LuaConfRefDispose();
        }

        public static void LuaConfRef()
        {
            GetConfigValue = LEnv.Global.GetInPath<Jyx2LuaFunRef.CallConfigInt>("Jyx2CSBridge.ConfigMgr.GetConfigValue");
            GetConfigString = LEnv.Global.GetInPath<Jyx2LuaFunRef.CallConfigStr>("Jyx2CSBridge.ConfigMgr.GetConfigValue");
            //ConfigData = LEnv.Global.GetInPath<Dictionary<string, object>>("Jyx2.ConfigMgr");

            List<string> ConfigList = LEnv.Global.GetInPath<List<string>>("Jyx2.ConfigMgr.ConfigList");
            if (ConfigList == null) return;

            CharacterTable = LEnv.Global.GetInPath<Dictionary<int, LRoleConfig>>("Jyx2.ConfigMgr.Character");
            SkillTable = LEnv.Global.GetInPath<Dictionary<int, LSkillConfig>>("Jyx2.ConfigMgr.Skill");
            ItemTable = LEnv.Global.GetInPath<Dictionary<int, LItemConfig>>("Jyx2.ConfigMgr.Item");
            BattleTable = LEnv.Global.GetInPath<Dictionary<int, LBattleConfig>>("Jyx2.ConfigMgr.Battle");
            ExtraTable = LEnv.Global.GetInPath<Dictionary<int, LExtraConfig>>("Jyx2.ConfigMgr.Extra");

        }
        public static void LuaConfRefDispose()
        {
            GetConfigValue = null;
            GetConfigString = null;
        }
    }
}
