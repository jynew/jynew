Shader "GPUInstancer/Billboard/2DRendererStandard" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_AlbedoAtlas ("Albedo Atlas", 2D) = "white" {}
		_NormalAtlas("Normal Atlas", 2D) = "white" {}
		_Cutoff ("Cutoff", Range(0,1)) = 0.5
		_FrameCount("_FrameCount", Float) = 8

		[Toggle(_BILLBOARDFACECAMPOS_ON)] _BillboardFaceCamPos("BillboardFaceCamPos", Float) = 0
	}
	SubShader {
		Tags { "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest"  "DisableBatching"="True" }
		LOD 400

		CGPROGRAM

		sampler2D _AlbedoAtlas;
		sampler2D _NormalAtlas;
		float _Cutoff;
		float _FrameCount;
		half4 _Color;

		#include "UnityCG.cginc"
		#include "../Include/GPUInstancerInclude.cginc"
		#include "../Include/GPUIBillboardInclude.cginc"

		#pragma multi_compile __ _BILLBOARDFACECAMPOS_ON
		#pragma instancing_options procedural:setupGPUI
		#pragma surface surf Standard noshadow vertex:vert //addshadow exclude_path:deferred
		#pragma multi_compile _ LOD_FADE_CROSSFADE
		#pragma target 4.5

		struct Input {
			float2 atlasUV;
			//float3 worldPosActual;
			float3 tangentWorld;
            float3 bitangentWorld;
            float3 normalWorld;
		};

		void vert(inout appdata_full v, out Input o) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			//float3 o.worldPosActual = unity_ObjectToWorld._m03_m13_m23;
			GPUIBillboardVertex(v.vertex, v.normal, v.tangent, o.tangentWorld, o.bitangentWorld, o.normalWorld, v.texcoord, o.atlasUV, _FrameCount);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 c = tex2D (_AlbedoAtlas, IN.atlasUV);
			clip(c.a - _Cutoff);
			o.Alpha = c.a;

			half4 normalDepth = GPUIBillboardNormals(_NormalAtlas, IN.atlasUV, _FrameCount, IN.tangentWorld, IN.bitangentWorld, IN.normalWorld);
			o.Normal = normalDepth.xyz;

			c.rgb = c.rgb * _Color;
			o.Albedo = lerp (c.rgb, float3(0,0,0), normalDepth.w);
		}
		ENDCG
	}

}