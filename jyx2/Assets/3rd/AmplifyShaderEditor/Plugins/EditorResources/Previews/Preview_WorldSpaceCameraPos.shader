Shader "Hidden/WorldSpaceCameraPos"
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
				//_WorldSpaceCameraPos
				return float4(float3(0,0,-5),0);
			}
			ENDCG
		}
	}
}
