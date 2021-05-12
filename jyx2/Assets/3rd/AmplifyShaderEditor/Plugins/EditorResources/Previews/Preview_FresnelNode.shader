Shader "Hidden/FresnelNode"
{
	Properties
	{
		_A ("_Normal", 2D) = "white" {}
		_B ("_Bias", 2D) = "white" {}
		_C ("_Scale", 2D) = "white" {}
		_D ("_Power", 2D) = "white" {}
		_E ("_View", 2D) = "white" {}
	}
	SubShader
	{
		Pass //not connected world
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			//sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;
			int _FresnelType;

			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
				float3 worldNormal = normalize(float3(xy, z));
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);

				float fresnel = 0;
				if(_FresnelType == 0)
					fresnel = (b + s*pow(s - dot( worldNormal, worldViewDir ) , pw));
				else if(_FresnelType == 1)
					fresnel = (b + (1-b) * pow(1 - dot( worldNormal, worldViewDir ) , 5));
				else if(_FresnelType == 2)
				{
					float f0 = pow((1-s)/(1+s),2);
					fresnel = (f0 + (1-f0) * pow(1 - dot( worldNormal, worldViewDir ) , 5));
				}
				return fresnel;
			}
			ENDCG
		}

		Pass //connected world
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;
			int _FresnelType;

			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
				float3 worldNormal = tex2D( _A, i.uv );
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);

				float fresnel = 0;
				if(_FresnelType == 0)
					fresnel = (b + s*pow(s - dot( worldNormal, worldViewDir ) , pw));
				else if(_FresnelType == 1)
					fresnel = (b + (1-b) * pow(1 - dot( worldNormal, worldViewDir ) , 5));
				else if(_FresnelType == 2)
				{
					float f0 = pow((1-s)/(1+s),2);
					fresnel = (f0 + (1-f0) * pow(1 - dot( worldNormal, worldViewDir ) , 5));
				}
				return fresnel;
			}
			ENDCG
		}

		Pass //connected tangent
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;
			int _FresnelType;

			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
				float3 worldNormal = normalize(float3(xy, z));

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
				float3 tangentNormal = tex2D(_A, sphereUVs).xyz;

				worldNormal = fixed3( dot( tSpace0.xyz, tangentNormal ), dot( tSpace1.xyz, tangentNormal ), dot( tSpace2.xyz, tangentNormal ) );

				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);

				float fresnel = 0;
				if(_FresnelType == 0)
					fresnel = (b + s*pow(s - dot( worldNormal, worldViewDir ) , pw));
				else if(_FresnelType == 1)
					fresnel = (b + (1-b) * pow(1 - dot( worldNormal, worldViewDir ) , 5));
				else if(_FresnelType == 2)
				{
					float f0 = pow((1-s)/(1+s),2);
					fresnel = (f0 + (1-f0) * pow(1 - dot( worldNormal, worldViewDir ) , 5));
				}
				return fresnel;
			}
			ENDCG
		}

		Pass //not connected half vector
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			//sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;
			int _FresnelType;
			float4 _EditorWorldLightPos;
			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-(dot(xy,xy)));
				float3 vertexPos = normalize(float3(xy, z));
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);
				float3 lightDir = normalize( _EditorWorldLightPos.xyz );
				float3 halfVector = normalize(worldViewDir+lightDir);

				float fresnel = 0;
				if(_FresnelType == 0)
					fresnel = (b + s*pow(s - dot( halfVector, worldViewDir ) , pw));
				else if(_FresnelType == 1)
					fresnel = (b + (1-b) * pow(1 - dot( halfVector, worldViewDir ) , 5));
				else if(_FresnelType == 2)
				{
					float f0 = pow((1-s)/(1+s),2);
					fresnel = (f0 + (1-f0) * pow(1 - dot( halfVector, worldViewDir ) , 5));
				}
				return fresnel;
			}
			ENDCG
		}

		Pass //connected both
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
			int _FresnelType;

			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
				float3 worldNormal = tex2D( _A, i.uv );
				float3 worldViewDir = tex2D( _E, i.uv );;
				
				float fresnel = 0;
				if(_FresnelType == 0)
					fresnel = (b + s*pow(s - dot( worldNormal, worldViewDir ) , pw));
				else if(_FresnelType == 1)
					fresnel = (b + (1-b) * pow(1 - dot( worldNormal, worldViewDir ) , 5));
				else if(_FresnelType == 2)
				{
					float f0 = pow((1-s)/(1+s),2);
					fresnel = (f0 + (1-f0) * pow(1 - dot( worldNormal, worldViewDir ) , 5));
				}
				return fresnel;
			}
			ENDCG
		}

		Pass //not connected world and light
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			//sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;
			int _FresnelType;
			float4 _EditorWorldLightPos;

			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-(dot(xy,xy)));
				float3 vertexPos = normalize(float3(xy, z));
				float3 normal = normalize(vertexPos);
				float3 worldNormal = UnityObjectToWorldNormal(normal);
				float3 lightDir = normalize( _EditorWorldLightPos.xyz );

				float fresnel = 0;
				if(_FresnelType == 0)
					fresnel = (b + s*pow(s - dot( worldNormal, lightDir ) , pw));
				else if(_FresnelType == 1)
					fresnel = (b + (1-b) * pow(1 - dot( worldNormal, lightDir ) , 5));
				else if(_FresnelType == 2)
				{
					float f0 = pow((1-s)/(1+s),2);
					fresnel = (f0 + (1-f0) * pow(1 - dot( worldNormal, lightDir ) , 5));
				}
				return fresnel;
			}
			ENDCG
		}

		Pass //connected view
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
			int _FresnelType;

			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
				float3 normal = normalize(vertexPos);
				float3 worldNormal = UnityObjectToWorldNormal(normal);
				float3 worldViewDir = tex2D( _E, i.uv );
				
				float fresnel = 0;
				if(_FresnelType == 0)
					fresnel = (b + s*pow(s - dot( worldNormal, worldViewDir ) , pw));
				else if(_FresnelType == 1)
					fresnel = (b + (1-b) * pow(1 - dot( worldNormal, worldViewDir ) , 5));
				else if(_FresnelType == 2)
				{
					float f0 = pow((1-s)/(1+s),2);
					fresnel = (f0 + (1-f0) * pow(1 - dot( worldNormal, worldViewDir ) , 5));
				}
				return fresnel;
			}
			ENDCG
		}

		Pass //not connected half vector with connected view
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			//sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;
			sampler2D _E;
			int _FresnelType;
			float4 _EditorWorldLightPos;
			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-(dot(xy,xy)));
				float3 vertexPos = normalize(float3(xy, z));
				float3 worldViewDir = tex2D( _E, i.uv );
				float3 lightDir = normalize( _EditorWorldLightPos.xyz );
				float3 halfVector = normalize(worldViewDir+lightDir);

				float fresnel = 0;
				if(_FresnelType == 0)
					fresnel = (b + s*pow(s - dot( halfVector, worldViewDir ) , pw));
				else if(_FresnelType == 1)
					fresnel = (b + (1-b) * pow(1 - dot( halfVector, worldViewDir ) , 5));
				else if(_FresnelType == 2)
				{
					float f0 = pow((1-s)/(1+s),2);
					fresnel = (f0 + (1-f0) * pow(1 - dot( halfVector, worldViewDir ) , 5));
				}
				return fresnel;
			}
			ENDCG
		}
	}
}
