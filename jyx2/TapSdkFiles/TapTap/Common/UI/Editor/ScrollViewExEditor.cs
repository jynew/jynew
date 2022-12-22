// -----------------------------------------------------------------------
// <copyright file="ScrollViewExEditor.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace TapSDK.UI.AillieoTech
{
    using UnityEditor;

    [CustomEditor(typeof(ScrollViewEx))]
    public class ScrollViewExEditor : ScrollViewEditor
    {
        private SerializedProperty pageSize;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.pageSize = this.serializedObject.FindProperty("pageSize");
        }

        protected override void DrawConfigInfo()
        {
            base.DrawConfigInfo();
            EditorGUILayout.PropertyField(this.pageSize);
        }

        [MenuItem("GameObject/UI/DynamicScrollViewEx", false, 90)]
        private static void AddScrollViewEx(MenuCommand menuCommand)
        {
            InternalAddScrollView<ScrollViewEx>(menuCommand);
        }
    }
}
