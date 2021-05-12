Shader "Hidden/TFHCPixelateUV"
{
	Properties
	{
		_A ("_UV", 2D) = "white" {}
		_B ("_PixelX", 2D) = "white" {}
		_C ("_PixelY", 2D) = "white" {}
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
				float2 uv = tex2D( _A, i.uv ).rg;
				float pix = tex2D( _B, i.uv ).r;
				float piy = tex2D( _C, i.uv ).r;

				float2 steppedPixel = float2( pix, piy );
				float2 pixelatedUV = floor( uv * steppedPixel ) / steppedPixel;
				return float4(pixelatedUV, 0 , 0);
			}
			ENDCG
		}
	}
}
