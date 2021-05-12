Shader "Hidden/PiNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _A;

			float4 frag(v2f_img i) : SV_Target
			{
				return tex2D( _A, i.uv ).r * UNITY_PI;
			}
			ENDCG
		}
	}
}
