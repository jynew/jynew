Shader "Hidden/TFHCCompareWithRange"
{
	Properties
	{
		_A ("_Value", 2D) = "white" {}
		_B ("_RangeMin", 2D) = "white" {}
		_C ("_RangeMax", 2D) = "white" {}
		_D ( "_True", 2D ) = "white" {}
		_E ( "_False", 2D ) = "white" {}
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
			sampler2D _E;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 Value = tex2D( _A, i.uv ).x;
				float4 RangeMin = tex2D( _B, i.uv ).x;
				float4 RangeMax = tex2D( _C, i.uv );
				float4 True = tex2D ( _D, i.uv );
				float4 False = tex2D ( _E, i.uv );
				return ( ( Value >= RangeMin && Value <= RangeMax ) ? True : False );
			}
			ENDCG
		}
	}
}
