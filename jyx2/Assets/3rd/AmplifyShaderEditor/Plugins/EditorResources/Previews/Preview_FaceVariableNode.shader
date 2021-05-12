Shader "Hidden/FaceVariableNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag( v2f_img i, half ase_vface : VFACE ) : SV_Target
			{
				return ase_vface;
			}
			ENDCG
		}
	}
}
