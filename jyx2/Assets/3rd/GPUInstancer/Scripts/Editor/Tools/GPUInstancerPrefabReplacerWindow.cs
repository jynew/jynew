using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPUInstancer
{
    // Inspired by: https://forum.unity.com/threads/replace-game-object-with-prefab.24311/
    public class GPUInstancerPrefabReplacerWindow : EditorWindow
    {
        private GameObject selectedPrefab;
        private bool replaceNames = true;

        private Vector2 scrollPos = Vector2.zero;
        private bool showHelpText;
        private Texture2D helpIcon;
        private Texture2D helpIconActive;

        public static void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(GPUInstancerPrefabReplacerWindow), true, "GPU Instancer Prefab Replacer", true);
            window.minSize = new Vector2(400, 560);
        }

        void OnGUI()
        {
            if (helpIcon == null)
                helpIcon = Resources.Load<Texture2D>(GPUInstancerConstants.EDITOR_TEXTURES_PATH + GPUInstancerEditorConstants.HELP_ICON);
            if (helpIconActive == null)
                helpIconActive = Resources.Load<Texture2D>(GPUInstancerConstants.EDITOR_TEXTURES_PATH + GPUInstancerEditorConstants.HELP_ICON_ACTIVE);

            EditorGUILayout.BeginHorizontal(GPUInstancerEditorConstants.Styles.box);
            EditorGUILayout.LabelField(GPUInstancerEditorConstants.GPUI_VERSION, GPUInstancerEditorConstants.Styles.boldLabel);
            GUILayout.FlexibleSpace();
            GPUInstancerEditor.DrawWikiButton(GUILayoutUtility.GetRect(40, 20), "#The_Prefab_Replacer");
            GUILayout.Space(10);
            DrawHelpButton(GUILayoutUtility.GetRect(20, 20), showHelpText);
            EditorGUILayout.EndHorizontal();

            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prefabReplacerIntro, true);

            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            GPUInstancerEditorConstants.DrawColoredButton(new GUIContent("Replace Selection With Prefab"), GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    ReplaceSelectionWithPrefab();
                });

            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.cancel, Color.red, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    this.Close();
                });

            EditorGUILayout.EndHorizontal();
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prefabReplacerReplaceCancel);

            GUILayout.Space(10);
            selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);

#if UNITY_2018_3_OR_NEWER
            if (selectedPrefab != null && 
                    PrefabUtility.GetPrefabAssetType(selectedPrefab) != PrefabAssetType.Regular 
                    && PrefabUtility.GetPrefabAssetType(selectedPrefab) != PrefabAssetType.Variant)
#else
            if (selectedPrefab != null && PrefabUtility.GetPrefabType(selectedPrefab) != PrefabType.Prefab)
#endif
                selectedPrefab = null;
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prefabReplacerPrefab);

            replaceNames = EditorGUILayout.Toggle("Replace Names", replaceNames);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prefabReplacerReplaceNames);

            GUILayout.Space(10);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            GUILayout.Space(5);
            GPUInstancerEditorConstants.DrawCustomLabel("Selected GameObjects", GPUInstancerEditorConstants.Styles.boldLabel);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prefabReplacerSelectedObjects);
            GUILayout.Space(5);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();
            foreach (Transform selectedTransform in Selection.transforms)
            {
                EditorGUILayout.LabelField(selectedTransform.name);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        public void ReplaceSelectionWithPrefab()
        {
            if (selectedPrefab != null && Selection.transforms.Length > 0)
            {
                GameObject prefabInstance;
                foreach (Transform selectedTransform in Selection.transforms)
                {
                    if (selectedTransform)
                    {
                        prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
                        if (!replaceNames)
                            prefabInstance.name = selectedTransform.name;
                        Undo.RegisterCreatedObjectUndo(prefabInstance, "GPUI Prefab Replacer");
                        prefabInstance.transform.parent = selectedTransform.parent;
                        prefabInstance.transform.SetSiblingIndex(selectedTransform.GetSiblingIndex());
                        prefabInstance.transform.localPosition = selectedTransform.localPosition;
                        prefabInstance.transform.localRotation = selectedTransform.localRotation;
                        prefabInstance.transform.localScale = selectedTransform.localScale;
                        Undo.DestroyObjectImmediate(selectedTransform.gameObject);
                    }
                }
                this.Close();
            }
        }

        public void DrawHelpText(string text, bool forceShow = false)
        {
            if (showHelpText || forceShow)
            {
                EditorGUILayout.HelpBox(text, MessageType.Info);
            }
        }

        public void DrawHelpButton(Rect buttonRect, bool showingHelp)
        {
            if (GUI.Button(buttonRect, new GUIContent(showHelpText ? helpIconActive : helpIcon,
                showHelpText ? GPUInstancerEditorConstants.TEXT_hideHelpTooltip : GPUInstancerEditorConstants.TEXT_showHelpTooltip), showHelpText ? GPUInstancerEditorConstants.Styles.helpButtonSelected : GPUInstancerEditorConstants.Styles.helpButton))
            {
                showHelpText = !showHelpText;
            }
        }
    }
}