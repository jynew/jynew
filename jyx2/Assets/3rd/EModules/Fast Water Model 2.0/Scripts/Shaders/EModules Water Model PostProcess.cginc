

#if !defined(SKIP_3DVERTEX_ANIMATION) && !defined(SKIP_3DVERTEX_HEIGHT_COLORIZE) && !defined(MINIMUM_MODE)
	//color.rgb += saturate(i._utils.y * 5 * color.rgb * _3DWavesYFoamAmount * i._utils.x);
//fixed w3dam01 = i._utils.y * 5 * _3DWavesYFoamAmount;

	//color.rgb += saturate(color.rgb  * w3dam01);


#if defined(VERT_FOAM_TEXTURE) && defined(HAS_SHORE_WAVES_GRAD)
#define VERT_TEX _ShoreWavesGrad
#elif  defined(VERT_SECOND_TEXTURE)
#define VERT_TEX _UF_NMASK_Texture
#else
#define VERT_TEX _MainTex
#endif

#if defined(VERT_B)
#define VERT_CHANNEL .b
#elif defined(VERT_G)
#define VERT_CHANNEL .g
#elif defined(VERT_A)
#define VERT_CHANNEL .a
#else
#define VERT_CHANNEL .r
#endif



fixed v = i.VER_TEX.w * _3DWavesYFoamAmount + _WaveGradTopOffset;
v = saturate(v - 2);

fixed VERT_RESULT = tex2D(VERT_TEX, i.uv.xy * _VERT_Tile)VERT_CHANNEL * v * _VERT_Amount;

#if defined(USE_VERT_FRESNEL)
VERT_RESULT *= 2 - wViewDir.y ;
#endif

#if defined(DEBUG_TOP_GRAD)
return float4(VERT_RESULT.rrr, 1);
#endif

//color.rgb = color.rgb * surface_grad;

#if defined(VERT_USE_MULTIPLUOUT)
fixed3 preres = VERT_RESULT * _VERT_Color;
color.rgb += color.rgb * preres;
#else

fixed3 VERT_SUM = VERT_RESULT * _VERT_Color;
color.rgb += VERT_SUM;


#if defined(HAS_REFLECTION)
#if !defined(SHOULD_RD_SATURATE)
#define SHOULD_RD_SATURATE 1
#endif
rd -= VERT_SUM.b;
#endif


#endif



/*fixed v = i.VER_TEX.w * _3DWavesYFoamAmount + _WaveGradTopOffset;
v = saturate(v - 2);
//fixed3 grl1 = _WaveGrad1 * 1.5;
fixed3 grl1 = _WaveGrad1 ;
//fixed3 grl2 = _WaveGrad0 * 2;
fixed3 grl2 = _WaveGrad0 ;
fixed grl3 = v;

fixed3 top_grad_color = lerp(grl1, grl2, grl3);

fixed grl10 = v;
fixed3 bottom_grad_color = lerp(_WaveGrad2, top_grad_color, grl10);

fixed3 surface_grad = bottom_grad_color	;

#if defined(DEBUG_TOP_GRAD)
return float4(surface_grad.rgb, 1);
#endif

//color.rgb = color.rgb * surface_grad;
color.rgb += tex2D(SHORE_TEX, i.uv.xy).r * surface_grad;*/


#endif

#if !defined(SKIP_3DVERTEX_ANIMATION) && defined(USE_SIMPLE_VHEIGHT_FADING)
color.rgb *= ((i.VER_TEX.w* SIMPLE_VHEIGHT_FADING_AFFECT + 1));
#endif


#if defined(USE_lerped_post)

#if defined(lerped_post_USE_depthZ)
fixed lerped_z = zdepth;
#else
#if defined(HAS_CAMERA_DEPTH) && defined(HAS_SHORE_Z)
fixed lerped_z = GET_BAKED_Z(i.uv.zw);
//fixed lerped_z = shore_z;
#else
fixed lerped_z = raw_zdepth;
#endif
#endif

