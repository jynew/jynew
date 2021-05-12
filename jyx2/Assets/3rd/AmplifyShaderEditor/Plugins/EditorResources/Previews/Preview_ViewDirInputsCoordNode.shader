Shader "Hidden/WorldPosInputsNode"
{
	SubShader
	{
		Pass //world space
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);

				return float4(worldViewDir, 1);
			}
			ENDCG
		}

		Pass //tangent space
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);

				float3 normal = normalize(vertexPos);
				float3 worldNormal = UnityObjectToWorldNormal(normal);

				float3 tangent = normalize(float3( -z, xy.y*0.01, xy.x ));
				float3 worldPos = mul(unity_ObjectToWorld, float4(vertexPos,1)).xyz;
				float3 worldTangent = UnityObjectToWorldDir(tangent);
				float tangentSign = -1;
				float3 worldBinormal = normalize( cross(worldNormal, worldTangent) * tangentSign);
				float4 tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				float4 tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				float4 tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				fixed3 viewDirTan = tSpace0.xyz * worldViewDir.x + tSpace1.xyz * worldViewDir.y + tSpace2.xyz * worldViewDir.z;

				return float4(viewDirTan, 1);
			}
			ENDCG
		}
	}
}
