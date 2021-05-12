Shader "Hidden/ScaleAndOffsetNode"
{
	Properties
	{
		_A ("_Value", 2D) = "white" {}
		_B ("_Scale", 2D) = "white" {}
		_C ("_Offset", 2D) = "white" {}
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
				float4 v = tex2D( _A, i.uv );
				float4 s = tex2D( _B, i.uv );
				float4 o = tex2D( _C, i.uv );

				return v * s + o;
			}
			ENDCG
		}
	}
}
