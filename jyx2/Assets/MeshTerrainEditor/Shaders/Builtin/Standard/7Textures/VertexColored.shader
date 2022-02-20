Shader "MTE/Standard/7 Textures/VertexColored"
{
	Properties
	{
		_Control("Control (RGBA)", 2D) = "red" {}
		_ControlExtra("Control Extra (RGB)", 2D) = "black" {}
		_Splat0("Layer 1", 2D) = "white" {}
		_Splat1("Layer 2", 2D) = "white" {}
		_Splat2("Layer 3", 2D) = "white" {}
		_Splat3("Layer 4", 2D) = "white" {}
		_Splat4("Layer 5", 2D) = "white" {}
		_Splat5("Layer 6", 2D) = "white" {}
		_Splat6("Layer 7", 2D) = "white" {}

		[Gamma] _Metallic0("Metallic 1", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic1("Metallic 2", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic2("Metallic 3", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic3("Metallic 4", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic4("Metallic 5", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic5("Metallic 6", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic6("Metallic 7", Range(0.0, 1.0)) = 0.0
		_Smoothness0("Smoothness 1", Range(0.0, 1.0)) = 1.0
		_Smoothness1("Smoothness 2", Range(0.0, 1.0)) = 1.0
		_Smoothness2("Smoothness 3", Range(0.0, 1.0)) = 1.0
		_Smoothness3("Smoothness 4", Range(0.0, 1.0)) = 1.0
		_Smoothness4("Smoothness 5", Range(0.0, 1.0)) = 1.0
		_Smoothness5("Smoothness 6", Range(0.0, 1.0)) = 1.0
		_Smoothness6("Smoothness 7", Range(0.0, 1.0)) = 1.0
	}

	CGINCLUDE
	#pragma surface surf Standard vertex:MTE_SplatmapVert finalcolor:MTE_SplatmapFinalColor finalprepass:MTE_SplatmapFinalPrepass finalgbuffer:MTE_SplatmapFinalGBuffer
	#pragma multi_compile_fog

	struct Input
	{
		float4 tc;
		UNITY_FOG_COORDS(0)
		float3 color : COLOR;
	};

	sampler2D _Control, _ControlExtra;
	float4 _Control_ST, _ControlExtra_ST;
	sampler2D _Splat0, _Splat1, _Splat2, _Splat3, _Splat4, _Splat5, _Splat6;
	float4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST, _Splat4_ST, _Splat5_ST, _Splat6_ST;

	#define MTE_STANDARD_SHADER
	#include "UnityPBSLighting.cginc"
	#include "../../MTECommon.hlsl"

	half _Metallic0;
	half _Metallic1;
	half _Metallic2;
	half _Metallic3;
	half _Metallic4;
	half _Metallic5;
	half _Metallic6;
	half _Smoothness0;
	half _Smoothness1;
	half _Smoothness2;
	half _Smoothness3;
	half _Smoothness4;
	half _Smoothness5;
	half _Smoothness6;

	void MTE_SplatmapVert(inout appdata_full v, out Input data)
	{
		UNITY_INITIALIZE_OUTPUT(Input, data);
		data.tc = v.texcoord;
		float4 pos = UnityObjectToClipPos(v.vertex);
		UNITY_TRANSFER_FOG(data, pos);

		v.tangent.xyz = cross(v.normal, float3(0, 0, 1));
		v.tangent.w = -1;
	}

	void MTE_SplatmapMix(Input IN, half4 smoothness0, half4 smoothness1, out half4 splat_control, out half4 splat_control_extra, out half weight, out fixed4 mixedDiffuse)
	{
		float2 uvControl = TRANSFORM_TEX(IN.tc.xy, _Control);
		float2 uvControlExtra = TRANSFORM_TEX(IN.tc.xy, _ControlExtra);
		float2 uvSplat0 = TRANSFORM_TEX(IN.tc.xy, _Splat0);
		float2 uvSplat1 = TRANSFORM_TEX(IN.tc.xy, _Splat1);
		float2 uvSplat2 = TRANSFORM_TEX(IN.tc.xy, _Splat2);
		float2 uvSplat3 = TRANSFORM_TEX(IN.tc.xy, _Splat3);
		float2 uvSplat4 = TRANSFORM_TEX(IN.tc.xy, _Splat4);
		float2 uvSplat5 = TRANSFORM_TEX(IN.tc.xy, _Splat5);
		float2 uvSplat6 = TRANSFORM_TEX(IN.tc.xy, _Splat6);

		splat_control = tex2D(_Control, uvControl);
		splat_control_extra = tex2D(_ControlExtra, uvControlExtra);
		weight = dot(splat_control, half4(1, 1, 1, 1));
		weight += dot(splat_control_extra, half4(1, 1, 1, 1));
		splat_control /= (weight + 1e-3f);
		splat_control_extra /= (weight + 1e-3f);

		mixedDiffuse = 0;
		mixedDiffuse += splat_control.r       * tex2D(_Splat0, uvSplat0) * half4(1.0, 1.0, 1.0, smoothness0.r);
		mixedDiffuse += splat_control.g       * tex2D(_Splat1, uvSplat1) * half4(1.0, 1.0, 1.0, smoothness0.g);
		mixedDiffuse += splat_control.b       * tex2D(_Splat2, uvSplat2) * half4(1.0, 1.0, 1.0, smoothness0.b);
		mixedDiffuse += splat_control.a       * tex2D(_Splat3, uvSplat3) * half4(1.0, 1.0, 1.0, smoothness0.a);
		mixedDiffuse += splat_control_extra.r * tex2D(_Splat4, uvSplat4) * half4(1.0, 1.0, 1.0, smoothness1.r);
		mixedDiffuse += splat_control_extra.g * tex2D(_Splat5, uvSplat5) * half4(1.0, 1.0, 1.0, smoothness1.g);
		mixedDiffuse += splat_control_extra.b * tex2D(_Splat6, uvSplat6) * half4(1.0, 1.0, 1.0, smoothness1.b);
	}

	void surf(Input IN, inout SurfaceOutputStandard o)
	{
		half4 splat_control, splat_control_extra;
		fixed4 mixedDiffuse;
		half weight;
		half4 defaultSmoothness0 = half4(_Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3);
		half4 defaultSmoothness1 = half4(_Smoothness4, _Smoothness5, _Smoothness6, 0);
		MTE_SplatmapMix(IN, defaultSmoothness0, defaultSmoothness1, splat_control, splat_control_extra, weight, mixedDiffuse);
		o.Albedo = mixedDiffuse.rgb * IN.color.rgb;
		o.Alpha = weight;
		o.Smoothness = mixedDiffuse.a;
		o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, _Metallic3)) + dot(splat_control_extra.rgb, half3(_Metallic4, _Metallic5, _Metallic6));
	}
	ENDCG
	
	Category
	{
		Tags
		{
			"Queue" = "Geometry-99"
			"RenderType" = "Opaque"
		}
		SubShader//for target 3.0+
		{
			CGPROGRAM
				#pragma target 3.0
			ENDCG
		}
	}

	Fallback "Diffuse"
	CustomEditor "MTE.MTEShaderGUI"
}
