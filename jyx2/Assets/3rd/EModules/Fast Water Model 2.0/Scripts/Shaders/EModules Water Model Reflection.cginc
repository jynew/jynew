

#if defined(HAS_REFLECTION)

fixed3 reflDist = worldNormal;
reflDist.xz *= baked_ReflectionTex_distortion;
#if defined(REFL_DIST_DIST)
//fixed rdd = i.screen.w / 30;
//reflDist.xz *= rdd;
#endif
reflDist.xz += i.vfoam.xy * LOW_ReflectionTex_distortion;

#if defined(REFLECTION_JUST_COLOR)
 reflectionColor = _ReflectionJustColor;

#elif defined(REFLECTION_2D) || defined(REFLECTION_PLANAR)

MYFIXED4 uv1;
//uv1.x = (tnormal.x) * 10 * reflDist.xz;
//uv1.y = -abs(tnormal.y) * 10 * reflDist.xz;
uv1.xy = reflDist.xz;
uv1.zw = 0;
#ifdef ORTO_CAMERA_ON
uv1.xy /= ORTO_PROJ_DIVIDER;
#endif
uv1 += i.screen;

#ifdef ORTO_CAMERA_ON
#else
uv1 = UNITY_PROJ_COORD(uv1);
#endif
#if defined(REFLECTION_2D)
#ifdef ORTO_CAMERA_ON
#define REFLTEX(uv) tex2D(_ReflectionTex, (uv)).rgb
#else
#define REFLTEX(uv) tex2Dproj(_ReflectionTex, (uv)).rgb
#endif
//sampler2D targetTexture = _ReflectionTex;
#else
//return tex2Dproj(_ReflectionTex_temp, UNITY_PROJ_COORD(uv1));
#ifdef ORTO_CAMERA_ON
#define REFLTEX(uv) tex2D(_ReflectionTex_temp, (uv)).rgb
#else
#define REFLTEX(uv) tex2Dproj(_ReflectionTex_temp, (uv)).rgb
#endif
		//sampler2D targetTexture = _ReflectionTex_temp;
#endif

#if defined(REFLECTION_BLUR)
uv1.x -= _ReflectionBlurRadius;
reflectionColor =  REFLTEX(uv1);
uv1.x += _ReflectionBlurRadius*2;
reflectionColor = (reflectionColor + REFLTEX(uv1))/2;
#else
reflectionColor = REFLTEX(uv1);
#endif

#else

	
			fixed3 worldRefl = reflect(-wViewDir, reflDist * (1 + _ReflectionYOffset));

	#if  !defined(REFLECTION_BLUR)
			fixed _REFL_BLUR = 0;
	#else
			fixed _REFL_BLUR = _ReflectionBlurRadius;
	#endif

	#if defined(REFLECTION_USER)
			 reflectionColor = UNITY_SAMPLE_TEXCUBE_LOD(_ReflectionUserCUBE, worldRefl, _REFL_BLUR);
	#endif
	#if defined(REFLECTION_PROBE)
			 reflectionColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, worldRefl, _REFL_BLUR);
	#endif
	#if defined(REFLECTION_PROBE_AND_INTENSITY)
			 reflectionColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, worldRefl, _REFL_BLUR) * unity_SpecCube0_HDR.x;
	#endif
#if defined(REFLECTION_PROBE_AND_INTENSITY)
			 reflectionColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, worldRefl, _REFL_BLUR) * unity_SpecCube0_HDR.x;
#endif



#endif
				
#if defined(REFLECTION_DEBUG_RGB) && !defined(REFLECTION_NONE)
			return float4(reflectionColor.rgb, 1);
#endif
			reflectionColor.rgb *= _ReflectionAmount;



#if defined(USE_FAST_FRESNEL) && defined(HAS_REFLECTION)
			fixed wd = 1 - wViewDir.y;
			//wd = wd * wd *wd;
			wd = pow((wd* 0.5 + 0.5), _FastFresnelPow);
			 rd = saturate((wd - wd * abs(reflDist.x + reflDist.z)*_FastFresnelAmount) );
#elif defined(USE_LERP_BLEND)
			//fixed rd = saturate(1 - wViewDir.y);
			 rd = (1 - wViewDir.y);
#else
//saturate((1 - wViewDir.y ) ) * 
			//fixed rd = saturate(reflectionColor.r * 2);
			 rd = reflectionColor.r;
#endif

#if defined(COLORIZE_REFLECTION)
			fixed3 targetRefl = reflectionColor * _ReflectColor ;
			rd *= (1 - _ReflectionBlendAmount);
#else
			//fixed3 targetRefl = color.rgb * _ReflectionBlendAmount + reflectionColor;
			fixed3 targetRefl = reflectionColor;
			rd *= (1 - _ReflectionBlendAmount);
#endif
			//color.rgb = color.rgb * _ReflectionBlendAmount + reflectionColor* rd;

#endif
