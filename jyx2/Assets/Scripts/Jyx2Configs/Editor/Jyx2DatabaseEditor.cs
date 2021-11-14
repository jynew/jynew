using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jyx2;
using Jyx2Configs;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class Jyx2DatabaseEditor : OdinMenuEditorWindow
{
    [MenuItem("项目快速导航/项目数据库")]
    private static void OpenWindow()
    {
        GetWindow<Jyx2DatabaseEditor>().titleContent = new GUIContent()
        {
            text = "jynew数据库"
        };
        GetWindow<Jyx2DatabaseEditor>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;

        CreateAssetsMenu<Jyx2ConfigCharacter>(tree, "角色", "Assets/BuildSource/Configs/Characters");
        CreateAssetsMenu<Jyx2ConfigSkill>(tree, "技能", "Assets/BuildSource/Configs/Skills");
        CreateAssetsMenu<Jyx2ConfigItem>(tree, "道具", "Assets/BuildSource/Configs/Items");
        CreateAssetsMenu<Jyx2ConfigMap>(tree, "场景", "Assets/BuildSource/Configs/Maps");
        CreateAssetsMenu<Jyx2ConfigShop>(tree, "商店", "Assets/BuildSource/Configs/Shops");
        CreateAssetsMenu<Jyx2ConfigBattle>(tree, "战斗", "Assets/BuildSource/Configs/Battles");

        tree.MarkDirty();
        return tree;
    }

    private void CreateAssetsMenu<T>(OdinMenuTree tree, string title, string path) where T : Jyx2ConfigBase
    {
        var query = tree.AddAllAssetsAtPath(title, path, 
            typeof(T), true, true);
        
        query.First().Name = $"{title} ({query.Count() - 1})";
        
        //所有孩子排序
        Comparison<OdinMenuItem> comparer = (a, b) =>
        {
            var asset1 = (T)a.Value;
            var asset2 = (T)b.Value;
            return asset1.Id.CompareTo(asset2.Id);
        };

        query.ForEach(x => x.ChildMenuItems.Sort(comparer));
    }
}
