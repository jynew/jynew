// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/ProGrids/pg_GridShader"
{
	Properties
	{
		_AlphaCutoff ("Fade Cutoff", float) = .1
		_AlphaFade ("Fade Start", float) = .6
		_Color ("Color", Color) = ( .5, .5, .5, .8 )
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass 
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _AlphaCutoff;
			float _AlphaFade;
			float4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 world : TEXCOORD0;
				float4 color : COLOR;
				float3 normal : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.world = mul(unity_ObjectToWorld, v.vertex);
				o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz;
				o.color = v.color;

				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.world.xyz);
				float d = abs(dot(i.normal, viewDir));
				float alpha = smoothstep(_AlphaCutoff, _AlphaFade, d);
				float4 col = i.color;
				col.a *= alpha;
				return col;
			}

			ENDCG
		}
	}
}
