Shader "MTE/Specialized/Mali 400/Surface/4 Textures/Diffuse"
{
	Properties
	{
		_Control ("Control (RGBA)", 2D) = "red" {}
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
		_Splat2 ("Layer 3", 2D) = "white" {}
		_Splat3 ("Layer 4", 2D) = "white" {}
	}

	CGINCLUDE
		#pragma fragmentoption ARB_precision_hint_nicest
		#pragma surface surf Lambert vertex:MTE_SplatmapVert finalcolor:MTE_SplatmapFinalColor finalprepass:MTE_SplatmapFinalPrepass finalgbuffer:MTE_SplatmapFinalGBuffer
		#pragma multi_compile_fog

		struct Input
		{
			float4 tc;
			UNITY_FOG_COORDS(0)
		};

		sampler2D _Control;
		float4 _Control_ST;
		sampler2D  _Splat0,_Splat1,_Splat2,_Splat3;
		float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

		#include "../../../../MTECommon.hlsl"

		void MTE_SplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc = v.texcoord;
			float4 pos = UnityObjectToClipPos (v.vertex);
			UNITY_TRANSFER_FOG(data, pos);

			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = -1;
		}

		void MTE_SplatmapMix(Input IN, out half weight, out float4 mixedDiffuse)
		{
			float2 uvControl = TRANSFORM_TEX(IN.tc.xy, _Control);
			float2 uvSplat0  = IN.tc.xy;
			float2 uvSplat1  = IN.tc.xy;
			float2 uvSplat2  = IN.tc.xy;
			float2 uvSplat3  = IN.tc.xy;

			half4 splat_control = tex2D(_Control, uvControl);
			weight = dot(splat_control, half4(1, 1, 1, 1));
			splat_control /= (weight + 1e-3f);

			mixedDiffuse = 0.0f;
			mixedDiffuse += splat_control.r * tex2D(_Splat0, uvSplat0);
			mixedDiffuse += splat_control.g * tex2D(_Splat1, uvSplat1);
			mixedDiffuse += splat_control.b * tex2D(_Splat2, uvSplat2);
			mixedDiffuse += splat_control.a * tex2D(_Splat2, uvSplat3);
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

	CustomEditor "MTE.MTEShaderGUI"
}
