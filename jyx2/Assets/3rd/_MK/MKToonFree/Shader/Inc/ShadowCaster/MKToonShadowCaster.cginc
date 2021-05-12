//shadow rendering input and output
#ifndef MK_TOON_SHADOWCASTER
	#define MK_TOON_SHADOWCASTER

	/////////////////////////////////////////////////////////////////////////////////////////////
	// VERTEX SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	void vertShadowCaster (
		 VertexInputShadowCaster v,
		 out VertexOutputShadowCaster o
		 #ifdef UNITY_STEREO_INSTANCING_ENABLED
			,out VertexOutputStereoShadowCaster os
		 #endif
		 ,out float4 pos : SV_POSITION
		)
	{
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_OUTPUT(VertexOutputShadowCaster, o);
		#ifdef UNITY_STEREO_INSTANCING_ENABLED
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(os);
		#endif

		TRANSFER_SHADOW_CASTER_NOPOS(o, pos)
	}

	/////////////////////////////////////////////////////////////////////////////////////////////
	// FRAGMENT SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	half4 fragShadowCaster 
		(
			VertexOutputShadowCaster o
			#if UNITY_VERSION >= 20171
				,UNITY_POSITION(vpos)
			#else
				,UNITY_VPOS_TYPE vpos : VPOS
			#endif
		) : SV_Target
	{	
		SHADOW_CASTER_FRAGMENT(o)
	}			
#endif