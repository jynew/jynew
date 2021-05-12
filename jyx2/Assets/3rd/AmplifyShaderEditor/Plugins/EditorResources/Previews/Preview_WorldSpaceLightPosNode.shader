Shader "Hidden/WorldSpaceLightPosNode"
{
	SubShader
	{
		CGINCLUDE
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag
		ENDCG

		Pass
		{
			CGPROGRAM
			float4 _EditorWorldLightPos;
			float4 frag( v2f_img i ) : SV_Target
			{
				float3 lightDir = normalize( _EditorWorldLightPos.xyz );
				return float4 ( lightDir, 0);
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			float4 frag( v2f_img i ) : SV_Target
			{
				return (0).xxxx; 
			}
			ENDCG
		}
	}
}
