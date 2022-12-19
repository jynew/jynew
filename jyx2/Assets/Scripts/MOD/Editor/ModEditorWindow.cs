using System.Net;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Jyx2.MOD.Editor
{
    public class ModEditorWindow : OdinEditorWindow
    {
        [MenuItem("MOD开发者/工具集")]
        private static void OpenModEditorWindow()
        { 
            var win = GetWindow<ModEditorWindow>();
            win.titleContent.text = "MOD开发者工具箱";
        }

        [BoxGroup("MOD基本运行环境")]
        [Button("打开MOD目录")]
        void OpenModFolder()
        {
            Middleware.Tools.openURL(Application.dataPath + "/Mods/");
        }

        [BoxGroup("MOD基本运行环境")]
        [Button("打开MOD打包输出目录")]
        void OpenModDir()
        {
            Middleware.Tools.openURL("ModBuild");
        }

        [BoxGroup("新建MOD")]
        [LabelText("MOD名称")] public string ModId = "输入新建的MOD ID";
        
        [BoxGroup("新建MOD")]
        [Button("新建MOD")]
        void CreateNewMod()
        {
            //ModName只允许包含英文数字下划线
            if (ModId == null || ModId.Length == 0 || !System.Text.RegularExpressions.Regex.IsMatch(ModId, @"^[a-zA-Z0-9_]+$"))
            {
                EditorUtility.DisplayDialog("错误", "MOD名称只允许包含英文数字下划线", "确定");
                return;
            }
            
            //检查是否已经存在同名的MOD
            if (System.IO.Directory.Exists(Application.dataPath + "/Mods/" + ModId))
            {
                EditorUtility.DisplayDialog("错误", "已经存在同名的MOD", "确定");
                return;
            }
            
            //确定是否创建MOD吗
            if (!EditorUtility.DisplayDialog("提示", $"确定要创建ID为 {ModId} 的新的游戏MOD吗？", "确定", "取消"))
            {
                return;
            }
            
            //创建MOD目录
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Mods/" + ModId);
            
            //创建MOD的BuildSource目录
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Mods/" + ModId + "/BuildSource");
            //创建RELEASENOTE文件
            var streamWriter = System.IO.File.CreateText(Application.dataPath + "/Mods/" + ModId + "/BuildSource/RELEASE_NOTE.txt");
            streamWriter.WriteLine("添加更新说明..");
            streamWriter.Close();
            
            
            //创建MOD的Lua目录
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Mods/" + ModId + "/Lua");

            //创建MOD的地图目录
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Mods/" + ModId + "/Maps");
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Mods/" + ModId + "/Maps/GameMaps");
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Mods/" + ModId + "/Maps/BattleMaps");
            
            //拷贝SAMPLE中的所有的配置表文件
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Mods/" + ModId + "/Configs");
            foreach (var file in System.IO.Directory.GetFiles(Application.dataPath + "/Mods/SAMPLE/Configs"))
            {
                System.IO.File.Copy(file, Application.dataPath + "/Mods/" + ModId + "/Configs/" + System.IO.Path.GetFileName(file));
            }
            
            //拷贝SAMPLE中的ModSetting文件
            System.IO.File.Copy(Application.dataPath + "/Mods/SAMPLE/ModSetting.asset", Application.dataPath + "/Mods/" + ModId + "/ModSetting.asset");

            AssetDatabase.Refresh();

            var newAsset =
                AssetDatabase.LoadAssetAtPath<MODRootConfig>("Assets/Mods/" + ModId + "/ModSetting.asset");

            newAsset.ModName = "请填入MOD名称";
            newAsset.IsNativeMod = false;
            newAsset.Author = "请输入作者名";
            newAsset.Desc = "请输入MOD描述";
            newAsset.PreloadedLua.Clear();
            newAsset.StoryIdNameFixes.Clear();
            newAsset.ModId = ModId;
            newAsset.ModRootDir = "Assets/Mods/" + ModId;
            
            
            
            
            //将newAsset保存
            EditorUtility.SetDirty(newAsset);
            AssetDatabase.SaveAssetIfDirty(newAsset);
            
            //选中新建的MOD
            Selection.activeObject = newAsset;
            
            //提示创建成功
            EditorUtility.DisplayDialog("提示", $"创建ID为 {ModId} 的新的游戏MOD成功", "确定");
        }


        [BoxGroup("MOD编辑")]
        [InfoBox("下列功能待实现，敬请期待\n引用依赖检查：OK\n发布待解决：无")]
        [LabelText("当前正在编辑MOD")]
        public MODRootConfig CurrentMod;

        
        [BoxGroup("MOD编辑")]
        [Button("定位到")]
        void SelectModMenu()
        {
            Selection.activeObject = CurrentMod;
        }
        
        [BoxGroup("MOD编辑")]
        [Button("创建Lua")]
        void CreateLua()
        {
            EditorUtility.DisplayDialog("提示", $"待实现", "确定");
        }
        
        [BoxGroup("MOD编辑")]
        [Button("创建一个场景")]
        void CreateGameScene()
        {
            EditorUtility.DisplayDialog("提示", $"待实现", "确定");
        }
        
        [BoxGroup("MOD编辑")]
        [Button("创建一个战斗场景")]
        void CreateBattleScene()
        {
            EditorUtility.DisplayDialog("提示", $"待实现", "确定");
        }
        
        [BoxGroup("MOD编辑")]
        [Button("创建一个技能展示")]
        void CreateSkillDisplayAsset()
        {
            EditorUtility.DisplayDialog("提示", $"待实现", "确定");
        }
        
        [BoxGroup("MOD编辑")]
        [Button("创建一个角色模型预设")]
        void CreateRoleModelAsset()
        {
            EditorUtility.DisplayDialog("提示", $"待实现", "确定");
        }
        
        [BoxGroup("MOD编辑")]
        [Button("检查本MOD的引用依赖")]
        void CheckRef()
        {
            EditorUtility.DisplayDialog("提示", $"待实现", "确定");
        }
        
        [BoxGroup("MOD编辑")]
        [Button("自动绑定和分配ab包")]
        void AutoAssetBundlePackingSet()
        {
            EditorUtility.DisplayDialog("提示", $"待实现", "确定");
        }
        
        [BoxGroup("MOD编辑")]
        [Button("MOD打包输出")]
        void Build()
        {
            EditorUtility.DisplayDialog("提示", $"待实现", "确定");
        }
        
        
    }
}