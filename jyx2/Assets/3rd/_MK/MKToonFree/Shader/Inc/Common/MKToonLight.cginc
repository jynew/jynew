//Light Calculations
#ifndef MK_TOON_LIGHT
	#define MK_TOON_LIGHT

	////////////
	// LIGHT
	////////////
	void MKToonLightMain(inout MKToonSurface mkts, in VertexOutputForward o)
	{
		//Base light calculation
		fixed baseLightCalc;

		baseLightCalc = mkts.Pcp.NdotL;
		mkts.Color_Diffuse = TreshHoldLighting(_LightThreshold, _LightSmoothness, baseLightCalc);
		mkts.Color_Diffuse *= mkts.Pcp.LightAttenuation;

		//Custom shadow
		#ifdef USING_DIRECTIONAL_LIGHT
			_ShadowColor.rgb = lerp(_HighlightColor * mkts.Pcp.LightColor, _ShadowColor, _ShadowIntensity);
			mkts.Color_Diffuse = lerp(_ShadowColor, _HighlightColor * mkts.Pcp.LightColor, mkts.Color_Diffuse);
		#else
			_ShadowColor.rgb = 0;
			//_ShadowColor.rgb = lerp(_HighlightColor * mkts.Pcp.LightColor, _ShadowColor, _ShadowIntensity);
			mkts.Color_Diffuse = lerp(_ShadowColor, _HighlightColor * mkts.Pcp.LightColor, mkts.Color_Diffuse);
		#endif

		fixed4 c;
		//Diffuse light
		c.rgb = mkts.Color_Albedo * mkts.Color_Diffuse;

		//Specular
		half spec;
		_Shininess *= mkts.Color_Specular.g;
		spec = GetSpecular(mkts.Pcp.NdotHV, _Shininess, mkts.Pcp.NdotL);
		mkts.Color_Specular = TreshHoldLighting(_LightThreshold, _LightSmoothness, spec);
		mkts.Color_Specular = mkts.Color_Specular * _SpecColor * (_SpecularIntensity *  mkts.Color_Specular.r);

		//apply specular
		c.rgb += mkts.Color_Specular * mkts.Pcp.LightColorXAttenuation;

		//apply alpha
		c.a = mkts.Alpha;

		mkts.Color_Out = c;
	}

	void MKToonLightLMCombined(inout MKToonSurface mkts, in VertexOutputForward o)
	{
		//apply lighting to surface
		MKToonLightMain(mkts, o);

		#ifdef MK_TOON_FWD_BASE_PASS
			//add ambient light
			fixed3 amb = mkts.Color_Albedo * o.aLight;
			mkts.Color_Out.rgb += amb;
		#endif

		#ifdef MK_TOON_FWD_BASE_PASS
			#if LIGHTMAP_ON || DYNAMICLIGHTMAP_ON
				half3 lm = 0;
				#ifdef LIGHTMAP_ON
						fixed4 lmBCT = UNITY_SAMPLE_TEX2D(unity_Lightmap, o.uv_Lm.xy);
						fixed3 bC = DecodeLightmap(lmBCT);
						//handle directional lightmaps
					#if DIRLIGHTMAP_COMBINED
						// directional lightmaps
						fixed4 bDT = UNITY_SAMPLE_TEX2D_SAMPLER (unity_LightmapInd, unity_Lightmap, o.uv_Lm.xy);
						lm = DecodeDirectionalLightmap (bC, bDT, o.normalWorld);

						#if defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN)
							lm = SubtractMainLightWithRealtimeAttenuationFromLightmap (lm, mkts.Pcp.LightAttenuation, lmBCT, o.normalWorld);
						#endif
					//handle not directional lightmaps
					#else
						lm = bC;
						#if defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN)
							lm = SubtractMainLightWithRealtimeAttenuationFromLightmap(lm, mkts.Pcp.LightAttenuation, lmBCT, o.normalWorld);
						#endif
					#endif
				#endif

				//handle dynamic lightmaps
				#ifdef DYNAMICLIGHTMAP_ON
					fixed4 lmRTCT = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, o.uv_Lm.zw);
					half3 rTC = DecodeRealtimeLightmap (lmRTCT);

					#ifdef DIRLIGHTMAP_COMBINED
						half4 rDT = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, o.uv_Lm.zw);
						lm += DecodeDirectionalLightmap (rTC, rDT, o.normalWorld);
					#else
						lm += rTC;
					#endif
				#endif

				//apply lightmap
				mkts.Color_Out.rgb += mkts.Color_Albedo * lm;
			#endif
		#endif
	}
#endif