Shader "MTE/Surface/8 Textures/Specular"
{
	Properties
	{
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range(0.03, 1)) = 0.078125

		_Control ("Control (RGBA)", 2D) = "red" {}
		_ControlExtra ("Control Extra (RGBA)", 2D) = "black" {}
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
		_Splat2 ("Layer 3", 2D) = "white" {}
		_Splat3 ("Layer 4", 2D) = "white" {}
		_Splat4 ("Layer 5", 2D) = "white" {}
		_Splat5 ("Layer 6", 2D) = "white" {}
		_Splat6 ("Layer 7", 2D) = "white" {}
		_Splat7 ("Layer 8", 2D) = "white" {}

		_Normal0("Normal 1", 2D) = "bump" {}
		_Normal1("Normal 2", 2D) = "bump" {}
		_Normal2("Normal 3", 2D) = "bump" {}
		_Normal3("Normal 4", 2D) = "bump" {}
	}

	CGINCLUDE
		#pragma surface surf BlinnPhong vertex:MTE_SplatmapVert finalcolor:MTE_SplatmapFinalColor finalprepass:MTE_SplatmapFinalPrepass finalgbuffer:MTE_SplatmapFinalGBuffer
		#pragma multi_compile_fog

		struct Input
		{
			float4 tc;
			UNITY_FOG_COORDS(0)
		};

		sampler2D _Control,_ControlExtra;
		float4 _Control_ST,_ControlExtra_ST;
		sampler2D _Splat0,_Splat1,_Splat2,_Splat3,_Splat4,_Splat5,_Splat6,_Splat7;
		float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST,_Splat4_ST,_Splat5_ST,_Splat6_ST,_Splat7_ST;
		sampler2D _Normal0, _Normal1, _Normal2, _Normal3;

		#include "../../MTECommon.hlsl"

		half _Shininess;

		void MTE_SplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc = v.texcoord;
			float4 pos = UnityObjectToClipPos(v.vertex);
			UNITY_TRANSFER_FOG(data, pos);

			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = -1;
		}

		void MTE_SplatmapMix(Input IN, out half weight, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
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
			float2 uvSplat7 = TRANSFORM_TEX(IN.tc.xy, _Splat7);

			half4 splat_control = tex2D(_Control, uvControl);
			half4 splat_control_extra = tex2D(_ControlExtra, uvControlExtra);
			weight = dot(splat_control, half4(1, 1, 1, 1));
			weight += dot(splat_control_extra, half4(1, 1, 1, 1));
			splat_control /= (weight + 1e-3f);
			splat_control_extra /= (weight + 1e-3f);

			mixedDiffuse = 0;
			mixedDiffuse += splat_control.r       * tex2D(_Splat0, uvSplat0);
			mixedDiffuse += splat_control.g       * tex2D(_Splat1, uvSplat1);
			mixedDiffuse += splat_control.b       * tex2D(_Splat2, uvSplat2);
			mixedDiffuse += splat_control.a       * tex2D(_Splat3, uvSplat3);
			mixedDiffuse += splat_control_extra.r * tex2D(_Splat4, uvSplat4);
			mixedDiffuse += splat_control_extra.g * tex2D(_Splat5, uvSplat5);
			mixedDiffuse += splat_control_extra.b * tex2D(_Splat6, uvSplat6);
			mixedDiffuse += splat_control_extra.a * tex2D(_Splat7, uvSplat7);

			fixed4 nrm = 0.0f;
			nrm += splat_control.r       * tex2D(_Normal0, uvSplat0);
			nrm += splat_control.g       * tex2D(_Normal1, uvSplat1);
			nrm += splat_control.b       * tex2D(_Normal2, uvSplat2);
			nrm += splat_control.a       * tex2D(_Normal3, uvSplat3);
			nrm += splat_control_extra.r * fixed4(0.5, 0.5, 1, 0.5);
			nrm += splat_control_extra.g * fixed4(0.5, 0.5, 1, 0.5);
			nrm += splat_control_extra.b * fixed4(0.5, 0.5, 1, 0.5);
			nrm += splat_control_extra.a * fixed4(0.5, 0.5, 1, 0.5);
			mixedNormal = UnpackNormal(nrm);
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 mixedDiffuse;
			half weight;
			MTE_SplatmapMix(IN, weight, mixedDiffuse, o.Normal);
			o.Albedo = mixedDiffuse.rgb;
			o.Alpha = weight;
			o.Gloss = mixedDiffuse.a;
			o.Specular = _Shininess;
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

	Fallback "MTE/Surface/8 Textures/Bumped"
	CustomEditor "MTE.MTEShaderGUI"
}
