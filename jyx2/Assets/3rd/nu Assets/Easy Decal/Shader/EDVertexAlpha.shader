// Easy Decal shader using Unity's standard pbs workflow. v1.3
Shader "Easy Decal/ED Standard (Metallic, Vertex Alpha)" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MetallicMap ("Metallic (R) Smoothness (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Occlusion ("Ambient Occlusion (R)", 2D) = "white" {}
		_Metallic ("Metallic Multiplier", Range(0,1)) = 1.0
		_Smoothness ("Smoothness Multiplier", Range(0,1)) = 1.0
	}

	SubShader 
	{
		Tags 
		{ 
			"Queue"="Transparent" 
			"RenderType"="Transparent" 
			"ForceNoShadowCasting" = "True"
		}
		LOD 200
		Offset -1,-1

		
		CGPROGRAM

		#pragma surface surf Standard alpha:fade vertex:vert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#include "UnityPBSLighting.cginc"
		sampler2D _MainTex;
		sampler2D _MetallicMap;
		sampler2D _BumpMap;
		sampler2D _Occlusion;

		half _Metallic;
		half _Smoothness;
		fixed4 _Color;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float4 color;
		};
 
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color; 
		}


		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 m = tex2D (_MetallicMap, IN.uv_MainTex);
			fixed4 n = tex2D (_BumpMap, IN.uv_BumpMap);
			fixed ao = tex2D (_Occlusion, IN.uv_BumpMap).r;

			o.Albedo = c.rgb * IN.color.rgb * _Color;
			o.Metallic = m.r * _Metallic;
			o.Smoothness = m.a * _Smoothness;
			o.Normal = UnpackNormal (n);
			o.Occlusion = ao;
			o.Alpha = c.a * IN.color.a;
		}
		ENDCG
	}
	FallBack "Transparent"
}
