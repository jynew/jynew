
#if defined(UFAST_SHORE_1)


//+ (sin(i.uv.x*50 + _FracTimeX*50) + 1)*0.1
#if defined(SKIP_SECOND_DEPTH_1)
fixed shore_z = saturate((raw_zdepth / _UFSHORE_Length_1) / 10 + _Z_BLACK_OFFSET_V *3);
	#else
fixed shore_z = GET_SHORE(i.uv.zw ) / _UFSHORE_Length_1 + _Z_BLACK_OFFSET_V;
#define HAS_SHORE_Z = 1;
#if defined(DEGUB_Z_SHORE)
			return fixed4(shore_z, shore_z, shore_z, 1);
#endif
	#endif

#if defined(USE_VERTEX_H_DISTORT)
		shore_z += (i.VER_TEX.w *VERTEX_H_DISTORT);
#endif
		//shore_z += 0.1;

			fixed compareVal = shore_z;
#if defined(SHORE_USE_ADDITIONALCONTUR_1) && !defined(MINIMUM_MODE)

			fixed adzmask =  
#if !defined(SHORE_ADDITIONALCONTUR_INVERSE)
				1 -
#endif
				saturate(raw_zdepth / (_UFSHORE_ADDITIONAL_Length_1) / 10);

#if !defined(SKIP_SHORE_ADDITIONALCONTUR_USE_Z)
			adzmask *= shore_z * shore_z;
#endif

/*#if defined(SHORE_USE_ADDITIONALCONTUR_SEPARATE)
			fixed adzmask = 1 - saturate(raw_zdepth / (_UFSHORE_ADDITIONAL_Length_1) / 10);
#else
			fixed adzmask = 1 - GET_SHORE(i.uv.zw) / _UFSHORE_ADDITIONAL_Length_1;
#endif*/
#if defined(SHORE_USE_ADDITIONALCONTUR_POW_1)
			adzmask = saturate( pow(adzmask, SHORE_USE_ADDITIONALCONTUR_POW_Amount_1) * SHORE_USE_ADDITIONALCONTUR_POW_Amount_1);
#endif
			compareVal -=  adzmask;
#endif
			//return compareVal;
			fixed refrv06 = 0;
			fixed adz = 0;

//#ifdef WATER_DOWNSAMPLING
//#else
//#if !defined(MINIMUM_MODE)
			//compareVal;
			//if (!floor(compareVal))

				if (compareVal < 1)

