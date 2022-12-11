using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WH.Editor;

public class TestListViewWithCol : TestListView
{
    private List<string> m_MsgList2;

    private int[] m_ColWidths = new []{100, 100};

   // [MenuItem("Test/ListView Col")]
    static void Init()
    {
        GetWindow(typeof(TestListViewWithCol));
    }

    public TestListViewWithCol()
    {
        m_MsgList2 = new List<string>();
        m_MsgList2.Add("第二列");
        m_MsgList2.Add("abc");
        m_MsgList2.Add("efg");
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
        foreach (ListViewElement el in ListViewGUI.ListView(m_ListView, m_ColWidths, s_Styles.listBackgroundStyle))
        {
            if (current.type == EventType.MouseDown && current.button == 0 && el.position.Contains(current.mousePosition) && current.clickCount == 2)
            {
                Debug.Log("点中了" + "行" + el.row + "列" + el.column);
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
    }

    private string GetRowText(ListViewElement el)
    {
        if (el.column == 0)
        {
            return m_MsgList[el.row];
        }
        return m_MsgList2[el.row];
    }
}