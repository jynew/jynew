// Unlit map preview shader. Based on Unity's "Unlit/Texture".  Version 1.3
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Hidden/nu Assets/UI/CanvasChannels"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OffsetX ("Offset X", Float) = 0
		_OffsetY ("Offset Y", Float) = 0
		_Tile ("Tiling", Float) = 1

	}

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
				#pragma multi_compile_fog
			
				#include "UnityCG.cginc"
				#include "CanvasUtil.cginc"

				int _Channel;
	
			
				fixed4 frag (v2f i) : SV_Target
				{
					//half2 offset = half2(-_OffsetX, _OffsetY);
					//half2 uv = (i.texcoord + offset) * _Tile;
					//fixed4 col = tex2D(_MainTex, uv);

					half2 uv = TransformUV(i.texcoord);
					fixed4 col =  tex2D(_MainTex, uv);
					
					// Won't compile on OSX
					//col =    _Channel == RGB_ ? fixed4(col.rgb, 1.0h) : 
					//		(_Channel == RGBA_ ? col :
					//		(_Channel ==  R_ ? fixed4(col.r, col.r, col.r, 1) : 
					//		(_Channel ==  G_ ? fixed4(col.g, col.g, col.g, 1) : 
					//		(_Channel ==  B_ ? fixed4(col.b, col.b, col.b, 1) : 
					//		(_Channel ==  A_ ? fixed4(col.a, col.a, col.a, 1) : ((col.r + col.g + col.b)/3.0h) )))));

					if(_Channel == RGB_)
					{
						col = fixed4(col.rgb, 1.0h);
					}
					else if(_Channel == R_)
					{
						col = fixed4(col.r, col.r, col.r, 1);
					}
					else if(_Channel == G_)
					{
						col = fixed4(col.g, col.g, col.g, 1);
					}
					else if(_Channel == B_)
					{
						col = fixed4(col.b, col.b, col.b, 1);
					}
					else if(_Channel == A_)
					{
						col = fixed4(col.a, col.a, col.a, 1);
					}
					else
					{
						col = (col.r + col.g + col.b)/3.0h;
					}

					//col = DrawSeam(uv, col);


					return col;
				}
			ENDCG
		}
	}
}
