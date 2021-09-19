#ifndef GPU_INSTANCER_INCLUDED
#define GPU_INSTANCER_INCLUDED

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

#include "GPUIPlatformDependent.cginc"

#if GPUI_MHT_COPY_TEXTURE
    uniform sampler2D gpuiTransformationMatrixTexture;
    uniform float4x4 gpuiTransformOffset;
    uniform float bufferSize;
    uniform float maxTextureSize;
#elif GPUI_MHT_MATRIX_APPEND
    uniform StructuredBuffer<float4x4> gpuiTransformationMatrix;
    uniform float4x4 gpuiTransformOffset;
#else
    uniform StructuredBuffer<uint> gpuiTransformationMatrix;
    uniform StructuredBuffer<float4x4> gpuiInstanceData;
    uniform StructuredBuffer<uint4> gpuiInstanceLODData;
    uniform float4x4 gpuiTransformOffset;
    uniform float LODLevel; // -1 if crossFade disabled, 0-7 if cross fade enabled
    uniform float fadeLevelMultiplier; // 1 no animation

#if UNITY_VERSION >= 201711

    #ifdef UNITY_APPLY_DITHER_CROSSFADE
        #undef UNITY_APPLY_DITHER_CROSSFADE
    #endif

#if SHADER_API_GLCORE || SHADER_API_GLES3
    #define UNITY_APPLY_DITHER_CROSSFADE(vpos)
#else
    #ifdef LOD_FADE_CROSSFADE
        #define UNITY_APPLY_DITHER_CROSSFADE(vpos) UnityApplyDitherCrossFadeGPUI(vpos)
    #else
        #define UNITY_APPLY_DITHER_CROSSFADE(vpos)
    #endif
#endif

#if UNITY_VERSION >= 201920
    uniform sampler2D _DitherMaskLOD2D;
#else
    #ifndef LOD_FADE_CROSSFADE
        uniform sampler2D _DitherMaskLOD2D;
    #endif // LOD_FADE_CROSSFADE
#endif

    void UnityApplyDitherCrossFadeGPUI(float2 vpos)
    {
        if (LODLevel >= 0)
        {
            uint4 lodData = gpuiInstanceLODData[gpuiTransformationMatrix[unity_InstanceID]];

            if(lodData.w > 0)
            {
                float fadeLevel = floor(lodData.w * fadeLevelMultiplier);

                if (lodData.z == uint(LODLevel))
                    fadeLevel = 15 - fadeLevel;

                if(fadeLevel > 0)
                {
                    vpos /= 4; // the dither mask texture is 4x4
                    vpos.y = (frac(vpos.y) + fadeLevel) * 0.0625 /* 1/16 */; // quantized lod fade by 16 levels
                    clip(tex2D(_DitherMaskLOD2D, vpos).a - 0.5);
                }
            }
        }
    }
#endif // UNITY_VERSION

#endif // SHADER_API

#ifdef unity_ObjectToWorld
    #undef unity_ObjectToWorld
#endif

#ifdef unity_WorldToObject
    #undef unity_WorldToObject
#endif

#endif // UNITY_PROCEDURAL_INSTANCING_ENABLED

void setupGPUI()
{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
#if GPUI_MHT_COPY_TEXTURE
    uint textureWidth = bufferSize;
    if(bufferSize > maxTextureSize)
        textureWidth = maxTextureSize;

    float indexX = ((unity_InstanceID % uint(maxTextureSize)) + 0.5) / textureWidth;
    float rowCount = ceil(bufferSize / maxTextureSize);
    float rowIndex = floor(unity_InstanceID / maxTextureSize);

    float4x4 transformData = float4x4(
        tex2Dlod(gpuiTransformationMatrixTexture, float4(indexX, (0.0f + 0.5f + rowIndex * 4) / (4.0f * rowCount), 0.0f, 0.0f)), // row0
        tex2Dlod(gpuiTransformationMatrixTexture, float4(indexX, (1.0f + 0.5f + rowIndex * 4) / (4.0f * rowCount), 0.0f, 0.0f)), // row1
        tex2Dlod(gpuiTransformationMatrixTexture, float4(indexX, (2.0f + 0.5f + rowIndex * 4) / (4.0f * rowCount), 0.0f, 0.0f)), // row2
        tex2Dlod(gpuiTransformationMatrixTexture, float4(indexX, (3.0f + 0.5f + rowIndex * 4) / (4.0f * rowCount), 0.0f, 0.0f))  // row3
    );
    unity_ObjectToWorld = mul(transformData, gpuiTransformOffset);
#elif GPUI_MHT_MATRIX_APPEND
		unity_ObjectToWorld = mul(gpuiTransformationMatrix[unity_InstanceID], gpuiTransformOffset);
#else        
        uint index = gpuiTransformationMatrix[unity_InstanceID];
		unity_ObjectToWorld = mul(gpuiInstanceData[index], gpuiTransformOffset);
#endif // SHADER_API

	// inverse transform matrix
	// taken from richardkettlewell's post on
	// https://forum.unity3d.com/threads/drawmeshinstancedindirect-example-comments-and-questions.446080/

	float3x3 w2oRotation;
	w2oRotation[0] = unity_ObjectToWorld[1].yzx * unity_ObjectToWorld[2].zxy - unity_ObjectToWorld[1].zxy * unity_ObjectToWorld[2].yzx;
	w2oRotation[1] = unity_ObjectToWorld[0].zxy * unity_ObjectToWorld[2].yzx - unity_ObjectToWorld[0].yzx * unity_ObjectToWorld[2].zxy;
	w2oRotation[2] = unity_ObjectToWorld[0].yzx * unity_ObjectToWorld[1].zxy - unity_ObjectToWorld[0].zxy * unity_ObjectToWorld[1].yzx;

	float det = dot(unity_ObjectToWorld[0].xyz, w2oRotation[0]);

	w2oRotation = transpose(w2oRotation);

	w2oRotation *= rcp(det);

	float3 w2oPosition = mul(w2oRotation, -unity_ObjectToWorld._14_24_34);

	unity_WorldToObject._11_21_31_41 = float4(w2oRotation._11_21_31, 0.0f);
	unity_WorldToObject._12_22_32_42 = float4(w2oRotation._12_22_32, 0.0f);
	unity_WorldToObject._13_23_33_43 = float4(w2oRotation._13_23_33, 0.0f);
	unity_WorldToObject._14_24_34_44 = float4(w2oPosition, 1.0f);
#endif // UNITY_PROCEDURAL_INSTANCING_ENABLED
}

#endif // GPU_INSTANCER_INCLUDED