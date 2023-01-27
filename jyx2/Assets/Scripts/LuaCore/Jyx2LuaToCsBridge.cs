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
using Cysharp.Threading.Tasks;
using XLua;
using UnityEngine;

namespace Jyx2
{
    /// <summary>
    /// 用来解读Lua的Battle配置表
    /// </summary>
    [CSharpCallLua]
    public interface LBattleConfig
    {
        int Id {get;set;}
        string Name {get;set;}
        //地图
        string MapScene {get;set;}
        //获得经验
        int Exp {get;set;}
        //音乐
        int Music {get;set;}
        //队友
        List<int> TeamMates {get;set;}
        //自动选择的队友
        List<int> AutoTeamMates {get;set;}
        //敌人
        List<int> Enemies {get;set;}
        //动态生成队友
        List<RoleInstance> DynamicTeammate {get;set;}
        //动态生成敌人
        List<RoleInstance> DynamicEnemies {get;set;}
    }

    /// <summary>
    /// 用来在Cs侧生成LBattleConfig对象
    /// </summary>
    public class CsBattleConfig : LBattleConfig
    {
        public int Id {get;set;}
        public string Name{get;set;}

        //地图
        public string MapScene{get;set;}
        
        //获得经验
        public int Exp{get;set;}
        
        //音乐
        public int Music{get;set;}
        
        //队友
        public List<int> TeamMates{get;set;}

        //自动队友
        public List<int> AutoTeamMates{get;set;}

        //敌人
        public List<int> Enemies{get;set;}

        //动态生成队友
        public List<RoleInstance> DynamicTeammate {get;set;}
        //动态生成的敌人
        public List<RoleInstance> DynamicEnemies {get;set;}

        public void InitForDynamicData()
        {
            DynamicTeammate = new List<RoleInstance>();
            DynamicEnemies = new List<RoleInstance>();
        }
        
        public CsBattleConfig()
        {
        }

        public CsBattleConfig(LBattleConfig b)
        {

            Id = b.Id;
            Name = b.Name;
            MapScene = b.MapScene;
            Exp = b.Exp;
            Music = b.Music;
            TeamMates = b.TeamMates;
            AutoTeamMates = b.AutoTeamMates;
            Enemies = b.Enemies;

            DynamicTeammate = b.DynamicTeammate;
            DynamicEnemies = b.DynamicEnemies;
        }
    }

    /// <summary>
    /// 用来解读Lua的Extra配置表
    /// </summary>
    [CSharpCallLua]
    public interface LExtraConfig
    {
        int Id {get;set;}
        //武器
        int Weapon {get;set;}
        //武功
        int Wugong {get;set;}
        //加成攻击
        int ExtraAttack {get;set;}
    }

    /// <summary>
    /// 用来解读Lua的角色技能信息
    /// </summary>
    [CSharpCallLua]
    public interface LRoleSkill
    {
        int Id {get;set;}
        int Level {get;set;}
    }

    /// <summary>
    /// 用来解读Lua的角色物品信息
    /// </summary>
    [CSharpCallLua]
    public interface LRoleItem
    {
        int Id {get;set;}
        int Count {get;set;}
    }

    /// <summary>
    /// 用来储存角色Item信息而不影响配置表
    /// </summary>
    [Serializable]
    public class CsRoleItem : LRoleItem
    {
        [SerializeField] public int Id {get;set;}
        [SerializeField] public int Count {get;set;}

        public CsRoleItem()
        {
        }

        public CsRoleItem(LRoleItem itm)
        {
            Id = itm.Id;
            Count = itm.Count;
        }
    }

