using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEditor.Utils;
using UnityEditorInternal;

#if UNITY_EDITOR
namespace MK.Toon
{
    #pragma warning disable CS0612, CS0618, CS1692
    
    public static class GuiStyles
    {
        public static GUIStyle header = new GUIStyle("ShurikenModuleTitle")
        {
            font = (new GUIStyle("Label")).font,
            border = new RectOffset(15, 7, 4, 4),
            fixedHeight = 22,
            contentOffset = new Vector2(20f, -2f),
        };

        public static GUIStyle headerCheckbox = new GUIStyle("ShurikenCheckMark");
        public static GUIStyle headerCheckboxMixed = new GUIStyle("ShurikenCheckMarkMixed");
    }

    public class MKToonFreeEditor : ShaderGUI
    {        
        //hdr config
        private ColorPickerHDRConfig colorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 1 / 99f, 3f);

        //Editor Properties
        private MaterialProperty showMainBehavior = null;
        private MaterialProperty showLightBehavior = null;
        private MaterialProperty showRenderBehavior = null;
        private MaterialProperty showSpecularBehavior = null;
        private MaterialProperty showRimBehavior = null;
        private MaterialProperty showShadowBehavior = null;
        private MaterialProperty showOutlineBehavior = null;

        //Main
        private MaterialProperty mainColor = null;
        private MaterialProperty mainTex = null;

        //Normalmap
        private MaterialProperty bumpMap = null;

        //Light
        private MaterialProperty lTreshold = null;

        //Render
        private MaterialProperty lightSmoothness = null;
        private MaterialProperty rimSmoothness = null;

        //Custom shadow
        private MaterialProperty shadowColor = null;
        private MaterialProperty highlightColor = null;
        private MaterialProperty shadowIntensity = null;

        //Outline
        private MaterialProperty outlineColor = null;
        private MaterialProperty outlineSize = null;

        //Rim
        private MaterialProperty rimColor = null;
        private MaterialProperty rimSize = null;
        private MaterialProperty rimIntensity = null;

        //Specular
        private MaterialProperty specularIntensity = null;
        private MaterialProperty shininess = null;
        private MaterialProperty specularColor = null;

        //Emission
        private MaterialProperty emissionColor = null;

        private bool showGIField = false;

        public void FindProperties(MaterialProperty[] props, Material mat)
        {
            //Editor Properties
            showMainBehavior = FindProperty("_MKEditorShowMainBehavior", props);
            showLightBehavior = FindProperty("_MKEditorShowLightBehavior", props);
            showRenderBehavior = FindProperty("_MKEditorShowRenderBehavior", props);
            showSpecularBehavior = FindProperty("_MKEditorShowSpecularBehavior", props);
            showRimBehavior = FindProperty("_MKEditorShowRimBehavior", props);
            showShadowBehavior = FindProperty("_MKEditorShowShadowBehavior", props);
            showOutlineBehavior = FindProperty("_MKEditorShowOutlineBehavior", props);

            //Main
            mainColor = FindProperty("_Color", props);
            mainTex = FindProperty("_MainTex", props);

            //Normalmap
            bumpMap = FindProperty("_BumpMap", props);

            //Light
            lTreshold = FindProperty("_LightThreshold", props);

            //Render
            lightSmoothness = FindProperty("_LightSmoothness", props);
            rimSmoothness = FindProperty("_RimSmoothness", props);

            //Custom shadow
            shadowIntensity = FindProperty("_ShadowIntensity", props);
            shadowColor = FindProperty("_ShadowColor", props);
            highlightColor = FindProperty("_HighlightColor", props);

            //Outline
            outlineColor = FindProperty("_OutlineColor", props);
            outlineSize = FindProperty("_OutlineSize", props);

            //Rim
            rimColor = FindProperty("_RimColor", props);
            rimSize = FindProperty("_RimSize", props);
            rimIntensity = FindProperty("_RimIntensity", props);

            //Specular
            shininess = FindProperty("_Shininess", props);
            specularColor = FindProperty("_SpecColor", props);
            specularIntensity = FindProperty("_SpecularIntensity", props);

            //Emission
            emissionColor = FindProperty("_EmissionColor", props);
        }

        //Colorfield
        private void ColorProperty(MaterialProperty prop, bool showAlpha, bool hdrEnabled, GUIContent label)
        {
            EditorGUI.showMixedValue = prop.hasMixedValue;
            EditorGUI.BeginChangeCheck();
            Color c = EditorGUILayout.ColorField(label, prop.colorValue, false, showAlpha, hdrEnabled, colorPickerHDRConfig);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                prop.colorValue = c;
        }

        //Setup GI emission
        private void SetGIFlags()
        {
            foreach (Material obj in emissionColor.targets)
            {
                bool emissive = true;
                if (obj.GetColor("_EmissionColor") == Color.black)
                {
                    emissive = false;
                }
                MaterialGlobalIlluminationFlags flags = obj.globalIlluminationFlags;
                if ((flags & (MaterialGlobalIlluminationFlags.BakedEmissive | MaterialGlobalIlluminationFlags.RealtimeEmissive)) != 0)
                {
                    flags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                    if (!emissive)
                        flags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;

                    obj.globalIlluminationFlags = flags;
                }
            }
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }
            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            MaterialProperty[] properties = MaterialEditor.GetMaterialProperties(new Material[] { material });
            FindProperties(properties, material);

