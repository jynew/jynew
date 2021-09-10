// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "cgwell/Additive Mask" 
{
	Properties 
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_Mask ("Mask ( R Channel )", 2D) = "white" {}
		[HideInInspector]_Center ("Center",Vector) = (0,0,0,1)
		[HideInInspector]_Scale ("Scale",Vector) = (1,1,1,1)
		[HideInInspector]_Normal ("Normal",Vector) = (0,0,1,0)
	}

	Category 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
		SubShader 
		{
			Pass 
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile SCALE_OFF SCALE_ON
				#pragma multi_compile MIRROR_OFF MIRROR_ON
				#pragma multi_compile MESH BILLBOARD
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				sampler2D _Mask;
				fixed4 _TintColor;
			
				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 texcoordMask : TEXCOORD1;
				};
			
				float4 _MainTex_ST;
				float4 _Mask_ST;

				float4 _Center;
				float4 _Scale;
				float4 _Normal;

				uniform float4x4 _Camera2World;

				v2f vert (appdata_t v)
				{
					v2f o;
					
					#if SCALE_ON
					float4 worldpos;
					#if BILLBOARD
					worldpos = mul(_Camera2World,v.vertex);
					#else //BILLBOARD
					worldpos = mul(unity_ObjectToWorld,v.vertex);		
					#endif //BILLBOARD
					#if MIRROR_ON
					float3 srcDir = _Center.xyz - worldpos.xyz;
					float3 refDir = reflect(srcDir,_Normal.xyz);
					refDir.y = -srcDir.y;
					worldpos.xyz = refDir *_Scale.xyz + _Center.xyz;
					#else //MIRROR_ON 
					worldpos.xyz = (worldpos.xyz-_Center.xyz)*_Scale.xyz + _Center.xyz;
					#endif //MIRROR_ON 
					o.vertex = mul(UNITY_MATRIX_VP, worldpos);
					#else //SCALE_ON
					 o.vertex = UnityObjectToClipPos(v.vertex);
					#endif //SCALE_ON

					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.texcoordMask = TRANSFORM_TEX(v.texcoord,_Mask);
					return o;
				}
			
				fixed4 frag (v2f i) : SV_Target
				{
				    fixed4 c = tex2D(_MainTex, i.texcoord);
					c.a *= tex2D(_Mask, i.texcoordMask).r;
					return 2.0f * i.color * _TintColor * c;
				}
				ENDCG 
			}
		}
		
		// ---- Dual texture cards
		SubShader 
		{
			Pass 
			{
				SetTexture [_MainTex] 
				{
					constantColor [_TintColor]
					combine constant * primary
				}
				SetTexture [_MainTex] 
				{
					combine texture * previous DOUBLE
				}
			}
		}
	
		// ---- Single texture cards (does not do color tint)
		SubShader 
		{
			Pass 
			{
				SetTexture [_MainTex] 
				{
					combine texture * primary
				}
			}
		}	
	}
}
