//shadow input and output
#ifndef MK_TOON_SHADOWCASTER_IO
	#define MK_TOON_SHADOWCASTER_IO

	/////////////////////////////////////////////////////////////////////////////////////////////
	// INPUT
	/////////////////////////////////////////////////////////////////////////////////////////////
	struct VertexInputShadowCaster
	{
		float4 vertex : POSITION;
		//use normals for cubemapped shadows (point lights)
		//#ifndef SHADOWS_CUBE
		float3 normal : NORMAL;
		//#endif
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	/////////////////////////////////////////////////////////////////////////////////////////////
	// OUTPUT
	/////////////////////////////////////////////////////////////////////////////////////////////
	struct VertexOutputShadowCaster
	{	
		//float3 sv : TEXCOORD0;
		V2F_SHADOW_CASTER_NOPOS

		fixed deb : TEXCOORD7;
	};

	#ifdef UNITY_STEREO_INSTANCING_ENABLED
	struct VertexOutputStereoShadowCaster
	{
		UNITY_VERTEX_OUTPUT_STEREO
	};
	#endif
#endif