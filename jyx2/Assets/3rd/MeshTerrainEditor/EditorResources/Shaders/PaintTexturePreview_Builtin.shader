// UNITY_SHADER_NO_UPGRADE
Shader "Hidden/MTE/PaintTexturePreview"
{
	Properties
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex ("Normalmap", 2D) = "bump" {}
		_MaskTex ("Mask (RGB) Trans (A)", 2D) = "white" {}
	}

	CGINCLUDE
		#pragma surface surf Lambert vertex:vert alpha:blend nodynlightmap nolightmap nofog
		#include <Lighting.cginc>
		#include <UnityCG.cginc>

		struct Input
		{
			float4 pos  : POSITION;
			float4 tex0 : TEXCOORD0;
			float4 tex1 : TEXCOORD1;
		};

		sampler2D _MainTex;
		sampler2D _MaskTex;
		sampler2D _NormalTex;

		float4x4 unity_Projector;
		float4 _MainTex_ST;
	
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.pos = UnityObjectToClipPos(v.vertex);
			o.tex0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.tex1 = mul(unity_Projector, v.vertex);
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			float4 color = tex2D(_MainTex, IN.tex0.xy);
			float4 mask = tex2Dproj(_MaskTex, UNITY_PROJ_COORD(IN.tex1));
			o.Albedo = color.rgb;
			o.Alpha = mask.a;
			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.tex0.xy));
			o.Emission = 0;
		}
	ENDCG

	Category
	{
		Tags { "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

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
}
