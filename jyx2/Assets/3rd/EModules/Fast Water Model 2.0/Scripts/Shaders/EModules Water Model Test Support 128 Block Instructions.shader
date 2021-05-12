//version: 1.0
Shader "EM-X/Fast Water Model 2.0/Test Support 128 Block Instructions"


























{

	Properties{



		_FrameRate("_FrameRate", Float) = 0



		_FracTimeFull("_FracTimeFull", Float) = 0

		_Frac2PITime("_Frac2PITime", Float) = 0

		_Frac01Time("_Frac01Time", Float) = 0

		_Frac01Time_d8_mBlendAnimSpeed("_Frac01Time_d8_mBlendAnimSpeed", Float) = 0

		_Frac01Time_MAIN_TEX_BA_Speed("_Frac01Time_MAIN_TEX_BA_Speed", Float) = 0

		_FracWTime_m4_m3DWavesSpeed_dPI2("_FracWTime_m4_m3DWavesSpeed_dPI2", Vector) = (0,0,0,0)

		_Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1("_Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1", Float) = 0

		_Frac_UFSHORE_Tile_1Time_d10("_Frac_UFSHORE_Tile_1Time_d10", Vector) = (0,0,0,0)

		_Frac_UFSHORE_Tile_2Time_d10("_Frac_UFSHORE_Tile_2Time_d10", Vector) = (0,0,0,0)

		_Frac_UFSHORE_Tile_2Time_d10("_Frac_UFSHORE_Tile_2Time_d10", Vector) = (0,0,0,0)

	_Frac_WaterTextureTilingTime_m_AnimMove("_Frac_WaterTextureTilingTime_m_AnimMove", Vector) = (0,0,0,0)

	_Frac_UVS_DIR("_Frac_UVS_DIR", Vector) = (0,0,0,0)

	_FracMAIN_TEX_TileTime_mMAIN_TEX_Move("_FracMAIN_TEX_TileTime_mMAIN_TEX_Move", Vector) = (0,0,0,0)

	_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed("_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed", Vector) = (0,0,0,0)

	_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed("_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed", Vector) = (0,0,0,0)



	_MM_Tile("_MM_Tile", Vector) = (1,1,1,1)

	_MM_offset("_MM_offset", Vector) = (0,0,0,0)

	_MM_Color("_MM_Color", COLOR) = (1,1,1,1)

	_ReflectionJustColor("_ReflectionJustColor", COLOR) = (1,1,1,1)



		MAIN_TEX_Bright("MAIN_TEX_Bright", Float) = 0



		lerped_post_offset("lerped_post_offset", Float) = 0

		lerped_post_offset_falloff("lerped_post_offset_falloff", Float) = 1

	lerped_post_color1("lerped_post_color1", COLOR) = (1,1,1,1)

	lerped_post_color2("lerped_post_color2", COLOR) = (1,1,1,1)



		[Header(Texture)]//////////////////////////////////////////////////

	[NoScaleOffset] _MainTex("Additional Color Texture(RGB) Mask for reflection(A)", 2D) = "black" {}

		_MainTexAngle("_MainTexAngle", Float) = 0 //Range(0.1,10)

	_MainTexTile("_MainTexTile", Vector) = (1,1,0,0)

	_MainTexColor("Tint Color (RGB) Amount Texture (A)", COLOR) = (1,1,1,1)

		_OutGradBlend("_OutGradBlend", Float) = 1 //Range(0.1,10)

		_OutShadowsAmount("_OutShadowsAmount", Float) = 20 //Range(0.1,10)

		_OutGradZ("_OutGradZ", Float) = 1 //Range(0.1,10)

		_UFSHORE_UNWRAP_Transparency("_UFSHORE_UNWRAP_Transparency", Float) = 0 //Range(0.1,10)



		_AnimMove("_AnimMove", Vector) = (0,0,0,0)





		_VERTEX_ANIM_DETILESPEED("_VERTEX_ANIM_DETILESPEED", Float) = 0

		_VERTEX_ANIM_DETILEAMOUNT("_VERTEX_ANIM_DETILEAMOUNT", Float) = 1





		_Z_BLACK_OFFSET_V("_Z_BLACK_OFFSET_V", Float) = 0



		MAINTEX_VHEIGHT_Amount("MAINTEX_VHEIGHT_Amount", Float) = 1

		MAINTEX_VHEIGHT_Offset("MAINTEX_VHEIGHT_Offset", Float) = 0.8



		_FoamBlendOffset("_FoamBlendOffset", Float) = 0



	[Header(Commons)]//////////////////////////////////////////////////

		_TransparencyPow("_TransparencyPow", Float) = 1 //Range(0,2)

		_TransparencyLuminosity("_TransparencyLuminosity", Float) = 1 //Range(0,2)

		_TransparencySimple("_TransparencySimple", Float) = 1 //Range(0,2)





	_WaterTextureTiling("_Tiling [x,y]; Speed[z];", Vector) = (1,1,1,-1)

	_BumpAmount("_BumpAmount", Float) = 1 //Range(0,1)

	[NoScaleOffset] _BumpMap("_BumpMap ", 2D) = "bump" {}

	_MM_MultyOffset("_MM_MultyOffset", Float) = 0.3



	[Header(Specular)]//////////////////////////////////////////////////

		_LightAmount("_LightAmount", Float) = 1//Range(0,2)

	_SpecularAmount("_SpecularAmount", Float) = 2 //Range(0,10)

	_SpecularShininess("_SpecularShininess", Float) = 48 //Range(0,512)

	_FlatSpecularAmount("_FlatSpecularAmount", Float) = 1.0//Range(0,10)

	_FlatSpecularShininess("_FlatSpecularShininess", Float) = 48//Range(0,512)

	_FlatSpecularClamp("_FlatSpecularClamp", Float) = 10//Range(0,100)

			_FlatFriqX("_FlatFriqX", Float) = 1//Range(0,100)

			_FlatFriqY("_FlatFriqY", Float) = 1//Range(0,100)



		_LightColor0Fake("_LightColor0Fake", COLOR) = (1,1,1,1)

			_BlendColor("_BlendColor", COLOR) = (1,1,1,1)



	[Header(Foam)]//////////////////////////////////////////////////

		_FoamAmount("_FoamAmount", Float) = 5 //Range(0,10)

	_FoamLength("_FoamLength", Float) = 10 //Range(0,20)

	_FoamColor("_FoamColor", COLOR) = (1,1,1,1)

		_FoamDistortion("_FoamDistortion", Float) = 1 //Range(0,10)

		_FoamDistortionFade("_FoamDistortionFade", Float) = 0.8 //Range(0,1)

		_FoamDirection("_FoamDirection", Float) = 0.05 //Range(0,1)

		_FixMulty("_FixMulty", Float) = 1 //Range(0,1)

		_FoamAlpha2Amount("_FoamAlpha2Amount", Float) = 0.5 //Range(0,1)



		_BlendAnimSpeed("_BlendAnimSpeed", Float) = 1 //Range(0,1)



	_WaterfrontFade("_WaterfrontFade", Float) = 10//Range(0,30) 





	_FoamTextureTiling("_FoamTextureTiling", Float) = 1 //Range(0.01,2)

	_FoamWavesSpeed("_FoamWavesSpeed", Float) = 0.15//Range(0,2 )

	_FoamOffsetSpeed("_FoamOffsetSpeed", Float) = 0.2//Range(0,4 ) 

		_FoamOffset("_FoamOffset", Float) = 0//Range(0,4 ) 





		_MultyOctavesSpeedOffset("_MultyOctavesSpeedOffset", Float) = 0.89

		_MultyOctavesTileOffset("_MultyOctavesTileOffset", Vector) = (1.63,1.63,1,1)

		_FadingFactor("_FadingFactor", Float) = 0.5

		FLAT_HQ_OFFSET("FLAT_HQ_OFFSET", Float) = 50





	[Header(3D Waves)]//////////////////////////////////////////////////

		_3DWavesTile("_3DWavesTile", Vector) = (1,1,-1,-1)

			//_3DWavesSpeed("_3DWavesSpeed",  Vector) = (0.1 ,0.1, 0.1, 0.1)

			_3DWavesSpeed("_3DWavesSpeed",  Float) = 0.1

			_3DWavesSpeedY("_3DWavesSpeedY",  Float) = 0.1

			_3DWavesHeight("_3DWavesHeight", Float) = 1//Range(0.1,32)

			_3DWavesWind("_3DWavesWind", Float) = 0.1//Range(0.1,32)

			_3DWavesYFoamAmount("_3DWavesYFoamAmount", Float) = 0.02//Range(0,10)

			_3DWavesTileZ("_3DWavesTileZ", Float) = 1 //Range(0.1,32)

			_3DWavesTileZAm("_3DWavesTileZAm", Float) = 1 //Range(0.1,32)

			ADDITIONAL_FOAM_DISTORTION_AMOUNT("ADDITIONAL_FOAM_DISTORTION_AMOUNT", Float) = 1 //Range(0.1,32)



			_SURFACE_FOG_Amount("_SURFACE_FOG_Amount", Float) = 1 //Range(0.1,32)

			_SURFACE_FOG_Speed("_SURFACE_FOG_Speed", Float) = 1 //Range(0.1,32)

			_SURFACE_FOG_Tiling("_SURFACE_FOG_Tiling", Float) = 1 //Range(0.1,32)



			_Light_FlatSpecTopDir("_Light_FlatSpecTopDir",  Float) = 0.5

			MAIN_TEX_Multy("MAIN_TEX_Multy",  Float) = 1









			_RefrLowDist("_RefrLowDist",  Float) = 0.1

			_VertexToUv("_VertexToUv",  Float) = 1





		_WavesDirAngle("_WavesDirAngle", Float) = 0

			_VertexSize("_VertexSize", Vector) = (0,0,1,1)



			_ZDistanceCalcOffset("_ZDistanceCalcOffset", Float) = 0.25



			[Header(NOISES)]//////////////////////////////////////////////////

				[Header(HQ)]//////////////////////////////////////////////////

				_NHQ_GlareAmount("_NHQ_GlareAmount", Float) = 1//Range(0.1,50)

			_NHQ_GlareFriq("_NHQ_GlareFriq", Float) = 30 //Range(0.1,100)

			_NHQ_GlareSpeedXY("_NHQ_GlareSpeedXY", Float) = 1//Range(0,3)

			_NHQ_GlareSpeedZ("_NHQ_GlareSpeedZ", Float) = 1 //Range(0,5)

			_NHQ_GlareContrast("_NHQ_GlareContrast", Float) = 0//Range(0,2.5)

			_NHQ_GlareBlackOffset("_NHQ_GlareBlackOffset", Float) = 0 //Range(-1,0)

			_NHQ_GlareNormalsEffect("_NHQ_GlareNormalsEffect", Float) = 1//Range(0,2) 

			[Header(LQ)]//////////////////////////////////////////////////

				_NE1_GlareAmount("_NE1_GlareAmount", Float) = 1//Range(0.1,50 )

			_NE1_GlareFriq("_NE1_GlareFriq", Float) = 1//Range(0.1,4)

			_NE1_GlareSpeed("_NE1_GlareSpeed", Float) = 1//Range(0.1,3)

			_NE1_GlareContrast("_NE1_GlareContrast", Float) = 0//Range(0,2.5)

			_NE1_GlareBlackOffset("_NE1_GlareBlackOffset", Float) = 0 //Range(-1,0) 

			_NE1_WavesDirection("_NE1_WavesDirection", Vector) = (0.5,0,0.25)

			[Header(WAWES1)]//////////////////////////////////////////////////

				_W1_GlareAmount("_W1_GlareAmount", Float) = 1//Range(0.1,50)

			_W1_GlareFriq("_W1_GlareFriq", Float) = 1//Range(0.1,10)

			_W1_GlareSpeed("_W1_GlareSpeed", Float) = 1//Range(0.1,10)

			_W1_GlareContrast("_W1_GlareContrast", Float) = 0//Range(0,2.5)

			_W1_GlareBlackOffset("_W1_GlareBlackOffset", Float) = 0//Range(-1,0)



			[Header(WAWES2)]//////////////////////////////////////////////////

				_W2_GlareAmount("_W2_GlareAmount", Float) = 1//Range(0.1,50)

			_W2_GlareFriq("_W2_GlareFriq", Float) = 1//Range(0.1,10)

			_W2_GlareSpeed("_W2_GlareSpeed", Float) = 1//Range(0.1,10) 

			_W2_GlareContrast("_W2_GlareContrast", Float) = 0//Range(0,2.5)

			_W2_GlareBlackOffset("_W2_GlareBlackOffset", Float) = 0//Range(-1,0) 

			MAIN_TEX_LQDistortion("MAIN_TEX_LQDistortion", Float) = 0.1

			MAIN_TEX_ADDDISTORTION_THAN_MOVE_Amount("MAIN_TEX_ADDDISTORTION_THAN_MOVE_Amount", Float) = 100





			[Header(PRC HQ)]//////////////////////////////////////////////////

				_PRCHQ_amount("_PRCHQ_amount", Float) = 1//Range(0.1,50)

			_PRCHQ_tileTex("_PRCHQ_tileTex", Float) = 1//Range(0.1,10 )

			_PRCHQ_tileWaves("_PRCHQ_tileWaves", Float) = 1//Range(0.1,10)

			_PRCHQ_speedTex("_PRCHQ_speedTex", Float) = 1//Range(0.1,10)

			_PRCHQ_speedWaves("_PRCHQ_speedWaves", Float) = 1 //Range(0.1,10)

			MAIN_TEX_Amount("MAIN_TEX_Amount", Float) = 3 //Range(0.1,10)

			MAIN_TEX_Contrast("MAIN_TEX_Contrast", Float) = 2 //Range(0.1,10)

			MAIN_TEX_BA_Speed("MAIN_TEX_BA_Speed", Float) = 8 //Range(0.1,10)

			MAIN_TEX_CA_Speed("MAIN_TEX_CA_Speed", Vector) = (0.1,0.1,0.1,0.1)

			MAIN_TEX_Tile("MAIN_TEX_Tile", Vector) = (1,1,1,1)

			MAIN_TEX_Move("MAIN_TEX_Move", Vector) = (0,0,0,0)



			SIMPLE_VHEIGHT_FADING_AFFECT("SIMPLE_VHEIGHT_FADING_AFFECT", Float) = 0.3

			MAIN_TEX_Distortion("MAIN_TEX_Distortion", Float) = 2

			_ReflectionBlendAmount("_ReflectionBlendAmount", Float) = 2





			[Header(fresnelFac)]//////////////////////////////////////////////////

				[NoScaleOffset] _Utility("_Utility", 2D) = "white" {}

			_FresnelPow("_FresnelPow", Float) = 1.58//Range(0.5,32)

				_FresnelFade("_FresnelFade", Float) = 0.7//Range(0.5,32)

				_FresnelAmount("_FresnelAmount", Float) = 4   //Range(0.5,32)

				_RefrDistortionZ("_RefrDistortionZ", Float) = 0   //Range(0.5,32)

				FRES_BLUR_OFF("FRES_BLUR_OFF", Float) = 0.3   //Range(0.5,32)

				FRES_BLUR_AMOUNT("FRES_BLUR_AMOUNT", Float) = 3   //Range(0.5,32)

				_BumpMixAmount("_BumpMixAmount", Float) = 0.5   //Range(0.5,32)

				_FastFresnelPow("_FastFresnelPow", Float) = 10   //Range(0.5,32)





			[Header(Reflection)]//////////////////////////////////////////////////

				_ReflectionAmount("_ReflectionAmount", Float) = 1//Range(0,2)

				_ReflectColor("_ReflectColor", COLOR) = (1,1,1,1)

				baked_ReflectionTex_distortion("baked_ReflectionTex_distortion", Float) = 15//Range(0,50)

				LOW_ReflectionTex_distortion("LOW_ReflectionTex_distortion", Float) = 0.1//Range(0,50)





				_ReflectionYOffset("_ReflectionYOffset", Float) = -0.15//Range(-0.4,0)

				_ReflectionLOD("_ReflectionLOD", Float) = 1//Range(0,4)



				[Header(ReflRefrMask)]//////////////////////////////////////////////////

				_ReflectionMask_Amount("_ReflectionMask_Amount", Float) = 3//Range(0, 5)

					_ReflectionMask_Offset("_ReflectionMask_Offset", Float) = 0.66//Range(0, 0.5)

					_ReflectionMask_UpClamp("_ReflectionMask_UpClamp", Float) = 1 //Range(0.5, 10)

				_ReflectionMask_Tiling("_ReflectionMask_Tiling", Float) = 0.1//Range(0, 2)

					_ReflectionBlurRadius("_ReflectionBlurRadius", Float) = 0.1//Range(0, 2)

					_ReflectionBlurZOffset("_ReflectionBlurZOffset", Float) = 0//Range(0, 2)

					_RefractionBlurZOffset("_RefractionBlurZOffset", Float) = 0//Range(0, 2)

					_ReflectionMask_TexOffsetF("_ReflectionMask_TexOffsetF", Vector) = (0,0,0,0)



					_ObjectScale("_ObjectScale", Vector) = (1,1,1,1)



					_AverageOffset("_AverageOffset", Float) = 0.5//Range(0, 2)

					_REFR_MASK_Tile("_REFR_MASK_Tile", Float) = 0.1//Range(0, 2)

					_REFR_MASK_Amount("_REFR_MASK_Amount", Float) = 3//Range(0, 5)

					_REFR_MASK_min("_REFR_MASK_min", Float) = 0.66//Range(0, 0.5)

					_REFR_MASK_max("_REFR_MASK_max", Float) = 1 //Range(0.5, 10)









				_UF_NMASK_Texture("_UF_NMASK_Texture", 2D) = "white" {}

				_UF_NMASK_Tile("_UF_NMASK_Tile", Float) = 0.1

					_UF_NMASK_offset("_UF_NMASK_offset", Vector) = (0,0,0,0)

					_UF_NMASK_Contrast("_UF_NMASK_Contrast", Float) = 1

					_UF_NMASK_Brightnes("_UF_NMASK_Brightnes", Float) = 0







				_AMOUNTMASK_Tile("_AMOUNTMASK_Tile", Float) = 0.1//Range(0, 2)

					_AMOUNTMASK_Amount("_AMOUNTMASK_Amount", Float) = 3//Range(0, 5)

					_AMOUNTMASK_min("_AMOUNTMASK_min", Float) = 0.66//Range(0, 0.5)

					_AMOUNTMASK_max("_AMOUNTMASK_max", Float) = 1 //Range(0.5, 10)



					_TILINGMASK_Tile("_TILINGMASK_Tile", Float) = 0.1//Range(0, 2)

					_TILINGMASK_Amount("_TILINGMASK_Amount", Float) = 3//Range(0, 5)

					_TILINGMASK_min("_TILINGMASK_min", Float) = 0.66//Range(0, 0.5)

					_TILINGMASK_max("_TILINGMASK_max", Float) = 1 //Range(0.5, 10)



					_MAINTEXMASK_Tile("_MAINTEXMASK_Tile", Float) = 0.1//Range(0, 2)

					_MAINTEXMASK_Amount("_MAINTEXMASK_Amount", Float) = 3//Range(0, 5)

					_MAINTEXMASK_min("_MAINTEXMASK_min", Float) = 0.66//Range(0, 0.5)

					_MAINTEXMASK_max("_MAINTEXMASK_max", Float) = 1 //Range(0.5, 10)

					_MAINTEXMASK_Blend("_MAINTEXMASK_Blend", Float) = 0.5 //Range(0.5, 10)



					_3Dwaves_BORDER_FACE("_3Dwaves_BORDER_FACE", Float) = 0

					_FixHLClamp("_FixHLClamp", Float) = 0.8

					_FastFresnelAmount("_FastFresnelAmount", Float) = 10

					_RefrDeepAmount("_RefrDeepAmount", Float) = 1

					_RefrTopAmount("_RefrTopAmount", Float) = 1

					_TexRefrDistortFix("_TexRefrDistortFix", Float) = 0

					_SpecularGlowAmount("_SpecularGlowAmount", Float) = 0.2

					_RefractionBLendAmount("_RefractionBLendAmount", Float) = 0.5



					_TILINGMASK_factor("_TILINGMASK_factor", Float) = 5 //Range(0.5, 10)

					_AMOUNTMASK_offset("_AMOUNTMASK_offset", Vector) = (0,0,0,0)

					_TILINGMASK_offset("_TILINGMASK_offset", Vector) = (0,0,0,0)

					_MAINTEXMASK_offset("_MAINTEXMASK_offset", Vector) = (0,0,0,0)

					_ReflectionMask_offset("_ReflectionMask_offset", Vector) = (0,0,0,0)

					_REFR_MASK_offset("_REFR_MASK_offset", Vector) = (0,0,0,0)

					_ReplaceColor("_ReplaceColor", COLOR) = (0.5, 0.5, 0.5, 1)





				[Header(Refraction)]//////////////////////////////////////////////////



				_RefrDistortion("_RefrDistortion", Float) = 100//Range(0, 600)

				_RefrAmount("_RefrAmount", Float) = 1//Range(0, 4)

				_RefrZColor("_RefrZColor", COLOR) = (0.29, 0.53, 0.608, 1)

					_RefrTopZColor("_RefrTopZColor", COLOR) = (0.89, 0.95, 1, 1)

					_RefrTextureFog("_RefrTextureFog", Float) = 0 //Range(0, 1)



					_RefrZOffset("_RefrZOffset", Float) = 1.8//Range(0, 2)

				_RefrZFallOff("_RefrZFallOff", Float) = 10 //Range(1, 20)

				_RefrBlur("_RefrBlur", Float) = 0.25 //Range(0, 1)

				_RefrRecover("_RefrRecover", Float) = 0.0 //Range(0, 1)

					_RefrDeepFactor("_RefrDeepFactor", Float) = 64 //Range(0, 1)

					_3dwanamnt("_3dwanamnt", Float) = 1 //Range(0, 1)







				[Header(Other)]//////////////////////////////////////////////////

				_LightDir("_LightDir", Vector) = (-0.5,0.46,0.88,0)





				_ObjecAngle("_ObjecAngle", Float) = 0 //Range(0, 1)







				_ReflectionTex_size("_ReflectionTex_size", Float) = 256.0 //Range(0, 1)

				_ReflectionTex_temp_size("_ReflectionTex_temp_size", Float) = 128.0 //Range(0, 1)



				_RefractionTex_size("_RefractionTex_size", Float) = 512.0 //Range(0, 1)

				[NoScaleOffset] _RefractionTex_temp("_RefractionTex_temp", 2D) = "black" {}

				_RefractionTex_temp_size("_RefractionTex_temp_size", Float) = 512.0 //Range(0, 1)



				_BakedData_size("_BakedData_size", Float) = 256.0 //Range(0, 1)

				[NoScaleOffset] _BakedData_temp("_BakedData_temp ", 2D) = "black" {}

				_BakedData_temp_size("_BakedData_temp_size", Float) = 256.0 //Range(0, 1)



					_MTDistortion("_MTDistortion", Float) = 0 //Range(1, 256)





					RIM_Minus("RIM_Minus", Float) = 0.4 //Range(1, 256)

					RIM_Plus("RIM_Plis", Float) = 50 //Range(1, 256)

					POSTRIZE_Colors("POSTRIZE_Colors", Float) = 12 //Range(1, 24)



					_MyNearClipPlane("_MyNearClipPlane", Float) = 0//Range(1, 256)

					_MyFarClipPlane("_MyFarClipPlane", Float) = 1000//Range(1, 256)



					_MultyOctaveNormals("_MultyOctaveNormals", Float) = 5//Range(1, 256)



					_SurfaceFoamAmount("_SurfaceFoamAmount", Float) = 10//Range(1, 256)

					_SurfaceFoamContrast("_SurfaceFoamContrast", Float) = 200//Range(1, 256)

					_SUrfaceFoamVector("_SUrfaceFoamVector", Vector) = (0.12065329648999294186660041025867, 0.98533525466827569191057001711249, 0.12065329648999294186660041025867, 0)





					_ReflectionBakeLayers("_ReflectionBakeLayers", Vector) = (255, 255, 255, 255)

					_RefractionBakeLayers("_RefractionBakeLayers", Vector) = (255, 255, 255, 255)

					_ZDepthBakeLayers("_ZDepthBakeLayers", Vector) = (255, 255, 255, 255)



					_DetileAmount("_DetileAmount", Float) = 0.15//Range(1, 256)

					_DetileFriq("_DetileFriq", Float) = 5//Range(1, 256)

					MainTexSpeed("MainTexSpeed", Float) = 1//Range(1, 256)

					_ReflectionDesaturate("_ReflectionDesaturate", Float) = 0.5//Range(1, 256)

					_RefractionDesaturate("_RefractionDesaturate", Float) = 0.5//Range(1, 256)



					_sinFriq("_sinFriq", Float) = 7.28//Range(1, 256)

					_sinAmount("_sinAmount", Float) = 0.1//Range(1, 256)





					_APPLY_REFR_TO_SPECULAR_DISSOLVE("_APPLY_REFR_TO_SPECULAR_DISSOLVE", Float) = 1

					_APPLY_REFR_TO_SPECULAR_DISSOLVE_FAST("_APPLY_REFR_TO_SPECULAR_DISSOLVE_FAST", Float) = 1







		  _UFSHORE_Amount_1("_UFSHORE_Amount_1", Float) = 5

		  _UFSHORE_Amount_2("_UFSHORE_Amount_2", Float) = 5

		  _UFSHORE_Length_1("_UFSHORE_Length_1", Float) = 10

		  _UFSHORE_Length_2("_UFSHORE_Length_2", Float) = 10

		  _UFSHORE_Color_1("_UFSHORE_Color_1", Color) = (1,1,1,1)

		  _UFSHORE_Color_2("_UFSHORE_Color_2", Color) = (1,1,1,1)

		  _UFSHORE_Distortion_1("_UFSHORE_Distortion_1", Float) = 0.8

		  _UFSHORE_Distortion_2("_UFSHORE_Distortion_2", Float) = 0.8

		  _UFSHORE_Tile_1("_UFSHORE_Tile_1", Vector) = (1,1,1,1)

		  _UFSHORE_Tile_2("_UFSHORE_Tile_2", Vector) = (1,1,1,1)

		  _UFSHORE_Speed_1("_UFSHORE_Speed_1", Float) = 1

		  _UFSHORE_Speed_2("_UFSHORE_Speed_2", Float) = 1

		  _UFSHORE_LowDistortion_1("_UFSHORE_LowDistortion_1", Float) = 1

		  _UFSHORE_LowDistortion_2("_UFSHORE_LowDistortion_2", Float) = 1

		  _UFSHORE_AlphaAmount_1("_UFSHORE_AlphaAmount_1", Float) = 0.25

		  _UFSHORE_AlphaAmount_2("_UFSHORE_AlphaAmount_2", Float) = 0.25

		  _UFSHORE_AlphaMax_1("_UFSHORE_AlphaMax_1", Float) = 1

		  _UFSHORE_AlphaMax_2("_UFSHORE_AlphaMax_2", Float) = 1

		  _UFSHORE_ShadowV1_1("_UFSHORE_ShadowV1_1", Float) = 1

		  _UFSHORE_ShadowV2_1("_UFSHORE_ShadowV2_1", Float) = 1

		  _UFSHORE_ShadowV1_2("_UFSHORE_ShadowV1_2", Float) = 1

		  _UFSHORE_ShadowV2_2("_UFSHORE_ShadowV2_2", Float) = 1

		  MAIN_TEX_FoamGradWavesSpeed_1("MAIN_TEX_FoamGradWavesSpeed_1", Float) = 0.1

		  MAIN_TEX_FoamGradWavesSpeed_2("MAIN_TEX_FoamGradWavesSpeed_2", Float) = 0.1

		  MAIN_TEX_FoamGradDirection_1("MAIN_TEX_FoamGradDirection_1", Float) = 0.01

		  MAIN_TEX_FoamGradDirection_2("MAIN_TEX_FoamGradDirection_2", Float) = 0.01

						MAIN_TEX_FoamGradTile_1("MAIN_TEX_FoamGradTile_1", Float) = 0.1
						MAIN_TEX_FoamGradTileYYY_1("MAIN_TEX_FoamGradTileYYY_1", Float) = 0.1

		  MAIN_TEX_FoamGradTile_2("MAIN_TEX_FoamGradTile_2", Float) = 0.1
						
		  _UFSHORE_ADDITIONAL_Length_1("_UFSHORE_ADDITIONAL_Length_1", Float) = 1

		  _UFSHORE_ADDITIONAL_Length_2("_UFSHORE_ADDITIONAL_Length_2", Float) = 1



		  _UFSHORE_ADD_Amount_1("_UFSHORE_ADD_Amount_1", Float) = 1

		  _UFSHORE_ADD_Amount_2("_UFSHORE_ADD_Amount_2", Float) = 1

		  _UFSHORE_ADD_Tile_1("_UFSHORE_ADD_Tile_1", Float) = 0.1

		  _UFSHORE_ADD_Tile_2("_UFSHORE_ADD_Tile_2", Float) = 0.1

		  _UFSHORE_ADD_Distortion_1("_UFSHORE_ADD_Distortion_1", Float) = 1

		  _UFSHORE_ADD_Distortion_2("_UFSHORE_ADD_Distortion_2", Float) = 1

		  _UFSHORE_ADD_LowDistortion_1("_UFSHORE_ADD_LowDistortion_1", Float) = 0.1

		  _UFSHORE_ADD_LowDistortion_2("_UFSHORE_ADD_LowDistortion_2", Float) = 0.1

		  _UFSHORE_ADD_Color_1("_UFSHORE_ADD_Color_1", Color) = (1,1,1,1)

		  _UFSHORE_ADD_Color_2("_UFSHORE_ADD_Color_2", Color) = (1,1,1,1)

		  SHORE_USE_ADDITIONALCONTUR_POW_Amount_1("SHORE_USE_ADDITIONALCONTUR_POW_Amount_1", Float) = 10

		  SHORE_USE_ADDITIONALCONTUR_POW_Amount_2("SHORE_USE_ADDITIONALCONTUR_POW_Amount_2", Float) = 10

		  _UFSHORE_UNWRAP_LowDistortion_1("_UFSHORE_UNWRAP_LowDistortion_1", Float) = 0



		  _LOW_DISTOR_Tile("_LOW_DISTOR_Tile", Float) = 20

		  _LOW_DISTOR_Speed("_LOW_DISTOR_Speed", Float) = 8

		  _LOW_DISTOR_Amount("_LOW_DISTOR_Amount", Float) = 1

					//_BAKED_DEPTH_EXTERNAL_TEXTURE_Amount("_BAKED_DEPTH_EXTERNAL_TEXTURE_Amount", Float ) = 1



					_LOW_DISTOR_MAINTEX_Tile("_LOW_DISTOR_MAINTEX_Tile", Float) = 20

					_LOW_DISTOR_MAINTEX_Speed("_LOW_DISTOR_MAINTEX_Speed", Float) = 8

					MAIN_TEX_MixOffset("MAIN_TEX_MixOffset", Float) = 0





					_FoamAmount_SW("_FoamAmount_SW", Float) = 5 //Range(0,10)

					  _FoamLength_SW("_FoamLength_SW", Float) = 10 //Range(0,20)

					  _FoamColor_SW("_FoamColor_SW", COLOR) = (1,1,1,1)

						  _FoamDistortion_SW("_FoamDistortion_SW", Float) = 1 //Range(0,10)

						  _FoamDistortionFade_SW("_FoamDistortionFade_SW", Float) = 0.8 //Range(0,1)

						  _FoamDirection_SW("_FoamDirection_SW", Float) = 0.05 //Range(0,1)

				  _WaterfrontFade_SW("_WaterfrontFade_SW", Float) = 0//Range(0,30) 

				  _LightNormalsFactor("_LightNormalsFactor", Float) = 1//Range(0,30) 





					  _FoamTextureTiling_SW("_FoamTextureTiling_SW", Float) = 1 //Range(0.01,2)

					  _FoamWavesSpeed_SW("_FoamWavesSpeed_SW", Float) = 0.15//Range(0,2 )

						  [NoScaleOffset] _ShoreWavesGrad("_ShoreWavesGrad",  2D) = "white" {}

					  _ShoreWavesRadius("_ShoreWavesRadius", Float) = 0.1//Range(0,2 )

					  _ShoreWavesQual("_ShoreWavesQual", Float) = 2//Range(0,2 )

					  _FoamLShoreWavesTileY_SW("_FoamLShoreWavesTileY_SW", Float) = 1//Range(0,2 )

					  _FoamLShoreWavesTileX_SW("_FoamLShoreWavesTileX_SW", Float) = 1//Range(0,2 )

					  _FoamMaskOffset_SW("_FoamMaskOffset_SW", Float) = 0.0//Range(0,2 )



					  _BumpZOffset("_BumpZOffset", Float) = 1.0//Range(0,2 )

					  _BumpZFade("_BumpZFade", Float) = 0//Range(0,2 )

					  _RefractionBlendOffset("_RefractionBlendOffset", Float) = 0.0//Range(0,2 )

					  _RefractionBlendFade("_RefractionBlendFade", Float) = 1//Range(0,2 )



					  _RefrBled_Fres_Amount("_RefrBled_Fres_Amount", Float) = 1//Range(0,2 )

					  _RefrBled_Fres_Pow("_RefrBled_Fres_Pow", Float) = 1//Range(0,2 )





					  _LutAmount("_LutAmount", Float) = 1.0//Range(0,2 )



					  _GAmplitude("Wave Amplitude", Vector) = (0.3 ,0.35, 0.25, 0.25)

					  _GFrequency("Wave Frequency", Vector) = (1.3, 1.35, 1.25, 1.25)

					  _GSteepness("Wave Steepness", Vector) = (1.0, 1.0, 1.0, 1.0)

					  _GSpeed("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)

					  _GDirectionAB("Wave Direction", Vector) = (0.3 ,0.85, 0.85, 0.25)

					  _GDirectionCD("Wave Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)

						  //_GAmplitude("Wave Amplitude", Vector) = 1

						  //	_GFrequency("Wave Frequency", Vector) = 1

						  //	_GSteepness("Wave Steepness", Vector) = 1

						  //	_GSpeed("Wave Speed", Vector) = 1

						  //	_GDirectionAB("Wave Direction", Vector) =(0.3 ,0.85, 0.85, 0.25)

						  //

								  //	_CameraFarClipPlane("_CameraFarClipPlane", FLOAT) = 4

								  //_LightPos("_LightPos", Vector) = (2001,1009,3274,0)





							  _FoamDistortionTexture("_FoamDistortionTexture", Float) = 1

							  _ShoreDistortionTexture("_ShoreDistortionTexture", Float) = 1



							  _MOR_Offset("_MOR_Offset", Float) = 1

							  _MOR_Base("_MOR_Base", Float) = 1

							  _C_BLUR_R("_C_BLUR_R", Float) = 0.05

							  _C_BLUR_S("_C_BLUR_S", Float) = 0.05

							  _MOR_Tile("_MOR_Tile", Float) = 82



							  _FracTimeX("_FracTimeX", Float) = 0

							  _FracTimeW("_FracTimeW", Float) = 0



							  _WaveGrad0("_WaveGrad0", COLOR) = (1,1,1,1)

							  _WaveGrad1("_WaveGrad1", COLOR) = (1,1,1,1)

							  _WaveGrad2("_WaveGrad2", COLOR) = (1,1,1,1)

								  _WaveGradMidOffset("_WaveGradMidOffset", Float) = 0.5

								  _WaveGradTopOffset("_WaveGradTopOffset", Float) = 1





							  _SFW_Tile("_SFW_Tile", Vector) = (1,1,1,1)

								  _SFW_Speed("_SFW_Speed", Vector) = (1,1,1,1)

								  _SFW_Amount("_SFW_Amount", Float) = 1

								  _SFW_NrmAmount("_SFW_NrmAmount", Float) = 1

								  _SFW_Dir("_SFW_Dir", Vector) = (1,0,0,0)

								  _SFW_Dir1("_SFW_Dir1", Vector) = (0.9,0,0.1,0)

								  _SFW_Distort("_SFW_Distort", Float) = 1









						  _CAUSTIC_FOG_Amount("_CAUSTIC_FOG_Amount", Float) = 1 //Range(0.1,32)

						  _CAUSTIC_FOG_Pow("_CAUSTIC_FOG_Pow", Float) = 4 //Range(0.1,32)

							  _CAUSTIC_Speed("_CAUSTIC_Speed", Float) = 1 //Range(0.1,32)

							  //_CAUSTIC_Tiling("_CAUSTIC_Tiling", Float ) = 1 //Range(0.1,32)

							  _CAUSTIC_Tiling("_CAUSTIC_Tiling", Vector) = (1,1,0,0)

							  _CAUSTIC_Offset("_CAUSTIC_Offset", Vector) = (1,2,1,0)



							  _CAUSTIC_PROC_Tiling("_CAUSTIC_PROC_Tiling", Vector) = (1,1,1,1)

							  _CAUSTIC_PROC_GlareSpeed("_CAUSTIC_PROC_GlareSpeed", Float) = 1 //Range(0.1,32)

							  _CAUSTIC_PROC_Contrast("_CAUSTIC_PROC_Contrast", Float) = 1 //Range(0.1,32)

							  _CAUSTIC_PROC_BlackOffset("_CAUSTIC_PROC_BlackOffset", Float) = 1 //Range(0.1,32)





							  DOWNSAMPLING_SAMPLE_SIZE("DOWNSAMPLING_SAMPLE_SIZE", Float) = 0.33









							  _CN_DISTANCE("_CN_DISTANCE", Float) = 200

							  _CN_TEXEL("_CN_TEXEL", Float) = 0.01

							  _CLASNOISE_PW("_CLASNOISE_PW", Float) = 0.35

							  _CN_AMOUNT("_CN_AMOUNT", Float) = 5

							  _CN_SPEED("_CN_SPEED", Float) = 2

							  _CN_TILING("_CN_TILING", Vector) = (5,5,1,1)





	}



		SubShader{




			 Tags{ "Queue" = "Geometry" "RenderType" = "Opaque"   "PreviewType" = "Plane"  "IgnoreProjector" = "False" }
	Lighting Off


						  //Cull Off								

				  Pass{
				  CGPROGRAM

							  #pragma vertex vert
							  #pragma fragment frag
							  #pragma multi_compile_fog
							  #define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))

				  #define USE_FAKE_LIGHTING = 1;
				  #define TILEMASKTYPE_TILE = 1;
				  #define GRAD_G = 1;
				  #define SKIP_FOAM_COAST_ALPHA_FAKE = 1;
				  #define REFR_MASK_A = 1;
				  #define RRSIMPLEBLEND = 1;
				  #define MAINTEXLERPBLEND = 1;
				  #define SKIP_FOAM = 1;
				  #define SKIP_SURFACE_FOAMS = 1;
				  #define SKIP_MAINTEXTURE = 1;
				  #define USE_OUTPUT_SHADOWS = 1;
				  #define REFLECTION_MASK_A = 1;
				  #define SKIP_SHORE_BORDERS = 1;
				  #define USE_SURFACE_GRADS = 1;
				  #define ULTRA_FAST_MODE = 1;
				  #define REFLECTION_BLUR_1 = 1;
				  #define REFRACTION_BLUR_1 = 1;
				  #define SHORE_WAVES = 1;
				  #define FORCE_OPAQUE = 1;
				  #define FIX_OVEREXPO = 1;
				  #define LIGHTING_BLEND_SIMPLE = 1;
				  #define SKIP_AMBIENT_COLOR = 1;
				  #define MAINTEX_HAS_MOVE = 1;
				  #define SHORE_UNWRAP_STRETCH_DOT = 1;
				  #define BAKED_DEPTH_ONAWAKE = 1;
				  #define RIM_INVERSE = 1;
				  #define REFLECTION_BLUR = 1;
				  #define null = 1;
				  #define MAIN_TEX_ADDDISTORTION_THAN_MOVE = 1;
				  #define FIX_HL = 1;
				  #define UFAST_SHORE_1 = 1;
				  #define MAIN_TEX_LowDistortion_1 = 1;
				  #define SKIP_MAIN_TEXTURE = 1;
				  #define SIN_OFFSET = 1;
				  #define USE_REFRACTION_BLEND_FRESNEL = 1;
				  #define HAS_WAVES_ROTATION = 1;
				  #define SKIP_BLEND_ANIMATION = 1;
				  #define SKIP_FLAT_SPECULAR_CLAMP = 1;
				  #define SKIP_REFLECTION_BLUR_ZDEPENDS = 1;
				  #define GRAD_1 = 1;
				  #define USE_OUTPUT_BLEND_2 = 1;
				  #define VERTEX_ANIMATION_BORDER_NONE = 1;
				  #define REFLECTION_NONE = 1;
				  #define UF_AMOUNTMASK = 1;
				  #define SHORE_USE_LOW_DISTORTION_1 = 1;
				  #define _UFSHORE_UNWRAP_Low = 1;
				  #define SHORE_USE_ADDITIONAL_GRAD_TEXTURE = 1;
				  #define DETILE_NONE = 1;
				  #define SKIP_LIGHTING = 1;
				  #define USE_REFR_DISTOR = 1;
				  #define WAW3D_NORMAL_CALCULATION = 1;
				  #define SKIP_3DVERTEX_HEIGHT_COLORIZE = 1;
				  #define USE_DEPTH_FOG = 1;
				  #define USE_lerped_post_Color_1 = 1;
				  #define USE_lerped_post_Color_2 = 1;
				  #define REFRACTION_BAKED_ONAWAKE = 1;
				  #define SHORE_SHADDOW_1 = 1;
				  #define FOAM_ALPHA_FLAT = 1;
				  #define SHORE_USE_WAVES_GRADIENT_1 = 1;
				  #define USE_REFR_LOW_DISTOR = 1;
				  #define WAVES_MAP_NORMAL = 1;

				  #pragma fragmentoption ARB_precision_hint_fastest
							  #pragma multi_compile ORTO_CAMERA_ON _
							  #pragma multi_compile WATER_DOWNSAMPLING WATER_DOWNSAMPLING_HARD _
							 #pragma target 2.0






				  #define MYFLOAT float
				  #define MYFLOAT2 float2
				  #define MYFLOAT3 float3
				  #define MYFLOAT4 float4




				  #if defined(MINIMUM_MODE) || defined(ULTRA_FAST_MODE)
				  #define MYFIXED fixed
				  #define MYFIXED2 fixed2
				  #define MYFIXED3 fixed3
				  #define MYFIXED4 fixed4
				  #else
				  #endif


















				  #if !defined(ULTRA_FAST_MODE)  && !defined(MINIMUM_MODE)
				  #endif
				  #if  defined(REFLECTION_2D) || defined(REFLECTION_PLANAR)||defined(REFLECTION_PROBE_AND_INTENSITY)||defined(REFLECTION_PROBE_AND_INTENSITY)||defined(REFLECTION_PROBE)|| defined(REFLECTION_USER) || defined(REFLECTION_JUST_COLOR)
				  #endif




				  uniform MYFIXED _ObjecAngle;
				  #if !defined(ULTRA_FAST_MODE)  && !defined(MINIMUM_MODE)
				  #endif
				  MYFIXED _Frac2PITime;
				  MYFIXED _Frac01Time;
				  MYFIXED _Frac01Time_d8_mBlendAnimSpeed;
				  MYFIXED _Frac01Time_MAIN_TEX_BA_Speed;
				  float2 _FracWTime_m4_m3DWavesSpeed_dPI2;
				  MYFIXED2 _Frac_UFSHORE_Tile_1Time_d10_m_UFSHORE_Speed1;
				  MYFIXED2 _Frac_UFSHORE_Tile_2Time_d10_m_UFSHORE_Speed2;
				  MYFIXED _Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1;


				  float2 _Frac_WaterTextureTilingTime_m_AnimMove;
				  float4 _Frac_UVS_DIR;

				  float2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move;
				  float2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed;
				  float2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed;



				  uniform MYFIXED _LOW_DISTOR_Tile;
				  uniform MYFIXED _LOW_DISTOR_Speed;
				  uniform MYFIXED _LOW_DISTOR_Amount;

				  uniform MYFIXED DOWNSAMPLING_SAMPLE_SIZE;
				  float _FrameRate;
				  sampler2D _FrameBuffer;
				 



				  uniform MYFIXED _BumpMixAmount;
				  uniform MYFIXED _Z_BLACK_OFFSET_V;


				  #if !defined(USING_FOG) && !defined(SKIP_FOG)
				  #define SKIP_FOG = 1
				  #endif




				  #if defined(REFRACTION_BAKED_FROM_TEXTURE) || defined(REFRACTION_BAKED_ONAWAKE) || defined(REFRACTION_BAKED_VIA_SCRIPT)
				  #define HAS_BAKED_REFRACTION = 1
				  #endif


				  #if defined(SURFACE_FOG)
				  #endif


				  #if defined(REFRACTION_GRABPASS) || defined(HAS_BAKED_REFRACTION) || defined(REFRACTION_ONLYZCOLOR)
				  #if !defined(DEPTH_NONE)
				  #define HAS_REFRACTION = 1
				  #endif
				  #define REQUEST_DEPTH = 1
				  #endif

				  #if (defined(REFRACTION_Z_BLEND) || defined(REFRACTION_Z_BLEND_AND_FRESNEL))&& !defined(DEPTH_NONE)
				  #endif

				  #if defined(RRFRESNEL) || defined(HAS_REFRACTION_Z_BLEND_AND_RRFRESNEL)
				  #endif

				  #if !defined(DEPTH_NONE)
				  uniform MYFIXED _RefrDistortionZ;
				  #endif




				  #if defined(USE_OUTPUT_GRADIENT) &&( defined(USE_OUTPUT_BLEND_1) || !defined(USE_OUTPUT_BLEND_3))
				  #endif

				  #if !defined(SKIP_3DVERTEX_ANIMATION) && (defined(VERTEX_ANIMATION_BORDER_FADE) )
				  #endif
				  #if !defined(SKIP_3DVERTEX_ANIMATION)
				  #if defined(USE_SIMPLE_VHEIGHT_FADING)
				  #endif
				  #endif






				  #if defined(DEPTH_NONE)
				  #elif defined(BAKED_DEPTH_ONAWAKE) || defined(BAKED_DEPTH_VIASCRIPT) || defined(BAKED_DEPTH_EXTERNAL_TEXTURE)
				  #define HAS_BAKED_DEPTH = 1
				  #else
				  #endif






				  #if !defined(SKIP_FOAM)
				  #endif
				  #if defined(DEPTH_NONE)
				  #endif


				  #if defined(RRSIMPLEBLEND) || defined(RRMULTIBLEND)
				  uniform MYFIXED _AverageOffset;
				  #endif



				  #if defined(REFLECTION_NONE) && !defined(SKIP_REFLECTION_MASK)
				  #define SKIP_REFLECTION_MASK = 1
				  #endif


				  #if defined(HAS_CAMERA_DEPTH) || defined(HAS_BAKED_DEPTH)|| defined(NEED_SHORE_WAVES_UNPACK) || defined(SHORE_WAVES) && !defined(DEPTH_NONE) || defined(USE_CAUSTIC) || defined(SURFACE_FOG)
				  #define USE_WPOS = 1


				  #endif


				  #include "UnityCG.cginc"
				  #if !defined(SKIP_LIGHTING) || !defined(SKIP_SPECULAR) || !defined(SKIP_FLAT_SPECULAR)
				  #if !defined(USE_FAKE_LIGHTING)
				  #endif
				  #endif


				  #if defined(USE_SHADOWS)
				  #endif


				  #if !defined(SKIP_FOAM_FINE_REFRACTIOND_DOSTORT) && !defined(SKIP_FOAM)
				  #endif


				  #if defined(HAS_BAKED_REFRACTION)
				  uniform MYFIXED _RefractionBakeLayers;
				  #endif
				  #if defined(HAS_BAKED_DEPTH)
				  uniform MYFIXED _ZDepthBakeLayers;
				  #endif




				  #if defined(HAS_REFRACTION) && defined(REFR_MASK)
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
				  #endif
				  #endif

				  #if defined(HAS_BAKED_REFRACTION)
				  #if defined(REFRACTION_BAKED_FROM_TEXTURE)
				  #else
				  uniform sampler2D _RefractionTex_temp;
				  #endif
				  #endif

				  #if !defined(SKIP_REFRACTION_CALC_DEPTH_FACTOR) || !defined(SKIP_Z_CALC_DEPTH_FACTOR)
				  uniform MYFIXED _RefrDeepFactor;
				  #endif

				  #if defined(HAS_CAMERA_DEPTH)
				  #endif

				  #if defined(USE_OUTPUT_GRADIENT)
				  #endif

				  uniform MYFIXED4 _MainTexColor;

				  #if defined(TRANSPARENT_LUMINOSITY)
				  #endif
				  #if defined(TRANSPARENT_POW)
				  #endif
				  #if defined(TRANSPARENT_SIMPLE)
				  #endif

				  #if defined(MULTI_OCTAVES)
				  #endif


				  uniform MYFIXED2 _AnimMove;
				  uniform MYFIXED _MainTexAngle;
				  uniform sampler2D _MainTex;
				  uniform MYFIXED4 _MainTex_ST;
				  #if !defined(SKIP_MAINTEXTURE) || defined(USE_NOISED_GLARE_PROCEDURALHQ) || !defined(SKIP_REFLECTION_MASK) || defined(HAS_REFR_MASK)
				  #endif
				  #if !defined(SKIP_MAINTEXTURE)
				  #endif

				  uniform MYFIXED4 _WaterTextureTiling;

				  uniform sampler2D _Utility;

				  uniform MYFIXED _BumpAmount;

				  uniform sampler2D _BumpMap;


				  #if defined(BAKED_DEPTH_EXTERNAL_TEXTURE)
				  #else
				  uniform sampler2D _BakedData_temp;
				  #endif


				  #if !defined(SKIP_NOISED_GLARE_HQ) || defined(USE_NOISED_GLARE_ADDWAWES2) || defined(USE_NOISED_GLARE_ADDWAWES1) || defined(USE_NOISED_GLARE_LQ)  || defined(USE_NOISED_GLARE_PROCEDURALHQ)


				  #endif


				  #if defined(REFRACTION_GRABPASS)
				  #endif
				  #if defined(HAS_REFRACTION)
				  uniform MYFIXED _RefrDistortion;

				  #if defined(USE_CAUSTIC) && !defined(ULTRA_FAST_MODE) && !defined(MINIMUM_MODE)
				  #endif


				  #if defined(WAVES_GERSTNER)
				  #endif



				  #if defined(HAS_NOISE_TEX)
				  #endif



				  #if defined(USE_REFR_LOW_DISTOR)
				  uniform MYFIXED	_RefrLowDist;
				  #endif

				  uniform MYFIXED	_RefrTopAmount;
				  uniform MYFIXED	_RefrDeepAmount;
				  #if !defined(ULTRA_FAST_MODE) && !defined(MINIMUM_MODE)
				  #endif

				  uniform MYFIXED	_TexRefrDistortFix;

				  uniform MYFIXED3	_RefrTopZColor;
				  uniform MYFIXED3	_RefrZColor;
				  uniform MYFIXED	_RefrRecover;
				  uniform MYFIXED	_RefrZOffset;
				  uniform MYFIXED _RefrZFallOff;

				  #if defined(REFRACTION_BLUR)
				  #endif

				  #if defined(USE_REFRACTION_BLEND_FRESNEL) && defined(REFLECTION_NONE)
				  #endif
				  uniform MYFIXED _RefractionBLendAmount;

				  #endif

				  #if defined(USE_OUTPUT_GRADIENT)
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


				  #if  !defined(SKIP_3DVERTEX_ANIMATION) && !defined(SKIP_3DVERTEX_HEIGHT_COLORIZE)
				  #endif
				  uniform MYFIXED _VERTEX_ANIM_DETILEAMOUNT;
				  uniform MYFIXED _VERTEX_ANIM_DETILESPEED;
				  #endif

				  #if defined(USE_LIGHTMAPS)
				  #endif

				  #if defined(USE_OUTPUT_SHADOWS)
				  uniform MYFIXED _OutShadowsAmount;
				  #endif





				  #if defined(POSTRIZE)
				  #endif

				  #if defined(RIM)
				  #endif





				  #if !defined(REFLECTION_NONE)
				  #endif



				  #if !defined(SKIP_FRESNEL_CALCULATION)
				  uniform MYFIXED _FresnelFade;
				  uniform MYFIXED _FresnelAmount;
				  uniform MYFIXED _FresnelPow;
				  #endif
				  #if !defined(SKIP_FRESNEL_CALCULATION) && defined(USE_FRESNEL_POW)
				  #endif


				  #if !defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR) || !defined(SKIP_FLAT_SPECULAR)
				  uniform MYFIXED3 _LightDir;

				  #endif
				  #if !defined(SKIP_LIGHTING)
				  #endif
				  #if !defined(SKIP_SPECULAR)
				  uniform MYFIXED _SpecularAmount;
				  uniform MYFIXED _SpecularShininess;
				  uniform MYFIXED _SpecularGlowAmount;
				  #endif
				  #if !defined(SKIP_FLAT_SPECULAR)
				  uniform MYFIXED _FlatSpecularAmount;
				  uniform MYFIXED _FlatSpecularShininess;
				  uniform MYFIXED _FlatSpecularClamp;
				  uniform MYFIXED _FlatFriqX;
				  uniform MYFIXED _FlatFriqY;
				  uniform MYFIXED _Light_FlatSpecTopDir;

				  #if defined(USE_FLAT_HQ)
				  #endif

				  #endif



				  #if !defined(SKIP_REFLECTION_MASK)
				  #endif


				  #if (defined(HAS_FOAM) || defined(SHORE_WAVES)) && !defined(ULTRA_FAST_MODE) && !defined(MINIMUM_MODE)
				  #endif



				  #if !defined(SKIP_FOAM)
				  #endif

				  #if !defined(SKIP_SURFACE_FOAMS) && !defined(ADVANCE_PC)
				  #endif

				  #ifdef ORTO_CAMERA_ON
				  uniform MYFIXED _MyNearClipPlane;
				  uniform MYFIXED _MyFarClipPlane;
				  #endif


				  #if defined(DETILE_HQ) || defined(DETILE_LQ)
				  #endif

				  #if defined(HAS_REFRACTION) && !defined(REFRACTION_ONLYZCOLOR)
				  uniform MYFIXED _RefrTextureFog;
				  #endif


				  #if(defined(APPLY_REFR_TO_SPECULAR) || defined(_APPLY_REFR_TO_TEX_DISSOLVE_FAST) )&& defined(HAS_REFRACTION)
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
				  #endif
				  #endif


				  #if defined(USE_SURFACE_GRADS)

				  #endif

				  #if defined(WAVES_GERSTNER)&& !defined(ADVANCE_PC)
				  #endif

				  uniform MYFIXED3 _ObjectScale;

				  #if defined(Z_AFFECT_BUMP) && !defined(DEPTH_NONE)
				  #endif



				  #if defined(USE_LUT)
				  #endif




				  #if  defined (ADDITIONAL_FOAM_DISTORTION) && defined(HAS_REFRACTION) &&  !defined(SKIP_FOAM)
				  #endif


				  #if defined(ZDEPTH_CALCANGLE)
				  #endif



				  #if defined(SURFACE_WAVES)&& !defined(ADVANCE_PC)
				  #endif


				  #if !defined(SKIP_BLEND_ANIMATION)
				  #endif


				  #if defined(USE_FAST_FRESNEL)
				  #endif



				  #if defined(SKIP_MAIN_TEXTURE)

				  #define NO_MAINTTEX = 1
				  #else
				  #endif

				  #if defined(POST_TEXTURE_TINT)
				  #endif



				  uniform MYFIXED3 _ReplaceColor;



				  #if defined(UFAST_SHORE_1)
				  #if defined(SHORE_USE_WAVES_GRADIENT_1)
				  uniform MYFIXED MAIN_TEX_FoamGradWavesSpeed_1;
				  uniform MYFIXED MAIN_TEX_FoamGradDirection_1;
				  uniform MYFIXED MAIN_TEX_FoamGradTile_1;
				  uniform MYFIXED MAIN_TEX_FoamGradTileYYY_1;
				  #endif
				  #if defined(SHORE_USE_ADDITIONALCONTUR_1)
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
				  #endif




				  #if defined(UFAST_SHORE_2)
				  #endif
































/*
{

    Properties{
    
    
    
        _FrameRate("_FrameRate", Float) = 0
        
        
        
        _FracTimeFull("_FracTimeFull", Float) = 0
        
        _Frac2PITime("_Frac2PITime", Float) = 0
        
        _Frac01Time("_Frac01Time", Float) = 0
        
        _Frac01Time_d8_mBlendAnimSpeed("_Frac01Time_d8_mBlendAnimSpeed", Float) = 0
        
        _Frac01Time_MAIN_TEX_BA_Speed("_Frac01Time_MAIN_TEX_BA_Speed", Float) = 0
        
        _FracWTime_m4_m3DWavesSpeed_dPI2("_FracWTime_m4_m3DWavesSpeed_dPI2", Vector) = (0, 0, 0, 0)
        
        _Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1("_Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1", Float) = 0
        
        _Frac_UFSHORE_Tile_1Time_d10("_Frac_UFSHORE_Tile_1Time_d10", Vector) = (0, 0, 0, 0)
        
        _Frac_UFSHORE_Tile_2Time_d10("_Frac_UFSHORE_Tile_2Time_d10", Vector) = (0, 0, 0, 0)
        
        _Frac_UFSHORE_Tile_2Time_d10("_Frac_UFSHORE_Tile_2Time_d10", Vector) = (0, 0, 0, 0)
        
        _Frac_WaterTextureTilingTime_m_AnimMove("_Frac_WaterTextureTilingTime_m_AnimMove", Vector) = (0, 0, 0, 0)
        
        _Frac_UVS_DIR("_Frac_UVS_DIR", Vector) = (0, 0, 0, 0)
        
        _FracMAIN_TEX_TileTime_mMAIN_TEX_Move("_FracMAIN_TEX_TileTime_mMAIN_TEX_Move", Vector) = (0, 0, 0, 0)
        
        _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed("_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed", Vector) = (0, 0, 0, 0)
        
        _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed("_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed", Vector) = (0, 0, 0, 0)
        
        
        
        _MM_Tile("_MM_Tile", Vector) = (1, 1, 1, 1)
        
        _MM_offset("_MM_offset", Vector) = (0, 0, 0, 0)
        
        _MM_Color("_MM_Color", COLOR) = (1, 1, 1, 1)
        
        _ReflectionJustColor("_ReflectionJustColor", COLOR) = (1, 1, 1, 1)
        
        
        
        MAIN_TEX_Bright("MAIN_TEX_Bright", Float) = 0
        
        
        
        lerped_post_offset("lerped_post_offset", Float) = 0
        
        lerped_post_offset_falloff("lerped_post_offset_falloff", Float) = 1
        
        lerped_post_color1("lerped_post_color1", COLOR) = (1, 1, 1, 1)
        
        lerped_post_color2("lerped_post_color2", COLOR) = (1, 1, 1, 1)
        
        
        
        [Header(Texture)]//////////////////////////////////////////////////
        
        [NoScaleOffset] _MainTex("Additional Color Texture(RGB) Mask for reflection(A)", 2D) = "black" {}
        
        _MainTexAngle("_MainTexAngle", Float) = 0 //Range(0.1,10)
        
        _MainTexTile("_MainTexTile", Vector) = (1, 1, 0, 0)
        
        _MainTexColor("Tint Color (RGB) Amount Texture (A)", COLOR) = (1, 1, 1, 1)
        
        _OutGradBlend("_OutGradBlend", Float) = 1 //Range(0.1,10)
        
        _OutShadowsAmount("_OutShadowsAmount", Float) = 20 //Range(0.1,10)
        
        _OutGradZ("_OutGradZ", Float) = 1 //Range(0.1,10)
        
        _UFSHORE_UNWRAP_Transparency("_UFSHORE_UNWRAP_Transparency", Float) = 0 //Range(0.1,10)
        
        
        
        _AnimMove("_AnimMove", Vector) = (0, 0, 0, 0)
        
        
        
        
        
        _VERTEX_ANIM_DETILESPEED("_VERTEX_ANIM_DETILESPEED", Float) = 0
        
        _VERTEX_ANIM_DETILEAMOUNT("_VERTEX_ANIM_DETILEAMOUNT", Float) = 1
        
        
        
        
        
        
        
        MAINTEX_VHEIGHT_Amount("MAINTEX_VHEIGHT_Amount", Float) = 1
        
        MAINTEX_VHEIGHT_Offset("MAINTEX_VHEIGHT_Offset", Float) = 0.8
        
        
        
        _FoamBlendOffset("_FoamBlendOffset", Float) = 0
        
        
        
        [Header(Commons)]//////////////////////////////////////////////////
        
        _TransparencyPow("_TransparencyPow", Float) = 1 //Range(0,2)
        
        _TransparencyLuminosity("_TransparencyLuminosity", Float) = 1 //Range(0,2)
        
        _TransparencySimple("_TransparencySimple", Float) = 1 //Range(0,2)
        
        
        
        
        
        _WaterTextureTiling("_Tiling [x,y]; Speed[z];", Vector) = (1, 1, 1, -1)
        
        _BumpAmount("_BumpAmount", Float) = 1 //Range(0,1)
        
        [NoScaleOffset] _BumpMap("_BumpMap ", 2D) = "bump" {}
        
        [NoScaleOffset] _MM_Texture("_MM_Texture ", 2D) = "bump" {}
        
        _MM_MultyOffset("_MM_MultyOffset", Float) = 0.3
        
        
        
        [Header(Specular)]//////////////////////////////////////////////////
        
        _LightAmount("_LightAmount", Float ) = 1//Range(0,2)
        
        _SpecularAmount("_SpecularAmount", Float) = 2 //Range(0,10)
        
        _SpecularShininess("_SpecularShininess", Float) = 48 //Range(0,512)
        
        _FlatSpecularAmount("_FlatSpecularAmount", Float ) = 1.0//Range(0,10)
        
        _FlatSpecularShininess("_FlatSpecularShininess", Float ) = 48//Range(0,512)
        
        _FlatSpecularClamp("_FlatSpecularClamp", Float  ) = 10//Range(0,100)
        
        _FlatFriqX("_FlatFriqX", Float) = 1//Range(0,100)
        
        _FlatFriqY("_FlatFriqY", Float) = 1//Range(0,100)
        
        
        
        _LightColor0Fake("_LightColor0Fake", COLOR) = (1, 1, 1, 1)
        
        _BlendColor("_BlendColor", COLOR) = (1, 1, 1, 1)
        
        
        
        [Header(Foam)]//////////////////////////////////////////////////
        
        _FoamAmount("_FoamAmount", Float ) = 5 //Range(0,10)
        
        _FoamLength("_FoamLength", Float ) = 10 //Range(0,20)
        
        _FoamColor("_FoamColor", COLOR) = (1, 1, 1, 1)
        
        _FoamDistortion("_FoamDistortion", Float ) = 1 //Range(0,10)
        
        _FoamDistortionFade("_FoamDistortionFade", Float) = 0.8 //Range(0,1)
        
        _FoamDirection("_FoamDirection", Float) = 0.05 //Range(0,1)
        
        _FixMulty("_FixMulty", Float) = 1 //Range(0,1)
        
        _FoamAlpha2Amount("_FoamAlpha2Amount", Float ) = 0.5 //Range(0,1)
        
        
        
        _BlendAnimSpeed("_BlendAnimSpeed", Float) = 1 //Range(0,1)
        
        
        
        _WaterfrontFade("_WaterfrontFade", Float ) = 10//Range(0,30)
        
        
        
        
        
        _FoamTextureTiling("_FoamTextureTiling", Float ) = 1 //Range(0.01,2)
        
        _FoamWavesSpeed("_FoamWavesSpeed", Float ) = 0.15//Range(0,2 )
        
        _FoamOffsetSpeed("_FoamOffsetSpeed", Float) = 0.2//Range(0,4 )
        
        _FoamOffset("_FoamOffset", Float) = 0//Range(0,4 )
        
        
        
        
        
        _MultyOctavesSpeedOffset("_MultyOctavesSpeedOffset", Float) = 0.89
        
        _MultyOctavesTileOffset("_MultyOctavesTileOffset", Vector) = (1.63, 1.63, 1, 1)
        
        _FadingFactor("_FadingFactor", Float) = 0.5
        
        FLAT_HQ_OFFSET("FLAT_HQ_OFFSET", Float) = 50
        
        
        
        
        
        [Header(3D Waves)]//////////////////////////////////////////////////
        
        _3DWavesTile("_3DWavesTile", Vector) = (1, 1, -1, -1)
        
        //_3DWavesSpeed("_3DWavesSpeed",  Vector) = (0.1 ,0.1, 0.1, 0.1)
        
        _3DWavesSpeed("_3DWavesSpeed",  Float) = 0.1
        
        _3DWavesSpeedY("_3DWavesSpeedY",  Float) = 0.1
        
        _3DWavesHeight("_3DWavesHeight", Float  ) = 1//Range(0.1,32)
        
        _3DWavesWind("_3DWavesWind", Float  ) = 0.1//Range(0.1,32)
        
        _3DWavesYFoamAmount("_3DWavesYFoamAmount", Float  ) = 0.02//Range(0,10)
        
        _3DWavesTileZ("_3DWavesTileZ", Float ) = 1 //Range(0.1,32)
        
        _3DWavesTileZAm("_3DWavesTileZAm", Float ) = 1 //Range(0.1,32)
        
        ADDITIONAL_FOAM_DISTORTION_AMOUNT("ADDITIONAL_FOAM_DISTORTION_AMOUNT", Float ) = 1 //Range(0.1,32)
        
        
        
        _SURFACE_FOG_Amount("_SURFACE_FOG_Amount", Float ) = 1 //Range(0.1,32)
        
        _SURFACE_FOG_Speed("_SURFACE_FOG_Speed", Float ) = 1 //Range(0.1,32)
        
        _SURFACE_FOG_Tiling("_SURFACE_FOG_Tiling", Float ) = 1 //Range(0.1,32)
        
        
        
        _Light_FlatSpecTopDir("_Light_FlatSpecTopDir",  Float ) = 0.5
        
        MAIN_TEX_Multy("MAIN_TEX_Multy",  Float ) = 1
        
        
        
        
        
        
        
        
        
        _RefrLowDist("_RefrLowDist",  Float ) = 0.1
        
        _VertexToUv("_VertexToUv",  Float ) = 1
        
        
        
        
        
        _WavesDirAngle("_WavesDirAngle", Float ) = 0
        
        _VertexSize("_VertexSize", Vector ) = (0, 0, 1, 1)
        
        
        
        _ZDistanceCalcOffset("_ZDistanceCalcOffset", Float ) = 0.25
        
        
        
        [Header(NOISES)]//////////////////////////////////////////////////
        
        [Header(HQ)]//////////////////////////////////////////////////
        
        _NHQ_GlareAmount("_NHQ_GlareAmount", Float  ) = 1//Range(0.1,50)
        
        _NHQ_GlareFriq("_NHQ_GlareFriq", Float ) = 30 //Range(0.1,100)
        
        _NHQ_GlareSpeedXY("_NHQ_GlareSpeedXY", Float  ) = 1//Range(0,3)
        
        _NHQ_GlareSpeedZ("_NHQ_GlareSpeedZ", Float ) = 1 //Range(0,5)
        
        _NHQ_GlareContrast("_NHQ_GlareContrast", Float  ) = 0//Range(0,2.5)
        
        _NHQ_GlareBlackOffset("_NHQ_GlareBlackOffset", Float ) = 0 //Range(-1,0)
        
        _NHQ_GlareNormalsEffect("_NHQ_GlareNormalsEffect", Float ) = 1//Range(0,2)
        
        [Header(LQ)]//////////////////////////////////////////////////
        
        _NE1_GlareAmount("_NE1_GlareAmount", Float ) = 1//Range(0.1,50 )
        
        _NE1_GlareFriq("_NE1_GlareFriq", Float  ) = 1//Range(0.1,4)
        
        _NE1_GlareSpeed("_NE1_GlareSpeed", Float  ) = 1//Range(0.1,3)
        
        _NE1_GlareContrast("_NE1_GlareContrast", Float  ) = 0//Range(0,2.5)
        
        _NE1_GlareBlackOffset("_NE1_GlareBlackOffset", Float) = 0 //Range(-1,0)
        
        _NE1_WavesDirection("_NE1_WavesDirection", Vector) = (0.5, 0, 0.25)
        
        [Header(WAWES1)]//////////////////////////////////////////////////
        
        _W1_GlareAmount("_W1_GlareAmount", Float  ) = 1//Range(0.1,50)
        
        _W1_GlareFriq("_W1_GlareFriq", Float  ) = 1//Range(0.1,10)
        
        _W1_GlareSpeed("_W1_GlareSpeed", Float  ) = 1//Range(0.1,10)
        
        _W1_GlareContrast("_W1_GlareContrast", Float  ) = 0//Range(0,2.5)
        
        _W1_GlareBlackOffset("_W1_GlareBlackOffset", Float  ) = 0//Range(-1,0)
        
        
        
        [Header(WAWES2)]//////////////////////////////////////////////////
        
        _W2_GlareAmount("_W2_GlareAmount", Float  ) = 1//Range(0.1,50)
        
        _W2_GlareFriq("_W2_GlareFriq", Float  ) = 1//Range(0.1,10)
        
        _W2_GlareSpeed("_W2_GlareSpeed", Float ) = 1//Range(0.1,10)
        
        _W2_GlareContrast("_W2_GlareContrast", Float  ) = 0//Range(0,2.5)
        
        _W2_GlareBlackOffset("_W2_GlareBlackOffset", Float ) = 0//Range(-1,0)
        
        MAIN_TEX_LQDistortion("MAIN_TEX_LQDistortion", Float ) = 0.1
        
        MAIN_TEX_ADDDISTORTION_THAN_MOVE_Amount("MAIN_TEX_ADDDISTORTION_THAN_MOVE_Amount", Float ) = 100
        
        
        
        
        
        [Header(PRC HQ)]//////////////////////////////////////////////////
        
        _PRCHQ_amount("_PRCHQ_amount", Float  ) = 1//Range(0.1,50)
        
        _PRCHQ_tileTex("_PRCHQ_tileTex", Float ) = 1//Range(0.1,10 )
        
        _PRCHQ_tileWaves("_PRCHQ_tileWaves", Float  ) = 1//Range(0.1,10)
        
        _PRCHQ_speedTex("_PRCHQ_speedTex", Float  ) = 1//Range(0.1,10)
        
        _PRCHQ_speedWaves("_PRCHQ_speedWaves", Float ) = 1 //Range(0.1,10)
        
        MAIN_TEX_Amount("MAIN_TEX_Amount", Float ) = 3 //Range(0.1,10)
        
        MAIN_TEX_Contrast("MAIN_TEX_Contrast", Float ) = 2 //Range(0.1,10)
        
        MAIN_TEX_BA_Speed("MAIN_TEX_BA_Speed", Float ) = 8 //Range(0.1,10)
        
        MAIN_TEX_CA_Speed("MAIN_TEX_CA_Speed", Vector ) = (0.1, 0.1, 0.1, 0.1)
        
        MAIN_TEX_Tile("MAIN_TEX_Tile", Vector ) = (1, 1, 1, 1)
        
        MAIN_TEX_Move("MAIN_TEX_Move", Vector ) = (0, 0, 0, 0)
        
        
        
        SIMPLE_VHEIGHT_FADING_AFFECT("SIMPLE_VHEIGHT_FADING_AFFECT", Float ) = 0.3
        
        MAIN_TEX_Distortion("MAIN_TEX_Distortion", Float ) = 2
        
        _ReflectionBlendAmount("_ReflectionBlendAmount", Float ) = 2
        
        
        
        
        
        [Header(fresnelFac)]//////////////////////////////////////////////////
        
        [NoScaleOffset] _Utility("_Utility", 2D) = "white" {}
        
        _FresnelPow("_FresnelPow", Float  ) = 1.58//Range(0.5,32)
        
        _FresnelFade("_FresnelFade", Float) = 0.7//Range(0.5,32)
        
        _FresnelAmount("_FresnelAmount", Float) = 4   //Range(0.5,32)
        
        _RefrDistortionZ("_RefrDistortionZ", Float) = 0   //Range(0.5,32)
        
        FRES_BLUR_OFF("FRES_BLUR_OFF", Float) = 0.3   //Range(0.5,32)
        
        FRES_BLUR_AMOUNT("FRES_BLUR_AMOUNT", Float) = 3   //Range(0.5,32)
        
        _BumpMixAmount("_BumpMixAmount", Float) = 0.5   //Range(0.5,32)
        
        _FastFresnelPow("_FastFresnelPow", Float) = 10   //Range(0.5,32)
        
        
        
        
        
        [Header(Reflection)]//////////////////////////////////////////////////
        
        _ReflectionAmount("_ReflectionAmount", Float  ) = 1//Range(0,2)
        
        _ReflectColor("_ReflectColor", COLOR) = (1, 1, 1, 1)
        
        baked_ReflectionTex_distortion("baked_ReflectionTex_distortion", Float  ) = 15//Range(0,50)
        
        LOW_ReflectionTex_distortion("LOW_ReflectionTex_distortion", Float  ) = 0.1//Range(0,50)
        
        
        
        
        
        _ReflectionYOffset("_ReflectionYOffset", Float  ) = -0.15//Range(-0.4,0)
        
        _ReflectionLOD("_ReflectionLOD", Float  ) = 1//Range(0,4)
        
        
        
        [Header(ReflRefrMask)]//////////////////////////////////////////////////
        
        _ReflectionMask_Amount("_ReflectionMask_Amount", Float  ) = 3//Range(0, 5)
        
        _ReflectionMask_Offset("_ReflectionMask_Offset", Float) = 0.66//Range(0, 0.5)
        
        _ReflectionMask_UpClamp("_ReflectionMask_UpClamp", Float) = 1 //Range(0.5, 10)
        
        _ReflectionMask_Tiling("_ReflectionMask_Tiling", Float  ) = 0.1//Range(0, 2)
        
        _ReflectionBlurRadius("_ReflectionBlurRadius", Float) = 0.1//Range(0, 2)
        
        _ReflectionBlurZOffset("_ReflectionBlurZOffset", Float) = 0//Range(0, 2)
        
        _RefractionBlurZOffset("_RefractionBlurZOffset", Float) = 0//Range(0, 2)
        
        _ReflectionMask_TexOffsetF("_ReflectionMask_TexOffsetF", Vector) = (0, 0, 0, 0)
        
        
        
        _ObjectScale("_ObjectScale", Vector) = (1, 1, 1, 1)
        
        
        
        _AverageOffset("_AverageOffset", Float) = 0.5//Range(0, 2)
        
        _REFR_MASK_Tile("_REFR_MASK_Tile", Float) = 0.1//Range(0, 2)
        
        _REFR_MASK_Amount("_REFR_MASK_Amount", Float) = 3//Range(0, 5)
        
        _REFR_MASK_min("_REFR_MASK_min", Float) = 0.66//Range(0, 0.5)
        
        _REFR_MASK_max("_REFR_MASK_max", Float) = 1 //Range(0.5, 10)
        
        
        
        
        
        
        
        
        
        _UF_NMASK_Texture("_UF_NMASK_Texture", 2D) = "white" {}
        
        _UF_NMASK_Tile("_UF_NMASK_Tile", Float) = 0.1
        
        _UF_NMASK_offset("_UF_NMASK_offset", Vector) = (0, 0, 0, 0)
        
        _UF_NMASK_Contrast("_UF_NMASK_Contrast", Float) = 1
        
        _UF_NMASK_Brightnes("_UF_NMASK_Brightnes", Float) = 0
        
        
        
        
        
        
        
        _AMOUNTMASK_Tile("_AMOUNTMASK_Tile", Float) = 0.1//Range(0, 2)
        
        _AMOUNTMASK_Amount("_AMOUNTMASK_Amount", Float) = 3//Range(0, 5)
        
        _AMOUNTMASK_min("_AMOUNTMASK_min", Float) = 0.66//Range(0, 0.5)
        
        _AMOUNTMASK_max("_AMOUNTMASK_max", Float) = 1 //Range(0.5, 10)
        
        
        
        _TILINGMASK_Tile("_TILINGMASK_Tile", Float) = 0.1//Range(0, 2)
        
        _TILINGMASK_Amount("_TILINGMASK_Amount", Float) = 3//Range(0, 5)
        
        _TILINGMASK_min("_TILINGMASK_min", Float) = 0.66//Range(0, 0.5)
        
        _TILINGMASK_max("_TILINGMASK_max", Float) = 1 //Range(0.5, 10)
        
        
        
        _MAINTEXMASK_Tile("_MAINTEXMASK_Tile", Float) = 0.1//Range(0, 2)
        
        _MAINTEXMASK_Amount("_MAINTEXMASK_Amount", Float) = 3//Range(0, 5)
        
        _MAINTEXMASK_min("_MAINTEXMASK_min", Float) = 0.66//Range(0, 0.5)
        
        _MAINTEXMASK_max("_MAINTEXMASK_max", Float) = 1 //Range(0.5, 10)
        
        _MAINTEXMASK_Blend("_MAINTEXMASK_Blend", Float) = 0.5 //Range(0.5, 10)
        
        
        
        _3Dwaves_BORDER_FACE("_3Dwaves_BORDER_FACE", Float) = 0
        
        _FixHLClamp("_FixHLClamp", Float) = 0.8
        
        _FastFresnelAmount("_FastFresnelAmount", Float) = 10
        
        _RefrDeepAmount("_RefrDeepAmount", Float) = 1
        
        _RefrTopAmount("_RefrTopAmount", Float) = 1
        
        _TexRefrDistortFix("_TexRefrDistortFix", Float) = 0
        
        _SpecularGlowAmount("_SpecularGlowAmount", Float) = 0.2
        
        _RefractionBLendAmount("_RefractionBLendAmount", Float) = 0.5
        
        
        
        _TILINGMASK_factor("_TILINGMASK_factor", Float) = 5 //Range(0.5, 10)
        
        _AMOUNTMASK_offset("_AMOUNTMASK_offset", Vector) = (0, 0, 0, 0)
        
        _TILINGMASK_offset("_TILINGMASK_offset", Vector) = (0, 0, 0, 0)
        
        _MAINTEXMASK_offset("_MAINTEXMASK_offset", Vector) = (0, 0, 0, 0)
        
        _ReflectionMask_offset("_ReflectionMask_offset", Vector) = (0, 0, 0, 0)
        
        _REFR_MASK_offset("_REFR_MASK_offset", Vector) = (0, 0, 0, 0)
        
        _ReplaceColor("_ReplaceColor", COLOR) = (0.5, 0.5, 0.5, 1)
        
        
        
        
        
        [Header(Refraction)]//////////////////////////////////////////////////
        
        
        
        _RefrDistortion("_RefrDistortion", Float  ) = 100//Range(0, 600)
        
        _RefrAmount("_RefrAmount", Float  ) = 1//Range(0, 4)
        
        _RefrZColor("_RefrZColor", COLOR) = (0.29, 0.53, 0.608, 1)
        
        _RefrTopZColor("_RefrTopZColor", COLOR) = (0.89, 0.95, 1, 1)
        
        _RefrTextureFog("_RefrTextureFog", Float) = 0 //Range(0, 1)
        
        
        
        _RefrZOffset("_RefrZOffset", Float  ) = 1.8//Range(0, 2)
        
        _RefrZFallOff("_RefrZFallOff", Float ) = 10 //Range(1, 20)
        
        _RefrBlur("_RefrBlur", Float) = 0.25 //Range(0, 1)
        
        _RefrRecover("_RefrRecover", Float ) = 0.0 //Range(0, 1)
        
        _RefrDeepFactor("_RefrDeepFactor", Float) = 64 //Range(0, 1)
        
        _3dwanamnt("_3dwanamnt", Float) = 1 //Range(0, 1)
        
        
        
        
        
        
        
        [Header(Other)]//////////////////////////////////////////////////
        
        _LightDir("_LightDir", Vector) = (-0.5, 0.46, 0.88, 0)
        
        
        
        
        
        _ObjecAngle("_ObjecAngle", Float) = 0 //Range(0, 1)
        
        
        
        
        
        
        
        _ReflectionTex_size("_ReflectionTex_size", Float) = 256.0 //Range(0, 1)
        
        _ReflectionTex_temp_size("_ReflectionTex_temp_size", Float) = 128.0 //Range(0, 1)
        
        
        
        _RefractionTex_size("_RefractionTex_size", Float) = 512.0 //Range(0, 1)
        
        [NoScaleOffset] _RefractionTex_temp("_RefractionTex_temp", 2D) = "black" {}
        
        _RefractionTex_temp_size("_RefractionTex_temp_size", Float) = 512.0 //Range(0, 1)
        
        
        
        _BakedData_size("_BakedData_size", Float) = 256.0 //Range(0, 1)
        
        [NoScaleOffset] _BakedData_temp("_BakedData_temp ", 2D) = "black" {}
        
        _BakedData_temp_size("_BakedData_temp_size", Float) = 256.0 //Range(0, 1)
        
        
        
        _MTDistortion("_MTDistortion", Float) = 0 //Range(1, 256)
        
        
        
        
        
        RIM_Minus("RIM_Minus", Float) = 0.4 //Range(1, 256)
        
        RIM_Plus("RIM_Plis", Float) = 50 //Range(1, 256)
        
        POSTRIZE_Colors("POSTRIZE_Colors", Float) = 12 //Range(1, 24)
        
        
        
        _MyNearClipPlane("_MyNearClipPlane", Float) = 0//Range(1, 256)
        
        _MyFarClipPlane("_MyFarClipPlane", Float) = 1000//Range(1, 256)
        
        
        
        _MultyOctaveNormals("_MultyOctaveNormals", Float) = 5//Range(1, 256)
        
        
        
        _SurfaceFoamAmount("_SurfaceFoamAmount", Float) = 10//Range(1, 256)
        
        _SurfaceFoamContrast("_SurfaceFoamContrast", Float) = 200//Range(1, 256)
        
        _SUrfaceFoamVector("_SUrfaceFoamVector", Vector) = (0.12065329648999294186660041025867, 0.98533525466827569191057001711249, 0.12065329648999294186660041025867, 0)
        
        
        
        
        
        _ReflectionBakeLayers("_ReflectionBakeLayers", Vector) = (255, 255, 255, 255)
        
        _RefractionBakeLayers("_RefractionBakeLayers", Vector) = (255, 255, 255, 255)
        
        _ZDepthBakeLayers("_ZDepthBakeLayers", Vector) = (255, 255, 255, 255)
        
        
        
        _DetileAmount("_DetileAmount", Float) = 0.15//Range(1, 256)
        
        _DetileFriq("_DetileFriq", Float) = 5//Range(1, 256)
        
        MainTexSpeed("MainTexSpeed", Float) = 1//Range(1, 256)
        
        _ReflectionDesaturate("_ReflectionDesaturate", Float) = 0.5//Range(1, 256)
        
        _RefractionDesaturate("_RefractionDesaturate", Float) = 0.5//Range(1, 256)
        
        
        
        _sinFriq("_sinFriq", Float) = 7.28//Range(1, 256)
        
        _sinAmount("_sinAmount", Float) = 0.1//Range(1, 256)
        
        
        
        
        
        _APPLY_REFR_TO_SPECULAR_DISSOLVE("_APPLY_REFR_TO_SPECULAR_DISSOLVE", Float) = 1
        
        _APPLY_REFR_TO_SPECULAR_DISSOLVE_FAST("_APPLY_REFR_TO_SPECULAR_DISSOLVE_FAST", Float) = 1
        
        
        
        
        
        
        
        _UFSHORE_Amount_1("_UFSHORE_Amount_1", Float ) = 5
        
        _UFSHORE_Amount_2("_UFSHORE_Amount_2", Float ) = 5
        
        _UFSHORE_Length_1("_UFSHORE_Length_1", Float ) = 10
        
        _UFSHORE_Length_2("_UFSHORE_Length_2", Float ) = 10
        
        _UFSHORE_Color_1("_UFSHORE_Color_1", Color ) = (1, 1, 1, 1)
        
        _UFSHORE_Color_2("_UFSHORE_Color_2", Color ) = (1, 1, 1, 1)
        
        _UFSHORE_Distortion_1("_UFSHORE_Distortion_1", Float ) = 0.8
        
        _UFSHORE_Distortion_2("_UFSHORE_Distortion_2", Float ) = 0.8
        
        _UFSHORE_Tile_1("_UFSHORE_Tile_1", Vector ) = (1, 1, 1, 1)
        
        _UFSHORE_Tile_2("_UFSHORE_Tile_2", Vector ) = (1, 1, 1, 1)
        
        _UFSHORE_Speed_1("_UFSHORE_Speed_1", Float ) = 1
        
        _UFSHORE_Speed_2("_UFSHORE_Speed_2", Float ) = 1
        
        _UFSHORE_LowDistortion_1("_UFSHORE_LowDistortion_1", Float ) = 1
        
        _UFSHORE_LowDistortion_2("_UFSHORE_LowDistortion_2", Float ) = 1
        
        _UFSHORE_AlphaAmount_1("_UFSHORE_AlphaAmount_1", Float ) = 0.25
        
        _UFSHORE_AlphaAmount_2("_UFSHORE_AlphaAmount_2", Float ) = 0.25
        
        _UFSHORE_AlphaMax_1("_UFSHORE_AlphaMax_1", Float ) = 1
        
        _UFSHORE_AlphaMax_2("_UFSHORE_AlphaMax_2", Float ) = 1
        
        _UFSHORE_ShadowV1_1("_UFSHORE_ShadowV1_1", Float ) = 1
        
        _UFSHORE_ShadowV2_1("_UFSHORE_ShadowV2_1", Float ) = 1
        
        _UFSHORE_ShadowV1_2("_UFSHORE_ShadowV1_2", Float ) = 1
        
        _UFSHORE_ShadowV2_2("_UFSHORE_ShadowV2_2", Float ) = 1
        
        MAIN_TEX_FoamGradWavesSpeed_1("MAIN_TEX_FoamGradWavesSpeed_1", Float ) = 0.1
        
        MAIN_TEX_FoamGradWavesSpeed_2("MAIN_TEX_FoamGradWavesSpeed_2", Float ) = 0.1
        
        MAIN_TEX_FoamGradDirection_1("MAIN_TEX_FoamGradDirection_1", Float ) = 0.01
        
        MAIN_TEX_FoamGradDirection_2("MAIN_TEX_FoamGradDirection_2", Float ) = 0.01
        
        MAIN_TEX_FoamGradTile_1("MAIN_TEX_FoamGradTile_1", Float ) = 0.1
        
        MAIN_TEX_FoamGradTile_2("MAIN_TEX_FoamGradTile_2", Float ) = 0.1
        
        _UFSHORE_ADDITIONAL_Length_1("_UFSHORE_ADDITIONAL_Length_1", Float ) = 1
        
        _UFSHORE_ADDITIONAL_Length_2("_UFSHORE_ADDITIONAL_Length_2", Float ) = 1
        
        
        
        _UFSHORE_ADD_Amount_1("_UFSHORE_ADD_Amount_1", Float ) = 1
        
        _UFSHORE_ADD_Amount_2("_UFSHORE_ADD_Amount_2", Float ) = 1
        
        _UFSHORE_ADD_Tile_1("_UFSHORE_ADD_Tile_1", Float ) = 0.1
        
        _UFSHORE_ADD_Tile_2("_UFSHORE_ADD_Tile_2", Float ) = 0.1
        
        _UFSHORE_ADD_Distortion_1("_UFSHORE_ADD_Distortion_1", Float ) = 1
        
        _UFSHORE_ADD_Distortion_2("_UFSHORE_ADD_Distortion_2", Float ) = 1
        
        _UFSHORE_ADD_LowDistortion_1("_UFSHORE_ADD_LowDistortion_1", Float ) = 0.1
        
        _UFSHORE_ADD_LowDistortion_2("_UFSHORE_ADD_LowDistortion_2", Float ) = 0.1
        
        _UFSHORE_ADD_Color_1("_UFSHORE_ADD_Color_1", Color ) = (1, 1, 1, 1)
        
        _UFSHORE_ADD_Color_2("_UFSHORE_ADD_Color_2", Color ) = (1, 1, 1, 1)
        
        SHORE_USE_ADDITIONALCONTUR_POW_Amount_1("SHORE_USE_ADDITIONALCONTUR_POW_Amount_1", Float ) = 10
        
        SHORE_USE_ADDITIONALCONTUR_POW_Amount_2("SHORE_USE_ADDITIONALCONTUR_POW_Amount_2", Float ) = 10
        
        _UFSHORE_UNWRAP_LowDistortion_1("_UFSHORE_UNWRAP_LowDistortion_1", Float ) = 0
        
        
        
        _LOW_DISTOR_Tile("_LOW_DISTOR_Tile", Float ) = 20
        
        _LOW_DISTOR_Speed("_LOW_DISTOR_Speed", Float ) = 8
        
        _LOW_DISTOR_Amount("_LOW_DISTOR_Amount", Float ) = 1
        
        //_BAKED_DEPTH_EXTERNAL_TEXTURE_Amount("_BAKED_DEPTH_EXTERNAL_TEXTURE_Amount", Float ) = 1
        
        
        
        _LOW_DISTOR_MAINTEX_Tile("_LOW_DISTOR_MAINTEX_Tile", Float ) = 20
        
        _LOW_DISTOR_MAINTEX_Speed("_LOW_DISTOR_MAINTEX_Speed", Float ) = 8
        
        MAIN_TEX_MixOffset("MAIN_TEX_MixOffset", Float ) = 0

		_Z_BLACK_OFFSET_V("_Z_BLACK_OFFSET_V", Float) = 0

        
        
        
        
        _FoamAmount_SW("_FoamAmount_SW", Float ) = 5 //Range(0,10)
        
        _FoamLength_SW("_FoamLength_SW", Float ) = 10 //Range(0,20)
        
        _FoamColor_SW("_FoamColor_SW", COLOR) = (1, 1, 1, 1)
        
        _FoamDistortion_SW("_FoamDistortion_SW", Float ) = 1 //Range(0,10)
        
        _FoamDistortionFade_SW("_FoamDistortionFade_SW", Float) = 0.8 //Range(0,1)
        
        _FoamDirection_SW("_FoamDirection_SW", Float) = 0.05 //Range(0,1)
        
        _WaterfrontFade_SW("_WaterfrontFade_SW", Float ) = 0//Range(0,30)
        
        _LightNormalsFactor("_LightNormalsFactor", Float ) = 1//Range(0,30)
        
        
        
        
        
        _FoamTextureTiling_SW("_FoamTextureTiling_SW", Float ) = 1 //Range(0.01,2)
        
        _FoamWavesSpeed_SW("_FoamWavesSpeed_SW", Float ) = 0.15//Range(0,2 )
        
        _ShoreWavesRadius("_ShoreWavesRadius", Float ) = 0.1//Range(0,2 )
        
        _ShoreWavesQual("_ShoreWavesQual", Float ) = 2//Range(0,2 )
        
        _FoamLShoreWavesTileY_SW("_FoamLShoreWavesTileY_SW", Float ) = 1//Range(0,2 )
        
        _FoamLShoreWavesTileX_SW("_FoamLShoreWavesTileX_SW", Float ) = 1//Range(0,2 )
        
        _FoamMaskOffset_SW("_FoamMaskOffset_SW", Float ) = 0.0//Range(0,2 )
        
        
        
        _BumpZOffset("_BumpZOffset", Float ) = 1.0//Range(0,2 )
        
        _BumpZFade("_BumpZFade", Float ) = 0//Range(0,2 )
        
        _RefractionBlendOffset("_RefractionBlendOffset", Float ) = 0.0//Range(0,2 )
        
        _RefractionBlendFade("_RefractionBlendFade", Float ) = 1//Range(0,2 )
        
        
        
        _RefrBled_Fres_Amount("_RefrBled_Fres_Amount", Float ) = 1//Range(0,2 )
        
        _RefrBled_Fres_Pow("_RefrBled_Fres_Pow", Float ) = 1//Range(0,2 )
        
        
        
        
        
        _LutAmount("_LutAmount", Float ) = 1.0//Range(0,2 )
        
        
        
        _GAmplitude("Wave Amplitude", Vector) = (0.3, 0.35, 0.25, 0.25)
        
        _GFrequency("Wave Frequency", Vector) = (1.3, 1.35, 1.25, 1.25)
        
        _GSteepness("Wave Steepness", Vector) = (1.0, 1.0, 1.0, 1.0)
        
        _GSpeed("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)
        
        _GDirectionAB("Wave Direction", Vector) = (0.3, 0.85, 0.85, 0.25)
        
        _GDirectionCD("Wave Direction", Vector) = (0.1, 0.9, 0.5, 0.5)
        
        //_GAmplitude("Wave Amplitude", Vector) = 1
        
        //	_GFrequency("Wave Frequency", Vector) = 1
        
        //	_GSteepness("Wave Steepness", Vector) = 1
        
        //	_GSpeed("Wave Speed", Vector) = 1
        
        //	_GDirectionAB("Wave Direction", Vector) =(0.3 ,0.85, 0.85, 0.25)
        
        //
        
        //	_CameraFarClipPlane("_CameraFarClipPlane", FLOAT) = 4
        
        //_LightPos("_LightPos", Vector) = (2001,1009,3274,0)
        
        
        
        
        
        _FoamDistortionTexture("_FoamDistortionTexture", Float ) = 1
        
        _ShoreDistortionTexture("_ShoreDistortionTexture", Float ) = 1
        
        
        
        _MOR_Offset("_MOR_Offset", Float ) = 1
        
        _MOR_Base("_MOR_Base", Float ) = 1
        
        _C_BLUR_R("_C_BLUR_R", Float ) = 0.05
        
        _C_BLUR_S("_C_BLUR_S", Float ) = 0.05
        
        _MOR_Tile("_MOR_Tile", Float ) = 82
        
        
        
        _FracTimeX("_FracTimeX", Float ) = 0
        
        _FracTimeW("_FracTimeW", Float ) = 0
        
        
        
        _WaveGrad0("_WaveGrad0", COLOR) = (1, 1, 1, 1)
        
        _WaveGrad1("_WaveGrad1", COLOR) = (1, 1, 1, 1)
        
        _WaveGrad2("_WaveGrad2", COLOR) = (1, 1, 1, 1)
        
        _WaveGradMidOffset("_WaveGradMidOffset", Float) = 0.5
        
        _WaveGradTopOffset("_WaveGradTopOffset", Float) = 1
        
        
        
        
        
        _SFW_Tile("_SFW_Tile", Vector) = (1, 1, 1, 1)
        
        _SFW_Speed("_SFW_Speed", Vector) = (1, 1, 1, 1)
        
        _SFW_Amount("_SFW_Amount", Float) = 1
        
        _SFW_NrmAmount("_SFW_NrmAmount", Float) = 1
        
        _SFW_Dir("_SFW_Dir", Vector) = (1, 0, 0, 0)
        
        _SFW_Dir1("_SFW_Dir1", Vector) = (0.9, 0, 0.1, 0)
        
        _SFW_Distort("_SFW_Distort", Float) = 1
        
        
        
        
        
        
        
        
        
        _CAUSTIC_FOG_Amount("_CAUSTIC_FOG_Amount", Float ) = 1 //Range(0.1,32)
        
        _CAUSTIC_FOG_Pow("_CAUSTIC_FOG_Pow", Float ) = 4 //Range(0.1,32)
        
        _CAUSTIC_Speed("_CAUSTIC_Speed", Float ) = 1 //Range(0.1,32)
        
        //_CAUSTIC_Tiling("_CAUSTIC_Tiling", Float ) = 1 //Range(0.1,32)
        
        _CAUSTIC_Tiling("_CAUSTIC_Tiling", Vector ) = (1, 1, 0, 0)
        
        _CAUSTIC_Offset("_CAUSTIC_Offset", Vector ) = (1, 2, 1, 0)
        
        
        
        _CAUSTIC_PROC_Tiling("_CAUSTIC_PROC_Tiling", Vector ) = (1, 1, 1, 1)
        
        _CAUSTIC_PROC_GlareSpeed("_CAUSTIC_PROC_GlareSpeed", Float ) = 1 //Range(0.1,32)
        
        _CAUSTIC_PROC_Contrast("_CAUSTIC_PROC_Contrast", Float ) = 1 //Range(0.1,32)
        
        _CAUSTIC_PROC_BlackOffset("_CAUSTIC_PROC_BlackOffset", Float ) = 1 //Range(0.1,32)
        
        
        
        
        
        DOWNSAMPLING_SAMPLE_SIZE("DOWNSAMPLING_SAMPLE_SIZE", Float ) = 0.33
        
        
        
        
        
        
        
        
        
        _CN_DISTANCE("_CN_DISTANCE", Float ) = 200
        
        _CN_TEXEL("_CN_TEXEL", Float ) = 0.01
        
        _CLASNOISE_PW("_CLASNOISE_PW", Float ) = 0.35
        
        _CN_AMOUNT("_CN_AMOUNT", Float ) = 5
        
        _CN_SPEED("_CN_SPEED", Float ) = 2
        
        _CN_TILING("_CN_TILING", Vector) = (5, 5, 1, 1)
        
        
        
        
        
    }
    
    
    
    SubShader{
    
    
    
    
        Tags{ "Queue" = "Geometry" "RenderType" = "Opaque"   "PreviewType" = "Plane"  "IgnoreProjector" = "False" }
        Lighting Off
        
        
        //Cull Off
        
        Pass{
            CGPROGRAM
            
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))
            
#define USE_FAKE_LIGHTING = 1;
#define TILEMASKTYPE_TILE = 1;
#define GRAD_G = 1;
#define SKIP_FOAM_COAST_ALPHA_FAKE = 1;
#define REFR_MASK_A = 1;
#define RRSIMPLEBLEND = 1;
#define MAINTEXLERPBLEND = 1;
#define SKIP_FOAM = 1;
#define SKIP_SURFACE_FOAMS = 1;
#define SKIP_MAINTEXTURE = 1;
#define USE_OUTPUT_SHADOWS = 1;
#define REFLECTION_MASK_A = 1;
#define DETILE_LQ = 1;
#define SKIP_SHORE_BORDERS = 1;
#define SKIP_REFLECTION_BLUR_ZDEPENDS = 1;
#define USE_SURFACE_GRADS = 1;
#define REFLECTION_BLUR_1 = 1;
#define REFRACTION_BLUR_1 = 1;
#define SHORE_WAVES = 1;
#define SKIP_FLAT_SPECULAR_CLAMP = 1;
#define WAVES_MAP_NORMAL = 1;
#define FIX_OVEREXPO = 1;
#define LIGHTING_BLEND_SIMPLE = 1;
#define SKIP_AMBIENT_COLOR = 1;
#define GRAD_4 = 1;
#define MAINTEX_HAS_MOVE = 1;
#define SHORE_UNWRAP_STRETCH_DOT = 1;
#define RIM_INVERSE = 1;
#define REFLECTION_BLUR = 1;
#define WAW3D_NORMAL_CALCULATION = 1;
#define FIX_HL = 1;
#define MAIN_TEX_LowDistortion_1 = 1;
#define SKIP_3DVERTEX_HEIGHT_COLORIZE = 1;
#define SHORE_USE_WAVES_GRADIENT_1 = 1;
#define VERTEX_ANIMATION_BORDER_FADE = 1;
#define SHORE_SHADDOW_1 = 1;
#define SIN_OFFSET = 1;
#define SKIP_LIGHTING = 1;
#define SKIP_MAINTEX_VHEIGHT = 1;
#define USE_SIM_RADIUS = 1;
#define USE_REFRACTION_BLEND_FRESNEL = 1;
#define USE_DEPTH_FOG = 1;
#define MAIN_TEX_ADDDISTORTION_THAN_MOVE = 1;
#define MM_SUB = 1;
#define lerped_post_SUB_2 = 1;
#define ULTRA_FAST_MODE = 1;
#define USE_REFR_LOW_DISTOR = 1;
#define USE_REFR_DISTOR = 1;
#define FOAM_ALPHA_FLAT = 1;
#define REFRACTION_BAKED_ONAWAKE = 1;
#define FORCE_OPAQUE = 1;
#define SMOOTH_BLEND_ANIMATION = 1;
#define APPLY_REFR_TO_SPECULAR = 1;
#define USE_FAST_FRESNEL = 1;
#define USE_lerped_post_Color_1 = 1;
#define USE_lerped_post = 1;
#define SKIP_BLEND_ANIMATION = 1;
#define BAKED_DEPTH_ONAWAKE = 1;
#define UF_AMOUNTMASK = 1;
#define POST_TEXTURE_TINT = 1;
#define _ShoreWaves_G = 1;
#define _ShoreWaves_SECOND_TEXTURE = 1;
#define UFAST_SHORE_1 = 1;
#define SHORE_USE_LOW_DISTORTION_1 = 1;
#define USE_BLENDANIMATED_MAINTEX = 1;
#define POST_OWN_TEXTURE = 1;
#define REFLECTION_NONE = 1;
#define USE_OUTPUT_BLEND_3 = 1;
            
//#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlightnoshadow
#pragma fragmentoption ARB_precision_hint_fastest
#pragma multi_compile ORTO_CAMERA_ON _
#pragma multi_compile WATER_DOWNSAMPLING _
#pragma target 2.0
            
            
            
            
            
            
#define MYFLOAT float
#define MYFLOAT2 float2
#define MYFLOAT3 float3
#define MYFLOAT4 float4
            
            
            
            
            #if defined(MINIMUM_MODE) || defined(ULTRA_FAST_MODE)
#define MYFIXED fixed
#define MYFIXED2 fixed2
#define MYFIXED3 fixed3
#define MYFIXED4 fixed4
            #else
            #endif
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            #if !defined(ULTRA_FAST_MODE)  && !defined(MINIMUM_MODE)
            #endif
            #if  defined(REFLECTION_2D) || defined(REFLECTION_PLANAR)||defined(REFLECTION_PROBE_AND_INTENSITY)||defined(REFLECTION_PROBE_AND_INTENSITY)||defined(REFLECTION_PROBE)|| defined(REFLECTION_USER) || defined(REFLECTION_JUST_COLOR)
            #endif
            
            
            
uniform MYFIXED _Z_BLACK_OFFSET_V;

            uniform MYFIXED _ObjecAngle;
            #if !defined(ULTRA_FAST_MODE)  && !defined(MINIMUM_MODE)
            #endif
            MYFIXED _Frac2PITime;
            MYFIXED _Frac01Time;
            MYFIXED _Frac01Time_d8_mBlendAnimSpeed;
            MYFIXED _Frac01Time_MAIN_TEX_BA_Speed;
            float2 _FracWTime_m4_m3DWavesSpeed_dPI2;
            MYFIXED2 _Frac_UFSHORE_Tile_1Time_d10_m_UFSHORE_Speed1;
            MYFIXED2 _Frac_UFSHORE_Tile_2Time_d10_m_UFSHORE_Speed2;
            MYFIXED _Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1;
            
            
            float2 _Frac_WaterTextureTilingTime_m_AnimMove;
            float4 _Frac_UVS_DIR;
            
            float2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move;
            float2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed;
            float2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed;
            
            
            
            uniform MYFIXED _LOW_DISTOR_Tile;
            uniform MYFIXED _LOW_DISTOR_Speed;
            uniform MYFIXED _LOW_DISTOR_Amount;
            
            uniform MYFIXED DOWNSAMPLING_SAMPLE_SIZE;
            float _FrameRate;
            sampler2D _FrameBuffer;
            
            
            
            
            uniform MYFIXED _BumpMixAmount;
            
            
            #if !defined(USING_FOG) && !defined(SKIP_FOG)
#define SKIP_FOG = 1
            #endif
            
            
            
            
            #if defined(REFRACTION_BAKED_FROM_TEXTURE) || defined(REFRACTION_BAKED_ONAWAKE) || defined(REFRACTION_BAKED_VIA_SCRIPT)
#define HAS_BAKED_REFRACTION = 1
            #endif
            
            
            #if defined(SURFACE_FOG)
            #endif
            
            
            #if defined(REFRACTION_GRABPASS) || defined(HAS_BAKED_REFRACTION) || defined(REFRACTION_ONLYZCOLOR)
            #if !defined(DEPTH_NONE)
#define HAS_REFRACTION = 1
            #endif
#define REQUEST_DEPTH = 1
            #endif
            
            #if (defined(REFRACTION_Z_BLEND) || defined(REFRACTION_Z_BLEND_AND_FRESNEL))&& !defined(DEPTH_NONE)
            #endif
            
            #if defined(RRFRESNEL) || defined(HAS_REFRACTION_Z_BLEND_AND_RRFRESNEL)
            #endif
            
            #if !defined(DEPTH_NONE)
            uniform MYFIXED _RefrDistortionZ;
            #endif
            
            
            
            
            #if defined(USE_OUTPUT_GRADIENT) &&( defined(USE_OUTPUT_BLEND_1) || !defined(USE_OUTPUT_BLEND_3))
            #endif
            
            #if !defined(SKIP_3DVERTEX_ANIMATION) && (defined(VERTEX_ANIMATION_BORDER_FADE) )
            
            #if !defined(REQUEST_DEPTH)
            #endif
            #endif
            #if !defined(SKIP_3DVERTEX_ANIMATION)
            #if defined(USE_SIMPLE_VHEIGHT_FADING)
            #endif
            #endif
            
            
            
            
            
            
            #if defined(DEPTH_NONE)
            #elif defined(BAKED_DEPTH_ONAWAKE) || defined(BAKED_DEPTH_VIASCRIPT) || defined(BAKED_DEPTH_EXTERNAL_TEXTURE)
#define HAS_BAKED_DEPTH = 1
            #else
            #endif
            
            
            
            
            
            
            #if !defined(SKIP_FOAM)
            #endif
            #if defined(DEPTH_NONE)
            #endif
            
            
            #if defined(RRSIMPLEBLEND) || defined(RRMULTIBLEND)
            uniform MYFIXED _AverageOffset;
            #endif
            
            
            
            #if defined(REFLECTION_NONE) && !defined(SKIP_REFLECTION_MASK)
#define SKIP_REFLECTION_MASK = 1
            #endif
            
            
            #if defined(HAS_CAMERA_DEPTH) || defined(HAS_BAKED_DEPTH)|| defined(NEED_SHORE_WAVES_UNPACK) || defined(SHORE_WAVES) && !defined(DEPTH_NONE) || defined(USE_CAUSTIC) || defined(SURFACE_FOG)
#define USE_WPOS = 1
            
            
            #endif
            
            
#include "UnityCG.cginc"
            #if !defined(SKIP_LIGHTING) || !defined(SKIP_SPECULAR) || !defined(SKIP_FLAT_SPECULAR)
            #if !defined(USE_FAKE_LIGHTING)
            #endif
            #endif
            
            
            #if defined(USE_SHADOWS)
            #endif
            
            
            #if !defined(SKIP_FOAM_FINE_REFRACTIOND_DOSTORT) && !defined(SKIP_FOAM)
            #endif
            
            
            #if defined(HAS_BAKED_REFRACTION)
            uniform MYFIXED _RefractionBakeLayers;
            #endif
            #if defined(HAS_BAKED_DEPTH)
            uniform MYFIXED _ZDepthBakeLayers;
            #endif
            
            
            
            
            #if defined(HAS_REFRACTION) && defined(REFR_MASK)
            #endif
            
            
            
            
            
            #if defined(UF_AMOUNTMASK) && !defined(_UF_NMASK_USE_MAINTEX) || !defined(SKIP_UNWRAP_TEXTURE) && defined(_ShoreWaves_SECOND_TEXTURE) || defined(POST_TEXTURE_TINT) && defined(POST_OWN_TEXTURE) || defined(POST_TEXTURE_TINT) && defined(POST_SECOND_TEXTURE)
            sampler2D _UF_NMASK_Texture;
#define HAS_SECOND_TEXTURE = 1
            #endif
            
            #if defined(SHORE_WAVES) && defined(ADVANCE_PC) || defined(UFAST_SHORE_1) && !defined(_ShoreWaves_SECOND_TEXTURE) && !defined(_ShoreWaves_USE_MAINTEX) && !defined(ADVANCE_PC)
            #endif
            
            
            
            #if defined(MINIMUM_MODE) || defined(ULTRA_FAST_MODE)
            
            
            
            
            
            
            #if defined(UF_AMOUNTMASK)
            uniform half _UF_NMASK_Tile;
            uniform MYFIXED2 _UF_NMASK_offset;
            uniform MYFIXED _UF_NMASK_Contrast;
            uniform MYFIXED _UF_NMASK_Brightnes;
            #endif
            #else
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
            #endif
            #endif
            
            #if defined(HAS_BAKED_REFRACTION)
            #if defined(REFRACTION_BAKED_FROM_TEXTURE)
            #else
            uniform sampler2D _RefractionTex_temp;
            #endif
            #endif
            
            #if !defined(SKIP_REFRACTION_CALC_DEPTH_FACTOR) || !defined(SKIP_Z_CALC_DEPTH_FACTOR)
            uniform MYFIXED _RefrDeepFactor;
            #endif
            
            #if defined(HAS_CAMERA_DEPTH)
            #endif
            
            #if defined(USE_OUTPUT_GRADIENT)
            #endif
            
            uniform MYFIXED4 _MainTexColor;
            
            #if defined(TRANSPARENT_LUMINOSITY)
            #endif
            #if defined(TRANSPARENT_POW)
            #endif
            #if defined(TRANSPARENT_SIMPLE)
            #endif
            
            #if defined(MULTI_OCTAVES)
            #endif
            
            
            uniform MYFIXED2 _AnimMove;
            uniform MYFIXED _MainTexAngle;
            uniform sampler2D _MainTex;
            uniform MYFIXED4 _MainTex_ST;
            #if !defined(SKIP_MAINTEXTURE) || defined(USE_NOISED_GLARE_PROCEDURALHQ) || !defined(SKIP_REFLECTION_MASK) || defined(HAS_REFR_MASK)
            #endif
            #if !defined(SKIP_MAINTEXTURE)
            #endif
            
            uniform MYFIXED4 _WaterTextureTiling;
            
            uniform sampler2D _Utility;
            
            uniform MYFIXED _BumpAmount;
            
            uniform sampler2D _BumpMap;
            
            
            #if defined(BAKED_DEPTH_EXTERNAL_TEXTURE)
            #else
            uniform sampler2D _BakedData_temp;
            #endif
            
            
            #if !defined(SKIP_NOISED_GLARE_HQ) || defined(USE_NOISED_GLARE_ADDWAWES2) || defined(USE_NOISED_GLARE_ADDWAWES1) || defined(USE_NOISED_GLARE_LQ)  || defined(USE_NOISED_GLARE_PROCEDURALHQ)
            
            
            #endif
            
            
            #if defined(REFRACTION_GRABPASS)
            #endif
            #if defined(HAS_REFRACTION)
            uniform MYFIXED _RefrDistortion;
            
            #if defined(USE_CAUSTIC) && !defined(ULTRA_FAST_MODE) && !defined(MINIMUM_MODE)
            #endif
            
            
            #if defined(WAVES_GERSTNER)
            #endif
            
            
            
            #if defined(HAS_NOISE_TEX)
            #endif
            
            
            
            #if defined(USE_REFR_LOW_DISTOR)
            uniform MYFIXED	_RefrLowDist;
            #endif
            
            uniform MYFIXED	_RefrTopAmount;
            uniform MYFIXED	_RefrDeepAmount;
            #if !defined(ULTRA_FAST_MODE) && !defined(MINIMUM_MODE)
            #endif
            
            uniform MYFIXED	_TexRefrDistortFix;
            
            uniform MYFIXED3	_RefrTopZColor;
            uniform MYFIXED3	_RefrZColor;
            uniform MYFIXED	_RefrRecover;
            uniform MYFIXED	_RefrZOffset;
            uniform MYFIXED _RefrZFallOff;
            
            #if defined(REFRACTION_BLUR)
            #endif
            
            #if defined(USE_REFRACTION_BLEND_FRESNEL) && defined(REFLECTION_NONE)
            #endif
            uniform MYFIXED _RefractionBLendAmount;
            
            #endif
            
            #if defined(USE_OUTPUT_GRADIENT)
            #endif
            uniform MYFIXED4 _VertexSize;
            
            #if !defined(SKIP_3DVERTEX_ANIMATION)
            
            #if defined(HAS_WAVES_ROTATION)
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
            
            
            #if  !defined(SKIP_3DVERTEX_ANIMATION) && !defined(SKIP_3DVERTEX_HEIGHT_COLORIZE)
            #endif
            uniform MYFIXED _VERTEX_ANIM_DETILEAMOUNT;
            uniform MYFIXED _VERTEX_ANIM_DETILESPEED;
            #endif
            
            #if defined(USE_LIGHTMAPS)
            #endif
            
            #if defined(USE_OUTPUT_SHADOWS)
            uniform MYFIXED _OutShadowsAmount;
            #endif
            
            
            
            
            
            #if defined(POSTRIZE)
            #endif
            
            #if defined(RIM)
            #endif
            
            
            
            
            
            #if !defined(REFLECTION_NONE)
            #endif
            
            
            
            #if !defined(SKIP_FRESNEL_CALCULATION)
            uniform MYFIXED _FresnelFade;
            uniform MYFIXED _FresnelAmount;
            uniform MYFIXED _FresnelPow;
            #endif
            #if !defined(SKIP_FRESNEL_CALCULATION) && defined(USE_FRESNEL_POW)
            #endif
            
            
            #if !defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR) || !defined(SKIP_FLAT_SPECULAR)
            uniform MYFIXED3 _LightDir;
            
            #endif
            #if !defined(SKIP_LIGHTING)
            #endif
            #if !defined(SKIP_SPECULAR)
            uniform MYFIXED _SpecularAmount;
            uniform MYFIXED _SpecularShininess;
            uniform MYFIXED _SpecularGlowAmount;
            #endif
            #if !defined(SKIP_FLAT_SPECULAR)
            uniform MYFIXED _FlatSpecularAmount;
            uniform MYFIXED _FlatSpecularShininess;
            uniform MYFIXED _FlatSpecularClamp;
            uniform MYFIXED _FlatFriqX;
            uniform MYFIXED _FlatFriqY;
            uniform MYFIXED _Light_FlatSpecTopDir;
            
            #if defined(USE_FLAT_HQ)
            #endif
            
            #endif
            
            
            
            #if !defined(SKIP_REFLECTION_MASK)
            #endif
            
            
            #if (defined(HAS_FOAM) || defined(SHORE_WAVES)) && !defined(ULTRA_FAST_MODE) && !defined(MINIMUM_MODE)
            #endif
            
            
            
            #if !defined(SKIP_FOAM)
            #endif
            
            #if !defined(SKIP_SURFACE_FOAMS) && !defined(ADVANCE_PC)
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
            #endif
            #endif
            
            
            #if defined(USE_SURFACE_GRADS)
            
            #endif
            
            #if defined(WAVES_GERSTNER)&& !defined(ADVANCE_PC)
            #endif
            
            uniform MYFIXED3 _ObjectScale;
            
            #if defined(Z_AFFECT_BUMP) && !defined(DEPTH_NONE)
            #endif
            
            
            
            #if defined(USE_LUT)
            #endif
            
            
            
            
            #if  defined (ADDITIONAL_FOAM_DISTORTION) && defined(HAS_REFRACTION) &&  !defined(SKIP_FOAM)
            #endif
            
            
            #if defined(ZDEPTH_CALCANGLE)
            #endif
            
            
            
            #if defined(SURFACE_WAVES)&& !defined(ADVANCE_PC)
            #endif
            
            
            #if !defined(SKIP_BLEND_ANIMATION)
            #endif
            
            
            #if defined(USE_FAST_FRESNEL)
            uniform MYFIXED _FastFresnelAmount;
            uniform MYFIXED _FastFresnelPow;
            #endif
            
            
            
            #if defined(SKIP_MAIN_TEXTURE)
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
            
            
            
            uniform MYFIXED3 _ReplaceColor;
            
            
            
            #if defined(UFAST_SHORE_1)
            #if defined(SHORE_USE_WAVES_GRADIENT_1)
            uniform MYFIXED MAIN_TEX_FoamGradWavesSpeed_1;
            uniform MYFIXED MAIN_TEX_FoamGradDirection_1;
            uniform MYFIXED MAIN_TEX_FoamGradTile_1;
            #endif
            #if defined(SHORE_USE_ADDITIONALCONTUR_1)
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
            
            
            
            
            #if defined(UFAST_SHORE_2)
            #endif
            */

































































			//#include "EModules Water Model Ultra Fast Mode uniform.cginc"
			//#include "EModules Water Model 2.0 uniform.cginc"



			struct appdata {
				float4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed4 color : COLOR;
				fixed4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0; // xy - world ; zw - object
				float4 uvscroll : TEXCOORD1;
				half4 screen : TEXCOORD2;
				fixed4 wPos : TEXCOORD3; //xyz - wpos // w - distance to camera
				fixed4 VER_TEX : TEXCOORD4;// xyz - face tangent view ; w - local vertex y pos 
				fixed4 helpers : TEXCOORD5; //xy - inputnormals ; x - vertex alpha ; w - up sun factor
				fixed4 vfoam : TEXCOORD6; //xy - low distortion ; z - stretched uv foam ; w - flat spec
				fixed4 fogCoord : TEXCOORD7;//x - fog ; y - ... ; zw - low distortion main tex
			/*#if !defined(SKIP_FOG)
			#ifdef ORTO_CAMERA_ON
				fixed4 fogCoord : TEXCOORD7;
			#else
				UNITY_FOG_COORDS(7)
			#endif
			#endif*/
			//fixed4 detileuv : TEXCOORD1;
			//fixed2 texcoord : TEXCOORD3;

			//fixed4 tspace0 : TEXCOORD4; // tangent.x, bitangent.x, normal.x, localnormal
			//fixed4 tspace1 : TEXCOORD5; // tangent.y, bitangent.y, normal.y, localnormal
			//fixed4 tspace2 : TEXCOORD6; // tangent.z, bitangent.z, normal.z, localnormal

#if defined(REFRACTION_GRABPASS)

				fixed4 grabPos : TEXCOORD8;
#endif


#if !defined(SKIP_BLEND_ANIMATION) && !defined(SHADER_API_GLES)
				fixed4 blend_index : TEXCOORD9;
				fixed4 blend_time : TEXCOORD10;
#endif



#if defined(USE_VERTEX_OUTPUT_STEREO)

				UNITY_VERTEX_OUTPUT_STEREO
#endif

			};

#include "EModules Water Model Utils.cginc"





			float2 INIT_COORDS(inout appdata v, inout v2f o) {

				o.wPos = mul(unity_ObjectToWorld, v.vertex);

				o.uv.zw = 1 - (v.vertex.xz - _VertexSize.xy) / (_VertexSize.zw);

				float TX = -(o.wPos.x / 1000 * _MainTex_ST.x + _MainTex_ST.z);
				float TY = -(o.wPos.z / 1000 * _MainTex_ST.y + _MainTex_ST.w);


#if defined(HAS_ROTATION)
				float a_x = cos(_MainTexAngle);
				float a_y = sin(_MainTexAngle);

				//o.detileuv.z = a_x;
				//o.detileuv.w = a_y;

				return fixed2(a_x, a_y) * TX + fixed2(-a_y, a_x) * TY;
#else
				return fixed2(TX, TY);
#endif
			}


			v2f vert(appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);


#if defined(USE_VERTEX_OUTPUT_STEREO)
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
#endif

				float2 texcoord = INIT_COORDS(v, o);

#if !defined(SKIP_BLEND_ANIMATION) && !defined(SHADER_API_GLES)
				BLEND_ANIMATION(o);
#endif

#if defined(SIN_OFFSET)
				texcoord.y += sin(texcoord.x *_sinFriq) * _sinAmount / 10;
#endif

				fixed3 n = v.normal;
#if !defined(SKIP_3DVERTEX_ANIMATION)
				VERTEX_MOVER(v, o, texcoord, n);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.wPos = mul(unity_ObjectToWorld, v.vertex);
				o.uv.zw = 1 - (v.vertex.xz - _VertexSize.xy) / (_VertexSize.zw);
				//texcoord = INIT_COORDS(v, o);

#else 
				o.pos = UnityObjectToClipPos(v.vertex);
				o.VER_TEX.w = 0.2;
#endif

				o.wPos.w = distance(_WorldSpaceCameraPos.xyz, o.wPos.xyz);

				o.screen = ComputeScreenPos(o.pos);
				o.uv.xy = texcoord.xy;

#if !defined(SKIP_3DVERTEX_ANIMATION)
				//fixed2 f = o.vfoam.xy / _VertexSize.zw;
				float2 f = o.vfoam.xy * _ObjectScale.xz;
#if defined(HAS_ROTATION)
				float a_x = cos(_MainTexAngle);
				float a_y = sin(_MainTexAngle);
				f = fixed2(a_x, a_y) * f.x + fixed2(-a_y, a_x) * f.y;
#endif
				fixed _vuv = 0.005 * (1 - _VertexToUv);
				o.uv.x = o.uv.x - f.y * _vuv;
				o.uv.y = o.uv.y + f.x * _vuv;
#endif



				o.uv.xy *= _WaterTextureTiling.xy;

#if defined(USE_VERTEX_OUTPUT_STEREO)
				COMPUTE_EYEDEPTH(o.screen.z);
#endif
#if defined(USE_LIGHTMAPS)
				//o._utils.zw = v.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
#if defined(REFRACTION_GRABPASS)
				o.grabPos = ComputeGrabScreenPos(o.pos);
#endif


				TANGENT_SPACE_ROTATION; // Unity macro that creates a rotation matrix called "rotation" that transforms vectors from object to tangent space.
				o.VER_TEX.xyz = normalize(mul(rotation, ObjSpaceViewDir(v.vertex))); // Get tangent space view dir from object space view dir.
				o.VER_TEX.xy = o.VER_TEX.xy / _VertexSize.wz * 150 / _ObjectScale.xz;
				o.VER_TEX.xy = o.VER_TEX.xy / (o.VER_TEX.z* 0.8 + 0.2);
				//	o.VER_TEX.z = sqrt(1 - o.VER_TEX.x * o.VER_TEX.x - o.VER_TEX.y * o.VER_TEX.y);
				o.helpers.z = max(0, v.color.a * 10 - 9);
				//fixed3 wViewDir = normalize(UnityWorldSpaceViewDir(o.wPos));
				//o.valpha.yzw = normalize((-(_LightDir)+(wViewDir)));
				//o.wViewDir = normalize(UnityWorldSpaceViewDir(o.wPos));


				//o.tspace.xy = fixed2(cos(_ObjecAngle), sin(_ObjecAngle));

				/*fixed3 wNormal = UnityObjectToWorldNormal(n);

				fixed3 wTangent = v.tangent.xyz;

				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 wBitangent = cross(wNormal, wTangent) * tangentSign;


				o.tspace0.xyz = fixed3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1.xyz = fixed3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2.xyz = fixed3(wTangent.z, wBitangent.z, wNormal.z);

				o.tspace0.w = n.x;
				o.tspace1.w = n.y;
				o.tspace2.w = n.z;*/
				o.helpers.xy = n.xz;

				/*	fixed SCSPDF_DIV = 1.4;

			#if defined(WAVES_MAP_CROSS)
				fixed4 UVS_DIR = (fixed4(
					(_FracTimeX / 2 + _CosTime.x * 0.02)	* _WaterTextureTiling.z,
					(_FracTimeX / 2 + _SinTime.x * 0.03)	* _WaterTextureTiling.w ,
					(_FracTimeX / 2 + _SinTime.x * 0.05 + 0.5)	* _WaterTextureTiling.z  / SCSPDF_DIV,
					(_FracTimeX / 2 + _CosTime.x * 0.04 + 0.5)* _WaterTextureTiling.w  / SCSPDF_DIV
				)1);
			#else
				fixed4 UVS_DIR = (fixed4(\
					(_FracTimeX / 2 + _CosTime.x * 0.04)	* _WaterTextureTiling.z ,
					(_FracTimeX / 2 + _SinTime.x * 0.05)	* _WaterTextureTiling.w ,
					(_FracTimeX / 2 + _CosTime.x * 0.02 + 0.5)	* _WaterTextureTiling.z  / SCSPDF_DIV,
					(_FracTimeX / 2 + _SinTime.x * 0.03 + 0.5) * _WaterTextureTiling.w  / SCSPDF_DIV
				));
			#endif*/
			// _FracTimeFull * _AnimMove.x
			// _FracTimeFull * _AnimMove.y

#if defined(WAVES_MAP_CROSS)
				o.uvscroll = float4(
					o.uv.x + _Frac_UVS_DIR.x + _Frac_WaterTextureTilingTime_m_AnimMove.x,
					o.uv.y + _Frac_UVS_DIR.y + _Frac_WaterTextureTilingTime_m_AnimMove.y,
					-o.uv.y + 0.5 + _Frac_UVS_DIR.z - _Frac_WaterTextureTilingTime_m_AnimMove.y,
					-o.uv.x + 0.5 + _Frac_UVS_DIR.w - _Frac_WaterTextureTilingTime_m_AnimMove.x
					);
#else
				o.uvscroll = float4(
					o.uv.x + _Frac_UVS_DIR.x + _Frac_WaterTextureTilingTime_m_AnimMove.x,
					o.uv.y + _Frac_UVS_DIR.y + _Frac_WaterTextureTilingTime_m_AnimMove.y,
					-o.uv.x + 0.5 + _Frac_UVS_DIR.z - _Frac_WaterTextureTilingTime_m_AnimMove.x,
					-o.uv.y + 0.5 + _Frac_UVS_DIR.w - _Frac_WaterTextureTilingTime_m_AnimMove.y
					);
#endif


				/*fixed4 UVS_DIR = o.uvscroll;
				fixed2 UVS_SPD = fixed2(_WaterTextureTiling.z, _WaterTextureTiling.w);
				fixed4 UVSCROLL;
				UVS(o.uv, UVS_DIR, UVS_SPD, UVSCROLL)
					o.uvscroll = (UVSCROLL);*/

#if defined(DETILE_LQ)
				MYFIXED DX = (sin((o.uv.x + o.uv.y)* _DetileFriq + _Frac2PITime)) * _DetileAmount;
				MYFIXED DY = (sin(o.uv.y * _DetileFriq + _Frac2PITime)) * _DetileAmount;
				o.uv.x += DX;
				o.uv.y += DY;
				o.uvscroll.x += DX;
				o.uvscroll.y += DY;
#if defined(DETILE_SAME_DIR)
				o.uvscroll.z -= DX;
				o.uvscroll.w -= DY;
#else
				o.uvscroll.z += DX;
				o.uvscroll.w += DY;
#endif
#endif





#if defined(SHORE_WAVES) && !defined(DEPTH_NONE)  

#if (defined(SHORE_UNWRAP_STRETCH_1) || defined(SHORE_UNWRAP_STRETCH_2)) && !defined(MINIMUM_MODE)
				/*fixed point_uvw = v.vertex.xz;
				fixed uvw = distance(point_uvw, v.vertex.xz);
				point_uvw.x += _VertexSize.z;
				fixed uvw += distance(point_uvw, v.vertex.xz);
				(v.vertex.xz - _VertexSize.xy)  _VertexSize.z;*/
				fixed3 point_uvw = _VertexSize;
				//point_uvw.y += _VertexSize.w;
				//o.vfoam.z = 0; //v.vertex.x - _VertexSize.x + v.vertex.z - _VertexSize.y + distance(point_uvw, v.vertex);
				//o.vfoam.z += distance(point_uvw, v.vertex);
				point_uvw.x += _VertexSize.z;
				//o.vfoam.z += distance(point_uvw, v.vertex);
				point_uvw.z += _VertexSize.w;
				//o.vfoam.z += distance(point_uvw, v.vertex);
				point_uvw.x -= _VertexSize.z;
				//o.vfoam.z += distance(point_uvw, v.vertex);
				//o.vfoam.z /= 3;


#if defined(SHORE_UNWRAP_STRETCH_DOT) || defined(SHORE_UNWRAP_STRETCH_OFFSETDOT)
				fixed sincor = 4 * sin(length((v.vertex - point_uvw) / 4));
#if defined(SHORE_UNWRAP_STRETCH_OFFSETDOT)
				fixed dot1 = dot(normalize(v.vertex.xz - _VertexSize.xy * 2), fixed2(0, 1));
				fixed dot2 = dot(normalize(v.vertex.xz - _VertexSize.xy * 2), fixed2(1, 0));
				fixed dot3 = dot(normalize(v.vertex.xz - _VertexSize.xy * 2), fixed2(1, 1));
#else
				fixed dot1 = dot(normalize(v.vertex.xz), fixed2(0, 1));
				fixed dot2 = dot(normalize(v.vertex.xz), fixed2(1, 0));
				fixed dot3 = dot(normalize(v.vertex.xz), fixed2(1, 1));
#endif
				dot1 = abs(dot1);
				dot2 = abs(dot2);
				dot3 = abs(dot3);
				//dot1 += dot3;
				//dot2 += dot3;
				o.vfoam.z = (v.vertex.x - _VertexSize.x) * dot1 + (v.vertex.z - _VertexSize.y) * dot2 + sincor;
#else 
				fixed sincor = 4 * sin(length((v.vertex - point_uvw) / 4));
				fixed d1 = length(v.vertex.xz - _VertexSize.xy);
				//fixed d2 = length(v.vertex.xz - _VertexSize.xy + _VertexSize.zw);
				o.vfoam.z = v.vertex.x - _VertexSize.x + v.vertex.z - _VertexSize.y + sincor + d1;
#endif
#endif //defined(SHORE_UNWRAP_STRETCH_1) || defined(SHORE_UNWRAP_STRETCH_2)


#endif

#if !defined(SKIP_FLAT_SPECULAR)
				o.vfoam.w = normalize(lerp(fixed3(0.1, 0.28, 0.55), fixed3(0.1, 0.98, 0.05), _Light_FlatSpecTopDir));
#endif

				//MYFLOAT2 LowDistUv = o.uv.xy * _LOW_DISTOR_Tile + _Frac2PITime * floor(_LOW_DISTOR_Speed);
				MYFLOAT2 LowDistUv2 = texcoord.xy * 20 * _LOW_DISTOR_Tile + _Frac2PITime * floor(_LOW_DISTOR_Speed);
				o.vfoam.x = (sin(LowDistUv2.x))*0.1;
				o.vfoam.y = (cos(LowDistUv2.y))*0.1;
				/*o.vfoam.x += cos(LowDistUv2.x / 35 + o.vfoam.x * 5)*0.1;
				o.vfoam.y += sin(LowDistUv2.y / 53 + o.vfoam.y * 5)*0.1;*/

#if !defined(SKIP_FOG)
#ifdef ORTO_CAMERA_ON
				//o.fogCoord = v.vertex;
				fixed3 eyePos = UnityObjectToViewPos(v.vertex);
				o.fogCoord.x = length(eyePos);
#else
				UNITY_TRANSFER_FOG(o, o.pos);
#endif
#endif

#if defined(USE_BLENDANIMATED_MAINTEX) && !defined(SKIP_MAIN_TEXTURE)
				MYFLOAT2 LowDistUv = texcoord.xy * _LOW_DISTOR_MAINTEX_Tile + _Frac2PITime * floor(_LOW_DISTOR_MAINTEX_Speed * 2);
				o.fogCoord.z = (sin(LowDistUv.x))*0.1;
				o.fogCoord.w = (cos(LowDistUv.y))*0.1;
#endif
				//o.vfoam.x = (sin(o.uv.x * _LOW_DISTOR_Tile + _Frac2PITime * floor(_LOW_DISTOR_Speed)))*0.1;
				//o.vfoam.y = (cos(o.uv.y * _LOW_DISTOR_Tile + _Frac2PITime * floor(_LOW_DISTOR_Speed)))*0.1;

				//o.fogCoord.xyzw = 0;

#if !defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR) ||  !defined(SKIP_FLAT_SPECULAR)
				o.helpers.w = (1 - dot(fixed3(0, 1, 0), -_LightDir))*0.65 + 0.35;
#endif


				/*fixed4 loduv;
				loduv.xy = o.uv.xy / 8;
					loduv.zw = 1.0;
				fixed msk = tex2Dlod(_MainTex, loduv).r;


				fixed fr = frac((_FracTimeXX) * 4 + msk * 2 + o.uv.x + o.uv.y) * 2;
				fixed time = abs(1 - fr);
				o.valpha.z = time;*/
				//time = frac(1 +  time * time *time);
				//	



				return o;
			}//!vert














				// , UNITY_VPOS_TYPE screenPos : VPOS
			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 tex;
				fixed zdepth;
				fixed raw_zdepth;
			fixed2 tnormal_GRAB;
			fixed2 tnormal_TEXEL;
			fixed3 tnormal;
					fixed3 worldNormal;
			fixed3 wViewDir;
			fixed2 DFACTOR;
			float2 DETILEUV = i.uv.xy;
			float4 UVSCROLL = i.uvscroll;
			fixed4 color = fixed4(0, 0, 0, 1);
			fixed APS;
			#if defined(HAS_REFLECTION)
			fixed3 reflectionColor;
			#endif
			#if defined(HAS_REFRACTION)
			fixed3 refractionColor;
			fixed lerped_refr;
			#endif
#if defined(USE_DEPTH_FOG) || defined(USE_FOG)
			fixed3 unionFog;
#endif

			half2 fb_wcoord = (i.screen.xy / i.screen.w);
			half RES = 16;
			half hRES = RES / 2;
			fixed fb_Yof = floor(frac((fb_wcoord.y * _ScreenParams.y) / RES)*hRES)*hRES;
			fixed fb_sp = frac((fb_wcoord.x * _ScreenParams.x + fb_Yof + _FrameRate) / RES) - DOWNSAMPLING_SAMPLE_SIZE;
			//if (fb_sp < 0) 	return tex2D(_FrameBuffer, fb_wcoord);
			fixed cond = saturate(fb_sp);
			//#define cond fb_sp < 0
			if (cond) {

			#if defined(WRONG_DEPTH)
						fixed f = min(0.1, frac(i.screen.x / 10)) * 10;
					return fixed4(f, f, f, 1);
			#endif

					/*i.uv.x += (sin(i.uv.x * 5 + i.uv.y * 5)) / _WaterTextureTiling.x / 2;
					i.uv.y += (sin(i.uv.y * 5)) / _WaterTextureTiling.y / 2;*/


					///////////////////////////////////////////////////

					//i.vfoam.x = (sin(i.uv.x * 20 + _Frac2PITime * 8))*0.1;
					//i.vfoam.y = (cos(i.uv.y * 20 + _Frac2PITime * 8))*0.1;
					///////////////////////////////////////////////////

			#if defined(SKIP_BLEND_ANIMATION) || defined(SHADER_API_GLES)
			#if defined(USE_4X_BUMP)
					fixed4 bump1A = (tex2D(_BumpMap, UVSCROLL.wx));
					fixed3 tnormal1A = UnpackNormal(bump1A);
					fixed4 bump2A = (tex2D(_BumpMap, UVSCROLL.yz));
					fixed3 tnormal2A = UnpackNormal(bump2A);
			#endif

					fixed4 bump1 = (tex2D(_BumpMap, UVSCROLL.xy));
					fixed3 tnormal1 = UnpackNormal(bump1);
					fixed4 bump2 = (tex2D(_BumpMap, UVSCROLL.zw));
					fixed3 tnormal2 = UnpackNormal(bump2);

			#if defined(USE_4X_BUMP)
					tnormal1 = (tnormal1 + tnormal1A)* 0.5;
					tnormal2 = (tnormal2 + tnormal2A)* 0.5;
			#endif

			#else
					float2 t1uv = UVSCROLL.xy + float2(i.blend_index.z, i.blend_index.z / 2);
					fixed4 t1tex = tex2D(_BumpMap, t1uv);
					fixed3 t1 = UnpackNormal(t1tex);

					float2 t2uv = UVSCROLL.zw + float2(i.blend_index.w, i.blend_index.w / 2);
					fixed4 t2tex = tex2D(_BumpMap, t2uv);
					fixed3 t2 = UnpackNormal(t2tex);

					float2 t3uv = UVSCROLL.xy + float2(i.blend_index.x, i.blend_index.x / 2);
					fixed4 t3tex = tex2D(_BumpMap, t3uv);
					fixed3 t3 = UnpackNormal(t3tex);

					float2 t4uv = UVSCROLL.zw + float2(i.blend_index.y, i.blend_index.y / 2);
					fixed4 t4tex = tex2D(_BumpMap, t4uv);
					fixed3 t4 = UnpackNormal(t4tex);
			#if defined(USE_4X_BUMP)
					fixed4 t1texA = tex2D(_BumpMap, float2(t1uv.y , t1uv.x));
					fixed3 t1A = UnpackNormal(t1texA);
					fixed4 t2texA = tex2D(_BumpMap, float2(t2uv.y, t2uv.x));
					fixed3 t2A = UnpackNormal(t2texA);
					fixed4 t3texA = tex2D(_BumpMap, float2(t3uv.y, t3uv.x));
					fixed3 t3A = UnpackNormal(t3texA);
					fixed4 t4texA = tex2D(_BumpMap, float2(t4uv.y, t4uv.x));
					fixed3 t4A = UnpackNormal(t4texA);
					t1 = (t1 + t1A)* 0.5;
					t2 = (t2 + t2A)* 0.5;
					t3 = (t3 + t3A)* 0.5;
					t4 = (t4 + t4A)* 0.5;
			#endif

					t1 *= (i.blend_time.x);
					t2 *= (i.blend_time.y);
					t3 *= (i.blend_time.z);
					t4 *= (i.blend_time.w);

					fixed3 tnormal1 = (t1 + t3);
					fixed3 tnormal2 = (t2 + t4);
			#endif


			#if !defined(SKIP_WNN)
					//tnormal1 = normalize(tnormal1);
					//tnormal2 = normalize(tnormal2);
			#endif

					//return 1-sum/50;
					fixed3 avt2 = (tnormal1 + tnormal2) * 0.5;

			#if defined(ALTERNATIVE_NORMALS_MIX)
					fixed sum = (tnormal1.z + tnormal2.z);
					fixed av15 = 1 - ((sum)) / _BumpMixAmount; //ues blend value and pop //used saturate
					fixed3 avt1 = fixed3(0, 0, 1);
					 tnormal = lerp(avt1, avt2, av15);
					 //tnormal = (tnormal* 0.3 + avt2 * 0.7);
					 tnormal = (tnormal + avt2)* 0.5;
					 //tnormal = (tnormal* 0.7 + avt2*0.3) ;
			 #else
					//fixed av15 = ((sum - 1.95)) / 0.05; //ues blend value and pop
					 tnormal = avt2;
			#endif
					 //return av15;
						 //av15 = saturate(av15);

					 tnormal.xy *= _BumpAmount;



					 /*#if defined(UF_AMOUNTMASK)
							 float2 amtile = DETILEUV * _UF_NMASK_Tile + _UF_NMASK_offset;
							 fixed amm = tex2D(_UF_NMASK_Texture, amtile).r;
							 fixed amount_mask = saturate(amm * _UF_NMASK_Contrast - _UF_NMASK_Contrast  / 2 + _UF_NMASK_Brightnes);
							 //fixed amount_mask = min(_AMOUNTMASK_max, (tex2D(_UF_AMountMaskTexture, DETILEUV * _AMOUNTMASK_Tile + _AMOUNTMASK_offset).r * _AMOUNTMASK_Amount + _AMOUNTMASK_min));
					 #if defined(AMOUNT_MASK_DEBUG)
							 return float4(amount_mask.rrr, 1);
					 #endif
							 tnormal.xy *= amount_mask;
					 #endif*/
					 #if defined(UF_AMOUNTMASK)
					 #if defined(_UF_NMASK_USE_MAINTEX)
					 #define MASK_TEX _MainTex
					 #else
					 #define MASK_TEX _UF_NMASK_Texture
					 #endif

							 MYFLOAT2 amtile = DETILEUV * _UF_NMASK_Tile + _UF_NMASK_offset;
					 #if defined(_UF_NMASK_G)
							 fixed amm = tex2D(MASK_TEX, amtile).g;
					 #elif defined(_UF_NMASK_B)
							 fixed amm = tex2D(MASK_TEX, amtile).b;
					 #elif defined(_UF_NMASK_A)
							 fixed amm = tex2D(MASK_TEX, amtile).a;
					 #else
							 fixed amm = tex2D(MASK_TEX, amtile).r;
					 #endif
							 fixed amount_mask = saturate(amm * _UF_NMASK_Contrast - _UF_NMASK_Contrast / 2 + _UF_NMASK_Brightnes);
					 #if defined(AMOUNT_MASK_DEBUG)
							 return float4(amount_mask.rrr, 1);
					 #endif
							 tnormal.xy *= amount_mask;
					 #endif



							 //fixed3 inputNormal = fixed3(i.tspace0.w, i.tspace2.w, i.tspace1.w);
							 //tnormal.xy += inputNormal.xy;

							 //tnormal = normalize(tnormal);
							 //fixed3 rawtnormal = normalize(tnormal);
							  tnormal_TEXEL = tnormal.xy / _VertexSize.zw / _ObjectScale.xz * 2;
							  tnormal_GRAB = tnormal.xy;

							  ///////////////////////////////////////////////////



							  ///////////////////////////////////////////////////

					  //#define WVIEW normalize( i.wPos.xyz - _WorldSpaceCameraPos.xyz)
							   wViewDir = (_WorldSpaceCameraPos.xyz - i.wPos.xyz) / i.wPos.w;

							   //worldNormal.xz = tnormal.x * i.tspace.xy + tnormal.y * fixed2(-i.tspace.y, i.tspace.x);
							   worldNormal.xz = tnormal.xy + i.helpers.xy;
							   worldNormal.y = tnormal.z;
							   /*	fixed3 worldNormal;
								   worldNormal.x = dot(i.tspace0.xyz, tnormal);
								   worldNormal.y = dot(i.tspace1.xyz, tnormal);
								   worldNormal.z = dot(i.tspace2.xyz, tnormal);*/
								   //fixed2 tspace = fixed2(1, 0);
						   #if !defined(SKIP_WNN)
								   worldNormal = normalize(worldNormal);
						   #endif
								   ///////////////////////////////////////////////////

						   #if defined(DEPTH_NONE)
									 zdepth = 1;
						   #else
						   #if defined(HAS_CAMERA_DEPTH)
						   #ifdef ORTO_CAMERA_ON
								   fixed4 UV = i.screen;
						   #else
								   fixed4 UV = (i.screen);
						   #endif
					   #if defined(FIX_DISTORTION)
								   fixed before_zdepth_raw = GET_Z(i, UV);
					   #endif

					   #if defined(USE_ZD_DISTOR)
								   fixed dv01 = _RefrDistortionZ * 30;
								   UV.xy += tnormal_TEXEL * dv01;
					   #endif

					   #if defined(FIX_DISTORTION)
									zdepth = max(before_zdepth_raw, GET_Z(i, UV));
					   #else
									zdepth = GET_Z(i, UV);
					   #endif
									raw_zdepth = zdepth;



						   #else
								   fixed2 UV = i.uv.zw;
								   /*#if defined(FIX_DISTORTION)
											   fixed before_zdepth_raw = GET_BAKED_Z(i, UV);
								   #endif*/
								   #if defined(USE_ZD_DISTOR)
											   fixed dv01 = _RefrDistortionZ * 0.3;
											   UV.xy += tnormal_TEXEL * dv01;
								   #endif

												zdepth = GET_BAKED_Z(UV);
												raw_zdepth = zdepth;

									   #if !defined(SKIP_Z_CALC_DEPTH_FACTOR)
												/*fixed av10 = saturate(zdepth / _RefrDeepFactor) / (i.VER_TEX.z* 0.8 + 0.2);
												fixed2 DFACTOR = i.VER_TEX.xy * av10;*/
												fixed av10 = saturate(zdepth / _RefrDeepFactor);
												 DFACTOR = i.VER_TEX.xy * av10;
									#define HAS_DEPTH_FACTOR = 1;
												zdepth = GET_BAKED_Z(UV - DFACTOR);
										#endif

										#endif


												/*#if defined(HAS_CAMERA_DEPTH)
															MYFIXED zdepth = GET_Z(i, (UV));
												#if defined(HAS_REFRACTION) && !defined(SKIP_ZDEPTH_REFRACTION_DISTORTIONCORRECTION)
															zdepth = max(before_zdepth_raw, zdepth);
												#endif
												#else

												#if defined(HAS_REFRACTION) && !defined(SKIP_ZDEPTH_REFRACTION_DISTORTIONCORRECTION)
															MYFIXED zdepth = max(before_zdepth_raw, GET_BAKED_Z(UV));
												#else
															MYFIXED zdepth = GET_BAKED_Z(UV);
												#endif

												#endif*/



													#if defined(DEGUB_Z)
															return fixed4(zdepth / 10, zdepth / 10, zdepth / 10, 1);
													#endif
													#endif 


															///////////////////////////////////////////////////

												#if defined(HAS_Z_AFFECT_BUMP)
															fixed zaff = saturate((zdepth - _BumpZFade) / _BumpZOffset + _BumpZFade);
												#if defined(INVERT_Z_AFFECT_BUMP)
															zaff = 1 - zaff;
												#endif
															tnormal.xy *= zaff;
															tnormal_GRAB.xy *= zaff;
															tnormal_TEXEL.xy *= zaff;
															//worldNormal.y += (worldNormal.x + worldNormal.z) * zaff * 3;
															worldNormal.xz *= zaff;
															worldNormal = normalize(worldNormal);
												#endif
												#if defined(DEBUG_NORMALS)
															return (tnormal.x + tnormal.y) * 3;
												#endif


															///////////////////////////////////////////////////
												//#ifdef WATER_DOWNSAMPLING

												//#endif
												#include "EModules Water Model Refraction.cginc"
												#include "EModules Water Model Texture.cginc"

															///////////////////////////////////////////////////

			
//{//128 instructions


		   ///////////////////////////////////////////////////

#if defined(APPLY_REF)
		   fixed3 apr01 = refractionColor;
#if defined(APPLY_REF_FOG)
		   apr01 = lerp(apr01, unionFog, unionFog.b);
#endif

		   fixed apamount05 = _RefrTopAmount * (1 - lerped_refr) + _RefrDeepAmount * lerped_refr;
		   apr01 *= apamount05;


#if defined(REFRACTION_DEBUG_RGB)
		   return float4(apr01.rgb, 1);
#endif

#if !defined(SKIP_TEX)
		   fixed apr05 = (tex.b / 2 + tex.b * MAIN_TEX_Contrast * abs(tnormal.x));
		   //if (apr05 < 0.1) return 1;

		   //fixed apr05 = tex.b ;
#else
		   fixed apr05 = 0;
#endif
#if defined(USE_REFRACTION_BLEND_FRESNEL) && defined(REFLECTION_NONE)
#endif
		   apr05 += ((1 - wViewDir.y) * _RefractionBLendAmount);

		   //apr05 = (apr05); //#saturate to fix
		   //return float4(apr05.rrr, 1);
		   fixed3 apr15 = tex * _ReplaceColor;
#if !defined(REFLECTION_NONE)
		   //#include "EModules Water Model Reflection.cginc"
				   //	apr15 *= reflectionColor;
#endif
		   color.rgb = lerp(apr01, apr15, apr05);
#else
		   color.rgb = tex * _ReplaceColor;
#endif

		   ///////////////////////////////////////////////////

		   color.rgb *= _MainTexColor.rgb;
		   ///////////////////////////////////////////////////

#if !defined(REFLECTION_NONE)

#include "EModules Water Model Reflection.cginc"

#endif


		///////////////////////////////////////////////////

#if defined(SHORE_WAVES) && !defined(DEPTH_NONE)

#include "EModules Water Model Shore.cginc"

#endif
		   fixed fresnelFac = 1;
		   fixed specularLight = 0;
#include "EModules Water Model Lighting.cginc"
		   color.rgb += specularLight;
#if !defined(SKIP_LIGHTING) 
		   color.rgb += lightLight;
#endif

		   ///////////////////////////////////////////////////

		   //fixed4 foamColor = fixed4(0, 0, 0, 1);


		   ///////////////////////////////////////////////////

#include "EModules Water Model PostProcess.cginc"

		///////////////////////////////////////////////////
		   return color;
}
else { return tex2D(_FrameBuffer, fb_wcoord); }
//POST_PROCESS(i, color, foamColor, zdepth);
		   //color.rgb = zdepth / 20;

}//!frag


            
            ENDCG
        }
        //UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
        //#if SHADER_TARGET < 30
        //#else
        //#endif
    }
    //Fallback "Mobile/VertexLit"
}
