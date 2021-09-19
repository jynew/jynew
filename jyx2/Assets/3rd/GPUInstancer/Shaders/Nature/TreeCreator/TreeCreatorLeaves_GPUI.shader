// GPUInstancer enabled version of Unity built-in shader "Nature/Tree Creator Leaves"

Shader "GPUInstancer/Nature/Tree Creator Leaves" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_GlossMap ("Gloss (A)", 2D) = "black" {}
		_TranslucencyMap ("Translucency (A)", 2D) = "white" {}
		_ShadowOffset ("Shadow Offset (A)", 2D) = "black" {}

		// These are here only to provide default values
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
		[HideInInspector] _TreeInstanceColor ("TreeInstanceColor", Vector) = (1,1,1,1)
		[HideInInspector] _TreeInstanceScale ("TreeInstanceScale", Vector) = (1,1,1,1)
		[HideInInspector] _SquashAmount ("Squash", Float) = 1
	}

	SubShader {
		Tags { "IgnoreProjector"="True" "RenderType"="TreeLeaf" }
		LOD 200

	CGPROGRAM

	#pragma multi_compile_instancing
	#pragma instancing_options procedural:setupGPUI
	#pragma surface surf TreeLeaf alphatest:_Cutoff vertex:TreeVertLeaf addshadow nolightmap noforwardadd
	#pragma multi_compile _ LOD_FADE_CROSSFADE
	#include "UnityCG.cginc"
	#include "../../Include/GPUInstancerInclude.cginc"
	#include "UnityBuiltin3xTreeLibrary.cginc"

	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _GlossMap;
	sampler2D _TranslucencyMap;
	half _Shininess;

	struct Input {
		float2 uv_MainTex;
		fixed4 color : COLOR;
	};

	void surf (Input IN, inout LeafSurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb * IN.color.rgb * IN.color.a;
		o.Translucency = tex2D(_TranslucencyMap, IN.uv_MainTex).rgb;
		o.Gloss = tex2D(_GlossMap, IN.uv_MainTex).a;
		o.Alpha = c.a;
		o.Specular = _Shininess;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
	}
	ENDCG
	}

}
