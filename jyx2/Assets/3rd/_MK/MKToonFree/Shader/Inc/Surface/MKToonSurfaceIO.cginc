//surface input and output
#ifndef MK_TOON_SURFACE_IO
	#define MK_TOON_SURFACE_IO

	/////////////////////////////////////////////////////////////////////////////////////////////
	// MKTOON SURFACE
	/////////////////////////////////////////////////////////////////////////////////////////////

	//Dynamic precalc struct
	struct MKToonPCP
	{
		half VdotN;
		half NdotL;
		half MLrefNdotV;
		half3 NormalDirection;
		half3 HV;
		half3 LightDirection;
		half3 LightColor;
		half LightAttenuation;
		half3 LightColorXAttenuation;
		half3 ViewDirection;
		half NdotHV;
	};

	//dynamic surface struct
	struct MKToonSurface
	{
		MKToonPCP Pcp;
		fixed4 Color_Out;
		fixed3 Color_Albedo;
		fixed Alpha;
		fixed3 Color_Diffuse;
		fixed3 Color_Specular;
		#if _MKTOON_EMISSION
			fixed3 Color_Emission;
		#endif
	};
#endif