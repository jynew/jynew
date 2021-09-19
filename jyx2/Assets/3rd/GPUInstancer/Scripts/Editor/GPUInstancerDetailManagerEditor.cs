using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GPUInstancer
{
    [CustomEditor(typeof(GPUInstancerDetailManager))]
    [CanEditMultipleObjects]
    public class GPUInstancerDetailManagerEditor : GPUInstancerManagerEditor
    {
        protected SerializedProperty prop_runInThreads;

        private GPUInstancerDetailManager _detailManager;

        protected override void OnEnable()
        {
            base.OnEnable();

            wikiHash = "#The_Detail_Manager";

            prop_runInThreads = serializedObject.FindProperty("runInThreads");

            _detailManager = (target as GPUInstancerDetailManager);
            if (!Application.isPlaying && _detailManager.gpuiSimulator == null)
                _detailManager.gpuiSimulator = new GPUInstancerEditorSimulator(_detailManager);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (!Application.isPlaying && _detailManager.gpuiSimulator != null && _detailManager.gpuiSimulator.simulateAtEditor && !_detailManager.keepSimulationLive)
                _detailManager.gpuiSimulator.StopSimulation();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            if (_detailManager.terrain == null)
            {
                if (!Application.isPlaying && Event.current.type == EventType.ExecuteCommand && pickerControlID > 0 && Event.current.commandName == "ObjectSelectorClosed")
                {
                    if (EditorGUIUtility.GetObjectPickerControlID() == pickerControlID)
                        AddTerrainPickerObject(EditorGUIUtility.GetObjectPickerObject());
                    pickerControlID = -1;
                }

                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                DrawDetailTerrainAddButton();
                EditorGUI.EndDisabledGroup();
                return;
            }
            else if (_detailManager.terrainSettings == null)
                _detailManager.SetupManagerWithTerrain(_detailManager.terrain);

            DrawSceneSettingsBox();

            if (_detailManager.terrainSettings != null)
            {
                DrawDebugBox(_detailManager.gpuiSimulator);

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

            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Texture"), false, () => { EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, "", pickerControlID); });
            menu.AddItem(new GUIContent("Prefab (Grass)"), false, () => { pickerMode = 0; EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:prefab", pickerControlID); });
            menu.AddItem(new GUIContent("Prefab (Other)"), false, () => { pickerMode = 1; EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:prefab", pickerControlID); });

            // display the menu
            menu.ShowAsContext();
        }

        public override void AddPickerObject(UnityEngine.Object pickerObject, GPUInstancerPrototype overridePrototype = null)
        {
            base.AddPickerObject(pickerObject, overridePrototype);

            if (pickerObject == null)
                return;

            Undo.RecordObject(this, "Add prototype");

            if (_detailManager.terrainSettings != null && _detailManager.terrain != null && _detailManager.terrain.terrainData != null)
            {
                List<DetailPrototype> newDetailPrototypes = new List<DetailPrototype>(_detailManager.terrain.terrainData.detailPrototypes);

                if (pickerObject is Texture2D)
                {
                    if (overridePrototype != null)
                    {
                        int prototypeIndex = prototypeList.IndexOf(overridePrototype);
                        if (prototypeIndex >= 0 && prototypeIndex < _detailManager.terrain.terrainData.detailPrototypes.Length)
                        {
                            DetailPrototype[] detailPrototypes = _detailManager.terrain.terrainData.detailPrototypes;
                            detailPrototypes[prototypeIndex].prototype = null;
                            detailPrototypes[prototypeIndex].prototypeTexture = (Texture2D)pickerObject;
                            detailPrototypes[prototypeIndex].renderMode = DetailRenderMode.GrassBillboard;
                            detailPrototypes[prototypeIndex].usePrototypeMesh = false;
                            overridePrototype.prefabObject = null;
                            ((GPUInstancerDetailPrototype)overridePrototype).prototypeTexture = (Texture2D)pickerObject;
                            ((GPUInstancerDetailPrototype)overridePrototype).detailRenderMode = DetailRenderMode.GrassBillboard;
                            ((GPUInstancerDetailPrototype)overridePrototype).usePrototypeMesh = false;
                            _detailManager.terrain.terrainData.detailPrototypes = detailPrototypes;
                            _detailManager.terrain.terrainData.RefreshPrototypes();
                        }
                    }
                    else
                    {
                        newDetailPrototypes.Add(new DetailPrototype()
                        {
                            usePrototypeMesh = false,
                            prototypeTexture = (Texture2D)pickerObject,
                            renderMode = DetailRenderMode.GrassBillboard
                        });

                        _detailManager.terrain.terrainData.detailPrototypes = newDetailPrototypes.ToArray();
                        _detailManager.terrain.terrainData.RefreshPrototypes();
                        _detailManager.GeneratePrototypes();
                    }
                }
                else if (pickerObject is GameObject)
                {
                    if (((GameObject)pickerObject).GetComponentInChildren<MeshRenderer>() == null)
                        return;

                    // Determine terrainDetailPrototype color to get a similar look on Unity Terrain
                    Color dryColor = Color.white;
                    Color healthyColor = Color.white;

                    MeshRenderer pickerObjectRenderer = ((GameObject)pickerObject).GetComponentInChildren<MeshRenderer>();

                    if (pickerObjectRenderer == null || pickerObjectRenderer.sharedMaterial == null)
                    {
                        Debug.LogError("Cannot add prefab to the Detail Manager: Mesh Renderer does not have any materials");
                        return;
                    }


                    if (pickerObjectRenderer.sharedMaterial.shader.name == GPUInstancerConstants.SHADER_GPUI_FOLIAGE ||
                        pickerObjectRenderer.sharedMaterial.shader.name == GPUInstancerConstants.SHADER_GPUI_FOLIAGE_LWRP)
                    {
                        dryColor = pickerObjectRenderer.sharedMaterial.GetColor("_DryColor");
                        healthyColor = pickerObjectRenderer.sharedMaterial.GetColor("_HealthyColor");
                    }
                    else
                    {
                        if (pickerObjectRenderer.sharedMaterial.HasProperty("_Color"))
                        {
                            healthyColor = pickerObjectRenderer.sharedMaterial.color;
                            dryColor = pickerObjectRenderer.sharedMaterial.color;
                        }
                    }

                    if (overridePrototype != null)
                    {
                        int prototypeIndex = prototypeList.IndexOf(overridePrototype);
                        if (prototypeIndex >= 0 && prototypeIndex < _detailManager.terrain.terrainData.detailPrototypes.Length)
                        {
                            DetailPrototype[] detailPrototypes = _detailManager.terrain.terrainData.detailPrototypes;
                            detailPrototypes[prototypeIndex].prototype = (GameObject)pickerObject;
                            detailPrototypes[prototypeIndex].prototypeTexture = null;
                            detailPrototypes[prototypeIndex].renderMode = pickerMode == 0 ? DetailRenderMode.Grass : DetailRenderMode.VertexLit;
                            detailPrototypes[prototypeIndex].usePrototypeMesh = true;
                            overridePrototype.prefabObject = (GameObject)pickerObject;
                            ((GPUInstancerDetailPrototype)overridePrototype).prototypeTexture = null;
                            ((GPUInstancerDetailPrototype)overridePrototype).detailRenderMode = pickerMode == 0 ? DetailRenderMode.Grass : DetailRenderMode.VertexLit;
                            ((GPUInstancerDetailPrototype)overridePrototype).usePrototypeMesh = true;
                            _detailManager.terrain.terrainData.detailPrototypes = detailPrototypes;
                            _detailManager.terrain.terrainData.RefreshPrototypes();
                        }
                    }
                    else
                    {
                        DetailPrototype terrainDetailPrototype = new DetailPrototype()
                        {
                            usePrototypeMesh = true,
                            prototype = ((GameObject)pickerObject).GetComponentInChildren<MeshRenderer>().gameObject,
                            renderMode = pickerMode == 0 ? DetailRenderMode.Grass : DetailRenderMode.VertexLit,
                            healthyColor = healthyColor,
                            dryColor = dryColor
                        };
                        newDetailPrototypes.Add(terrainDetailPrototype);

                        _detailManager.terrain.terrainData.detailPrototypes = newDetailPrototypes.ToArray();
                        _detailManager.terrain.terrainData.RefreshPrototypes();
                        GPUInstancerUtility.AddDetailInstancePrototypeFromTerrainPrototype(_detailManager.gameObject, prototypeList, terrainDetailPrototype, newDetailPrototypes.Count - 1, 1,
                            _detailManager.terrainSettings, (GameObject)pickerObject);
                    }
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
                    _detailManager.SetupManagerWithTerrain(go.GetComponent<Terrain>());
                }
            }
        }

        public override void ApplyEditorDataChanges()
        {
            base.ApplyEditorDataChanges();

            if (_detailManager.terrain.terrainData.detailPrototypes.Length != prototypeList.Count)
                return;

            // set detail prototypes
            DetailPrototype[] detailPrototypes = _detailManager.terrain.terrainData.detailPrototypes;
            for (int i = 0; i < prototypeList.Count; i++)
            {
                GPUInstancerDetailPrototype prototype = (GPUInstancerDetailPrototype)prototypeList[i];
                GameObject prefab = null;
                if (prototype.prefabObject != null)
                {
                    prefab = ((GameObject)prototype.prefabObject).GetComponentInChildren<MeshRenderer>().gameObject;
                }

                DetailPrototype dp = detailPrototypes[i];

                dp.renderMode = prototype.detailRenderMode;
                dp.usePrototypeMesh = prototype.usePrototypeMesh;
                dp.prototype = prefab;
                dp.prototypeTexture = prototype.prototypeTexture;
                dp.noiseSpread = prototype.noiseSpread;
                dp.minWidth = prototype.detailScale.x;
                dp.maxWidth = prototype.detailScale.y;
                dp.minHeight = prototype.detailScale.z;
                dp.maxHeight = prototype.detailScale.w;
                dp.healthyColor = prototype.detailHealthyColor;
                dp.dryColor = prototype.detailDryColor;

                // Update terrainDetailPrototype color form prototype material to get a similar look on Unity Terrain for Mesh type prototypes.
                if (prototype.usePrototypeMesh)
                {
                    if (prototype.prefabObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.shader.name == GPUInstancerConstants.SHADER_GPUI_FOLIAGE ||
                        prototype.prefabObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.shader.name == GPUInstancerConstants.SHADER_GPUI_FOLIAGE_LWRP)
                    {
                        dp.healthyColor = prototype.prefabObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.GetColor("_HealthyColor");
                        dp.dryColor = prototype.prefabObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.GetColor("_DryColor");
                    }
                    else
                    {
                        if (prototype.prefabObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.HasProperty("_Color"))
                        {
                            dp.healthyColor = prototype.prefabObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;
                            dp.dryColor = prototype.prefabObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;
                        }
                    }
                }

                if (prototype.useCustomMaterialForTextureDetail && prototype.textureDetailCustomMaterial != null)
                {
                    if (prototype.prefabObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.HasProperty("_Color"))
                    {
                        dp.healthyColor = prototype.prefabObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;
                        dp.dryColor = prototype.prefabObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;
                    }
                }
            }

            _detailManager.terrain.terrainData.detailPrototypes = detailPrototypes;
            editorDataChanged = false;
        }

        public override void DrawSettingContents()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            //EditorGUILayout.PropertyField(prop_settings);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(GPUInstancerEditorConstants.TEXT_terrain, _detailManager.terrain, typeof(Terrain), true);
            EditorGUI.EndDisabledGroup();


            EditorGUILayout.BeginHorizontal();
            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.paintOnTerrain, GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    if (_detailManager.terrain != null)
                    {
                        GPUInstancerTerrainProxy proxy = _detailManager.AddProxyToTerrain();
                        Selection.activeGameObject = _detailManager.terrain.gameObject;

                        proxy.terrainSelectedToolIndex = 5;
                    }
                });
            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.removeTerrain, Color.red, Color.white, FontStyle.Bold, Rect.zero,
            () =>
            {
                if (EditorUtility.DisplayDialog(GPUInstancerEditorConstants.TEXT_removeTerrainConfirmation, GPUInstancerEditorConstants.TEXT_removeTerrainAreYouSure, GPUInstancerEditorConstants.TEXT_unset, GPUInstancerEditorConstants.TEXT_cancel))
                {
                    _detailManager.SetupManagerWithTerrain(null);
                }
            });
            EditorGUILayout.EndHorizontal();
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_terrain);

            EditorGUILayout.Space();

            EditorGUI.EndDisabledGroup();

            DrawCameraDataFields();

            DrawCullingSettings(_detailManager.prototypeList);

            DrawFloatingOriginFields();

            DrawLayerMaskFields();
        }

        public void DrawDetailTerrainAddButton()
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
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_setTerrain, true);
            GUILayout.Space(10);
        }

        public override void DrawGlobalValuesContents()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(GPUInstancerEditorConstants.TEXT_terrainSettingsSO, _detailManager.terrainSettings, typeof(GPUInstancerTerrainSettings), false);
            EditorGUI.EndDisabledGroup();

            float newMaxDetailDistance = EditorGUILayout.Slider(GPUInstancerEditorConstants.TEXT_maxDetailDistance, _detailManager.terrainSettings.maxDetailDistance, 0,
                GPUInstancerConstants.gpuiSettings.MAX_DETAIL_DISTANCE);
            if (_detailManager.terrainSettings.maxDetailDistance != newMaxDetailDistance)
            {
                foreach (GPUInstancerDetailPrototype p in _detailManager.prototypeList)
                {
                    if (p.maxDistance == _detailManager.terrainSettings.maxDetailDistance || p.maxDistance > newMaxDetailDistance)
                    {
                        p.maxDistance = newMaxDetailDistance;
                        EditorUtility.SetDirty(p);
                    }
                }
                _detailManager.terrainSettings.maxDetailDistance = newMaxDetailDistance;
            }
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_maxDetailDistance);
            EditorGUILayout.Space();

            float newDetailDensity = EditorGUILayout.Slider(GPUInstancerEditorConstants.TEXT_detailDensity, _detailManager.terrainSettings.detailDensity, 0.0f, 1.0f);
            if (_detailManager.terrainSettings.detailDensity != newDetailDensity)
            {
                foreach (GPUInstancerDetailPrototype p in _detailManager.prototypeList)
                {
                    if (p.detailDensity == _detailManager.terrainSettings.detailDensity || p.detailDensity > newDetailDensity)
                    {
                        p.detailDensity = newDetailDensity;
                        EditorUtility.SetDirty(p);
                    }
                }
                _detailManager.terrainSettings.detailDensity = newDetailDensity;
            }
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_detailDensity);
            EditorGUILayout.Space();

            _detailManager.detailLayer = EditorGUILayout.LayerField(GPUInstancerEditorConstants.TEXT_detailLayer, _detailManager.detailLayer);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_detailLayer);
            EditorGUILayout.Space();

            _detailManager.terrainSettings.windVector = EditorGUILayout.Vector2Field(GPUInstancerEditorConstants.TEXT_windVector, _detailManager.terrainSettings.windVector);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windVector);
            EditorGUILayout.Space();

            _detailManager.terrainSettings.healthyDryNoiseTexture = (Texture2D)EditorGUILayout.ObjectField(GPUInstancerEditorConstants.TEXT_healthyDryNoiseTexture, _detailManager.terrainSettings.healthyDryNoiseTexture, typeof(Texture2D), false);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_healthyDryNoiseTexture);
            if (_detailManager.terrainSettings.healthyDryNoiseTexture == null)
                _detailManager.terrainSettings.healthyDryNoiseTexture = Resources.Load<Texture2D>(GPUInstancerConstants.NOISE_TEXTURES_PATH + GPUInstancerConstants.DEFAULT_HEALTHY_DRY_NOISE);

            _detailManager.terrainSettings.windWaveNormalTexture = (Texture2D)EditorGUILayout.ObjectField(GPUInstancerEditorConstants.TEXT_windWaveNormalTexture, _detailManager.terrainSettings.windWaveNormalTexture, typeof(Texture2D), false);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windWaveNormalTexture);
            if (_detailManager.terrainSettings.windWaveNormalTexture == null)
                _detailManager.terrainSettings.windWaveNormalTexture = Resources.Load<Texture2D>(GPUInstancerConstants.NOISE_TEXTURES_PATH + GPUInstancerConstants.DEFAULT_WIND_WAVE_NOISE);

            _detailManager.terrainSettings.autoSPCellSize = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_autoSPCellSize, _detailManager.terrainSettings.autoSPCellSize);
            if (!_detailManager.terrainSettings.autoSPCellSize)
                _detailManager.terrainSettings.preferedSPCellSize = EditorGUILayout.IntSlider(GPUInstancerEditorConstants.TEXT_preferedSPCellSize, _detailManager.terrainSettings.preferedSPCellSize, 25, 500);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_spatialPartitioningCellSize);

            EditorGUILayout.PropertyField(prop_runInThreads, GPUInstancerEditorConstants.Contents.runInThreads);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_runInThreads);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_detailManager, "Editor data changed.");
                OnEditorDataChanged();
                EditorUtility.SetDirty(_detailManager.terrainSettings);
            }

            EditorGUI.EndDisabledGroup();
        }

        public override string GetGlobalValuesTitle()
        {
            return GPUInstancerEditorConstants.TEXT_detailGlobal;
        }

        public override void DrawAddPrototypeHelpText()
        {
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_addprototypedetail);
        }

        public override void DrawPrototypeBoxButtons()
        {
            if (!string.IsNullOrEmpty(_detailManager.terrainSettings.warningText))
                EditorGUILayout.HelpBox(_detailManager.terrainSettings.warningText, MessageType.Error);

            if (!Application.isPlaying)
            {
                GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.generatePrototypes, GPUInstancerEditorConstants.Colors.darkBlue, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    if (EditorUtility.DisplayDialog(GPUInstancerEditorConstants.TEXT_generatePrototypesConfirmation, GPUInstancerEditorConstants.TEXT_generatePrototypeAreYouSure, GPUInstancerEditorConstants.TEXT_generatePrototypes, GPUInstancerEditorConstants.TEXT_cancel))
                    {
                        _detailManager.GeneratePrototypes(true);
                    }
                });
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_generatePrototypesDetail);
            }
        }

        public override bool DrawGPUInstancerPrototypeInfo(List<GPUInstancerPrototype> selectedPrototypeList)
        {
            return DrawGPUInstancerPrototypeInfo(selectedPrototypeList, (string t) => { DrawHelpText(t); }, _detailManager, OnEditorDataChanged,
                _detailManager.gpuiSimulator, _detailManager.terrainSettings, _detailManager.detailLayer);
        }

        public static bool DrawGPUInstancerPrototypeInfo(List<GPUInstancerPrototype> selectedPrototypeList, UnityAction<string> DrawHelpText, UnityEngine.Object component, UnityAction OnEditorDataChanged,
            GPUInstancerEditorSimulator simulator, GPUInstancerTerrainSettings terrainSettings, int detailLayer)
        {
            GPUInstancerDetailPrototype prototype0 = (GPUInstancerDetailPrototype)selectedPrototypeList[0];
            #region Determine Multiple Values
            bool hasChanged = false;
            bool detailDensityMixed = false;
            float detailDensity = prototype0.detailDensity;
            bool detailScaleMixed = false;
            Vector4 detailScale = prototype0.detailScale;
            bool noiseSpreadMixed = false;
            float noiseSpread = prototype0.noiseSpread;
            bool usePrototypeMeshMixed = false;
            bool usePrototypeMesh = prototype0.usePrototypeMesh;
            bool isBillboardMixed = false;
            bool isBillboard = prototype0.isBillboard;
            bool useCrossQuadsMixed = false;
            bool useCrossQuads = prototype0.useCrossQuads;
            bool quadCountMixed = false;
            int quadCount = prototype0.quadCount;
            bool useCustomMaterialForTextureDetailMixed = false;
            bool useCustomMaterialForTextureDetail = prototype0.useCustomMaterialForTextureDetail;
            bool billboardDistanceMixed = false;
            float billboardDistance = prototype0.billboardDistance;
            bool billboardFaceCamPosMixed = false;
            bool billboardFaceCamPos = prototype0.billboardFaceCamPos;
            bool detailHealthyColorMixed = false;
            Color detailHealthyColor = prototype0.detailHealthyColor;
            bool detailDryColorMixed = false;
            Color detailDryColor = prototype0.detailDryColor;
            bool ambientOcclusionMixed = false;
            float ambientOcclusion = prototype0.ambientOcclusion;
            bool gradientPowerMixed = false;
            float gradientPower = prototype0.gradientPower;
            bool windIdleSwayMixed = false;
            float windIdleSway = prototype0.windIdleSway;
            bool windWavesOnMixed = false;
            bool windWavesOn = prototype0.windWavesOn;
            bool windWaveTintColorMixed = false;
            Color windWaveTintColor = prototype0.windWaveTintColor;
            bool windWaveSizeMixed = false;
            float windWaveSize = prototype0.windWaveSize;
            bool windWaveTintMixed = false;
            float windWaveTint = prototype0.windWaveTint;
            bool windWaveSwayMixed = false;
            float windWaveSway = prototype0.windWaveSway;
            for (int i = 1; i < selectedPrototypeList.Count; i++)
            {
                GPUInstancerDetailPrototype prototypeI = (GPUInstancerDetailPrototype)selectedPrototypeList[i];
                if (!detailDensityMixed && detailDensity != prototypeI.detailDensity)
                    detailDensityMixed = true;
                if (!detailScaleMixed && detailScale != prototypeI.detailScale)
                    detailScaleMixed = true;
                if (!noiseSpreadMixed && noiseSpread != prototypeI.noiseSpread)
                    noiseSpreadMixed = true;
                if (!usePrototypeMeshMixed && usePrototypeMesh != prototypeI.usePrototypeMesh)
                    usePrototypeMeshMixed = true;
                if (!isBillboardMixed && isBillboard != prototypeI.isBillboard)
                    isBillboardMixed = true;
                if (!useCrossQuadsMixed && useCrossQuads != prototypeI.useCrossQuads)
                    useCrossQuadsMixed = true;
                if (!quadCountMixed && quadCount != prototypeI.quadCount)
                    quadCountMixed = true;
                if (!useCustomMaterialForTextureDetailMixed && useCustomMaterialForTextureDetail != prototypeI.useCustomMaterialForTextureDetail)
                    useCustomMaterialForTextureDetailMixed = true;
                if (!billboardDistanceMixed && billboardDistance != prototypeI.billboardDistance)
                    billboardDistanceMixed = true;
                if (!billboardFaceCamPosMixed && billboardFaceCamPos != prototypeI.billboardFaceCamPos)
                    billboardFaceCamPosMixed = true;
                if (!detailHealthyColorMixed && detailHealthyColor != prototypeI.detailHealthyColor)
                    detailHealthyColorMixed = true;
                if (!detailDryColorMixed && detailDryColor != prototypeI.detailDryColor)
                    detailDryColorMixed = true;
                if (!ambientOcclusionMixed && ambientOcclusion != prototypeI.ambientOcclusion)
                    ambientOcclusionMixed = true;
                if (!gradientPowerMixed && gradientPower != prototypeI.gradientPower)
                    gradientPowerMixed = true;
                if (!windIdleSwayMixed && windIdleSway != prototypeI.windIdleSway)
                    windIdleSwayMixed = true;
                if (!windWavesOnMixed && windWavesOn != prototypeI.windWavesOn)
                    windWavesOnMixed = true;
                if (!windWaveTintColorMixed && windWaveTintColor != prototypeI.windWaveTintColor)
                    windWaveTintColorMixed = true;
                if (!windWaveSizeMixed && windWaveSize != prototypeI.windWaveSize)
                    windWaveSizeMixed = true;
                if (!windWaveTintMixed && windWaveTint != prototypeI.windWaveTint)
                    windWaveTintMixed = true;
                if (!windWaveSwayMixed && windWaveSway != prototypeI.windWaveSway)
                    windWaveSwayMixed = true;
            }
            #endregion Determine Multiple Values

            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_detailProperties, GPUInstancerEditorConstants.Styles.boldLabel);

            hasChanged |= MultiSlider(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_detailDensity, detailDensity, 0.0f, terrainSettings.detailDensity, detailDensityMixed, (p, v) => ((GPUInstancerDetailPrototype)p).detailDensity = v);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_detailDensity);
            hasChanged |= MultiVector4(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_detailScale, detailScale, detailScaleMixed, (p, v) => ((GPUInstancerDetailPrototype)p).detailScale = v);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_detailScale);

            hasChanged |= MultiFloat(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_noiseSpread, noiseSpread, noiseSpreadMixed, (p, v) => ((GPUInstancerDetailPrototype)p).noiseSpread = v);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_noiseSpread);

            EditorGUILayout.EndVertical();

            if (!usePrototypeMeshMixed && !usePrototypeMesh && !isBillboardMixed && !isBillboard)
            {
                EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
                GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_crossQuads, GPUInstancerEditorConstants.Styles.boldLabel);

                hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_crossQuads, useCrossQuads, useCrossQuadsMixed, (p, v) => ((GPUInstancerDetailPrototype)p).useCrossQuads = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_crossQuads);

                if (!useCrossQuadsMixed && useCrossQuads)
                {
                    hasChanged |= MultiIntSlider(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_quadCount, quadCount, 2, 4, quadCountMixed, (p, v) => ((GPUInstancerDetailPrototype)p).quadCount = v);
                    DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_quadCount);

                    if (!useCustomMaterialForTextureDetailMixed && !useCustomMaterialForTextureDetail)
                    {
                        hasChanged |= MultiSlider(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_billboardDistance, billboardDistance, 0.5f, 1f, billboardDistanceMixed, (p, v) => ((GPUInstancerDetailPrototype)p).billboardDistance = v);
                        DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_billboardDistance);
                        hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_CQBillboardFaceCamPos, billboardFaceCamPos, billboardFaceCamPosMixed, (p, v) => ((GPUInstancerDetailPrototype)p).billboardFaceCamPos = v);
                        DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_CQBillboardFaceCamPos);
                    }
                }
                else if (!useCrossQuadsMixed && !useCrossQuads)
                {
                    for (int i = 1; i < selectedPrototypeList.Count; i++)
                    {
                        ((GPUInstancerDetailPrototype)selectedPrototypeList[i]).quadCount = 1;
                    }
                }

                EditorGUILayout.EndVertical();
            }
            else
            {
                for (int i = 1; i < selectedPrototypeList.Count; i++)
                {
                    GPUInstancerDetailPrototype prototypeI = (GPUInstancerDetailPrototype)selectedPrototypeList[i];
                    if (prototypeI.usePrototypeMesh || prototypeI.isBillboard)
                        prototypeI.useCrossQuads = false;
                }
            }

            if (!usePrototypeMeshMixed && !usePrototypeMesh && !useCustomMaterialForTextureDetailMixed && !useCustomMaterialForTextureDetail)
            {
                EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
                GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_foliageShaderProperties, GPUInstancerEditorConstants.Styles.boldLabel);

                EditorGUI.BeginChangeCheck();
                hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_isBillboard, isBillboard, isBillboardMixed, (p, v) => ((GPUInstancerDetailPrototype)p).isBillboard = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_isBillboard);

                if (!isBillboardMixed && isBillboard)
                {
                    hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_billboardFaceCamPos, billboardFaceCamPos, billboardFaceCamPosMixed, (p, v) => ((GPUInstancerDetailPrototype)p).billboardFaceCamPos = v);
                    DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_billboardFaceCamPos);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    if (simulator != null && simulator.simulateAtEditor && !simulator.initializingInstances)
                    {
                        GPUInstancerUtility.UpdateDetailInstanceRuntimeDataList(simulator.gpuiManager.runtimeDataList, terrainSettings, false, detailLayer);
                    }
                }

                EditorGUI.BeginChangeCheck();

                hasChanged |= MultiColor(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_detailHealthyColor, detailHealthyColor, detailHealthyColorMixed, (p, v) => ((GPUInstancerDetailPrototype)p).detailHealthyColor = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_detailHealthyColor);
                hasChanged |= MultiColor(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_detailDryColor, detailDryColor, detailDryColorMixed, (p, v) => ((GPUInstancerDetailPrototype)p).detailDryColor = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_detailDryColor);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(component, "Editor data changed.");
                    if (OnEditorDataChanged != null)
                        OnEditorDataChanged();
                    if (simulator != null && simulator.simulateAtEditor && !simulator.initializingInstances)
                    {
                        GPUInstancerUtility.UpdateDetailInstanceRuntimeDataList(simulator.gpuiManager.runtimeDataList, terrainSettings, false, detailLayer);
                    }
                }

                EditorGUI.BeginChangeCheck();

                hasChanged |= MultiSlider(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_ambientOcclusion, ambientOcclusion, 0.0f, 1f, ambientOcclusionMixed, (p, v) => ((GPUInstancerDetailPrototype)p).ambientOcclusion = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_ambientOcclusion);
                hasChanged |= MultiSlider(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_gradientPower, gradientPower, 0.0f, 1f, gradientPowerMixed, (p, v) => ((GPUInstancerDetailPrototype)p).gradientPower = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_gradientPower);

                GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_windSettings, GPUInstancerEditorConstants.Styles.boldLabel);

                hasChanged |= MultiSlider(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_windIdleSway, windIdleSway, 0.0f, 1f, windIdleSwayMixed, (p, v) => ((GPUInstancerDetailPrototype)p).windIdleSway = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windIdleSway);
                hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_windWavesOn, windWavesOn, windWavesOnMixed, (p, v) => ((GPUInstancerDetailPrototype)p).windWavesOn = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windWavesOn);
                EditorGUI.BeginDisabledGroup(windWavesOnMixed || !windWavesOn);
                hasChanged |= MultiColor(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_windWaveTintColor, windWaveTintColor, windWaveTintColorMixed, (p, v) => ((GPUInstancerDetailPrototype)p).windWaveTintColor = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windWaveTintColor);
                hasChanged |= MultiSlider(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_windWaveSize, windWaveSize, 0.0f, 1f, windWaveSizeMixed, (p, v) => ((GPUInstancerDetailPrototype)p).windWaveSize = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windWaveSize);
                hasChanged |= MultiSlider(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_windWaveTint, windWaveTint, 0.0f, 1f, windWaveTintMixed, (p, v) => ((GPUInstancerDetailPrototype)p).windWaveTint = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windWaveTint);
                hasChanged |= MultiSlider(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_windWaveSway, windWaveSway, 0.0f, 1f, windWaveSwayMixed, (p, v) => ((GPUInstancerDetailPrototype)p).windWaveSway = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windWaveSway);
                EditorGUI.EndDisabledGroup();
                if (EditorGUI.EndChangeCheck())
                {
                    if (simulator != null && simulator.simulateAtEditor && !simulator.initializingInstances)
                    {
                        GPUInstancerUtility.UpdateDetailInstanceRuntimeDataList(simulator.gpuiManager.runtimeDataList, terrainSettings, false, detailLayer);
                    }
                }
                EditorGUILayout.EndVertical();
            }

            return hasChanged;
        }

        public override void DrawGPUInstancerPrototypeInfo(GPUInstancerPrototype selectedPrototype)
        {
            DrawGPUInstancerPrototypeInfo(selectedPrototype, (string t) => { DrawHelpText(t); }, _detailManager, OnEditorDataChanged,
                _detailManager.gpuiSimulator, _detailManager.terrainSettings, _detailManager.detailLayer);
        }

        public static void DrawGPUInstancerPrototypeInfo(GPUInstancerPrototype selectedPrototype, UnityAction<string> DrawHelpText, UnityEngine.Object component, UnityAction OnEditorDataChanged,
            GPUInstancerEditorSimulator simulator, GPUInstancerTerrainSettings terrainSettings, int detailLayer)
        {
            GPUInstancerDetailPrototype prototype = (GPUInstancerDetailPrototype)selectedPrototype;

            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_detailProperties, GPUInstancerEditorConstants.Styles.boldLabel);

            EditorGUI.BeginChangeCheck();

            prototype.detailDensity = EditorGUILayout.Slider(GPUInstancerEditorConstants.TEXT_detailDensity, prototype.detailDensity, 0.0f, terrainSettings.detailDensity);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_detailDensity);
            prototype.detailScale = EditorGUILayout.Vector4Field(GPUInstancerEditorConstants.TEXT_detailScale, prototype.detailScale);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_detailScale);

            prototype.noiseSpread = EditorGUILayout.FloatField(GPUInstancerEditorConstants.TEXT_noiseSpread, prototype.noiseSpread);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_noiseSpread);

            prototype.useCustomHealthyDryNoiseTexture = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_useCustomHealthyDryNoiseTexture, prototype.useCustomHealthyDryNoiseTexture);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_useCustomHealthyDryNoiseTexture);
            if (prototype.useCustomHealthyDryNoiseTexture)
            {
                prototype.healthyDryNoiseTexture = (Texture2D)EditorGUILayout.ObjectField(GPUInstancerEditorConstants.TEXT_healthyDryNoiseTexture, prototype.healthyDryNoiseTexture, typeof(Texture2D), false);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_healthyDryNoiseTexture);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(component, "Editor data changed.");
                if (OnEditorDataChanged != null)
                    OnEditorDataChanged();
                EditorUtility.SetDirty(prototype);
            }

            EditorGUI.BeginChangeCheck();
            if (!prototype.usePrototypeMesh)
            {
                prototype.useCustomMaterialForTextureDetail = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_useCustomMaterialForTextureDetail, prototype.useCustomMaterialForTextureDetail);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_useCustomMaterialForTextureDetail);
                if (prototype.useCustomMaterialForTextureDetail)
                {
                    prototype.textureDetailCustomMaterial = (Material)EditorGUILayout.ObjectField(GPUInstancerEditorConstants.TEXT_textureDetailCustomMaterial, prototype.textureDetailCustomMaterial, typeof(Material), false);
                    DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_textureDetailCustomMaterial);
                    prototype.isBillboard = false;
                }
                else
                {
                    prototype.textureDetailCustomMaterial = null;
                }
            }

            EditorGUILayout.EndVertical();

            if (!prototype.usePrototypeMesh && !prototype.isBillboard)
            {
                EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
                GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_crossQuads, GPUInstancerEditorConstants.Styles.boldLabel);

                prototype.useCrossQuads = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_crossQuads, prototype.useCrossQuads);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_crossQuads);

                if (prototype.useCrossQuads)
                {
                    prototype.quadCount = EditorGUILayout.IntSlider(GPUInstancerEditorConstants.TEXT_quadCount, prototype.quadCount, 2, 4);
                    DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_quadCount);

                    if (!prototype.useCustomMaterialForTextureDetail)
                    {
                        prototype.billboardDistance = EditorGUILayout.Slider(GPUInstancerEditorConstants.TEXT_billboardDistance, prototype.billboardDistance, 0.5f, 1f);
                        DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_billboardDistance);
                        prototype.billboardDistanceDebug = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_billboardDistanceDebug, prototype.billboardDistanceDebug);
                        DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_billboardDistanceDebug);
                        if (prototype.billboardDistanceDebug)
                        {
                            prototype.billboardDistanceDebugColor = EditorGUILayout.ColorField(GPUInstancerEditorConstants.TEXT_billboardDistanceDebugColor, prototype.billboardDistanceDebugColor);
                            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_billboardDistanceDebugColor);
                        }
                        prototype.billboardFaceCamPos = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_CQBillboardFaceCamPos, prototype.billboardFaceCamPos);
                        DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_CQBillboardFaceCamPos);
                    }

                }
                else
                {
                    prototype.quadCount = 1;
                }

                EditorGUILayout.EndVertical();
            }
            else
            {
                prototype.useCrossQuads = false;
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (!prototype.usePrototypeMesh && prototype.useCustomMaterialForTextureDetail && prototype.textureDetailCustomMaterial != null)
                {
                    if (!GPUInstancerConstants.gpuiSettings.shaderBindings.IsShadersInstancedVersionExists(prototype.textureDetailCustomMaterial.shader.name))
                    {
                        Shader instancedShader;
                        if (GPUInstancerUtility.IsShaderInstanced(prototype.textureDetailCustomMaterial.shader))
                            instancedShader = prototype.textureDetailCustomMaterial.shader;
                        else
                            instancedShader = GPUInstancerUtility.CreateInstancedShader(prototype.textureDetailCustomMaterial.shader);

                        if (instancedShader != null)
                            GPUInstancerConstants.gpuiSettings.shaderBindings.AddShaderInstance(prototype.textureDetailCustomMaterial.shader.name, instancedShader);
                        else
                            Debug.LogWarning("Can not create instanced version for shader: " + prototype.textureDetailCustomMaterial.shader.name + ". Standard Shader will be used instead.");
                    }
                }
                EditorUtility.SetDirty(prototype);
            }

            if (!prototype.usePrototypeMesh && !prototype.useCustomMaterialForTextureDetail)
            {
                EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
                GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_foliageShaderProperties, GPUInstancerEditorConstants.Styles.boldLabel);

                EditorGUI.BeginChangeCheck();
                prototype.isBillboard = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_isBillboard, prototype.isBillboard);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_isBillboard);

                if (prototype.isBillboard)
                {
                    prototype.billboardFaceCamPos = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_billboardFaceCamPos, prototype.billboardFaceCamPos);
                    DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_billboardFaceCamPos);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(prototype);
                    if (simulator != null && simulator.simulateAtEditor && !simulator.initializingInstances)
                    {
                        GPUInstancerUtility.UpdateDetailInstanceRuntimeDataList(simulator.gpuiManager.runtimeDataList, terrainSettings, false, detailLayer);
                    }
                }

                EditorGUI.BeginChangeCheck();

                prototype.detailHealthyColor = EditorGUILayout.ColorField(GPUInstancerEditorConstants.TEXT_detailHealthyColor, prototype.detailHealthyColor);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_detailHealthyColor);
                prototype.detailDryColor = EditorGUILayout.ColorField(GPUInstancerEditorConstants.TEXT_detailDryColor, prototype.detailDryColor);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_detailDryColor);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(component, "Editor data changed.");
                    if (OnEditorDataChanged != null)
                        OnEditorDataChanged();
                    if (simulator != null && simulator.simulateAtEditor && !simulator.initializingInstances)
                    {
                        GPUInstancerUtility.UpdateDetailInstanceRuntimeDataList(simulator.gpuiManager.runtimeDataList, terrainSettings, false, detailLayer);
                    }
                    EditorUtility.SetDirty(prototype);
                }

                EditorGUI.BeginChangeCheck();

                prototype.ambientOcclusion = EditorGUILayout.Slider(GPUInstancerEditorConstants.TEXT_ambientOcclusion, prototype.ambientOcclusion, 0f, 1f);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_ambientOcclusion);
                prototype.gradientPower = EditorGUILayout.Slider(GPUInstancerEditorConstants.TEXT_gradientPower, prototype.gradientPower, 0f, 1f);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_gradientPower);

                GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_windSettings, GPUInstancerEditorConstants.Styles.boldLabel);

                prototype.windIdleSway = EditorGUILayout.Slider(GPUInstancerEditorConstants.TEXT_windIdleSway, prototype.windIdleSway, 0f, 1f);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windIdleSway);
                prototype.windWavesOn = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_windWavesOn, prototype.windWavesOn);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windWavesOn);
                EditorGUI.BeginDisabledGroup(!prototype.windWavesOn);
                prototype.windWaveTintColor = EditorGUILayout.ColorField(GPUInstancerEditorConstants.TEXT_windWaveTintColor, prototype.windWaveTintColor);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windWaveTintColor);
                prototype.windWaveSize = EditorGUILayout.Slider(GPUInstancerEditorConstants.TEXT_windWaveSize, prototype.windWaveSize, 0f, 1f);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windWaveSize);
                prototype.windWaveTint = EditorGUILayout.Slider(GPUInstancerEditorConstants.TEXT_windWaveTint, prototype.windWaveTint, 0f, 1f);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windWaveTint);
                prototype.windWaveSway = EditorGUILayout.Slider(GPUInstancerEditorConstants.TEXT_windWaveSway, prototype.windWaveSway, 0f, 1f);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_windWaveSway);
                EditorGUI.EndDisabledGroup();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(prototype);
                    if (simulator != null && simulator.simulateAtEditor && !simulator.initializingInstances)
                    {
                        GPUInstancerUtility.UpdateDetailInstanceRuntimeDataList(simulator.gpuiManager.runtimeDataList, terrainSettings, false, detailLayer);
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        public override void DrawGPUInstancerPrototypeActions()
        {
            GUILayout.Space(10);
            GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_actions, GPUInstancerEditorConstants.Styles.boldLabel, false);

            if (!editorDataChanged)
                EditorGUI.BeginDisabledGroup(true);
            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.applyChangesToTerrain, GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    ApplyEditorDataChanges();
                });
            if (!editorDataChanged)
                EditorGUI.EndDisabledGroup();
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_applyChangesToTerrain);

            DrawDeleteButton();
        }

        public override float GetMaxDistance(GPUInstancerPrototype selectedPrototype)
        {
            return _detailManager.terrainSettings != null ? _detailManager.terrainSettings.maxDetailDistance : GPUInstancerConstants.gpuiSettings.MAX_DETAIL_DISTANCE;
        }

        public override void DrawPrefabField(GPUInstancerPrototype selectedPrototype)
        {
            EditorGUILayout.BeginHorizontal();
            if (selectedPrototype.prefabObject != null)
                base.DrawPrefabField(selectedPrototype);
            else
            {
                GPUInstancerDetailPrototype detailPrototype = (GPUInstancerDetailPrototype)selectedPrototype;
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(GPUInstancerEditorConstants.TEXT_prototypeTexture, detailPrototype.prototypeTexture, typeof(GameObject), false);
                EditorGUI.EndDisabledGroup();
            }

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