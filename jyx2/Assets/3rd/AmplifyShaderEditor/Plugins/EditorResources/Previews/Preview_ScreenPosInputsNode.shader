Shader "Hidden/ScreenPosInputsNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			inline float4 PrevComputeNonStereoScreenPos(float4 pos) {
				float4 o = pos * 0.5f;
				o.xy = float2(o.x, o.y*_ProjectionParams.x) + o.w;
				o.zw = pos.zw;
				return o;
			}

			float4 frag( v2f_img i ) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
				float4x4 P = float4x4(1,0,0,0,0,-1,0,0,0,0,1,0,0,0,0,1); //UNITY_MATRIX_P
				float4x4 V = UNITY_MATRIX_V;//float4x4(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1); //UNITY_MATRIX_V
				float4x4 M = unity_ObjectToWorld;//float4x4(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1); //unity_ObjectToWorld
				float4x4 VPmatrix = mul(P, V);
				float4 clipPos = mul(VPmatrix, mul(M, float4(vertexPos, 1.0))); //same as object to clip pos
				float4 screenPos = ComputeScreenPos(clipPos);
				return screenPos;
			}
			ENDCG
		}
	}
}
