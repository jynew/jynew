//-----------------------------------------------------
// Forward screen space decal multiply shader. Version 0.8
// Copyright (c) 2017 by Sycoforge
//-----------------------------------------------------
Shader "Easy Decal/SSD/Multiply SSD" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags 
		{ 
			"RenderType"= "Transparent" 
			"Queue" = "Transparent+1" 
			// Temporarily disable batching -> TODO encoding transform to geometry channels
			"DisableBatching" = "True" 
		}

		ZWrite On ZTest Always
		Lighting Off Cull Front
		Blend Zero SrcColor
		Offset -1,-1

		Pass
		{		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_fog


			#include "SycoFSSD.cginc"

			ENDCG
		}
	} 
	FallBack "Unlit/Transparent"
}

