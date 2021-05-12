Shader "Hidden/Texture2D"
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
				return 0;
			}
			ENDCG
		}
	}
}
