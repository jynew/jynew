using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WH.Editor;

public class TestListViewButton : TestListView
{

    //[MenuItem("Test/ListView Button")]
    static void Init()
    {
        GetWindow(typeof(TestListViewButton));
    }

    private void OnGUI()
    {
        if (s_Styles == null)
        {
            s_Styles = new Styles();
        }
        s_Styles.listItem.fixedHeight = 22f;
        m_ListView.totalRows = m_MsgList.Count;
        m_ListView.rowHeight = 22;

        Event current = Event.current;
        EditorGUILayout.BeginVertical();
        GUIContent textContent = new GUIContent();
        Rect btnRect = new Rect();
        foreach (ListViewElement el in ListViewGUI.ListView(m_ListView, s_Styles.listBackgroundStyle))
        {
            btnRect = el.position;
            btnRect.x += btnRect.width - 32f;
            btnRect.y += 2f;
            btnRect.width = 30f;
            btnRect.height = 18f;

            if (current.type == EventType.MouseDown && current.button == 0 && el.position.Contains(current.mousePosition))
            {
                if (btnRect.Contains(current.mousePosition))
                {
                    Debug.Log("点击第" + el.row + "的按钮");
                }
                else if (current.clickCount == 2)
                {
                    Debug.Log(el.row);
                }
            }
            if (current.type == EventType.Repaint)
            {
                textContent.text = GetRowText(el);

                // 交替显示不同背景色
                GUIStyle style = (el.row % 2 != 0) ? s_Styles.listItemBackground2 : s_Styles.listItemBackground;
                style.Draw(el.position, false, false, m_ListView.row == el.row, false);
                s_Styles.listItem.Draw(el.position, textContent, false, false, m_ListView.row == el.row, m_Focus);

                
                textContent.text = ">>";
                GUI.skin.button.Draw(btnRect, textContent, false, false, false, false);
            }
        }
        EditorGUILayout.EndVertical();
        s_Styles.listItem.fixedHeight = 16f;
    }
}