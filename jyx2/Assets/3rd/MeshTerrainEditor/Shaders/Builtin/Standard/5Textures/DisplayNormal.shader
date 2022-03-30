Shader "MTE/Standard/5 Textures/Display Normal"
{
	Properties
	{
		_Control ("Control (RGBA)", 2D) = "red" {}
		[NoScaleOffset]_ControlExtra ("Control Extra (R)", 2D) = "black" {}
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
		_Splat2 ("Layer 3", 2D) = "white" {}
		_Splat3 ("Layer 4", 2D) = "white" {}
		_Splat4 ("Layer 5", 2D) = "white" {}

		[NoScaleOffset]_Normal0 ("Normalmap 1", 2D) = "bump" {}
		[NoScaleOffset]_Normal1 ("Normalmap 2", 2D) = "bump" {}
		[NoScaleOffset]_Normal2 ("Normalmap 3", 2D) = "bump" {}
		[NoScaleOffset]_Normal3 ("Normalmap 4", 2D) = "bump" {}
		[NoScaleOffset]_Normal4 ("Normalmap 5", 2D) = "bump" {}
	}

	CGINCLUDE
		#pragma surface surf NoLighting nofog noambient noforwardadd vertex:MTE_SplatmapVert
		#pragma exclude_renderers d3d9 gles//TextureArray is not available on d3d9 and gles2
		//#pragma enable_d3d11_debug_symbols //for debug

		struct Input
		{
			float2 tc_Control  : TEXCOORD0;
			float4 tc_Splat01  : TEXCOORD1;
			float4 tc_Splat23  : TEXCOORD2;
		};
		
		#define MTE_STANDARD_SHADER
		#include "UnityPBSLighting.cginc"
		#include "../../MTECommon.hlsl"
	
		sampler2D _Control,_ControlExtra;
		float4 _Control_ST,_ControlExtra_ST;
		sampler2D _Splat0,_Splat1,_Splat2,_Splat3,_Splat4;
		sampler2D _Normal0,_Normal1,_Normal2,_Normal3,_Normal4;
		float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

		void MTE_SplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
			data.tc_Splat01.xy = TRANSFORM_TEX(v.texcoord, _Splat0);
			data.tc_Splat01.zw = TRANSFORM_TEX(v.texcoord, _Splat1);
			data.tc_Splat23.xy = TRANSFORM_TEX(v.texcoord, _Splat2);
			data.tc_Splat23.zw = TRANSFORM_TEX(v.texcoord, _Splat3);
			//Splat4 shares texcoord with Splat3 - both using tc_Splat23.zw.
		}

		void MTE_SplatmapMix(Input IN, out half4 packedNormal)
		{
			half4 splat_control = tex2D(_Control, IN.tc_Control.xy);
			half4 splat_control_extra = tex2D(_ControlExtra, IN.tc_Control.xy);
			
			half4 nrm = 0.0f;
			nrm += splat_control.r * tex2D(_Normal0, IN.tc_Splat01.xy);
			nrm += splat_control.g * tex2D(_Normal1, IN.tc_Splat01.zw);
			nrm += splat_control.b * tex2D(_Normal2, IN.tc_Splat23.xy);
			nrm += splat_control.a * tex2D(_Normal3, IN.tc_Splat23.zw);
			nrm += splat_control_extra.r * tex2D(_Normal4, IN.tc_Splat23.zw);
			packedNormal = nrm;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			half4 packedNormal;
			MTE_SplatmapMix(IN, packedNormal);
			half3 normal = UnpackNormal(packedNormal);
			o.Albedo.rgb = normal.rgb*0.5+0.5;
			o.Alpha = 1.0;
		}

		half4 LightingNoLighting(SurfaceOutput s, half3 lightDir, half atten)
		{
			return half4(s.Albedo, s.Alpha);
        }
	ENDCG
	
	Category
	{
		Tags
		{
			"Queue" = "Geometry-99"
			"RenderType" = "Opaque"
		}
		SubShader
		{
			CGPROGRAM
				#pragma target 4.0
			ENDCG
		}
	}

	Fallback "Diffuse"
}