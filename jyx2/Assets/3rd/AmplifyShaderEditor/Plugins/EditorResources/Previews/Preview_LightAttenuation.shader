Shader "Hidden/LightAttenuation"
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

			float4 _EditorWorldLightPos;

			float4 frag(v2f_img i) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 worldNormal = normalize(float3(xy, z));
				float3 lightDir = normalize( _EditorWorldLightPos.xyz );
				return saturate(dot(worldNormal ,lightDir) * 10 + 0.1);
			}
			ENDCG
		}
	}
}
