Shader "Unlit/Colored Transparent" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_SecondTex ("Second (RGB) Trans (A)", 2D) = "white" {}
		_ThirdTex ("Third (RGB) Trans (A)", 2D) = "white" {}
		_FourthTex ("Fourth (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}

	SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
		Pass {  // SINGLE LINE
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_Position;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
					float2 clipUV : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Color;
				uniform float4x4 unity_GUIClipTextureMatrix;
				sampler2D _GUIClipTexture;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color * _Color;
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					float3 eyePos = UnityObjectToViewPos( v.vertex );
					o.clipUV = mul( unity_GUIClipTextureMatrix, float4( eyePos.xy, 0, 1.0 ) );
					return o;
				}
				
				fixed4 frag( v2f i ) : SV_Target
				{
					float4 l1 = tex2D( _MainTex, i.texcoord);
					float clipAlpha = tex2D( _GUIClipTexture, i.clipUV ).a;
					l1.rgb *= i.color.rgb;
					l1.a *= clipAlpha;
					return l1;
				}
			ENDCG
		}

		Pass {  // MULTI LINE
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_Position;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
					float2 clipUV : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _SecondTex;
				float4 _SecondTex_ST;
				sampler2D _ThirdTex;
				float4 _ThirdTex_ST;
				sampler2D _FourthTex;
				float4 _FourthTex_ST;
				float4 _Color;
				uniform float4x4 unity_GUIClipTextureMatrix;
				sampler2D _GUIClipTexture;
				float _InvertedZoom;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color * _Color;
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					float3 eyePos = UnityObjectToViewPos( v.vertex );
					o.clipUV = mul( unity_GUIClipTextureMatrix, float4( eyePos.xy, 0, 1.0 ) );
					return o;
				}
				
				fixed4 frag( v2f i ) : SV_Target
				{
					float4 l1 = tex2D( _MainTex, i.texcoord);
					float clipAlpha = tex2D( _GUIClipTexture, i.clipUV ).a;
					l1.rgb *= i.color.rgb;
					l1.a *= clipAlpha;
					
					float4 l2 = tex2D( _SecondTex, i.texcoord);
					float4 l3 = tex2D( _ThirdTex, i.texcoord);
					float4 l4 = tex2D( _FourthTex, i.texcoord);

					float2 coords2 = i.texcoord;
					coords2.y *= 2;
					float4 m2 = tex2D( _MainTex, coords2 );
					m2 = pow( m2, 0.9 );

					float2 coords3 = i.texcoord;
					coords3.y *= 3;
					float4 m3 = tex2D( _MainTex, coords3 );
					m3 = pow( m3, 0.8 );

					float2 coords4 = i.texcoord;
					coords4.y *= 4;
					float4 m4 = tex2D( _MainTex, coords4 );
					m4 = pow( m4, 0.7 );

					l2.rgb *= i.color.rgb;
					l3.rgb *= i.color.rgb;
					l4.rgb *= i.color.rgb;

					m2.rgb *= i.color.rgb;
					m3.rgb *= i.color.rgb;
					m4.rgb *= i.color.rgb;

					l2.a *= clipAlpha;
					l3.a *= clipAlpha;
					l4.a *= clipAlpha;

					m2.a *= clipAlpha;
					m3.a *= clipAlpha;
					m4.a *= clipAlpha;

					float zoomLerp = saturate( ( ( _InvertedZoom ) * 2 ) - 0.0 );

					if ( i.color.a >= 1 )
						return lerp( l4, m4, zoomLerp );
					else if ( i.color.a >= 0.75 )
						return lerp( l3, m3, zoomLerp );
					else if ( i.color.a >= 0.5 )
						return lerp( l2, m2, zoomLerp );
					else
						return l1;
				}
			ENDCG
		}
	}
}
