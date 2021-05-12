Shader "Hidden/SimpleAddOpNode"
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
		_J ("_J", 2D) = "white" {}
		_Count ("_Count", Int) = 0
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
			sampler2D _J;
			int _Count;

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 b = tex2D( _B, i.uv );
				float4 final = a + b;

				if( _Count > 2 )
					final += tex2D( _C, i.uv );
				if( _Count > 3 )
					final += tex2D( _D, i.uv );
				if( _Count > 4 )
					final += tex2D( _E, i.uv );
				if( _Count > 5 )
					final += tex2D( _F, i.uv );
				if( _Count > 6 )
					final += tex2D( _G, i.uv );
				if( _Count > 7 )
					final += tex2D( _H, i.uv );
				if( _Count > 8 )
					final += tex2D( _I, i.uv );
				if( _Count > 9 )
					final += tex2D( _J, i.uv );
				
				return final;
			}
			ENDCG
		}
	}
}
