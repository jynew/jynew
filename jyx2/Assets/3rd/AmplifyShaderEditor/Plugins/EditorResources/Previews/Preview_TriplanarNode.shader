Shader "Hidden/TriplanarNode"
{
	Properties
	{
		_A ("_TopTex", 2D) = "white" {}
		_B ("_MidTex", 2D) = "white" {}
		_C ("_BotTex", 2D) = "white" {}
		_D ("_Tilling", 2D) = "white" {}
		_E ("_Falloff", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"

			sampler2D _A;		
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;
			sampler2D _E;
			float _IsNormal;
			float _IsSpherical;

			inline float3 TriplanarSamplingCNF( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling )
			{
				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
				projNormal /= projNormal.x + projNormal.y + projNormal.z;
				float3 nsign = sign( worldNormal );
				float negProjNormalY = max( 0, projNormal.y * -nsign.y );
				projNormal.y = max( 0, projNormal.y * nsign.y );
				half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
				xNorm = ( tex2D( midTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
				yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
				yNormN = ( tex2D( botTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
				zNorm = ( tex2D( midTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
				xNorm.xyz = half3( UnpackNormal( xNorm ).xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
				yNorm.xyz = half3( UnpackNormal( yNorm ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
				zNorm.xyz = half3( UnpackNormal( zNorm ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
				yNormN.xyz = half3( UnpackNormal( yNormN ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
				return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + yNormN.xyz * negProjNormalY + zNorm.xyz * projNormal.z );
			}

			inline float4 TriplanarSamplingCF( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling )
			{
				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
				projNormal /= projNormal.x + projNormal.y + projNormal.z;
				float3 nsign = sign( worldNormal );
				float negProjNormalY = max( 0, projNormal.y * -nsign.y );
				projNormal.y = max( 0, projNormal.y * nsign.y );
				half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
				xNorm = ( tex2D( midTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
				yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
				yNormN = ( tex2D( botTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
				zNorm = ( tex2D( midTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
				return xNorm * projNormal.x + yNorm * projNormal.y + yNormN * negProjNormalY + zNorm * projNormal.z;
			}

			float4 frag( v2f_img i ) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
				float3 normal = normalize(vertexPos);
				float3 worldNormal = UnityObjectToWorldNormal(normal);
				float3 worldPos = mul(unity_ObjectToWorld, vertexPos).xyz;

				float falloff = tex2D( _E, xy ).r;
				float tilling = tex2D( _D, xy ).r * 0.625;
				float4 triplanar = 1;

				if ( _IsNormal == 1 ) {
					float3 tangent = normalize(float3( -z, xy.y*0.01, xy.x ));
					float3 worldTangent = UnityObjectToWorldDir(tangent);
					float tangentSign = -1;
					float3 worldBinormal = normalize( cross(worldNormal, worldTangent) * tangentSign);
					float3x3 worldToTangent = float3x3( worldTangent, worldBinormal, worldNormal );
					if ( _IsSpherical == 1 )
						triplanar.xyz = TriplanarSamplingCNF( _A, _A, _A, worldPos, worldNormal, falloff, tilling );
					else
						triplanar.xyz = TriplanarSamplingCNF( _A, _B, _C, worldPos, worldNormal, falloff, tilling );

					triplanar.xyz = mul( worldToTangent, triplanar.xyz );
				}
				else 
				{
					if ( _IsSpherical == 1 )
						triplanar = TriplanarSamplingCF( _A, _A, _A, worldPos, worldNormal, falloff, tilling );
					else
						triplanar = TriplanarSamplingCF( _A, _B, _C, worldPos, worldNormal, falloff, tilling );
				}

				return triplanar;
			}
			ENDCG
		}
	}
}
