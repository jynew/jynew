// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//-----------------------------------------------------
// Deferred screen space decal includes. Version 0.9 [Beta]
// Copyright (c) 2017 by Sycoforge
//-----------------------------------------------------

#ifndef DSSD_SYCO_CG_INCLUDED
#define DSSD_SYCO_CG_INCLUDED

#include "UnityCG.cginc"
#include "UnityGlobalIllumination.cginc"


#include "Util.cginc"
#include "SycoSSD.cginc"	
#include "DSSDSignatures.cginc"	

uniform sampler2D _DiffuseAOBuffer;		  // RT0
uniform sampler2D _SpecSmoothBuffer;	  // RT1
uniform sampler2D _NormalBuffer;		  // RT2
uniform sampler2D _LightingEmissionBuffer;// RT3

fixed _NormalMultiplier;
fixed _SpecularMultiplier;
fixed _SmoothnessMultiplier;

half4 _SpecularColor;

//-------------------------
// Deferred SSD
//-------------------------

#define SETUP_DSSD_DIFF(input,o) \
	SETUP_BASIC_SSD(input,o) \
	o.orientation[0] = V_RIGHT; \
	o.orientation[1] = mul ((float3x3)unity_ObjectToWorld, V_UP); \
	o.orientation[2] = V_FORWARD;

#define SETUP_DSSD_DIFF_NORM(input,o) \
	SETUP_BASIC_SSD(input,o) \
	o.orientation[0] = mul ((float3x3)unity_ObjectToWorld, V_RIGHT); \
	o.orientation[1] = mul ((float3x3)unity_ObjectToWorld, V_UP); \
	o.orientation[2] = mul ((float3x3)unity_ObjectToWorld, V_FORWARD);


#define COLOR_TO_NORMAL(rgb) rgb * 2.0f - 1.0f 
#define NORMAL_TO_COLOR(xzy) fixed4(xzy * 0.5f + 0.5f, 1)

#define DSSD_DISCARD_WORLD_NORMAL(worldNormalBuffered, input) \
	float c = dot(worldNormalBuffered, input.orientation[1]); \
	clip(c - input.normal.x)

#define DSSD_DISCARD_BUFFER_MASK(mask) clip((mask).w - 0.1f)

#define DSSD_DISCARD_ALPHA(color) clip((color).a - 0.25f)

#define DSSD_DISCARD_MASK(color) clip((color).a - 0.25f)

#define DSSD_DISCARD_BORDER(uv) \
	fixed2 uv2 = uv.xz + 0.5f; \
	fixed2 pixelSize = (_ScreenParams.zw - 1.0f)*2; \
	clip(uv2 - pixelSize); \
	clip(uv2 + pixelSize > 1.0f ? -1 : 1)

#define WRITE_ATLAS_NORMALS_LERP(tex, st, input, localPos, worldNormalBuffered, output) \
	fixed3 n = UnpackNormal(tex2D((tex), ATLAS_TEXCOORDS((input), (st), (localPos)))); \
	n.xy *= _NormalMultiplier; \
	half3x3 norMat = half3x3((input).orientation[0], (input).orientation[2], (input).orientation[1]); \
	n = mul(n, norMat); \
	fixed fac = abs(dot(worldNormalBuffered, n)); \
	n = normalize(lerp(worldNormalBuffered, n, fac)); \
	output = fixed4(n * 0.5f + 0.5f, 1.0f)

#define WRITE_ATLAS_NORMALS(tex, st, input, localPos, worldNormalBuffered, output) \
	fixed3 n = UnpackNormal(tex2D((tex), ATLAS_TEXCOORDS((input), (st), (localPos)))); \
	n.xy *= _NormalMultiplier;  \
	half3x3 norMat = half3x3((input).orientation[0], (input).orientation[2], (input).orientation[1]); \
	n = mul(n, norMat); \
	n = normalize(worldNormalBuffered + n); \
	output = fixed4(n * 0.5f + 0.5f, 1.0f)

#define GBUFFER_RAW_WORLD_NORMAL tex2D(_NormalBuffer, screenUV)
#define GBUFFER_SPEC_SMOOTH tex2D(_SpecSmoothBuffer, screenUV)
#define GBUFFER_DIFFUSE_AO tex2D(_DiffuseAOBuffer, screenUV)
#define GBUFFER_LIGHTING_EMISSION tex2D(_LightingEmissionBuffer, screenUV)

#define DIFFUSE(input, localPos) ATLAS_TEX2D(_MainTex, _MainTex_ST, (input), (localPos)) * _Color;
#define SPECSMOOTH(input, localPos) ATLAS_TEX2D(_SpecGlossMap, _MainTex_ST, (input), (localPos));
#define EMISSION(input, localPos) ATLAS_TEX2D(_EmissionMap, _MainTex_ST, (input), (localPos));
#define NORMAL(input, localPos) UnpackNormal(ATLAS_TEX2D(_BumpMap, _MainTex_ST, (input), (localPos)));

	
#define SPECSMOOTH_SCALE(sm) float4((sm).rgb * _SpecularMultiplier * _SpecularColor, (sm).a * _SmoothnessMultiplier);
#define NORMAL_SCALE(n) normalize(fixed3(n.xy * _NormalMultiplier, n.z));


