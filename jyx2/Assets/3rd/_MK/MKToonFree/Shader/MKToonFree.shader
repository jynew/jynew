Shader "MK/Toon/Free"
{
	Properties
	{
		//Main
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Color (RGB)", 2D) = "white"{}

		//Normalmap
		_BumpMap ("Normalmap", 2D) = "bump" {}

		//Light
		_LightThreshold("LightThreshold", Range (0.01, 1)) = 0.3

		//Render
		_LightSmoothness ("Light Smoothness", Range(0,1)) = 0
		_RimSmoothness ("Rim Smoothness", Range(0,1)) = 0.5

		//Custom shadow
		_ShadowColor ("Shadow Color", Color) = (0.0,0.0,0.0,1.0)
		_HighlightColor ("Highlight Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_ShadowIntensity("Shadow Intensity", Range (0.0, 2.0)) = 1.0

		//Outline
		_OutlineColor ("Outline Color", Color) = (0,0,0,1.0)
		_OutlineSize ("Outline Size", Float) = 0.02

		//Rim
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimSize ("Rim Size", Range(0.0,3.0)) = 1.5
		_RimIntensity("Intensity", Range (0, 1)) = 0.5

		//Specular
		_Shininess ("Shininess",  Range (0.01, 1)) = 0.275
		_SpecColor ("Specular Color", Color) = (1,1,1,0.5)
		_SpecularIntensity("Intensity", Range (0, 1)) = 0.5

		//Emission
		_EmissionColor("Emission Color", Color) = (0,0,0)

		//Editor
		[HideInInspector] _MKEditorShowMainBehavior ("Main Behavior", int) = 1
		[HideInInspector] _MKEditorShowDetailBehavior ("Detail Behavior", int) = 0
		[HideInInspector] _MKEditorShowLightBehavior ("Light Behavior", int) = 0
		[HideInInspector] _MKEditorShowShadowBehavior ("Shadow Behavior", int) = 0
		[HideInInspector] _MKEditorShowRenderBehavior ("Render Behavior", int) = 0
		[HideInInspector] _MKEditorShowSpecularBehavior ("Specular Behavior", int) = 0
		[HideInInspector] _MKEditorShowTranslucentBehavior ("Translucent Behavior", int) = 0
		[HideInInspector] _MKEditorShowRimBehavior ("Rim Behavior", int) = 0
		[HideInInspector] _MKEditorShowReflectionBehavior ("Reflection Behavior", int) = 0
		[HideInInspector] _MKEditorShowDissolveBehavior ("Dissolve Behavior", int) = 0
		[HideInInspector] _MKEditorShowOutlineBehavior ("Outline Behavior", int) = 0
		[HideInInspector] _MKEditorShowSketchBehavior ("Sketch Behavior", int) = 0
	}
	SubShader
	{
		LOD 300
		Tags {"RenderType"="Opaque" "PerformanceChecks"="False"}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// FORWARD BASE
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode" = "ForwardBase" } 
			Name "FORWARDBASE" 
			Cull Back
			Blend One Zero
			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vertfwd
			#pragma fragment fragfwd

			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma multi_compile_instancing

			#include "Inc/Forward/MKToonForwardBaseSetup.cginc"
			#include "Inc/Forward/MKToonForward.cginc"
			
			ENDCG
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// FORWARD ADD
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode" = "ForwardAdd" } 
			Name "FORWARDADD"
			Cull Back
			Blend One One
			ZWrite Off
			ZTest LEqual
			Fog { Color (0,0,0,0) }

			CGPROGRAM
			#pragma target 3.0

			#pragma vertex vertfwd
			#pragma fragment fragfwd

			#pragma multi_compile_fog
			#pragma multi_compile_fwdadd_fullshadows
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "Inc/Forward/MKToonForwardAddSetup.cginc"
			#include "Inc/Forward/MKToonForward.cginc"
			
			ENDCG
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// SHADOWCASTER
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass 
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1

			CGPROGRAM
			#pragma target 3.0
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#pragma multi_compile_instancing

			#include "Inc/ShadowCaster/MKToonShadowCasterSetup.cginc"
			#include "Inc/ShadowCaster/MKToonShadowCaster.cginc"

			ENDCG
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// META
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            CGPROGRAM
            #pragma vertex vert_meta
            #pragma fragment frag_meta

            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION

            #include "UnityStandardMeta.cginc"
            ENDCG
        }

		/////////////////////////////////////////////////////////////////////////////////////////////
		// OUTLINE
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			LOD 300
			Tags {"LightMode" = "Always"}
			Name "OUTLINE_SM_3_0"
			Cull Front
			Blend One Zero
			ZWrite On
			ZTest LEqual

			CGPROGRAM 
			#pragma target 3.0
			#pragma vertex outlinevert 
			#pragma fragment outlinefrag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma multi_compile_fog

			#pragma multi_compile_instancing

			#include "/Inc/Outline/MKToonOutlineOnlySetup.cginc"
			#include "/Inc/Outline/MKToonOutlineOnlyBase.cginc"
			ENDCG 
		}
    }
	FallBack "Hidden/MK/Toon/FreeMobile"
	CustomEditor "MK.Toon.MKToonFreeEditor"
}
