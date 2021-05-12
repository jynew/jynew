//forward base setup
#ifndef MK_TOON_FORWARD_BASE_SETUP
	#define MK_TOON_FORWARD_BASE_SETUP

	#ifndef MK_TOON_FWD_BASE_PASS
		#define MK_TOON_FWD_BASE_PASS 1
	#endif

	#include "UnityGlobalIllumination.cginc"
	#include "UnityCG.cginc"
	#include "AutoLight.cginc"

	#include "../Common/MKToonV.cginc"
	#include "../Common/MKToonInc.cginc"
	#include "../Forward/MKToonForwardIO.cginc"
	#include "../Surface/MKToonSurfaceIO.cginc"
	#include "../Common/MKToonLight.cginc"
	#include "../Surface/MKToonSurface.cginc"
#endif