// Unlit map preview shader. Based on Unity's "Unlit/Texture".  Version 1.3
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Hidden/nu Assets/UI/CanvasOpaque"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OffsetX ("Offset X", Float) = 0
		_OffsetY ("Offset Y", Float) = 0
		_Tile ("Tiling", Float) = 1
		_SeamColor ("Zoom", Color) = (1,1,1,0.0)
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Pass 
		{  
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
			
				#include "UnityCG.cginc"
				#include "CanvasUtil.cginc"


			
				fixed4 frag (v2f i) : SV_Target
				{					
					half2 uv = TransformUV(i.texcoord);
					fixed4 col =  tex2D(_MainTex, uv);

					col = DrawSeam(uv, col);
					col = ClampClipSpace(uv, col);

					return col;
				}
			ENDCG
		}
	}
}
