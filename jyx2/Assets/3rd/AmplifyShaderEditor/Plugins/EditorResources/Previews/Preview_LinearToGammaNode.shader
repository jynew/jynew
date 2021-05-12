Shader "Hidden/LinearToGammaNode"
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

			float4 frag(v2f_img i) : SV_Target
			{
				float4 c = tex2D( _A, i.uv );
				c.rgb = LinearToGammaSpace( c.rgb );
				return c;
			}
			ENDCG
		}
	}
}
