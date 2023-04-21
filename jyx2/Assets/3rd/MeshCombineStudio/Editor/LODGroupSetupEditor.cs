using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MeshCombineStudio
{
    [CustomEditor(typeof(LODGroupSetup))]
    public class LODGroupSetupEditor : Editor
    {
        LODGroupSetup lodGroupSetup;
        LOD[] oldLods;
        bool animateCrossFadingOld;
        LODFadeMode fadeModeOld;

        void OnEnable()
        {
            lodGroupSetup = (LODGroupSetup)target;
            LODGroup lodGroup = lodGroupSetup.lodGroup;

            oldLods = lodGroup.GetLODs();
            animateCrossFadingOld = lodGroup.animateCrossFading;
            fadeModeOld = lodGroup.fadeMode;

            UnityEditor.EditorApplication.update += MyUpdate;
        }

        void OnDisable()
        {
            UnityEditor.EditorApplication.update -= MyUpdate;
        }

        void MyUpdate()
        {
            LODGroup lodGroup = lodGroupSetup.lodGroup;
            lodGroup.size = lodGroupSetup.meshCombiner.cellSize;
            LOD[] lods = lodGroup.GetLODs();

            if (lods.Length != oldLods.Length)
            {
                Debug.LogError("Mesh Combine Studio -> Please don't change the amount of LODs, this is just a dummy LOD Group to apply settings to the LOD Groups in all children.");
                lodGroup.SetLODs(oldLods);
                return;
            }

            bool hasChanged = false;

            if (lodGroup.animateCrossFading != animateCrossFadingOld || lodGroup.fadeMode != fadeModeOld)
            {
                hasChanged = true;
            }
            else
            {
                for (int i = 0; i < lods.Length; i++)
                {
                    if (lods[i].renderers.Length != 0)
                    {
                        Debug.LogError("Mesh Combine Studio -> Please don't add any renderes, this is just a dummy LOD Group to apply settings to the LOD Groups in all children.");
                        lods[i].renderers = null;
                        lodGroup.SetLODs(lods);
                        return;
                    }
                    if (lods[i].screenRelativeTransitionHeight != oldLods[i].screenRelativeTransitionHeight) { hasChanged = true; break; }
                    if (lods[i].fadeTransitionWidth != oldLods[i].fadeTransitionWidth) { hasChanged = true; break; }
                }
            }

            if (hasChanged)
            {
                lodGroupSetup.ApplySetup();
                oldLods = lods;
            }
        }

        public override void OnInspectorGUI()
        {
            GUIDraw.DrawSpacer();
            GUI.color = Color.red;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;
            GUIDraw.Label("Modifications to this LOD Group will apply to all children", 12);
            EditorGUILayout.EndVertical();
            GUIDraw.DrawSpacer();
            
        }
    }
}
