Shader "Hidden/SimpleContrastNode"
{
	Properties
	{
		_A ( "_RBG",  2D ) = "white" {}
		_B ( "_Fraction",  2D ) = "white" {}
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
			
			float4 CalculateContrast ( float contrastValue, float4 colorTarget )
			{
				float t = 0.5 * ( 1.0 - contrastValue );
				return mul ( float4x4( contrastValue, 0, 0, t, 0, contrastValue, 0, t, 0, 0, contrastValue, t, 0, 0, 0, 1 ), colorTarget );
			}

			float4 frag ( v2f_img i ) : SV_Target
			{
				float4 rgba = tex2D ( _B, i.uv );
				float value = tex2D ( _A, i.uv ).r;

				float4 finalColor = CalculateContrast( value , rgba );
				
				return finalColor;
			}
			ENDCG
		}
	}
}
