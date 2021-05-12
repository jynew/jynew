Shader "Hidden/ColorNode"
{
	Properties {
		_InputColor ("_InputColor", Color) = (0,0,0,0)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			float4 _InputColor;

			float4 frag( v2f_img i ) : SV_Target
			{
				return _InputColor;
			}
			ENDCG
		}
	}
}
