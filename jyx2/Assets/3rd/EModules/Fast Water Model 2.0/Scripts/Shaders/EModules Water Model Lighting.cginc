#if !defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR) ||  !defined(SKIP_FLAT_SPECULAR)

#if defined(USE_LIGHTIN_NORMALS) 
			

#if defined(ULTRA_FAST_MODE) || defined(MINIMUM_MODE)
		fixed3 LWN = worldNormal;
		LWN.xz += worldNormal.xz * _LightNormalsFactor;
		LWN = normalize(LWN);
#else 
		fixed3 LWN = worldNormal;
		fixed3 LDIR = LWN * abs(fixed3(tspace.y,0, tspace.x));
		LWN += LDIR * (_LightNormalsFactor);
		LWN = normalize(LWN);
#endif
#else 
		fixed3 LWN = worldNormal;
#endif

		fixed UP_SUN = i.helpers.w;

#if !defined(SKIP_LIGHTING) && !defined(MINIMUM_MODE)
		fixed nl = (dot(LWN, -_LightDir)) * UP_SUN;
#if !defined(USE_FAKE_LIGHTING)
		fixed3 NL_MULT = _LightColor0;
#else
		fixed3 NL_MULT = _LightColor0Fake;
#endif	
#if !defined(SKIP_LIGHTING_POW)
		fixed lv15 = pow(nl, 8) * 200;
#else
		fixed lv15 = nl/3;
#endif
#if defined(FORCE_OPAQUE) && (defined (SHADER_API_GLES) || defined (SHADER_API_GLES3))
		lv15 /= 2;
#endif
		fixed3 lightLight = (lv15 * _LightAmount   * NL_MULT);

#endif

#if !defined(SKIP_SPECULAR) || !defined(SKIP_FLAT_SPECULAR)
		
		//fixed3 fixedAngleVector = i.valpha.yzw;
		fixed3 av = wViewDir - _LightDir;
#if !defined(SKIP_SPECULAR_ANIZO)
		av.z /= 2;
#endif
		fixed3 fixedAngleVector = normalize(av);
			//fixed3 fixedAngleVector = normalize(-(_LightDir)+(WVIEW));

#if !defined(SKIP_SPECULAR)
		fixed SPEC_DOT = (dot(fixedAngleVector, LWN)+1)*0.5;
		//if (SPEC_DOT > 0.95)
		//{
			specularLight += pow(SPEC_DOT, _SpecularShininess * 30) * _SpecularAmount;
#if !defined(SKIP_SPECULAR_GLOW)
			specularLight += pow(SPEC_DOT, _SpecularShininess * 0.8333) * _SpecularAmount * _SpecularGlowAmount;
#endif
			specularLight *= UP_SUN;
#endif
		//}
		

#if !defined(SKIP_FLAT_SPECULAR)
			fixed3 flat_tnormal = (tnormal);
			//fixed DD = saturate((dot(flat_tnormal, i.vfoam.w) *0.5 + 0.5) * _FlatSpecularAmount);
			fixed DD = saturate((dot(flat_tnormal, i.vfoam.w) ) * _FlatSpecularAmount);


			//fixed3 flat_tnormal = worldNormal;
		/*	fixed3 flat_tnormal = (tnormal);
		fixed DD = saturate(dot(flat_tnormal, (fixed3(0.1, 0.98, 0.05))) * _FlatSpecularAmount);*/
		fixed flat_result = ((pow(DD, _FlatSpecularShininess))) * _FlatSpecularAmount;

#if !defined(SKIP_FLAT_SPECULAR_CLAMP)
		fixed3 CCC = LWN;
		fixed FLAT_NL = pow((dot(fixedAngleVector, CCC)), _FlatSpecularClamp);
		flat_result *= FLAT_NL;
#endif
		specularLight += flat_result;
#endif

#if defined(APPLY_REFR_TO_SPECULAR) && defined(HAS_REFRACTION)
		//FIXEDAngleVector *= saturate(1 - lerped_refr);
		//#if defined(FOAM_ALPHA_NEED)
		//return  1 - refrv06;
	//	specularLight *= 1 - refrv06;
//#else
		specularLight *= APS;

	///	specularLight *= 1 - refrv06;
	//	specularLight *= 1- refrv06;
#if !defined(SKIP_LIGHTING) && !defined(MINIMUM_MODE)
		lightLight *= APS;
#endif
		//#endif
#endif

#endif


		
#endif