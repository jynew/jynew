// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

Shader "Hidden/SWS/NormalMapRenderer"
{
	Properties
	{
		_MainTex("Texture", 2D) = "black" {}
		_Offset("Offset", Float) = 0.005
		_Strength("Strength", Float) = 6

	}
		SubShader
		{

			Pass
			{
				CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float _Offset;
				float _Strength;

				float4 NormalFromHeight(sampler2D heightmap, float2 uv) {
					float xLeft = tex2D(heightmap, uv - float2(_Offset, 0.0)) * _Strength;
					float xRight = tex2D(heightmap, uv + float2(_Offset, 0.0)) * _Strength;

					float yUp = tex2D(heightmap, uv - float2(0.0, _Offset)) * _Strength;
					float yDown = tex2D(heightmap, uv + float2(0.0, _Offset)) * _Strength;

					float xDelta = ((xLeft - xRight) + 1.0) * 0.5f;
					float yDelta = ((yUp - yDown) + 1.0) * 0.5f;

#if UNITY_VERSION >= 201810
	#if !UNITY_COLORSPACE_GAMMA
					xLeft = GammaToLinearSpace(xLeft);
					xRight = GammaToLinearSpace(xRight);
					yUp = GammaToLinearSpace(yUp);
					yDown = GammaToLinearSpace(yDown);
					xDelta = GammaToLinearSpace(xDelta);
					yDelta = GammaToLinearSpace(yDelta);
	#endif
#endif
					float4 normals = float4(xDelta, yDelta, 1.0, yDelta);

					return normals;
				}

				fixed4 frag(v2f_img  IN) : SV_Target
				{
					return NormalFromHeight(_MainTex, IN.uv);
				}
				ENDCG
			}
		}
}
