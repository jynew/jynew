

//#pragma shader_feature SKIP_REFRACTION_GRABPASS

/*#if defined(MINIMUM_MODE)
#define MYFLOAT half
#define MYFLOAT2 half2
#define MYFLOAT3 half3
#define MYFLOAT4 half4
#else*/
#define MYFLOAT float
#define MYFLOAT2 float2
#define MYFLOAT3 float3
#define MYFLOAT4 float4
/*#endif*/



#if defined(MINIMUM_MODE) || defined(ULTRA_FAST_MODE)
#define MYFIXED fixed
#define MYFIXED2 fixed2
#define MYFIXED3 fixed3
#define MYFIXED4 fixed4
#else
#define MYFIXED half
#define MYFIXED2 half2
#define MYFIXED3 half3
#define MYFIXED4 half4
#endif

///DEBUG
//RWStructuredBuffer<float4> buffer : register(u1);
//static const MYFIXED4 DEBUG_VECTOR ;
///DEBUG

//#define USE_WPOS = 1


/*#if !defined(REFRACTION_GRABPASS)
		#define SKIP_REFRACTION_GRABPASS = 1
#endif*/
/*#if !defined(USE_NOISED_GLARE_ADDWAWES1)
		#define SKIP_NOISED_GLARE_ADDWAWES1 = 1
#endif*/
/*#if !defined(USE_NOISED_GLARE_ADDWAWES2)
		#define SKIP_NOISED_GLARE_ADDWAWES2 = 1
#endif*/
/*#if !defined(USE_NOISED_GLARE_LQ)
		#define SKIP_NOISED_GLARE_LQ = 1
#endif*/

//#define USE_SHADOWS = 1
//#define USE_GRABPASS = 1


#if !defined(ULTRA_FAST_MODE) && !defined(MINIMUM_MODE)
#define ADVANCE_PC = 1
#endif
#if  defined(REFLECTION_2D) || defined(REFLECTION_PLANAR)||defined(REFLECTION_PROBE_AND_INTENSITY)||defined(REFLECTION_PROBE_AND_INTENSITY)||defined(REFLECTION_PROBE)|| defined(REFLECTION_USER) || defined(REFLECTION_JUST_COLOR)
#define HAS_REFLECTION = 1
#endif



//#define LOW_NORMALS = 1
uniform MYFIXED _ObjecAngle;
#if !defined(ULTRA_FAST_MODE)  && !defined(MINIMUM_MODE)
 float _FracTimeFull;
#endif
 MYFIXED _Frac2PITime;
 MYFIXED _Frac01Time;
 MYFIXED _Frac01Time_d8_mBlendAnimSpeed;
 MYFIXED _Frac01Time_MAIN_TEX_BA_Speed;
 float2 _FracWTime_m4_m3DWavesSpeed_dPI2;
 MYFIXED2 _Frac_UFSHORE_Tile_1Time_d10_m_UFSHORE_Speed1;
 MYFIXED2 _Frac_UFSHORE_Tile_2Time_d10_m_UFSHORE_Speed2;
 MYFIXED _Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1;

//uv scroll
 float2 _Frac_WaterTextureTilingTime_m_AnimMove;
 float4 _Frac_UVS_DIR;
//main tex anim
 float2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move;
 float2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed;
 float2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed;


//uniform MYFIXED _BAKED_DEPTH_EXTERNAL_TEXTURE_Amount;
uniform MYFIXED _LOW_DISTOR_Tile;
uniform MYFIXED _LOW_DISTOR_Speed;
uniform MYFIXED _LOW_DISTOR_Amount;
// REQUEST CHECK //
#ifdef WATER_DOWNSAMPLING
uniform MYFIXED DOWNSAMPLING_SAMPLE_SIZE;
float _FrameRate;
sampler2D _FrameBuffer;
#endif
#ifdef WATER_DOWNSAMPLING_HARD
uniform MYFIXED DOWNSAMPLING_SAMPLE_SIZE;
float _FrameRate;
sampler2D _FrameBuffer;
#endif

//uniform half _FracTimeX;
//uniform half _FracTimeW;
uniform MYFIXED _BumpMixAmount;
uniform MYFIXED _Z_BLACK_OFFSET_V;


#if !defined(USING_FOG) && !defined(SKIP_FOG)
#define SKIP_FOG = 1
#endif




#if defined(REFRACTION_BAKED_FROM_TEXTURE) || defined(REFRACTION_BAKED_ONAWAKE) || defined(REFRACTION_BAKED_VIA_SCRIPT)
#define HAS_BAKED_REFRACTION = 1
#endif


#if defined(SURFACE_FOG)
uniform MYFIXED _SURFACE_FOG_Amount;
uniform MYFIXED _SURFACE_FOG_Speed;
uniform MYFIXED _SURFACE_FOG_Tiling;
#endif


#if defined(REFRACTION_GRABPASS) || defined(HAS_BAKED_REFRACTION) || defined(REFRACTION_ONLYZCOLOR)
#if !defined(DEPTH_NONE)
#define HAS_REFRACTION = 1
#endif
#define REQUEST_DEPTH = 1
#endif

