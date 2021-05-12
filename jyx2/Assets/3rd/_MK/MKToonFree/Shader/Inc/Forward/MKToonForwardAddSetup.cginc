//forward add setup
#ifndef MK_TOON_FORWARD_ADD_SETUP
	#define MK_TOON_FORWARD_ADD_SETUP

	#ifndef MK_TOON_FWD_ADD_PASS
		#define MK_TOON_FWD_ADD_PASS 1 
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