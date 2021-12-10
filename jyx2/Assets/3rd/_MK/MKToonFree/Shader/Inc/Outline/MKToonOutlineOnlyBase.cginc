//base include for outline
#ifndef MK_TOON_OUTLINE_ONLY_BASE
	#define MK_TOON_OUTLINE_ONLY_BASE
	/////////////////////////////////////////////////////////////////////////////////////////////
	// VERTEX SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	VertexOutputOutlineOnly outlinevert(VertexInputOutlineOnly v)
	{
		UNITY_SETUP_INSTANCE_ID(v);
		VertexOutputOutlineOnly o;
		UNITY_INITIALIZE_OUTPUT(VertexOutputOutlineOnly, o);
		UNITY_TRANSFER_INSTANCE_ID(v,o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		//v.vertex.xyz += normalize(v.normal) * _OutlineSize;
		o.pos = UnityObjectToClipPos(v.vertex);
		float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, normalize(v.normal));
		float3 ndcNormal = mul((float3x3)UNITY_MATRIX_P, normalize(viewNormal)) * o.pos.w; //变换法线到ndc空间
		o.pos.xy += 0.01 * _OutlineSize * ndcNormal.xy * 8; //8是新旧效果的大概参数倍率， 免去重新调参
		o.color = _OutlineColor;

		UNITY_TRANSFER_FOG(o,o.pos);
		return o;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////
	// FRAGMENT SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	fixed4 outlinefrag(VertexOutputOutlineOnly o) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(o);
		UNITY_APPLY_FOG(o.fogCoord, o.color);
		return o.color;
	}
#endif