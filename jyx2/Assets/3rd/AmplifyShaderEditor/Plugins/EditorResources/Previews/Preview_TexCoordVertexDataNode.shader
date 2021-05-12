Shader "Hidden/TexCoordVertexDataNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag( v2f_img i ) : SV_Target
			{
				return float4( i.uv, 0, 0 );
			}
			ENDCG
		}
	}
}
