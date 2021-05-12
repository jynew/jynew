using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ch.sycoforge.Decal.Editor
{

    public class DSSDShaderGUI : ShaderGUI
    {
        //-----------------------------
        // Fields
        //-----------------------------
        private bool firstTimeApply = true;

        private MaterialProperty albedoMap;
        private MaterialProperty bumpMap;
        private MaterialProperty emissionMap;
        private MaterialProperty specSmoothMap;
        private MaterialEditor materialEditor;



        //-----------------------------
        // Methods
        //-----------------------------
        private void FindProperties(MaterialProperty[] props)
        {
            this.albedoMap =     ShaderGUI.FindProperty(ShaderConstants.TEXTURE_DIFFUSE, props);
            this.bumpMap =       ShaderGUI.FindProperty(ShaderConstants.TEXTURE_NORMAL, props);
            this.emissionMap =   ShaderGUI.FindProperty(ShaderConstants.TEXTURE_EMISSION, props);
            this.specSmoothMap = ShaderGUI.FindProperty(ShaderConstants.TEXTURE_SPECSMOOTH, props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            this.FindProperties(props);
            this.materialEditor = materialEditor;
            Material material = materialEditor.target as Material;
            this.ShaderPropertiesGUI(material);

            if (!this.firstTimeApply) { return; }

            SetMaterialKeywords(material);
            this.firstTimeApply = false;
        }

        public void ShaderPropertiesGUI(Material material)
        {
            EditorGUIUtility.labelWidth = 0.0f;
            EditorGUI.BeginChangeCheck();

            GUILayout.Label("Main Maps", EditorStyles.boldLabel, new GUILayoutOption[0]);

            this.DoAlbedoArea(material);
            this.DoNormalArea(material);
            this.DoSpecSmoothArea(material);
            this.DoEmissionArea(material);


            if(EditorGUI.EndChangeCheck())
            {
                SetMaterialKeywords(material);
            }
        }

        private void DoAlbedoArea(Material material)
        {
            this.materialEditor.TexturePropertySingleLine(new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)"), this.albedoMap, null);
        }

        private void DoNormalArea(Material material)
        {
            this.materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map", "Normal Map (RGB)"), this.bumpMap, null);
        }

        private void DoSpecSmoothArea(Material material)
        {
            this.materialEditor.TexturePropertySingleLine(new GUIContent("Specular Smoothness", "Specular (RGB) Smoothness (A)"), this.specSmoothMap, null);
        }

        private void DoEmissionArea(Material material)
        {
            this.materialEditor.TexturePropertySingleLine(new GUIContent("Emission", "Emission (RGB)"), this.emissionMap, null);
        }

        private static void SetMaterialKeywords(Material material)
        {
            SetKeyword(material, "_Blend", true);
            SetKeyword(material, "_AlphaTest", false);

            SetKeyword(material, ShaderConstants.KEYWORD_NORMAL, material.GetTexture(ShaderConstants.TEXTURE_NORMAL) != null);
            SetKeyword(material, ShaderConstants.KEYWORD_DIFFUSE, material.GetTexture(ShaderConstants.TEXTURE_DIFFUSE) != null);
            SetKeyword(material, ShaderConstants.KEYWORD_SPECSMOOTH, material.GetTexture(ShaderConstants.TEXTURE_SPECSMOOTH) != null);
            SetKeyword(material, ShaderConstants.KEYWORD_EMISSION, material.GetTexture(ShaderConstants.TEXTURE_EMISSION) != null);
        }

        private static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
            {
                m.EnableKeyword(keyword);
            }
            else
            {
                m.DisableKeyword(keyword);
            }
        }
    }
}
