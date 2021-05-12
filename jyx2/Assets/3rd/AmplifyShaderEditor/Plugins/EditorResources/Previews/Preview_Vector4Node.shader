Shader "Hidden/Vector4Node"
{
	Properties {
		_InputVector ("_InputVector", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			float4 _InputVector;

			float4 frag( v2f_img i ) : SV_Target
			{
				return _InputVector;
			}
			ENDCG
		}
	}
}
