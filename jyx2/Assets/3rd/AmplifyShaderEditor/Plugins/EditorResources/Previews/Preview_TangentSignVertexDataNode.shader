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
				//float2 tp = 2 * i.uv - 1;
				//float tr = sqrt( dot(tp,tp) );
				//tr = saturate( tr );
				
				//float2 tuvs;
				//float f = ( 1 - sqrt( 1 - tr ) ) / tr;

				//float3 tangent = normalize(float3( (1-f)*2, tp.y*0.01, tp.x ));

				//float4((tangent), 1);

				//TODO: needs revision at some point
				return -1;
			}
			ENDCG
		}
	}
}
