// Easy Decal shader using Unity's standard pbs workflow. v1.1
Shader "Easy Decal/ED Standard Normal (Vertex Alpha)" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_NormalStrength ("Normal Strength", Range(1,2)) = 1.0
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
		//Blend SrcAlpha OneMinusSrcAlpha   
		//AlphaToMask On
		
		CGPROGRAM

		#pragma surface surf Standard alpha:fade vertex:vert fullforwardshadows addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#include "UnityPBSLighting.cginc"
		sampler2D _MainTex;
		sampler2D _BumpMap;

		half _Glossiness;
		half _Metallic;
		half _NormalStrength;
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
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed3 n = UnpackNormal (tex2D (_BumpMap, IN.uv_MainTex))*_NormalStrength;
			o.Albedo = c.rgb;
			o.Normal = n;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a * IN.color.a * _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