    /// <summary>
    /// 解读Lua配置的角色信息
    /// </summary>
    [CSharpCallLua]
    public interface LRoleConfig
    {
        int Id {get;set;}
        string Name {get;set;}
        //性别
        int Sexual {get;set;}
        //头像
        int Pic {get;set;}
        //品德
        int Pinde {get;set;}
        //资质
        int IQ {get;set;}
        //生命上限
        int MaxHp {get;set;}
        //内力上限
        int MaxMp {get;set;}
        //生命增长
        int HpInc {get;set;}
        //开场等级
        int Level {get;set;}
        //经验
        int Exp {get;set;}
        //内力性质 - 0:阴 1:阳 2:调和
        int MpType {get;set;}
        //攻击力
        int Attack {get;set;}
        //轻功
        int Qinggong {get;set;}
        //防御力
        int Defence {get;set;}
        //医疗
        int Heal {get;set;}
        //用毒
        int UsePoison {get;set;}
        //解毒
        int DePoison {get;set;}
        //抗毒
        int AntiPoison {get;set;}
        //拳掌
        int Quanzhang {get;set;}
        //御剑
        int Yujian {get;set;}
        //耍刀
        int Shuadao {get;set;}
        //特殊兵器
        int Qimen {get;set;}
        //暗器技巧
        int Anqi {get;set;}
        //武学常识
        int Wuxuechangshi {get;set;}
        //攻击带毒
        int AttackPoison {get;set;}
        //左右互搏
        int Zuoyouhubo {get;set;}
        //所会武功
        List<LRoleSkill> Skills {get;set;}
        //携带道具
        List<LRoleItem> Items {get;set;}
        //武器
        int Weapon {get;set;}
        //防具
        int Armor {get;set;}
        //队友离场对话
        string LeaveStoryId {get;set;}
        //模型文件名称
        string ModelFileKey {get;set;}
    }

    /// <summary>
    /// 商品信息
    /// </summary>
    [CSharpCallLua]
    public interface LShopItem
    {
        //ID
        int Id {get;set;}
        //数量
        int Count {get;set;}
        //价格
        int Price {get;set;}
    }

    /// <summary>
    /// 用来在Cs侧生成商品对象
    /// </summary>
    public class CsShopItem :LShopItem
    {
        public int Id {get;set;}
        public int Count {get;set;}
        public int Price {get;set;}

        public CsShopItem(LShopItem itm)
        {
            Id = itm.Id;
            Count = itm.Count;
            Price = itm.Price;
        }
    }

    /// <summary>
    /// 用来解读Lua的Shop配置表
    /// </summary>
    [CSharpCallLua]
    public interface LShopConfig
    {
        int Id {get;set;}
        //商品列表
        List<LShopItem> ShopItems {get;set;}
        //商人触发器编号
        int Trigger {get;set;}
    }

    /// <summary>
    /// 技能等级信息
    /// </summary>
    [CSharpCallLua]
    public interface LSkillLevel
    {
        //攻击力
        int Attack {get;set;}
        //移动范围
        int SelectRange {get;set;}
        //杀伤范围
        int AttackRange {get;set;}
        //加内力
        int AddMp {get;set;}
        //杀伤内力
        int KillMp {get;set;}
    }

    /// <summary>
    /// 用来解读Lua的Skill配置
    /// </summary>
    [CSharpCallLua]
    public interface LSkillConfig
    {
        int Id {get;set;}
        string Name {get;set;}
        //伤害类型
        //普通 = 0, 吸内 = 1, 用毒 = 2, 解毒 = 3, 医疗 = 4
        int DamageType {get;set;}
        //攻击范围类型
        //点攻击 = 0, 线攻击 = 1, 十字攻击 = 2, 面攻击 = 3,
        int SkillCoverType {get;set;}
        //消耗内力点数
        int MpCost {get;set;}
        //带毒点数
        int Poison {get;set;}
        //技能等级配置
        List<LSkillLevel> Levels {get;set;}
        //技能外观配置
        string DisplayFileName {get;set;}
    }

    /// <summary>
    /// 用来复制存储一份LevelInfo，在修改暗器伤害时与原配置表隔离
    /// </summary>
    public class CsSkillLevel:LSkillLevel
    {
        public int Attack {get;set;}
        public int SelectRange {get;set;}
        public int AttackRange {get;set;}
        public int AddMp {get;set;}
        public int KillMp {get;set;}

        public CsSkillLevel()
        {
        }

        public CsSkillLevel(LSkillLevel sk)
        {
            Attack = sk.Attack;
            SelectRange = sk.SelectRange;
            AttackRange = sk.AttackRange;
            AddMp = sk.AddMp;
            KillMp = sk.KillMp;
        }
    }

    /// <summary>
    /// 物品分类表
    /// </summary>
    public enum Jyx2ItemType
    {
        TaskItem = 0, //道具
        Equipment = 1, //装备
        Book = 2, //经书
        Costa = 3, //消耗品
        Anqi = 4, //暗器
    }

