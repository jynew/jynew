using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class UIExportEditor : EditorWindow
{
    private static readonly string m_exportPanelPath = string.Format("{0}/Scripts/Jyx2UIScripts/",Application.dataPath);
    private static readonly string m_exportPanelDataPath = string.Format("{0}/Scripts/Jyx2UIScripts/UIData/", Application.dataPath);

    [MenuItem("GameObject/选中物体操作/生成UI脚本文件", priority = 0)]
    public static void ExportUIScript() 
    {
        GameObject selectObj = Selection.activeGameObject;

        string fullPath = string.Format("{0}{1}.cs", m_exportPanelPath, selectObj.gameObject.name);
        if (!File.Exists(fullPath))
        {
            GenUIScript(selectObj, fullPath);
        }
        //uidata每次都重新生产 不要手动更改
        GenUIData(selectObj);

        AssetDatabase.Refresh();
        Debug.LogWarning("生成UI脚本完成");
    }

    [MenuItem("GameObject/选中物体操作/生成UIData文件", priority = 1)]
    public static void ExportUITemplate()
    {
        GameObject selectObj = Selection.activeGameObject;

        GenUIData(selectObj);

        AssetDatabase.Refresh();
        Debug.LogWarning("生成UI数据完成");
    }

    static void GenUIScript(GameObject selectObj,string path) 
    {
        string templatePath = string.Format("{0}/Scripts/Editor/UIScriptTemplate.lua", Application.dataPath);
        string templateData = File.ReadAllText(templatePath);

        templateData = templateData.Replace("UIScriptTemplate", selectObj.gameObject.name);
        File.WriteAllText(path, templateData);
    }

    static void GenUIData(GameObject selectObj) 
    {
        //生成UIData文件
        string templatePath = string.Format("{0}/Scripts/Editor/UIDataTemplate.lua", Application.dataPath);
        string templateData = File.ReadAllText(templatePath);

        List<Component> allComponent = GetUIComponents(selectObj.transform);
        string propertyStr = "";
        string pathStr = "";
        Component com;
        for (int i = 0; i < allComponent.Count; i++)
        {
            com = allComponent[i];
            string typeName = com.GetType().Name;
            string proName = string.Format("{0}_{1}", com.gameObject.name, typeName);
            propertyStr = propertyStr + string.Format("\tprivate {0} {1};\n", typeName, proName);

            string path = GetFullPath(selectObj.transform, com.transform);
            pathStr = pathStr + string.Format("\t\t{0} = transform.Find(\"{1}\").GetComponent<{2}>();\n", proName, path, typeName);
        }
        templateData = templateData.Replace("UIDataTemplate", string.Format("{0}", selectObj.gameObject.name));
        templateData = templateData.Replace("#property", propertyStr);
        templateData = templateData.Replace("#findtrans", pathStr);

        string uiDataPath = string.Format("{0}{1}_UIData.cs", m_exportPanelDataPath, selectObj.gameObject.name);
        File.WriteAllText(uiDataPath, templateData);
    }

    static string GetFullPath(Transform parent, Transform child) 
    {
        Transform node = child;
        string path = "";
        while (node != null)
        {
            if (node == parent)
                break;
            if (node == child)
                path = child.name;
            else
                path = string.Format("{0}/{1}", node.name, path);
            node = node.parent;
        }
        return path;
    }

    static List<Component> GetUIComponents(Transform trans,bool includeInactive =false) 
    {
        List<Component> result = new List<Component>();
        List<UIComponentFilter> allFilter = new List<UIComponentFilter>();
        trans.GetComponentsInChildren<UIComponentFilter>(true, allFilter);

        UIComponentFilter filter;
        for (int i = 0; i < allFilter.Count; i++)
        {
            filter = allFilter[i];
            result.AddRange(filter.m_exportComponentList);
        }
        return result;
    }

}
