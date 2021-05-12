#if defined(BEAUTIFY_ORTHO)
	#if UNITY_REVERSED_Z
		#define Linear01Depth(x) (1.0-x)
		#define LinearEyeDepth(x) (1.0-x) * _ProjectionParams.z

	#else
		#define Linear01Depth(x) (x)
		#define LinearEyeDepth(x) (x * _ProjectionParams.z)
	#endif
#endif
