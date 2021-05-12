Shader "Hidden/TextureCoordinatesNode"
{
	Properties
	{
		_A ("_Tilling", 2D) = "white" {}
		_B ("_Offset", 2D) = "white" {}
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

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 t = tex2D( _A, i.uv );
				float4 o = tex2D( _B, i.uv );
				return float4( i.uv * t.xy + o.xy, 0, 0 );
			}
			ENDCG
		}
	}
}
