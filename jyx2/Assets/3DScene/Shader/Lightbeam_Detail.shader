// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Lightbeam/Lightbeam Detail" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DetailTex ("Detail Texture", 2D) = "white" {}
		_Width ("Width", Float) = 8.71
		_Tweak ("Tweak", Float) = 0.65
		_DetailContrast ("Detail Strength", Range(0, 1)) = 0
		_DetailAnim ("Detail Animation", Vector) = (0,0,0,0)
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True"}
		Pass {
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _DetailTex;
			uniform float4 _DetailTex_ST;
			float4 _Color;
			float _Width;
			float _Tweak;
			float _DetailContrast;
			float4 _DetailAnim;
			
			struct v2f 
			{
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
			    float4 falloffUVs : TEXCOORD1;
			    float4 screenPos : TEXCOORD2;
			};
			
			v2f vert (appdata_tan v)
			{
			    v2f o;			    		
			    o.pos = UnityObjectToClipPos( v.vertex );
							
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _DetailTex);
				o.uv.xy += float2(_DetailAnim.x, _DetailAnim.y) * _Time.x;
								
				// Generate the falloff texture UVs
				TANGENT_SPACE_ROTATION;	
				float3 refVector = mul(rotation, normalize(ObjSpaceViewDir(v.vertex)));
				
				float z = sqrt((refVector.z + _Tweak) * _Width);
				float x = (refVector.x / z) + 0.5;
				float y = (refVector.y / z) + 0.5;
				
				float2 uv1 = float2(x, v.texcoord.y);
				float2 uv2 = float2(x, y);				
				o.falloffUVs = float4(uv1, uv2);
				
				o.screenPos = ComputeScreenPos(o.pos);
				COMPUTE_EYEDEPTH(o.screenPos.z);
								
			    return o;
			}
						
			half4 frag( v2f In ) : COLOR
			{			
				fixed falloff1 = tex2D(_MainTex, In.falloffUVs.xy).r;
				fixed falloff2 = tex2D(_MainTex, In.falloffUVs.zw).g;
				
				float4 c = _Color;
				c.a *= falloff1 * falloff2;				
				
				// Detail Texture
				float2 uv = In.uv.xy;
				float4 detailTex = tex2D(_DetailTex, uv);
				detailTex = lerp(float4(1,1,1,1), detailTex, _DetailContrast);
				c *= detailTex;
												
				// Fade when near the camera
				c.a *=  saturate(In.screenPos.z * 0.2);

			    return c;
			}
			
			ENDCG
		}
	} 
	Fallback "Lightbeam/Lightbeam"
}
