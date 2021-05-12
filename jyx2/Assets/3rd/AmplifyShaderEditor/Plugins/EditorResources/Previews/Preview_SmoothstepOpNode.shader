Shader "Hidden/SmoothstepOpNode"
{
	Properties
	{
		_A ("_Alpha", 2D) = "white" {}
		_B ("_Min", 2D) = "white" {}
		_C ("_Max", 2D) = "white" {}
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
				float4 alpha = tex2D( _A, i.uv );
				float4 min = tex2D( _B, i.uv );
				float4 max = tex2D( _C, i.uv );
				return smoothstep(min, max, alpha);
			}
			ENDCG
		}
	}
}
