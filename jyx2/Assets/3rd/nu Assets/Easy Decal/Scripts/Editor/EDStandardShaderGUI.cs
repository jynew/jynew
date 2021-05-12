using System;
using UnityEngine;

namespace UnityEditor
{
    internal class EDStandardShaderGUI : ShaderGUI
    {
        //------------------------------------
        // Fields
        //------------------------------------
        private ColorPickerHDRConfig m_ColorPickerHDRConfig = new ColorPickerHDRConfig(0.0f, 99f, 1.0f / 99.0f, 3f);
        private bool m_FirstTimeApply = true;
        private MaterialProperty blendMode;
        private MaterialProperty albedoMap;
        private MaterialProperty grungeMap;
        private MaterialProperty grungeFactor;


        private MaterialProperty albedoColor;
        //private MaterialProperty alphaCutoff;
        private MaterialProperty specularMap;
        private MaterialProperty specularColor;
        private MaterialProperty metallicMap;
        private MaterialProperty metallic;
        private MaterialProperty smoothness;
        private MaterialProperty bumpScale;
        private MaterialProperty bumpMap;
        private MaterialProperty occlusionStrength;
        private MaterialProperty occlusionMap;
        private MaterialProperty heigtMapScale;
        private MaterialProperty heightMap;
        private MaterialProperty emissionColorForRendering;
        private MaterialProperty emissionMap;
        private MaterialProperty detailMask;
        private MaterialProperty detailAlbedoMap;
        private MaterialProperty detailNormalMapScale;
        private MaterialProperty detailNormalMap;
        private MaterialProperty uvSetSecondary;
        private MaterialEditor m_MaterialEditor;
        private EDStandardShaderGUI.WorkflowMode m_WorkflowMode;

