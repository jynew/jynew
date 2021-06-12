using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ui生成用组件
/// </summary>
[DisallowMultipleComponent]
public class UIComponentFilter : MonoBehaviour {
    [SerializeField]
    public List<Component> m_exportComponentList = new List<Component>();

    public Dictionary<string, bool> m_componentNameDics = new Dictionary<string, bool>();
    public Dictionary<string, bool> GetComponentNames()
    {
        m_componentNameDics.Clear();
        Component[] monos = GetComponents<Component>();
        for (int i = 0, len = monos.Length; i < len; i++)
        {
            System.Type type = monos[i].GetType();
            if (type == typeof(CanvasRenderer) || type == typeof(UIComponentFilter))
            {
                continue;
            }
            string szName = type.ToString();
            if ((!szName.Contains("UI"))&&(!szName.Contains("Transform")))
                continue;
            szName = szName.Replace("UnityEngine.", "");
            szName = szName.Replace("UI.", "");
            if (!m_componentNameDics.ContainsKey(szName))
            {
                m_componentNameDics.Add(szName, m_exportComponentList.Contains(GetComponent(szName)));
            }
        }
        return m_componentNameDics;
    }
    public void ChangeExportComponents(List<string> list)
    {
        m_exportComponentList.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            string comName = list[i];
            Component com = GetComponent(comName);
            m_exportComponentList.Add(com);
        }
    }
}
