// Decal Shader: Unlit
// Three Eyed Games

// Based on: Kim, Pope: Screen Space Decals in Warhammer 40,000: Space Marine. SIGGRAPH 2012.
// http://www.popekim.com/2012/10/siggraph-2012-screen-space-decals-in.html

Shader "Decalicious/Unlit Decal"
{
	Properties
	{
		_MaskTex("Mask", 2D) = "white" {}
		[PerRendererData] _MaskMultiplier("Mask (Multiplier)", Float) = 1.0

		_MainTex("Albedo", 2D) = "white" {}
		[HDR] _Color("Albedo (Multiplier)", Color) = (1,1,1,1)

		_DecalBlendMode("Blend Mode", Float) = 0
		_DecalSrcBlend("SrcBlend", Float) = 1.0 // One
		_DecalDstBlend("DstBlend", Float) = 10.0 // OneMinusSrcAlpha

		_AngleLimit("Angle Limit", Float) = 0.5
	}

	// Use custom GUI for the decal shader
	CustomEditor "ThreeEyedGames.DecalShaderGUI"

	SubShader
	{
		// Draw backfaces only with inverted ZTest to avoid clipping problems
		Cull Front
		ZTest GEqual
		ZWrite Off
	
		// Pass 0: Unlit decal
		Pass
		{
			// Blend mode set from script
			Blend [_DecalSrcBlend] [_DecalDstBlend]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "DecaliciousCommon.cginc"

			float4 frag(v2f i) : SV_Target0
			{
				// Common header for all fragment shaders
				DEFERRED_FRAG_HEADER

				// Get normal from GBuffer
				float3 gbuffer_normal = tex2D(_CameraGBufferTexture2, uv) * 2.0f - 1.0f;
				clip(dot(gbuffer_normal, i.decalNormal) - _AngleLimit); // 60 degree clamp

				// Get color from texture and property
				float4 color = tex2D(_MainTex, texUV) * _Color;
				color.a *= mask;

				// Write albedo, premultiply for proper blending
				return float4(color.rgb * color.a, color.a);
			}
			ENDCG
		}
	}

	Fallback Off
}
