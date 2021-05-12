Shader "Hidden/EncodeFloatRGNode"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
		

			float4 frag( v2f_img i ) : SV_Target
			{
				float a = tex2D( _A, i.uv ).r;
				return float4( EncodeFloatRG( a ), 0,0);
			}
			ENDCG
		}
	}
}
