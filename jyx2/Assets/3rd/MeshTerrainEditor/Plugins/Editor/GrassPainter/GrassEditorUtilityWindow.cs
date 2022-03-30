using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MTE
{
    internal class GrassEditorUtilityWindow : MTEWindow
    {
        /// <summary>
        /// Editing grass detail list
        /// </summary>
        public IList<GrassDetail> detailList;
        public int editingIndex;
        public bool IsAdding = false;

        public Action OnSave { private get; set; }

        #region Parameters
        Material material;
        float minWidth = GrassDetail.DefaultMinWidth;
        float maxWidth = GrassDetail.DefaultMaxWidth;
        float minHeight = GrassDetail.DefaultMinHeight;
        float maxHeight = GrassDetail.DefaultMaxHeight;
        GrassType grassType = GrassDetail.DefaultGrassType;
        #endregion

        GrassDetail grassDetail;

        public override void OnGUI()
        {
            base.OnGUI();

            if(IsAdding)
            {
                material = (Material)EditorGUILayout.ObjectField(StringTable.Get(C.Material),
                    material, typeof(Material), false);
                minWidth =  EditorGUILayout.Slider(StringTable.Get(C.MinWidth), minWidth, 0f, 9999f);
                maxWidth =  EditorGUILayout.Slider(StringTable.Get(C.MaxWidth), maxWidth, 0f, 9999f);
                minHeight = EditorGUILayout.Slider(StringTable.Get(C.MinHeight), minHeight, 0f, 9999f);
                maxHeight = EditorGUILayout.Slider(StringTable.Get(C.MaxHeight), maxHeight, 0f, 9999f);
                grassType = (GrassType)EditorGUILayout.EnumPopup(StringTable.Get(C.Type), grassType);
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(StringTable.Get(C.Add)))
                {
                    if (material != null)
                    {
                        GrassDetail grassDetail = new GrassDetail
                        {
                            Material = material,
                            MinWidth = minWidth,
                            MaxWidth = maxWidth,
                            MinHeight = minHeight,
                            MaxHeight = maxHeight,
                            GrassType = grassType,
                        };
                        this.detailList.Add(grassDetail);
                        OnSave();
                        MTEEditorWindow.Instance.Repaint();
                        this.Close();
                    }
                }
                if (GUILayout.Button(StringTable.Get(C.Cancel)))
                {
                    this.Close();
                }
                EditorGUILayout.EndHorizontal();
            }
            else//editing
            {
                if (grassDetail == null)
                {
                    grassDetail = detailList[editingIndex];
                    if (grassDetail != null)
                    {
                        grassDetail = grassDetail.ShallowCopy();
                    }
                }
                if (grassDetail == null)
                {
                    EditorGUILayout.HelpBox(
                        $"Ignored invalid grass detail detected at index {editingIndex}",
                        MessageType.Warning);
                }
                else
                {
                    grassDetail.Material = (Material)EditorGUILayout.ObjectField(StringTable.Get(C.Material),
                        grassDetail.Material, typeof(Material), false);
                    grassDetail.MinWidth  = EditorGUILayout.Slider(StringTable.Get(C.MinWidth), grassDetail.MinWidth, 0f, 9999f);
                    grassDetail.MaxWidth  = EditorGUILayout.Slider(StringTable.Get(C.MaxWidth), grassDetail.MaxWidth, 0f, 9999f);
                    grassDetail.MinHeight = EditorGUILayout.Slider(StringTable.Get(C.MinHeight), grassDetail.MinHeight, 0f, 9999f);
                    grassDetail.MaxHeight = EditorGUILayout.Slider(StringTable.Get(C.MaxHeight), grassDetail.MaxHeight, 0f, 9999f);
                    grassDetail.GrassType = (GrassType)EditorGUILayout.EnumPopup(StringTable.Get(C.Type), grassDetail.GrassType);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(StringTable.Get(C.Apply)))
                    {
                        detailList[editingIndex] = grassDetail;
                        OnSave();
                        MTEEditorWindow.Instance.Repaint();
                        this.Close();
                    }
                }
                if (GUILayout.Button(StringTable.Get(C.Cancel)))
                {
                    this.Close();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}