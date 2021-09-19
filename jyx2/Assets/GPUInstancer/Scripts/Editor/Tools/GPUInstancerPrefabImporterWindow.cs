using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerPrefabImporterWindow : EditorWindow
    {
        private static List<GameObject> prefabList;
        private static bool[] isEnabledArray;
        private static int[] instanceCountArray;
        private static int maxInstanceCount;
        private int instanceCountToSelect;
        private Vector2 scrollPos = Vector2.zero;
        private bool showHelpText;
        private Texture2D helpIcon;
        private Texture2D helpIconActive;

        public static void ShowWindow(List<GameObject> prefabList, string title = null)
        {
            if (prefabList == null || prefabList.Count == 0)
            {
                EditorUtility.DisplayDialog(GPUInstancerEditorConstants.TEXT_info, GPUInstancerEditorConstants.TEXT_noPrefabInstanceFound, GPUInstancerEditorConstants.TEXT_close);
            }
            else
            {
                foreach (GameObject prefab in prefabList.ToArray())
                {
                    if (prefab.transform.GetComponentInChildren<MeshRenderer>() == null)
                        prefabList.Remove(prefab);
                }

                GPUInstancerPrefabImporterWindow.prefabList = prefabList;
                isEnabledArray = new bool[prefabList.Count];
                instanceCountArray = new int[prefabList.Count];
                GetPrefabInstanceCounts();
                //Show existing window instance. If one doesn't exist, make one.
                EditorWindow window = EditorWindow.GetWindow(typeof(GPUInstancerPrefabImporterWindow), true, title == null ? "GPU Instancer Scene Prefab Importer" : title, true);
                window.minSize = new Vector2(400, 560);
            }
        }

        void OnGUI()
        {
            if (helpIcon == null)
                helpIcon = Resources.Load<Texture2D>(GPUInstancerConstants.EDITOR_TEXTURES_PATH + GPUInstancerEditorConstants.HELP_ICON);
            if (helpIconActive == null)
                helpIconActive = Resources.Load<Texture2D>(GPUInstancerConstants.EDITOR_TEXTURES_PATH + GPUInstancerEditorConstants.HELP_ICON_ACTIVE);

            if (prefabList == null || prefabList.Count == 0)
                this.Close();

            EditorGUILayout.BeginHorizontal(GPUInstancerEditorConstants.Styles.box);
            EditorGUILayout.LabelField(GPUInstancerEditorConstants.GPUI_VERSION, GPUInstancerEditorConstants.Styles.boldLabel);
            GUILayout.FlexibleSpace();
            GPUInstancerEditor.DrawWikiButton(GUILayoutUtility.GetRect(40, 20), "#Scene_Prefab_Importer");
            GUILayout.Space(10);
            DrawHelpButton(GUILayoutUtility.GetRect(20, 20), showHelpText);
            EditorGUILayout.EndHorizontal();

            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prefabImporterIntro, true);

            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.importSelectedPrefabs, GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    List<GameObject> selectedPrefabs = new List<GameObject>();
                    for (int i = 0; i < prefabList.Count; i++)
                    {
                        if (isEnabledArray[i])
                            selectedPrefabs.Add(prefabList[i]);
                    }

                    if (selectedPrefabs.Count > 0)
                    {
                        GameObject go = new GameObject("GPUI Prefab Manager");
                        GPUInstancerPrefabManager prefabManager = go.AddComponent<GPUInstancerPrefabManager>();
                        selectedPrefabs.ForEach(p => GPUInstancerPrefabManagerEditor.AddPickerObject(prefabManager, p));
                        this.Close();
                        Selection.activeGameObject = go;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(GPUInstancerEditorConstants.TEXT_info, GPUInstancerEditorConstants.TEXT_noPrefabInstanceSelected, GPUInstancerEditorConstants.TEXT_close);
                    }
                });

            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.cancel, Color.red, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    this.Close();
                });

            EditorGUILayout.EndHorizontal();
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prefabImporterImportCancel);
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All"))
            {
                for (int i = 0; i < prefabList.Count; i++)
                {
                    isEnabledArray[i] = true;
                }
            }
            if (GUILayout.Button("Select None"))
            {
                for (int i = 0; i < prefabList.Count; i++)
                {
                    isEnabledArray[i] = false;
                }
            }
            EditorGUILayout.EndHorizontal();
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prefabImporterSelectAllNone);

            EditorGUILayout.BeginHorizontal();
            instanceCountToSelect = EditorGUILayout.IntSlider(instanceCountToSelect, 0, maxInstanceCount);
            if (GUILayout.Button("Select Min. Instance Count"))
            {
                for (int i = 0; i < prefabList.Count; i++)
                {
                    isEnabledArray[i] = instanceCountToSelect <= instanceCountArray[i];
                }
            }
            EditorGUILayout.EndHorizontal();
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prefabImporterSelectOverCount);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);

            GUILayout.Space(5);
            GPUInstancerEditorConstants.DrawCustomLabel("Prefab Instances", GPUInstancerEditorConstants.Styles.boldLabel);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prefabImporterInstanceList);
            GUILayout.Space(5);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();
            if (prefabList != null)
            {
                for (int i = 0; i < prefabList.Count; i++)
                {
                    isEnabledArray[i] = EditorGUILayout.BeginToggleGroup(prefabList[i].name + " (Instance Count: " + instanceCountArray[i] + ")", isEnabledArray[i]);
                    EditorGUILayout.EndToggleGroup();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            GUILayout.Space(5);
            EditorGUILayout.HelpBox(GPUInstancerEditorConstants.WARNINGTEXT_instanceCounts, MessageType.Warning);
            EditorGUILayout.EndVertical();

            //GameObject go = new GameObject("GPUI Prefab Manager");
            //GPUInstancerPrefabManager prefabManager = go.AddComponent<GPUInstancerPrefabManager>();

            //Selection.activeGameObject = go;
        }

        private static void GetPrefabInstanceCounts()
        {
            GameObject[] prefabInstances = (GameObject[])FindObjectsOfType(typeof(GameObject));
#if UNITY_2018_3_OR_NEWER
            Dictionary<GameObject, bool> prefabHasChildCacheDict = new Dictionary<GameObject, bool>();
            List<GameObject> prefabAssetList = GPUInstancerUtility.GetCorrespondingPrefabAssetsOfGameObjects(prefabInstances);
            Dictionary<GameObject, int> distinctPrefabs = new Dictionary<GameObject, int>();

            foreach (GameObject prefab in prefabAssetList)
            {
                if (!distinctPrefabs.ContainsKey(prefab))
                    distinctPrefabs.Add(prefab, 1);
                else
                    distinctPrefabs[prefab]++;
            }

            foreach (GameObject prefab in distinctPrefabs.Keys)
            {
                CalculateInstaneCountsForPrefab(prefab, prefabHasChildCacheDict, distinctPrefabs[prefab]);
            }
#else
            foreach (GameObject go in prefabInstances)
            {
                bool isPrefabInstance = PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance;
                if (isPrefabInstance)
                {
#if UNITY_2018_2_OR_NEWER
                    GameObject prefab = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(go);
#else
                    GameObject prefab = (GameObject)PrefabUtility.GetPrefabParent(go);
#endif
                    int prefabIndex = prefabList.IndexOf(prefab);
                    if (prefabIndex >= 0)
                    {
                        instanceCountArray[prefabIndex]++;
                    }
                }
            }
#endif
            maxInstanceCount = 0;
            for (int i = 0; i < instanceCountArray.Length; i++)
            {
                if (maxInstanceCount < instanceCountArray[i])
                    maxInstanceCount = instanceCountArray[i];
            }
        }

#if UNITY_2018_3_OR_NEWER
        private static void CalculateInstaneCountsForPrefab(GameObject prefab, Dictionary<GameObject, bool> prefabHasChildCacheDict, int additionCount = 1)
        {
            int prefabIndex = prefabList.IndexOf(prefab);

            if (prefabIndex >= 0)
                instanceCountArray[prefabIndex] += additionCount;
            else
                return;

            bool hasKey = prefabHasChildCacheDict.ContainsKey(prefab);
            if (hasKey && !prefabHasChildCacheDict[prefab])
                return;

            GameObject prefabContents = GPUInstancerUtility.LoadPrefabContents(prefab);
            List<Transform> childTransforms = new List<Transform>(prefabContents.GetComponentsInChildren<Transform>());
            childTransforms.Remove(prefabContents.transform);
            if (!hasKey)
                prefabHasChildCacheDict.Add(prefab, childTransforms.Count > 0);

            foreach (Transform childTransform in childTransforms)
            {
                if (!childTransform)
                    continue;
                GameObject cgo = childTransform.gameObject;
                if (PrefabUtility.GetPrefabAssetType(cgo) == PrefabAssetType.Regular)
                {
                    GameObject cprefab = PrefabUtility.GetCorrespondingObjectFromSource(cgo);
                    CalculateInstaneCountsForPrefab(cprefab, prefabHasChildCacheDict, additionCount);
                }
            }
            GPUInstancerUtility.UnloadPrefabContents(prefab, prefabContents, false);
        }
#endif

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