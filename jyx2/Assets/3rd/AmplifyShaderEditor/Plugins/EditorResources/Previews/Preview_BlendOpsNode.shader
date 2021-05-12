Shader "Hidden/BlendOpsNode"
{
	Properties
	{
		_A ("_Source", 2D) = "white" {}
		_B ("_Destiny", 2D) = "white" {}
	}
	SubShader
	{
		Pass //colorburn
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( 1.0 - ( ( 1.0 - des) / src) ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //colordodge
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( des/ ( 1.0 - src ) ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //darken
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( min( src , des ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //divide
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( des / src ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //difference
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( abs( src - des ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //exclusion
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( 0.5 - 2.0 * ( src - 0.5 ) * ( des - 0.5 ) ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //softlight
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( 2.0f*src*des + src*src*(1.0f - 2.0f*des) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //hardlight
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = (  ( src > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( src - 0.5 ) ) * ( 1.0 - des ) ) : ( 2.0 * src * des ) ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //hardmix
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( round( 0.5 * ( src + des ) ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //lighten
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( max( src, des ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //linearburn
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( src + des - 1.0 ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //lineardodge
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( src + des ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //linearlight
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( src > 0.5 ? ( des + 2.0 * src - 1.0 ) : ( des + 2.0 * ( src - 0.5 ) ) ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //multiply
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( src * des ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //overlay
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( des > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( des - 0.5 ) ) * ( 1.0 - src ) ) : ( 2.0 * des * src ) ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //pinlight
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( src > 0.5 ? max( des, 2.0 * ( src - 0.5 ) ) : min( des, 2.0 * src ) ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //subtract
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( des - src ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //screen
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( 1.0 - ( 1.0 - src ) * ( 1.0 - des ) ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}

		Pass //vividlight
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Sat;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 src = tex2D( _A, i.uv );
				float4 des = tex2D( _B, i.uv );

				float4 c = ( ( src > 0.5 ? ( des / ( ( 1.0 - src ) * 2.0 ) ) : ( 1.0 - ( ( ( 1.0 - des ) * 0.5 ) / src ) ) ) );
				if( _Sat == 1 )
					c = saturate( c );
				return c;
			}
			ENDCG
		}
	}
}
