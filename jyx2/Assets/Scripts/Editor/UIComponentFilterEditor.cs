using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIComponentFilter), true)]
public class UIComponentFilterEditor : Editor 
{
	UIComponentFilter m_UIComponentFilter;
    public List<string> m_selectList = new List<string>();
    void OnEnable()
    {
        m_UIComponentFilter = (UIComponentFilter)target;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        m_selectList.Clear();

        EditorGUILayout.LabelField("请勾选需要导出的组件");
        Dictionary<string, bool> m_componentNameDics = m_UIComponentFilter.GetComponentNames();
        foreach (KeyValuePair<string, bool> kv in m_componentNameDics)
        {
            if (EditorGUILayout.Toggle(kv.Key, kv.Value))
            {
                m_selectList.Add(kv.Key);
            }
        }
        m_UIComponentFilter.ChangeExportComponents(m_selectList);

        serializedObject.ApplyModifiedProperties();
    }
}
