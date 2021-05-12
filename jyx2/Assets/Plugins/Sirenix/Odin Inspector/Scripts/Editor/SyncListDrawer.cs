//-----------------------------------------------------------------------
// <copyright file="SyncListDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR && !UNITY_2019_1_OR_NEWER
#pragma warning disable 0618

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;

    /// <summary>
    /// SyncList property drawer.
    /// </summary>
    [DrawerPriority(0, 0, 2)]
    public class SyncListDrawer<TList, TElement> : OdinValueDrawer<TList> where TList : SyncList<TElement>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var property = entry.Property;
            int minCount = int.MaxValue;
            int maxCount = 0;

            PropertyContext<bool> isVisible;

            if (entry.Context.Get(this, "is_visible", out isVisible))
            {
                isVisible.Value = GeneralDrawerConfig.Instance.OpenListsByDefault;
            }

            for (int i = 0; i < entry.ValueCount; i++)
            {
                if (entry.Values[i].Count > maxCount)
                {
                    maxCount = entry.Values[i].Count;
                }

                if (entry.Values[i].Count < minCount)
                {
                    minCount = entry.Values[i].Count;
                }
            }

            SirenixEditorGUI.BeginHorizontalToolbar();
            isVisible.Value = SirenixEditorGUI.Foldout(isVisible.Value, GUIHelper.TempContent("SyncList " + label.text + "  [" + typeof(TList).Name + "]"));
            EditorGUILayout.LabelField(GUIHelper.TempContent(minCount == maxCount ? (minCount == 0 ? "Empty" : minCount + " items") : minCount + " (" + maxCount + ") items"), SirenixGUIStyles.RightAlignedGreyMiniLabel);
            SirenixEditorGUI.EndHorizontalToolbar();

            if (SirenixEditorGUI.BeginFadeGroup(isVisible, isVisible.Value))
            {
                GUIHelper.PushGUIEnabled(false);
                SirenixEditorGUI.BeginVerticalList();
                {
                    var elementLabel = new GUIContent();
                    for (int i = 0; i < maxCount; i++)
                    {
                        SirenixEditorGUI.BeginListItem();
                        elementLabel.text = "Item " + i;

                        if (i < minCount)
                        {
                            property.Children[i].Draw(elementLabel);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(elementLabel, SirenixEditorGUI.MixedValueDashChar);
                        }
                        SirenixEditorGUI.EndListItem();
                    }
                }
                SirenixEditorGUI.EndVerticalList();
                GUIHelper.PopGUIEnabled();
            }
            SirenixEditorGUI.EndFadeGroup();
        }
    }
}

#endif // UNITY_EDITOR && !UNITY_2019_1_OR_NEWER