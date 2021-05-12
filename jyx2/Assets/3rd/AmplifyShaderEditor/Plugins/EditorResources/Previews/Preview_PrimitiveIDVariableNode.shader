Shader "Hidden/PrimitiveIDVariableNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag( v2f_img i, uint ase_primitiveId : SV_PrimitiveID ) : SV_Target
			{
				return ase_primitiveId;
			}
			ENDCG
		}
	}
}
