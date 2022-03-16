Shader "MTE/VertexColored/ColorOnly"
{
	Properties
	{
	}

	CGINCLUDE
		#pragma surface surf Lambert vertex:vert finalcolor:MTE_SplatmapFinalColor finalprepass:MTE_SplatmapFinalPrepass finalgbuffer:MTE_SplatmapFinalGBuffer
		#pragma multi_compile_fog

		struct Input
		{
			float3 color: COLOR;
			UNITY_FOG_COORDS(0)
		};

		#include "../../MTECommon.hlsl"

		void vert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			float4 pos = UnityObjectToClipPos(v.vertex);
			UNITY_TRANSFER_FOG(data, pos);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			o.Albedo = IN.color.rgb;
			o.Alpha = 1.0;
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

	FallBack "Diffuse"
}