        //------------------------------------
        // Methods
        //------------------------------------
        public void FindProperties(MaterialProperty[] props)
        {
            this.blendMode = ShaderGUI.FindProperty("_Mode", props);
            this.albedoMap = ShaderGUI.FindProperty("_MainTex", props);
            this.grungeMap = ShaderGUI.FindProperty("_GrungeMask", props);
            this.grungeFactor = ShaderGUI.FindProperty("_GrungeFactor", props);
            this.albedoColor = ShaderGUI.FindProperty("_Color", props);
            //this.alphaCutoff = ShaderGUI.FindProperty("_Cutoff", props);
            this.specularMap = ShaderGUI.FindProperty("_SpecGlossMap", props, false);
            this.specularColor = ShaderGUI.FindProperty("_SpecColor", props, false);
            this.metallicMap = ShaderGUI.FindProperty("_MetallicGlossMap", props, false);
            this.metallic = ShaderGUI.FindProperty("_Metallic", props, false);
            this.m_WorkflowMode = this.specularMap == null || this.specularColor == null ? (this.metallicMap == null || this.metallic == null ? EDStandardShaderGUI.WorkflowMode.Dielectric : EDStandardShaderGUI.WorkflowMode.Metallic) : EDStandardShaderGUI.WorkflowMode.Specular;
            this.smoothness = ShaderGUI.FindProperty("_Glossiness", props);
            this.bumpScale = ShaderGUI.FindProperty("_BumpScale", props);
            this.bumpMap = ShaderGUI.FindProperty("_BumpMap", props);
            this.heigtMapScale = ShaderGUI.FindProperty("_Parallax", props);
            this.heightMap = ShaderGUI.FindProperty("_ParallaxMap", props);
            this.occlusionStrength = ShaderGUI.FindProperty("_OcclusionStrength", props);
            this.occlusionMap = ShaderGUI.FindProperty("_OcclusionMap", props);
            this.emissionColorForRendering = ShaderGUI.FindProperty("_EmissionColor", props);
            this.emissionMap = ShaderGUI.FindProperty("_EmissionMap", props);
            this.detailMask = ShaderGUI.FindProperty("_DetailMask", props);
            this.detailAlbedoMap = ShaderGUI.FindProperty("_DetailAlbedoMap", props);
            this.detailNormalMapScale = ShaderGUI.FindProperty("_DetailNormalMapScale", props);
            this.detailNormalMap = ShaderGUI.FindProperty("_DetailNormalMap", props);
            this.uvSetSecondary = ShaderGUI.FindProperty("_UVSec", props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            this.FindProperties(props);
            this.m_MaterialEditor = materialEditor;
            Material material = materialEditor.target as Material;
            this.ShaderPropertiesGUI(material);
            if (!this.m_FirstTimeApply)
                return;
            EDStandardShaderGUI.SetMaterialKeywords(material, this.m_WorkflowMode);
            this.m_FirstTimeApply = false;
        }

        public void ShaderPropertiesGUI(Material material)
        {
            EditorGUIUtility.labelWidth = 0.0f;
            EditorGUI.BeginChangeCheck();
            this.BlendModePopup();
            GUILayout.Label(EDStandardShaderGUI.Styles.primaryMapsText, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.DoAlbedoArea(material);
            this.DoGrungMaskArea(material);

            this.DoSpecularMetallicArea();
            this.m_MaterialEditor.TexturePropertySingleLine(EDStandardShaderGUI.Styles.normalMapText, this.bumpMap, !((UnityEngine.Object)this.bumpMap.textureValue != (UnityEngine.Object)null) ? (MaterialProperty)null : this.bumpScale);
            this.m_MaterialEditor.TexturePropertySingleLine(EDStandardShaderGUI.Styles.heightMapText, this.heightMap, !((UnityEngine.Object)this.heightMap.textureValue != (UnityEngine.Object)null) ? (MaterialProperty)null : this.heigtMapScale);
            this.m_MaterialEditor.TexturePropertySingleLine(EDStandardShaderGUI.Styles.occlusionText, this.occlusionMap, !((UnityEngine.Object)this.occlusionMap.textureValue != (UnityEngine.Object)null) ? (MaterialProperty)null : this.occlusionStrength);
            this.DoEmissionArea(material);
            this.m_MaterialEditor.TexturePropertySingleLine(EDStandardShaderGUI.Styles.detailMaskText, this.detailMask);
            EditorGUI.BeginChangeCheck();
            this.m_MaterialEditor.TextureScaleOffsetProperty(this.albedoMap);
            if (EditorGUI.EndChangeCheck())
                this.emissionMap.textureScaleAndOffset = this.albedoMap.textureScaleAndOffset;
            EditorGUILayout.Space();
            GUILayout.Label(EDStandardShaderGUI.Styles.secondaryMapsText, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.m_MaterialEditor.TexturePropertySingleLine(EDStandardShaderGUI.Styles.detailAlbedoText, this.detailAlbedoMap);
            this.m_MaterialEditor.TexturePropertySingleLine(EDStandardShaderGUI.Styles.detailNormalMapText, this.detailNormalMap, this.detailNormalMapScale);
            this.m_MaterialEditor.TextureScaleOffsetProperty(this.detailAlbedoMap);
            this.m_MaterialEditor.ShaderProperty(this.uvSetSecondary, EDStandardShaderGUI.Styles.uvSetLabel.text);
            if (!EditorGUI.EndChangeCheck())
                return;
            foreach (Material material1 in this.blendMode.targets)
                EDStandardShaderGUI.MaterialChanged(material1, this.m_WorkflowMode);
        }

        internal void DetermineWorkflow(MaterialProperty[] props)
        {
            if (ShaderGUI.FindProperty("_SpecGlossMap", props, false) != null && ShaderGUI.FindProperty("_SpecColor", props, false) != null)
                this.m_WorkflowMode = EDStandardShaderGUI.WorkflowMode.Specular;
            else if (ShaderGUI.FindProperty("_MetallicGlossMap", props, false) != null && ShaderGUI.FindProperty("_Metallic", props, false) != null)
                this.m_WorkflowMode = EDStandardShaderGUI.WorkflowMode.Metallic;
            else
                this.m_WorkflowMode = EDStandardShaderGUI.WorkflowMode.Dielectric;
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            base.AssignNewShaderToMaterial(material, oldShader, newShader);
            if ((UnityEngine.Object)oldShader == (UnityEngine.Object)null || !oldShader.name.Contains("Legacy Shaders/"))
                return;
            EDStandardShaderGUI.BlendMode blendMode = EDStandardShaderGUI.BlendMode.Opaque;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
                blendMode = EDStandardShaderGUI.BlendMode.Cutout;
            else if (oldShader.name.Contains("/Transparent/"))
                blendMode = EDStandardShaderGUI.BlendMode.Fade;
            material.SetFloat("_Mode", (float)blendMode);
            Material[] materialArray = new Material[1];
            int index = 0;
            Material material1 = material;
            materialArray[index] = material1;
            this.DetermineWorkflow(MaterialEditor.GetMaterialProperties((UnityEngine.Object[])materialArray));
            EDStandardShaderGUI.MaterialChanged(material, this.m_WorkflowMode);
        }

        private void BlendModePopup()
        {
            EditorGUI.showMixedValue = this.blendMode.hasMixedValue;
            EDStandardShaderGUI.BlendMode blendMode1 = (EDStandardShaderGUI.BlendMode)this.blendMode.floatValue;
            EditorGUI.BeginChangeCheck();
            EDStandardShaderGUI.BlendMode blendMode2 = (EDStandardShaderGUI.BlendMode)EditorGUILayout.Popup(EDStandardShaderGUI.Styles.renderingMode, (int)blendMode1, EDStandardShaderGUI.Styles.blendNames, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                this.blendMode.floatValue = (float)blendMode2;
            }
            EditorGUI.showMixedValue = false;
        }


        private void DoGrungMaskArea(Material material)
        {
            this.m_MaterialEditor.TexturePropertySingleLine(EDStandardShaderGUI.Styles.grungeMaskText, this.grungeMap);

            this.m_MaterialEditor.ShaderProperty(this.grungeFactor, EDStandardShaderGUI.Styles.grungeFactorText.text, 3);
        }

        private void DoAlbedoArea(Material material)
        {
            this.m_MaterialEditor.TexturePropertySingleLine(EDStandardShaderGUI.Styles.albedoText, this.albedoMap, this.albedoColor);
            if ((int)material.GetFloat("_Mode") != 1)
                return;
            this.m_MaterialEditor.ShaderProperty(this.grungeFactor, EDStandardShaderGUI.Styles.alphaCutoffText.text, 3);
        }

        private void DoEmissionArea(Material material)
        {
            float maxColorComponent = this.emissionColorForRendering.colorValue.maxColorComponent;
            bool flag1 = !this.HasValidEmissiveKeyword(material);
            bool flag2 = (double)maxColorComponent > 0.0;
            bool flag3 = (UnityEngine.Object)this.emissionMap.textureValue != (UnityEngine.Object)null;
            this.m_MaterialEditor.TexturePropertyWithHDRColor(EDStandardShaderGUI.Styles.emissionText, this.emissionMap, this.emissionColorForRendering, this.m_ColorPickerHDRConfig, false);
            if ((UnityEngine.Object)this.emissionMap.textureValue != (UnityEngine.Object)null && !flag3 && (double)maxColorComponent <= 0.0)
                this.emissionColorForRendering.colorValue = Color.white;
            if (flag2)
            {
                EditorGUI.BeginDisabledGroup(!EDStandardShaderGUI.ShouldEmissionBeEnabled(this.emissionColorForRendering.colorValue));
                this.m_MaterialEditor.LightmapEmissionProperty(3);
                EditorGUI.EndDisabledGroup();
            }
            if (!flag1)
                return;
            EditorGUILayout.HelpBox(EDStandardShaderGUI.Styles.emissiveWarning.text, MessageType.Warning);
        }

        private void DoSpecularMetallicArea()
        {
            if (this.m_WorkflowMode == EDStandardShaderGUI.WorkflowMode.Specular)
            {
                if ((UnityEngine.Object)this.specularMap.textureValue == (UnityEngine.Object)null)
                    this.m_MaterialEditor.TexturePropertyTwoLines(EDStandardShaderGUI.Styles.specularMapText, this.specularMap, this.specularColor, EDStandardShaderGUI.Styles.smoothnessText, this.smoothness);
                else
                    this.m_MaterialEditor.TexturePropertySingleLine(EDStandardShaderGUI.Styles.specularMapText, this.specularMap);
            }
            else
            {
                if (this.m_WorkflowMode != EDStandardShaderGUI.WorkflowMode.Metallic)
                    return;
                if ((UnityEngine.Object)this.metallicMap.textureValue == (UnityEngine.Object)null)
                    this.m_MaterialEditor.TexturePropertyTwoLines(EDStandardShaderGUI.Styles.metallicMapText, this.metallicMap, this.metallic, EDStandardShaderGUI.Styles.smoothnessText, this.smoothness);
                else
                    this.m_MaterialEditor.TexturePropertySingleLine(EDStandardShaderGUI.Styles.metallicMapText, this.metallicMap);
            }
        }

        public static void SetupMaterialWithBlendMode(Material material, EDStandardShaderGUI.BlendMode blendMode)
        {
            switch (blendMode)
            {
                case EDStandardShaderGUI.BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", string.Empty);
                    material.SetInt("_SrcBlend", 1);
                    material.SetInt("_DstBlend", 0);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case EDStandardShaderGUI.BlendMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", 1);
                    material.SetInt("_DstBlend", 0);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case EDStandardShaderGUI.BlendMode.Fade:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", 5);
                    material.SetInt("_DstBlend", 10);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case EDStandardShaderGUI.BlendMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", 1);
                    material.SetInt("_DstBlend", 10);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }

        private static bool ShouldEmissionBeEnabled(Color color)
        {
            return (double)color.maxColorComponent > 0.000392156856833026;
        }

        private static void SetMaterialKeywords(Material material, EDStandardShaderGUI.WorkflowMode workflowMode)
        {
            EDStandardShaderGUI.SetKeyword(material, "_NORMALMAP", (bool)((UnityEngine.Object)material.GetTexture("_BumpMap")) || (bool)((UnityEngine.Object)material.GetTexture("_DetailNormalMap")));
            if (workflowMode == EDStandardShaderGUI.WorkflowMode.Specular)
                EDStandardShaderGUI.SetKeyword(material, "_SPECGLOSSMAP", (bool)((UnityEngine.Object)material.GetTexture("_SpecGlossMap")));
            else if (workflowMode == EDStandardShaderGUI.WorkflowMode.Metallic)
                EDStandardShaderGUI.SetKeyword(material, "_METALLICGLOSSMAP", (bool)((UnityEngine.Object)material.GetTexture("_MetallicGlossMap")));
            EDStandardShaderGUI.SetKeyword(material, "_PARALLAXMAP", (bool)((UnityEngine.Object)material.GetTexture("_ParallaxMap")));
            EDStandardShaderGUI.SetKeyword(material, "_DETAIL_MULX2", (bool)((UnityEngine.Object)material.GetTexture("_DetailAlbedoMap")) || (bool)((UnityEngine.Object)material.GetTexture("_DetailNormalMap")));
            bool state = EDStandardShaderGUI.ShouldEmissionBeEnabled(material.GetColor("_EmissionColor"));
            EDStandardShaderGUI.SetKeyword(material, "_EMISSION", state);
            MaterialGlobalIlluminationFlags illuminationFlags1 = material.globalIlluminationFlags;
            if ((illuminationFlags1 & (MaterialGlobalIlluminationFlags.RealtimeEmissive | MaterialGlobalIlluminationFlags.BakedEmissive)) == MaterialGlobalIlluminationFlags.None)
                return;
            MaterialGlobalIlluminationFlags illuminationFlags2 = illuminationFlags1 & ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            if (!state)
                illuminationFlags2 |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            material.globalIlluminationFlags = illuminationFlags2;
        }

        private bool HasValidEmissiveKeyword(Material material)
        {
            return material.IsKeywordEnabled("_EMISSION") || !EDStandardShaderGUI.ShouldEmissionBeEnabled(this.emissionColorForRendering.colorValue);
        }

        private static void MaterialChanged(Material material, EDStandardShaderGUI.WorkflowMode workflowMode)
        {
            EDStandardShaderGUI.SetupMaterialWithBlendMode(material, (EDStandardShaderGUI.BlendMode)material.GetFloat("_Mode"));
            EDStandardShaderGUI.SetMaterialKeywords(material, workflowMode);
        }

        private static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
                m.EnableKeyword(keyword);
            else
                m.DisableKeyword(keyword);
        }

        private enum WorkflowMode
        {
            Specular,
            Metallic,
            Dielectric,
        }

        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent,
        }

        private static class Styles
        {
            public static GUIStyle optionsButton = (GUIStyle)"PaneOptions";
            public static GUIContent uvSetLabel = new GUIContent("UV Set");
            public static GUIContent[] uvSetOptions;
            public static string emptyTootip;
            public static GUIContent albedoText;
            public static GUIContent grungeMaskText;
            public static GUIContent grungeFactorText;
            public static GUIContent alphaCutoffText;
            public static GUIContent specularMapText;
            public static GUIContent metallicMapText;
            public static GUIContent smoothnessText;
            public static GUIContent normalMapText;
            public static GUIContent heightMapText;
            public static GUIContent occlusionText;
            public static GUIContent emissionText;
            public static GUIContent detailMaskText;
            public static GUIContent detailAlbedoText;
            public static GUIContent detailNormalMapText;
            public static string whiteSpaceString;
            public static string primaryMapsText;
            public static string secondaryMapsText;
            public static string renderingMode;
            public static GUIContent emissiveWarning;
            public static GUIContent emissiveColorWarning;
            public static readonly string[] blendNames;

            static Styles()
            {
                GUIContent[] guiContentArray = new GUIContent[2];
                int index1 = 0;
                GUIContent guiContent1 = new GUIContent("UV channel 0");
                guiContentArray[index1] = guiContent1;
                int index2 = 1;
                GUIContent guiContent2 = new GUIContent("UV channel 1");
                guiContentArray[index2] = guiContent2;
                EDStandardShaderGUI.Styles.uvSetOptions = guiContentArray;
                EDStandardShaderGUI.Styles.emptyTootip = string.Empty;
                EDStandardShaderGUI.Styles.albedoText = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");
                EDStandardShaderGUI.Styles.grungeMaskText = new GUIContent("Grunge Mask", "Grunge Map (Grayscale RGB)");
                EDStandardShaderGUI.Styles.grungeFactorText = new GUIContent("Grunge Factor", "Grunge Map Factor");
                EDStandardShaderGUI.Styles.alphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff");
                EDStandardShaderGUI.Styles.specularMapText = new GUIContent("Specular", "Specular (RGB) and Smoothness (A)");
                EDStandardShaderGUI.Styles.metallicMapText = new GUIContent("Metallic", "Metallic (R) and Smoothness (A)");
                EDStandardShaderGUI.Styles.smoothnessText = new GUIContent("Smoothness", string.Empty);
                EDStandardShaderGUI.Styles.normalMapText = new GUIContent("Normal Map", "Normal Map");
                EDStandardShaderGUI.Styles.heightMapText = new GUIContent("Height Map", "Height Map (G)");
                EDStandardShaderGUI.Styles.occlusionText = new GUIContent("Occlusion", "Occlusion (G)");
                EDStandardShaderGUI.Styles.emissionText = new GUIContent("Emission", "Emission (RGB)");
                EDStandardShaderGUI.Styles.detailMaskText = new GUIContent("Detail Mask", "Mask for Secondary Maps (A)");
                EDStandardShaderGUI.Styles.detailAlbedoText = new GUIContent("Detail Albedo x2", "Albedo (RGB) multiplied by 2");
                EDStandardShaderGUI.Styles.detailNormalMapText = new GUIContent("Normal Map", "Normal Map");
                EDStandardShaderGUI.Styles.whiteSpaceString = " ";
                EDStandardShaderGUI.Styles.primaryMapsText = "Main Maps";
                EDStandardShaderGUI.Styles.secondaryMapsText = "Secondary Maps";
                EDStandardShaderGUI.Styles.renderingMode = "Rendering Mode";
                EDStandardShaderGUI.Styles.emissiveWarning = new GUIContent("Emissive value is animated but the material has not been configured to support emissive. Please make sure the material itself has some amount of emissive.");
                EDStandardShaderGUI.Styles.emissiveColorWarning = new GUIContent("Ensure emissive color is non-black for emission to have effect.");
                EDStandardShaderGUI.Styles.blendNames = Enum.GetNames(typeof(EDStandardShaderGUI.BlendMode));
            }
        }
    }
}
