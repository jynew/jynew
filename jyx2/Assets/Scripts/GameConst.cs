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
using System.Collections;
using System.Collections.Generic;
using i18n.TranslatorDef;
using Jyx2;
using UnityEngine;

public enum Jyx2_MpType
{
    Yin = 0,     //阴性内力
    Yang = 1,    //阳性内力
    Neutral = 2, //中性内力
}

public enum Jyx2_GameDifficulty
{
    Simple = 0,
    Normal = 1,
    Hard = 2,
}


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

    public static Color main_menu_selected = new Color(238/255.0f,39/255.0f,24/255.0f,1.0f);
    public static Color main_menu_normal = new Color(125/255.0f,9/255.0f,0.0f,1.0f);
    public static Color save_selected = new Color(255/255.0f,150/255.0f,70/255.0f,1.0f);
    public static Color save_normal = new Color(1.0f,1.0f,1.0f);
    public static Color system_item_selected = new Color(1.0f,223/255.0f,181/255.0f);
    public static Color system_item_normal = new Color(1.0f,150/255.0f,16/255.0f,1.0f);
}

public class GameConst
{
    public const string DEFAULT_GAME_MOD_NAME = "JYX2";
    
    public const string DefaultMainMenuScene = "0_MainMenu";
    
    public const float MapAngularSpeed = 9999;
    public const float MapAcceleration = 9999;

    public const bool SEMI_REAL = false;//游戏是否是半即时制
    public const int ACTION_SP = 1000;//半即时制的情况下 行动一次的花费

    public static int MAX_ROLE_LEVEL => GameSettings.GetInt(nameof(MAX_ROLE_LEVEL));

    public const int MAX_EXP = 9999999;

    //JYX2最大体力上限
    public static int MAX_ROLE_TILI => GameSettings.GetInt(nameof(MAX_ROLE_TILI));

    public static int MAX_POISON => GameSettings.GetInt(nameof(MAX_POISON));

    public static int MAX_USE_POISON => GameSettings.GetInt(nameof(MAX_USE_POISON));
    public static int MAX_HEAL => GameSettings.GetInt(nameof(MAX_HEAL));
    public static int MAX_DEPOISON => GameSettings.GetInt(nameof(MAX_DEPOISON));
    public static int MAX_ANTIPOISON => GameSettings.GetInt(nameof(MAX_ANTIPOISON));
    public static int MAX_HURT => GameSettings.GetInt(nameof(MAX_HURT));

    public static int GAME_START_MUSIC_ID => GameSettings.GetInt(nameof(GAME_START_MUSIC_ID));
    /// <summary>
    /// 最大角色的武器熟练度
    /// </summary>
    public static int MAX_ROLE_WEAPON_ATTR => GameSettings.GetInt(nameof(MAX_ROLE_WEAPON_ATTR));

    public static int MAX_ROLE_HP => GameSettings.GetInt(nameof(MAX_ROLE_HP));
    public static int MAX_ROLE_MP => GameSettings.GetInt(nameof(MAX_ROLE_MP));

    public static int MAX_ROLE_ATTACK => GameSettings.GetInt(nameof(MAX_ROLE_ATTACK));
    public static int MAX_ROLE_DEFENCE => GameSettings.GetInt(nameof(MAX_ROLE_DEFENCE));
    public static int MAX_ROLE_QINGGONG => GameSettings.GetInt(nameof(MAX_ROLE_QINGGONG));
    public static int MAX_ROLE_ATK_POISON => GameSettings.GetInt(nameof(MAX_ROLE_ATK_POISON));
    public static int MAX_ROLE_SHENGWANG => GameSettings.GetInt(nameof(MAX_ROLE_SHENGWANG));
    public static int MAX_ROLE_PINDE => GameSettings.GetInt(nameof(MAX_ROLE_PINDE));

    public static int MAX_ROLE_ZIZHI => GameSettings.GetInt(nameof(MAX_ROLE_ZIZHI));

    /// <summary>
    /// 最大技能等级，每1级是100，最大10级所以是1000
    /// </summary>
    public static int MAX_SKILL_LEVEL = 100 * (MAX_WUGONG_LEVEL - 1);

    /// <summary>
    /// 最大技能等级
    /// </summary>
    public static int MAX_WUGONG_LEVEL => GameSettings.GetInt(nameof(MAX_WUGONG_LEVEL));
    
    //银两ID
    public static int MONEY_ID => GameSettings.GetInt(nameof(MONEY_ID));

