using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GPUInstancer
{
    [CustomEditor(typeof(GPUInstancerPrefabManager))]
    [CanEditMultipleObjects]
    public class GPUInstancerPrefabManagerEditor : GPUInstancerManagerEditor
    {
        private GPUInstancerPrefabManager _prefabManager;

        protected SerializedProperty prop_enableMROnManagerDisable;

        protected override void OnEnable()
        {
            base.OnEnable();

            wikiHash = "#The_Prefab_Manager";

            _prefabManager = (target as GPUInstancerPrefabManager);
            
            prop_enableMROnManagerDisable = serializedObject.FindProperty("enableMROnManagerDisable");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            //if (!Application.isPlaying && _prefabManager.gpuiSimulator != null && _prefabManager.gpuiSimulator.simulateAtEditor)
            //    _prefabManager.gpuiSimulator.StopSimulation();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            DrawSceneSettingsBox();

            DrawPrefabGlobalInfoBox();

            DrawRegisteredPrefabsBox();
            foreach (GPUInstancerPrefabPrototype prototype in _prefabManager.prototypeList)
            {
                if (prototype == null)
                    continue;

                CheckPrefabRigidbodies(prototype);
            }

            DrawGPUInstancerPrototypesBox();

            HandlePickerObjectSelection();

            serializedObject.ApplyModifiedProperties();

            base.InspectorGUIEnd();
        }

        public override void ShowObjectPicker()
        {
            base.ShowObjectPicker();

            EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:prefab", pickerControlID);
        }

        public override void AddPickerObject(UnityEngine.Object pickerObject, GPUInstancerPrototype overridePrototype = null)
        {
            base.AddPickerObject(pickerObject, overridePrototype);

            AddPickerObject(_prefabManager, pickerObject, overridePrototype);
        }

        public static GPUInstancerPrefabPrototype AddPickerObject(GPUInstancerPrefabManager _prefabManager, UnityEngine.Object pickerObject, GPUInstancerPrototype overridePrototype = null)
        {
            if (pickerObject == null)
                return null;

            if (pickerObject is GPUInstancerPrefabPrototype)
            {
                GPUInstancerPrefabPrototype prefabPrototype = (GPUInstancerPrefabPrototype)pickerObject;
                if (prefabPrototype.prefabObject != null)
                {
                    pickerObject = prefabPrototype.prefabObject;
                }
            }

            if (!(pickerObject is GameObject))
            {
#if UNITY_2018_3_OR_NEWER
                if (PrefabUtility.GetPrefabAssetType(pickerObject) == PrefabAssetType.Model)
#else
                if (PrefabUtility.GetPrefabType(pickerObject) == PrefabType.ModelPrefab)
#endif
                    EditorUtility.DisplayDialog(GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_TITLE, GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_3D, GPUInstancerConstants.TEXT_OK);
                else
                    EditorUtility.DisplayDialog(GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_TITLE, GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING, GPUInstancerConstants.TEXT_OK);
                return null;
            }

            GameObject prefabObject = (GameObject)pickerObject;

#if UNITY_2018_3_OR_NEWER
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(pickerObject);

            if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
            {
                if (PrefabUtility.IsPartOfNonAssetPrefabInstance(prefabObject))
                    prefabObject = GPUInstancerUtility.GetOutermostPrefabAssetRoot(prefabObject);
            }
            else
            {
                if (prefabType == PrefabAssetType.Model)
                    EditorUtility.DisplayDialog(GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_TITLE, GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_3D, GPUInstancerConstants.TEXT_OK);
                else
                    EditorUtility.DisplayDialog(GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_TITLE, GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING, GPUInstancerConstants.TEXT_OK);
                return null;
            }
            if (prefabType == PrefabAssetType.Variant)
            {
                if (prefabObject.GetComponent<GPUInstancerPrefab>() == null &&
                    !EditorUtility.DisplayDialog("Variant Prefab Warning",
                        "Prefab is a Variant. Do you wish to add the Variant as a prototype or the corresponding Prefab?" +
                        "\n\nIt is recommended to add the Prefab, if you do not have different renderers for the Variant.",
                        "Add Variant",
                        "Add Prefab"))
                {
                    prefabObject = GPUInstancerUtility.GetCorrespongingPrefabOfVariant(prefabObject);
                }
            }
#else
            PrefabType prefabType = PrefabUtility.GetPrefabType(pickerObject);

            if (prefabType != PrefabType.Prefab)
            {
                bool instanceFound = false;
                if (prefabType == PrefabType.PrefabInstance)
                {
#if UNITY_2018_2_OR_NEWER
                    GameObject newPrefabObject = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(prefabObject);
#else
                    GameObject newPrefabObject = (GameObject)PrefabUtility.GetPrefabParent(prefabObject);
#endif
                    if (PrefabUtility.GetPrefabType(newPrefabObject) == PrefabType.Prefab)
                    {
                        while (newPrefabObject.transform.parent != null)
                            newPrefabObject = newPrefabObject.transform.parent.gameObject;
                        prefabObject = newPrefabObject;
                        instanceFound = true;
                    }
                }
                if (!instanceFound)
                {
                    if (prefabType == PrefabType.ModelPrefab)
                        EditorUtility.DisplayDialog(GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_TITLE, GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_3D, GPUInstancerConstants.TEXT_OK);
                    else
                        EditorUtility.DisplayDialog(GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_TITLE, GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING, GPUInstancerConstants.TEXT_OK);
                    return null;
                }
            }
#endif

            if (_prefabManager.prefabList.Contains(prefabObject))
            {
                return prefabObject.GetComponent<GPUInstancerPrefab>().prefabPrototype;
            }

            GPUInstancerPrefab prefabScript = prefabObject.GetComponent<GPUInstancerPrefab>();
            if(prefabScript != null && prefabScript.prefabPrototype != null && prefabScript.prefabPrototype.prefabObject != prefabObject)
            {
#if UNITY_2018_3_OR_NEWER
                GPUInstancerUtility.RemoveComponentFromPrefab<GPUInstancerPrefab>(prefabObject);
#else
                DestroyImmediate(prefabScript, true);
#endif
                prefabScript = null;
            }

            if (prefabScript == null)
            {
#if UNITY_2018_3_OR_NEWER
                prefabScript = GPUInstancerUtility.AddComponentToPrefab<GPUInstancerPrefab>(prefabObject);
#else
                prefabScript = prefabObject.AddComponent<GPUInstancerPrefab>();
#endif
            }
            if (prefabScript == null)
                return null;

            if (prefabScript.prefabPrototype != null && _prefabManager.prototypeList.Contains(prefabScript.prefabPrototype))
            {
                return prefabScript.prefabPrototype;
            }

            Undo.RecordObject(_prefabManager, "Add prototype");

            if (!_prefabManager.prefabList.Contains(prefabObject))
            {
                _prefabManager.prefabList.Add(prefabObject);
                _prefabManager.GeneratePrototypes();
            }

            if (prefabScript.prefabPrototype != null)
            {
                if (_prefabManager.registeredPrefabs == null)
                    _prefabManager.registeredPrefabs = new List<RegisteredPrefabsData>();

                RegisteredPrefabsData data = _prefabManager.registeredPrefabs.Find(d => d.prefabPrototype == prefabScript.prefabPrototype);
                if (data == null)
                {
                    data = new RegisteredPrefabsData(prefabScript.prefabPrototype);
                    _prefabManager.registeredPrefabs.Add(data);
                }

                GPUInstancerPrefab[] scenePrefabInstances = FindObjectsOfType<GPUInstancerPrefab>();
                foreach (GPUInstancerPrefab prefabInstance in scenePrefabInstances)
                    if (prefabInstance.prefabPrototype == prefabScript.prefabPrototype)
                        data.registeredPrefabs.Add(prefabInstance);
            }
            return prefabScript.prefabPrototype;
        }

        public override void DrawSettingContents()
        {
            EditorGUILayout.Space();

            DrawCameraDataFields();

            DrawCullingSettings(_prefabManager.prototypeList);

            DrawFloatingOriginFields();

            DrawLayerMaskFields();
        }

        public override void DrawLayerMaskFields()
        {
            base.DrawLayerMaskFields();
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.PropertyField(prop_enableMROnManagerDisable, GPUInstancerEditorConstants.Contents.enableMROnManagerDisable);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_enableMROnManagerDisable);
            EditorGUI.EndDisabledGroup();
        }

        public void DrawPrefabGlobalInfoBox()
        {
            //if (_prefabManager.prefabList == null)
            //    return;

            //EditorGUI.BeginDisabledGroup(Application.isPlaying);
            //EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            //GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_prefabGlobal, GPUInstancerEditorConstants.Styles.boldLabel);

            //EditorGUILayout.EndVertical();
            //EditorGUI.EndDisabledGroup();
        }

        public static void SetRenderersEnabled(GPUInstancerPrefabPrototype prefabPrototype, bool enabled)
        {
#if UNITY_2018_3_OR_NEWER
            GameObject prefabContents = GPUInstancerUtility.LoadPrefabContents(prefabPrototype.prefabObject);
#else
            GameObject prefabContents = prefabPrototype.prefabObject;
#endif
            MeshRenderer[] meshRenderers = prefabContents.GetComponentsInChildren<MeshRenderer>(true);
            if (meshRenderers != null && meshRenderers.Length > 0)
                for (int mr = 0; mr < meshRenderers.Length; mr++)
                    meshRenderers[mr].enabled = enabled;

            BillboardRenderer[] billboardRenderers = prefabContents.GetComponentsInChildren<BillboardRenderer>(true);
            if (billboardRenderers != null && billboardRenderers.Length > 0)
                for (int mr = 0; mr < billboardRenderers.Length; mr++)
                    billboardRenderers[mr].enabled = enabled;

            LODGroup lodGroup = prefabContents.GetComponent<LODGroup>();
            if (lodGroup != null)
                lodGroup.enabled = enabled;

            if (prefabPrototype.hasRigidBody)
            {
                Rigidbody rigidbody = prefabContents.GetComponent<Rigidbody>();

                if (enabled || prefabPrototype.autoUpdateTransformData)
                {
                    if (rigidbody == null)
                    {
                        GPUInstancerPrefabPrototype.RigidbodyData rigidbodyData = prefabPrototype.rigidbodyData;
                        if (rigidbodyData != null)
                        {
                            rigidbody = prefabPrototype.prefabObject.AddComponent<Rigidbody>();
                            rigidbody.useGravity = rigidbodyData.useGravity;
                            rigidbody.angularDrag = rigidbodyData.angularDrag;
                            rigidbody.mass = rigidbodyData.mass;
                            rigidbody.constraints = rigidbodyData.constraints;
                            rigidbody.detectCollisions = true;
                            rigidbody.drag = rigidbodyData.drag;
                            rigidbody.isKinematic = rigidbodyData.isKinematic;
                            rigidbody.interpolation = rigidbodyData.interpolation;
                        }
                    }
                }
                else if (rigidbody != null && !prefabPrototype.autoUpdateTransformData)
                    DestroyImmediate(rigidbody, true);
            }

#if UNITY_2018_3_OR_NEWER
            GPUInstancerUtility.UnloadPrefabContents(prefabPrototype.prefabObject, prefabContents, true);
#endif
            EditorUtility.SetDirty(prefabPrototype.prefabObject);
            prefabPrototype.meshRenderersDisabled = !enabled;
            EditorUtility.SetDirty(prefabPrototype);
        }

#if UNITY_2018_1_OR_NEWER
        public void SerializeTransforms(GPUInstancerPrefabPrototype prefabPrototype)
        {
            if (_prefabManager == null || _prefabManager.registeredPrefabs == null)
                return;
            RegisteredPrefabsData registeredPrefabsData = _prefabManager.registeredPrefabs.Find(rpd => rpd.prefabPrototype == prefabPrototype);
            if (registeredPrefabsData == null || registeredPrefabsData.registeredPrefabs == null || registeredPrefabsData.registeredPrefabs.Count == 0)
                return;
            string transformString = "";
            foreach (GPUInstancerPrefab prefabInstance in registeredPrefabsData.registeredPrefabs)
            {
                transformString += GPUInstancerUtility.Matrix4x4ToString(prefabInstance.transform.localToWorldMatrix) + "\n";
            }
            transformString = transformString.Substring(0, transformString.Length - 2);
            TextAsset textAsset = new TextAsset(transformString);

            string assetPath = GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_SERIALIZED_PATH + prefabPrototype.name + ".asset";
            if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_SERIALIZED_PATH))
            {
                System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_SERIALIZED_PATH);
            }
            AssetDatabase.CreateAsset(textAsset, assetPath);
            AssetDatabase.SaveAssets();
            prefabPrototype.isTransformsSerialized = true;
            prefabPrototype.serializedTransformData = textAsset;
            prefabPrototype.serializedTransformDataCount = registeredPrefabsData.registeredPrefabs.Count;

            foreach (GPUInstancerPrefab prefabInstance in registeredPrefabsData.registeredPrefabs)
            {
                DestroyImmediate(prefabInstance.gameObject);
            }
            registeredPrefabsData.registeredPrefabs.Clear();
        }

        public void DeserializeTransforms(GPUInstancerPrefabPrototype prefabPrototype)
        {
            if (_prefabManager == null || _prefabManager.registeredPrefabs == null)
                return;
            RegisteredPrefabsData registeredPrefabsData = _prefabManager.registeredPrefabs.Find(rpd => rpd.prefabPrototype == prefabPrototype);
            if (registeredPrefabsData == null || registeredPrefabsData.registeredPrefabs == null)
                return;

            string matrixStr;
            System.IO.StringReader strReader = new System.IO.StringReader(prefabPrototype.serializedTransformData.text);
            Matrix4x4 matrix;
            GameObject parent = new GameObject(prefabPrototype.prefabObject.name + " Instances");
            parent.transform.position = Vector3.zero;
            parent.transform.rotation = Quaternion.identity;
            parent.transform.localScale = Vector3.one;
            while (true)
            {
                matrixStr = strReader.ReadLine();
                if (!string.IsNullOrEmpty(matrixStr))
                {
                    matrix = GPUInstancerUtility.Matrix4x4FromString(matrixStr);
                    GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabPrototype.prefabObject);
                    prefabInstance.transform.SetMatrix4x4ToTransform(matrix);
                    registeredPrefabsData.registeredPrefabs.Add(prefabInstance.GetComponent<GPUInstancerPrefab>());
                    prefabInstance.transform.parent = parent.transform;
                }
                else
                    break;
            }

            DestroyImmediate(prefabPrototype.serializedTransformData, true);

            prefabPrototype.isTransformsSerialized = false;
            prefabPrototype.serializedTransformData = null;
            prefabPrototype.serializedTransformDataCount = 0;

            AssetDatabase.Refresh();
        }


        public void SerializeAdditionalTransforms()
        {
            if (_prefabManager == null || _prefabManager.registeredPrefabs == null)
                return;

            foreach(GPUInstancerPrefabPrototype prefabPrototype in _prefabManager.prototypeList)
            {
                if (!prefabPrototype.isTransformsSerialized)
                    continue;

                RegisteredPrefabsData registeredPrefabsData = _prefabManager.registeredPrefabs.Find(rpd => rpd.prefabPrototype == prefabPrototype);
                if (registeredPrefabsData == null || registeredPrefabsData.registeredPrefabs == null || registeredPrefabsData.registeredPrefabs.Count == 0)
                    continue;
                string transformString = prefabPrototype.serializedTransformData.text + "\n";
                foreach (GPUInstancerPrefab prefabInstance in registeredPrefabsData.registeredPrefabs)
                {
                    transformString += GPUInstancerUtility.Matrix4x4ToString(prefabInstance.transform.localToWorldMatrix) + "\n";
                }
                transformString = transformString.Substring(0, transformString.Length - 2);
                TextAsset textAsset = new TextAsset(transformString);

                string assetPath = GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_SERIALIZED_PATH + prefabPrototype.name + ".asset";
                if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_SERIALIZED_PATH))
                {
                    System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_SERIALIZED_PATH);
                }
                AssetDatabase.CreateAsset(textAsset, assetPath);
                AssetDatabase.SaveAssets();
                prefabPrototype.isTransformsSerialized = true;
                prefabPrototype.serializedTransformData = textAsset;
                prefabPrototype.serializedTransformDataCount += registeredPrefabsData.registeredPrefabs.Count;

                foreach (GPUInstancerPrefab prefabInstance in registeredPrefabsData.registeredPrefabs)
                {
                    DestroyImmediate(prefabInstance.gameObject);
                }
                registeredPrefabsData.registeredPrefabs.Clear();
            }
        }
