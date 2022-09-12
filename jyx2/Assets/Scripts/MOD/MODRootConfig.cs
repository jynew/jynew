using System.Collections.Generic;
using System.IO;
using Jyx2.Middleware;
using Jyx2.MOD;
using Jyx2Configs;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


/// <summary>
/// MOD的根配置
///
/// 本运行框架下，所有的可游玩内容都对等为一个MOD。
/// </summary>
[CreateAssetMenu(menuName = "金庸重制版/MOD/生成根配置文件", fileName = "ModRootConfig")]
public class MODRootConfig : ScriptableObject
{
    [LabelText("MOD ID（全局唯一）")] public string ModId;

    [LabelText("MOD名称")] public string ModName;
    [LabelText("游戏MOD的根目录")] public string ModRootDir;
    [Multiline] [LabelText("游戏的欢迎语")] public string WelcomeWord;
    [LabelText("游戏作者名")] public string Author;

    [LabelText("LUA文件名配置")] public string LuaFilePatten = "ka{0}";

    [LabelText("主角姓名")] public string PlayerName;

    [LabelText("预加载的lua文件（比如热更新）")] public List<string> PreloadedLua;
    
    [InfoBox("某些角色名与人物ID不严格对应，在此修正。用于对话中正确显示名字")] [BoxGroup("对话人物ID修正")] [TableList] 
    [HideLabel]
    public List<StoryIdNameFix> StoryIdNameFixes;

#if UNITY_EDITOR
    [Button("生成配置表")]
    public void GenerateConfigs()
    {
        string dataPath = Path.Combine(ModRootDir, "Configs", "Datas.bytes");
        if (File.Exists(dataPath))
        {
            File.Delete(dataPath);
        }
        ExcelTools.GenerateConfigsFromExcel<Jyx2ConfigBase>($"{ModRootDir}/Configs");
        AssetDatabase.Refresh();
    }
#endif
}