    //最大队伍人数
    public static int MAX_TEAMCOUNT => GameSettings.GetInt(nameof(MAX_TEAMCOUNT));

    //最大技能数量
    public static int MAX_SKILL_COUNT => GameSettings.GetInt(nameof(MAX_SKILL_COUNT));
    
    //最大属性
    public static int MAX_ROLE_ATTRIBUTE => GameSettings.GetInt(nameof(MAX_ROLE_ATTRIBUTE));

    //最大战斗上场人数
    public static int MAX_BATTLE_TEAMMATE_COUNT = GameSettings.GetInt(nameof(MAX_BATTLE_TEAMMATE_COUNT));

    //对话框最大可以显示字符
    public const int MAX_CHAT_CHART_NUM = 156;

    //战斗胜利结果最大显示行数
    public const int MAX_BATTLE_RESULT_LINE_NUM = 7;

    //世界地图ID
    public static int WORLD_MAP_ID = GameSettings.GetInt(nameof(WORLD_MAP_ID));

    //升级经验
    public static List<int> _levelUpExpList
    {
        get
        {
            if (_levelUpExpListCache == null)
            {
                _levelUpExpListCache = new List<int>();
                var str = GameSettings.Get("LEVEL_UP_EXP");
                foreach (var tmp in str.Split(','))
                {
                    _levelUpExpListCache.Add(int.Parse(tmp));
                }
            }

            return _levelUpExpListCache;
        }
    }

    private static List<int> _levelUpExpListCache = null;


    #region 体力配置相关

    #endregion