#define BLEND_LINEAR(buffer,color) lerp((buffer),(color),opacity)
#define PREMUL(color) float4(color.rgb*opacity,opacity)


#define OPACITY(input) color.a * input.color.a;

UnityLight DummyLight(half3 normalWorld)
{
	UnityLight l;
	l.color = (half3)0;
	l.dir = V_UP;
	l.ndotl = LambertTerm(normalWorld, l.dir);
	return l;
}

inline UnityGI FragmentGI(float3 posWorld, float3 eyeVec, float3 normalWorld, half oneMinusRoughness, half occlusion, half4 i_ambientOrLightmapUV, half atten, UnityLight light, bool reflections)
{
	UnityGIInput d;

	d.light = light;
	d.worldPos = posWorld;
	d.worldViewDir = -eyeVec;
	d.atten = atten;

#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
	d.ambient = 0;
	d.lightmapUV = i_ambientOrLightmapUV;
#else
	d.ambient = i_ambientOrLightmapUV.rgb;
	d.lightmapUV = 0;
#endif

	d.probeHDR[0] = unity_SpecCube0_HDR;
	d.probeHDR[1] = unity_SpecCube1_HDR;
#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
	d.boxMin[0] = unity_SpecCube0_BoxMin;
#endif
#if UNITY_SPECCUBE_BOX_PROJECTION
	d.boxMax[0] = unity_SpecCube0_BoxMax;
	d.probePosition[0] = unity_SpecCube0_ProbePosition;
	d.boxMax[1] = unity_SpecCube1_BoxMax;
	d.boxMin[1] = unity_SpecCube1_BoxMin;
	d.probePosition[1] = unity_SpecCube1_ProbePosition;
#endif

	if (reflections)
	{
		Unity_GlossyEnvironmentData g;
		g.roughness = 1 - oneMinusRoughness;
		g.reflUVW = reflect(eyeVec, normalWorld);
		return UnityGlobalIllumination(d, occlusion, normalWorld, g);
	}
	else
	{
		return UnityGlobalIllumination(d, occlusion, normalWorld);
	}
}

inline half3 GetAmbient(float4 convertedColor, float4 specColor, float3 posScreen, float3 normalWorld, float3 positionWorld, float3 eyeVec, float occlusion, float oneMinusRoughness, float oneMinusReflectivity)
{
#if UNITY_ENABLE_REFLECTION_BUFFERS
	bool sampleReflectionsInDeferred = false;
#else
	bool sampleReflectionsInDeferred = true;
#endif

	UnityLight dummyLight = DummyLight(normalWorld);

	half atten = 0;

	UnityGI gi = FragmentGI(positionWorld, eyeVec, normalWorld, oneMinusRoughness, occlusion, half4(0,0,0,0), atten, dummyLight, sampleReflectionsInDeferred);

	half3 ambient = UNITY_BRDF_PBS(convertedColor, specColor, oneMinusReflectivity, oneMinusRoughness, normalWorld, -eyeVec, gi.light, gi.indirect).rgb;
	ambient += UNITY_BRDF_GI(convertedColor, specColor, oneMinusReflectivity, oneMinusRoughness, normalWorld, -eyeVec, occlusion, gi);

	return ambient;
}

inline half4 GetFastAmbient(half4 emission, half4 color, half4 lightingBuffered, half3 normalWorld, half opacity)
{
	half4 ambientContrib = half4(ShadeSH9(half4(normalWorld, 1.0f)), 1);
	half f = lightingBuffered * ambientContrib;
	half4 c = (color * f ) + emission;

	#ifndef UNITY_HDR_ON
		c.rgb = exp2(-1.0 * c.rgb);
	#endif

	half4 ambient = BLEND_LINEAR(lightingBuffered, c);

	return ambient;
}

inline half4 GetApproxAmbient(half4 color, half4 lightingBuffered, half3 normalWorld, half opacity)
{
	#ifndef UNITY_HDR_ON
		color.rgb = exp2(-1.0 * color.rgb);
	#endif

	float d = float4(normalWorld, 1.0f);

	half4 ambient = float4(dot(d, unity_SHAr), dot(d, unity_SHAg), dot(d, unity_SHAb), 1) * color;

	return ambient;
}


FragmentInputDSSD vert(VertexInputExt input)
{
	FragmentInputDSSD o = (FragmentInputDSSD)0;

	SETUP_DSSD_DIFF_NORM(input,o)

	return o;
}

inline void DiscardPixel(FragmentInputDSSD input, fixed4 rawWorldNormalBuffered, fixed3 worldNormalBuffered )
{
	DSSD_DISCARD_BUFFER_MASK(rawWorldNormalBuffered);
	DSSD_DISCARD_WORLD_NORMAL(worldNormalBuffered, input);
	//DSSD_DISCARD_ALPHA(color);
	//DSSD_DISCARD_BORDER(localPos);
}

