Shader "Hidden/WorldNormalVector"
{
	Properties
	{
		_A ("_WorldNormal", 2D) = "white" {}
	}
	SubShader
	{
		Pass //not connected
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 worldNormal = normalize(float3(xy, z));
				return float4(worldNormal, 1);
			}
			ENDCG
		}

		Pass //connected
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;

			float4 frag(v2f_img i) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
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

				float2 sphereUVs = i.uv;

				sphereUVs.x = (atan2(vertexPos.x, -vertexPos.z) / (UNITY_PI) + 0.5);
				// Needs further checking
				//float3 tangentNormal = tex2Dlod(_A, float4(sphereUVs,0,0)).xyz;
				float3 tangentNormal = tex2D(_A, sphereUVs).xyz;

				worldNormal = fixed3( dot( tSpace0.xyz, tangentNormal ), dot( tSpace1.xyz, tangentNormal ), dot( tSpace2.xyz, tangentNormal ) );

				return float4(worldNormal, 1);
			}
			ENDCG
		}
	}
}
