using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GPUInstancer
{
    [CustomEditor(typeof(GPUInstancerTerrainProxy))]
    public class GPUInstancerTerrainProxyEditor : Editor
    {
        private static Type terrainInspectorType;
        private GPUInstancerTerrainProxy terrainProxy;

        public override void OnInspectorGUI()
        {
            terrainProxy = (GPUInstancerTerrainProxy)target;

            if(terrainProxy.detailManager != null)
            {
                GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.goToGPUInstancerDetail, GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                    () =>
                    {
                        if (terrainProxy.detailManager != null && terrainProxy.detailManager.gameObject != null)
                            Selection.activeGameObject = terrainProxy.detailManager.gameObject;
                    });
            }
            if (terrainProxy.treeManager != null)
            {
                GPUInstancerEditorConstants.DrawColoredButton(GPUInstancerEditorConstants.Contents.goToGPUInstancerTree, GPUInstancerEditorConstants.Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                    () =>
                    {
                        if (terrainProxy.treeManager != null && terrainProxy.treeManager.gameObject != null)
                            Selection.activeGameObject = terrainProxy.treeManager.gameObject;
                    });
            }
            EditorGUILayout.HelpBox(GPUInstancerEditorConstants.HELPTEXT_terrainProxyWarning, MessageType.Warning);

            // select terrain tool
            //if(terrainProxy.terrainSelectedToolIndex > 0)
            //{
            //    try
            //    {
            //        if(terrainInspectorType == null)
            //            terrainInspectorType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.TerrainInspector");
            //        PropertyInfo selectedTool = terrainInspectorType.GetProperty("selectedTool", BindingFlags.NonPublic | BindingFlags.Instance);

            //        UnityEngine.Object[] terrainInspectors = Resources.FindObjectsOfTypeAll(terrainInspectorType);
            //        foreach (UnityEngine.Object terrainInspector in terrainInspectors)
            //        {
            //            //Debug.Log(selectedTool.GetValue(terrainInspector, new object[0]));
            //            selectedTool.SetValue(terrainInspector, terrainProxy.terrainSelectedToolIndex, new object[0]);
            //            break;
            //        }
            //    }
            //    catch (Exception) { };

            //    terrainProxy.terrainSelectedToolIndex = -1;
            //}
        }

        void OnSceneGUI()
        {
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                if (terrainProxy != null && terrainProxy.detailManager != null && terrainProxy.detailManager.gpuiSimulator != null && terrainProxy.detailManager.gpuiSimulator.simulateAtEditor
                    && terrainProxy.detailManager.keepSimulationLive && terrainProxy.detailManager.updateSimulation)
                {
                    if (terrainProxy.detailManager.isInitialized)
                    {
                        //Debug.Log("restarting simulation");
                        terrainProxy.detailManager.gpuiSimulator.StopSimulation();
                        terrainProxy.detailManager.gpuiSimulator.StartSimulation();
                    }
                }
            }
        }
    }
}