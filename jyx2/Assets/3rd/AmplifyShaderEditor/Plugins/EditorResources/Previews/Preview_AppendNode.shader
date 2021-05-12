Shader "Hidden/AppendNode"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
		_B ("_B", 2D) = "white" {}
		_C ("_C", 2D) = "white" {}
		_D ("_D", 2D) = "white" {}
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
				float x = tex2D(_A, i.uv).x;
				float y = tex2D(_B, i.uv).y;
				float z = tex2D(_C, i.uv).z;
				float w = tex2D(_D, i.uv).w;

				return float4(x,y,z,w);
			}
			ENDCG
		}
	}
}
