using Sirenix.OdinInspector;
using UnityEngine;

namespace Jyx2.MOD
{
    /// <summary>
    /// MOD的根配置
    ///
    /// 本运行框架下，所有的可游玩内容都对等为一个MOD。
    /// </summary>
    [CreateAssetMenu(menuName = "金庸重制版/MOD/生成根配置文件", fileName = "MOD_ID")]
    public class ModRootConfig : ScriptableObject
    {
        [LabelText("MOD ID（全局唯一）")] public string ModId;
        [LabelText("游戏MOD的根目录")] public string ModRootDir;
        [Multiline][LabelText("游戏的欢迎语")] public string WelcomeWord;
        [LabelText("游戏作者名")] public string Author;
    }
}
