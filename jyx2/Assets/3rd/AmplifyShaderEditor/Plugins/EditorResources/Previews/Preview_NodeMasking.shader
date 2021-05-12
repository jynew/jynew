Shader "Hidden/NodeMasking"
{
	Properties {
		_Ports ("_Ports", Vector) = (0,0,0,0)
		_MainTex("_MainTex", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _Ports;

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _MainTex, i.uv );
				return a * _Ports;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _MaskTex;
			float _Port;

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _MaskTex, i.uv );
				float4 c = 0;
				if ( _Port == 1 )
					c = a.x;
				else if ( _Port == 2 )
					c = a.y;
				else if ( _Port == 3 )
					c = a.z;
				else if ( _Port == 4 )
					c = a.w;

				return c;
			}
			ENDCG
		}
	}
}
