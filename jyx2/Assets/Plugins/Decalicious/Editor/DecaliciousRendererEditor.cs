using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThreeEyedGames
{
    [CustomEditor(typeof(DecaliciousRenderer))]
    public class DecaliciousRendererEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

#if UNITY_5_5_OR_NEWER
            DecaliciousRenderer renderer = target as DecaliciousRenderer;
            bool supportsInstancing = SystemInfo.supportsInstancing;
            if (supportsInstancing)
                renderer.UseInstancing = EditorGUILayout.Toggle("Use Instancing?", supportsInstancing && renderer.UseInstancing);
            else
                EditorGUILayout.HelpBox("Your GPU does not support instancing. Try updating your drivers.", MessageType.Warning);
#else
            EditorGUILayout.HelpBox("Instanced decals require Unity 5.5 or newer. Consider updating your Unity version if performance is bad.", MessageType.Warning);
#endif
        }
    }
}
