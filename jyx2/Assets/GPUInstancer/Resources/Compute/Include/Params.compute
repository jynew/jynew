#ifndef __params_hlsl_
#define __params_hlsl_

// instance data
RWStructuredBuffer<float4x4> gpuiInstanceData;
uniform uint bufferSize;

// bounds
uniform float3 boundsCenter;
uniform float3 boundsExtents;

// camera data
uniform float4x4 mvpMatrix;
uniform float3 camPos;
uniform float halfAngle;

// global culling
uniform float minCullingDistance;

// distance culling
uniform float minDistance;
uniform float maxDistance;

//frustum culling
uniform bool isFrustumCulling;
uniform float frustumOffset;

// occlusion culling
uniform bool isOcclusionCulling;
uniform float occlusionOffset;
uniform uint occlusionAccuracy;
uniform float2 hiZTxtrSize;
uniform Texture2D<float4> hiZMap;
uniform SamplerState sampler_hiZMap; // variable name is recognized by the compiler to reference hiZMap

// shadows
uniform bool cullShadows;
uniform float shadowDistance;
uniform float4x4 shadowLODMap;

// LOD
uniform float4x4 lodSizes;
uniform uint lodCount;
uniform float deltaTime;
uniform bool animateCrossFade;

#endif
