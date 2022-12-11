using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using WH.Editor;

public class TestListView : EditorWindow
{
    protected List<string> m_MsgList;

    protected ListViewState m_ListView;
    protected bool m_Focus;

    protected class Styles
    {
        public readonly GUIStyle listItem = new GUIStyle("PR Label");
        public readonly GUIStyle listItemBackground = new GUIStyle("CN EntryBackOdd");
        public readonly GUIStyle listItemBackground2 = new GUIStyle("CN EntryBackEven");
        public readonly GUIStyle listBackgroundStyle = new GUIStyle("CN Box");
        public Styles()
        {
            Texture2D background = this.listItem.hover.background;
            // 开启即失去焦点时，也显示蓝色
            //this.listItem.onNormal.background = background;
            this.listItem.onActive.background = background;
            this.listItem.onFocused.background = background;
        }
    }
    protected static Styles s_Styles;

   // [MenuItem("Test/ListView default")]
    static void Init()
    {
        GetWindow(typeof(TestListView));
    }

    public TestListView()
    {
        m_ListView = new ListViewState();
        m_MsgList = new List<string>();
        m_MsgList.Add("第一行");
        m_MsgList.Add("第二行数据");
        m_MsgList.Add("第三行内容");
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
        foreach (ListViewElement el in ListViewGUI.ListView(m_ListView, s_Styles.listBackgroundStyle))
        {
            if (current.type == EventType.MouseDown && current.button == 0 && el.position.Contains(current.mousePosition) && current.clickCount == 2)
            {
                Debug.Log(el.row);
            }
            if (current.type == EventType.Repaint)
            {
                textContent.text = GetRowText(el);

                // 交替显示不同背景色
                GUIStyle style = (el.row%2 != 0) ? s_Styles.listItemBackground2 : s_Styles.listItemBackground;
                style.Draw(el.position, false, false, m_ListView.row == el.row, false);
                s_Styles.listItem.Draw(el.position, textContent, false, false, m_ListView.row == el.row, m_Focus);
            }
        }
        EditorGUILayout.EndVertical();
    }

    protected string GetRowText(ListViewElement el)
    {
        return m_MsgList[el.row];
    }

    private void OnFocus()
    {
        m_Focus = true;
    }

    private void OnLostFocus()
    {
        m_Focus = false;
    }
}
