//Shared outline shader for rendering
Shader "Hidden/MK/Toon/FreeOutlineOnly" 
{
	Properties
	{
		//Outline
		_OutlineColor ("Outline Color", Color) = (0,0,0,1.0)
		_OutlineSize ("Outline Size", Float) = 0.02
	}
	/////////////////////////////////////////////////////////////////////////////////////////////
	// SHADER MODEL 3
	/////////////////////////////////////////////////////////////////////////////////////////////
	SubShader 
	{ 
		Tags {"RenderType"="Opaque" "PerformanceChecks"="False"}
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

			#include "/Inc/Outline/MKToonOutlineOnlySetup.cginc"
			#include "/Inc/Outline/MKToonOutlineOnlyBase.cginc"
			ENDCG 
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////
	// SHADER MODEL 2
	/////////////////////////////////////////////////////////////////////////////////////////////
	SubShader 
	{ 
		Tags {"RenderType"="Opaque" "PerformanceChecks"="False"}
		Pass
		{
			LOD 150
			Tags {"LightMode" = "Always"}
			Name "OUTLINE_SM_2_5"
			Cull Front
			Blend One Zero
			ZWrite On
			ZTest LEqual

			CGPROGRAM 
			#pragma target 2.5
			#pragma vertex outlinevert 
			#pragma fragment outlinefrag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma multi_compile_fog

			#include "/Inc/Outline/MKToonOutlineOnlySetup.cginc"
			#include "/Inc/Outline/MKToonOutlineOnlyBase.cginc"
			ENDCG 
		}
	}
	CustomEditor "MK.Toon.MKToonEditor"
	FallBack Off
}