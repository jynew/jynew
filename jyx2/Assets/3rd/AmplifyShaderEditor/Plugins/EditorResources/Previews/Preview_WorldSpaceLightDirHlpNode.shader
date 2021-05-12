Shader "Hidden/WorldSpaceLightDirHlpNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _EditorWorldLightPos;

			float4 frag( v2f_img i ) : SV_Target
			{
				float3 lightDir = normalize( _EditorWorldLightPos.xyz );
				return float4 ( lightDir, 1);
			}
			ENDCG
		}
	}
}
