using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WH.Editor;

public class TestListViewReordering : TestListView
{

    //[MenuItem("Test/ListView Reordering")]
    static void Init()
    {
        GetWindow(typeof(TestListViewReordering));
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
        foreach (ListViewElement el in ListViewGUI.ListView(m_ListView, ListViewOptions.wantsReordering, s_Styles.listBackgroundStyle))
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

        if (m_ListView.totalRows > 0 && m_ListView.selectionChanged)
        {
            // 拖动更新
            if (m_ListView.draggedFrom != -1 && m_ListView.draggedTo != -1)
            {
                var tmp = m_MsgList[m_ListView.draggedFrom];
                m_MsgList[m_ListView.draggedFrom] = m_MsgList[m_ListView.row];
                m_MsgList[m_ListView.row] = tmp;
            }
        }
    }
}