Shader "Hidden/DecodeDepthNormalNode"
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
		

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float depthValue = 0;
				float3 normalValue = 0;
				DecodeDepthNormal( a , depthValue, normalValue );
				return float4( depthValue,normalValue );
			}
			ENDCG
		}
	}
}
