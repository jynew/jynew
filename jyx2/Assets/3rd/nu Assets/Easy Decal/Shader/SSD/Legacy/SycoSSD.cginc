// Upgrade NOTE: commented out 'float4x4 _CameraToWorld', a built-in variable
// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//-----------------------------------------------------
// Deferred screen space decal includes. Version 0.9 [Beta]
// Based on concept by Pope Kim. SIGGRAPH 2012.
// Copyright (c) 2017 by Sycoforge
//-----------------------------------------------------

#ifndef SSD_SYCO_CG_INCLUDED
#define SSD_SYCO_CG_INCLUDED

#include "UnityCG.cginc"
#include "UnityStandardInput.cginc"
#include "Util.cginc"


uniform sampler2D _CameraDepthTexture;

CBUFFER_START(UnityPerCamera2)
// float4x4 _CameraToWorld;
CBUFFER_END


struct VertexInputExt 
{
	float4 vertex	: POSITION;
	float4 normal	: NORMAL;
	fixed2 uv		: TEXCOORD0;
	fixed2 uv2		: TEXCOORD1;
	fixed4 color	: COLOR0;
};

struct FragmentInputDSSD
{
	float4 position		: SV_POSITION;
	float4 normal		: NORMAL;

	fixed4 uv			: TEXCOORD0;
	float4x4 mpv_inv	: TEXCOORD1;		
	float4 screenPos	: TEXCOORD5;
	float3 ray			: TEXCOORD6;

	half3x3 orientation : TEXCOORD7;

	//fixed3 eyeVec		: TEXCOORD10;
				
	fixed4 color		: COLOR;	
};

struct FragmentInputFSSD
{
	float4 position		: SV_POSITION;
	float3 normal		: NORMAL;

	fixed4 uv			: TEXCOORD0;
	float4x4 mpv_inv	: TEXCOORD1;		
	float4 screenPos	: TEXCOORD5;
	float3 ray			: TEXCOORD6;

	UNITY_FOG_COORDS(7)	  
				
	fixed4 color	: COLOR;

};


#define PROCESS_BASIC_SSD(input) \
	fixed ortho = unity_OrthoParams.w; \
	float4x4 invMVP = input.mpv_inv; \
	input.ray = ortho ? input.ray : input.ray * (_ProjectionParams.z / input.ray.z); \
	float2 screenUV = input.screenPos.xy / input.screenPos.w; \
	float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV); \
	depth = ortho ? depth : Linear01Depth(depth); \
	float4 viewPos = ortho ? float4(input.ray.xy, depth, 1.0f) : float4(input.ray * depth, 1.0f); \
	float4 worldPos = mul (invMVP, viewPos); \
	float3 localPos = worldPos.xyz / worldPos.w; \
	clip(0.5f - abs(localPos)) \


#define SETUP_BASIC_SSD(input,o) \
	fixed ortho = unity_OrthoParams.w; \
	float4 pp = float4(input.vertex.xyz, 1.0f); \
	float4 p = UnityObjectToClipPos (pp); \
	float4x4 mi = ortho ? inverseMat(UNITY_MATRIX_MVP) : mul(unity_WorldToObject, unity_CameraToWorld); \
	float4 worldPos = mul(unity_ObjectToWorld, input.vertex); \
	(o).position = p; \
	(o).normal = input.normal; \
	(o).screenPos = ComputeScreenPos(p); \
	(o).ray = mul(UNITY_MATRIX_MV, pp).xyz * float3(-1, -1, 1); \
	(o).uv = float4(input.uv, input.uv2); \
	(o).color = input.color; \
	(o).mpv_inv = mi; \

//(o).eyeVec = float4(worldPos.xyz - _WorldSpaceCameraPos, 0.0); \

#define ATLAS_TEXCOORDS(input, st, localPos) (input.uv.xy * st.xy) * (localPos.xz + 0.5f) + (input.uv.zw + st.zw)
#define ATLAS_TEX2D(tex, st, input, localPos) tex2D(tex,  ATLAS_TEXCOORDS(input, st, localPos))


#endif // SSD_SYCO_CG_INCLUDED
