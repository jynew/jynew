using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MTE
{
    internal abstract class DetailListBox<T> where T: Detail
    {
        public int DoGUI(int selectedDetailIndex)
        {
            EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(64));
            {
                if (detailList == null || this.detailList.Count == 0)
                {
                    NoDetailGUI();
                }
                else
                {
                    for (int i = 0; i < this.detailList.Count; i += 4)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if (i + j >= this.detailList.Count) break;

                                EditorGUILayout.BeginVertical();

                                //toggle button
                                bool toggleOn = selectedDetailIndex == i + j;
                                var oldBgColor = GUI.backgroundColor;
                                if (toggleOn)
                                {
                                    GUI.backgroundColor = new Color(62/255.0f, 125/255.0f, 231/255.0f);
                                }
                                var new_toggleOn = GUILayout.Toggle(toggleOn,
                                    GUIContent.none,
                                    GUI.skin.button,
                                    GUILayout.Width(72), GUILayout.Height(72));
                                GUI.backgroundColor = oldBgColor;
                                var rect = GUILayoutUtility.GetLastRect();

                                var detailIndex = i + j;
                                DrawButtonBackground(detailIndex, rect);

                                if (new_toggleOn && !toggleOn)
                                {
                                    selectedDetailIndex = i + j;
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndVertical();

            MenuButton(new GUIContent($"{StringTable.Get(C.Edit)} {StringTable.Get(DetailType)}"));

            this.selectedIndex = selectedDetailIndex;
            return selectedDetailIndex;
        }

        public virtual void NoDetailGUI()
        {
            EditorGUILayout.LabelField(StringTable.Get(C.Warning_NoDetail));
        }

        private void MenuButton(GUIContent title)
        {
            GUIContent content = new GUIContent(title.text, EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSettings").image, title.tooltip);
            Rect rect = GUILayoutUtility.GetRect(content, "Button");
            if (GUI.Button(rect, content))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(StringTable.Get(C.Add)), false, AddCallback);
                if (this.detailList.Count > 0)
                {
                    menu.AddItem(new GUIContent(StringTable.Get(C.Edit)), false, EditCallback);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(StringTable.Get(C.Edit)));
                }
                if (this.detailList.Count > 0)
                {
                    menu.AddItem(new GUIContent(StringTable.Get(C.Remove)), false, RemoveCallback);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(StringTable.Get(C.Remove)));
                }
                menu.ShowAsContext();
            }
        }

        public virtual void DrawButtonBackground(int detailIndex, Rect buttonRect)
        {
        }

        public virtual void SetEditingTarget(IList<T> targetDetailList)
        {
            detailList = targetDetailList;
        }

        protected virtual void AddCallback()
        {
        }

        protected virtual void EditCallback()
        {
        }

        private void RemoveCallback()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                StringTable.Get(C.Warning),
                StringTable.Get(C.Warning_Confirm),
                StringTable.Get(C.Yes), StringTable.Get(C.No));
            if (confirmed)
            {
                this.detailList.RemoveAt(this.selectedIndex);
                this.SaveDetailList();
                MTEEditorWindow.Instance.Repaint();
            }
        }

        protected abstract void SaveDetailList();
        protected abstract C DetailType { get; }

        protected IList<T> detailList;
        protected int selectedIndex;
    }
}