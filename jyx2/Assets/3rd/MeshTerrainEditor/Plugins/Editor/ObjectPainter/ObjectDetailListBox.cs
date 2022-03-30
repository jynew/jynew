using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MTE
{
    internal class ObjectDetailListBox : DetailListBox<ObjectDetail>
    {
        public override void NoDetailGUI()
        {
            EditorGUILayout.LabelField(StringTable.Get(C.Warning_NoPrefabAdded));

            if (GUILayout.Button(StringTable.Get(C.LoadFromFile)))
            {
                string path;
                if(Utility.OpenFileDialog(StringTable.Get(C.Open), s_assetFileFilter, out path))
                {
                    LoadDetailListFromAFile(path);
                }
            }
        }

        public override void DrawButtonBackground(int detailIndex, Rect buttonRect)
        {
            var detail = this.detailList[detailIndex] as ObjectDetail;
            if (detail == null)
            {
                Debug.LogWarning($"Ignored invalid Object detail at {detailIndex}");
                return;
            }

            var rect = buttonRect;
            rect.min += new Vector2(5, 5);
            rect.max -= new Vector2(5, 5);

            //draw preview texture
            var previewTexture = AssetPreview.GetAssetPreview(detail.Object);
            if (previewTexture)
            {
                GUI.DrawTexture(rect, previewTexture);
            }
        }

        protected override void SaveDetailList()
        {
            var path = Res.DetailDir + "SavedObjectDetailList.asset";
            var relativePath = Utility.GetUnityPath(path);
            ObjectDetailList obj = ScriptableObject.CreateInstance<ObjectDetailList>();
            obj.list = this.detailList as List<ObjectDetail>;
            AssetDatabase.CreateAsset(obj, relativePath);
            MTEDebug.LogFormat("ObjectDetailList saved to {0}", path);
        }

        protected override C DetailType { get; } = C.Object;

        protected override void AddCallback()
        {
            ObjectDetailEditorWindow window = ScriptableObject.CreateInstance<ObjectDetailEditorWindow>();
            window.titleContent = new GUIContent($"{StringTable.Get(C.Add)} {StringTable.Get(DetailType)}");
            window.detailList = this.detailList as List<ObjectDetail>;
            window.IsAdding = true;
            window.OnSave = this.SaveDetailList;
            window.ShowUtility();
        }

        protected override void EditCallback()
        {
            ObjectDetailEditorWindow window = ScriptableObject.CreateInstance<ObjectDetailEditorWindow>();
            window.titleContent = new GUIContent($"{StringTable.Get(C.Edit)} {StringTable.Get(DetailType)}");
            window.detailList = this.detailList as List<ObjectDetail>;
            window.editingIndex = selectedIndex;
            window.IsAdding = false;
            window.OnSave = this.SaveDetailList;
            window.ShowUtility();
        }

        public void LoadDetailListFromAFile(string path)
        {
            MTEDebug.LogFormat("Loading Object detail list from <{0}>", path);
            var relativePath = Utility.GetUnityPath(path);
            var obj = AssetDatabase.LoadAssetAtPath<ObjectDetailList>(relativePath);
            if (obj != null)
            {
                this.detailList = obj.list;
                MTEDebug.LogFormat("Detail list loaded from {0}", path);
                if (this.detailList.Count == 0)
                {
                    MTEDebug.Log("No detail found in the detail list.");
                }
            }
            else
            {
                MTEDebug.LogFormat("No detail list found in {0}.", path);
            }
        }
        
        private static readonly string[] s_assetFileFilter = {"detail list", "asset"};
    }
}