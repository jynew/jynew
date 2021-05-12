//uniform variables
#ifndef MK_TOON_V
	#define MK_TOON_V

	/////////////////////////////////////////////////////////////////////////////////////////////
	// UNIFORM VARIABLES
	/////////////////////////////////////////////////////////////////////////////////////////////

	//enabled uniform variables only if needed

	//Main
	uniform fixed4 _Color;
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform half _Brightness;

	//Normalmap
	uniform sampler2D _BumpMap;

	//Light
	#ifndef UNITY_LIGHTING_COMMON_INCLUDED
		uniform fixed4 _LightColor0;
	#endif
	uniform half _LightThreshold;

	//Render
	uniform half _LightSmoothness;
	uniform half _RimSmoothness;

	//Custom shadow
	uniform fixed3 _ShadowColor;
	uniform fixed3 _HighlightColor;
	uniform fixed _ShadowIntensity;

	//Rim
	uniform fixed3 _RimColor;
	uniform half _RimSize;
	uniform fixed _RimIntensity;

	//Specular
	uniform half _Shininess;
	#ifndef UNITY_LIGHTING_COMMON_INCLUDED
		uniform fixed3 _SpecColor;
	#endif
	uniform fixed _SpecularIntensity;

	//Outline
	#ifdef MKTOON_OUTLINE_PASS_ONLY
		uniform fixed4 _OutlineColor;
		uniform half _OutlineSize;
	#endif

	//Emission
	uniform fixed3 _EmissionColor;
#endif