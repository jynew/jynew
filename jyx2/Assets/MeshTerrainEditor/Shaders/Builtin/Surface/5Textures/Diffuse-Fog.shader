Shader "MTE/Surface/5 Textures/Diffuse/Fog"
{
	Properties
	{
		_Control ("Control (RGBA)", 2D) = "red" {}
		_ControlExtra ("Control Extra (R)", 2D) = "black" {}
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
		_Splat2 ("Layer 3", 2D) = "white" {}
		_Splat3 ("Layer 4", 2D) = "white" {}
		_Splat4 ("Layer 5", 2D) = "white" {}
	}

	CGINCLUDE
		#pragma surface surf Lambert vertex:MTE_SplatmapVert finalcolor:MTE_SplatmapFinalColor finalprepass:MTE_SplatmapFinalPrepass finalgbuffer:MTE_SplatmapFinalGBuffer
		#pragma multi_compile_fog

		struct Input
		{
			float2 tc_Control  : TEXCOORD0;
			float4 tc_Splat01  : TEXCOORD1;
			float4 tc_Splat23  : TEXCOORD2;
			UNITY_FOG_COORDS(3)
		};

		sampler2D _Control,_ControlExtra;
		float4 _Control_ST,_ControlExtra_ST;
		sampler2D _Splat0,_Splat1,_Splat2,_Splat3,_Splat4;
		float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

		#include "../../MTECommon.hlsl"

		void MTE_SplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
			data.tc_Splat01.xy = TRANSFORM_TEX(v.texcoord, _Splat0);
			data.tc_Splat01.zw = TRANSFORM_TEX(v.texcoord, _Splat1);
			data.tc_Splat23.xy = TRANSFORM_TEX(v.texcoord, _Splat2);
			data.tc_Splat23.zw = TRANSFORM_TEX(v.texcoord, _Splat3);
			// Because the UNITY_FOG_COORDS(3) occupied one TEXCOORD, Splat4 have to share texcoord with Splat3 - both using tc_Splat23.zw.
			float4 pos = UnityObjectToClipPos (v.vertex);
			UNITY_TRANSFER_FOG(data, pos);
		}

		void MTE_SplatmapMix(Input IN, out half weight, out fixed4 mixedDiffuse)
		{
			half4 splat_control = tex2D(_Control, IN.tc_Control.xy);
			half4 splat_control_extra = tex2D(_ControlExtra, IN.tc_Control.xy);
			weight = dot(splat_control, half4(1, 1, 1, 1));
			weight += dot(splat_control_extra.r, half(1));
			splat_control /= (weight + 1e-3f);
			splat_control_extra /= (weight + 1e-3f);

			mixedDiffuse = 0;
			mixedDiffuse += splat_control.r * tex2D(_Splat0, IN.tc_Splat01.xy);
			mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.tc_Splat01.zw);
			mixedDiffuse += splat_control.b * tex2D(_Splat2, IN.tc_Splat23.xy);
			mixedDiffuse += splat_control.a * tex2D(_Splat3, IN.tc_Splat23.zw);
			mixedDiffuse += splat_control_extra.r * tex2D(_Splat4, IN.tc_Splat23.zw);
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 mixedDiffuse;
			half weight;
			MTE_SplatmapMix(IN, weight, mixedDiffuse);
			o.Albedo = mixedDiffuse.rgb;
			o.Alpha = weight;
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
		SubShader//for target 2.5
		{
			CGPROGRAM
				#pragma target 2.5
			ENDCG
		}
		SubShader//for target 2.0
		{
			CGPROGRAM
				#pragma target 2.0
			ENDCG
		}
	}

	Fallback "Diffuse"
	CustomEditor "MTE.MTEShaderGUI"
}