fixed lerped_post = saturate((lerped_z - lerped_post_offset) / lerped_post_offset_falloff

#ifdef ORTO_CAMERA_ON//////////////////////////////////////
	* (dot(wViewDir, fixed3(0, 1, 0)) * 5 + 1)
#endif
	);
#if defined(lerped_post_Debug)
return float4(lerped_post, lerped_post, lerped_post,1);
#endif
fixed3 c1 = (1-lerped_post) * lerped_post_color1;
fixed3 c2 = lerped_post * lerped_post_color2;
//return float4(c1, 1);
#if defined(USE_lerped_post_Color_1)
#if defined(lerped_post_LERP_1)
color.rgb = lerp(color.rgb, c1, c1);
#elif defined(lerped_post_MUL_1)
color.rgb += color.rgb * c1;
#elif defined(lerped_post_SUB_1)
color.rgb -= c1;
#else
color.rgb += c1;
#endif
#endif
#if defined(USE_lerped_post_Color_2)
#if defined(lerped_post_LERP_2)
color.rgb = lerp(color.rgb, c2, c2);
#elif defined(lerped_post_MUL_2)
color.rgb += color.rgb * c2;
#elif defined(lerped_post_SUB_2)
color.rgb -= c2;
#else
color.rgb += c2;
#endif
#endif
#endif




#if defined(POST_TEXTURE_TINT)

#if defined(MM_B)
#define POS_CHANNEL .b
#elif defined(MM_G)
#define POS_CHANNEL .g
#elif defined(MM_A)
#define POS_CHANNEL .a
#else
#define POS_CHANNEL .r
#endif

#if defined(POST_FOAM_TEXTURE) && defined(HAS_SHORE_WAVES_GRAD)
fixed mm_channel = tex2D(_ShoreWavesGrad, DETILEUV * _MM_Tile + _MM_offset)POS_CHANNEL;
#elif defined(POST_OWN_TEXTURE)
fixed mm_channel = tex2D(_MM_Texture, DETILEUV * _MM_Tile + _MM_offset)POS_CHANNEL;
#elif defined(POST_SECOND_TEXTURE)
fixed mm_channel = tex2D(_UF_NMASK_Texture, DETILEUV * _MM_Tile + _MM_offset)POS_CHANNEL;
#else
fixed mm_channel = tex2D(_MainTex, DETILEUV * _MM_Tile + _MM_offset)POS_CHANNEL;
#endif

fixed3 mm_res = mm_channel * _MM_Color;

#if defined(MM_LERP)
color.rgb = lerp(color.rgb, mm_res, mm_res);
#elif defined(MM_MUL)
color.rgb = color.rgb * (mm_res + _MM_MultyOffset);
#elif defined(MM_SUB)
color.rgb -= mm_res;
#else
color.rgb += mm_res;
#endif

#endif











#if !defined(FORCE_OPAQUE)
	//color.a = foamColor.a;

#if defined(TRANSPARENT_LUMINOSITY)
	color.a *= lerp(1 - saturate((tnormal.x + tnormal.y) * 5) - 1 + _TransparencyLuminosity, 1, 1 - fresnelFac);
#endif
#if defined(TRANSPARENT_POW)
#if !defined(TRANSPARENT_POW_INV)
	//color.a *= saturate((pow((color.rgb - (2 - _TransparencyPow * 3)), 1)));
	color.a *= saturate(((color.rgb - (2 - _TransparencyPow * 3))));
#else 
	//color.a *= saturate(pow(abs(1 - color.rgb - (3 - (1 - _TransparencyPow) * 3)), 1)) + specularLight;
	color.a *= saturate(abs(1 - color.rgb - (3 - (1 - _TransparencyPow) * 3))) + specularLight;
#endif
	//color.a *= saturate((pow(abs(color.rgb - (2 - _TransparencyPow * 3)), 1))) + specularLight * (1 - saturate( _TransparencyPow * 10 - 5));
#endif
#if defined(TRANSPARENT_SIMPLE)
	color.a *= _TransparencySimple;
#endif

#endif


