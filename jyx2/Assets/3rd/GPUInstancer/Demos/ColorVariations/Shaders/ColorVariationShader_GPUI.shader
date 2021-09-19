Shader "GPUInstancer/ColorVariationShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#include "UnityCG.cginc"
		#include "./../../../Shaders/Include/GPUInstancerInclude.cginc"
		#pragma instancing_options procedural:setupGPUI
		#pragma multi_compile_instancing
		#pragma surface surf Standard addshadow fullforwardshadows vertex:colorVariationVert

		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 colorVariation;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			StructuredBuffer<float4> colorBuffer;
		#endif
		
		void colorVariationVert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);

			o.colorVariation = _Color;

			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				o.colorVariation = colorBuffer[gpui_InstanceID];
			#endif
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * saturate(IN.colorVariation);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}

		ENDCG
	}
	FallBack "Standard"
}
