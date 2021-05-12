Shader "Hidden/TFHCIf"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
		_B ("_B", 2D) = "white" {}
		_C ("_AGreaterB", 2D) = "white" {}
		_D ( "_AEqualsB", 2D ) = "white" {}
		_E ( "_ALessB", 2D ) = "white" {}
		_F ( "_EqualThreshold", 2D ) = "white" {}
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
			sampler2D _F;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 A = tex2D( _A, i.uv ).x;
				float4 B = tex2D( _B, i.uv ).x;
				float4 AGreaterB = tex2D( _C, i.uv );
				float4 AEqualsB = tex2D ( _D, i.uv );
				float4 ALessB = tex2D ( _E, i.uv );
				float4 EqualThreshold = tex2D ( _F, i.uv );
				return ( A - EqualThreshold > B ? AGreaterB : A - EqualThreshold <= B && A + EqualThreshold >= B ? AEqualsB : ALessB );
			}
			ENDCG
		}
	}
}
