Shader "Hidden/RotatorNode"
{
	Properties
	{
		_A ("_UVs", 2D) = "white" {}
		_B ("_Anchor", 2D) = "white" {}
		_C ("_RotTimeTex", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			float _UsingEditor;
			float _EditorTime;

			float4 frag(v2f_img i) : SV_Target
			{
				float multiplier = tex2D ( _C, i.uv ).r;
				float time = _EditorTime*multiplier;
				
				if ( _UsingEditor == 0 ) 
				{
					time = multiplier;
				}

				float cosT = cos( time );
				float sinT = sin( time );

				float2 a = tex2D( _B, i.uv ).rg;
				return float4( mul( tex2D( _A, i.uv ).xy - a, float2x2( cosT, -sinT, sinT, cosT ) ) + a, 0, 1 );
			}
			ENDCG
		}
	}
}