//-------------------------------------------------------------------
// Fragment Shaders
//-------------------------------------------------------------------

// Mappings and channel order
//out half4 outDiffuse			: SV_TARGET0,	// RT0: diffuse color (rgb), occlusion (a)
//out half4 outSpecSmoothness	: SV_TARGET1,	// RT1: spec color (rgb), smoothness (a)
//out half4 outNormal			: SV_TARGET2,	// RT2: normal (rgb), --unused, very low precision-- (a) 
//out half4 outEmission			: SV_TARGET3	// RT3: emission (rgb), --unused-- (a)


void frag(FragmentInputDSSD input, 

#ifndef SIGN
#ifdef DIFFUSE_ON
	#ifdef SPECSMOOTH_ON
		#ifdef NORMAL_ON
			SIGNATURE_DSN
			#define SIGN
		#endif
	#endif
#endif
#endif

//-------------------
#ifndef SIGN
#ifdef DIFFUSE_ON
	#ifdef SPECSMOOTH_ON
		SIGNATURE_DS
		#define SIGN
	#endif
#endif
#endif

//-------------------
#ifndef SIGN
#ifdef DIFFUSE_ON
	#ifdef NORMAL_ON
		SIGNATURE_DN
		#define SIGN
	#endif
#endif
#endif

//-------------------
#ifndef SIGN
#ifdef SPECSMOOTH_ON
	#ifdef NORMAL_ON
		SIGNATURE_SN
		#define SIGN
	#endif
#endif
#endif

//-------------------
#ifndef SIGN
#ifdef DIFFUSE_ON
	SIGNATURE_D
	#define SIGN
#endif
#endif

//-------------------
#ifndef SIGN
#ifdef SPECSMOOTH_ON
	SIGNATURE_S
	#define SIGN
#endif
#endif

//-------------------
#ifndef SIGN
#ifdef NORMAL_ON
	SIGNATURE_N
	#define SIGN
#endif
#endif

//out float4 outEmission		: SV_TARGET3)
{
	PROCESS_BASIC_SSD(input);

	fixed4 color = DIFFUSE(input, localPos);
	fixed4 specSmooth = SPECSMOOTH(input, localPos);
	specSmooth = SPECSMOOTH_SCALE(specSmooth);

	fixed3 normalTangent = NORMAL(input, localPos);
	normalTangent = NORMAL_SCALE(normalTangent);

	fixed4 emission = EMISSION(input, localPos);
	emission *= _EmissionColor;


	float opacity = OPACITY(input);

	//-----------------------
	// Fetch gbuffer values
	//-----------------------
	fixed4 diffuseBuffered = GBUFFER_DIFFUSE_AO;
	fixed4 rawWorldNormalBuffered = GBUFFER_RAW_WORLD_NORMAL;
	fixed3 worldNormalBuffered = COLOR_TO_NORMAL(rawWorldNormalBuffered);
	fixed4 specSmoothBuffered = GBUFFER_SPEC_SMOOTH;
	fixed4 lightingBuffered = GBUFFER_LIGHTING_EMISSION;

	DiscardPixel(input, rawWorldNormalBuffered, worldNormalBuffered);


	half3x3 norMat = half3x3(input.orientation[0], input.orientation[2], input.orientation[1]);
	fixed3 normalWorld = mul((fixed3)normalTangent, norMat);
	fixed4 normalWorldColor = NORMAL_TO_COLOR(normalWorld);

	half oneMinusReflectivity;
	half oneMinusRoughness = specSmooth.a;

	half4 convertedColor = float4( EnergyConservationBetweenDiffuseAndSpecular(color, specSmooth.rgb, /*out*/ oneMinusReflectivity), 1);
	convertedColor.rgb = PreMultiplyAlpha(color.rgb, opacity, oneMinusReflectivity, /*out*/ opacity);


	//-----------------------
	// Blend values
	//-----------------------
	#ifdef DIFFUSE_ON
	float4 diffuseBlended = BLEND_LINEAR(diffuseBuffered, convertedColor);
	diffuseBlended.a = 1;// Skip AO for now
	#endif

	#ifdef SPECSMOOTH_ON
	float4 specSmoothBlended = BLEND_LINEAR(specSmoothBuffered, specSmooth);
	#endif

	#ifdef NORMAL_ON
	float4 normalBlended = BLEND_LINEAR(rawWorldNormalBuffered, normalWorldColor);
	#endif

	float4 gi = GetFastAmbient(emission, convertedColor, lightingBuffered, normalWorld, opacity);
	float4 emissionBlended = BLEND_LINEAR(lightingBuffered, gi);


	//-----------------------
	// Output values
	//-----------------------
	#ifdef DIFFUSE_ON
	outDiffuse = diffuseBlended;		// RT0
	#endif

	#ifdef SPECSMOOTH_ON
	outSpecSmooth = specSmoothBlended;	// RT1
	#endif
	
	#ifdef NORMAL_ON
	outNormal = normalBlended;			// RT2
	#endif

	outEmission = emissionBlended;		// RT3
}

#endif // DSSD_SYCO_CG_INCLUDED