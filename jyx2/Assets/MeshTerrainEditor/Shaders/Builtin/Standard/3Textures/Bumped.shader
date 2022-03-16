Shader "MTE/Standard/3 Textures/Bumped"
{
	Properties
	{
		_Control ("Control (RGBA)", 2D) = "red" {}
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
		_Splat2 ("Layer 3", 2D) = "white" {}

		_Normal0 ("Normalmap 1", 2D) = "bump" {}
		_Normal1 ("Normalmap 2", 2D) = "bump" {}
		_Normal2 ("Normalmap 3", 2D) = "bump" {}

		[Gamma] _Metallic0("Metallic 0", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic1("Metallic 1", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic2("Metallic 2", Range(0.0, 1.0)) = 0.0
		_Smoothness0("Smoothness 0", Range(0.0, 1.0)) = 1.0
		_Smoothness1("Smoothness 1", Range(0.0, 1.0)) = 1.0
		_Smoothness2("Smoothness 2", Range(0.0, 1.0)) = 1.0
		
		[Toggle(SMOOTHNESS_TEXTURE)] SMOOTHNESS_TEXTURE ("Smoothness Texture", Float) = 0

		[Toggle(ENABLE_NORMAL_INTENSITY)] ENABLE_NORMAL_INTENSITY ("Normal Intensity", Float) = 0
		_NormalIntensity0 ("Normal Intensity 0", Range(0.01, 10)) = 1.0
		_NormalIntensity1 ("Normal Intensity 1", Range(0.01, 10)) = 1.0
		_NormalIntensity2 ("Normal Intensity 2", Range(0.01, 10)) = 1.0
	}

	CGINCLUDE
		#pragma surface surf Standard vertex:MTE_SplatmapVert finalcolor:MTE_SplatmapFinalColor finalprepass:MTE_SplatmapFinalPrepass finalgbuffer:MTE_SplatmapFinalGBuffer
		#pragma multi_compile_fog
		#pragma shader_feature ENABLE_NORMAL_INTENSITY

		struct Input
		{
			float4 tc_ControlSplat0 : TEXCOORD0;
			float4 tc_Splat12 : TEXCOORD1;
			UNITY_FOG_COORDS(2)
		};

		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0,_Splat1,_Splat2;
		float4 _Splat0_ST,_Splat1_ST,_Splat2_ST;
		sampler2D _Normal0,_Normal1,_Normal2;
#ifdef ENABLE_NORMAL_INTENSITY
		fixed _NormalIntensity0, _NormalIntensity1, _NormalIntensity2;
#endif

		#define MTE_STANDARD_SHADER
		#include "UnityPBSLighting.cginc"
		#include "../../MTECommon.hlsl"

		half _Metallic0;
		half _Metallic1;
		half _Metallic2;
		half _Smoothness0;
		half _Smoothness1;
		half _Smoothness2;

		void MTE_SplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc_ControlSplat0.xy = TRANSFORM_TEX(v.texcoord, _Control);
			data.tc_ControlSplat0.zw = TRANSFORM_TEX(v.texcoord, _Splat0);
			data.tc_Splat12.xy = TRANSFORM_TEX(v.texcoord, _Splat1);
			data.tc_Splat12.zw = TRANSFORM_TEX(v.texcoord, _Splat2);
			float4 pos = UnityObjectToClipPos (v.vertex);
			UNITY_TRANSFER_FOG(data, pos);

			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = -1;
		}

		void MTE_SplatmapMix(Input IN, half4 defaultAlpha, out half4 splat_control, out half weight, out fixed4 mixedDiffuse, inout float3 mixedNormal)
		{
			splat_control = tex2D(_Control, IN.tc_ControlSplat0.xy);
			weight = dot(splat_control, half4(1, 1, 1, 1));
			splat_control /= (weight + 1e-3f);

			mixedDiffuse = 0.0f;
			mixedDiffuse += splat_control.r * tex2D(_Splat0, IN.tc_ControlSplat0.zw) * half4(1.0, 1.0, 1.0, defaultAlpha.r);
			mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.tc_Splat12.xy) * half4(1.0, 1.0, 1.0, defaultAlpha.g);
			mixedDiffuse += splat_control.b * tex2D(_Splat2, IN.tc_Splat12.zw) * half4(1.0, 1.0, 1.0, defaultAlpha.b);
			
#ifdef ENABLE_NORMAL_INTENSITY
			fixed3 nrm0 = UnpackNormal(tex2D(_Normal0, IN.tc_ControlSplat0.zw));
			fixed3 nrm1 = UnpackNormal(tex2D(_Normal1, IN.tc_Splat12.xy));
			fixed3 nrm2 = UnpackNormal(tex2D(_Normal2, IN.tc_Splat12.zw));
			nrm0 = splat_control.r * MTE_NormalIntensity_fixed(nrm0, _NormalIntensity0);
			nrm1 = splat_control.g * MTE_NormalIntensity_fixed(nrm1, _NormalIntensity1);
			nrm2 = splat_control.b * MTE_NormalIntensity_fixed(nrm2, _NormalIntensity2);
			mixedNormal = normalize(nrm0 + nrm1 + nrm2);
#else
			fixed4 nrm = 0.0f;
			nrm += splat_control.r * tex2D(_Normal0, IN.tc_ControlSplat0.zw);
			nrm += splat_control.g * tex2D(_Normal1, IN.tc_Splat12.xy);
			nrm += splat_control.b * tex2D(_Normal2, IN.tc_Splat12.zw);
			mixedNormal = UnpackNormal(nrm);
#endif
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half4 splat_control;
			fixed4 mixedDiffuse;
			half weight;
			half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, _Smoothness2, 0);
			MTE_SplatmapMix(IN, defaultSmoothness, splat_control, weight, mixedDiffuse, o.Normal);
			o.Albedo = mixedDiffuse.rgb;
			o.Alpha = weight;
			o.Smoothness = mixedDiffuse.a;
			o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, 0));
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
		//target 2.0 not supported
	}

	Fallback "MTE/Standard/3 Textures/Diffuse"
	CustomEditor "MTE.MTEShaderGUI"
}
