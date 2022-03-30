Shader "MTE/VertexColored/Bumped Diffuse"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Normal ("Normalmap", 2D) = "bump" {}
	}

	CGINCLUDE
		#pragma surface surf Lambert vertex:vert finalcolor:MTE_SplatmapFinalColor finalprepass:MTE_SplatmapFinalPrepass finalgbuffer:MTE_SplatmapFinalGBuffer
		#pragma multi_compile_fog

		struct Input
		{
			float2 uv_MainTex : TEXCOORD0;
			UNITY_FOG_COORDS(1)
			float3 color: COLOR;
		};

		sampler2D _MainTex;
		sampler2D _Normal;

		#include "../../MTECommon.hlsl"

		void vert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			float4 pos = UnityObjectToClipPos(v.vertex);
			UNITY_TRANSFER_FOG(data, pos);
			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = -1;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * IN.color.rgb;
			o.Alpha = 1.0;
			o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_MainTex));
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