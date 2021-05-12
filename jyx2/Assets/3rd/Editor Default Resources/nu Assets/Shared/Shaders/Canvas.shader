// Unlit map preview shader. Based on Unity's "Unlit/Texture".  Version 1.3
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Hidden/nu Assets/UI/Canvas"
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
				#pragma multi_compile_fog
			
				#include "UnityCG.cginc"
				#include "CanvasUtil.cginc"



			
				fixed4 frag (v2f i) : SV_Target
				{
					//half2 offset = half2(-_OffsetX, _OffsetY);
					//half2 uv = (i.texcoord + offset) * _Tile;
					//fixed4 col = tex2D(_MainTex, uv);
					
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