#endif

        public override void DrawAddPrototypeHelpText()
        {
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_addprototypeprefab);
        }

        public static void CheckPrefabRigidbodies(GPUInstancerPrefabPrototype prototype)
        {
            if (prototype.prefabObject != null && !prototype.meshRenderersDisabled)
            {
                EditorGUI.BeginChangeCheck();
                Rigidbody rigidbody = prototype.prefabObject.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    prototype.hasRigidBody = true;
                    if (prototype.rigidbodyData == null)
                        prototype.rigidbodyData = new GPUInstancerPrefabPrototype.RigidbodyData();
                    prototype.rigidbodyData.useGravity = rigidbody.useGravity;
                    prototype.rigidbodyData.angularDrag = rigidbody.angularDrag;
                    prototype.rigidbodyData.mass = rigidbody.mass;
                    prototype.rigidbodyData.constraints = rigidbody.constraints;
                    prototype.rigidbodyData.drag = rigidbody.drag;
                    prototype.rigidbodyData.isKinematic = rigidbody.isKinematic;
                    prototype.rigidbodyData.interpolation = rigidbody.interpolation;
                }
                else
                {
                    prototype.hasRigidBody = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(prototype);
                }
            }
        }

        public override void DrawRegisteredPrefabsBoxButtons()
        {
            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.registerPrefabsInScene, GPUInstancerEditorConstants.Colors.darkBlue, Color.white, FontStyle.Bold, Rect.zero,
                    () =>
                    {
                        Undo.RecordObject(_prefabManager, "Register prefabs in scene");
                        _prefabManager.RegisterPrefabsInScene();
#if UNITY_2018_1_OR_NEWER
                        SerializeAdditionalTransforms();
#endif
                    });
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_registerPrefabsInScene);

            if (!GPUInstancerConstants.gpuiSettings.disableInstanceCountWarning)
            {
                bool hasLowInstanceCounts = false;
                if (!Application.isPlaying && _prefabManager.registeredPrefabs.Count > 0)
                {
                    foreach (RegisteredPrefabsData rpd in _prefabManager.registeredPrefabs)
                    {
                        int count = rpd.prefabPrototype.isTransformsSerialized ? rpd.prefabPrototype.serializedTransformDataCount : rpd.registeredPrefabs.Count;
                        if (count > 0 && count < 10)
                            hasLowInstanceCounts = true;
                    }
                }

                if (hasLowInstanceCounts)
                {
                    GUILayout.Space(5);
                    EditorGUILayout.HelpBox(GPUInstancerEditorConstants.WARNINGTEXT_instanceCounts, MessageType.Warning);
                    DrawWikiButton(GUILayoutUtility.GetRect(40, 20), "GPU_Instancer:BestPractices", "#Instance_Counts", "More Information on Instance Counts", GPUInstancerEditorConstants.Colors.darkred);
                    GUILayout.Space(5);
                }
            }
        }

        public override void DrawRegisteredPrefabsBoxList()
        {
            if (!Application.isPlaying && _prefabManager.registeredPrefabs.Count > 0)
            {
                Color defaultColor = GPUInstancerEditorConstants.Styles.label.normal.textColor;
                foreach (RegisteredPrefabsData rpd in _prefabManager.registeredPrefabs)
                {
                    int count = rpd.prefabPrototype.isTransformsSerialized ? rpd.prefabPrototype.serializedTransformDataCount : rpd.registeredPrefabs.Count;
                    if (count > 0 && count < 10)
                        GPUInstancerEditorConstants.Styles.label.normal.textColor = Color.red;
                    else
                        GPUInstancerEditorConstants.Styles.label.normal.textColor = defaultColor;
                    prototypeSelection[rpd.prefabPrototype] = EditorGUILayout.ToggleLeft(rpd.prefabPrototype.ToString() + " Instance Count: " +
                        count, prototypeSelection[rpd.prefabPrototype], GPUInstancerEditorConstants.Styles.label);
                }

                GPUInstancerEditorConstants.Styles.label.normal.textColor = defaultColor;
            }
            else
            {
                base.DrawRegisteredPrefabsBoxList();
            }
        }

        public override bool DrawGPUInstancerPrototypeInfo(List<GPUInstancerPrototype> selectedPrototypeList)
        {
            return DrawGPUInstancerPrototypeInfo(selectedPrototypeList, (string t) => { DrawHelpText(t); });
        }

        public static bool DrawGPUInstancerPrototypeInfo(List<GPUInstancerPrototype> selectedPrototypeList, UnityAction<string> DrawHelpText)
        {
            GPUInstancerPrefabPrototype prototype0 = (GPUInstancerPrefabPrototype)selectedPrototypeList[0];
            #region Determine Multiple Values
            bool hasChanged = false;
            bool enableRuntimeModificationsMixed = false;
            bool enableRuntimeModifications = prototype0.enableRuntimeModifications;
            bool hasRigidBodyMixed = false;
            bool hasRigidBody = prototype0.hasRigidBody;
            bool autoUpdateTransformDataMixed = false;
            bool autoUpdateTransformData = prototype0.autoUpdateTransformData;
            bool startWithRigidBodyMixed = false;
            bool startWithRigidBody = prototype0.startWithRigidBody;
            bool addRemoveInstancesAtRuntimeMixed = false;
            bool addRemoveInstancesAtRuntime = prototype0.addRemoveInstancesAtRuntime;
            bool extraBufferSizeMixed = false;
            int extraBufferSize = prototype0.extraBufferSize;
            bool addRuntimeHandlerScriptMixed = false;
            bool addRuntimeHandlerScript = prototype0.addRuntimeHandlerScript;
            bool meshRenderersDisabledMixed = false;
            bool meshRenderersDisabled = prototype0.meshRenderersDisabled;
            for (int i = 1; i < selectedPrototypeList.Count; i++)
            {
                GPUInstancerPrefabPrototype prototypeI = (GPUInstancerPrefabPrototype)selectedPrototypeList[i];
                if (!enableRuntimeModificationsMixed && enableRuntimeModifications != prototypeI.enableRuntimeModifications)
                    enableRuntimeModificationsMixed = true;
                if (!hasRigidBodyMixed && hasRigidBody != prototypeI.hasRigidBody)
                    hasRigidBodyMixed = true;
                if (!autoUpdateTransformDataMixed && autoUpdateTransformData != prototypeI.autoUpdateTransformData)
                    autoUpdateTransformDataMixed = true;
                if (!startWithRigidBodyMixed && startWithRigidBody != prototypeI.startWithRigidBody)
                    startWithRigidBodyMixed = true;
                if (!addRemoveInstancesAtRuntimeMixed && addRemoveInstancesAtRuntime != prototypeI.addRemoveInstancesAtRuntime)
                    addRemoveInstancesAtRuntimeMixed = true;
                if (!extraBufferSizeMixed && extraBufferSize != prototypeI.extraBufferSize)
                    extraBufferSizeMixed = true;
                if (!addRuntimeHandlerScriptMixed && addRuntimeHandlerScript != prototypeI.addRuntimeHandlerScript)
                    addRuntimeHandlerScriptMixed = true;
                if (!meshRenderersDisabledMixed && meshRenderersDisabled != prototypeI.meshRenderersDisabled)
                    meshRenderersDisabledMixed = true;
            }
            #endregion Determine Multiple Values

            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_prefabRuntimeSettings, GPUInstancerEditorConstants.Styles.boldLabel);

            hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_enableRuntimeModifications, enableRuntimeModifications, enableRuntimeModificationsMixed, (p, v) => ((GPUInstancerPrefabPrototype)p).enableRuntimeModifications = v);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_enableRuntimeModifications);

            EditorGUI.BeginDisabledGroup(enableRuntimeModificationsMixed || !enableRuntimeModifications);

            EditorGUI.BeginDisabledGroup(hasRigidBodyMixed || !hasRigidBody || autoUpdateTransformDataMixed || autoUpdateTransformData);
            hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_startWithRigidBody, startWithRigidBody, startWithRigidBodyMixed, (p, v) => ((GPUInstancerPrefabPrototype)p).startWithRigidBody = v);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_startWithRigidBody);
            EditorGUI.EndDisabledGroup();

            hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_addRemoveInstancesAtRuntime, addRemoveInstancesAtRuntime, addRemoveInstancesAtRuntimeMixed, (p, v) => ((GPUInstancerPrefabPrototype)p).addRemoveInstancesAtRuntime = v);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_addRemoveInstancesAtRuntime);

            EditorGUI.BeginDisabledGroup(addRemoveInstancesAtRuntimeMixed || !addRemoveInstancesAtRuntime);
            hasChanged |= MultiIntSlider(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_extraBufferSize, extraBufferSize, 0, GPUInstancerConstants.gpuiSettings.MAX_PREFAB_EXTRA_BUFFER_SIZE, extraBufferSizeMixed, (p, v) => ((GPUInstancerPrefabPrototype)p).extraBufferSize = v);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_extraBufferSize);

            hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_addRuntimeHandlerScript, addRuntimeHandlerScript, addRuntimeHandlerScriptMixed, (p, v) => ((GPUInstancerPrefabPrototype)p).addRuntimeHandlerScript = v);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_addRuntimeHandlerScript);

            if (!addRemoveInstancesAtRuntimeMixed && addRemoveInstancesAtRuntime && !addRuntimeHandlerScriptMixed && !Application.isPlaying)
            {
                if (addRuntimeHandlerScript)
                {
                    foreach (GPUInstancerPrefabPrototype prefabPrototype in selectedPrototypeList)
                    {
                        GPUInstancerPrefabRuntimeHandler prefabRuntimeHandler = prefabPrototype.prefabObject.GetComponent<GPUInstancerPrefabRuntimeHandler>();
                        if (prefabRuntimeHandler == null)
                        {
#if UNITY_2018_3_OR_NEWER
                            GPUInstancerUtility.AddComponentToPrefab<GPUInstancerPrefabRuntimeHandler>(prefabPrototype.prefabObject);
#else
                            prefabPrototype.prefabObject.AddComponent<GPUInstancerPrefabRuntimeHandler>();
#endif
                            EditorUtility.SetDirty(prefabPrototype.prefabObject);
                        }
                    }
                }
                else
                {
                    foreach (GPUInstancerPrefabPrototype prefabPrototype in selectedPrototypeList)
                    {
                        GPUInstancerPrefabRuntimeHandler prefabRuntimeHandler = prefabPrototype.prefabObject.GetComponent<GPUInstancerPrefabRuntimeHandler>();
                        if(prefabRuntimeHandler != null)
                        {
#if UNITY_2018_3_OR_NEWER
                            GPUInstancerUtility.RemoveComponentFromPrefab<GPUInstancerPrefabRuntimeHandler>(prefabPrototype.prefabObject);
#else
                            DestroyImmediate(prefabRuntimeHandler, true);
#endif
                        }
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            if(!meshRenderersDisabledMixed && !meshRenderersDisabled)
            {
                hasChanged |= MultiToggle(selectedPrototypeList, GPUInstancerEditorConstants.TEXT_autoUpdateTransformData, autoUpdateTransformData, autoUpdateTransformDataMixed, (p, v) => ((GPUInstancerPrefabPrototype)p).autoUpdateTransformData = v);
                DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_autoUpdateTransformData);
            }
            EditorGUI.EndDisabledGroup();

            if (!enableRuntimeModificationsMixed && !enableRuntimeModifications)
            {
                foreach (GPUInstancerPrefabPrototype prefabPrototype in selectedPrototypeList)
                {
                    if (prefabPrototype.addRemoveInstancesAtRuntime)
                        prefabPrototype.addRemoveInstancesAtRuntime = false;
                    if (prefabPrototype.startWithRigidBody)
                        prefabPrototype.startWithRigidBody = false;
                    if (prefabPrototype.autoUpdateTransformData)
                    {
                        prefabPrototype.autoUpdateTransformData = false;
                        if (prefabPrototype.meshRenderersDisabled)
                            SetRenderersEnabled(prefabPrototype, !prefabPrototype.meshRenderersDisabled);
                    }
                }
            }

            foreach (GPUInstancerPrefabPrototype prefabPrototype in selectedPrototypeList)
            {
                if ((!prefabPrototype.enableRuntimeModifications || !prefabPrototype.addRemoveInstancesAtRuntime) && prefabPrototype.extraBufferSize > 0)
                    prefabPrototype.extraBufferSize = 0;

                if ((!prefabPrototype.enableRuntimeModifications || !prefabPrototype.addRemoveInstancesAtRuntime) && prefabPrototype.addRuntimeHandlerScript)
                {
                    prefabPrototype.addRuntimeHandlerScript = false;
                    GPUInstancerPrefabRuntimeHandler prefabRuntimeHandler = prefabPrototype.prefabObject.GetComponent<GPUInstancerPrefabRuntimeHandler>();
                    if (prefabRuntimeHandler != null)
                    {
#if UNITY_2018_3_OR_NEWER
                        GPUInstancerUtility.RemoveComponentFromPrefab<GPUInstancerPrefabRuntimeHandler>(prefabPrototype.prefabObject);
#else
                        DestroyImmediate(prefabRuntimeHandler, true);
#endif
                        EditorUtility.SetDirty(prefabPrototype.prefabObject);
                    }
                }
            }

            EditorGUILayout.EndVertical();

            return hasChanged;
        }

        public override void DrawGPUInstancerPrototypeInfo(GPUInstancerPrototype selectedPrototype)
        {
            DrawGPUInstancerPrototypeInfo(selectedPrototype, (string t) => { DrawHelpText(t); });
        }

        public static void DrawGPUInstancerPrototypeInfo(GPUInstancerPrototype selectedPrototype, UnityAction<string> DrawHelpText)
        {
            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_prefabRuntimeSettings, GPUInstancerEditorConstants.Styles.boldLabel);

            GPUInstancerPrefabPrototype prototype = (GPUInstancerPrefabPrototype)selectedPrototype;
            prototype.enableRuntimeModifications = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_enableRuntimeModifications, prototype.enableRuntimeModifications);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_enableRuntimeModifications);

            EditorGUI.BeginDisabledGroup(!prototype.enableRuntimeModifications);

            EditorGUI.BeginDisabledGroup(!prototype.hasRigidBody || prototype.autoUpdateTransformData);
            prototype.startWithRigidBody = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_startWithRigidBody, prototype.startWithRigidBody);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_startWithRigidBody);
            EditorGUI.EndDisabledGroup();

            prototype.addRemoveInstancesAtRuntime = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_addRemoveInstancesAtRuntime, prototype.addRemoveInstancesAtRuntime);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_addRemoveInstancesAtRuntime);

            EditorGUI.BeginDisabledGroup(!prototype.addRemoveInstancesAtRuntime);
            prototype.extraBufferSize = EditorGUILayout.IntSlider(GPUInstancerEditorConstants.TEXT_extraBufferSize, prototype.extraBufferSize, 0, GPUInstancerConstants.gpuiSettings.MAX_PREFAB_EXTRA_BUFFER_SIZE);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_extraBufferSize);

            prototype.addRuntimeHandlerScript = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_addRuntimeHandlerScript, prototype.addRuntimeHandlerScript);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_addRuntimeHandlerScript);

            if (prototype.addRemoveInstancesAtRuntime && !Application.isPlaying)
            {
                GPUInstancerPrefabRuntimeHandler prefabRuntimeHandler = prototype.prefabObject.GetComponent<GPUInstancerPrefabRuntimeHandler>();
                if (prototype.addRuntimeHandlerScript && prefabRuntimeHandler == null)
                {
#if UNITY_2018_3_OR_NEWER
                    GPUInstancerUtility.AddComponentToPrefab<GPUInstancerPrefabRuntimeHandler>(prototype.prefabObject);
#else
                    prototype.prefabObject.AddComponent<GPUInstancerPrefabRuntimeHandler>();
#endif
                    EditorUtility.SetDirty(prototype.prefabObject);
                }
                else if (!prototype.addRuntimeHandlerScript && prefabRuntimeHandler != null)
                {
#if UNITY_2018_3_OR_NEWER
                    GPUInstancerUtility.RemoveComponentFromPrefab<GPUInstancerPrefabRuntimeHandler>(prototype.prefabObject);
#else
                    DestroyImmediate(prefabRuntimeHandler, true);
#endif
                    EditorUtility.SetDirty(prototype.prefabObject);
                }
            }
            EditorGUI.EndDisabledGroup();

            bool autoUpdateTransformData = prototype.autoUpdateTransformData;
            prototype.autoUpdateTransformData = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_autoUpdateTransformData, prototype.autoUpdateTransformData);
            DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_autoUpdateTransformData);
            if (autoUpdateTransformData != prototype.autoUpdateTransformData && prototype.meshRenderersDisabled)
                SetRenderersEnabled(prototype, !prototype.meshRenderersDisabled);
            EditorGUI.EndDisabledGroup();

            if (!prototype.enableRuntimeModifications)
            {
                if (prototype.addRemoveInstancesAtRuntime)
                    prototype.addRemoveInstancesAtRuntime = false;
                if (prototype.startWithRigidBody)
                    prototype.startWithRigidBody = false;
                if (prototype.autoUpdateTransformData)
                {
                    prototype.autoUpdateTransformData = false;
                    if (prototype.meshRenderersDisabled)
                        SetRenderersEnabled(prototype, !prototype.meshRenderersDisabled);
                }
            }

            if ((!prototype.enableRuntimeModifications || !prototype.addRemoveInstancesAtRuntime) && prototype.extraBufferSize > 0)
                prototype.extraBufferSize = 0;

            if ((!prototype.enableRuntimeModifications || !prototype.addRemoveInstancesAtRuntime) && prototype.addRuntimeHandlerScript)
            {
                prototype.addRuntimeHandlerScript = false;
                GPUInstancerPrefabRuntimeHandler prefabRuntimeHandler = prototype.prefabObject.GetComponent<GPUInstancerPrefabRuntimeHandler>();
                if (prefabRuntimeHandler != null)
                {
#if UNITY_2018_3_OR_NEWER
                        GPUInstancerUtility.RemoveComponentFromPrefab<GPUInstancerPrefabRuntimeHandler>(prototype.prefabObject);
#else
                    DestroyImmediate(prefabRuntimeHandler, true);
#endif
                    EditorUtility.SetDirty(prototype.prefabObject);
                }
            }

            EditorGUILayout.EndVertical();
        }

        public override void DrawGPUInstancerPrototypeActions()
        {
            if (Application.isPlaying)
                return;

            GUILayout.Space(10);

            GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_actions, GPUInstancerEditorConstants.Styles.boldLabel, false);

            DrawDeleteButton();
        }

        public override void DrawGPUInstancerPrototypeAdvancedActions()
        {
            if (Application.isPlaying)
                return;

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();
            // title
            Rect foldoutRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
            foldoutRect.x += 12;
            showAdvancedBox = EditorGUI.Foldout(foldoutRect, showAdvancedBox, GPUInstancerEditorConstants.TEXT_advancedActions, true, GPUInstancerEditorConstants.Styles.foldout);

            //GUILayout.Space(10);

            if (showAdvancedBox)
            {
                bool showSimulator = false;
                foreach (GPUInstancerPrefabPrototype prototype in _prefabManager.prototypeList)
                {
                    if (prototype == null)
                        continue;

                    if (prototype.isTransformsSerialized || prototype.meshRenderersDisabled)
                        showSimulator = true;
                }

                EditorGUILayout.HelpBox(GPUInstancerEditorConstants.HELPTEXT_advancedActions, MessageType.Warning);

                if (_prefabManager.selectedPrototypeList != null && _prefabManager.selectedPrototypeList.Count > 0)
                {
                    GPUInstancerPrefabPrototype prototype0 = (GPUInstancerPrefabPrototype)_prefabManager.selectedPrototypeList[0];

                    foreach (GPUInstancerPrefabPrototype prefabPrototype in _prefabManager.selectedPrototypeList)
                    {
                        if (prefabPrototype.isTransformsSerialized && prefabPrototype.serializedTransformData == null)
                        {
                            prefabPrototype.isTransformsSerialized = false;
                        }
                    }

                    #region Determine Multiple Values
                    bool meshRenderersDisabledMixed = false;
                    bool meshRenderersDisabled = prototype0.meshRenderersDisabled;
                    //bool meshRenderersDisabledSimulationMixed = false;
                    //bool meshRenderersDisabledSimulation = prototype0.meshRenderersDisabledSimulation;
                    bool isTransformsSerializedMixed = false;
                    bool isTransformsSerialized = prototype0.isTransformsSerialized;
                    for (int i = 1; i < _prefabManager.selectedPrototypeList.Count; i++)
                    {
                        GPUInstancerPrefabPrototype prototypeI = (GPUInstancerPrefabPrototype)_prefabManager.selectedPrototypeList[i];
                        if (!meshRenderersDisabledMixed && meshRenderersDisabled != prototypeI.meshRenderersDisabled)
                            meshRenderersDisabledMixed = true;
                        if (!isTransformsSerializedMixed && isTransformsSerialized != prototypeI.isTransformsSerialized)
                            isTransformsSerializedMixed = true;
                    }
                    #endregion Determine Multiple Values

                    if (!meshRenderersDisabledMixed && meshRenderersDisabled)
                    {
                        GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.enableMeshRenderers, GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                            () =>
                            {
                                foreach (GPUInstancerPrefabPrototype prefabPrototype in _prefabManager.selectedPrototypeList)
                                {
                                    SetRenderersEnabled(prefabPrototype, true);
                                }
                            });
                        //MultiToggle(_prefabManager.selectedPrototypeList, GPUInstancerEditorConstants.TEXT_disableMeshRenderersSimulation, meshRenderersDisabledSimulation, meshRenderersDisabledSimulationMixed, (p, v) => ((GPUInstancerPrefabPrototype)p).meshRenderersDisabledSimulation = v);
                    }
                    else if (!meshRenderersDisabledMixed && !meshRenderersDisabled)
                    {
                        GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.disableMeshRenderers, GPUInstancerEditorConstants.Colors.lightBlue, Color.white, FontStyle.Bold, Rect.zero,
                        () =>
                        {
                            if (EditorUtility.DisplayDialog(GPUInstancerEditorConstants.TEXT_disableMeshRenderers, GPUInstancerEditorConstants.TEXT_disableMeshRenderersAreYouSure, "Yes", "No"))
                            {
                                foreach (GPUInstancerPrefabPrototype prefabPrototype in _prefabManager.selectedPrototypeList)
                                {
                                    SetRenderersEnabled(prefabPrototype, false);
                                }
                            }
                        });
                    }
                    DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_disableMeshRenderers);

