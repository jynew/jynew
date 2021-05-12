Shader "Hidden/RangedFloatNode"
{
	Properties {
		_InputFloat ("_InputFloat", Float) = 0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			float _InputFloat;

			float4 frag( v2f_img i ) : SV_Target
			{
				return _InputFloat;
			}
			ENDCG
		}
	}
}
