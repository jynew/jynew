Shader "Hidden/SwitchNode"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
		_B ("_B", 2D) = "white" {}
		_C ("_C", 2D) = "white" {}
		_D ("_D", 2D) = "white" {}
		_E ("_E", 2D) = "white" {}
		_F ("_F", 2D) = "white" {}
		_G ("_G", 2D) = "white" {}
		_H ("_H", 2D) = "white" {}
		_I ("_I", 2D) = "white" {}
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
			sampler2D _E;
			sampler2D _F;
			sampler2D _G;
			sampler2D _H;
			sampler2D _I;
			float _Current;

			float4 frag( v2f_img i ) : SV_Target
			{
				if( _Current == 0 )
					return  tex2D( _A, i.uv );
				else if( _Current == 1 )
					return  tex2D( _B, i.uv );
				else if( _Current == 2 )
					return  tex2D( _C, i.uv );
				else if( _Current == 3 )
					return  tex2D( _D, i.uv );
				else if( _Current == 4 )
					return  tex2D( _E, i.uv );
				else if( _Current == 5 )
					return  tex2D( _F, i.uv );
				else if( _Current == 6 )
					return  tex2D( _G, i.uv );
				else if( _Current == 7 )
					return  tex2D( _H, i.uv );
				else
					return  tex2D( _I, i.uv );
			}
			ENDCG
		}
	}
}