#if defined(RIM)


	/*#if defined(SKIP_FRESNEL_CALCULATION) || defined(ULTRA_FAST_MODE)

	//fixed dott = pow(dot(fixed3(0, 1, 0), tnormal), RIM_Plus);
#if defined(RIM_INVERSE)
	fixed dott = pow(1-	dot(wViewDir, worldNormal), RIM_Plus);
#else 
	fixed dott = pow(dot(wViewDir, worldNormal), RIM_Plus);
#endif
	fixed rim = 0.98 - (dott);
	//rim = min(0.95, rim);
	//rim = max(0.6, rim);
	fixed minus = saturate(pow(rim, RIM_Minus)) * color.b;
	//fixed3 plus = saturate(pow(rim, RIM_Plus)) * color.rgb;
	//color.rgb -= minus;
	//color.rgb += plus;
	//return dott;

	//dott = saturate(pow(dot(normalize(i.wViewDir), worldNormal), RIM_Plus));



	fixed diff = dott * minus;
#else 
	fixed diff = (1 - fresnelFac * RIM_Plus + RIM_Minus);
#endif*/

	/*fixed NdotL = dot(fixed3(0, -1, 0), worldNormal);
	NdotL = NdotL * 0.5 + 1;
	fixed diff =  pow(NdotL, RIM_Minus);
	diff *= RIM_Plus*20;*/

	fixed NdotL = dot(worldNormal, wViewDir);
	NdotL = NdotL * 0.5 + 1;
	fixed diff = pow(NdotL, RIM_Minus);
	diff *= RIM_Plus ;
	
//fixed NdotL = dot(worldNormal, wViewDir) + dot(fixed3(0, 1, 0), worldNormal);
	
	//fixed NdotL = (abs(i.helpers.x) + abs(i.helpers.y))*RIM_Plus;
	/*fixed3 RN = worldNormal;
	RN.xz += i.helpers.xy * RIM_Plus * 200;
	RN = normalize(RN);
fixed NdotL = dot(fixed3(0, 1, 0), RN);
//	fixed NdotL = dot(wViewDir, RN);
	//fixed NdotL = dot(worldNormal, wViewDir) ;
//NdotL = NdotL *0.5 + 1;
NdotL *= 1 - wViewDir.y / 2;
//NdotL = ((-i.helpers.x)- (i.helpers.y))*5;

	//fixed diff = NdotL + 1;
	//diff =saturate( pow(diff, RIM_Plus ) - 1) * RIM_Minus;
	fixed diff = pow(NdotL, RIM_Minus);
#if defined(RIM_INVERSE)
	//diff = 1 - diff;
#endif
	//diff *= RIM_Plus;
	diff *= 10;
	//diff = saturate(diff);
#if defined(RIM_INVERSE)
	//diff = 1 - diff;
#endif*/

#if defined(RIM_SCHORE_SKIP) && !defined(DEPTH_NONE)
#if defined(UFAST_SHORE_1)
	
	diff =  lerp(diff,1,
		refrv06
#if defined(HAS_COUNTUR)
		+ adz
#endif
	) ;
#endif
#endif
#if defined(RIM_DEBUG)
	return float4(1-diff.rrr,1);
#endif
	//
	//diff = 1 - diff;
	fixed3 ramp = TEX2DGRAD(_RimGradient, saturate(fixed2(diff, 0))).rgb;
#if defined(USE_RIM_BLEND)
	ramp = lerp(ramp, 1, _RIM_BLEND);
#endif
	color.rgb *= ramp;
	//
#endif
		//RIM



#if defined(USE_SURFACE_GRADS) && !defined(DEPTH_NONE) && FALSE

	/*	fixed v = 1 - zdepth * _WaveGradTopOffset;
	fixed I_WaveGradMidOffset = 1 - _WaveGradMidOffset;

	fixed3 grl1 = _WaveGrad1 * 1.5;
	fixed3 grl2 = _WaveGrad0 * 2;
	fixed grl3 = saturate((v - _WaveGradMidOffset) / I_WaveGradMidOffset);

	fixed3 top_grad_color = lerp(grl1, grl2, grl3);

	fixed grl10 = saturate(v / _WaveGradMidOffset);
	fixed3 bottom_grad_color = lerp(_WaveGrad2, top_grad_color, grl10);

	fixed3 surface_grad = bottom_grad_color;

#if defined(DEBUG_TOP_GRAD)
	return float4(surface_grad.rgb, 1);
#endif

	color.rgb = color.rgb * surface_grad;*/

