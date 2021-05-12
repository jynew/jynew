Shader "Hidden/CrossProductOpNode"
{
	Properties
	{
		_A ("_Lhs", 2D) = "white" {}
		_B ("_Rhs", 2D) = "white" {}
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
			sampler2D _B;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 b = tex2D( _B, i.uv );
				return float4(cross(a.rgb, b.rgb),0);
			}
			ENDCG
		}
	}
}