    /// <summary>
    /// 用来解读Lua的Item配置表
    /// </summary>
    [CSharpCallLua]
    public interface LItemConfig
    {
        int Id {get;set;}
        string Name {get;set;}
        //物品说明
        string Desc {get;set;}
        //物品类型
        //道具 = 0, 装备 = 1, 经书 = 2, 消耗品 = 3, 暗器 = 4,
        int ItemType {get;set;}
        //装备类型
        //不是装备 = -1, 武器 = 0, 防具 = 1
        int EquipmentType {get;set;}
        //练出武功
        int Skill {get;set;}
        //加生命
        int AddHp {get;set;}
        //加生命最大值
        int AddMaxHp {get;set;}
        //加中毒解毒
        int ChangePoisonLevel {get;set;}
        //加体力
        int AddTili {get;set;}
        //改变内力性质
        int ChangeMPType {get;set;}
        //加内力
        int AddMp {get;set;}
        //加内力最大值
        int AddMaxMp {get;set;}
        //加攻击力
        int Attack {get;set;}
        //加轻功
        int Qinggong {get;set;}
        //加防御力
        int Defence {get;set;}
        //加医疗
        int Heal {get;set;}
        //加使毒
        int UsePoison {get;set;}
        //加解毒
        int DePoison {get;set;}
        //加毒抗
        int AntiPoison {get;set;}
        //加拳掌
        int Quanzhang {get;set;}
        //加御剑
        int Yujian {get;set;}
        //加耍刀
        int Shuadao {get;set;}
        //加特殊兵器
        int Qimen {get;set;}
        //加暗器技巧
        int Anqi {get;set;}
        //加武学常识
        int Wuxuechangshi {get;set;}
        //加品德
        int AddPinde {get;set;}
        //左右互搏
        int Zuoyouhubo {get;set;}
        //加功夫带毒
        int AttackPoison {get;set;}
        //仅修炼人物
        int OnlySuitableRole {get;set;}
        //需内力性质
        int NeedMPType {get;set;}
        //需内力
        int ConditionMp {get;set;}
        //需攻击力
        int ConditionAttack {get;set;}
        //需轻功
        int ConditionQinggong {get;set;}
        //需用毒
        int ConditionPoison {get;set;}
        //需医疗
        int ConditionHeal {get;set;}
        //需解毒
        int ConditionDePoison {get;set;}
        //需拳掌
        int ConditionQuanzhang {get;set;}
        //需御剑
        int ConditionYujian {get;set;}
        //需耍刀
        int ConditionShuadao {get;set;}
        //需特殊兵器
        int ConditionQimen {get;set;}
        //需暗器
        int ConditionAnqi {get;set;}
        //需资质
        int ConditionIQ {get;set;}
        //需经验
        int NeedExp {get;set;}
        //需自宫: -1不需要, 1需要
        int NeedCastration {get;set;}
        //练出物品所需经验
        int GenerateItemNeedExp {get;set;}
        //练出物品需材料
        int GenerateItemNeedCost {get;set;}
        //练出物品
        string GenerateItems {get;set;}

        //获取物品图片地址
        string GetPic();
        //物品是武器
        bool IsWeapon();
        //物品是防具
        bool IsArmor();
        //物品是秘籍
        bool IsBook();
        //物品无人使用
        bool NoItemUser();
        //物品是否正被特定的人使用
        bool IsBeingUsedBy(int roleId);
        bool IsBeingUsedBy(RoleInstance role);
        //获取物品的类型
        Jyx2ItemType GetItemType();
    }

    /// <summary>
    /// 用来解读Lua的Map配置表
    /// </summary>
    [CSharpCallLua]
    public interface LMapConfig
    {
        int Id {get;set;}
        string Name {get;set;}
        //地图文件名
        string MapScene {get;set;}
        //跳转场景
        string TransportToMap {get;set;}
        //进门音乐
        int InMusic {get;set;}
        //出门音乐
        int OutMusic {get;set;}
        //进入条件: 0-开局开启 1-开局关闭
        int EnterCondition {get;set;}
        //标签
        string Tags {get;set;}
        //脚本绑定
        string BindScript {get;set;}
        //强行指定离开音乐
        int ForceSetLeaveMusicId {get;set;}

