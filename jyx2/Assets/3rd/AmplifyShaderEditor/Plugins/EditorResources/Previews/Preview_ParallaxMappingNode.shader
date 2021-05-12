Shader "Hidden/ParallaxMappingNode"
{
	Properties
	{
		_A ("_UV", 2D) = "white" {}
		_B ("_Height", 2D) = "white" {}
		_C ("_Scale", 2D) = "white" {}
		_D ("_ViewDirTan", 2D) = "white" {}
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
			float _ParallaxType;

			float4 frag(v2f_img i) : SV_Target
			{
				float2 uv = tex2D( _A, i.uv ).rg;
				float h = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float3 vt = tex2D( _D, i.uv ).xyz;
				float2 parallaxed = uv;
				if ( _ParallaxType == 1 ) {
					parallaxed = ( ( h - 1 )*( vt.xy / vt.z ) * s ) + uv;
				}
				else {
					parallaxed = ( ( h - 1 )*( vt.xy ) * s ) + uv;
				}

				return float4(parallaxed, 0 , 0);
			}
			ENDCG
		}
	}
}
