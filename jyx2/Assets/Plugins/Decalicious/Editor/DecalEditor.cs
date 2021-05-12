using UnityEditor;
using UnityEngine;

namespace ThreeEyedGames
{
    [CustomEditor(typeof(Decal))]
    [CanEditMultipleObjects]
    public class DecalEditor : Editor
    {
        protected Decal _decal;

        public override void OnInspectorGUI()
        {
            _decal = target as Decal;

            SerializedProperty material = serializedObject.FindProperty("Material");
            SerializedProperty renderOrder = serializedObject.FindProperty("RenderOrder");
            SerializedProperty fade = serializedObject.FindProperty("Fade");
            SerializedProperty limitTo = serializedObject.FindProperty("LimitTo");
            SerializedProperty drawAlbedo = serializedObject.FindProperty("DrawAlbedo");
            SerializedProperty useLightProbes = serializedObject.FindProperty("UseLightProbes");
            SerializedProperty drawNormalAndGloss = serializedObject.FindProperty("DrawNormalAndGloss");
            SerializedProperty highQualityBlending = serializedObject.FindProperty("HighQualityBlending");

            EditorGUILayout.PropertyField(material);
            EditorGUILayout.PropertyField(renderOrder);
            Decal.DecalRenderMode dRenderMode = _decal.RenderMode;
            switch (dRenderMode)
            {
                case Decal.DecalRenderMode.Deferred:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Deferred Decal", EditorStyles.boldLabel);
                    if (Camera.main != null && Camera.main.actualRenderingPath != RenderingPath.DeferredShading)
                    {
                        EditorGUILayout.HelpBox("Main camera is not using the Deferred rendering path. " +
                            "Deferred decals will not be drawn. Current path: " + Camera.main.actualRenderingPath, MessageType.Error);
                    }
                    EditorGUILayout.PropertyField(drawAlbedo);
                    EditorGUI.BeginDisabledGroup(!drawAlbedo.boolValue);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(useLightProbes);
                    EditorGUI.indentLevel--;
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.PropertyField(drawNormalAndGloss);
                    EditorGUI.BeginDisabledGroup(!drawNormalAndGloss.boolValue);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(highQualityBlending);
                    EditorGUI.indentLevel--;
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.PropertyField(fade);
                    EditorGUILayout.PropertyField(limitTo);
                    break;
                case Decal.DecalRenderMode.Unlit:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Unlit Decal", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(fade);
                    EditorGUILayout.PropertyField(limitTo);
                    break;
                case Decal.DecalRenderMode.Invalid:
                default:
                    EditorGUILayout.HelpBox("Please select a Material with a Decalicious shader.", MessageType.Info);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
