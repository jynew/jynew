Shader "Hidden/RefractOpVec"
{
	Properties
	{
		_A ("_Incident", 2D) = "white" {}
		_B ("_Normal", 2D) = "white" {}
		_C ("_Eta", 2D) = "white" {}
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

			float4 frag(v2f_img i) : SV_Target
			{
				float4 inc = tex2D( _A, i.uv );
				float4 nor = tex2D( _B, i.uv );
				float4 eta = tex2D( _C, i.uv );
				return refract( inc, nor, eta );
			}
			ENDCG
		}
	}
}
