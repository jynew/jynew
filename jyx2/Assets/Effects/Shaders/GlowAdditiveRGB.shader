// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/GlowAdditiveRGB" {
	Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_CoreColor ("Core Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_TintStrength ("Tint Color Strength", Range(0, 5)) = 1
	_CoreStrength ("Core Color Strength", Range(0, 5)) = 1
	_CutOutLightCore ("CutOut Light Core", Range(0, 1)) = 0.5
	_Chanel ("Chanel Int R - 0 G - 1 B - 2", Range(0,2)) = 0
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	//AlphaTest Greater .01
	ColorMask RGB
	Cull Off 
	Lighting Off 
	ZWrite Off 
	Fog { Color (0,0,0,0) }
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
		
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			fixed4 _CoreColor;
			float _CutOutLightCore;
			float _TintStrength;
			float _CoreStrength;
			int _Chanel;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			sampler2D _CameraDepthTexture;
			float _InvFade;
			
			fixed4 frag (v2f i) : COLOR
			{

				fixed4 tex = tex2D(_MainTex, i.texcoord);
				fixed texCol = _Chanel == 0 ? tex.r : _Chanel == 1 ? tex.g : tex.b;
				fixed a = step(texCol, _CutOutLightCore);
				fixed4 col = texCol * (a * _TintColor * _TintStrength + (1 - a) *_CoreColor * _CoreStrength);
				//if(texCol > _CutOutLightCore) 
				//	col = texCol * _CoreColor * _CoreStrength;
				//else 
				//	col = texCol * _TintColor * _TintStrength;
				return i.color * clamp(col, 0, 255);
			}
			ENDCG 
		}
	}	
}
}
