Shader "Hidden/ComponentMaskNode"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
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
			float _Singular;
			float4 _Order;

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 r = 0;
				if(_Singular == 0)
					r = a.x;
				else if(_Singular == 1)
					r = a.y;
				else if(_Singular == 2)
					r = a.z;
				else if(_Singular == 3)
					r = a.w;

				if ( _Order.x == 0 )
					r.x = a.x;
				else if(_Order.y == 0)
					r.x = a.y;
				else if(_Order.z == 0)
					r.x = a.z;
				else if(_Order.w == 0)
					r.x = a.w;

				if(_Order.y == 1)
					r.y = a.y;
				else if(_Order.z == 1)
					r.y = a.z;
				else if(_Order.w == 1)
					r.y = a.w;

				if(_Order.z == 2)
					r.z = a.z;
				else if(_Order.w == 2)
					r.z = a.w;

				if(_Order.w == 3)
					r.w = a.w;

				return r;
			}
			ENDCG
		}
	}
}
