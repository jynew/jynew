using System.Collections.Generic;
using Jyx2.MOD;
using Sirenix.OdinInspector;
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

    
    [InfoBox("某些角色名与人物ID不严格对应，在此修正。用于对话中正确显示名字")] [BoxGroup("对话人物ID修正")] [TableList] 
    [HideLabel]
    public List<StoryIdNameFix> StoryIdNameFixes;


    [Button("生成索引")]
    void GenerateIndexFile()
    {
        MODLoader.WriteModIndexFile(ModRootDir);
    }
}