#if (defined(REFRACTION_Z_BLEND) || defined(REFRACTION_Z_BLEND_AND_FRESNEL))&& !defined(DEPTH_NONE)
#if defined(REFRACTION_Z_BLEND_AND_FRESNEL)
#define HAS_REFRACTION_Z_BLEND_AND_RRFRESNEL = 1
#else
#define HAS_REFRACTION_Z_BLEND = 1
#endif
uniform MYFIXED _RefractionBlendFade;
uniform MYFIXED _RefractionBlendOffset;
#endif

#if defined(RRFRESNEL) || defined(HAS_REFRACTION_Z_BLEND_AND_RRFRESNEL)
uniform MYFIXED _RefrBled_Fres_Amount;
uniform MYFIXED _RefrBled_Fres_Pow;
#endif

#if !defined(DEPTH_NONE)
uniform MYFIXED _RefrDistortionZ;
#endif




#if defined(USE_OUTPUT_GRADIENT) &&( defined(USE_OUTPUT_BLEND_1) || !defined(USE_OUTPUT_BLEND_3)) 
#if !defined(REQUEST_DEPTH)
#define REQUEST_DEPTH = 1
#endif
#endif

#if !defined(SKIP_3DVERTEX_ANIMATION) && (defined(VERTEX_ANIMATION_BORDER_FADE) )

#if !defined(REQUEST_DEPTH)
#define REQUEST_DEPTH = 1
#endif
#endif
#if !defined(SKIP_3DVERTEX_ANIMATION)
#if defined(USE_SIMPLE_VHEIGHT_FADING)
uniform MYFIXED SIMPLE_VHEIGHT_FADING_AFFECT;
#endif
#endif


/*#if !defined(SKIP_3DVERTEX_ANIMATION) && (defined(VERTEX_ANIMATION_BORDER_FADE) || !defined(SKIP_3DVERTEX_HEIGHT_COLORIZE))
#if  !defined(SKIP_3DVERTEX_HEIGHT_COLORIZE)
#define HAS_FOAM = 1
#endif
#define REQUEST_DEPTH = 1
#endif*/
// REQUEST CHECK //


/**/#if defined(DEPTH_NONE)//
#if !defined(SKIP_FOAM)

#define SKIP_FOAM = 1
#endif
/**/#elif defined(BAKED_DEPTH_ONAWAKE) || defined(BAKED_DEPTH_VIASCRIPT) || defined(BAKED_DEPTH_EXTERNAL_TEXTURE)//
#define HAS_BAKED_DEPTH = 1
/**/#else//

#define HAS_CAMERA_DEPTH = 1
/*#if defined(UNITY_MIGHT_NOT_HAVE_DEPTH_Texture) || defined(UNITY_MIGHT_NOT_HAVE_DEPTH_TEX)
#if defined(ALLOW_MANUAL_DEPTH) 
#define HAS_CAMERA_DEPTH = 1
#endif
#else
#define HAS_CAMERA_DEPTH = 1
#endif*/
/**/#endif//






#if !defined(SKIP_FOAM)
#if !defined(DEPTH_NONE)
#define HAS_FOAM = 1
#endif
#if !defined(REQUEST_DEPTH)
#define REQUEST_DEPTH = 1
#endif
#endif
/**/#if defined(DEPTH_NONE)//
#if defined(REQUEST_DEPTH) 
#define WRONG_DEPTH = 1
#endif
#endif


//#if defined(RRSIMPLEBLEND) || defined(RRMULTIBLEND)
uniform MYFIXED _AverageOffset;
//#endif



#if defined(REFLECTION_NONE) && !defined(SKIP_REFLECTION_MASK)
#define SKIP_REFLECTION_MASK = 1
#endif


#if defined(HAS_CAMERA_DEPTH) || defined(HAS_BAKED_DEPTH)|| defined(NEED_SHORE_WAVES_UNPACK) || defined(SHORE_WAVES) && !defined(DEPTH_NONE) || defined(USE_CAUSTIC) || defined(SURFACE_FOG) /*&& !defined(SKIP_CALCULATE_HEIGHT_DEPTH)*/
#define USE_WPOS = 1
		//uniform MYFIXED _CameraFarClipPlane;
		//uniform FIXED4x4 _ClipToWorld;
#endif
//#endif//

#include "UnityCG.cginc"
#if !defined(SKIP_LIGHTING) || !defined(SKIP_SPECULAR) || !defined(SKIP_FLAT_SPECULAR)
#if !defined(USE_FAKE_LIGHTING)

#include "UnityLightingCommon.cginc"
#endif 
#endif 
		//#include "Lighting.cginc"
		//SHADOW

#if defined(USE_SHADOWS) && !defined(SHADER_API_GLES) && !defined(MINIMUM_MODE)
#define HAS_USE_SHADOWS = 1
#endif

#if defined(HAS_USE_SHADOWS)
//#include "Lighting.cginc"
uniform MYFIXED _ShadorAmount;
#include "AutoLight.cginc"
#endif 


#if !defined(SKIP_FOAM_FINE_REFRACTIOND_DOSTORT) && !defined(SKIP_FOAM)
#define FOAM_FINE_REFRACTIOND_DOSTORT = 1
uniform MYFIXED _FixMulty;
#endif 


