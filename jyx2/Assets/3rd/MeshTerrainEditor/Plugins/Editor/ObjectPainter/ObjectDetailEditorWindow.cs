using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace MTE
{
    internal class ObjectDetailEditorWindow : MTEWindow
    {
        /// <summary>
        /// Editing grass detail list
        /// </summary>
        public IList detailList;
        public int editingIndex;
        public bool IsAdding = false;

        public Action OnSave { private get; set; }

        #region Parameters
        private GameObject obj;
        private Vector3 minScale = Vector3.one;
        private Vector3 maxScale = Vector3.one;
        private bool useUnifiedScale = true;
        #endregion

        public override void OnEnable()
        {
            base.OnEnable();

            var rect = position;
            rect.width = 550;
            rect.height = 270;
            position = rect;
            objectDetail = null;
        }

        private ObjectDetail objectDetail;

        public override void OnGUI()
        {
            base.OnGUI();

            if(IsAdding)
            {
                obj = (GameObject) EditorGUILayout.ObjectField(StringTable.Get(C.Object), obj, typeof(GameObject), false);
                EditorGUILayout.PrefixLabel(StringTable.Get(C.Scale));
                useUnifiedScale =
                    EditorGUILayout.Toggle(StringTable.Get(C.UnifiedScale), useUnifiedScale);
                EditorGUILayout.PrefixLabel(StringTable.Get(C.Scale));
                EditorGUI.indentLevel++;
                if (useUnifiedScale)
                {
                    var newMin = EditorGUILayout.FloatField("min", minScale.x);
                    var newMax =  EditorGUILayout.FloatField("max", maxScale.x);
                    EditorGUILayout.MinMaxSlider(ref newMin, ref newMax, 0.01f, 100.0f);
                    if (newMin >= newMax)
                    {
                        newMin = newMax;
                    }
                    minScale.z = minScale.y = minScale.x = newMin;
                    maxScale.z = maxScale.y = maxScale.x = newMax;
                }
                else
                {
                    EditorGUILayoutEx.MinMaxSlider(StringTable.Get(C.Scale),
                        ref minScale, ref maxScale, 0.01f, 100.0f);
                }
                EditorGUI.indentLevel--;

                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(StringTable.Get(C.Add)))
                {
                    if (obj != null)
                    {
                        ObjectDetail detail = new ObjectDetail
                        {
                            Object = obj,
                            MinScale = minScale,
                            MaxScale = maxScale,
                            UseUnifiedScale = useUnifiedScale,
                        };
                        this.detailList.Add(detail);
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
                if (objectDetail == null)
                {
                    objectDetail = detailList[editingIndex] as ObjectDetail;
                    if (objectDetail != null)
                    {
                        objectDetail = objectDetail.ShallowCopy();
                    }
                }

                if (objectDetail == null)
                {
                    EditorGUILayout.HelpBox(
                        $"Ignored invalid Object detail detected at index {editingIndex}",
                        MessageType.Warning);
                }
                else
                {
                    objectDetail.Object = (GameObject) EditorGUILayout.ObjectField(StringTable.Get(C.Prefab),
                        objectDetail.Object, typeof(GameObject), true);

                    EditorGUILayout.PrefixLabel(StringTable.Get(C.Scale));
                    EditorGUI.indentLevel++;
                    objectDetail.UseUnifiedScale =
                        EditorGUILayout.Toggle(StringTable.Get(C.UnifiedScale), objectDetail.UseUnifiedScale);
                    if (objectDetail.UseUnifiedScale)
                    {
                        var newMin = EditorGUILayout.FloatField("min", objectDetail.MinScale.x);
                        var newMax = EditorGUILayout.FloatField("max", objectDetail.MaxScale.x);
                        EditorGUILayout.MinMaxSlider(ref newMin, ref newMax, 0.01f, 100.0f);
                        if (newMin >= newMax)
                        {
                            newMin = newMax;
                        }

                        objectDetail.MinScale.z = objectDetail.MinScale.y = objectDetail.MinScale.x = newMin;
                        objectDetail.MaxScale.z = objectDetail.MaxScale.y = objectDetail.MaxScale.x = newMax;
                    }
                    else
                    {
                        EditorGUILayoutEx.MinMaxSlider(StringTable.Get(C.Scale),
                            ref objectDetail.MinScale, ref objectDetail.MaxScale, 0.01f, 100.0f);
                    }

                    EditorGUI.indentLevel--;

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(StringTable.Get(C.Apply)))
                    {
                        detailList[editingIndex] = objectDetail;
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