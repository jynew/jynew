Shader "Hidden/TimeNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _EditorTime;

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 t = _EditorTime;
				t.x = _EditorTime / 20;
				t.z = _EditorTime * 2;
				t.w = _EditorTime * 3;
				return t;
			}
			ENDCG
		}
	}
}