#if defined(HAS_BAKED_REFRACTION)
uniform MYFIXED _RefractionBakeLayers;
#endif
#if defined(HAS_BAKED_DEPTH)
uniform MYFIXED _ZDepthBakeLayers;
#endif

//#if !defined(SKIP_Z_WORLD_CALCULATION)


#if defined(HAS_REFRACTION) && defined(REFR_MASK)
#define HAS_REFR_MASK = 1
uniform half _REFR_MASK_Tile;
uniform MYFIXED _REFR_MASK_Amount;
uniform MYFIXED _REFR_MASK_min;
uniform MYFIXED _REFR_MASK_max;
uniform MYFIXED2 _REFR_MASK_offset;
#endif





#if defined(UF_AMOUNTMASK) && !defined(_UF_NMASK_USE_MAINTEX) || !defined(SKIP_UNWRAP_TEXTURE) && defined(_ShoreWaves_SECOND_TEXTURE) || defined(POST_TEXTURE_TINT) && defined(POST_OWN_TEXTURE) || defined(POST_TEXTURE_TINT) && defined(POST_SECOND_TEXTURE)
sampler2D _UF_NMASK_Texture;
#define HAS_SECOND_TEXTURE = 1
#endif

#if defined(SHORE_WAVES) && defined(ADVANCE_PC) || defined(UFAST_SHORE_1) && !defined(_ShoreWaves_SECOND_TEXTURE) && !defined(_ShoreWaves_USE_MAINTEX) && !defined(ADVANCE_PC)
uniform sampler2D _ShoreWavesGrad;
#define HAS_SHORE_WAVES_GRAD = 1
#endif



#if defined(MINIMUM_MODE) || defined(ULTRA_FAST_MODE)






#if defined(UF_AMOUNTMASK)
uniform half _UF_NMASK_Tile;
uniform MYFIXED2 _UF_NMASK_offset;
uniform MYFIXED _UF_NMASK_Contrast;
uniform MYFIXED _UF_NMASK_Brightnes;
#endif
#else
#if defined(AMOUNTMASK)
uniform half _AMOUNTMASK_Tile;
uniform MYFIXED _AMOUNTMASK_Amount;
uniform MYFIXED _AMOUNTMASK_min;
uniform MYFIXED _AMOUNTMASK_max;
uniform MYFIXED2 _AMOUNTMASK_offset;

#endif
#if defined(TILINGMASK)
uniform half _TILINGMASK_Tile;
uniform MYFIXED _TILINGMASK_Amount;
uniform MYFIXED _TILINGMASK_min;
uniform MYFIXED _TILINGMASK_max;
uniform MYFIXED2 _TILINGMASK_offset;
uniform MYFIXED _TILINGMASK_factor;
#endif
#if defined(MAINTEXMASK)
uniform half _MAINTEXMASK_Tile;
uniform MYFIXED _MAINTEXMASK_Amount;
uniform MYFIXED _MAINTEXMASK_min;
uniform MYFIXED _MAINTEXMASK_max;
uniform MYFIXED2 _MAINTEXMASK_offset;
#endif
#endif


#if defined(SIN_OFFSET)
uniform MYFIXED _sinFriq;
uniform MYFIXED _sinAmount;
#endif


#if !defined(SKIP_LIGHTING) || !defined(SKIP_SPECULAR) || !defined(SKIP_FLAT_SPECULAR)
uniform MYFIXED _LightNormalsFactor;
#if defined(USE_FAKE_LIGHTING)
uniform MYFIXED3 _LightColor0Fake;
#endif 
#if defined(LIGHTING_BLEND_COLOR)
uniform MYFIXED3 _BlendColor;
#endif 
#endif 

#if defined(HAS_BAKED_REFRACTION)
#if defined(REFRACTION_BAKED_FROM_TEXTURE) 
uniform sampler2D _RefractionTex;
#else
uniform sampler2D _RefractionTex_temp;
#endif
#endif

#if !defined(SKIP_REFRACTION_CALC_DEPTH_FACTOR) || !defined(SKIP_Z_CALC_DEPTH_FACTOR)
uniform MYFIXED _RefrDeepFactor;
#endif

#if defined(HAS_CAMERA_DEPTH)
//uniform sampler2D _CameraDepthTexture;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
//uniform sampler2D_float _CameraDepthTexture;
#endif

#if defined(USE_OUTPUT_GRADIENT)
uniform sampler2D _GradTexture;
uniform MYFIXED _OutGradZ;
uniform MYFIXED _FixHLClamp;
#endif

uniform MYFIXED4 _MainTexColor;

#if defined(TRANSPARENT_LUMINOSITY)
uniform MYFIXED _TransparencyLuminosity;
#endif
#if defined(TRANSPARENT_POW)
uniform MYFIXED _TransparencyPow;
#endif
#if defined(TRANSPARENT_SIMPLE)
uniform MYFIXED _TransparencySimple;
#endif

#if defined(MULTI_OCTAVES)
uniform MYFIXED _MultyOctaveNormals;
uniform MYFIXED _MultyOctavesSpeedOffset;
uniform MYFIXED2 _MultyOctavesTileOffset;
uniform MYFIXED _FadingFactor;

