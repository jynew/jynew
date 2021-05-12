//outline setup
#ifndef MK_TOON_OUTLINE_ONLY
	#define MK_TOON_OUTLINE_ONLY

	#ifndef MKTOON_OUTLINE_PASS_ONLY
		#define MKTOON_OUTLINE_PASS_ONLY 1
	#endif
	
	#include "UnityGlobalIllumination.cginc"
	#include "UnityCG.cginc"

	#include "../Common/MKToonDef.cginc"
	#include "../Common/MKToonV.cginc"
	#include "../Common/MKToonInc.cginc"
	#include "MKToonOutlineOnlyIO.cginc"
#endif