#endif





#if !defined(SKIP_AMBIENT_COLOR)
	color *= UNITY_LIGHTMODEL_AMBIENT;
#endif


	//return float4(UNITY_LIGHTMODEL_AMBIENT.rrr, 1);



#if defined(POSTRIZE)
	color.rgb = postrize(color.rgb, POSTRIZE_Colors);
	//color.b = floor(color.b * POSTRIZE_Colors) / (POSTRIZE_Colors - 1);
#endif



#if defined(USE_OUTPUT_GRADIENT)
		/* fixed gr = tex2D(_Utility, fixed2(color.r, 0.75)).b;
		fixed gg = tex2D(_Utility, fixed2(color.g, 0.75)).b;
		fixed gb = tex2D(_Utility, fixed2(color.b, 0.75)).b;
		fixed3 grad = fixed3(gr, gg, gb);*/
#if defined(GRAD_5)
	fixed POS = 0.85;
#elif defined(GRAD_4)
	fixed POS = 0.65;
#elif defined(GRAD_3)
	fixed POS = 0.5;
#elif defined(GRAD_2)
	fixed POS = 0.35;
#else
	fixed POS = 0.15;
#endif
	//fixed3 grad = TEX2DGRAD(_GradTexture, fixed2(((color.r + color.g + color.b) / 3), POS));
	fixed3 grad = TEX2DGRAD(_GradTexture, fixed2(( dot ( fixed3(color.r , color.g , color.b), fixed3(0.333,0.333,0.333)) ), POS));
	// color.rgb = grad;


#if defined(USE_OUTPUT_BLEND_1)
	fixed ouv05 = _OutGradBlend * saturate(zdepth / _OutGradZ);
	/*fixed3 suv07 = saturate(grad - color.rgb);
	fixed3 ouv10 = suv07 * ouv05;
	color.rgb = ouv10 + color.rgb;*/
	//fixed3 zv01 = (grad - color.rgb) * ouv05 + color.rgb;
	fixed3 zv01 = lerp(color.rgb, grad , ouv05 );
	color.rgb = max(zv01, color.rgb);
	//color.rgb = max(color.rgb, );

#elif defined(USE_OUTPUT_BLEND_3)
#if defined(FIX_HL)
	fixed fixhl = saturate(color.rgb - _FixHLClamp);
#endif
	//color.rgb = (grad - color.rgb) * _OutGradBlend + color.rgb;
	color.rgb = lerp(color.rgb, grad, _OutGradBlend);
#if defined(FIX_HL)
	color.rgb += fixhl;
#endif
#else
#if defined(FIX_HL)
	fixed fixhl = saturate(color.rgb - _FixHLClamp);
#endif
	fixed ouv10 = _OutGradBlend * saturate(zdepth / _OutGradZ);
	//color.rgb = (grad - color.rgb) * ouv10 + color.rgb;
	color.rgb = lerp(color.rgb, grad, ouv10);
#if defined(FIX_HL)
	color.rgb += fixhl;
#endif
#endif


#if defined(USE_OUTPUT_SHADOWS)
	/*fixed NdotL = dot(worldNormal, fixed3(i.tspace0.z, i.tspace1.z, i.tspace2.z));
	//fixed NdotL = dot(worldNormal, fixed3(0,1,0)) ;
	NdotL = 1 - pow(NdotL, 4048);
	// NdotL =  pow( NdotL , 16);
	fixed ds = (NdotL / 10) * _OutShadowsAmount;
	color.rgb -= ds;*/
#endif

#endif
	//color.b = floor(color.b * 6) / (6 - 1);

#if !defined(FORCE_OPAQUE)
	color.a *= saturate(i.helpers.z*10);
