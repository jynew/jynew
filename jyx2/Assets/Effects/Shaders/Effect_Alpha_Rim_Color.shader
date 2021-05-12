// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MaShen/Effect/Effect_Alpha_Rim_Color" {
	Properties{
		_RimColor("Rim Color", Color) = (0.5,0.5,0.5,0.5)
		_InnerColor("Inner Color", Color) = (0.5,0.5,0.5,0.5)
		_InnerColorPower("Inner Color Power", Range(0.0,1.0)) = 0.5
		//_RimPower ("Rim Power", Range(0.0,5.0)) = 2.5
		//_AlphaPower ("Alpha Rim Power", Range(0.0,8.0)) = 4.0
		_AllPower("All Power", Range(0.0, 10.0)) = 1.0
		_RimWidth("Rim width", Float) = 0
		_RimIntensity("Rim Intensity", Float) = 1
		_RimWidthAlpha("Rim width Alpha", Float) = 0
		_RimIntensityAlpha("Rim Intensity Alpha", Float) = 1
	}
	CGINCLUDE
	#include "UnityCG.cginc"

	ENDCG

	SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass{
			Tags { "LightMode" = "Vertex" }
			Blend SrcAlpha OneMinusSrcAlpha		
			ZWrite Off

			CGPROGRAM	
			//pragma	
			#pragma vertex vert
			#pragma fragment frag

			uniform fixed4 _RimColor;
			//uniform fixed _RimPower;
			//uniform fixed _AlphaPower;
			uniform fixed _InnerColorPower;
			uniform fixed _AllPower;
			uniform fixed4 _InnerColor;

			uniform fixed _RimWidth;
			uniform fixed _RimIntensity;
			uniform fixed _RimWidthAlpha;
			uniform fixed _RimIntensityAlpha;

			struct Input {
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed2 texcoord : TEXCOORD0;
			};

			struct Output {
				fixed4	pos : SV_POSITION;
				fixed2	uv : TEXCOORD0; // _MainTex
				fixed3	worldNormal : TEXCOORD1;
				fixed3  worldViewDir : TEXCOORD3;
			};

			Output vert(Input v)
			{
				Output o;
				UNITY_INITIALIZE_OUTPUT(Output, o);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;

				fixed3 viewpos = mul(UNITY_MATRIX_MV, v.vertex).xyz;
				fixed3 viewN = mul((fixed3x3)UNITY_MATRIX_IT_MV, v.normal);

				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldNormal = worldNormal;

				o.worldViewDir = normalize(WorldSpaceViewDir(v.vertex));

				return o;
			}
			fixed4 frag(Output IN) : SV_Target{
				fixed4 c;
				fixed rim = 1.0 - saturate(dot(normalize(IN.worldViewDir), IN.worldNormal));
				fixed colorRim = smoothstep(1 - _RimWidth, 1.0, rim) * _RimIntensity;
				c.rgb = _RimColor.rgb * colorRim *_AllPower + (_InnerColor.rgb * 2 * _InnerColorPower);
				fixed alphaRim = smoothstep(1 - _RimWidthAlpha, 1.0, rim) * _RimIntensityAlpha;
				c.a = alphaRim*_AllPower;

				return c;
			}


			ENDCG
		}
	}
	FallBack Off
}