#if defined(MULTI_OCTAVES_ROTATE)
uniform MYFIXED _MOR_Base;
uniform MYFIXED _MOR_Offset;

#if defined(MULTI_OCTAVES_ROTATE_TILE)
uniform half _MOR_Tile;
#endif

#endif
#endif


uniform MYFIXED2 _AnimMove;
uniform MYFIXED _MainTexAngle;
uniform sampler2D _MainTex;
uniform MYFIXED4 _MainTex_ST;
#if !defined(SKIP_MAINTEXTURE) || defined(USE_NOISED_GLARE_PROCEDURALHQ) || !defined(SKIP_REFLECTION_MASK) || defined(HAS_REFR_MASK)
//uniform sampler2D _MainTex;
#endif
#if !defined(SKIP_MAINTEXTURE)
uniform MYFIXED4 _MainTexTile;
uniform MYFIXED _MAINTEXMASK_Blend;
uniform MYFIXED _MTDistortion;

#endif

uniform MYFIXED4 _WaterTextureTiling;

uniform sampler2D _Utility;

uniform MYFIXED _BumpAmount;
//uniform sampler2D_float _BumpMap;
uniform sampler2D _BumpMap;


#if defined(BAKED_DEPTH_EXTERNAL_TEXTURE)
uniform sampler2D _BakedData;// was _float #! fix
#else
uniform sampler2D _BakedData_temp; //was _float #! fix
#endif


#if !defined(SKIP_NOISED_GLARE_HQ) || defined(USE_NOISED_GLARE_ADDWAWES2) || defined(USE_NOISED_GLARE_ADDWAWES1) || defined(USE_NOISED_GLARE_LQ)  || defined(USE_NOISED_GLARE_PROCEDURALHQ)
//#define HAS_NOISED_GLARE = 1
//uniform sampler2D _NoiseHQ;
#endif


#if defined(REFRACTION_GRABPASS)
sampler2D _GrabTexture;
#endif
#if defined(HAS_REFRACTION)
uniform MYFIXED _RefrDistortion;

#if defined(USE_CAUSTIC) && !defined(ULTRA_FAST_MODE) && !defined(MINIMUM_MODE)
sampler2D _CAUSTIC_MAP;
uniform MYFIXED	_CAUSTIC_FOG_Amount;

uniform MYFIXED	_CAUSTIC_Speed;
uniform MYFIXED4	_CAUSTIC_Tiling;
uniform MYFIXED3	_CAUSTIC_Offset;
uniform MYFIXED	_CAUSTIC_FOG_Pow;
#if defined(C_BLUR)
uniform MYFIXED	_C_BLUR_R;
#endif
#if defined(C_ANIM)
uniform MYFIXED	_C_BLUR_S;
#endif

/*
uniform MYFIXED4	_CAUSTIC_PROC_Tiling;
uniform MYFIXED	_CAUSTIC_PROC_GlareSpeed;
uniform MYFIXED	_CAUSTIC_PROC_Contrast;
uniform MYFIXED	_CAUSTIC_PROC_BlackOffset;*/

#define HAS_NOISE_TEX = 1
#endif


#if defined(WAVES_GERSTNER)
#if !defined(HAS_NOISE_TEX)
#define HAS_NOISE_TEX = 1
#endif
#endif



#if defined(HAS_NOISE_TEX)
uniform sampler2D _NoiseHQ;
#endif



#if defined(USE_REFR_LOW_DISTOR)
uniform MYFIXED	_RefrLowDist;
#endif

uniform MYFIXED	_RefrTopAmount;
uniform MYFIXED	_RefrDeepAmount;
#if !defined(ULTRA_FAST_MODE) && !defined(MINIMUM_MODE)
uniform MYFIXED	_RefrAmount;

#if defined(DESATURATE_REFR)
uniform MYFIXED _RefractionDesaturate;
#endif
#endif

uniform MYFIXED	_TexRefrDistortFix;

uniform MYFIXED3	_RefrTopZColor;
uniform MYFIXED3	_RefrZColor;
uniform MYFIXED	_RefrRecover;
uniform MYFIXED	_RefrZOffset;
uniform MYFIXED _RefrZFallOff;

#if defined(REFRACTION_BLUR)
uniform MYFIXED _RefrBlur;
uniform MYFIXED _RefractionBlurZOffset;

#endif
 
#if defined(USE_REFRACTION_BLEND_FRESNEL) && defined(REFLECTION_NONE)
#endif
uniform MYFIXED _RefractionBLendAmount;

#endif

#if defined(USE_OUTPUT_GRADIENT)
uniform MYFIXED _OutGradBlend;
#endif
uniform MYFIXED4 _VertexSize;

#if !defined(SKIP_3DVERTEX_ANIMATION)

#if defined(HAS_WAVES_ROTATION)
uniform MYFIXED _WavesDirAngle;
#endif

uniform MYFIXED _VertexToUv;

uniform MYFIXED _3Dwaves_BORDER_FACE;
uniform MYFIXED2 _3DWavesSpeed;
uniform MYFIXED2 _3DWavesSpeedY;

