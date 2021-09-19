Shader "Hidden/GPUInstancer/HiZOcclusionGenerator" 
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "black" {}
	}
    SubShader
    {
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE
		#include "UnityCG.cginc"

		struct Input
		{
			float4 pos : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct Varyings
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		Texture2D _MainTex;
		SamplerState sampler_MainTex;

		float4 _MainTex_TexelSize;

		Texture2D _CameraDepthTexture;
		SamplerState sampler_CameraDepthTexture;

		Varyings vertex(in Input input)
		{
			Varyings output;

			output.pos = UnityObjectToClipPos(input.pos.xyz);
			output.uv = input.uv;

			return output;
		}

		float4 sampleDepth(in Varyings input) : SV_Target
		{
				#ifdef SINGLEPASS_VR_ENABLED
					#ifndef HIZ_TEXTURE_FOR_BOTH_EYES
						input.uv.x *= 0.5;
					#endif
				#endif

				#ifdef MULTIPASS_VR_ENABLED
					#ifdef HIZ_TEXTURE_FOR_BOTH_EYES
						input.uv.x *= 2;
					#else
						clip(1 - unity_StereoEyeIndex);
					#endif
				
					/*
					if (unity_StereoEyeIndex == 0)
					{
						clip (1 - input.uv.x * 0.5 - 0.5);
					}
					else
					{
						clip(input.uv.x * 0.5 - 0.5);
						input.uv.x -= 1;
					}
					*/
				
					clip ( (1 - unity_StereoEyeIndex) + ( input.uv.x * sign(unity_StereoEyeIndex - 0.1) * 0.5 - 0.5) );
					input.uv.x -= unity_StereoEyeIndex;
				#endif

#if UNITY_REVERSED_Z
			return _CameraDepthTexture.Sample(sampler_CameraDepthTexture, input.uv).r;
#else
			return 1.0 - _CameraDepthTexture.Sample(sampler_CameraDepthTexture, input.uv).r;
#endif
		}

		float4 reduce(in Varyings input) : SV_Target
		{
			float4 r;
			r.x = _MainTex.SampleLevel(sampler_MainTex, float2(input.uv.x + _MainTex_TexelSize.x, input.uv.y), 0).r;
			r.y = _MainTex.SampleLevel(sampler_MainTex, float2(input.uv.x - _MainTex_TexelSize.x, input.uv.y), 0).r;
			r.z = _MainTex.SampleLevel(sampler_MainTex, float2(input.uv.x, input.uv.y + _MainTex_TexelSize.y), 0).r;
			r.w = _MainTex.SampleLevel(sampler_MainTex, float2(input.uv.x, input.uv.y - _MainTex_TexelSize.y), 0).r;

			float minimum = min(min(min(r.x, r.y), r.z), r.w);
			return float4(minimum, 1.0, 1.0, 1.0);
		}
		ENDCG

        Pass
        {
			CGPROGRAM
			#pragma target 4.5
            #pragma vertex vertex
            #pragma fragment sampleDepth
			#pragma multi_compile __ MULTIPASS_VR_ENABLED SINGLEPASS_VR_ENABLED
			#pragma multi_compile __ HIZ_TEXTURE_FOR_BOTH_EYES
			ENDCG
        }

		Pass
		{
			CGPROGRAM
			#pragma target 4.5
			#pragma vertex vertex
			#pragma fragment reduce
			ENDCG
		}
    }
}
