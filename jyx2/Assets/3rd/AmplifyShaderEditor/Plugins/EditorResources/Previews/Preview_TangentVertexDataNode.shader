Shader "Hidden/TangentVertexDataNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 frag( v2f_img i ) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 tangent = normalize(float3( -z, xy.y*0.01, xy.x ));
				return float4((tangent), 1);
			}
			ENDCG
		}
	}
}
