using GLib;
using HanSquirrel.ResourceManager;
using System;
using System.Collections.Generic;
using System.Threading;

public class ConStr
{
    public const string NLogConfigAssetPath = "Assets/BuildSource/Configs/NLogDefault.xml";
    public const string GLOBAL_DESKEY = "koidfjwhguireufcbdadr8232dsf";

    public static readonly HSLeanPoolConfig[] PrefabPoolConfig =
    {};

    public const string ConfigPath = "Assets/BuildSource/Configs/";
    public const string BattleBlockDatasetPath = "Assets/BuildSource/BattleBlockDataset/";
    public const string BattleboxDatasetPath = "Assets/BuildSource/BattleboxDataset/";
    public const string LevelMaster = "Assets/Prefabs/LevelMaster.prefab";
    public const string Player = "Assets/Prefabs/Player.prefab";
    public const string PlayerPointLight = "Assets/Prefabs/PlayerPointLight.prefab";

    public const string DES_KEY = "test";

    public const string DefaultSword = "Assets/BuildSource/ModelWeapons/W_Right_SinSword.prefab";
    public const string DefaultKnife = "Assets/BuildSource/ModelWeapons/W_Right_SinKnif.prefab";
    public const string DefaultSpear = "Assets/BuildSource/ModelWeapons/W_Right_Gudgel.prefab";
    #region HeadAvata配置
    public const string NamesPath = "Assets/BuildSource/names.txt";
    public const string HEADAVATA_TEMPLATE_XML = "Assets/BuildSource/headavata/avatas.xml";
    public const string HeadAvataPath = "Assets/BuildSource/headavata/";
    public const string HeadAvataNewPath = "Assets/BuildSource/HeadAvataNew/";
    public const string HeadAvataBodyPath = "Assets/BuildSource/HeadAvataBodys/";
    public const string CharacterPath = "Assets/BuildSource/Character/";
    public const string RoleHeadUI = "Assets/Prefabs/RoleHeadUI.prefab";
    public const string MiniHeadUI = "Assets/Prefabs/MiniHeadUI.prefab";
    public const string EnabledFemaleHeadAvata = "5,6,7,8,9,10,11";
    #endregion


    #region AB打包配置
    public static readonly string[] PrefabSearchPaths = new string[]
    {
        //GG需要包含一个没有Prefab的路径（目前程序的BUG，如果PrefabSearchPaths是空，则会遍历所有的Prefab）
        ConfigPath
    };

    private static Dictionary<string, string> _ABFolderDict;
    public static Dictionary<string, string> ABFolderDict
    {
        get
        {
            if (_ABFolderDict == null)
            {
                _ABFolderDict = new Dictionary<string, string>
                {
                    //JYX2
                    { "ModelDescXmls", "Assets/3D/XML".RemoveDirEndTag() },
                    { "configs", "Assets/BuildSource/Configs/".RemoveDirEndTag() },
                    { "battleboxdataset", BattleboxDatasetPath.RemoveDirEndTag() },
                };
            }
            return _ABFolderDict;
        }
    }

    /// <summary>
    /// 除了在上面 PrefabSearchPaths 和 _ABFolderDict 中定义的AB包之外，还有哪些AB包是有效的。
    /// 如果StreamingAsset目录下面还存在其他的AB包，在打包过程可能会被删除。
    ///（框架内部会自动添加：Android、lua、filter、values、以及values_*）
    /// </summary>
    public static readonly string[] AdditionalABs = new string[]
    {
        "values.zip"
    };
    #endregion
    
}