uniform MYFIXED _3DWavesHeight;
uniform MYFIXED _3DWavesWind;
uniform half2 _3DWavesTile;
#if defined(WAW3D_NORMAL_CALCULATION)
uniform MYFIXED _3dwanamnt;
#endif
//uniform MYFIXED _3DWavesTileZ;
//uniform MYFIXED _3DWavesTileZAm;
#if  !defined(SKIP_3DVERTEX_ANIMATION) && !defined(SKIP_3DVERTEX_HEIGHT_COLORIZE) 
uniform MYFIXED _3DWavesYFoamAmount;
uniform MYFIXED _WaveGradTopOffset;

/*
uniform MYFIXED3 _WaveGrad0;
uniform MYFIXED3 _WaveGrad1;
uniform MYFIXED3 _WaveGrad2;
uniform MYFIXED _WaveGradMidOffset;*/

uniform MYFIXED _VERT_Amount;
uniform MYFIXED2 _VERT_Tile;
uniform MYFIXED3 _VERT_Color;

#endif


#if defined(HAS_WAVES_DETILE)
uniform MYFIXED _VERTEX_ANIM_DETILEAMOUNT;
uniform MYFIXED _VERTEX_ANIM_DETILESPEED;
uniform MYFIXED _VERTEX_ANIM_DETILEFRIQ;
uniform MYFIXED3 _VERTEX_ANIM_DETILE_YOFFSET;
#endif

#endif

#if defined(USE_LIGHTMAPS)
uniform MYFIXED4 unity_Lightmap_ST;
#endif

#if defined(USE_OUTPUT_SHADOWS)
uniform MYFIXED _OutShadowsAmount;
#endif





#if defined(POSTRIZE)
uniform MYFIXED POSTRIZE_Colors;
#endif

#if defined(RIM)
uniform MYFIXED RIM_Minus;
uniform MYFIXED RIM_Plus;
uniform sampler2D _RimGradient;
uniform MYFIXED _RIM_BLEND;

#endif
/*
#if defined(HAS_NOISED_GLARE)
#if !defined(SKIP_NOISED_GLARE_HQ)
uniform MYFIXED _NHQ_GlareAmount;
uniform MYFIXED _NHQ_GlareFriq;
uniform MYFIXED _NHQ_GlareSpeedXY;
uniform MYFIXED _NHQ_GlareSpeedZ;
uniform MYFIXED _NHQ_GlareContrast;
uniform MYFIXED _NHQ_GlareBlackOffset;
#if !defined(SKIP_NOISED_GLARE_HQ_NORMALEFFECT)
uniform MYFIXED _NHQ_GlareNormalsEffect;
#endif
#endif
#if defined(USE_NOISED_GLARE_LQ)
uniform MYFIXED _NE1_GlareAmount;
uniform MYFIXED _NE1_GlareFriq;
uniform MYFIXED _NE1_GlareSpeed;
uniform MYFIXED _NE1_GlareContrast;
uniform MYFIXED _NE1_GlareBlackOffset;
uniform MYFIXED3 _NE1_WavesDirection;
#if !defined(NOISED_GLARE_LQ_SKIPOWNTEXTURE)
uniform sampler2D _NoiseLQ;
#endif
#endif
#if defined(USE_NOISED_GLARE_ADDWAWES1)
uniform MYFIXED _W1_GlareAmount;
uniform MYFIXED _W1_GlareFriq;
uniform MYFIXED _W1_GlareSpeed;
uniform MYFIXED _W1_GlareContrast;
uniform MYFIXED _W1_GlareBlackOffset;
#endif
#if defined(USE_NOISED_GLARE_ADDWAWES2)
uniform MYFIXED _W2_GlareAmount;
uniform MYFIXED _W2_GlareFriq;
uniform MYFIXED _W2_GlareSpeed;
uniform MYFIXED _W2_GlareContrast;
uniform MYFIXED _W2_GlareBlackOffset;
#endif
#if defined(USE_NOISED_GLARE_PROCEDURALHQ)
uniform MYFIXED _PRCHQ_amount;
uniform half _PRCHQ_tileTex;
uniform half _PRCHQ_tileWaves;
uniform MYFIXED _PRCHQ_speedTex;
uniform MYFIXED _PRCHQ_speedWaves;
#endif
#endif*/

//REFLECTION


#if !defined(REFLECTION_NONE)
#if defined(REFLECTION_USER)
UNITY_DECLARE_TEXCUBE(_ReflectionUserCUBE);
#endif//defined(REFLECTION_USER)

#if defined(REFLECTION_2D)
uniform sampler2D _ReflectionTex;
#endif//defined(REFLECTION_2D)
#if defined(REFLECTION_PLANAR)
 sampler2D _ReflectionTex_temp;
#endif//defined(REFLECTION_2D)



//uniform MYFIXED3 _ReflectColor;
uniform MYFIXED baked_ReflectionTex_distortion;
uniform MYFIXED _ReflectionAmount;
uniform MYFIXED _ReflectionBlendAmount;
uniform MYFIXED LOW_ReflectionTex_distortion;

