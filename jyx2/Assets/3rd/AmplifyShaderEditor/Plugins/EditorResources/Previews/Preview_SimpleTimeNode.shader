Shader "Hidden/SimpleTimeNode"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
		_Count ("_Count", Int) = 0
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
			float _EditorTime;

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 a = tex2D( _A, i.uv );
				float4 t = _EditorTime;
				return t * a.x;
			}
			ENDCG
		}
	}
}
