using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ThreeEyedGames
{
    public class DecalShaderGUI : ShaderGUI
    {
        protected MaterialEditor _editor;
        protected MaterialProperty[] _properties;
        protected Material _target;

        protected enum DecalBlendMode
        {
            Default,
            Additive,
            Multiply
        }

        protected enum NormalBlendMode
        {
            Modulate,
            Overwrite
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            _target = materialEditor.target as Material;
            _editor = materialEditor;
            _properties = properties;

            DrawTextureSingleLine("_MaskTex", "_MaskMultiplier");
            EditorGUILayout.Space();

            DrawTextureSingleLine("_MainTex", "_Color");
            EditorGUI.indentLevel += 2;
            DecalBlendMode blendMode = (DecalBlendMode)_target.GetFloat("_DecalBlendMode");
            blendMode = (DecalBlendMode)EditorGUILayout.EnumPopup(new GUIContent("Blend Mode"), blendMode);
            ApplyBlendMode(blendMode);
            EditorGUI.indentLevel -= 2;

            if (_target.HasProperty("_NormalTex"))
            {
                EditorGUILayout.Space();
                DrawTextureSingleLine("_NormalTex", "_NormalMultiplier");
            }
            if (_target.HasProperty("_NormalBlendMode"))
            {
                EditorGUI.indentLevel += 2;
                NormalBlendMode normalBlendMode = (NormalBlendMode)_target.GetFloat("_NormalBlendMode");
                normalBlendMode = (NormalBlendMode)EditorGUILayout.EnumPopup(new GUIContent("Blend Mode"), normalBlendMode);
                _target.SetFloat("_NormalBlendMode", (float)normalBlendMode);
                bool maskNormals = (_target.GetInt("_MaskNormals") != 0);
                maskNormals = EditorGUILayout.Toggle(new GUIContent("Mask Normals?"), maskNormals);
                _target.SetFloat("_MaskNormals", (maskNormals ? 1.0f : 0.0f));
                EditorGUI.indentLevel -= 2;
                EditorGUILayout.Space();
            }

            DrawTextureSingleLine("_SpecularTex", "_SpecularMultiplier");
            DrawTextureSingleLine("_SmoothnessTex", "_SmoothnessMultiplier");
            EditorGUILayout.Space();

            DrawTextureSingleLine("_EmissionMap", "_EmissionColor");
            EditorGUILayout.Space();
            
            _editor.TextureScaleOffsetProperty(FindProperty("_MainTex", _properties));

            EditorGUILayout.Space();

            _editor.FloatProperty(FindProperty("_AngleLimit", _properties), "Angle Limit");
        }

        protected void ApplyBlendMode(DecalBlendMode source)
        {
            _target.SetFloat("_DecalBlendMode", (float)source);
            switch (source)
            {
                case DecalBlendMode.Default:
                    _target.SetFloat("_DecalSrcBlend", (float)BlendMode.One);
                    _target.SetFloat("_DecalDstBlend", (float)BlendMode.OneMinusSrcAlpha);
                    break;
                case DecalBlendMode.Additive:
                    _target.SetFloat("_DecalSrcBlend", (float)BlendMode.One);
                    _target.SetFloat("_DecalDstBlend", (float)BlendMode.One);
                    break;
                case DecalBlendMode.Multiply:
                    _target.SetFloat("_DecalSrcBlend", (float)BlendMode.DstColor);
                    _target.SetFloat("_DecalDstBlend", (float)BlendMode.OneMinusSrcAlpha);
                    break;
                default:
                    _target.SetFloat("_DecalSrcBlend", (float)BlendMode.Zero);
                    _target.SetFloat("_DecalDstBlend", (float)BlendMode.Zero);
                    Debug.LogError("Unsupported decal blend mode: " + source);
                    break;
            }
        }

        protected void DrawTextureSingleLine(string baseName, string extraName, bool showScale = false)
        {
            MaterialProperty texture = FindProperty(baseName, _properties, false);
            MaterialProperty extra = FindProperty(extraName, _properties, false);
            if (texture != null)
            {
                if (extra != null && extra.flags != MaterialProperty.PropFlags.PerRendererData)
                    _editor.TexturePropertySingleLine(new GUIContent(texture.displayName), texture, extra);
                else
                    _editor.TexturePropertySingleLine(new GUIContent(texture.displayName), texture);
                if (showScale)
                    _editor.TextureScaleOffsetProperty(texture);
            }
        }
    }
}