#if defined(COLORIZE_REFLECTION)
uniform MYFIXED3 _ReflectColor;
#endif


uniform MYFIXED _ReflectionYOffset;
uniform MYFIXED _ReflectionLOD;
#if defined(DESATURATE_REFL)
uniform MYFIXED _ReflectionDesaturate;
#endif
#if defined(REFLECTION_BLUR) || !defined(REFLECTION_2D) && !defined(REFLECTION_PLANAR)
uniform MYFIXED  _ReflectionBlurRadius;
uniform MYFIXED  _ReflectionBlurZOffset;
#if !defined(SKIP_FRES_BLUR)
uniform MYFIXED FRES_BLUR_AMOUNT;
uniform MYFIXED FRES_BLUR_OFF;

#endif
#endif 
#if defined(REFLECTION_JUST_COLOR)
uniform MYFIXED3 _ReflectionJustColor;
#endif
#endif//!defined(REFLECTION_NONE)
//REFLECTION


#if !defined(SKIP_FRESNEL_CALCULATION)
uniform MYFIXED _FresnelFade;
uniform MYFIXED _FresnelAmount;
uniform MYFIXED _FresnelPow;
#endif
#if !defined(SKIP_FRESNEL_CALCULATION) && defined(USE_FRESNEL_POW)
#endif

//LIGHTING
#if !defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR) || !defined(SKIP_FLAT_SPECULAR)
uniform MYFIXED3 _LightDir;
//uniform MYFIXED3 _LightPos;
#endif//!defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR)
#if !defined(SKIP_LIGHTING)
uniform MYFIXED _LightAmount;
#endif//!defined(SKIP_LIGHTING)
#if !defined(SKIP_SPECULAR)
uniform MYFIXED _SpecularAmount;
uniform MYFIXED _SpecularShininess;
uniform MYFIXED _SpecularGlowAmount;
#endif//!defined(SKIP_SPECULAR)
#if !defined(SKIP_FLAT_SPECULAR)
uniform MYFIXED _FlatSpecularAmount;
uniform MYFIXED _FlatSpecularShininess;
uniform MYFIXED _FlatSpecularClamp;
uniform MYFIXED _FlatFriqX;
uniform MYFIXED _FlatFriqY;
uniform MYFIXED _Light_FlatSpecTopDir;

#if defined(USE_FLAT_HQ)
uniform MYFIXED FLAT_HQ_OFFSET;

#endif//!defined(SKIP_SPECULAR)

#endif//!defined(SKIP_SPECULAR)
//LIGHTING


#if !defined(SKIP_REFLECTION_MASK)
uniform MYFIXED _ReflectionMask_Amount; //3
uniform MYFIXED _ReflectionMask_Offset; //0.2
uniform MYFIXED _ReflectionMask_UpClamp; //5
uniform MYFIXED _ReflectionMask_Tiling; //0.1
uniform MYFIXED2 _ReflectionMask_TexOffsetF;
#endif


#if (defined(HAS_FOAM) || defined(SHORE_WAVES)) && !defined(ULTRA_FAST_MODE) && !defined(MINIMUM_MODE)

#if defined(FOAM_BLEND_LUM)
uniform MYFIXED _FoamBlendOffset;
#endif

#if defined(NEED_FOAM_UNPACK) 
uniform sampler2D _FoamTexture;
uniform MYFIXED _FoamTextureTiling;
#endif
#if  defined(NEED_SHORE_WAVES_UNPACK)
uniform sampler2D _FoamTexture_SW;
uniform MYFIXED _FoamTextureTiling_SW;
#endif

#if defined(FOAM_COAST_ALPHA_V2)
uniform MYFIXED _FoamAlpha2Amount;
#endif
uniform MYFIXED3 _FoamColor;
#endif//



#if !defined(SKIP_FOAM)
uniform MYFIXED _FoamAmount;
uniform MYFIXED _FoamLength;
uniform MYFIXED _WaterfrontFade;
uniform MYFIXED _FoamWavesSpeed;
uniform MYFIXED _FoamDistortion;
uniform MYFIXED _FoamDistortionFade;
uniform MYFIXED _FoamDirection;

uniform MYFIXED _FoamOffset;
uniform MYFIXED _FoamOffsetSpeed;

#if defined(NEED_FOAM_UNPACK)
uniform MYFIXED _FoamDistortionTexture;
#endif

#endif//!defined(SKIP_FOAM)

#if !defined(SKIP_SURFACE_FOAMS) && defined(ADVANCE_PC)
uniform MYFIXED3 _SUrfaceFoamVector;
uniform MYFIXED _SurfaceFoamContrast;
uniform MYFIXED _SurfaceFoamAmount;
#endif

#ifdef ORTO_CAMERA_ON
uniform MYFIXED _MyNearClipPlane;
uniform MYFIXED _MyFarClipPlane;
#endif


#if defined(DETILE_HQ) || defined(DETILE_LQ)
uniform half _DetileAmount;
uniform half _DetileFriq;
#endif

#if defined(HAS_REFRACTION) && !defined(REFRACTION_ONLYZCOLOR)
uniform MYFIXED _RefrTextureFog;
#endif


