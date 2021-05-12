Shader "Hidden/UnpackScaleNormalNode"
{
	Properties
	{
		_A ("_Value", 2D) = "white" {}
		_B ("_NormalScale", 2D) = "white" {}
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

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 c = tex2D( _A, i.uv );
				float n = tex2D( _B, i.uv ).r;
				c.rgb = UnpackScaleNormal( c, n );

				return float4(c.rgb, 0);
			}
			ENDCG
		}
	}
}
