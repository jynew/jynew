using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPUInstancer
{
    public abstract class GPUInstancerManagerEditor : GPUInstancerEditor
    {
        private GPUInstancerManager _manager;
        protected GPUInstancerPrototype _pickerOverride;

        protected SerializedProperty prop_useFloatingOriginHandler;
        protected SerializedProperty prop_floatingOriginTransform;

        protected SerializedProperty prop_layerMask;
        protected SerializedProperty prop_disableLightProbes;

        protected int pickerControlID = -1;
        protected bool editorDataChanged = false;
        protected int pickerMode = 0;

#if !UNITY_2017_1_OR_NEWER
        private PreviewRenderUtility _previewRenderUtility;
#endif

        protected override void OnEnable()
        {
            base.OnEnable();

            prototypeContents = null;
            _pickerOverride = null;

            _manager = (target as GPUInstancerManager);
            FillPrototypeList();

            prop_useFloatingOriginHandler = serializedObject.FindProperty("useFloatingOriginHandler");
            prop_floatingOriginTransform = serializedObject.FindProperty("floatingOriginTransform");

            prop_layerMask = serializedObject.FindProperty("layerMask");
            prop_disableLightProbes = serializedObject.FindProperty("lightProbeDisabled");

            showSceneSettingsBox = _manager.showSceneSettingsBox;
            showPrototypeBox = _manager.showPrototypeBox;
            showAdvancedBox = _manager.showAdvancedBox;
            showHelpText = _manager.showHelpText;
            showDebugBox = _manager.showDebugBox;
            showGlobalValuesBox = _manager.showGlobalValuesBox;
            showRegisteredPrefabsBox = _manager.showRegisteredPrefabsBox;
            showPrototypesBox = _manager.showPrototypesBox;
        }

        public override void FillPrototypeList()
        {
            prototypeList = _manager.prototypeList;

            UpdatePrototypeSelection();
        }

        public virtual void UpdatePrototypeSelection()
        {
            if (prototypeSelection == null)
                prototypeSelection = new Dictionary<GPUInstancerPrototype, bool>();
            prototypeSelection.Clear();

            if (_manager.selectedPrototypeList == null)
                _manager.selectedPrototypeList = new List<GPUInstancerPrototype>();

            foreach (GPUInstancerPrototype prototype in prototypeList)
            {
                prototypeSelection.Add(prototype, _manager.selectedPrototypeList.Contains(prototype));
            }
        }

        public override void OnInspectorGUI()
        {
            if (GPUInstancerConstants.gpuiSettings != null)
            {
                useCustomPreviewBackgroundColor = GPUInstancerConstants.gpuiSettings.useCustomPreviewBackgroundColor;
                previewBackgroundColor = GPUInstancerConstants.gpuiSettings.previewBackgroundColor;
            }

            isTextMode = _manager.isPrototypeTextMode;

            base.OnInspectorGUI();

            if (Application.isPlaying && _manager.cameraData != null && _manager.cameraData.mainCamera == null)
                EditorGUILayout.HelpBox(GPUInstancerEditorConstants.ERRORTEXT_cameraNotFound, MessageType.Error);
        }

        public override void InspectorGUIEnd()
        {
            _manager.showSceneSettingsBox = showSceneSettingsBox;
            _manager.showPrototypeBox = showPrototypeBox;
            _manager.showAdvancedBox = showAdvancedBox;
            _manager.showHelpText = showHelpText;
            _manager.showDebugBox = showDebugBox;
            _manager.showGlobalValuesBox = showGlobalValuesBox;
            _manager.showRegisteredPrefabsBox = showRegisteredPrefabsBox;
            _manager.showPrototypesBox = showPrototypesBox;
            base.InspectorGUIEnd();
        }

        public virtual void ShowObjectPicker()
        {

        }

        public virtual void AddPickerObject(UnityEngine.Object pickerObject, GPUInstancerPrototype overridePrototype = null)
        {
            if (overridePrototype != null && GPUInstancerDefines.previewCache != null)
            {
                GPUInstancerDefines.previewCache.RemovePreview(overridePrototype);
            }
        }

        public virtual void OnEditorDataChanged()
        {
            editorDataChanged = true;
        }

        public virtual void ApplyEditorDataChanges()
        {

        }

        public bool HandlePickerObjectSelection()
        {
            if (!Application.isPlaying && Event.current.type == EventType.ExecuteCommand && pickerControlID > 0 && Event.current.commandName == "ObjectSelectorClosed")
            {
                if (EditorGUIUtility.GetObjectPickerControlID() == pickerControlID)
                {
                    AddPickerObject(EditorGUIUtility.GetObjectPickerObject(), _pickerOverride);
                    prototypeContents = null;
                }
                pickerControlID = -1;
                return true;
            }
            return false;
        }

        public override void DrawFloatingOriginFields()
        {
            EditorGUILayout.PropertyField(prop_useFloatingOriginHandler, GPUInstancerEditorConstants.Contents.useFloatingOriginHandler);
            if (_manager is GPUInstancerTerrainManager)
            {
                if (prop_useFloatingOriginHandler.boolValue)
                {
                    GPUInstancerTerrainManager terrainManager = (GPUInstancerTerrainManager)_manager;
                    if (terrainManager.terrain != null && terrainManager.terrain.transform != _manager.floatingOriginTransform)
                    {
                        Undo.RecordObject(this, "Floating Origin Transform Set");
                        _manager.floatingOriginTransform = terrainManager.terrain.transform;
                    }
                }
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_floatingOriginTerrain);
            }
            else
            {
                if (prop_useFloatingOriginHandler.boolValue)
                {
                    EditorGUILayout.PropertyField(prop_floatingOriginTransform, GPUInstancerEditorConstants.Contents.floatingOriginTransform);
                }
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_floatingOriginPrefab);
            }
        }

        public override void DrawLayerMaskFields()
        {
            EditorGUILayout.PropertyField(prop_disableLightProbes, GPUInstancerEditorConstants.Contents.disableLightProbes);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_disableLightProbes);
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.PropertyField(prop_layerMask, GPUInstancerEditorConstants.Contents.layerMask);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_layerMask);
            EditorGUI.EndDisabledGroup();
        }

        public void DrawDebugBox(GPUInstancerEditorSimulator simulator = null)
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);

                Rect foldoutRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
                foldoutRect.x += 12;
                showDebugBox = EditorGUI.Foldout(foldoutRect, showDebugBox, GPUInstancerEditorConstants.TEXT_debug, true, GPUInstancerEditorConstants.Styles.foldout);

                if (showDebugBox)
                {
                    if (simulator != null)
                    {
                        if (simulator.simulateAtEditor)
                        {
                            if (simulator.initializingInstances)
                            {
                                EditorGUI.BeginDisabledGroup(true);
                                GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.simulateAtEditorPrep, GPUInstancerEditorConstants.Colors.darkBlue, Color.white,
                                    FontStyle.Bold, Rect.zero, null);
                                EditorGUI.EndDisabledGroup();
                            }
                            else
                            {
                                GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.simulateAtEditorStop, Color.red, Color.white,
                                    FontStyle.Bold, Rect.zero, () =>
                                    {
                                        simulator.StopSimulation();
                                    });
                            }
                        }
                        else
                        {
                            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.simulateAtEditor, GPUInstancerEditorConstants.Colors.green, Color.white,
                                FontStyle.Bold, Rect.zero, () =>
                                {
                                    simulator.StartSimulation();
                                });
                        }
                    }
                    DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_simulator);

                    _manager.keepSimulationLive = EditorGUILayout.Toggle("Keep Simulation Live", _manager.keepSimulationLive);

                    if (_manager.keepSimulationLive)
                    {
                        _manager.updateSimulation = EditorGUILayout.Toggle(new GUIContent("Update Simulation On Change", "Update Simulation On Change"), _manager.updateSimulation);

                        EditorGUILayout.HelpBox("Simulation is kept alive. The simulation might not show every change on terrain details. Changing some prototype settings during the simulation can also cause errors. Please stop and start the simulation again to solve these issues.", MessageType.Warning);
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        public virtual void DrawGlobalValuesBox()
        {
            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);

            Rect foldoutRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
            foldoutRect.x += 12;
            showGlobalValuesBox = EditorGUI.Foldout(foldoutRect, showGlobalValuesBox, GetGlobalValuesTitle(), true, GPUInstancerEditorConstants.Styles.foldout);

            if (showGlobalValuesBox)
            {
                DrawGlobalValuesContents();
            }
            EditorGUILayout.EndVertical();
        }

        public virtual void DrawGlobalValuesContents()
        {
        }

        public virtual string GetGlobalValuesTitle()
        {
            return GPUInstancerEditorConstants.TEXT_prefabGlobal;
        }

        public virtual void DrawRegisteredPrefabsBox()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);

            Rect foldoutRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
            foldoutRect.x += 12;
            showRegisteredPrefabsBox = EditorGUI.Foldout(foldoutRect, showRegisteredPrefabsBox, GPUInstancerEditorConstants.TEXT_registeredPrefabs, true, GPUInstancerEditorConstants.Styles.foldout);

            if (showRegisteredPrefabsBox)
            {
                DrawRegisteredPrefabsBoxButtons();

                DrawRegisteredPrefabsBoxList();

                _manager.selectedPrototypeList.Clear();
                foreach (GPUInstancerPrototype prototype in _manager.prototypeList)
                {
                    if (prototypeSelection[prototype])
                        _manager.selectedPrototypeList.Add(prototype);
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
        }

        public virtual void DrawRegisteredPrefabsBoxButtons()
        {

        }

        public virtual void DrawRegisteredPrefabsBoxList()
        {
            if ((Application.isPlaying || _manager.isInitialized) && _manager.runtimeDataList != null && _manager.runtimeDataList.Count > 0)
            {
                int totalInsanceCount = 0;
                int totalDrawCallCount = 0;
                int totalShadowDrawCall = 0;
                foreach (GPUInstancerRuntimeData runtimeData in _manager.runtimeDataList)
                {
                    if (runtimeData == null)
                        continue;

                    int drawCallCount = 0;
                    int shadowDrawCallCount = 0;
                    if (runtimeData.transformationMatrixVisibilityBuffer != null && runtimeData.instanceLODs != null && runtimeData.bufferSize > 0 && runtimeData.instanceCount > 0)
                    {
                        for (int i = 0; i < runtimeData.instanceLODs.Count; i++)
                        {
                            for (int j = 0; j < runtimeData.instanceLODs[i].renderers.Count; j++)
                            {
                                GPUInstancerRenderer gpuiRenderer = runtimeData.instanceLODs[i].renderers[j];
                                if (!GPUInstancerUtility.IsInLayer(prop_layerMask.intValue, gpuiRenderer.layer))
                                    continue;
                                drawCallCount += gpuiRenderer.materials.Count;
                                if (runtimeData.prototype.isShadowCasting && gpuiRenderer.castShadows && runtimeData.IsLODShadowCasting(i))
                                    shadowDrawCallCount += gpuiRenderer.materials.Count * QualitySettings.shadowCascades;
                            }
                        }
                    }
                    string prototypeName = runtimeData.prototype.ToString();
                    GUILayout.Label(prototypeName + " Instance Count: " + String.Format("{0:n0}", runtimeData.instanceCount) +
                        //"\n" + prototypeName + " Buffer Size: " + String.Format("{0:n0}", runtimeData.bufferSize) +
                        "\n" + prototypeName + " Geometry Draw Call Count: " + drawCallCount +
                        (shadowDrawCallCount > 0 ? "\n" + prototypeName + " Shadow Draw Call Count: " + shadowDrawCallCount : ""), GPUInstancerEditorConstants.Styles.label);

                    totalInsanceCount += runtimeData.instanceCount;
                    totalDrawCallCount += drawCallCount;
                    totalShadowDrawCall += shadowDrawCallCount;
                }

                GUILayout.Label("\nTotal Instance Count: " + String.Format("{0:n0}", totalInsanceCount) +
                    "\n\n" + "Total Geometry Draw Call Count: " + totalDrawCallCount +
                    "\n" + "Total Shadow Draw Call Count: " + totalShadowDrawCall + " (" + QualitySettings.shadowCascades + " Cascades)" +
                    "\n\n" + "Total Draw Call Count: " + (totalDrawCallCount + totalShadowDrawCall)
                    , GPUInstancerEditorConstants.Styles.boldLabel);
            }
            else
                GPUInstancerEditorConstants.DrawCustomLabel("No registered prefabs.", GPUInstancerEditorConstants.Styles.label, false);
        }

        public virtual void DrawGPUInstancerPrototypesBox()
        {
            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);

            EditorGUILayout.BeginHorizontal();
            Rect foldoutRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
            foldoutRect.x += 12;
            showPrototypesBox = EditorGUI.Foldout(foldoutRect, showPrototypesBox, GPUInstancerEditorConstants.TEXT_prototypes, true, GPUInstancerEditorConstants.Styles.foldout);


            Rect switchRect = GUILayoutUtility.GetRect(80, 20, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            GPUInstancerEditorConstants.DrawColoredButton(new GUIContent(_manager.isPrototypeTextMode ? "Icon Mode" : "Text Mode"), GPUInstancerEditorConstants.Colors.green, Color.white,
                FontStyle.Bold, switchRect, () => _manager.isPrototypeTextMode = !_manager.isPrototypeTextMode);
            EditorGUILayout.EndHorizontal();

            if (showPrototypesBox)
            {
                int prototypeRowCount = Mathf.FloorToInt((EditorGUIUtility.currentViewWidth - 30f) / (_manager.isPrototypeTextMode ? PROTOTYPE_TEXT_RECT_SIZE_X : PROTOTYPE_RECT_SIZE));
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prototypes);

                DrawPrototypeBoxButtons();

                if (prototypeContents == null || prototypeContents.Length != _manager.prototypeList.Count)
                    GeneratePrototypeContents();

                int i = 0;
                EditorGUILayout.BeginHorizontal();
                foreach (GPUInstancerPrototype prototype in _manager.prototypeList)
                {
                    if (prototype == null)
                        continue;
                    if (i != 0 && i % prototypeRowCount == 0)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }

                    DrawGPUInstancerPrototypeButton(prototype, prototypeContents[i]);
                    i++;
                }

                if (i != 0 && i % prototypeRowCount == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                if (!Application.isPlaying)
                {
                    if (_manager.isPrototypeTextMode)
                    {
                        DrawGPUInstancerPrototypeAddButtonTextMode();
                        i++;
                        if (i != 0 && i % prototypeRowCount == 0)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                        DrawGPUInstancerPrototypeAddMultiButtonTextMode();
                    }
                    else
                        DrawGPUInstancerPrototypeAddButton();
                }

                EditorGUILayout.EndHorizontal();
                DrawAddPrototypeHelpText();

                GPUInstancerEditorConstants.DrawCustomLabel("<size=10><i>*Ctrl+Clict to select multiple, Shift+Click to select adjacent items.</i></size>",
                    GPUInstancerEditorConstants.Styles.richLabel, false);

                DrawGPUInstancerPrototypeBox(_manager.selectedPrototypeList, _manager.isFrustumCulling, _manager.isOcclusionCulling);
            }
            EditorGUILayout.EndVertical();
        }

        public virtual void DrawPrototypeBoxButtons()
        {
        }

        public virtual void DrawAddPrototypeHelpText()
        {
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_addprototypedetail);
        }

        public void DrawGPUInstancerPrototypeButton(GPUInstancerPrototype prototype, GUIContent prototypeContent)
        {
            base.DrawGPUInstancerPrototypeButton(prototype, prototypeContent, _manager.selectedPrototypeList.Contains(prototype), () =>
            {
                if (_manager.selectedPrototypeList.Count == 1 && Event.current.shift)
                {
                    int oldIndex = prototypeList.IndexOf(_manager.selectedPrototypeList[0]);
                    int newIndex = prototypeList.IndexOf(prototype);
                    for (int i = (oldIndex < newIndex ? oldIndex : newIndex); i <= (oldIndex < newIndex ? newIndex : oldIndex); i++)
                    {
                        if (!_manager.selectedPrototypeList.Contains(prototypeList[i]))
                            _manager.selectedPrototypeList.Add(prototypeList[i]);
                    }
                }
                else if (Event.current.control)
                {
                    if (_manager.selectedPrototypeList.Contains(prototype))
                        _manager.selectedPrototypeList.Remove(prototype);
                    else
                        _manager.selectedPrototypeList.Add(prototype);
                }
                else
                {
                    _manager.selectedPrototypeList.Clear();
                    _manager.selectedPrototypeList.Add(prototype);
                }

                UpdatePrototypeSelection();
                GUI.FocusControl(prototypeContent.tooltip);
            }, _manager.isPrototypeTextMode);
        }

        public void DrawGPUInstancerPrototypeAddButton()
        {
            Rect prototypeRect = GUILayoutUtility.GetRect(PROTOTYPE_RECT_SIZE, PROTOTYPE_RECT_SIZE, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

            Rect iconRect = new Rect(prototypeRect.position + PROTOTYPE_RECT_PADDING_VECTOR, PROTOTYPE_RECT_SIZE_VECTOR);
            iconRect.height -= 22;

            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.add, GPUInstancerEditorConstants.Colors.lightBlue, Color.white, FontStyle.Bold, iconRect,
                () =>
                {
                    _pickerOverride = null;
                    pickerControlID = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
                    ShowObjectPicker();
                },
                true, true,
                (o) =>
                {
                    AddPickerObject(o);
                });

            iconRect.y += iconRect.height;
            iconRect.height = 22;

            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.addMulti, GPUInstancerEditorConstants.Colors.darkBlue, Color.white, FontStyle.Bold, iconRect,
                () =>
                {
                    GPUInstancerMultiAddWindow.ShowWindow(GUIUtility.GUIToScreenPoint(iconRect.position), this);
                },
                true, true,
                (o) =>
                {
                    AddPickerObject(o);
                });
        }

        public void DrawGPUInstancerPrototypeAddButtonTextMode()
        {
            Rect prototypeRect = GUILayoutUtility.GetRect(PROTOTYPE_TEXT_RECT_SIZE_X, PROTOTYPE_TEXT_RECT_SIZE_Y, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

            Rect iconRect = new Rect(prototypeRect.position + PROTOTYPE_RECT_PADDING_VECTOR, PROTOTYPE_TEXT_RECT_SIZE_VECTOR);

            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.addTextMode, GPUInstancerEditorConstants.Colors.lightBlue, Color.white, FontStyle.Bold, iconRect,
                () =>
                {
                    _pickerOverride = null;
                    pickerControlID = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
                    ShowObjectPicker();
                },
                true, true,
                (o) =>
                {
                    AddPickerObject(o);
                });
        }

        public void DrawGPUInstancerPrototypeAddMultiButtonTextMode()
        {
            Rect prototypeRect = GUILayoutUtility.GetRect(PROTOTYPE_TEXT_RECT_SIZE_X, PROTOTYPE_TEXT_RECT_SIZE_Y, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

            Rect iconRect = new Rect(prototypeRect.position + PROTOTYPE_RECT_PADDING_VECTOR, PROTOTYPE_TEXT_RECT_SIZE_VECTOR);

            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.addMultiTextMode, GPUInstancerEditorConstants.Colors.darkBlue, Color.white, FontStyle.Bold, iconRect,
                () =>
                {
                    GPUInstancerMultiAddWindow.ShowWindow(GUIUtility.GUIToScreenPoint(iconRect.position), this);
                },
                true, true,
                (o) =>
                {
                    AddPickerObject(o);
                });
        }

        public virtual void DrawDeleteButton()
        {
            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.delete, Color.red, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    string selectedPrototypesText = "";
                    foreach (GPUInstancerPrototype prototype in _manager.selectedPrototypeList)
                    {
                        selectedPrototypesText += "\n\"" + prototype.ToString() + "\"";
                    }
                    if (EditorUtility.DisplayDialog(GPUInstancerEditorConstants.TEXT_deleteConfirmation, GPUInstancerEditorConstants.TEXT_deleteAreYouSure + selectedPrototypesText, GPUInstancerEditorConstants.TEXT_delete, GPUInstancerEditorConstants.TEXT_cancel))
                    {
                        foreach (GPUInstancerPrototype prototype in _manager.selectedPrototypeList)
                        {
                            _manager.DeletePrototype(prototype);
                        }
                        _manager.selectedPrototypeList.Clear();
                    }
                });
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_delete);
        }

        public GPUInstancerManager GetManager()
        {
            return _manager;
        }
    }
}