#if(defined(APPLY_REFR_TO_SPECULAR) || defined(_APPLY_REFR_TO_TEX_DISSOLVE_FAST) )&& defined(HAS_REFRACTION)
uniform MYFIXED _APPLY_REFR_TO_SPECULAR_DISSOLVE;
uniform MYFIXED _APPLY_REFR_TO_SPECULAR_DISSOLVE_FAST;

#endif





#if defined(SHORE_WAVES)
uniform MYFIXED _FoamMaskOffset_SW;
uniform MYFIXED _FoamLength_SW;
uniform MYFIXED _FoamDistortionFade_SW;
uniform MYFIXED _WaterfrontFade_SW;
uniform MYFIXED _FoamDistortion_SW;
uniform MYFIXED3 _FoamColor_SW;
uniform MYFIXED _FoamWavesSpeed_SW;
uniform MYFIXED _FoamDirection_SW;
uniform MYFIXED _FoamAmount_SW;
uniform half _FoamLShoreWavesTileY_SW;
uniform half _FoamLShoreWavesTileX_SW;


#if defined(NEED_SHORE_WAVES_UNPACK)
uniform MYFIXED _ShoreDistortionTexture;
#endif
#endif


#if defined(USE_SURFACE_GRADS)

#endif

#if defined(WAVES_GERSTNER)&& defined(ADVANCE_PC)
/*uniform MYFIXED4 _GAmplitude;
uniform MYFIXED4 _GFrequency;
uniform MYFIXED4 _GSteepness;
uniform MYFIXED4 _GSpeed;
uniform MYFIXED4 _GDirectionAB;
uniform MYFIXED4 _GDirectionCD;*/

uniform MYFIXED _CN_DISTANCE;
uniform MYFIXED _CN_TEXEL;
uniform MYFIXED _CLASNOISE_PW;
uniform MYFIXED _CN_AMOUNT;
uniform MYFIXED2 _CN_TILING;
uniform MYFIXED _CN_SPEED;

/*uniform MYFIXED2 _GAmplitude;
uniform MYFIXED2 _GFrequency;
uniform MYFIXED2 _GSteepness;
uniform MYFIXED2 _GSpeed;
uniform MYFIXED2 _GDirectionAB;*/
#endif

uniform MYFIXED3 _ObjectScale;

#if defined(Z_AFFECT_BUMP) && !defined(DEPTH_NONE)
#define HAS_Z_AFFECT_BUMP = 1
uniform MYFIXED _BumpZOffset;
uniform MYFIXED _BumpZFade;
#endif



#if defined(USE_LUT)

//TEXTURE2D_SAMPLER2D(_Lut2D, sampler_Lut2D);
//UNITY_DECLARE_TEX2D(_Lut2D);
uniform sampler2D _Lut2D;
uniform half4 _Lut2D_params;
uniform MYFIXED _LutAmount;

#endif




#if  defined (ADDITIONAL_FOAM_DISTORTION) && defined(HAS_REFRACTION) &&  !defined(SKIP_FOAM)
uniform MYFIXED ADDITIONAL_FOAM_DISTORTION_AMOUNT;
#endif


#if defined(ZDEPTH_CALCANGLE)
uniform MYFIXED _ZDistanceCalcOffset;
#endif



#if defined(SURFACE_WAVES)&& defined(ADVANCE_PC)
uniform half4 _SFW_Tile;
uniform MYFIXED2 _SFW_Speed;
uniform MYFIXED _SFW_Amount;
uniform MYFIXED3 _SFW_Dir;
uniform MYFIXED3 _SFW_Dir1;
uniform MYFIXED _SFW_Distort;

#if !defined(SKIP_SURFACE_WAVES_NORMALEFFECT)
uniform MYFIXED _SFW_NrmAmount;
#endif

#endif


#if !defined(SKIP_BLEND_ANIMATION)
uniform MYFIXED _BlendAnimSpeed;

#endif


#if defined(USE_FAST_FRESNEL)
uniform MYFIXED _FastFresnelAmount;
uniform MYFIXED _FastFresnelPow;
#endif



#if defined(SKIP_MAIN_TEXTURE)
//#if !defined(USE_STATIC_MAINTEX) && !defined(USE_CROSSANIMATED_MAINTEX) && !defined(USE_BLENDANIMATED_MAINTEX)
#define NO_MAINTTEX = 1
#else

uniform MYFIXED MAIN_TEX_Bright;
uniform half2 MAIN_TEX_Tile;
uniform MYFIXED2 MAIN_TEX_CA_Speed;
uniform MYFIXED2 MAIN_TEX_Move;
uniform MYFIXED MAIN_TEX_BA_Speed;
uniform MYFIXED MAIN_TEX_Amount;
uniform MYFIXED MAIN_TEX_Contrast;
uniform MYFIXED MAIN_TEX_Distortion;
uniform MYFIXED MAIN_TEX_LQDistortion;

uniform MYFIXED MAIN_TEX_ADDDISTORTION_THAN_MOVE_Amount;

#if !defined(SKIP_MAINTEX_VHEIGHT)
uniform MYFIXED MAINTEX_VHEIGHT_Amount;
uniform MYFIXED MAINTEX_VHEIGHT_Offset;
#endif


