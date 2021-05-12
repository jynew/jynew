Shader "Hidden/GrayscaleNode"
{
	Properties
	{
		_A ("_RGB", 2D) = "white" {}
	}
	SubShader
	{
		Pass //Luminance
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			
			float4 frag(v2f_img i) : SV_Target
			{
				float lum = Luminance( tex2D( _A, i.uv ) );
				return float4( lum.xxx, 1);
			}
			ENDCG
		}

		Pass //Natural Classic
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;

			float4 frag ( v2f_img i ) : SV_Target
			{
				float lum = dot ( tex2D ( _A, i.uv ), float3( 0.299,0.587,0.114 ) ); 
				return float4( lum.xxx, 1 );
			}
			ENDCG
		}

		Pass //Old School
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;

			float4 frag ( v2f_img i ) : SV_Target
			{
				float3 rgbValue = tex2D ( _A, i.uv ).rgb;
				float lum = ( rgbValue.r + rgbValue.g + rgbValue.b ) / 3;
				return float4( lum.xxx, 1 );
			}
			ENDCG
		}

	}
}
