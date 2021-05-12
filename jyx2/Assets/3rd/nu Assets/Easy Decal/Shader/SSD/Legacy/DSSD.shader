//-----------------------------------------------------
// Deferred screen space decal diffuse shader. Version 0.9 [Beta]
// Copyright (c) 2017 by Sycoforge
//-----------------------------------------------------
Shader "Easy Decal/SSD/Deferred SSD" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)

		_BumpMap ("Normal Map", 2D) = "bump" {}

		_SpecGlossMap("Specular Map", 2D) = "white" {}
		_SpecularColor("Specular Tint", Color) = (1,1,1,1)
		
		_EmissionMap("Emission", 2D) = "black" {}
		[HDR]_EmissionColor("Emission Color", Color) = (0,0,0)

		_NormalMultiplier ("Normal Strength", Range(-4.0, 4.0)) = 1.0
		_SpecularMultiplier ("Specular Strength", Range(0.0, 1.0)) = 1.0
		_SmoothnessMultiplier ("Smoothness Strength", Range(0.0, 1.0)) = 0.5
	}

	SubShader 
	{
		Tags
		{ 
			"Queue" = "Transparent+1" 
			"DisableBatching" = "True"  
			"IgnoreProjector" = "True" 
		}

		ZWrite Off 
		ZTest Always 
		Cull Front
		Fog { Mode Off } 

		// Use custom blend function
		Blend Off

		//------------------------------------------
		// Pass 0 ** Diffuse/SpecSmooth/Normal
		//------------------------------------------
		Pass
		{		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma exclude_renderers nomrt d3d9
			#pragma multi_compile ___ UNITY_HDR_ON
			#define DIFFUSE_ON
			#define SPECSMOOTH_ON
			#define NORMAL_ON
			#include "SycoDSSD.cginc"
			ENDCG
		}
		//------------------------------------------
		// Pass 1 ** Diffuse/SpecSmooth
		//------------------------------------------
		Pass
		{		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma exclude_renderers nomrt d3d9
			#pragma multi_compile ___ UNITY_HDR_ON
			#define DIFFUSE_ON
			#define SPECSMOOTH_ON
			#include "SycoDSSD.cginc"
			ENDCG
		}
		//------------------------------------------
		// Pass 2 ** Diffuse/Normal
		//------------------------------------------
		Pass
		{		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma exclude_renderers nomrt d3d9
			#pragma multi_compile ___ UNITY_HDR_ON
			#define DIFFUSE_ON
			#define NORMAL_ON
			#include "SycoDSSD.cginc"
			ENDCG
		}
		//------------------------------------------
		// Pass 3 ** SpecSmooth/Normal
		//------------------------------------------
		Pass
		{		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma exclude_renderers nomrt d3d9
			#pragma multi_compile ___ UNITY_HDR_ON
			#define SPECSMOOTH_ON
			#define NORMAL_ON
			#include "SycoDSSD.cginc"
			ENDCG
		}
		//------------------------------------------
		// Pass 4 ** Diffuse
		//------------------------------------------
		Pass
		{		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma exclude_renderers nomrt d3d9
			#pragma multi_compile ___ UNITY_HDR_ON
			#define DIFFUSE_ON
			#include "SycoDSSD.cginc"
			ENDCG
		}
		//------------------------------------------
		// Pass 5 ** SpecSmooth
		//------------------------------------------
		Pass
		{		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma exclude_renderers nomrt d3d9
			#pragma multi_compile ___ UNITY_HDR_ON
			#define SPECSMOOTH_ON
			#include "SycoDSSD.cginc"
			ENDCG
		}
		//------------------------------------------
		// Pass 6 ** Normal
		//------------------------------------------
		Pass
		{		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma exclude_renderers nomrt d3d9
			#pragma multi_compile ___ UNITY_HDR_ON
			#define NORMAL_ON
			#include "SycoDSSD.cginc"
			ENDCG
		}
	} 
	FallBack Off
	//CustomEditor "ch.sycoforge.Decal.Editor.DSSDShaderGUI"
}

