Shader "Hidden/DeltaTime"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _EditorDeltaTime;

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 t = _EditorDeltaTime;
				t.y = 1 / _EditorDeltaTime;
				t.z = _EditorDeltaTime;
				t.w = 1 / _EditorDeltaTime;
				return cos(t);
			}
			ENDCG
		}

	}
}
