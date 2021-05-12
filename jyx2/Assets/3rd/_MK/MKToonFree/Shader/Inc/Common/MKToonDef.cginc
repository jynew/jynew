//Basic definitions for the rendering
#ifndef MK_TOON_DEF
	#define MK_TOON_DEF
	/////////////////////////////////////////////////////////////////////////////////////////////
	// DEF
	/////////////////////////////////////////////////////////////////////////////////////////////
	#ifndef T_H
		#define T_H 0.25
	#endif
	#ifndef T_V
		#define T_V 0.5
	#endif
	#ifndef T_O
		#define T_O 1.0
	#endif
	#ifndef T_G
		#define T_G 1.75
	#endif
	#ifndef T_D
		#define T_D 2.0
	#endif
	#ifndef T_T
		#define T_T 10.0
	#endif

	#ifndef SHINE_MULT
		#define SHINE_MULT 64
	#endif

	//Emission
	#if MK_TOON_META_PASS || MK_TOON_FWD_BASE_PASS
		#ifndef _MKTOON_EMISSION
			#define _MKTOON_EMISSION 1
		#endif
	#endif
#endif