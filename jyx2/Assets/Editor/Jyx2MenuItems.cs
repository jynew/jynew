using UnityEngine;
using UnityEditor;
using UnityToolbarExtender.Examples;

namespace Jyx2Editor
{
    public class Jyx2MenuItems 
    {
        [MenuItem("配置管理器/技能编辑器")]
        private static void OpenSkillEditor()
        {
            SceneHelper.StartScene("Assets/Jyx2BattleScene/Jyx2SkillEditor.unity");
        }
        
        [MenuItem("配置管理器/全模型预览")]
        private static void OpenAllModels()
        {
            SceneHelper.StartScene("Assets/3D/AllModels.unity");
        }
    }
}