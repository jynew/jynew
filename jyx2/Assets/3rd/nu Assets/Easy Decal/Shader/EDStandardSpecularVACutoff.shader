// Easy Decal shader using Unity's standard pbs workflow. v1.0
Shader "Easy Decal/ED Standard (Specular, Vertex Alpha, Cutoff)" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SpecMap ("Specular (RGB) Smoothness (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Occlusion ("Ambient Occlusion (R)", 2D) = "white" {}
		_Specularity ("Specular Multiplier", Range(0,1)) = 1.0
		_Smoothness ("Smoothness Multiplier", Range(0,1)) = 1.0
		_Cutoff ("Cutoff", Range(0.01,1)) = 0.1
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
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular fullforwardshadows alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SpecMap;
		sampler2D _BumpMap;
		sampler2D _Occlusion;

		half _Specularity;
		half _Smoothness;
		fixed4 _Color;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float4 color : COLOR;
		};


		void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 s = tex2D (_SpecMap, IN.uv_MainTex);
			fixed4 n = tex2D (_BumpMap, IN.uv_BumpMap);
			fixed ao = tex2D (_Occlusion, IN.uv_BumpMap).r;

			o.Albedo = c.rgb * IN.color.rgb * _Color;
			o.Specular = s.rgb * _Specularity;
			o.Smoothness = s.a * _Smoothness;
			o.Normal = UnpackNormal (n);
			o.Occlusion = ao;
			o.Alpha = c.a * IN.color.a;
		}
		ENDCG
	}
	FallBack "Transparent"
}
