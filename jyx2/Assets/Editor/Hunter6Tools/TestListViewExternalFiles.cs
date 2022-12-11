using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WH.Editor;

public class TestListViewExternalFiles : TestListView
{
    //[MenuItem("Test/ListView ExternalFiles")]
    static void Init()
    {
        GetWindow(typeof(TestListViewExternalFiles));
    }

    private void OnGUI()
    {
        if (s_Styles == null)
        {
            s_Styles = new Styles();
        }
        m_ListView.totalRows = m_MsgList.Count;

        Event current = Event.current;
        EditorGUILayout.BeginVertical();
        GUIContent textContent = new GUIContent();
        foreach (ListViewElement el in ListViewGUI.ListView(m_ListView, ListViewOptions.wantsExternalFiles, s_Styles.listBackgroundStyle))
        {
            if (current.type == EventType.MouseDown && current.button == 0 && el.position.Contains(current.mousePosition) && current.clickCount == 2)
            {
                Debug.Log(el.row);
            }
            if (current.type == EventType.Repaint)
            {
                textContent.text = GetRowText(el);

                // 交替显示不同背景色
                GUIStyle style = (el.row % 2 != 0) ? s_Styles.listItemBackground2 : s_Styles.listItemBackground;
                style.Draw(el.position, false, false, m_ListView.row == el.row, false);
                s_Styles.listItem.Draw(el.position, textContent, false, false, m_ListView.row == el.row, m_Focus);
            }
        }
        EditorGUILayout.EndVertical();

        {
            // 拖动文件
            if (m_ListView.fileNames != null)
            {
                m_MsgList.InsertRange(m_ListView.row, m_ListView.fileNames);
            }
        }
    }
}