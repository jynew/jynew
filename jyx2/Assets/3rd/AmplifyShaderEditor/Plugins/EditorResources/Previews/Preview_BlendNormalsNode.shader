Shader "Hidden/BlendNormalsNode"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
		_B ("_B", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"

			sampler2D _A;
			sampler2D _B;

			float4 frag(v2f_img i) : SV_Target
			{
				float3 a = tex2D( _A, i.uv ).rgb;
				float3 b = tex2D( _B, i.uv ).rgb;
				return float4(BlendNormals(a, b), 0);
			}
			ENDCG
		}
	}
}
