// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//扰动
Shader "ThorShader/Effect/DisturbanceAddMask" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "black" {}//主贴图
	_MainUVspeed ("Main UVspeed",Vector ) = (0,0,0,0)
	_Color ("Tint Color", Color) = (1,1,1,1)
	_AlphaTex ("Alpha (RGB)", 2D) = "white" {}
	_noiseTex ("Noise Tex", 2D) = "white" {}//扰动噪点贴图
	_maskTex("Mask Tex", 2D) = "white" {} // 遮罩贴图
	//_UVTex ("UV Tex", 2D) = "Black" {}//UV贴图
	//_UVTintColor ("Tint Color", Color) = (1,1,1,1)
	//_UVspeed ("UV speed",Vector ) = (0,0,0,0)
	_HeatSpeed ("Heat speed",Vector) = (0,0,0,0)//扰动速度
	_HeatForce  ("Heat Force", range (0,0.2)) = 0//扰动强度0

	[KeywordEnum(Off, On)] DayNight ("Day Night", Int) = 0

	_Emission ("Emission", Range(0,8)) = 1
	[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
	//[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Float) = 1//SrcAlpha
	//[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Float) = 1//One
}

SubShader {
	Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}	
	LOD 100
	
	Lighting Off
	ZWrite Off
	Cull Off 
	Blend SrcAlpha One
	ColorMask RGB
	ZTest [_ZTest]
	Offset -1, -1
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest	
			#pragma shader_feature _ DAYNIGHT_ON
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 uvMain : TEXCOORD0;
				float2 uvMask : TEXCOORD1;
				//float2 texcoord2 : TEXCOORD2;
				float4 color : COLOR;
			};

			float4 _MainTex_ST;
			sampler2D _MainTex;
			sampler2D _AlphaTex;
			sampler2D _noiseTex;
			sampler2D _maskTex;
			float4 _maskTex_ST;
			//sampler2D _UVTex;
			//float4 _UVTex_ST;

		  
		  	half4 _Color;
		    //float4 _UVTintColor;
			//half4 _UVspeed;
			half4 _HeatSpeed;
		    half _HeatForce;
			half _Emission;

			float4 _MainUVspeed;
			half4 _DayNightTintInv;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uvMain = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uvMask = TRANSFORM_TEX(v.texcoord, _maskTex);
				//o.texcoord2 = TRANSFORM_TEX(v.texcoord, _UVTex);

				o.color = v.color;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//扰流
				float2 uv = i.uvMain;
				half4 maskCol = tex2D(_maskTex, i.uvMask);
				half4 offsetColor1 = tex2D(_noiseTex, uv + _Time.xz * _HeatSpeed.x);
			    half4 offsetColor2 = tex2D(_noiseTex, uv - _Time.yx * _HeatSpeed.y);
			    
				uv.x += ((offsetColor1.r + offsetColor2.r) - 1) * _HeatForce + _MainUVspeed.x * _Time.y;
				uv.y += ((offsetColor1.g + offsetColor2.g) - 1) * _HeatForce + _MainUVspeed.y * _Time.y;
				
				//float2 uv1 = i.texcoord1;
				//uv1.x += _UVspeed.x * _Time.y; 
				//uv1.y += _UVspeed.y * _Time.y;
				
				fixed4 mainCol = tex2D(_MainTex, uv);
				fixed4 alphaCol = tex2D(_AlphaTex, uv);			
				
				//fixed4 uvCol = tex2D(_UVTex, uv1) * alphaCol;
				
				fixed4 finalCol = mainCol  * _Emission * i.color * _Color;
				finalCol.a *= maskCol.a;

				#ifdef DAYNIGHT_ON
				finalCol.rgb *= (1 - _DayNightTintInv.rgb);
				#endif

				return finalCol;
			}
		ENDCG
		}
	}
	FallBack Off
}