//#endif
			{
//#endif
			//	return 1;
#if defined(SHORE_USE_ADDITIONALCONTUR_1) && !defined(MINIMUM_MODE) && defined(SHORE_USE_ADDITIONALCONTUR_SEPARATE)
				//shore_z = max(shore_z, adzmask * shore_z * shore_z);
#endif


#if defined(SHORE_ANIM_YSIN_1)
				//shore_uv.y += _SinTime.w / 15;
#endif

#if defined(LOW_DISTORTION)
				//shore_uv += i.vfoam.xy * _UFSHORE_LowDistortion_1; ////////////////////// LOW DISTORTION
#endif

				//shore_uv.y += abs((frac((i.uv.x + i.uv.y) * 5 + _Frac01Time * 5) - 0.5)*2)*0.1;
				//shore_uv.y += (sin((i.uv.x + i.uv.y)* 20 + _Frac2PITime * 20) + 1)*0.1;

			
				
				//shore_uv += i.vfoam.zw;  //vertex additional offset value
#if defined(SHORE_USE_LOW_DISTORTION_1)
				fixed2 shwUVDist = i.vfoam.xy * _UFSHORE_LowDistortion_1;
#endif

				MYFLOAT2 foamDistortUV_SW = tnormal_TEXEL * _UFSHORE_Distortion_1 * shore_z;
				
#if defined(SHORE_USE_WAVES_GRADIENT_1)



				MYFLOAT2 shwUV = MYFLOAT2(i.wPos.x * MAIN_TEX_FoamGradDirection_1 * 0.1, -shore_z * MAIN_TEX_FoamGradTileYYY_1);
			//	shwUV.x = i.wPos.x * MAIN_TEX_FoamGradDirection_1 * 0.1;
#if defined(SHORE_USE_GLOB_DIR_1) && !defined(SKIP_UNWRAP_TEXTURE)
				//shwUV.y = i.wPos.y * MAIN_TEX_FoamGradTile_1;
#else
				//shwUV.y = 0;
#endif

#if defined(SHORE_USE_LOW_DISTORTION_1)
				shwUV += shwUVDist; ////////////////////// LOW DISTORTION
#endif
				
				shwUV = fixed2(shore_z * MAIN_TEX_FoamGradTile_1 + _Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1, foamDistortUV_SW.x) + shwUV;
				fixed foamGradient = TEX2DGRAD(_Utility, shwUV).g;
				//foam *= foamGradient;
				//return foamGradient.xxxx;

#if !defined(MINIMUM_MODE)
				//foamDistortUV_SW *= foamGradient;
#endif
#endif



#if defined(_ShoreWaves_SECOND_TEXTURE)
#define SHORE_TEX _UF_NMASK_Texture
#elif defined(_ShoreWaves_USE_MAINTEX)
#define SHORE_TEX _MainTex
#else
#define SHORE_TEX _ShoreWavesGrad
#endif

#if defined(_ShoreWaves_B)
#define SHORE_CHANNEL .b
#elif defined(_ShoreWaves_R)
#define SHORE_CHANNEL .r
#elif defined(_ShoreWaves_A)
#define SHORE_CHANNEL .a
#else
#define SHORE_CHANNEL .g
#endif









#if !defined(SKIP_UNWRAP_TEXTURE)
#if defined(SHORE_UNWRAP_STRETCH_1) && !defined(MINIMUM_MODE)
				MYFLOAT2 shore_uv = MYFLOAT2(i.vfoam.z, shore_z);
				shore_uv.x = (shore_uv.x + foamDistortUV_SW.x)* _UFSHORE_Tile_1 / 100;
				shore_uv.y = (shore_uv.y + foamDistortUV_SW.y * 5 
#if defined(_UFSHORE_UNWRAP_Low)
					+ (i.vfoam.y + i.vfoam.x)*_UFSHORE_UNWRAP_LowDistortion_1
#endif
					); //+ _Frac01Time

#else
				MYFLOAT2 shore_uv = i.uv.xy;
#if defined(_UFSHORE_UNWRAP_Low	)
				shore_uv += i.vfoam.xy * _UFSHORE_UNWRAP_LowDistortion_1; ////////////////////// LOW DISTORTION
#endif
				shore_uv = (shore_uv + foamDistortUV_SW.xy)* _UFSHORE_Tile_1;
#endif

#if defined(SHORE_ANIM_XMOVEBLEND_1) && !defined(MINIMUM_MODE)
				MYFLOAT2 unpack_uv ;
				unpack_uv.x = shore_uv.x + _Frac_UFSHORE_Tile_1Time_d10_m_UFSHORE_Speed1.x;//_FracTimeX /10;
				unpack_uv.y = shore_uv.y;
				fixed foam = tex2D(SHORE_TEX, unpack_uv)SHORE_CHANNEL;
				unpack_uv.x = shore_uv.x - _Frac_UFSHORE_Tile_1Time_d10_m_UFSHORE_Speed1.x;
				//return 1;
#if !defined(SHORE_UNWRAP_STRETCH_1)
				unpack_uv.y = shore_uv.y + 0.5;
#endif
				foam = (foam + tex2D(SHORE_TEX, unpack_uv)SHORE_CHANNEL) * 0.5;
#else
				fixed foam = tex2D(SHORE_TEX, shore_uv)SHORE_CHANNEL;
#endif
				foam -= _UFSHORE_UNWRAP_Transparency;
#else
				fixed foam = 1;
#endif



#if defined(SHORE_USE_WAVES_GRADIENT_1)
				foam *= foamGradient;
#endif
				//return shore_z * 15;
				//foam *= _UFSHORE_Amount_1;
				fixed Amount = _UFSHORE_Amount_1;
				//fixed3 Color_add = _UFSHORE_Color_1;




















				//return float4(foam.rrr,1);
				//fixed foamA = foam ;
				//fixed foamA = foam * (1- shore_z)  ;
				//return foamA;
				//return foam;

#if defined(SHORE_USE_ADDITIONALCONTUR_1)&& !defined(MINIMUM_MODE)
#define HAS_COUNTUR
#endif

				//fixed foamA = foam;
#if defined(SHORE_SHADDOW_1)
				foam.x -= shore_z * _UFSHORE_ShadowV1_1;
#if defined(HAS_COUNTUR)
				foam = max(0, foam);
#endif
				//foamA -= shore_z * _UFSHORE_ShadowV2_1;
#endif
				
#if defined(SHORE_USE_LOW_DISTORTION_1)
				//_UFSHORE_AlphaMax_1 -= abs(shwUVDist.y * 4); ////////////////////// LOW DISTORTION
#endif











#if defined(HAS_COUNTUR)
//fixed contur = pow(1 - shore_z, 100) * 2;

				MYFLOAT add_dist = tnormal_TEXEL * _UFSHORE_ADD_Distortion_1;
				MYFLOAT2 add_shore_uv = (i.uv.xy + add_dist)* _UFSHORE_ADD_Tile_1;
				add_shore_uv += i.vfoam.xy * _UFSHORE_ADD_LowDistortion_1; ////////////////////// LOW DISTORTION

#if !defined(SHORE_USE_ADDITIONAL_GRAD_TEXTURE)
				fixed3 unpack_add_texture = tex2D(_MainTex, add_shore_uv);
#else
				fixed3 unpack_add_texture = tex2D(SHORE_TEX, add_shore_uv);
#endif
#if defined(SHORE_USE_ADDITIONAL_GRAD_TEXTURE_B)
				fixed texture_channel = unpack_add_texture.b;
#elif defined(SHORE_USE_ADDITIONAL_GRAD_TEXTURE_G)
				fixed texture_channel = unpack_add_texture.g;
#else
				fixed texture_channel = unpack_add_texture.r;
#endif

				 adz = (texture_channel* _UFSHORE_ADD_Amount_1) * (adzmask); // #todo saturate to fix
				 //foam = lerp(foam, adz, adz);

				//Color_add = lerp(Color_add, _UFSHORE_ADD_Color_1, adz);
				//Amount -= adz;
				//_UFSHORE_AlphaMax_1 -= adz;
				//return foam;
				//foam += adz * adzmask;

#endif








				fixed IA = saturate((_UFSHORE_AlphaMax_1 - shore_z
					) / _UFSHORE_AlphaMax_1) /*- abs(_SinTime.y)/10*/;

				fixed foamA = foam;
#if defined(SHORE_SHADDOW_1)
				foamA += shore_z * _UFSHORE_ShadowV2_1;
#endif
				foamA *= IA;
				foam *= IA * Amount;

				/*fixed foamA = foam * IA;
				foam = foamA * Amount;*/











#if defined(HAS_COUNTUR)
				foam += adz;
				foamA = saturate(foamA + adz / 4);
#endif






#if !defined(MINIMUM_MODE)
				

				//foam.y =  IA +  foam.y;
				//foam.y = saturate(IA /2+ IA *  foam.y);
				//return foamColor;
				//return foamA;

				//forgot what this
/*#if defined(USE_ALPHA_FADING)
				foamA *= saturate(0.2 + abs(i.vfoam.z) *4);
#endif*/
				//foam = saturate(foam);
				//foam *= _UFSHORE_Color_1;
				//color.rgb = lerp(color.rgb, foam * _UFSHORE_Color_1, (foamA));
				//return color.rgbb;
				//+ _SinTime.w *0.2
				fixed FOAM_ALPHA = saturate(IA *IA*IA* _UFSHORE_AlphaAmount_1 * 4);
#else
			//	fixed foamA = IA * Amount * foam;
				//foam *= Amount;
				fixed FOAM_ALPHA=  IA;
#endif

				//if (foamA < 0) return 0;
				//if (foamA < 0) return float4(1, 0, 0, 1);
				//if (foam < 0) return float4(0, 1, 0, 1);
				//return float4(0, 1, 0, 1);
				color.rgb = lerp(color.rgb, foam * _UFSHORE_Color_1, foamA);

				//return color.rgbb;
				//color.rgb = foam;

#if defined(HAS_REFLECTION)
#define SHOULD_RD_SATURATE 1
				rd -= foamA;
				//rd -= foam;
#endif








				//#if defined(FORCE_OPAQUE)
	#if defined(APPLY_REF)
#define FOAM_ALPHA_NEED = 1;
				 refrv06 = FOAM_ALPHA;
				//return refrv06;
				//refrv06 = pow(1-shore_z, 50);
				//return refrv06;


#endif
#if defined(APPLY_FOAM_OUTALPHA_1) && !defined(FORCE_OPAQUE)
#if defined (APPLY_REF)
				color.a = 1 - refrv06;
#else
				color.a = 1 - FOAM_ALPHA;
#endif
#endif
				/*#else
				fixed refrv06 = FOAM_ALPHA;
				color.a *= 1 - refrv06;
	#endif*/
				//color.a *= 1 - FOAM_ALPHA;
//#if !defined(MINIMUM_MODE)
//#ifdef WATER_DOWNSAMPLING
//#else
			}
//#endif
			//#endif

#endif