        //获取离开音乐
        int GetOutMusic();
        //获取特定Tag字符串
        string GetTagValue(string str);
        //获取跳转场景号
        int GetTransportToMapValue(string str);
        //获取地图显示名
        string GetShowName();
        //获取游戏启动地图
        LMapConfig GetGameStartMap();
        //根据地图文件名获取地图
        LMapConfig GetMapBySceneName(string str);
        //根据地图显示名获取地图
        LMapConfig GetMapByName(string str);
    }

    /// <summary>
    /// 用来解读Lua的Settings配置表
    /// </summary>
    [CSharpCallLua]
    public interface LSettingsConfig
    {
        int Id {get;set;}
        //配置项名称
        string Name {get;set;}
        //值
        string Value {get;set;}
    }

    public class LuaToCsBridge
    {
        static private LuaEnv _luaEnv;
        static private LuaEnv LEnv {get;set;}

#region Lua Configs
        // 用来读取Lua的配置文件
        public static Dictionary<int, LRoleConfig> CharacterTable;
        public static Dictionary<int, LSkillConfig> SkillTable;
        public static Dictionary<int, LItemConfig> ItemTable;
        public static Dictionary<int, LBattleConfig> BattleTable;
        public static Dictionary<int, LExtraConfig> ExtraTable;
        public static Dictionary<int, LMapConfig> MapTable;
        public static Dictionary<int, LShopConfig> ShopTable;
        public static Dictionary<int, LSettingsConfig> SettingsTable;
#endregion

        // 下面两个Lua函数用来在c#侧调用Lua侧的函数
        public static LuaFunction cs_await;
        public static LuaFunction cs_calllua;

        public static bool IsLuaFunExists(string funName)
        {
            return true;
        }
        //暂时决定不用这个函数来初始化，而是在需要的时候直接分别运行不同的初始化方法
        public static void LuaToCsBridgeInit()
        {
            //用来在cs侧调用Lua函数
            LEnv = LuaManager.GetLuaEnv();
            cs_await = LEnv.Global.GetInPath<LuaFunction>("jy_utils.cs_await");
            cs_calllua = LEnv.Global.GetInPath<LuaFunction>("jy_utils.cs_calllua");
        }

        public static void LuaToCsBridgeDispose()
        {
            cs_await = null;
            cs_calllua = null;
            LuaConfigToCsDispose();
        }

        public static void LuaConfigToCsInit()
        {
            LEnv = LuaManager.GetLuaEnv();
            List<string> ConfigList = LEnv.Global.GetInPath<List<string>>("Jyx2.ConfigMgr.ConfigList");
            if (ConfigList == null) return;

            CharacterTable = LEnv.Global.GetInPath<Dictionary<int, LRoleConfig>>("Jyx2.ConfigMgr.Character");
            SkillTable = LEnv.Global.GetInPath<Dictionary<int, LSkillConfig>>("Jyx2.ConfigMgr.Skill");
            ItemTable = LEnv.Global.GetInPath<Dictionary<int, LItemConfig>>("Jyx2.ConfigMgr.Item");
            BattleTable = LEnv.Global.GetInPath<Dictionary<int, LBattleConfig>>("Jyx2.ConfigMgr.Battle");
            ExtraTable = LEnv.Global.GetInPath<Dictionary<int, LExtraConfig>>("Jyx2.ConfigMgr.Extra");
            MapTable = LEnv.Global.GetInPath<Dictionary<int, LMapConfig>>("Jyx2.ConfigMgr.Map");
            ShopTable = LEnv.Global.GetInPath<Dictionary<int, LShopConfig>>("Jyx2.ConfigMgr.Shop");
            SettingsTable = LEnv.Global.GetInPath<Dictionary<int, LSettingsConfig>>("Jyx2.ConfigMgr.Settings");

            TestMethod();
        }
        public static async UniTaskVoid TestMethod()
        {
        }
        public static void LuaConfigToCsDispose()
        {
            CharacterTable = null;
            SkillTable = null;
            ItemTable = null;
            BattleTable = null;
            ExtraTable = null;
            MapTable = null;
            ShopTable = null;
            SettingsTable = null;
        }
    }
}
