Shader "Hidden/TFHCRemap"
{
	Properties
	{
		_A ("_Value", 2D) = "white" {}
		_B ("_MinOld", 2D) = "white" {}
		_C ("_MaxOld", 2D) = "white" {}
		_D ("_MinNew", 2D) = "white" {}
		_E ("_MaxNew", 2D) = "white" {}
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

			float4 frag(v2f_img i) : SV_Target
			{
				float4 value = tex2D( _A, i.uv );
				float4 minold = tex2D( _B, i.uv );
				float4 maxold = tex2D( _C, i.uv );
				float4 minnew = tex2D( _D, i.uv );
				float4 maxnew = tex2D( _E, i.uv );

				float4 denom = maxold - minold;
				if(denom.x == 0)
					denom.x = 0.000001;
				if(denom.y == 0)
					denom.y = 0.000001;
				if(denom.z == 0)
					denom.z = 0.000001;
				if(denom.w == 0)
					denom.w = 0.000001;

				return (minnew + (value - minold) * (maxnew - minnew) / denom);
			}
			ENDCG
		}
	}
}
