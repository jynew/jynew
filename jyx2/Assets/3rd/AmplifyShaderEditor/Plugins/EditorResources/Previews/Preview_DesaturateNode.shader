Shader "Hidden/DesaturateNode"
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
			
			float4 frag ( v2f_img i ) : SV_Target
			{
				float3 rgb = tex2D ( _A, i.uv ).rgb;
				float fraction = tex2D ( _B, i.uv ).r;

				float dotResult = dot ( rgb, float3( 0.299, 0.587, 0.114 ) );
				float3 finalColor = lerp ( rgb, dotResult.xxx, fraction );
				
				return float4( finalColor, 1 );
			}
			ENDCG
		}
	}
}
