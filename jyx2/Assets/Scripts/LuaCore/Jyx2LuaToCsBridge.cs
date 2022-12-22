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
    //用来解读Lua的Skill配置表
    [CSharpCallLua]
    public interface LSkillConfig
    {
        int Id {get;set;}
        string Name {get;set;}
        int DamageType {get;set;}
        int SkillCoverType {get;set;}
        int MpCost {get;set;}
        int Poison {get;set;}
        List<List<int>> Levels {get;set;}
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
        public delegate void CallRoleWithKey(RoleInstance role, int Key);

        [CSharpCallLua]
        public delegate void LuaInitAllRole(Dictionary<int,RoleInstance> AllRoles);

        // 用来读取Lua的配置文件
        public static Dictionary<int, LSkillConfig> SkillTable;
        public static Dictionary<int, LItemConfig> ItemTable;

        public static Jyx2LuaFunRef.CallConfigInt GetConfigValue;
        public static Jyx2LuaFunRef.CallConfigStr GetConfigString;

        public static Dictionary<int, object> ConfigData;

        public static void LuaConfRef()
        {
            GetConfigValue = LEnv.Global.GetInPath<Jyx2LuaFunRef.CallConfigInt>("Jyx2CSBridge.ConfigMgr.GetConfigValue");
            GetConfigString = LEnv.Global.GetInPath<Jyx2LuaFunRef.CallConfigStr>("Jyx2CSBridge.ConfigMgr.GetConfigValue");
            //ConfigData = LEnv.Global.GetInPath<Dictionary<string, object>>("Jyx2.ConfigMgr");
            SkillTable = LEnv.Global.GetInPath<Dictionary<int, LSkillConfig>>("Jyx2.ConfigMgr.Skill");
            ItemTable = LEnv.Global.GetInPath<Dictionary<int, LItemConfig>>("Jyx2.ConfigMgr.Item");
            Debug.Log(ItemTable[1].Name);
        }
    }
}
