using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace MTE
{
    [CustomEditor(typeof(GrassLoader))]
    internal class GrassLoaderEditor : Editor
    {
        /// <summary>
        /// Remove lightmap data from the grass GameObject, so it won't be influenced by the possibly invalid lightmap data.
        /// </summary>
        private static void ClearLightmapData()
        {
            var items = GrassMap.GetAllGrassItems();
            foreach (var item in items)
            {
                if (item == null)
                {
                    MTEDebug.LogWarning("A grass item is null.");
                    continue;
                }

                var obj = item.gameObject;
                if (obj == null)
                {
                    MTEDebug.LogWarning(
                        "A grass object is not available: null or the referenced object is missing.");
                    continue;
                }

                var renderer = obj.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                    MTEDebug.LogWarning(
                        "Cannot access lightmap data because the grass GameObject" +
                        " don't have a MeshRenderer.");
                    continue;
                }

                renderer.lightmapIndex = -1;
                renderer.lightmapScaleOffset = new Vector4(1, 1, 0, 0);
            }
        }

        private static void SaveLightmapData()
        {
            var items = GrassMap.GetAllGrassItems();
            foreach (var item in items)
            {
                if (item == null)
                {
                    MTEDebug.LogWarning("A grass item is null.");
                    continue;
                }

                var obj = item.gameObject;
                if (obj == null)
                {
                    MTEDebug.LogWarning(
                        "A grass GameObject is not available: null or the referenced object is missing.");
                    continue;
                }

                var renderer = obj.gameObject.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                    MTEDebug.LogWarning(
                        "A grass object is broken: not MeshRenderer attached." +
                        " Please *Rebuild Grasses*.");
                    continue;
                }

                var lightmapIndex = renderer.lightmapIndex;
                var lightmapScaleOffset = renderer.lightmapScaleOffset;

                //check if lightmap data is valid
                if (lightmapIndex == -1)
                {
                    MTEDebug.LogWarning(
                        "Lightmap data for some grass is invalid: lightmap index is -1.");
                    continue;
                }

                var quad = item.Quad;
                var star = item.Star;
                if (quad != null)
                {
                    quad.SaveLightmapData(lightmapIndex, lightmapScaleOffset);
                }
                else if (star != null)
                {
                    star.SaveLightmapData(lightmapIndex, lightmapScaleOffset);
                }
                else
                {
                    MTEDebug.LogError(
                        "A grass object is not a quad nor a star: the data for this grass is invalid." +
                        " The grass asset file may have been corrupted.");
                    return;
                }
            }

            // Save grass asset file
            EditorUtility.SetDirty(MTEContext.TheGrassLoader.grassInstanceList);

            Utility.ShowNotification(StringTable.Get(C.Info_LightmapDataSaved));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = !Application.isPlaying;

            GUILayout.Space(10);

            var thisGameObject = ((GrassLoader) this.target).gameObject;

            if (CompatibilityUtil.IsPrefab(thisGameObject))
            {
                EditorGUILayout.HelpBox("Cannot use prefab for GrassLoader. Click the button below to fix it:", MessageType.Error);
                if (GUILayout.Button("Revert to normal GameObject."))
                {
                    Utility.ConvertPrefabToGameObject(thisGameObject);
                }
                return;
            }

            if (GUILayout.Button(StringTable.Get(C.ReloadFromFile), GUILayout.Height(60)))
            {
                GrassEditorUtil.ReloadGrassesFromFile((GrassLoader)target);
            }
            GUILayout.Space(5);
            if(GUILayout.Button(StringTable.Get(C.BakeLightmap), GUILayout.Height(60)))
            {
                Lightmapping.BakeAsync();
            }
            GUILayout.Space(5);
            if (GUILayout.Button(StringTable.Get(C.SaveLightmapData), GUILayout.Height(60)))
            {
                SaveLightmapData();
            }
            GUILayout.Space(5);
            if (GUILayout.Button(StringTable.Get(C.ClearPreviewGrasssesAndSaveScene), GUILayout.Height(60)))
            {
                ClearGrassesFromScene();
                var activeScene = SceneManager.GetActiveScene();
                var result = EditorSceneManager.SaveScene(activeScene);
                if (!result)
                {
                    MTEDebug.LogError("Failed to save the active scene. Please try to save the scene manually.");
                }
            }

            EditorGUILayout.HelpBox(StringTable.Get(C.Warning_HowToSaveASceneWithGrasses), MessageType.Warning);

            GUILayout.Space(100);

            GUI.enabled = true;
            if (Settings.DebugMode)
            {
                if (GUILayout.Button(StringTable.Get(C.ShowGrassObjects)))
                {
                    var grassLoader = (GrassLoader)target;
                    var loaderObject = grassLoader.gameObject;
                    var childCount = loaderObject.transform.childCount;
                    var transform = loaderObject.transform;
                    for (int i = 0; i < childCount; i++)
                    {
                        var gameObject = transform.GetChild(i).gameObject;
                        gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
                    }
                }
            }

        }

        private void ClearGrassesFromScene()
        {
            //clear grass GameObjects
            var grassLoader = (GrassLoader)target;
            grassLoader.RemoveOldGrasses();
        }

    }
}
