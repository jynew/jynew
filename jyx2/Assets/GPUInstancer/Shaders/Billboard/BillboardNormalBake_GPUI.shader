Shader "Hidden/GPUInstancer/Billboard/NormalBake"
{	
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_Cutoff("Cutoff" , Range(0,1)) = 0.3
		_GPUIBillboardCutoffOverride("Cutoff Override", Range(0, 1)) = 0.0
		_IsLinearSpace("Add Gama Correction", Float) = 0.0
		
	}
	
	SubShader
	{
		Cull Off

		Pass
		{
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"
			#include "../Include/GPUIBillboardInclude.cginc"
			
			sampler2D _BumpMap;
			sampler2D _MainTex;
			float4 _BumpMap_ST;
			float _Cutoff;
			float _GPUIBillboardCutoffOverride;
			float _IsLinearSpace;
			
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float  depth : TEXCOORD4;
				//float3 normal : NORMAL;
				float4 tangentToWorldX : TEXCOORD1;
				float4 tangentToWorldY : TEXCOORD2;
				float4 tangentToWorldZ : TEXCOORD3;
			};

			v2f vert (float4 uv : TEXCOORD0, float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT)
            {
				

				v2f o;
				o.vertex = UnityObjectToClipPos(vertex);
				o.uv = uv;
				
				float3 worldPos = mul(unity_ObjectToWorld,vertex);
                float3 worldNormal = UnityObjectToWorldNormal(normal);
                float3 worldTangent = UnityObjectToWorldDir(tangent.xyz);
                float3 worldBinormal = cross(worldNormal, worldTangent) * tangent.w;
   
                o.tangentToWorldX = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.tangentToWorldY = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.tangentToWorldZ = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
				
				o.depth = -(UnityObjectToViewPos( vertex ).z * _ProjectionParams.w);
				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
				if (_GPUIBillboardCutoffOverride > 0.0){
					_Cutoff = _GPUIBillboardCutoffOverride;
				}

				float3 textureNormal = UnpackNormal(tex2D(_BumpMap, TRANSFORM_TEX(i.uv, _BumpMap)));
				float4 col = tex2D(_MainTex, i.uv);
				clip (col.a - _Cutoff);

				
				float3 worldPos = float3(i.tangentToWorldX.w, i.tangentToWorldY.w, i.tangentToWorldZ.w);
				float3 worldNormal = normalize(float3(dot(i.tangentToWorldX.xyz, textureNormal), dot(i.tangentToWorldY.xyz, textureNormal), dot(i.tangentToWorldZ.xyz, textureNormal)));
				
				#if UNITY_REVERSED_Z
					i.depth = 1-i.depth;
				#endif
				
				// remap normals to [0, 1] (a is depth atlas) and account fot gamma correction if necessary
				return float4((worldNormal.rgb * 0.5 + 0.5), i.depth) * (1 - _IsLinearSpace) + (_IsLinearSpace * half4(LinearToGamma(worldNormal.rgb * 0.5 + 0.5), LinearToGammaExact(i.depth)));
				

            }
            ENDCG
		}
	}
}