using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("shader", "renderQueue", "shaderKeywords", "globalIlluminationFlags", "properties")]
	public class ES3Type_Material : ES3UnityObjectType
	{
		public static ES3Type Instance = null;

		public ES3Type_Material() : base(typeof(UnityEngine.Material)){ Instance = this; }

		protected override void WriteUnityObject(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Material)obj;

			writer.WriteProperty("name", instance.name);
			writer.WriteProperty("shader", instance.shader);
			writer.WriteProperty("renderQueue", instance.renderQueue, ES3Type_int.Instance);
			writer.WriteProperty("shaderKeywords", instance.shaderKeywords);
			writer.WriteProperty("globalIlluminationFlags", instance.globalIlluminationFlags);
			if(instance.HasProperty("_Color"))
				writer.WriteProperty("_Color", instance.GetColor("_Color"));
			if(instance.HasProperty("_SpecColor"))
				writer.WriteProperty("_SpecColor", instance.GetColor("_SpecColor"));
			if(instance.HasProperty("_Shininess"))
				writer.WriteProperty("_Shininess", instance.GetFloat("_Shininess"));
			if(instance.HasProperty("_MainTex"))
			{
				writer.WriteProperty<Texture>("_MainTex", instance.GetTexture("_MainTex"));
				writer.WriteProperty<Vector2>("_MainTex_Scale", instance.GetTextureScale("_MainTex"));
			}
			if(instance.HasProperty("_MainTex_TextureOffset"))
				writer.WriteProperty("_MainTex_TextureOffset", instance.GetTextureOffset("_MainTex_TextureOffset"));
			if(instance.HasProperty("_MainTex_TextureScale"))
				writer.WriteProperty("_MainTex_TextureScale", instance.GetTextureScale("_MainTex_TextureScale"));
			if(instance.HasProperty("_Illum"))
				writer.WriteProperty<Texture>("_Illum", instance.GetTexture("_Illum"));
			if(instance.HasProperty("_Illum_TextureOffset"))
				writer.WriteProperty("_Illum_TextureOffset", instance.GetTextureOffset("_Illum_TextureOffset"));
			if(instance.HasProperty("_Illum_TextureScale"))
				writer.WriteProperty("_Illum_TextureScale", instance.GetTextureScale("_Illum_TextureScale"));
			if(instance.HasProperty("_BumpMap"))
				writer.WriteProperty<Texture>("_BumpMap", instance.GetTexture("_BumpMap"));
			if(instance.HasProperty("_BumpMap_TextureOffset"))
				writer.WriteProperty("_BumpMap_TextureOffset", instance.GetTextureOffset("_BumpMap_TextureOffset"));
			if(instance.HasProperty("_BumpMap_TextureScale"))
				writer.WriteProperty("_BumpMap_TextureScale", instance.GetTextureScale("_BumpMap_TextureScale"));
			if(instance.HasProperty("_Emission"))
				writer.WriteProperty("_Emission", instance.GetFloat("_Emission"));
			if(instance.HasProperty("_Specular"))
				writer.WriteProperty("_Specular", instance.GetColor("_Specular"));
			if(instance.HasProperty("_MainBump"))
				writer.WriteProperty<Texture>("_MainBump", instance.GetTexture("_MainBump"));
			if(instance.HasProperty("_MainBump_TextureOffset"))
				writer.WriteProperty("_MainBump_TextureOffset", instance.GetTextureOffset("_MainBump_TextureOffset"));
			if(instance.HasProperty("_MainBump_TextureScale"))
				writer.WriteProperty("_MainBump_TextureScale", instance.GetTextureScale("_MainBump_TextureScale"));
			if(instance.HasProperty("_Mask"))
				writer.WriteProperty<Texture>("_Mask", instance.GetTexture("_Mask"));
			if(instance.HasProperty("_Mask_TextureOffset"))
				writer.WriteProperty("_Mask_TextureOffset", instance.GetTextureOffset("_Mask_TextureOffset"));
			if(instance.HasProperty("_Mask_TextureScale"))
				writer.WriteProperty("_Mask_TextureScale", instance.GetTextureScale("_Mask_TextureScale"));
			if(instance.HasProperty("_Focus"))
				writer.WriteProperty("_Focus", instance.GetFloat("_Focus"));
			if(instance.HasProperty("_StencilComp"))
				writer.WriteProperty("_StencilComp", instance.GetFloat("_StencilComp"));
			if(instance.HasProperty("_Stencil"))
				writer.WriteProperty("_Stencil", instance.GetFloat("_Stencil"));
			if(instance.HasProperty("_StencilOp"))
				writer.WriteProperty("_StencilOp", instance.GetFloat("_StencilOp"));
			if(instance.HasProperty("_StencilWriteMask"))
				writer.WriteProperty("_StencilWriteMask", instance.GetFloat("_StencilWriteMask"));
			if(instance.HasProperty("_StencilReadMask"))
				writer.WriteProperty("_StencilReadMask", instance.GetFloat("_StencilReadMask"));
			if(instance.HasProperty("_ColorMask"))
				writer.WriteProperty("_ColorMask", instance.GetFloat("_ColorMask"));
			if(instance.HasProperty("_UseUIAlphaClip"))
				writer.WriteProperty("_UseUIAlphaClip", instance.GetFloat("_UseUIAlphaClip"));
			if(instance.HasProperty("_SrcBlend"))
				writer.WriteProperty("_SrcBlend", instance.GetFloat("_SrcBlend"));
			if(instance.HasProperty("_DstBlend"))
				writer.WriteProperty("_DstBlend", instance.GetFloat("_DstBlend"));
			if(instance.HasProperty("_ReflectColor"))
				writer.WriteProperty("_ReflectColor", instance.GetColor("_ReflectColor"));
			if(instance.HasProperty("_Cube"))
				writer.WriteProperty<Texture>("_Cube", instance.GetTexture("_Cube"));
			if(instance.HasProperty("_Cube_TextureOffset"))
				writer.WriteProperty("_Cube_TextureOffset", instance.GetTextureOffset("_Cube_TextureOffset"));
			if(instance.HasProperty("_Cube_TextureScale"))
				writer.WriteProperty("_Cube_TextureScale", instance.GetTextureScale("_Cube_TextureScale"));
			if(instance.HasProperty("_Tint"))
				writer.WriteProperty("_Tint", instance.GetColor("_Tint"));
			if(instance.HasProperty("_Exposure"))
				writer.WriteProperty("_Exposure", instance.GetFloat("_Exposure"));
			if(instance.HasProperty("_Rotation"))
				writer.WriteProperty("_Rotation", instance.GetFloat("_Rotation"));
			if(instance.HasProperty("_Tex"))
				writer.WriteProperty<Texture>("_Tex", instance.GetTexture("_Tex"));
			if(instance.HasProperty("_Tex_TextureOffset"))
				writer.WriteProperty("_Tex_TextureOffset", instance.GetTextureOffset("_Tex_TextureOffset"));
			if(instance.HasProperty("_Tex_TextureScale"))
				writer.WriteProperty("_Tex_TextureScale", instance.GetTextureScale("_Tex_TextureScale"));
			if(instance.HasProperty("_Control"))
				writer.WriteProperty<Texture>("_Control", instance.GetTexture("_Control"));
			if(instance.HasProperty("_Control_TextureOffset"))
				writer.WriteProperty("_Control_TextureOffset", instance.GetTextureOffset("_Control_TextureOffset"));
			if(instance.HasProperty("_Control_TextureScale"))
				writer.WriteProperty("_Control_TextureScale", instance.GetTextureScale("_Control_TextureScale"));
			if(instance.HasProperty("_Splat3"))
				writer.WriteProperty<Texture>("_Splat3", instance.GetTexture("_Splat3"));
			if(instance.HasProperty("_Splat3_TextureOffset"))
				writer.WriteProperty("_Splat3_TextureOffset", instance.GetTextureOffset("_Splat3_TextureOffset"));
			if(instance.HasProperty("_Splat3_TextureScale"))
				writer.WriteProperty("_Splat3_TextureScale", instance.GetTextureScale("_Splat3_TextureScale"));
			if(instance.HasProperty("_Splat2"))
				writer.WriteProperty<Texture>("_Splat2", instance.GetTexture("_Splat2"));
			if(instance.HasProperty("_Splat2_TextureOffset"))
				writer.WriteProperty("_Splat2_TextureOffset", instance.GetTextureOffset("_Splat2_TextureOffset"));
			if(instance.HasProperty("_Splat2_TextureScale"))
				writer.WriteProperty("_Splat2_TextureScale", instance.GetTextureScale("_Splat2_TextureScale"));
			if(instance.HasProperty("_Splat1"))
				writer.WriteProperty<Texture>("_Splat1", instance.GetTexture("_Splat1"));
			if(instance.HasProperty("_Splat1_TextureOffset"))
				writer.WriteProperty("_Splat1_TextureOffset", instance.GetTextureOffset("_Splat1_TextureOffset"));
			if(instance.HasProperty("_Splat1_TextureScale"))
				writer.WriteProperty("_Splat1_TextureScale", instance.GetTextureScale("_Splat1_TextureScale"));
			if(instance.HasProperty("_Splat0"))
				writer.WriteProperty<Texture>("_Splat0", instance.GetTexture("_Splat0"));
			if(instance.HasProperty("_Splat0_TextureOffset"))
				writer.WriteProperty("_Splat0_TextureOffset", instance.GetTextureOffset("_Splat0_TextureOffset"));
			if(instance.HasProperty("_Splat0_TextureScale"))
				writer.WriteProperty("_Splat0_TextureScale", instance.GetTextureScale("_Splat0_TextureScale"));
			if(instance.HasProperty("_Normal3"))
				writer.WriteProperty<Texture>("_Normal3", instance.GetTexture("_Normal3"));
			if(instance.HasProperty("_Normal3_TextureOffset"))
				writer.WriteProperty("_Normal3_TextureOffset", instance.GetTextureOffset("_Normal3_TextureOffset"));
			if(instance.HasProperty("_Normal3_TextureScale"))
				writer.WriteProperty("_Normal3_TextureScale", instance.GetTextureScale("_Normal3_TextureScale"));
			if(instance.HasProperty("_Normal2"))
				writer.WriteProperty<Texture>("_Normal2", instance.GetTexture("_Normal2"));
			if(instance.HasProperty("_Normal2_TextureOffset"))
				writer.WriteProperty("_Normal2_TextureOffset", instance.GetTextureOffset("_Normal2_TextureOffset"));
			if(instance.HasProperty("_Normal2_TextureScale"))
				writer.WriteProperty("_Normal2_TextureScale", instance.GetTextureScale("_Normal2_TextureScale"));
			if(instance.HasProperty("_Normal1"))
				writer.WriteProperty<Texture>("_Normal1", instance.GetTexture("_Normal1"));
			if(instance.HasProperty("_Normal1_TextureOffset"))
				writer.WriteProperty("_Normal1_TextureOffset", instance.GetTextureOffset("_Normal1_TextureOffset"));
			if(instance.HasProperty("_Normal1_TextureScale"))
				writer.WriteProperty("_Normal1_TextureScale", instance.GetTextureScale("_Normal1_TextureScale"));
			if(instance.HasProperty("_Normal0"))
				writer.WriteProperty<Texture>("_Normal0", instance.GetTexture("_Normal0"));
			if(instance.HasProperty("_Normal0_TextureOffset"))
				writer.WriteProperty("_Normal0_TextureOffset", instance.GetTextureOffset("_Normal0_TextureOffset"));
			if(instance.HasProperty("_Normal0_TextureScale"))
				writer.WriteProperty("_Normal0_TextureScale", instance.GetTextureScale("_Normal0_TextureScale"));
			if(instance.HasProperty("_Cutoff"))
				writer.WriteProperty("_Cutoff", instance.GetFloat("_Cutoff"));
			if(instance.HasProperty("_BaseLight"))
				writer.WriteProperty("_BaseLight", instance.GetFloat("_BaseLight"));
			if(instance.HasProperty("_AO"))
				writer.WriteProperty("_AO", instance.GetFloat("_AO"));
			if(instance.HasProperty("_Occlusion"))
				writer.WriteProperty("_Occlusion", instance.GetFloat("_Occlusion"));
			if(instance.HasProperty("_TreeInstanceColor"))
				writer.WriteProperty("_TreeInstanceColor", instance.GetVector("_TreeInstanceColor"));
			if(instance.HasProperty("_TreeInstanceScale"))
				writer.WriteProperty("_TreeInstanceScale", instance.GetVector("_TreeInstanceScale"));
			if(instance.HasProperty("_SquashAmount"))
				writer.WriteProperty("_SquashAmount", instance.GetFloat("_SquashAmount"));
			if(instance.HasProperty("_TranslucencyColor"))
				writer.WriteProperty("_TranslucencyColor", instance.GetColor("_TranslucencyColor"));
			if(instance.HasProperty("_TranslucencyViewDependency"))
				writer.WriteProperty("_TranslucencyViewDependency", instance.GetFloat("_TranslucencyViewDependency"));
			if(instance.HasProperty("_ShadowStrength"))
				writer.WriteProperty("_ShadowStrength", instance.GetFloat("_ShadowStrength"));
			if(instance.HasProperty("_ShadowOffsetScale"))
				writer.WriteProperty("_ShadowOffsetScale", instance.GetFloat("_ShadowOffsetScale"));
			if(instance.HasProperty("_ShadowTex"))
				writer.WriteProperty<Texture>("_ShadowTex", instance.GetTexture("_ShadowTex"));
			if(instance.HasProperty("_ShadowTex_TextureOffset"))
				writer.WriteProperty("_ShadowTex_TextureOffset", instance.GetTextureOffset("_ShadowTex_TextureOffset"));
			if(instance.HasProperty("_ShadowTex_TextureScale"))
				writer.WriteProperty("_ShadowTex_TextureScale", instance.GetTextureScale("_ShadowTex_TextureScale"));
			if(instance.HasProperty("_BumpSpecMap"))
				writer.WriteProperty<Texture>("_BumpSpecMap", instance.GetTexture("_BumpSpecMap"));
			if(instance.HasProperty("_BumpSpecMap_TextureOffset"))
				writer.WriteProperty("_BumpSpecMap_TextureOffset", instance.GetTextureOffset("_BumpSpecMap_TextureOffset"));
			if(instance.HasProperty("_BumpSpecMap_TextureScale"))
				writer.WriteProperty("_BumpSpecMap_TextureScale", instance.GetTextureScale("_BumpSpecMap_TextureScale"));
			if(instance.HasProperty("_TranslucencyMap"))
				writer.WriteProperty<Texture>("_TranslucencyMap", instance.GetTexture("_TranslucencyMap"));
			if(instance.HasProperty("_TranslucencyMap_TextureOffset"))
				writer.WriteProperty("_TranslucencyMap_TextureOffset", instance.GetTextureOffset("_TranslucencyMap_TextureOffset"));
			if(instance.HasProperty("_TranslucencyMap_TextureScale"))
				writer.WriteProperty("_TranslucencyMap_TextureScale", instance.GetTextureScale("_TranslucencyMap_TextureScale"));
			if(instance.HasProperty("_LightMap"))
				writer.WriteProperty<Texture>("_LightMap", instance.GetTexture("_LightMap"));
			if(instance.HasProperty("_LightMap_TextureOffset"))
				writer.WriteProperty("_LightMap_TextureOffset", instance.GetTextureOffset("_LightMap_TextureOffset"));
			if(instance.HasProperty("_LightMap_TextureScale"))
				writer.WriteProperty("_LightMap_TextureScale", instance.GetTextureScale("_LightMap_TextureScale"));
			if(instance.HasProperty("_DetailTex"))
				writer.WriteProperty<Texture>("_DetailTex", instance.GetTexture("_DetailTex"));
			if(instance.HasProperty("_DetailTex_TextureOffset"))
				writer.WriteProperty("_DetailTex_TextureOffset", instance.GetTextureOffset("_DetailTex_TextureOffset"));
			if(instance.HasProperty("_DetailTex_TextureScale"))
				writer.WriteProperty("_DetailTex_TextureScale", instance.GetTextureScale("_DetailTex_TextureScale"));
			if(instance.HasProperty("_DetailBump"))
				writer.WriteProperty<Texture>("_DetailBump", instance.GetTexture("_DetailBump"));
			if(instance.HasProperty("_DetailBump_TextureOffset"))
				writer.WriteProperty("_DetailBump_TextureOffset", instance.GetTextureOffset("_DetailBump_TextureOffset"));
			if(instance.HasProperty("_DetailBump_TextureScale"))
				writer.WriteProperty("_DetailBump_TextureScale", instance.GetTextureScale("_DetailBump_TextureScale"));
			if(instance.HasProperty("_Strength"))
				writer.WriteProperty("_Strength", instance.GetFloat("_Strength"));
			if(instance.HasProperty("_InvFade"))
				writer.WriteProperty("_InvFade", instance.GetFloat("_InvFade"));
			if(instance.HasProperty("_EmisColor"))
				writer.WriteProperty("_EmisColor", instance.GetColor("_EmisColor"));
			if(instance.HasProperty("_Parallax"))
				writer.WriteProperty("_Parallax", instance.GetFloat("_Parallax"));
			if(instance.HasProperty("_ParallaxMap"))
				writer.WriteProperty<Texture>("_ParallaxMap", instance.GetTexture("_ParallaxMap"));
			if(instance.HasProperty("_ParallaxMap_TextureOffset"))
				writer.WriteProperty("_ParallaxMap_TextureOffset", instance.GetTextureOffset("_ParallaxMap_TextureOffset"));
			if(instance.HasProperty("_ParallaxMap_TextureScale"))
				writer.WriteProperty("_ParallaxMap_TextureScale", instance.GetTextureScale("_ParallaxMap_TextureScale"));
			if(instance.HasProperty("_DecalTex"))
				writer.WriteProperty<Texture>("_DecalTex", instance.GetTexture("_DecalTex"));
			if(instance.HasProperty("_DecalTex_TextureOffset"))
				writer.WriteProperty("_DecalTex_TextureOffset", instance.GetTextureOffset("_DecalTex_TextureOffset"));
			if(instance.HasProperty("_DecalTex_TextureScale"))
				writer.WriteProperty("_DecalTex_TextureScale", instance.GetTextureScale("_DecalTex_TextureScale"));
			if(instance.HasProperty("_GlossMap"))
				writer.WriteProperty<Texture>("_GlossMap", instance.GetTexture("_GlossMap"));
			if(instance.HasProperty("_GlossMap_TextureOffset"))
				writer.WriteProperty("_GlossMap_TextureOffset", instance.GetTextureOffset("_GlossMap_TextureOffset"));
			if(instance.HasProperty("_GlossMap_TextureScale"))
				writer.WriteProperty("_GlossMap_TextureScale", instance.GetTextureScale("_GlossMap_TextureScale"));
			if(instance.HasProperty("_ShadowOffset"))
				writer.WriteProperty<Texture>("_ShadowOffset", instance.GetTexture("_ShadowOffset"));
			if(instance.HasProperty("_ShadowOffset_TextureOffset"))
				writer.WriteProperty("_ShadowOffset_TextureOffset", instance.GetTextureOffset("_ShadowOffset_TextureOffset"));
			if(instance.HasProperty("_ShadowOffset_TextureScale"))
				writer.WriteProperty("_ShadowOffset_TextureScale", instance.GetTextureScale("_ShadowOffset_TextureScale"));
			if(instance.HasProperty("_SunDisk"))
				writer.WriteProperty("_SunDisk", instance.GetFloat("_SunDisk"));
			if(instance.HasProperty("_SunSize"))
				writer.WriteProperty("_SunSize", instance.GetFloat("_SunSize"));
			if(instance.HasProperty("_AtmosphereThickness"))
				writer.WriteProperty("_AtmosphereThickness", instance.GetFloat("_AtmosphereThickness"));
			if(instance.HasProperty("_SkyTint"))
				writer.WriteProperty("_SkyTint", instance.GetColor("_SkyTint"));
			if(instance.HasProperty("_GroundColor"))
				writer.WriteProperty("_GroundColor", instance.GetColor("_GroundColor"));
			if(instance.HasProperty("_WireThickness"))
				writer.WriteProperty("_WireThickness", instance.GetFloat("_WireThickness"));
			if(instance.HasProperty("_ZWrite"))
				writer.WriteProperty("_ZWrite", instance.GetFloat("_ZWrite"));
			if(instance.HasProperty("_ZTest"))
				writer.WriteProperty("_ZTest", instance.GetFloat("_ZTest"));
			if(instance.HasProperty("_Cull"))
				writer.WriteProperty("_Cull", instance.GetFloat("_Cull"));
			if(instance.HasProperty("_ZBias"))
				writer.WriteProperty("_ZBias", instance.GetFloat("_ZBias"));
			if(instance.HasProperty("_HueVariation"))
				writer.WriteProperty("_HueVariation", instance.GetColor("_HueVariation"));
			if(instance.HasProperty("_WindQuality"))
				writer.WriteProperty("_WindQuality", instance.GetFloat("_WindQuality"));
			if(instance.HasProperty("_DetailMask"))
				writer.WriteProperty<Texture>("_DetailMask", instance.GetTexture("_DetailMask"));
			if(instance.HasProperty("_DetailMask_TextureOffset"))
				writer.WriteProperty("_DetailMask_TextureOffset", instance.GetTextureOffset("_DetailMask_TextureOffset"));
			if(instance.HasProperty("_DetailMask_TextureScale"))
				writer.WriteProperty("_DetailMask_TextureScale", instance.GetTextureScale("_DetailMask_TextureScale"));
			if(instance.HasProperty("_MetallicTex"))
				writer.WriteProperty<Texture>("_MetallicTex", instance.GetTexture("_MetallicTex"));
			if(instance.HasProperty("_MetallicTex_TextureOffset"))
				writer.WriteProperty("_MetallicTex_TextureOffset", instance.GetTextureOffset("_MetallicTex_TextureOffset"));
			if(instance.HasProperty("_MetallicTex_TextureScale"))
				writer.WriteProperty("_MetallicTex_TextureScale", instance.GetTextureScale("_MetallicTex_TextureScale"));
			if(instance.HasProperty("_Glossiness"))
				writer.WriteProperty("_Glossiness", instance.GetFloat("_Glossiness"));
			if(instance.HasProperty("_GlossMapScale"))
				writer.WriteProperty("_GlossMapScale", instance.GetFloat("_GlossMapScale"));
			if(instance.HasProperty("_SmoothnessTextureChannel"))
				writer.WriteProperty("_SmoothnessTextureChannel", instance.GetFloat("_SmoothnessTextureChannel"));
			if(instance.HasProperty("_Metallic"))
				writer.WriteProperty("_Metallic", instance.GetFloat("_Metallic"));
			if(instance.HasProperty("_MetallicGlossMap"))
				writer.WriteProperty<Texture>("_MetallicGlossMap", instance.GetTexture("_MetallicGlossMap"));
			if(instance.HasProperty("_MetallicGlossMap_TextureOffset"))
				writer.WriteProperty("_MetallicGlossMap_TextureOffset", instance.GetTextureOffset("_MetallicGlossMap_TextureOffset"));
			if(instance.HasProperty("_MetallicGlossMap_TextureScale"))
				writer.WriteProperty("_MetallicGlossMap_TextureScale", instance.GetTextureScale("_MetallicGlossMap_TextureScale"));
			if(instance.HasProperty("_SpecularHighlights"))
				writer.WriteProperty("_SpecularHighlights", instance.GetFloat("_SpecularHighlights"));
			if(instance.HasProperty("_GlossyReflections"))
				writer.WriteProperty("_GlossyReflections", instance.GetFloat("_GlossyReflections"));
			if(instance.HasProperty("_BumpScale"))
				writer.WriteProperty("_BumpScale", instance.GetFloat("_BumpScale"));
			if(instance.HasProperty("_OcclusionStrength"))
				writer.WriteProperty("_OcclusionStrength", instance.GetFloat("_OcclusionStrength"));
			if(instance.HasProperty("_OcclusionMap"))
				writer.WriteProperty<Texture>("_OcclusionMap", instance.GetTexture("_OcclusionMap"));
			if(instance.HasProperty("_OcclusionMap_TextureOffset"))
				writer.WriteProperty("_OcclusionMap_TextureOffset", instance.GetTextureOffset("_OcclusionMap_TextureOffset"));
			if(instance.HasProperty("_OcclusionMap_TextureScale"))
				writer.WriteProperty("_OcclusionMap_TextureScale", instance.GetTextureScale("_OcclusionMap_TextureScale"));
			if(instance.HasProperty("_EmissionColor"))
				writer.WriteProperty("_EmissionColor", instance.GetColor("_EmissionColor"));
			if(instance.HasProperty("_EmissionMap"))
				writer.WriteProperty<Texture>("_EmissionMap", instance.GetTexture("_EmissionMap"));
			if(instance.HasProperty("_EmissionMap_TextureOffset"))
				writer.WriteProperty("_EmissionMap_TextureOffset", instance.GetTextureOffset("_EmissionMap_TextureOffset"));
			if(instance.HasProperty("_EmissionMap_TextureScale"))
				writer.WriteProperty("_EmissionMap_TextureScale", instance.GetTextureScale("_EmissionMap_TextureScale"));
			if(instance.HasProperty("_DetailAlbedoMap"))
				writer.WriteProperty<Texture>("_DetailAlbedoMap", instance.GetTexture("_DetailAlbedoMap"));
			if(instance.HasProperty("_DetailAlbedoMap_TextureOffset"))
				writer.WriteProperty("_DetailAlbedoMap_TextureOffset", instance.GetTextureOffset("_DetailAlbedoMap_TextureOffset"));
			if(instance.HasProperty("_DetailAlbedoMap_TextureScale"))
				writer.WriteProperty("_DetailAlbedoMap_TextureScale", instance.GetTextureScale("_DetailAlbedoMap_TextureScale"));
			if(instance.HasProperty("_DetailNormalMapScale"))
				writer.WriteProperty("_DetailNormalMapScale", instance.GetFloat("_DetailNormalMapScale"));
			if(instance.HasProperty("_DetailNormalMap"))
				writer.WriteProperty<Texture>("_DetailNormalMap", instance.GetTexture("_DetailNormalMap"));
			if(instance.HasProperty("_DetailNormalMap_TextureOffset"))
				writer.WriteProperty("_DetailNormalMap_TextureOffset", instance.GetTextureOffset("_DetailNormalMap_TextureOffset"));
			if(instance.HasProperty("_DetailNormalMap_TextureScale"))
				writer.WriteProperty("_DetailNormalMap_TextureScale", instance.GetTextureScale("_DetailNormalMap_TextureScale"));
			if(instance.HasProperty("_UVSec"))
				writer.WriteProperty("_UVSec", instance.GetFloat("_UVSec"));
			if(instance.HasProperty("_Mode"))
				writer.WriteProperty("_Mode", instance.GetFloat("_Mode"));
			if(instance.HasProperty("_TintColor"))
				writer.WriteProperty("_TintColor", instance.GetColor("_TintColor"));
			if(instance.HasProperty("_WavingTint"))
				writer.WriteProperty("_WavingTint", instance.GetColor("_WavingTint"));
			if(instance.HasProperty("_WaveAndDistance"))
				writer.WriteProperty("_WaveAndDistance", instance.GetVector("_WaveAndDistance"));
			if(instance.HasProperty("_LightTexture0"))
				writer.WriteProperty<Texture>("_LightTexture0", instance.GetTexture("_LightTexture0"));
			if(instance.HasProperty("_LightTexture0_TextureOffset"))
				writer.WriteProperty("_LightTexture0_TextureOffset", instance.GetTextureOffset("_LightTexture0_TextureOffset"));
			if(instance.HasProperty("_LightTexture0_TextureScale"))
				writer.WriteProperty("_LightTexture0_TextureScale", instance.GetTextureScale("_LightTexture0_TextureScale"));
			if(instance.HasProperty("_LightTextureB0"))
				writer.WriteProperty<Texture>("_LightTextureB0", instance.GetTexture("_LightTextureB0"));
			if(instance.HasProperty("_LightTextureB0_TextureOffset"))
				writer.WriteProperty("_LightTextureB0_TextureOffset", instance.GetTextureOffset("_LightTextureB0_TextureOffset"));
			if(instance.HasProperty("_LightTextureB0_TextureScale"))
				writer.WriteProperty("_LightTextureB0_TextureScale", instance.GetTextureScale("_LightTextureB0_TextureScale"));
			if(instance.HasProperty("_ShadowMapTexture"))
				writer.WriteProperty<Texture>("_ShadowMapTexture", instance.GetTexture("_ShadowMapTexture"));
			if(instance.HasProperty("_ShadowMapTexture_TextureOffset"))
				writer.WriteProperty("_ShadowMapTexture_TextureOffset", instance.GetTextureOffset("_ShadowMapTexture_TextureOffset"));
			if(instance.HasProperty("_ShadowMapTexture_TextureScale"))
				writer.WriteProperty("_ShadowMapTexture_TextureScale", instance.GetTextureScale("_ShadowMapTexture_TextureScale"));
			if(instance.HasProperty("_SecondTex"))
				writer.WriteProperty<Texture>("_SecondTex", instance.GetTexture("_SecondTex"));
			if(instance.HasProperty("_SecondTex_TextureOffset"))
				writer.WriteProperty("_SecondTex_TextureOffset", instance.GetTextureOffset("_SecondTex_TextureOffset"));
			if(instance.HasProperty("_SecondTex_TextureScale"))
				writer.WriteProperty("_SecondTex_TextureScale", instance.GetTextureScale("_SecondTex_TextureScale"));
			if(instance.HasProperty("_ThirdTex"))
				writer.WriteProperty<Texture>("_ThirdTex", instance.GetTexture("_ThirdTex"));
			if(instance.HasProperty("_ThirdTex_TextureOffset"))
				writer.WriteProperty("_ThirdTex_TextureOffset", instance.GetTextureOffset("_ThirdTex_TextureOffset"));
			if(instance.HasProperty("_ThirdTex_TextureScale"))
				writer.WriteProperty("_ThirdTex_TextureScale", instance.GetTextureScale("_ThirdTex_TextureScale"));
			if(instance.HasProperty("PixelSnap"))
				writer.WriteProperty("PixelSnap", instance.GetFloat("PixelSnap"));
			if(instance.HasProperty("_RendererColor"))
				writer.WriteProperty("_RendererColor", instance.GetColor("_RendererColor"));
			if(instance.HasProperty("_Flip"))
				writer.WriteProperty("_Flip", instance.GetVector("_Flip"));
			if(instance.HasProperty("_AlphaTex"))
				writer.WriteProperty<Texture>("_AlphaTex", instance.GetTexture("_AlphaTex"));
			if(instance.HasProperty("_AlphaTex_TextureOffset"))
				writer.WriteProperty("_AlphaTex_TextureOffset", instance.GetTextureOffset("_AlphaTex_TextureOffset"));
			if(instance.HasProperty("_AlphaTex_TextureScale"))
				writer.WriteProperty("_AlphaTex_TextureScale", instance.GetTextureScale("_AlphaTex_TextureScale"));
			if(instance.HasProperty("_EnableExternalAlpha"))
				writer.WriteProperty("_EnableExternalAlpha", instance.GetFloat("_EnableExternalAlpha"));
			if(instance.HasProperty("_Level"))
				writer.WriteProperty("_Level", instance.GetFloat("_Level"));
			if(instance.HasProperty("_SpecGlossMap"))
				writer.WriteProperty<Texture>("_SpecGlossMap", instance.GetTexture("_SpecGlossMap"));
			if(instance.HasProperty("_SpecGlossMap_TextureOffset"))
				writer.WriteProperty("_SpecGlossMap_TextureOffset", instance.GetTextureOffset("_SpecGlossMap_TextureOffset"));
			if(instance.HasProperty("_SpecGlossMap_TextureScale"))
				writer.WriteProperty("_SpecGlossMap_TextureScale", instance.GetTextureScale("_SpecGlossMap_TextureScale"));
			if(instance.HasProperty("_FrontTex"))
				writer.WriteProperty<Texture>("_FrontTex", instance.GetTexture("_FrontTex"));
			if(instance.HasProperty("_FrontTex_TextureOffset"))
				writer.WriteProperty("_FrontTex_TextureOffset", instance.GetTextureOffset("_FrontTex_TextureOffset"));
			if(instance.HasProperty("_FrontTex_TextureScale"))
				writer.WriteProperty("_FrontTex_TextureScale", instance.GetTextureScale("_FrontTex_TextureScale"));
			if(instance.HasProperty("_BackTex"))
				writer.WriteProperty<Texture>("_BackTex", instance.GetTexture("_BackTex"));
			if(instance.HasProperty("_BackTex_TextureOffset"))
				writer.WriteProperty("_BackTex_TextureOffset", instance.GetTextureOffset("_BackTex_TextureOffset"));
			if(instance.HasProperty("_BackTex_TextureScale"))
				writer.WriteProperty("_BackTex_TextureScale", instance.GetTextureScale("_BackTex_TextureScale"));
			if(instance.HasProperty("_LeftTex"))
				writer.WriteProperty<Texture>("_LeftTex", instance.GetTexture("_LeftTex"));
			if(instance.HasProperty("_LeftTex_TextureOffset"))
				writer.WriteProperty("_LeftTex_TextureOffset", instance.GetTextureOffset("_LeftTex_TextureOffset"));
			if(instance.HasProperty("_LeftTex_TextureScale"))
				writer.WriteProperty("_LeftTex_TextureScale", instance.GetTextureScale("_LeftTex_TextureScale"));
			if(instance.HasProperty("_RightTex"))
				writer.WriteProperty<Texture>("_RightTex", instance.GetTexture("_RightTex"));
			if(instance.HasProperty("_RightTex_TextureOffset"))
				writer.WriteProperty("_RightTex_TextureOffset", instance.GetTextureOffset("_RightTex_TextureOffset"));
			if(instance.HasProperty("_RightTex_TextureScale"))
				writer.WriteProperty("_RightTex_TextureScale", instance.GetTextureScale("_RightTex_TextureScale"));
			if(instance.HasProperty("_UpTex"))
				writer.WriteProperty<Texture>("_UpTex", instance.GetTexture("_UpTex"));
			if(instance.HasProperty("_UpTex_TextureOffset"))
				writer.WriteProperty("_UpTex_TextureOffset", instance.GetTextureOffset("_UpTex_TextureOffset"));
			if(instance.HasProperty("_UpTex_TextureScale"))
				writer.WriteProperty("_UpTex_TextureScale", instance.GetTextureScale("_UpTex_TextureScale"));
			if(instance.HasProperty("_DownTex"))
				writer.WriteProperty<Texture>("_DownTex", instance.GetTexture("_DownTex"));
			if(instance.HasProperty("_DownTex_TextureOffset"))
				writer.WriteProperty("_DownTex_TextureOffset", instance.GetTextureOffset("_DownTex_TextureOffset"));
			if(instance.HasProperty("_DownTex_TextureScale"))
				writer.WriteProperty("_DownTex_TextureScale", instance.GetTextureScale("_DownTex_TextureScale"));
			if(instance.HasProperty("_Metallic0"))
				writer.WriteProperty("_Metallic0", instance.GetFloat("_Metallic0"));
			if(instance.HasProperty("_Metallic1"))
				writer.WriteProperty("_Metallic1", instance.GetFloat("_Metallic1"));
			if(instance.HasProperty("_Metallic2"))
				writer.WriteProperty("_Metallic2", instance.GetFloat("_Metallic2"));
			if(instance.HasProperty("_Metallic3"))
				writer.WriteProperty("_Metallic3", instance.GetFloat("_Metallic3"));
			if(instance.HasProperty("_Smoothness0"))
				writer.WriteProperty("_Smoothness0", instance.GetFloat("_Smoothness0"));
			if(instance.HasProperty("_Smoothness1"))
				writer.WriteProperty("_Smoothness1", instance.GetFloat("_Smoothness1"));
			if(instance.HasProperty("_Smoothness2"))
				writer.WriteProperty("_Smoothness2", instance.GetFloat("_Smoothness2"));
			if(instance.HasProperty("_Smoothness3"))
				writer.WriteProperty("_Smoothness3", instance.GetFloat("_Smoothness3"));
			if(instance.HasProperty("_TexA"))
				writer.WriteProperty<Texture>("_TexA", instance.GetTexture("_TexA"));
			if(instance.HasProperty("_TexA_TextureOffset"))
				writer.WriteProperty("_TexA_TextureOffset", instance.GetTextureOffset("_TexA_TextureOffset"));
			if(instance.HasProperty("_TexA_TextureScale"))
				writer.WriteProperty("_TexA_TextureScale", instance.GetTextureScale("_TexA_TextureScale"));
			if(instance.HasProperty("_TexB"))
				writer.WriteProperty<Texture>("_TexB", instance.GetTexture("_TexB"));
			if(instance.HasProperty("_TexB_TextureOffset"))
				writer.WriteProperty("_TexB_TextureOffset", instance.GetTextureOffset("_TexB_TextureOffset"));
			if(instance.HasProperty("_TexB_TextureScale"))
				writer.WriteProperty("_TexB_TextureScale", instance.GetTextureScale("_TexB_TextureScale"));
			if(instance.HasProperty("_value"))
				writer.WriteProperty("_value", instance.GetFloat("_value"));
			if(instance.HasProperty("_Texel"))
				writer.WriteProperty("_Texel", instance.GetFloat("_Texel"));
			if(instance.HasProperty("_Detail"))
				writer.WriteProperty<Texture>("_Detail", instance.GetTexture("_Detail"));
			if(instance.HasProperty("_Detail_TextureOffset"))
				writer.WriteProperty("_Detail_TextureOffset", instance.GetTextureOffset("_Detail_TextureOffset"));
			if(instance.HasProperty("_Detail_TextureScale"))
				writer.WriteProperty("_Detail_TextureScale", instance.GetTextureScale("_Detail_TextureScale"));
			if(instance.HasProperty("_HalfOverCutoff"))
				writer.WriteProperty("_HalfOverCutoff", instance.GetFloat("_HalfOverCutoff"));

		}

		protected override object ReadUnityObject<T>(ES3Reader reader)
		{
			var obj = new Material(Shader.Find("Diffuse"));
			ReadUnityObject<T>(reader, obj);
			return obj;
		}

		protected override void ReadUnityObject<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.Material)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					case "name":
						instance.name = reader.Read<string>(ES3Type_string.Instance);
						break;
					case "shader":
						instance.shader = reader.Read<UnityEngine.Shader>(ES3Type_Shader.Instance);
						break;
					case "renderQueue":
						instance.renderQueue = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "shaderKeywords":
						instance.shaderKeywords = reader.Read<System.String[]>();
						break;
					case "globalIlluminationFlags":
						instance.globalIlluminationFlags = reader.Read<UnityEngine.MaterialGlobalIlluminationFlags>();
						break;
				    case "_Color":
					    instance.SetColor("_Color", reader.Read<Color>());
					    break;
				    case "_SpecColor":
					    instance.SetColor("_SpecColor", reader.Read<Color>());
					    break;
				    case "_Shininess":
					    instance.SetFloat("_Shininess", reader.Read<float>());
					    break;
				    case "_MainTex":
					    instance.SetTexture("_MainTex", reader.Read<Texture>());
					    break;
				    case "_MainTex_TextureOffset":
					    instance.SetTextureOffset("_MainTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_MainTex_TextureScale":
					    instance.SetTextureScale("_MainTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Illum":
					    instance.SetTexture("_Illum", reader.Read<Texture>());
					    break;
				    case "_Illum_TextureOffset":
					    instance.SetTextureOffset("_Illum_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Illum_TextureScale":
					    instance.SetTextureScale("_Illum_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_BumpMap":
					    instance.SetTexture("_BumpMap", reader.Read<Texture>());
					    break;
				    case "_BumpMap_TextureOffset":
					    instance.SetTextureOffset("_BumpMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_BumpMap_TextureScale":
					    instance.SetTextureScale("_BumpMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Emission":
					    instance.SetFloat("_Emission", reader.Read<float>());
					    break;
				    case "_Specular":
					    instance.SetColor("_Specular", reader.Read<Color>());
					    break;
				    case "_MainBump":
					    instance.SetTexture("_MainBump", reader.Read<Texture>());
					    break;
				    case "_MainBump_TextureOffset":
					    instance.SetTextureOffset("_MainBump_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_MainBump_TextureScale":
					    instance.SetTextureScale("_MainBump_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Mask":
					    instance.SetTexture("_Mask", reader.Read<Texture>());
					    break;
				    case "_Mask_TextureOffset":
					    instance.SetTextureOffset("_Mask_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Mask_TextureScale":
					    instance.SetTextureScale("_Mask_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Focus":
					    instance.SetFloat("_Focus", reader.Read<float>());
					    break;
				    case "_StencilComp":
					    instance.SetFloat("_StencilComp", reader.Read<float>());
					    break;
				    case "_Stencil":
					    instance.SetFloat("_Stencil", reader.Read<float>());
					    break;
				    case "_StencilOp":
					    instance.SetFloat("_StencilOp", reader.Read<float>());
					    break;
				    case "_StencilWriteMask":
					    instance.SetFloat("_StencilWriteMask", reader.Read<float>());
					    break;
				    case "_StencilReadMask":
					    instance.SetFloat("_StencilReadMask", reader.Read<float>());
					    break;
				    case "_ColorMask":
					    instance.SetFloat("_ColorMask", reader.Read<float>());
					    break;
				    case "_UseUIAlphaClip":
					    instance.SetFloat("_UseUIAlphaClip", reader.Read<float>());
					    break;
				    case "_SrcBlend":
					    instance.SetFloat("_SrcBlend", reader.Read<float>());
					    break;
				    case "_DstBlend":
					    instance.SetFloat("_DstBlend", reader.Read<float>());
					    break;
				    case "_ReflectColor":
					    instance.SetColor("_ReflectColor", reader.Read<Color>());
					    break;
				    case "_Cube":
					    instance.SetTexture("_Cube", reader.Read<Texture>());
					    break;
				    case "_Cube_TextureOffset":
					    instance.SetTextureOffset("_Cube_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Cube_TextureScale":
					    instance.SetTextureScale("_Cube_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Tint":
					    instance.SetColor("_Tint", reader.Read<Color>());
					    break;
				    case "_Exposure":
					    instance.SetFloat("_Exposure", reader.Read<float>());
					    break;
				    case "_Rotation":
					    instance.SetFloat("_Rotation", reader.Read<float>());
					    break;
				    case "_Tex":
					    instance.SetTexture("_Tex", reader.Read<Texture>());
					    break;
				    case "_Tex_TextureOffset":
					    instance.SetTextureOffset("_Tex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Tex_TextureScale":
					    instance.SetTextureScale("_Tex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_MainTex_Scale":
					    instance.SetTextureScale("_MainTex", reader.Read<Vector2>());
					    break;
				    case "_Control":
					    instance.SetTexture("_Control", reader.Read<Texture>());
					    break;
				    case "_Control_TextureOffset":
					    instance.SetTextureOffset("_Control_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Control_TextureScale":
					    instance.SetTextureScale("_Control_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Splat3":
					    instance.SetTexture("_Splat3", reader.Read<Texture>());
					    break;
				    case "_Splat3_TextureOffset":
					    instance.SetTextureOffset("_Splat3_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Splat3_TextureScale":
					    instance.SetTextureScale("_Splat3_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Splat2":
					    instance.SetTexture("_Splat2", reader.Read<Texture>());
					    break;
				    case "_Splat2_TextureOffset":
					    instance.SetTextureOffset("_Splat2_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Splat2_TextureScale":
					    instance.SetTextureScale("_Splat2_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Splat1":
					    instance.SetTexture("_Splat1", reader.Read<Texture>());
					    break;
				    case "_Splat1_TextureOffset":
					    instance.SetTextureOffset("_Splat1_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Splat1_TextureScale":
					    instance.SetTextureScale("_Splat1_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Splat0":
					    instance.SetTexture("_Splat0", reader.Read<Texture>());
					    break;
				    case "_Splat0_TextureOffset":
					    instance.SetTextureOffset("_Splat0_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Splat0_TextureScale":
					    instance.SetTextureScale("_Splat0_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Normal3":
					    instance.SetTexture("_Normal3", reader.Read<Texture>());
					    break;
				    case "_Normal3_TextureOffset":
					    instance.SetTextureOffset("_Normal3_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Normal3_TextureScale":
					    instance.SetTextureScale("_Normal3_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Normal2":
					    instance.SetTexture("_Normal2", reader.Read<Texture>());
					    break;
				    case "_Normal2_TextureOffset":
					    instance.SetTextureOffset("_Normal2_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Normal2_TextureScale":
					    instance.SetTextureScale("_Normal2_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Normal1":
					    instance.SetTexture("_Normal1", reader.Read<Texture>());
					    break;
				    case "_Normal1_TextureOffset":
					    instance.SetTextureOffset("_Normal1_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Normal1_TextureScale":
					    instance.SetTextureScale("_Normal1_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Normal0":
					    instance.SetTexture("_Normal0", reader.Read<Texture>());
					    break;
				    case "_Normal0_TextureOffset":
					    instance.SetTextureOffset("_Normal0_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Normal0_TextureScale":
					    instance.SetTextureScale("_Normal0_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Cutoff":
					    instance.SetFloat("_Cutoff", reader.Read<float>());
					    break;
				    case "_BaseLight":
					    instance.SetFloat("_BaseLight", reader.Read<float>());
					    break;
				    case "_AO":
					    instance.SetFloat("_AO", reader.Read<float>());
					    break;
				    case "_Occlusion":
					    instance.SetFloat("_Occlusion", reader.Read<float>());
					    break;
				    case "_TreeInstanceColor":
					    instance.SetVector("_TreeInstanceColor", reader.Read<Vector4>());
					    break;
				    case "_TreeInstanceScale":
					    instance.SetVector("_TreeInstanceScale", reader.Read<Vector4>());
					    break;
				    case "_SquashAmount":
					    instance.SetFloat("_SquashAmount", reader.Read<float>());
					    break;
				    case "_TranslucencyColor":
					    instance.SetColor("_TranslucencyColor", reader.Read<Color>());
					    break;
				    case "_TranslucencyViewDependency":
					    instance.SetFloat("_TranslucencyViewDependency", reader.Read<float>());
					    break;
				    case "_ShadowStrength":
					    instance.SetFloat("_ShadowStrength", reader.Read<float>());
					    break;
				    case "_ShadowOffsetScale":
					    instance.SetFloat("_ShadowOffsetScale", reader.Read<float>());
					    break;
				    case "_ShadowTex":
					    instance.SetTexture("_ShadowTex", reader.Read<Texture>());
					    break;
				    case "_ShadowTex_TextureOffset":
					    instance.SetTextureOffset("_ShadowTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_ShadowTex_TextureScale":
					    instance.SetTextureScale("_ShadowTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_BumpSpecMap":
					    instance.SetTexture("_BumpSpecMap", reader.Read<Texture>());
					    break;
				    case "_BumpSpecMap_TextureOffset":
					    instance.SetTextureOffset("_BumpSpecMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_BumpSpecMap_TextureScale":
					    instance.SetTextureScale("_BumpSpecMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_TranslucencyMap":
					    instance.SetTexture("_TranslucencyMap", reader.Read<Texture>());
					    break;
				    case "_TranslucencyMap_TextureOffset":
					    instance.SetTextureOffset("_TranslucencyMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_TranslucencyMap_TextureScale":
					    instance.SetTextureScale("_TranslucencyMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_LightMap":
					    instance.SetTexture("_LightMap", reader.Read<Texture>());
					    break;
				    case "_LightMap_TextureOffset":
					    instance.SetTextureOffset("_LightMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_LightMap_TextureScale":
					    instance.SetTextureScale("_LightMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_DetailTex":
					    instance.SetTexture("_DetailTex", reader.Read<Texture>());
					    break;
				    case "_DetailTex_TextureOffset":
					    instance.SetTextureOffset("_DetailTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_DetailTex_TextureScale":
					    instance.SetTextureScale("_DetailTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_DetailBump":
					    instance.SetTexture("_DetailBump", reader.Read<Texture>());
					    break;
				    case "_DetailBump_TextureOffset":
					    instance.SetTextureOffset("_DetailBump_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_DetailBump_TextureScale":
					    instance.SetTextureScale("_DetailBump_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Strength":
					    instance.SetFloat("_Strength", reader.Read<float>());
					    break;
				    case "_InvFade":
					    instance.SetFloat("_InvFade", reader.Read<float>());
					    break;
				    case "_EmisColor":
					    instance.SetColor("_EmisColor", reader.Read<Color>());
					    break;
				    case "_Parallax":
					    instance.SetFloat("_Parallax", reader.Read<float>());
					    break;
				    case "_ParallaxMap":
					    instance.SetTexture("_ParallaxMap", reader.Read<Texture>());
					    break;
				    case "_ParallaxMap_TextureOffset":
					    instance.SetTextureOffset("_ParallaxMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_ParallaxMap_TextureScale":
					    instance.SetTextureScale("_ParallaxMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_DecalTex":
					    instance.SetTexture("_DecalTex", reader.Read<Texture>());
					    break;
				    case "_DecalTex_TextureOffset":
					    instance.SetTextureOffset("_DecalTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_DecalTex_TextureScale":
					    instance.SetTextureScale("_DecalTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_GlossMap":
					    instance.SetTexture("_GlossMap", reader.Read<Texture>());
					    break;
				    case "_GlossMap_TextureOffset":
					    instance.SetTextureOffset("_GlossMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_GlossMap_TextureScale":
					    instance.SetTextureScale("_GlossMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_ShadowOffset":
					    instance.SetTexture("_ShadowOffset", reader.Read<Texture>());
					    break;
				    case "_ShadowOffset_TextureOffset":
					    instance.SetTextureOffset("_ShadowOffset_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_ShadowOffset_TextureScale":
					    instance.SetTextureScale("_ShadowOffset_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_SunDisk":
					    instance.SetFloat("_SunDisk", reader.Read<float>());
					    break;
				    case "_SunSize":
					    instance.SetFloat("_SunSize", reader.Read<float>());
					    break;
				    case "_AtmosphereThickness":
					    instance.SetFloat("_AtmosphereThickness", reader.Read<float>());
					    break;
				    case "_SkyTint":
					    instance.SetColor("_SkyTint", reader.Read<Color>());
					    break;
				    case "_GroundColor":
					    instance.SetColor("_GroundColor", reader.Read<Color>());
					    break;
				    case "_WireThickness":
					    instance.SetFloat("_WireThickness", reader.Read<float>());
					    break;
				    case "_ZWrite":
					    instance.SetFloat("_ZWrite", reader.Read<float>());
					    break;
				    case "_ZTest":
					    instance.SetFloat("_ZTest", reader.Read<float>());
					    break;
				    case "_Cull":
					    instance.SetFloat("_Cull", reader.Read<float>());
					    break;
				    case "_ZBias":
					    instance.SetFloat("_ZBias", reader.Read<float>());
					    break;
				    case "_HueVariation":
					    instance.SetColor("_HueVariation", reader.Read<Color>());
					    break;
				    case "_WindQuality":
					    instance.SetFloat("_WindQuality", reader.Read<float>());
					    break;
				    case "_DetailMask":
					    instance.SetTexture("_DetailMask", reader.Read<Texture>());
					    break;
				    case "_DetailMask_TextureOffset":
					    instance.SetTextureOffset("_DetailMask_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_DetailMask_TextureScale":
					    instance.SetTextureScale("_DetailMask_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_MetallicTex":
					    instance.SetTexture("_MetallicTex", reader.Read<Texture>());
					    break;
				    case "_MetallicTex_TextureOffset":
					    instance.SetTextureOffset("_MetallicTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_MetallicTex_TextureScale":
					    instance.SetTextureScale("_MetallicTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Glossiness":
					    instance.SetFloat("_Glossiness", reader.Read<float>());
					    break;
				    case "_GlossMapScale":
					    instance.SetFloat("_GlossMapScale", reader.Read<float>());
					    break;
				    case "_SmoothnessTextureChannel":
					    instance.SetFloat("_SmoothnessTextureChannel", reader.Read<float>());
					    break;
				    case "_Metallic":
					    instance.SetFloat("_Metallic", reader.Read<float>());
					    break;
				    case "_MetallicGlossMap":
					    instance.SetTexture("_MetallicGlossMap", reader.Read<Texture>());
					    break;
				    case "_MetallicGlossMap_TextureOffset":
					    instance.SetTextureOffset("_MetallicGlossMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_MetallicGlossMap_TextureScale":
					    instance.SetTextureScale("_MetallicGlossMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_SpecularHighlights":
					    instance.SetFloat("_SpecularHighlights", reader.Read<float>());
					    break;
				    case "_GlossyReflections":
					    instance.SetFloat("_GlossyReflections", reader.Read<float>());
					    break;
				    case "_BumpScale":
					    instance.SetFloat("_BumpScale", reader.Read<float>());
					    break;
				    case "_OcclusionStrength":
					    instance.SetFloat("_OcclusionStrength", reader.Read<float>());
					    break;
				    case "_OcclusionMap":
					    instance.SetTexture("_OcclusionMap", reader.Read<Texture>());
					    break;
				    case "_OcclusionMap_TextureOffset":
					    instance.SetTextureOffset("_OcclusionMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_OcclusionMap_TextureScale":
					    instance.SetTextureScale("_OcclusionMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_EmissionColor":
					    instance.SetColor("_EmissionColor", reader.Read<Color>());
					    break;
				    case "_EmissionMap":
					    instance.SetTexture("_EmissionMap", reader.Read<Texture>());
					    break;
				    case "_EmissionMap_TextureOffset":
					    instance.SetTextureOffset("_EmissionMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_EmissionMap_TextureScale":
					    instance.SetTextureScale("_EmissionMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_DetailAlbedoMap":
					    instance.SetTexture("_DetailAlbedoMap", reader.Read<Texture>());
					    break;
				    case "_DetailAlbedoMap_TextureOffset":
					    instance.SetTextureOffset("_DetailAlbedoMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_DetailAlbedoMap_TextureScale":
					    instance.SetTextureScale("_DetailAlbedoMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_DetailNormalMapScale":
					    instance.SetFloat("_DetailNormalMapScale", reader.Read<float>());
					    break;
				    case "_DetailNormalMap":
					    instance.SetTexture("_DetailNormalMap", reader.Read<Texture>());
					    break;
				    case "_DetailNormalMap_TextureOffset":
					    instance.SetTextureOffset("_DetailNormalMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_DetailNormalMap_TextureScale":
					    instance.SetTextureScale("_DetailNormalMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_UVSec":
					    instance.SetFloat("_UVSec", reader.Read<float>());
					    break;
				    case "_Mode":
					    instance.SetFloat("_Mode", reader.Read<float>());
					    break;
				    case "_TintColor":
					    instance.SetColor("_TintColor", reader.Read<Color>());
					    break;
				    case "_WavingTint":
					    instance.SetColor("_WavingTint", reader.Read<Color>());
					    break;
				    case "_WaveAndDistance":
					    instance.SetVector("_WaveAndDistance", reader.Read<Vector4>());
					    break;
				    case "_LightTexture0":
					    instance.SetTexture("_LightTexture0", reader.Read<Texture>());
					    break;
				    case "_LightTexture0_TextureOffset":
					    instance.SetTextureOffset("_LightTexture0_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_LightTexture0_TextureScale":
					    instance.SetTextureScale("_LightTexture0_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_LightTextureB0":
					    instance.SetTexture("_LightTextureB0", reader.Read<Texture>());
					    break;
				    case "_LightTextureB0_TextureOffset":
					    instance.SetTextureOffset("_LightTextureB0_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_LightTextureB0_TextureScale":
					    instance.SetTextureScale("_LightTextureB0_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_ShadowMapTexture":
					    instance.SetTexture("_ShadowMapTexture", reader.Read<Texture>());
					    break;
				    case "_ShadowMapTexture_TextureOffset":
					    instance.SetTextureOffset("_ShadowMapTexture_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_ShadowMapTexture_TextureScale":
					    instance.SetTextureScale("_ShadowMapTexture_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_SecondTex":
					    instance.SetTexture("_SecondTex", reader.Read<Texture>());
					    break;
				    case "_SecondTex_TextureOffset":
					    instance.SetTextureOffset("_SecondTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_SecondTex_TextureScale":
					    instance.SetTextureScale("_SecondTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_ThirdTex":
					    instance.SetTexture("_ThirdTex", reader.Read<Texture>());
					    break;
				    case "_ThirdTex_TextureOffset":
					    instance.SetTextureOffset("_ThirdTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_ThirdTex_TextureScale":
					    instance.SetTextureScale("_ThirdTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "PixelSnap":
					    instance.SetFloat("PixelSnap", reader.Read<float>());
					    break;
				    case "_RendererColor":
					    instance.SetColor("_RendererColor", reader.Read<Color>());
					    break;
				    case "_Flip":
					    instance.SetVector("_Flip", reader.Read<Vector4>());
					    break;
				    case "_AlphaTex":
					    instance.SetTexture("_AlphaTex", reader.Read<Texture>());
					    break;
				    case "_AlphaTex_TextureOffset":
					    instance.SetTextureOffset("_AlphaTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_AlphaTex_TextureScale":
					    instance.SetTextureScale("_AlphaTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_EnableExternalAlpha":
					    instance.SetFloat("_EnableExternalAlpha", reader.Read<float>());
					    break;
				    case "_Level":
					    instance.SetFloat("_Level", reader.Read<float>());
					    break;
				    case "_SpecGlossMap":
					    instance.SetTexture("_SpecGlossMap", reader.Read<Texture>());
					    break;
				    case "_SpecGlossMap_TextureOffset":
					    instance.SetTextureOffset("_SpecGlossMap_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_SpecGlossMap_TextureScale":
					    instance.SetTextureScale("_SpecGlossMap_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_FrontTex":
					    instance.SetTexture("_FrontTex", reader.Read<Texture>());
					    break;
				    case "_FrontTex_TextureOffset":
					    instance.SetTextureOffset("_FrontTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_FrontTex_TextureScale":
					    instance.SetTextureScale("_FrontTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_BackTex":
					    instance.SetTexture("_BackTex", reader.Read<Texture>());
					    break;
				    case "_BackTex_TextureOffset":
					    instance.SetTextureOffset("_BackTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_BackTex_TextureScale":
					    instance.SetTextureScale("_BackTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_LeftTex":
					    instance.SetTexture("_LeftTex", reader.Read<Texture>());
					    break;
				    case "_LeftTex_TextureOffset":
					    instance.SetTextureOffset("_LeftTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_LeftTex_TextureScale":
					    instance.SetTextureScale("_LeftTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_RightTex":
					    instance.SetTexture("_RightTex", reader.Read<Texture>());
					    break;
				    case "_RightTex_TextureOffset":
					    instance.SetTextureOffset("_RightTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_RightTex_TextureScale":
					    instance.SetTextureScale("_RightTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_UpTex":
					    instance.SetTexture("_UpTex", reader.Read<Texture>());
					    break;
				    case "_UpTex_TextureOffset":
					    instance.SetTextureOffset("_UpTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_UpTex_TextureScale":
					    instance.SetTextureScale("_UpTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_DownTex":
					    instance.SetTexture("_DownTex", reader.Read<Texture>());
					    break;
				    case "_DownTex_TextureOffset":
					    instance.SetTextureOffset("_DownTex_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_DownTex_TextureScale":
					    instance.SetTextureScale("_DownTex_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_Metallic0":
					    instance.SetFloat("_Metallic0", reader.Read<float>());
					    break;
				    case "_Metallic1":
					    instance.SetFloat("_Metallic1", reader.Read<float>());
					    break;
				    case "_Metallic2":
					    instance.SetFloat("_Metallic2", reader.Read<float>());
					    break;
				    case "_Metallic3":
					    instance.SetFloat("_Metallic3", reader.Read<float>());
					    break;
				    case "_Smoothness0":
					    instance.SetFloat("_Smoothness0", reader.Read<float>());
					    break;
				    case "_Smoothness1":
					    instance.SetFloat("_Smoothness1", reader.Read<float>());
					    break;
				    case "_Smoothness2":
					    instance.SetFloat("_Smoothness2", reader.Read<float>());
					    break;
				    case "_Smoothness3":
					    instance.SetFloat("_Smoothness3", reader.Read<float>());
					    break;
				    case "_TexA":
					    instance.SetTexture("_TexA", reader.Read<Texture>());
					    break;
				    case "_TexA_TextureOffset":
					    instance.SetTextureOffset("_TexA_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_TexA_TextureScale":
					    instance.SetTextureScale("_TexA_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_TexB":
					    instance.SetTexture("_TexB", reader.Read<Texture>());
					    break;
				    case "_TexB_TextureOffset":
					    instance.SetTextureOffset("_TexB_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_TexB_TextureScale":
					    instance.SetTextureScale("_TexB_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_value":
					    instance.SetFloat("_value", reader.Read<float>());
					    break;
				    case "_Texel":
					    instance.SetFloat("_Texel", reader.Read<float>());
					    break;
				    case "_Detail":
					    instance.SetTexture("_Detail", reader.Read<Texture>());
					    break;
				    case "_Detail_TextureOffset":
					    instance.SetTextureOffset("_Detail_TextureOffset", reader.Read<Vector2>());
					    break;
				    case "_Detail_TextureScale":
					    instance.SetTextureScale("_Detail_TextureScale", reader.Read<Vector2>());
					    break;
				    case "_HalfOverCutoff":
					    instance.SetFloat("_HalfOverCutoff", reader.Read<float>());
					    break;

					default:
						reader.Skip();
						break;
				}
			}
		}
	}

		public class ES3Type_MaterialArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_MaterialArray() : base(typeof(Material[]), ES3Type_Material.Instance)
		{
			Instance = this;
		}
	}
}