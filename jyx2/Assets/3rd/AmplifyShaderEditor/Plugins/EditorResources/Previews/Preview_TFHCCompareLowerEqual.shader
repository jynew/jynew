Shader "Hidden/TFHCCompareLessEqual"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
		_B ("_B", 2D) = "white" {}
		_C ("_True", 2D) = "white" {}
		_D ( "_False", 2D ) = "white" {}
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
			sampler2D _C;
			sampler2D _D;

			float4 frag(v2f_img i) : SV_Target
			{
				float A = tex2D( _A, i.uv ).x;
				float B = tex2D( _B, i.uv ).x;
				float4 True = tex2D( _C, i.uv );
				float4 False = tex2D ( _D, i.uv );
				return ( ( A <= B ) ? True : False );
			}
			ENDCG
		}
	}
}
