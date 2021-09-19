using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GPUInstancer
{
    [CustomEditor(typeof(GPUInstancerTreeManager))]
    [CanEditMultipleObjects]
    public class GPUInstancerTreeManagerEditor : GPUInstancerManagerEditor
    {
        protected SerializedProperty prop_initializeWithCoroutine;
        protected SerializedProperty prop_additionalTerrains;

        private GPUInstancerTreeManager _treeManager;

        protected override void OnEnable()
        {
            base.OnEnable();

            wikiHash = "#The_Tree_Manager";

            prop_initializeWithCoroutine = serializedObject.FindProperty("initializeWithCoroutine");
            prop_additionalTerrains = serializedObject.FindProperty("additionalTerrains");

            _treeManager = (target as GPUInstancerTreeManager);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            if (_treeManager.terrain == null)
            {
                if (!Application.isPlaying && Event.current.type == EventType.ExecuteCommand && pickerControlID > 0 && Event.current.commandName == "ObjectSelectorClosed")
                {
                    if (EditorGUIUtility.GetObjectPickerControlID() == pickerControlID)
                        AddTerrainPickerObject(EditorGUIUtility.GetObjectPickerObject());
                    pickerControlID = -1;
                }

                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                DrawTreeTerrainAddButton();
                EditorGUI.EndDisabledGroup();
                return;
            }
            else if (_treeManager.terrainSettings == null)
                _treeManager.SetupManagerWithTerrain(_treeManager.terrain);

            DrawSceneSettingsBox();

            if (_treeManager.terrainSettings != null)
            {
                DrawGlobalValuesBox();

                if (Application.isPlaying)
                    DrawRegisteredPrefabsBox();

                DrawGPUInstancerPrototypesBox();
            }

            HandlePickerObjectSelection();

            serializedObject.ApplyModifiedProperties();

            base.InspectorGUIEnd();
        }

        public override void ShowObjectPicker()
        {
            base.ShowObjectPicker();

            EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "", pickerControlID);
        }

        public override void AddPickerObject(UnityEngine.Object pickerObject, GPUInstancerPrototype overridePrototype = null)
        {
            base.AddPickerObject(pickerObject, overridePrototype);

            if (pickerObject == null)
                return;

            if (!(pickerObject is GameObject))
            {
                EditorUtility.DisplayDialog(GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_TITLE, GPUInstancerConstants.TEXT_TREE_PREFAB_TYPE_WARNING, GPUInstancerConstants.TEXT_OK);
                return;
            }

            GameObject prefabObject = (GameObject)pickerObject;

#if UNITY_2018_3_OR_NEWER
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(pickerObject);

            if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant || prefabType == PrefabAssetType.Model)
            {
                GameObject newPrefabObject = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(prefabObject);
                if (newPrefabObject != null)
                {
                    while (newPrefabObject.transform.parent != null)
                        newPrefabObject = newPrefabObject.transform.parent.gameObject;
                    prefabObject = newPrefabObject;
                }
            }
            else
            {
                EditorUtility.DisplayDialog(GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_TITLE, GPUInstancerConstants.TEXT_TREE_PREFAB_TYPE_WARNING, GPUInstancerConstants.TEXT_OK);
                return;
            }
#else
            PrefabType prefabType = PrefabUtility.GetPrefabType(pickerObject);

            if (prefabType != PrefabType.Prefab && prefabType != PrefabType.ModelPrefab)
            {
                bool instanceFound = false;
                if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.ModelPrefabInstance)
                {
#if UNITY_2018_2_OR_NEWER
                    GameObject newPrefabObject = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(prefabObject);
#else
                    GameObject newPrefabObject = (GameObject)PrefabUtility.GetPrefabParent(prefabObject);
#endif
                    if (PrefabUtility.GetPrefabType(newPrefabObject) == PrefabType.Prefab || PrefabUtility.GetPrefabType(newPrefabObject) == PrefabType.ModelPrefab)
                    {
                        while (newPrefabObject.transform.parent != null)
                            newPrefabObject = newPrefabObject.transform.parent.gameObject;
                        prefabObject = newPrefabObject;
                        instanceFound = true;
                    }
                }
                if (!instanceFound)
                {
                    EditorUtility.DisplayDialog(GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_TITLE, GPUInstancerConstants.TEXT_TREE_PREFAB_TYPE_WARNING, GPUInstancerConstants.TEXT_OK);
                    return;
                }
            }
