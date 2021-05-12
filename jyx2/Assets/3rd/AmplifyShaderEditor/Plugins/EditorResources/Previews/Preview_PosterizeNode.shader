Shader "Hidden/PosterizeNode"
{
	Properties
	{
		_A ( "_RGBA",  2D ) = "white" {}
		_B ( "_Power",  2D ) = "white" {}
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
				float4 rgba = tex2D ( _B, i.uv );
				float power = tex2D ( _A, i.uv ).r;
				if ( power < 1 )
					return float4(0,0,0,0);
				float divideOp = 256.0 / float ( (int)power );
				float4 finalColor = ( floor ( rgba * divideOp ) / divideOp );

				return finalColor;
			}
			ENDCG
		}
	}
}