#endif
	//color.a *= max(0,i.helpers.z);


#if defined (USE_LUT)
	fixed3 uvw = (color.rgb);

#if !UNITY_COLORSPACE_GAMMA
	uvw = LinearToGammaSpace(uvw);
#endif

#if defined(FIX_OVEREXPO)
	fixed3 maxvalue = saturate(uvw - 1);
	uvw = saturate(uvw);
#endif

	fixed3 scaleOffset = _Lut2D_params.xyz;

	uvw.z *= scaleOffset.z;
	fixed shift = floor(uvw.z);
#if defined(LUT_HQ_MODE)
	uvw.xy = (uvw.xy * scaleOffset.z  +  0.5)*scaleOffset.xy;
#else
	uvw.xy = (uvw.xy * scaleOffset.z )*scaleOffset.xy;
#endif
	//uvw.xy = uvw.xy * scaleOffset.z * scaleOffset.xy + scaleOffset.xy * 0.5;
	uvw.x += shift * scaleOffset.y;
#if defined(LUT_HQ_MODE)
	uvw.y = saturate(uvw.y);
#endif
	fixed3 lutv02 = TEX2DGRAD(_Lut2D, uvw.xy).rgb;
#if !defined(LUT_HQ_MODE)
	uvw.xyz = lutv02;
#else
	fixed2 uvw2 = uvw;
	uvw2.x += scaleOffset.y;
	fixed3 lutv05 = TEX2DGRAD(_Lut2D, uvw2).rgb;
	fixed lutv08 = uvw.z - shift;
	uvw.xyz = lerp(lutv02, lutv05, lutv08);
#endif

#if defined(FIX_OVEREXPO)
	uvw += maxvalue;
#endif

#if !UNITY_COLORSPACE_GAMMA
	uvw = GammaToLinearSpace(uvw);
#endif
	//color.rgb = lerp(color.rgb, uvw, _Lut2D_ST.w);
	//color.rgb = uvw;



	color.rgb = lerp(color.rgb, uvw, _LutAmount);
#endif


#if defined(HAS_REFLECTION)  && defined(ULTRA_FAST_MODE)
	// !defined(MINIMUM_MODE)
#if defined(SHOULD_RD_SATURATE)
	rd = saturate(rd);
#endif
	color.rgb = lerp(color.rgb, targetRefl, rd);
#endif


#if !defined(SKIP_FOAM_ALPHA_OVERRIDE)
#if defined(FOAM_ALPHA_NEED) && !defined(NO_REFR_TEX)
	
#if defined(FOAM_ALPHA_FLAT)
#if	defined(REFRACTION_GRABPASS)
		fixed3 refractionColor_raw = REFRTEX(i.grabPos);
#else
	fixed3 refractionColor_raw = REFRTEX(i.uv.zw);
#endif
	//return float4(refractionColor_raw, 1); 
	//color.rgb = REFRTEX(i.uv.zw);
	color.rgb = lerp(color.rgb, refractionColor_raw, refrv06);
#else
	color.rgb = lerp(color.rgb, refractionColor, refrv06);
#endif
	//return lerp(color.rgb, refractionColor, refrv06).rgbb;
#endif
#endif


#if defined(USE_LIGHTMAPS)
		//i._utils.zw
#if LIGHTMAP_ON
	color.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv.zw * unity_LightmapST.xy + unity_LightmapST.zw));
#endif
#endif

#if defined(HAS_USE_SHADOWS)
	fixed shad_at = lerp( 1, SHADOW_ATTENUATION(i), _ShadorAmount);
	color.rgb *= shad_at;
#endif
//	return SHADOW_ATTENUATION(i);
#if !defined(SKIP_FOG)
#ifdef ORTO_CAMERA_ON

	UNITY_CALC_FOG_FACTOR_RAW(i.fogCoord.x);
	APP_FOG(unityFogFactor, color);
#else
	//color.rgb = lerp(unity_FogColor.rgb, color.rgb, i.fogCoord);
	UNITY_APPLY_FOG(i.fogCoord, color);
#endif
#endif