#endif

            Undo.RecordObject(this, "Add tree prototype");

            if (_treeManager.terrainSettings != null && _treeManager.terrain != null && _treeManager.terrain.terrainData != null)
            {
                if (overridePrototype != null)
                {
                    int prototypeIndex = prototypeList.IndexOf(overridePrototype);
                    if (prototypeIndex >= 0 && prototypeIndex < _treeManager.terrain.terrainData.treePrototypes.Length)
                    {
                        TreePrototype[] treePrototypes = _treeManager.terrain.terrainData.treePrototypes;

                        treePrototypes[prototypeIndex].prefab = prefabObject;
                        overridePrototype.prefabObject = prefabObject;
                        _treeManager.terrain.terrainData.treePrototypes = treePrototypes;
                        _treeManager.terrain.terrainData.RefreshPrototypes();

                        GPUInstancerUtility.DetermineTreePrototypeType(overridePrototype);

                        if (overridePrototype.billboard != null && overridePrototype.useGeneratedBillboard)
                        {
                            GPUInstancerUtility.GeneratePrototypeBillboard(overridePrototype, true);
                        }
                    }
                }
                else
                {
                    List<TreePrototype> newTreePrototypes = new List<TreePrototype>(_treeManager.terrain.terrainData.treePrototypes);

                    TreePrototype terrainTreePrototype = new TreePrototype()
                    {
                        prefab = prefabObject,
                        bendFactor = 0
                    };
                    newTreePrototypes.Add(terrainTreePrototype);

                    _treeManager.terrain.terrainData.treePrototypes = newTreePrototypes.ToArray();
                    _treeManager.terrain.terrainData.RefreshPrototypes();
                    GPUInstancerUtility.AddTreeInstancePrototypeFromTerrainPrototype(_treeManager.gameObject, prototypeList, terrainTreePrototype, 
                        newTreePrototypes.Count - 1, _treeManager.terrainSettings);
                }
            }
        }

        public void ShowTerrainPicker()
        {
            EditorGUIUtility.ShowObjectPicker<Terrain>(null, true, null, pickerControlID);
        }

        public void AddTerrainPickerObject(UnityEngine.Object pickerObject)
        {
            if (pickerObject == null)
                return;

            if (pickerObject is GameObject)
            {
                GameObject go = (GameObject)pickerObject;
                if (go.GetComponent<Terrain>() != null)
                {
                    _treeManager.SetupManagerWithTerrain(go.GetComponent<Terrain>());
                }
            }
        }

        public override void DrawSettingContents()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            //EditorGUILayout.PropertyField(prop_settings);

            if (Application.isPlaying)
            {
                int index = 1;
                foreach (Terrain terrain in _treeManager.GetTerrains())
                {
                    EditorGUILayout.ObjectField("Terrain " + index, terrain, typeof(Terrain), true);
                    index++;
                }
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Main " + GPUInstancerEditorConstants.TEXT_terrain, _treeManager.terrain, typeof(Terrain), true);
                EditorGUI.EndDisabledGroup();

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(prop_additionalTerrains, true);
                EditorGUI.indentLevel--;

                if (prop_additionalTerrains.arraySize > 0)
                {
                    EditorGUILayout.HelpBox("When using Tree Manager with multiple Terrains, it requires every Terrain to have the same Tree prototypes defined on it. Adding Terrains with different tree prototypes might cause incorrect rendering and errors.", MessageType.Warning);
                }
            }

            EditorGUILayout.BeginHorizontal();
            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.paintOnTerrain, GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    if (_treeManager.terrain != null)
                    {
                        GPUInstancerTerrainProxy proxy = _treeManager.AddProxyToTerrain();
                        Selection.activeGameObject = _treeManager.terrain.gameObject;

                        proxy.terrainSelectedToolIndex = 4;
                    }
                });
            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.removeTerrain, Color.red, Color.white, FontStyle.Bold, Rect.zero,
            () =>
            {
                if (EditorUtility.DisplayDialog(GPUInstancerEditorConstants.TEXT_removeTerrainConfirmation, GPUInstancerEditorConstants.TEXT_removeTerrainAreYouSure, GPUInstancerEditorConstants.TEXT_unset, GPUInstancerEditorConstants.TEXT_cancel))
                {
                    _treeManager.SetupManagerWithTerrain(null);
                }
            });
            EditorGUILayout.EndHorizontal();
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_terrain);

            EditorGUILayout.Space();

            EditorGUI.EndDisabledGroup();

            DrawCameraDataFields();

            DrawCullingSettings(_treeManager.prototypeList);

            DrawFloatingOriginFields();

            DrawLayerMaskFields();
        }

        public void DrawTreeTerrainAddButton()
        {
            GUILayout.Space(10);
            Rect buttonRect = GUILayoutUtility.GetRect(100, 40, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));

            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.setTerrain, GPUInstancerEditorConstants.Colors.lightBlue, Color.white, FontStyle.Bold, buttonRect,
                () =>
                {
                    pickerControlID = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
                    ShowTerrainPicker();
                },
                true, true,
                (o) =>
                {
                    AddTerrainPickerObject(o);
                });
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_setTerrainTree, true);
            GUILayout.Space(10);
        }

        public override void DrawGlobalValuesContents()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(GPUInstancerEditorConstants.TEXT_terrainSettingsSO, _treeManager.terrainSettings, typeof(GPUInstancerTerrainSettings), false);
            EditorGUI.EndDisabledGroup();

            float newMaxTreeDistance = EditorGUILayout.Slider(GPUInstancerEditorConstants.TEXT_maxTreeDistance, _treeManager.terrainSettings.maxTreeDistance, 0,
                GPUInstancerConstants.gpuiSettings.MAX_TREE_DISTANCE);
            if (_treeManager.terrainSettings.maxTreeDistance != newMaxTreeDistance)
            {
                foreach (GPUInstancerPrototype p in _treeManager.prototypeList)
                {
                    if (p.maxDistance == _treeManager.terrainSettings.maxTreeDistance || p.maxDistance > newMaxTreeDistance)
                    {
                        p.maxDistance = newMaxTreeDistance;
                        EditorUtility.SetDirty(p);
                    }
                }
                _treeManager.terrainSettings.maxTreeDistance = newMaxTreeDistance;
            }
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_maxTreeDistance);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_treeManager, "Editor data changed.");
                OnEditorDataChanged();
                EditorUtility.SetDirty(_treeManager.terrainSettings);
            }

            EditorGUILayout.PropertyField(prop_initializeWithCoroutine, GPUInstancerEditorConstants.Contents.initializeWithCoroutine);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_initializeWithCoroutine);

            EditorGUI.EndDisabledGroup();
        }

        public override string GetGlobalValuesTitle()
        {
            return GPUInstancerEditorConstants.TEXT_treeGlobal;
        }

        public override void DrawAddPrototypeHelpText()
        {
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_addprototypetree);
        }

        public override void DrawPrototypeBoxButtons()
        {
            if (!Application.isPlaying)
            {
                GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.generatePrototypes, GPUInstancerEditorConstants.Colors.darkBlue, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    if (EditorUtility.DisplayDialog(GPUInstancerEditorConstants.TEXT_generatePrototypesConfirmation, GPUInstancerEditorConstants.TEXT_generatePrototypeAreYouSure, GPUInstancerEditorConstants.TEXT_generatePrototypes, GPUInstancerEditorConstants.TEXT_cancel))
                    {
                        _treeManager.GeneratePrototypes(true);
                    }
                });
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_generatePrototypesTree);

                GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.regenerateBillboards, GPUInstancerEditorConstants.Colors.darkBlue, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    if (EditorUtility.DisplayDialog(GPUInstancerEditorConstants.TEXT_regenerateBillboardsConfirmation, GPUInstancerEditorConstants.TEXT_regenerateBillboardsAreYouSure, GPUInstancerEditorConstants.TEXT_regenerateBillboards, GPUInstancerEditorConstants.TEXT_cancel))
                    {
                        foreach (GPUInstancerPrototype prototype in _treeManager.prototypeList)
                        {
                            if (prototype.useGeneratedBillboard)
                            {
                                GPUInstancerUtility.GeneratePrototypeBillboard(prototype, true);
                            }
                        }
                    }
                });
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_regenerateBillboards);
            }
        }

        public override bool DrawGPUInstancerPrototypeInfo(List<GPUInstancerPrototype> selectedPrototypeList)
        {
            return DrawGPUInstancerPrototypeInfo(selectedPrototypeList, (string t) => { DrawHelpText(t); }, _treeManager, null, null, _treeManager.terrainSettings);
        }

        public static bool DrawGPUInstancerPrototypeInfo(List<GPUInstancerPrototype> selectedPrototypeList, UnityAction<string> DrawHelpText, Object component, UnityAction OnEditorDataChanged,
            GPUInstancerEditorSimulator simulator, GPUInstancerTerrainSettings terrainSettings)
        {
            GPUInstancerTreePrototype prototype0 = (GPUInstancerTreePrototype)selectedPrototypeList[0];
            #region Determine Multiple Values
            bool hasChanged = false;
            bool isApplyRotationMixed = false;
            bool isApplyRotation = prototype0.isApplyRotation;
            bool isApplyPrefabScaleMixed = false;
            bool isApplyPrefabScale = prototype0.isApplyPrefabScale;
            bool isApplyTerrainHeightMixed = false;
            bool isApplyTerrainHeight = prototype0.isApplyTerrainHeight;
            for (int i = 1; i < selectedPrototypeList.Count; i++)
            {
                GPUInstancerTreePrototype prototypeI = (GPUInstancerTreePrototype)selectedPrototypeList[i];
                if (!isApplyRotationMixed && isApplyRotation != prototypeI.isApplyRotation)
                    isApplyRotationMixed = true;
                if (!isApplyPrefabScaleMixed && isApplyPrefabScale != prototypeI.isApplyPrefabScale)
                    isApplyPrefabScaleMixed = true;
                if (!isApplyTerrainHeightMixed && isApplyTerrainHeight != prototypeI.isApplyTerrainHeight)
                    isApplyTerrainHeightMixed = true;
            }
            #endregion Determine Multiple Values

            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_treeSettings, GPUInstancerEditorConstants.Styles.boldLabel);

            hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_useRandomTreeRotation, isApplyRotation, isApplyRotationMixed, (p, v) => ((GPUInstancerTreePrototype)p).isApplyRotation = v);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_useRandomTreeRotation);

            hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_useTerrainHeight, isApplyTerrainHeight, isApplyTerrainHeightMixed, (p, v) => ((GPUInstancerTreePrototype)p).isApplyTerrainHeight = v);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_useTerrainHeight);

            hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_usePrefabScale, isApplyPrefabScale, isApplyPrefabScaleMixed, (p, v) => ((GPUInstancerTreePrototype)p).isApplyPrefabScale = v);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_usePrefabScale);

            EditorGUILayout.EndVertical();

            return hasChanged;
        }

        public override void DrawGPUInstancerPrototypeInfo(GPUInstancerPrototype selectedPrototype)
        {
            DrawGPUInstancerPrototypeInfo(selectedPrototype, (string t) => { DrawHelpText(t); }, _treeManager, null, null, _treeManager.terrainSettings);
        }

        public static void DrawGPUInstancerPrototypeInfo(GPUInstancerPrototype selectedPrototype, UnityAction<string> DrawHelpText, Object component, UnityAction OnEditorDataChanged,
            GPUInstancerEditorSimulator simulator, GPUInstancerTerrainSettings terrainSettings)
        {
            GPUInstancerTreePrototype treePrototype = (GPUInstancerTreePrototype)selectedPrototype;
            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_treeSettings, GPUInstancerEditorConstants.Styles.boldLabel);

            treePrototype.isApplyRotation = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_useRandomTreeRotation, treePrototype.isApplyRotation);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_useRandomTreeRotation);

            treePrototype.isApplyTerrainHeight = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_useTerrainHeight, treePrototype.isApplyTerrainHeight);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_useTerrainHeight);

            treePrototype.isApplyPrefabScale = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_usePrefabScale, treePrototype.isApplyPrefabScale);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_usePrefabScale);

            EditorGUILayout.EndVertical();
        }

        public override void DrawGPUInstancerPrototypeActions()
        {
            GUILayout.Space(10);
            GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_actions, GPUInstancerEditorConstants.Styles.boldLabel, false);

            DrawDeleteButton();
        }

        public override float GetMaxDistance(GPUInstancerPrototype selectedPrototype)
        {
            return _treeManager.terrainSettings != null ? _treeManager.terrainSettings.maxTreeDistance : GPUInstancerConstants.gpuiSettings.MAX_TREE_DISTANCE;
        }

        public override void DrawPrefabField(GPUInstancerPrototype selectedPrototype)
        {
            EditorGUILayout.BeginHorizontal();
            base.DrawPrefabField(selectedPrototype);
            if (!Application.isPlaying)
            {
                Rect prototypeRect = GUILayoutUtility.GetRect(120, 20, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.editPrefab, GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, prototypeRect,
                () =>
                {
                    _pickerOverride = selectedPrototype;
                    pickerControlID = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
                    ShowObjectPicker();
                },
                true, true,
                (o) =>
                {
                    AddPickerObject(o, selectedPrototype);
                    prototypeContents = null;
                });
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}