Shader "Hidden/SimpleMultiplyOpNode"
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
		Pass //2
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 b = tex2D( _B, i.uv );

				return a * b;
			}
			ENDCG
		}

		Pass //3
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			sampler2D _C;

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 b = tex2D( _B, i.uv );
				float4 c = tex2D( _C, i.uv );

				return a * b * c;
			}
			ENDCG
		}

		Pass //4
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 b = tex2D( _B, i.uv );
				float4 c = tex2D( _C, i.uv );
				float4 d = tex2D( _D, i.uv );

				return a * b * c * d;
			}
			ENDCG
		}

		Pass //5
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

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 b = tex2D( _B, i.uv );
				float4 c = tex2D( _C, i.uv );
				float4 d = tex2D( _D, i.uv );
				float4 e = tex2D( _E, i.uv );

				return a * b * c * d * e;
			}
			ENDCG
		}

		Pass //6
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

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 b = tex2D( _B, i.uv );
				float4 c = tex2D( _C, i.uv );
				float4 d = tex2D( _D, i.uv );
				float4 e = tex2D( _E, i.uv );
				float4 f = tex2D( _F, i.uv );

				return a * b * c * d * e * f;
			}
			ENDCG
		}

		Pass //7
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

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 b = tex2D( _B, i.uv );
				float4 c = tex2D( _C, i.uv );
				float4 d = tex2D( _D, i.uv );
				float4 e = tex2D( _E, i.uv );
				float4 f = tex2D( _F, i.uv );
				float4 g = tex2D( _G, i.uv );

				return a * b * c * d * e * f * g;
			}
			ENDCG
		}

		Pass //8
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

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 b = tex2D( _B, i.uv );
				float4 c = tex2D( _C, i.uv );
				float4 d = tex2D( _D, i.uv );
				float4 e = tex2D( _E, i.uv );
				float4 f = tex2D( _F, i.uv );
				float4 g = tex2D( _G, i.uv );
				float4 h = tex2D( _H, i.uv );

				return a * b * c * d * e * f * g * h;
			}
			ENDCG
		}

		Pass //9
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

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 b = tex2D( _B, i.uv );
				float4 c = tex2D( _C, i.uv );
				float4 d = tex2D( _D, i.uv );
				float4 e = tex2D( _E, i.uv );
				float4 f = tex2D( _F, i.uv );
				float4 g = tex2D( _G, i.uv );
				float4 h = tex2D( _H, i.uv );
				float4 is = tex2D( _I, i.uv );

				return a * b * c * d * e * f * g * h * is;
			}
			ENDCG
		}

		Pass //10
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

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 b = tex2D( _B, i.uv );
				float4 c = tex2D( _C, i.uv );
				float4 d = tex2D( _D, i.uv );
				float4 e = tex2D( _E, i.uv );
				float4 f = tex2D( _F, i.uv );
				float4 g = tex2D( _G, i.uv );
				float4 h = tex2D( _H, i.uv );
				float4 is = tex2D( _I, i.uv );
				float4 j = tex2D( _J, i.uv );

				return a * b * c * d * e * f * g * h * is * j;
			}
			ENDCG
		}
	}
}
