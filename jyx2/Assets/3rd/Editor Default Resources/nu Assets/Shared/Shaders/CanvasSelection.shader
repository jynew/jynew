// Selection preview shader. Based on Unity's "Unlit/Texture". Version 1.3
// Copyright (c) by Sycoforge

Shader "Hidden/nu Assets/UI/CanvasSelection"
{
	SubShader 
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Pass 
		{  
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			
				#include "UnityCG.cginc"
				#include "CanvasUtil.cginc"


				//Texel size of canvas texture
				float2 _TexelSize;
				float4 _Selection;
				float _EditorTime;
				
				#define widthBlack 10
				#define widthWhite 5

				fixed4 frag (v2f i) : SV_Target
				{
					half2 uv = TransformUV(i.texcoord);


					fixed3 c = RectBorderClamped(uv, _Selection, _TexelSize);

					fixed vertical = c.x;
					fixed horizontal = c.y;
					fixed inside = c.z;

					fixed clamped = (c.x + c.y) * c.z;

					fixed2 p = fmod(abs(uv + _EditorTime), _TexelSize * widthBlack);
					fixed2 ap = step(p, _TexelSize * widthWhite);
	

					fixed a = !vertical ? ap.y : ap.x;


					fixed4 col = clamped? fixed4(a, a, a, inside) : fixed4(0, 0, 0, 0);



					return col;
				}

				fixed4 frag2 (v2f i) : SV_Target
				{
					half2 offset = half2(-_OffsetX, _OffsetY);
					half2 uv = (i.texcoord + offset) * _Tile;
					fixed4 col = tex2D(_MainTex, uv);


					fixed cUp = tex2D(_MainTex, uv + fixed2(0, _TexelSize.y)).a;
					fixed cDown = tex2D(_MainTex, uv + fixed2(0, -_TexelSize.y)).a;
					fixed cLeft = tex2D(_MainTex, uv + fixed2(-_TexelSize.x, 0)).a;
					fixed cRight = tex2D(_MainTex, uv + fixed2(_TexelSize.x, 0)).a;
					
					fixed vertical = cUp * cDown;
					fixed horizontal = cLeft * cRight;

					fixed clamped = vertical * horizontal;


					fixed2 p = fmod(abs(uv+_EditorTime), _TexelSize * widthBlack);
					fixed2 ap = step(p, _TexelSize * widthWhite);
	

					fixed a = vertical < 1 ? ap.x : ap.y;

					col = clamped < 0.9 ? fixed4(a, a, a, col.a) : fixed4(1, 0, 0, 1) * _Time.y;


					return col;
				}
			ENDCG
		}
	}
}