    [Obsolete]
    public const string UI_PREFAB_PATH = "Assets/Prefabs/Jyx2UI/{0}.prefab";
    public static Dictionary<string, PropertyItem> ProItemDic = new Dictionary<string, PropertyItem>()
    {
        //---------------------------------------------------------------------------
        //["0"] = new PropertyItem(0, "MpType", "内力性质", 1, 0),
        //["1"] = new PropertyItem(1, "MaxMp", "内力", 40, 30),//创角用到 特殊
        //["2"] = new PropertyItem(2, "Attack", "武力", 30, 20),//创角用到 特殊
        //["3"] = new PropertyItem(3, "Qinggong", "轻功", 30, 20),
        //["4"] = new PropertyItem(4, "Defence", "防御", 30, 20),
        //["5"] = new PropertyItem(5, "MaxHp", "生命", 50, 30),//创角用到 特殊
        //["6"] = new PropertyItem(6, "Heal", "医疗", 30, 20),
        //["7"] = new PropertyItem(7, "UsePoison", "使毒", 30, 20),
        //["8"] = new PropertyItem(8, "DePoison", "解毒", 30, 20),
        //["9"] = new PropertyItem(9, "Quanzhang", "拳掌", 30, 20),
        //["10"] = new PropertyItem(10, "Yujian", "剑术", 30, 20),
        //["11"] = new PropertyItem(11, "Shuadao", "刀术", 30, 20),
        //["12"] = new PropertyItem(12, "Anqi", "暗器", 30, 20),
        //["13"] = new PropertyItem(13, "Hp", "生命", 30, 20),
        //["14"] = new PropertyItem(14, "Tili", "体力", 30, 20),
        //["15"] = new PropertyItem(15, "Mp", "内力", 40, 30),
        //["16"] = new PropertyItem(16, "MaxHp", "最大生命", 50, 30),
        //["17"] = new PropertyItem(17, "MaxMp", "最大内力", 40, 30),
        //["18"] = new PropertyItem(18, "Attack", "攻击力", 30, 20),
        //["19"] = new PropertyItem(19, "AntiPoison", "抗毒", 30, 20),
        //["20"] = new PropertyItem(20, "Qimen", "奇门", 30, 20),
        //["21"] = new PropertyItem(21, "Wuxuechangshi", "武学常识", 30, 20),
        //["22"] = new PropertyItem(22, "Pinde", "品德", 30, 20),
        //["23"] = new PropertyItem(23, "AttackPoison", "功夫带毒", 30, 20),
        //["24"] = new PropertyItem(24, "Zuoyouhubo", "左右互搏", 30, 20),
        //["25"] = new PropertyItem(25, "IQ", "资质", 100, 30),
        //["26"] = new PropertyItem(26, "Poison", "中毒", 30, 20),
        //---------------------------------------------------------------------------
        //特定位置的翻译【MainMenu右下角当前版本的翻译】
        //---------------------------------------------------------------------------
        ["0"] = new PropertyItem(0, "MpType", "内力性质".GetContent(nameof(GameConst)), (int)Jyx2_MpType.Yang, (int)Jyx2_MpType.Yin),
        ["1"] = new PropertyItem(1, "MaxMp", "内力".GetContent(nameof(GameConst)), 40, 30),//创角用到 特殊
        ["2"] = new PropertyItem(2, "Attack", "武力".GetContent(nameof(GameConst)), 30, 20),//创角用到 特殊
        ["3"] = new PropertyItem(3, "Qinggong", "轻功".GetContent(nameof(GameConst)), 30, 20),
        ["4"] = new PropertyItem(4, "Defence", "防御".GetContent(nameof(GameConst)), 30, 20),
        ["5"] = new PropertyItem(5, "MaxHp", "生命".GetContent(nameof(GameConst)), 50, 30),//创角用到 特殊
        ["6"] = new PropertyItem(6, "Heal", "医疗".GetContent(nameof(GameConst)), 30, 20),
        ["7"] = new PropertyItem(7, "UsePoison", "使毒".GetContent(nameof(GameConst)), 30, 20),
        ["8"] = new PropertyItem(8, "DePoison", "解毒".GetContent(nameof(GameConst)), 30, 20),
        ["9"] = new PropertyItem(9, "Quanzhang", "拳掌".GetContent(nameof(GameConst)), 30, 20),
        ["10"] = new PropertyItem(10, "Yujian", "剑术".GetContent(nameof(GameConst)), 30, 20),
        ["11"] = new PropertyItem(11, "Shuadao", "刀术".GetContent(nameof(GameConst)), 30, 20),
        ["12"] = new PropertyItem(12, "Anqi", "暗器".GetContent(nameof(GameConst)), 30, 20),
        //-------以上自动生成
        
        ["13"] = new PropertyItem(13, "Hp", "生命".GetContent(nameof(GameConst)), 30, 20),
        ["14"] = new PropertyItem(14, "Tili", "体力".GetContent(nameof(GameConst)), 30, 20),
        ["15"] = new PropertyItem(15, "Mp", "内力".GetContent(nameof(GameConst)), 40, 30),
        ["16"] = new PropertyItem(16, "MaxHp", "最大生命".GetContent(nameof(GameConst)), 50, 30),
        ["17"] = new PropertyItem(17, "MaxMp", "最大内力".GetContent(nameof(GameConst)), 40, 30),
        ["18"] = new PropertyItem(18, "Attack", "攻击力".GetContent(nameof(GameConst)), 30, 20),
        ["19"] = new PropertyItem(19, "AntiPoison", "抗毒".GetContent(nameof(GameConst)), 30, 20),
        ["20"] = new PropertyItem(20, "Qimen", "特殊".GetContent(nameof(GameConst)), 30, 20),
        ["21"] = new PropertyItem(21, "Wuxuechangshi", "武学常识".GetContent(nameof(GameConst)), 30, 20),
        ["22"] = new PropertyItem(22, "Pinde", "品德".GetContent(nameof(GameConst)), 30, 20),
        ["23"] = new PropertyItem(23, "AttackPoison", "功夫带毒".GetContent(nameof(GameConst)), 30, 20),
        ["24"] = new PropertyItem(24, "Zuoyouhubo", "左右互搏".GetContent(nameof(GameConst)), 30, 20),
        ["25"] = new PropertyItem(25, "IQ", "资质".GetContent(nameof(GameConst)), 100, 30),
        ["26"] = new PropertyItem(26, "Poison", "中毒".GetContent(nameof(GameConst)), 30, 20),
        //["27"] = new PropertyItem(27, "HpInc", "生命增长".GetContent(nameof(GameConst)), 7, 3),
        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------

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
    
    // PayerPrefs keys
    public const string PLAYER_PREF_VOLUME = "volume";
    public const string PLAYER_PREF_SOUND_EFFECT = "sound_effect";
    public const string PLAYER_PREF_RESOLUTION = "resolution";
    public const string PLAYER_PREF_FULLSCREEN = "fullscreen";
    public const string PLAYER_PREF_VIEWPORT_TYPE = "viewport_type";
    public const string PLAYER_PREF_Difficulty = "difficulty";
    public const string PLAYER_PREF_LANGUAGE = "language";
    public const string PLAYER_PREF_DEBUGMODE = "debugmode";
    public const string PLAYER_MOBILE_MOVE_MODE = "mobile_move_mode";
}
