using System;
using Jyx2;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class ModelAssetManager : OdinMenuEditorWindow
{
    [MenuItem("项目快速导航/模型配置管理器")]
    private static void OpenWindow()
    {
        GetWindow<ModelAssetManager>().titleContent = new GUIContent()
        {
            text = "模型配置管理器"
        };
        GetWindow<ModelAssetManager>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;

        tree.Add("创建模型配置", new CreateNewModelAsset());
        var query = tree.AddAllAssetsAtPath("模型配置", "Assets/BuildSource/Jyx2RoleModelAssets", typeof(ModelAsset), true, false);
        Comparison<OdinMenuItem> comparer = (a, b) =>
        {
            if (!(a.Value is ModelAsset && b.Value is ModelAsset)) return 0;
            ModelAsset assetA = (ModelAsset)a.Value;
            ModelAsset assetB = (ModelAsset)b.Value;
            return assetA.name.CompareTo(assetB.name);
        };
        query.ForEach(x => x.ChildMenuItems.Sort(comparer));
        tree.MarkDirty();
        return tree;
    }

    public class CreateNewModelAsset
    {
        public CreateNewModelAsset()
        {
            modelAsset = ScriptableObject.CreateInstance<ModelAsset>();
        }

        [InlineEditor(Expanded = true, DrawPreview = true)]
        public ModelAsset modelAsset;
        
        [BoxGroup("新增配置")]
        [LabelText("配置名称")]
        public string m_AssetName;
        
        [BoxGroup("新增配置")]
        [Button("新增模型配置")]
        private void CreateNewData()
        {
            if (string.IsNullOrEmpty(m_AssetName)) return;

            AssetDatabase.CreateAsset(modelAsset, "Assets/BuildSource/Jyx2RoleModelAssets/" + m_AssetName + ".asset");
            AssetDatabase.SaveAssets();
        }
    }

    protected override void OnBeginDrawEditors()
    {
        OdinMenuTreeSelection selected = this.MenuTree.Selection;

        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            GUILayout.FlexibleSpace();

            if (SirenixEditorGUI.ToolbarButton("删除"))
            {
                ModelAsset asset = selected.SelectedValue as ModelAsset;
                string path = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
            }

        }
        SirenixEditorGUI.EndHorizontalToolbar(); 
    }
}
