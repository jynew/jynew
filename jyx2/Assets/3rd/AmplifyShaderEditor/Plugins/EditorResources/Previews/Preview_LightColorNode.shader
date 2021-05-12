Shader "Hidden/LightColorNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			float4 _EditorLightColor;

			float4 frag(v2f_img i) : SV_Target
			{
				return _EditorLightColor;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			float4 _EditorLightColor;

			float4 frag(v2f_img i) : SV_Target
			{
				return float4(_EditorLightColor.rgb, 0);
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			float4 _EditorLightColor;

			float4 frag(v2f_img i) : SV_Target
			{
				return _EditorLightColor.a;
			}
			ENDCG
		}
	}
}
