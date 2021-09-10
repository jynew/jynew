/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PropertyItem 
{
    public int ID;
    public string Name;
    public string PropertyName;
    public int DefaulMax;//初始化的时候的最大值
    public int DefaulMin;//初始化的时候的最小值

    public PropertyItem(int ID, string PropertyName, string Name, int DefaulMax, int DefaulMin)
    {
        this.ID = ID;
        this.Name = Name;
        this.PropertyName = PropertyName;
        this.DefaulMax = DefaulMax;
        this.DefaulMin = DefaulMin;
    }
}

public class ColorStringDefine 
{
    public const string Red = "#EE2616";
    public const string Yellow = "#EFCB28";
	
	public const string Default="white";
	public const string Mp_type1="orange";
	public const string Mp_type0="#A564DB";
	public const string Hp_posion="green";
	public const string Hp_hurt_light="#FD726F";
	public const string Hp_hurt_heavy="#F8413E";
}

public class GameConst
{
    public const string DefaultMainMenuScene = "0_GameStart";
    
    public const float MapSpeed = 5;
    public const float MapAngularSpeed = 360;
    public const float MapAcceleration = 15;

    
    public const bool SEMI_REAL = false;//游戏是否是半即时制
    public const int ACTION_SP = 1000;//半即时制的情况下 行动一次的花费
    public const int MAX_ROLE_LEVEL = 30;

    public const int MAX_EXP = 9999999;

    public const int MAX_ROLE_TILI = 100;

    public const int MAX_POISON = 100;

    public const int MAX_USE_POISON = 100;
    public const int MAX_HEAL = 100;
    public const int MAX_DEPOISON = 100;
    public const int MAX_ANTIPOISON = 100;

    public const int GAME_START_MUSIC_ID = 16;
    /// <summary>
    /// 最大角色的武器熟练度
    /// </summary>
    public const int MAX_ROLE_WEAPON_ATTR = 100;

    public const int MAX_ROLE_HP = 999;
    public const int MAX_ROLE_MP = 999;

    public const int MAX_ROLE_ATTACK = 100;
    public const int MAX_ROLE_DEFENCE = 100;
    public const int MAX_ROLE_QINGGONG = 100;
    public const int MAX_ROLE_ATK_POISON = 100;
    public const int MAX_ROLE_SHENGWANG = 200;
    public const int MAX_ROLE_PINDE = 100;

    public const int MAX_ROLE_ZIZHI = 100;

    /// <summary>
    /// 最大技能等级，每1级是100，最大10级所以是1000
    /// </summary>
    public const int MAX_SKILL_LEVEL = 100 * (MAX_WUGONG_LEVEL - 1);

    /// <summary>
    /// 最大技能等级
    /// </summary>
    public const int MAX_WUGONG_LEVEL = 10;

    /// <summary>
    /// 角色的最大武功学习数量
    /// </summary>
    public const int MAX_ROLE_WUGONG_COUNT = 10;
    
    //银两ID
    public const int MONEY_ID = 174; 

    //JYX2最大体力上限
    public const int MaxTili = 100;

    //最大队伍人数
    public const int MAX_TEAMCOUNT = 6;

    //最大技能数量
    public const int MAX_SKILL_COUNT = 10;

    //最大资质
    public const int MAX_ZIZHI = 100;

    //最大属性
    public const int MAX_ROLE_ATTRITE = 100;

    //最大生命和内力
    public const int MAX_HPMP = 999;
        
    //最大战斗上场人数
    public const int MAX_BATTLE_TEAMMATE_COUNT = 6;
	
	//对话框最大可以显示字符
	public const int MAX_CHAT_CHART_NUM=156;
	
	//战斗胜利结果最大显示行数
	public const int MAX_BATTLE_RESULT_LINE_NUM=7;
	
    readonly static public int[] _levelUpExpList = new int[] { 50, 100, 150, 200, 250, 300, 350, 400, 450, 500, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 2400, 1500, 3150, 3300, 3450, 3650, 3700, 3900, 4050, 4200, 4350, 4500, 4600, 4800, 5000, 5100, 5200, 5300, 5500, 5600, 5800, 6000, 6200, 6400, 6600, 6800, 7000, 7200, 7400, 7600, 7800, 8000, 8200, 8400, 8600, 8800, 9200, 9600, 10000, 20000, 30000 };

    public const string UI_PREFAB_PATH = "Assets/Prefabs/Jyx2UI/{0}.prefab";
    public static Dictionary<string, PropertyItem> ProItemDic = new Dictionary<string, PropertyItem>() {
        ["1"] = new PropertyItem(1, "MaxMp", "内力",40,30),//创角用到 特殊
        ["2"] = new PropertyItem(2, "Attack", "武力",30,20),//创角用到 特殊
        ["3"] = new PropertyItem(3, "Qinggong", "轻功", 30, 20),
        ["4"] = new PropertyItem(4, "Defence", "防御", 30, 20),
        ["5"] = new PropertyItem(5, "MaxHp", "生命",50,30),//创角用到 特殊
        ["6"] = new PropertyItem(6, "Heal", "医疗", 30, 20),
        ["7"] = new PropertyItem(7, "UsePoison", "使毒", 30, 20),
        ["8"] = new PropertyItem(8, "DePoison", "解毒", 30, 20),
        ["9"] = new PropertyItem(9, "Quanzhang", "拳掌", 30, 20),
        ["10"] = new PropertyItem(10, "Yujian", "剑术", 30, 20),
        ["11"] = new PropertyItem(11, "Shuadao", "刀术", 30, 20),
        ["12"] = new PropertyItem(12, "Anqi", "暗器", 30, 20),

        ["13"] = new PropertyItem(13, "Hp", "生命", 30, 20),
        ["14"] = new PropertyItem(14, "Tili", "体力", 30, 20),
        ["15"] = new PropertyItem(15, "Mp", "内力", 40, 30),
        ["16"] = new PropertyItem(16, "MaxHp", "最大生命", 50, 30),
        ["17"] = new PropertyItem(17, "MaxMp", "最大内力", 40, 30),
        ["18"] = new PropertyItem(18, "Attack", "攻击力", 30, 20),
        ["19"] = new PropertyItem(19, "AntiPoison", "抗毒", 30, 20),
        ["20"] = new PropertyItem(20, "Qimen", "奇门", 30, 20),
        ["21"] = new PropertyItem(21, "Wuxuechangshi", "武学常识", 30, 20),
        ["22"] = new PropertyItem(22, "Pinde", "品德", 30, 20),
        ["23"] = new PropertyItem(23, "AttackPoison", "功夫带毒", 30, 20),
        ["24"] = new PropertyItem(24, "Zuoyouhubo", "左右互搏", 30, 20),
        ["25"] = new PropertyItem(25, "IQ", "资质", 100, 30),
        ["26"] = new PropertyItem(26, "Poison", "中毒", 30, 20),
    };

    //存档的数量
    public const int SAVE_COUNT = 3;
    public static string GetUPNumber(int index) 
    {
        switch (index) 
        {
            case 1:
                return "一";
            case 2:
                return "二";
            case 3:
                return "三";
            default:
                return "";
        }
    }
}
