Shader "Hidden/FunctionInputNode"
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
			int _Type;

			float4 frag(v2f_img i) : SV_Target
			{
				if( _Type == 1 )
				{
					return tex2D( _A, i.uv ).r;
				} else if( _Type == 2 )
				{
					return float4(tex2D( _A, i.uv ).rg,0,0);
				} else if( _Type == 3 )
				{
					return float4(tex2D( _A, i.uv ).rgb,0);
				}
				else
				{
					return tex2D( _A, i.uv );
				}
			}
			ENDCG
		}
	}
}
