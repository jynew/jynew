Shader "Hidden/TauNode"
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
				return UNITY_PI * 2;
			}
			ENDCG
		}
	}
}
