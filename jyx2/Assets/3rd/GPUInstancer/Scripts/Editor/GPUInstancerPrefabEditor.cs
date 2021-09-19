using UnityEditor;
using UnityEngine;

namespace GPUInstancer
{
    [CustomEditor(typeof(GPUInstancerPrefab)), CanEditMultipleObjects]
    public class GPUInstancerPrefabEditor : Editor
    {
        private GPUInstancerPrefab[] _prefabScripts;
        
        protected void OnEnable()
        {
            Object[] monoObjects = targets;
            _prefabScripts = new GPUInstancerPrefab[monoObjects.Length];
            for (int i = 0; i < monoObjects.Length; i++)
            {
                _prefabScripts[i] = monoObjects[i] as GPUInstancerPrefab;
            }
        }

        public override void OnInspectorGUI()
        {
            if(_prefabScripts != null)
            {
                    
                if (_prefabScripts.Length >= 1 && _prefabScripts[0] != null && _prefabScripts[0].prefabPrototype != null)
                {
                    bool isPrefab = _prefabScripts[0].prefabPrototype.prefabObject == _prefabScripts[0].gameObject;

                    if (_prefabScripts.Length == 1)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.ObjectField(GPUInstancerEditorConstants.TEXT_prototypeSO, _prefabScripts[0].prefabPrototype, typeof(GPUInstancerPrefabPrototype), false);
                        EditorGUI.EndDisabledGroup();

                        if (!isPrefab)
                        {
                            if (Application.isPlaying)
                            {
                                if (_prefabScripts[0].state == PrefabInstancingState.Instanced)
                                    GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_prefabInstancingActive + _prefabScripts[0].gpuInstancerID, GPUInstancerEditorConstants.Styles.boldLabel);
                                else if (_prefabScripts[0].state == PrefabInstancingState.Disabled)
                                    GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_prefabInstancingDisabled + _prefabScripts[0].gpuInstancerID, GPUInstancerEditorConstants.Styles.boldLabel);
                                else
                                    GPUInstancerEditorConstants.DrawCustomLabel(GPUInstancerEditorConstants.TEXT_prefabInstancingNone, GPUInstancerEditorConstants.Styles.boldLabel);
                            }
                        }
                    }

                    if (isPrefab && !Application.isPlaying)
                    {
                        foreach (GPUInstancerPrefab prefabScript in _prefabScripts)
                        {
                            if (prefabScript != null && prefabScript.prefabPrototype != null)
                            {
                                GPUInstancerPrefabManagerEditor.CheckPrefabRigidbodies(prefabScript.prefabPrototype);
                            }
                        }

                        EditorGUILayout.BeginHorizontal();
                        if (_prefabScripts[0].prefabPrototype.meshRenderersDisabled)
                        {
                            GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.enableMeshRenderers, GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                                () =>
                                {
                                    foreach (GPUInstancerPrefab prefabScript in _prefabScripts)
                                    {
                                        if (prefabScript != null && prefabScript.prefabPrototype != null)
                                        {
                                            GPUInstancerPrefabManagerEditor.SetRenderersEnabled(prefabScript.prefabPrototype, true);
                                        }
                                    }
                                });
                            //_prefabScripts[0].prefabPrototype.meshRenderersDisabledSimulation = EditorGUILayout.Toggle(GPUInstancerEditorConstants.TEXT_disableMeshRenderersSimulation, _prefabScripts[0].prefabPrototype.meshRenderersDisabledSimulation);
                        }
                        //if (!_prefabScripts[0].prefabPrototype.meshRenderersDisabled)
                        //{
                        //    GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.disableMeshRenderers, Color.red, Color.white, FontStyle.Bold, Rect.zero,
                        //    () =>
                        //    {
                        //        if (EditorUtility.DisplayDialog(GPUInstancerEditorConstants.TEXT_disableMeshRenderers, GPUInstancerEditorConstants.TEXT_disableMeshRenderersAreYouSure, "Yes", "No"))
                        //        {
                        //            foreach (GPUInstancerPrefab prefabScript in _prefabScripts)
                        //            {
                        //                if (prefabScript != null && prefabScript.prefabPrototype != null)
                        //                {
                        //                    GPUInstancerPrefabManagerEditor.SetRenderersEnabled(prefabScript.prefabPrototype, false);
                        //                }
                        //            }
                        //        }
                        //    });
                        //}
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        //[DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.InSelectionHierarchy)]
        //private static void DrawGizmo(GPUInstancerPrefab instance, GizmoType gizmoType)
        //{
        //    if (EditorApplication.isPlaying || !instance.enabled || instance.prefabPrototype == null || !instance.prefabPrototype.meshRenderersDisabled)
        //        return;

        //    if (instance.prefabPrototype.meshRenderersDisabledSimulation)
        //    {
        //        if (instance.GetComponent<LODGroup>())
        //        {
        //            foreach (Renderer r in instance.GetComponent<LODGroup>().GetLODs()[0].renderers)
        //            {
        //                if(r is MeshRenderer)
        //                {
        //                    DrawMesh((MeshRenderer)r);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();
        //            for (int i = 0; i != renderers.Length; ++i)
        //            {
        //                DrawMesh(renderers[i]);
        //            }
        //        }
        //    }
        //}

        //private static void DrawMesh(MeshRenderer renderer)
        //{
        //    Matrix4x4 matrix = renderer.transform.localToWorldMatrix;
        //    MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
        //    Material mat;
        //    for (int m = 0; m < renderer.sharedMaterials.Length; m++)
        //    {
        //        mat = renderer.sharedMaterials[m];
        //        //Graphics.DrawMesh(meshFilter.sharedMesh, matrix, renderer.sharedMaterials[m], renderer.gameObject.layer, null, m);
        //        for (int p = 0; p < mat.passCount; p++)
        //        {
        //            if(mat.GetShaderPassEnabled(mat.GetPassName(p)))
        //            {
        //                renderer.sharedMaterials[m].SetPass(p);
        //                Graphics.DrawMeshNow(meshFilter.sharedMesh, matrix, m);
        //            }
        //        }
        //    }
        //}
    }
}