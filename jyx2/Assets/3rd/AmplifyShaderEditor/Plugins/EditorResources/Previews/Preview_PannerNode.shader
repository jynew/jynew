Shader "Hidden/PannerNode"
{
	Properties
	{
		_A ("_UVs", 2D) = "white" {}
		_B ("_PanTime", 2D) = "white" {}
		_C ("_PanSpeed", 2D ) = "white" {}
	}
	SubShader
	{
		Pass
		{
			Name "Panner" // 14 - UV panner node
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
				float multiplier = tex2D ( _B, i.uv ).r;
				float time = _EditorTime*multiplier;
				if ( _UsingEditor == 0 ) 
				{
					time = multiplier;
				}
				float2 speed = tex2D ( _C, i.uv ).rg;
				return tex2D( _A, i.uv) + time * float4( speed, 0, 0 );
			}
			ENDCG
		}
	}
}