            SetGIFlags();
        }

        private bool HandleBehavior(string title, ref MaterialProperty behavior, MaterialEditor materialEditor)
        {
            EditorGUI.showMixedValue = behavior.hasMixedValue;
            var rect = GUILayoutUtility.GetRect(16f, 22f, GuiStyles.header);
            rect.x -= 10;
            rect.width += 10;
            var e = Event.current;

            GUI.Box(rect, title, GuiStyles.header);

            var foldoutRect = new Rect(EditorGUIUtility.currentViewWidth * 0.5f, rect.y + 2, 13f, 13f);
            if (behavior.hasMixedValue)
            {
                foldoutRect.x -= 13;
                foldoutRect.y -= 2;
            }

            EditorGUI.BeginChangeCheck();
            if (e.type == EventType.MouseDown)
            {
                if (rect.Contains(e.mousePosition))
                {
                    if (behavior.hasMixedValue)
                        behavior.floatValue = 0.0f;
                    else
                        behavior.floatValue = Convert.ToSingle(!Convert.ToBoolean(behavior.floatValue));
                    e.Use();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (Convert.ToBoolean(behavior.floatValue))
                    materialEditor.RegisterPropertyChangeUndo(behavior.displayName + " Show");
                else
                    materialEditor.RegisterPropertyChangeUndo(behavior.displayName + " Hide");
            }

            EditorGUI.showMixedValue = false;

            if (e.type == EventType.Repaint && behavior.hasMixedValue)
                EditorStyles.radioButton.Draw(foldoutRect, "", false, false, true, false);
            else
                EditorGUI.Foldout(foldoutRect, Convert.ToBoolean(behavior.floatValue), "");

            if (behavior.hasMixedValue)
                return true;
            else
                return Convert.ToBoolean(behavior.floatValue);
        }

        override public void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material targetMat = materialEditor.target as Material;
            //get properties
            FindProperties(properties, targetMat);

            if (emissionColor.colorValue != Color.black)
                showGIField = true;
            else
                showGIField = false;

            EditorGUI.BeginChangeCheck();

            //main settings
            if (HandleBehavior("Main", ref showMainBehavior, materialEditor))
            {
                ColorProperty(mainColor, false, false, new GUIContent(mainColor.displayName));
                materialEditor.TexturePropertySingleLine(new GUIContent(mainTex.displayName), mainTex);
                materialEditor.TexturePropertySingleLine(new GUIContent(bumpMap.displayName), bumpMap);
                EditorGUI.BeginChangeCheck();
                ColorProperty(emissionColor, false, true, new GUIContent(emissionColor.displayName));
                if (showGIField)
                    materialEditor.LightmapEmissionProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
                if (EditorGUI.EndChangeCheck())
                {
                    SetGIFlags();
                }
                materialEditor.TextureScaleOffsetProperty(mainTex);
            }

            //light settings
            if (HandleBehavior("Light", ref showLightBehavior, materialEditor))
            {
                materialEditor.ShaderProperty(lTreshold, lTreshold.displayName);
                materialEditor.ShaderProperty(lightSmoothness, lightSmoothness.displayName);
            }

            //custom shadow settings
            if (HandleBehavior("Shadow", ref showShadowBehavior, materialEditor))
            {
                ColorProperty(highlightColor, false, false, new GUIContent(highlightColor.displayName));
                ColorProperty(shadowColor, false, false, new GUIContent(shadowColor.displayName));
                materialEditor.ShaderProperty(shadowIntensity, shadowIntensity.displayName);
            }

            //render settings
            if (HandleBehavior("Render", ref showRenderBehavior, materialEditor))
            {
                materialEditor.EnableInstancingField();
            }

            //specular settings
            if (HandleBehavior("Specular", ref showSpecularBehavior, materialEditor))
            {
                ColorProperty(specularColor, false, false, new GUIContent(specularColor.displayName));
                materialEditor.ShaderProperty(shininess, shininess.displayName);
                materialEditor.ShaderProperty(specularIntensity, specularIntensity.displayName);
            }

            //rim settings
            if (HandleBehavior("Rim", ref showRimBehavior, materialEditor))
            {
                ColorProperty(rimColor, false, false, new GUIContent(rimColor.displayName));
                materialEditor.ShaderProperty(rimSize, rimSize.displayName);
                materialEditor.ShaderProperty(rimIntensity, rimIntensity.displayName);
                materialEditor.ShaderProperty(rimSmoothness, rimSmoothness.displayName);
            }

            //set outline
            if (HandleBehavior("Outline", ref showOutlineBehavior, materialEditor))
            {
                ColorProperty(outlineColor, false, false, new GUIContent(outlineColor.displayName));
                materialEditor.ShaderProperty(outlineSize, outlineSize.displayName);
            }

            ///Other
            EditorGUI.EndChangeCheck();
        }

        private void Divider()
        {
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        }
    }
}
#endif