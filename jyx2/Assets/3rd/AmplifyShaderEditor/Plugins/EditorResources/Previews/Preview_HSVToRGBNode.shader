Shader "Hidden/HSVToRGBNode"
{
	Properties
	{
		_A ( "_Hue",  2D ) = "white" {}
		_B ( "_Saturation",  2D ) = "white" {}
		_C ( "_Value",  2D ) = "white" {}
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			uniform sampler2D _A;
			uniform sampler2D _B;
			uniform sampler2D _C;

			float4 frag ( v2f_img i ) : SV_Target
			{
				float h = tex2D ( _A, i.uv ).r;
				float s = tex2D ( _B, i.uv ).r;
				float v = tex2D ( _C, i.uv ).r;

				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs ( frac ( (h).xxx + K.xyz ) * 6.0 - K.www );
				float3 hsvToRgb = v * lerp ( K.xxx, clamp ( p - K.xxx, 0.0, 1.0 ), (s).xxx );

				return float4( GammaToLinearSpace(hsvToRgb), 1 );
			}
			ENDCG
		}
	}
}
