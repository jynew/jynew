

#if defined(HAS_REFRACTION)

		fixed ref_zdepth = zdepth;

		 lerped_refr = saturate((ref_zdepth - _RefrZOffset) / _RefrZFallOff
#ifdef ORTO_CAMERA_ON//////////////////////////////////////
			* (dot(wViewDir, fixed3(0, 1, 0)) * 5 + 1)
#endif
			+ _RefrZOffset);



#if defined(USE_REFR_DISTOR)
		//fixed RefrDist = _RefrDistortion * (lerped_refr - _TexRefrDistortFix);
		fixed RefrDist = _RefrDistortion;
#if defined(HAS_REFR_Z_AFFECT_BUMP)
		RefrDist *= lerped_refr - _TexRefrDistortFix;
#endif
		//RefrDist *= saturate(lerped_refr - _TexRefrDistortFix);
#endif

#if defined(HAS_BAKED_REFRACTION)

		//pp.xy += inputNormal.xy ;


		fixed2 refrUv = i.uv.zw ;


#if defined(USE_REFR_DISTOR)
#ifdef ORTO_CAMERA_ON
		RefrDist = RefrDist / ORTO_PROJ_DIVIDER;
#else
		RefrDist = RefrDist;
#endif
		refrUv += tnormal_TEXEL * RefrDist;

#endif
#if defined(USE_REFR_LOW_DISTOR)
		refrUv += i.vfoam.xy * _RefrLowDist;
#endif


#if !defined(DEPTH_NONE) && !defined(SKIP_REFRACTION_CALC_DEPTH_FACTOR)
#if !defined (HAS_DEPTH_FACTOR)
		/*fixed3 pp = (i.VER_TEX.xyz);
		fixed preRes = saturate(((ref_zdepth) / _RefrDeepFactor)) / (pp.z * 0.8 + 0.2);
		fixed2 DFACTOR = pp.xy * preRes;*/
		fixed preRes = saturate(ref_zdepth / _RefrDeepFactor);
		 DFACTOR = i.VER_TEX.xy * preRes;
		
#endif
		refrUv -= DFACTOR;
#endif


#if defined(REFRACTION_BAKED_FROM_TEXTURE) 
#define REFRTEX(uva) TEX2DGRAD(_RefractionTex, (uva)).rgb
#else
#define REFRTEX(uva) TEX2DGRAD(_RefractionTex_temp, (uva)).rgb
#endif

		 refractionColor = REFRTEX(refrUv);

#if defined(REFRACTION_BLUR) && !defined(MINIMUM_MODE)

#define REFR_BLUR4(inrefrUv, inRefrBlur)\
			REFR_BLUR4_result = REFRTEX(inrefrUv + fixed2(0, inRefrBlur));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + fixed2(inRefrBlur, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + fixed2(0, -inRefrBlur));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + fixed2(-inRefrBlur, 0));\
			REFR_BLUR4_result /= 4

		fixed _REFR_BLUR = _RefrBlur / 100;

#if !defined(SKIP_REFRACTION_BLUR_ZDEPENDS) && (defined(HAS_CAMERA_DEPTH) || defined(HAS_BAKED_DEPTH))
		_REFR_BLUR *= saturate(lerped_refr + _RefractionBlurZOffset);
#endif
		fixed3 REFR_BLUR4_result;
		 
		REFR_BLUR4(refrUv, _REFR_BLUR);
		fixed3 blured2 = REFR_BLUR4_result;
		refractionColor = blured2;
#endif

#define APPLY_REF = 1;
#elif defined(REFRACTION_GRABPASS)


		fixed4 refrUv = i.grabPos;

#if defined(USE_REFR_DISTOR)
#ifdef ORTO_CAMERA_ON
		RefrDist /= ORTO_PROJ_DIVIDER;
#endif
		refrUv.xy += (tnormal_GRAB)* RefrDist;
#endif
#define REFRTEX(uv) tex2Dproj(_GrabTexture, (uv)).rgb

		 refractionColor = REFRTEX( refrUv);

#if defined(REFRACTION_BLUR) && !defined(MINIMUM_MODE)
		fixed _REFR_BLUR = _RefrBlur;


#define REFR_BLUR4_PROJ(inrefrUv, inReflBlur)\
			REFR_BLUR4_result = REFRTEX(inrefrUv + fixed4(0, inReflBlur, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + fixed4(inReflBlur, 0, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + fixed4(0, -inReflBlur, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + fixed4(-inReflBlur, 0, 0, 0));\
			REFR_BLUR4_result /= 4

#ifdef ORTO_CAMERA_ON
		_REFR_BLUR /= ORTO_PROJ_DIVIDER;
#endif

		fixed3 REFR_BLUR4_result;

		REFR_BLUR4_PROJ(refrUv, _REFR_BLUR);
		fixed3 blured = REFR_BLUR4_result;
		refractionColor = blured;
#endif

#define APPLY_REF = 1;

#else
#define USE_FOG = 1;
#define NO_REFR_TEX = 1;

#endif


#if defined(USE_DEPTH_FOG) || defined(USE_FOG)
		fixed ivll1 = 1 - lerped_refr;
		fixed3 topColor = ivll1 * _RefrTopZColor;
		fixed3 deepColor = lerped_refr * _RefrZColor;
		unionFog = topColor + deepColor;
#if defined(USE_FOG)
		//refractionColor *= unionFog;
		 refractionColor = unionFog;
		#define APPLY_REF = 1;
#else
#define APPLY_REF_FOG = 1;
#endif
#endif


#if defined(REFRACTION_DEBUG)
		return float4(lerped_refr.rrr, 1);
#endif



		

#if (defined(APPLY_REFR_TO_SPECULAR)||defined(_APPLY_REFR_TO_TEX_DISSOLVE_FAST)) && defined(HAS_REFRACTION)
			//FIXEDAngleVector *= saturate(1 - lerped_refr);
		 APS = 1 - (1 - lerped_refr * lerped_refr) * _APPLY_REFR_TO_SPECULAR_DISSOLVE_FAST;
#endif



#endif