#if UNITY_2018_1_OR_NEWER
                    if (!isTransformsSerializedMixed && isTransformsSerialized)
                    {
                        GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.deserializeRegisteredInstances, GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                                () =>
                                {
                                    Undo.RecordObject(_prefabManager, "DeserializeTransforms");
                                    EditorUtility.DisplayProgressBar(GPUInstancerEditorConstants.TEXT_prefabInstanceSerialization, "Deserializing instances...", 0.1f);
                                    try
                                    {
                                        foreach (GPUInstancerPrefabPrototype prefabPrototype in _prefabManager.selectedPrototypeList)
                                        {
                                            DeserializeTransforms(prefabPrototype);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.LogError(e);
                                    }
                                    EditorUtility.ClearProgressBar();
                                });
                    }
                    else if (!isTransformsSerializedMixed && !isTransformsSerialized)
                    {
                        GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.serializeRegisteredInstances, GPUInstancerEditorConstants.Colors.lightBlue, Color.white, FontStyle.Bold, Rect.zero,
                                () =>
                                {
                                    if (EditorUtility.DisplayDialog(GPUInstancerEditorConstants.TEXT_prefabInstanceSerialization, GPUInstancerEditorConstants.TEXT_prefabInstanceSerializationAreYouSure, GPUInstancerEditorConstants.TEXT_prefabInstanceSerializationYes, "No"))
                                    {
                                        Undo.RecordObject(_prefabManager, "SerializeTransforms");
                                        EditorUtility.DisplayProgressBar(GPUInstancerEditorConstants.TEXT_prefabInstanceSerialization, "Serializing instances... This might take a while.", 0.1f);
                                        try
                                        {
                                            foreach (GPUInstancerPrefabPrototype prefabPrototype in _prefabManager.selectedPrototypeList)
                                            {
                                                SerializeTransforms(prefabPrototype);
                                            }
                                        }
                                        catch(Exception e)
                                        {
                                            Debug.LogError(e);
                                        }
                                        EditorUtility.ClearProgressBar();
                                    }
                                });
                    }
                    DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_prefabInstanceSerialization);
#endif

                    if (showSimulator)
                    {
                        if (_prefabManager.gpuiSimulator != null)
                        {
                            if (_prefabManager.gpuiSimulator.simulateAtEditor)
                            {
                                if (_prefabManager.gpuiSimulator.initializingInstances)
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
                                            _prefabManager.gpuiSimulator.StopSimulation();
                                        });
                                }
                            }
                            else
                            {
                                GPUInstancerEditorConstants.DrawColoredButton(new GUIContent("Simulate at Scene Camera [Experimental]"), GPUInstancerEditorConstants.Colors.green, Color.white,
                                    FontStyle.Bold, Rect.zero, () =>
                                    {
                                        _prefabManager.gpuiSimulator.StartSimulation();
                                    });
                            }
                        }
                        DrawHelpText(GPUInstancerEditorConstants.HELPTEXT_simulator);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        public override float GetMaxDistance(GPUInstancerPrototype selectedPrototype)
        {
            return GPUInstancerConstants.gpuiSettings.MAX_PREFAB_DISTANCE;
        }
    }
}