#if defined(USE_BLENDANIMATED_MAINTEX)
uniform MYFIXED _LOW_DISTOR_MAINTEX_Tile;
uniform MYFIXED _LOW_DISTOR_MAINTEX_Speed;
uniform MYFIXED MAIN_TEX_MixOffset;
uniform MYFIXED MAIN_TEX_Multy;

#endif

#endif

#if defined(POST_TEXTURE_TINT)
uniform MYFIXED3 _MM_Color;
uniform MYFIXED2 _MM_offset;
uniform MYFIXED2 _MM_Tile;
uniform MYFIXED _MM_MultyOffset;

#if defined(POST_OWN_TEXTURE)
uniform sampler2D _MM_Texture;
#endif
#endif


//#if defined(NO_MAINTTEX)
uniform MYFIXED3 _ReplaceColor;
//#endif

 
#if defined(UFAST_SHORE_1)

#if defined(USE_VERTEX_H_DISTORT)
uniform MYFIXED VERTEX_H_DISTORT;
#endif


#if defined(SHORE_USE_WAVES_GRADIENT_1)
uniform MYFIXED MAIN_TEX_FoamGradWavesSpeed_1;
uniform MYFIXED MAIN_TEX_FoamGradDirection_1;
uniform MYFIXED MAIN_TEX_FoamGradTile_1;
uniform MYFIXED MAIN_TEX_FoamGradTileYYY_1;
#endif
#if defined(SHORE_USE_ADDITIONALCONTUR_1)
uniform MYFIXED3 _UFSHORE_ADD_Color_1;
uniform MYFIXED _UFSHORE_ADDITIONAL_Length_1;
uniform MYFIXED _UFSHORE_ADD_Amount_1;
uniform MYFIXED _UFSHORE_ADD_Tile_1;
uniform MYFIXED _UFSHORE_ADD_Distortion_1;
uniform MYFIXED _UFSHORE_ADD_LowDistortion_1;

#endif
#if defined(SHORE_USE_LOW_DISTORTION_1)
uniform MYFIXED _UFSHORE_LowDistortion_1;
#endif
uniform MYFIXED SHORE_USE_ADDITIONALCONTUR_POW_Amount_1;
uniform MYFIXED _UFSHORE_UNWRAP_LowDistortion_1;
uniform MYFIXED _UFSHORE_UNWRAP_Transparency;

uniform MYFIXED _UFSHORE_Amount_1;
uniform MYFIXED _UFSHORE_Length_1;
uniform MYFIXED3 _UFSHORE_Color_1;
uniform MYFIXED _UFSHORE_Distortion_1;
uniform half2 _UFSHORE_Tile_1;
uniform MYFIXED _UFSHORE_AlphaAmount_1;
uniform MYFIXED _UFSHORE_AlphaMax_1;
uniform float _UFSHORE_Speed_1;
#if defined(SHORE_SHADDOW_1)
uniform MYFIXED _UFSHORE_ShadowV1_1;
uniform MYFIXED _UFSHORE_ShadowV2_1;
#endif
#endif

#if defined(USE_lerped_post)
uniform MYFIXED3 lerped_post_color1;
uniform MYFIXED3 lerped_post_color2;
uniform MYFIXED lerped_post_offset;
uniform MYFIXED lerped_post_offset_falloff;
#endif



/*
#if defined(UFAST_SHORE_2)
#if defined(SHORE_USE_WAVES_GRADIENT_2)

uniform MYFIXED MAIN_TEX_FoamGradWavesSpeed_2;
uniform MYFIXED MAIN_TEX_FoamGradDirection_2;
uniform MYFIXED MAIN_TEX_FoamGradTile_2;
#endif
#if defined(SHORE_USE_ADDITIONALCONTUR_2)

uniform MYFIXED3 _UFSHORE_ADD_Color_2;
uniform MYFIXED _UFSHORE_ADDITIONAL_Length_2;
uniform MYFIXED _UFSHORE_ADD_Amount_2;
uniform MYFIXED _UFSHORE_ADD_Tile_2;
uniform MYFIXED _UFSHORE_ADD_Distortion_2;
uniform MYFIXED _UFSHORE_ADD_LowDistortion_2;
#endif
#if defined(SHORE_USE_LOW_DISTORTION_2)
uniform MYFIXED _UFSHORE_LowDistortion_2;
#endif

uniform MYFIXED SHORE_USE_ADDITIONALCONTUR_POW_Amount_2;
uniform MYFIXED _UFSHORE_Amount_2;
uniform MYFIXED _UFSHORE_Length_2;
uniform MYFIXED3 _UFSHORE_Color_2;
uniform MYFIXED _UFSHORE_Distortion_2;
uniform half2 _UFSHORE_Tile_2;
uniform MYFIXED _UFSHORE_AlphaAmount_2;
uniform MYFIXED _UFSHORE_AlphaMax_2;
uniform float _UFSHORE_Speed_2;
#if defined(SHORE_SHADDOW_2)
uniform MYFIXED _UFSHORE_ShadowV1_2;
uniform MYFIXED _UFSHORE_ShadowV2_2;
#endif
#endif*/

