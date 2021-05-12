Shader "Hidden/NormalVertexDataNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 normal = normalize(float3(xy, z));
				return float4(normal, 1);
			}
			ENDCG
		}
	}
}
