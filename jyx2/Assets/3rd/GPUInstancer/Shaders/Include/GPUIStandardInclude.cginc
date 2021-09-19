#ifndef GPU_INSTANCER_STANDARD_INCLUDED
#define GPU_INSTANCER_STANDARD_INCLUDED

// FORWARD RENDERING:
#if !UNITY_STANDARD_SIMPLE
half4 fragForwardBaseInternalGPUI(VertexOutputForwardBase i)
{
    UNITY_SETUP_INSTANCE_ID(i);
#if UNITY_VERSION >= 201711
    UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);
#endif

    FRAGMENT_SETUP(s);
								
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

    UnityLight mainLight = MainLight();
    UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);

    half occlusion = Occlusion(i.tex.xy);
    UnityGI gi = FragmentGI(s, occlusion, i.ambientOrLightmapUV, atten, mainLight);

    half4 c = UNITY_BRDF_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
    c.rgb += Emission(i.tex.xy);

#ifdef UNITY_EXTRACT_FOG_FROM_EYE_VEC
    UNITY_EXTRACT_FOG_FROM_EYE_VEC(i);
#endif
#if UNITY_VERSION > 201820
    UNITY_APPLY_FOG(_unity_fogCoord, c.rgb);
#else
    UNITY_APPLY_FOG(i.fogCoord, c.rgb);
#endif
    return OutputForward(c, s.alpha);
}
#endif

#if UNITY_STANDARD_SIMPLE
    half4 fragBaseAllGPUI (VertexOutputBaseSimple i) : SV_Target { return fragForwardBaseSimpleInternal(i); }
#else
    half4 fragBaseAllGPUI(VertexOutputForwardBase i) : SV_Target { return fragForwardBaseInternalGPUI(i); }
#endif

// DEFERRED RENDERING
#ifndef UNITY_POSITION
#define UNITY_POSITION(pos) float4 pos : SV_POSITION
#endif

struct VertexOutputDeferredGPUI
{
    UNITY_POSITION(pos);
    float4 tex : TEXCOORD0;
#if UNITY_VERSION == 565 || UNITY_VERSION == 201710 || UNITY_VERSION == 201711 || UNITY_VERSION == 201712 || UNITY_VERSION == 201713 || UNITY_VERSION == 201730 || UNITY_VERSION == 201731 || UNITY_VERSION == 201720 || UNITY_VERSION == 201721 || UNITY_VERSION == 201722
    half3 eyeVec : TEXCOORD1;
    half4 tangentToWorldAndPackedData[3] : TEXCOORD2; // [3x3:tangentToWorld | 1x3:viewDirForParallax or worldPos]
#else
    float3 eyeVec : TEXCOORD1;
    float4 tangentToWorldAndPackedData[3] : TEXCOORD2; // [3x3:tangentToWorld | 1x3:viewDirForParallax or worldPos]
#endif
    
    half4 ambientOrLightmapUV : TEXCOORD5; // SH or Lightmap UVs

#if UNITY_REQUIRE_FRAG_WORLDPOS && !UNITY_PACK_WORLDPOS_WITH_TANGENT
            float3 posWorld                     : TEXCOORD6;
#endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

VertexOutputDeferredGPUI vertDeferredGPUI(VertexInput v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    VertexOutputDeferredGPUI o;
    UNITY_INITIALIZE_OUTPUT(VertexOutputDeferredGPUI, o);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
#if UNITY_REQUIRE_FRAG_WORLDPOS
#if UNITY_PACK_WORLDPOS_WITH_TANGENT
	o.tangentToWorldAndPackedData[0].w = posWorld.x;
	o.tangentToWorldAndPackedData[1].w = posWorld.y;
	o.tangentToWorldAndPackedData[2].w = posWorld.z;
#else
    o.posWorld = posWorld.xyz;
#endif
#endif
    o.pos = UnityObjectToClipPos(v.vertex);

    o.tex = TexCoords(v);
    o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
    float3 normalWorld = UnityObjectToWorldNormal(v.normal);
#ifdef _TANGENT_TO_WORLD
	float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

	float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
	o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
	o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
	o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
#else
    o.tangentToWorldAndPackedData[0].xyz = 0;
    o.tangentToWorldAndPackedData[1].xyz = 0;
    o.tangentToWorldAndPackedData[2].xyz = normalWorld;
#endif

    o.ambientOrLightmapUV = 0;
#ifdef LIGHTMAP_ON
	o.ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#elif UNITY_SHOULD_SAMPLE_SH
	o.ambientOrLightmapUV.rgb = ShadeSHPerVertex (normalWorld, o.ambientOrLightmapUV.rgb);
#endif
#ifdef DYNAMICLIGHTMAP_ON
	o.ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#endif

#ifdef _PARALLAXMAP
	TANGENT_SPACE_ROTATION;
	half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
	o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
	o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
	o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
#endif

    return o;
}
    
void fragDeferredGPUI(VertexOutputDeferredGPUI i,
				      out half4 outGBuffer0 : SV_Target0,
				      out half4 outGBuffer1 : SV_Target1,
				      out half4 outGBuffer2 : SV_Target2,
				      out half4 outEmission : SV_Target3 // RT3: emission (rgb), --unused-- (a)
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
      				  ,out half4 outShadowMask : SV_Target4       // RT4: shadowmask (rgba)
#endif
			          )
{
#if (SHADER_TARGET < 30)
    outGBuffer0 = 1;
    outGBuffer1 = 1;
    outGBuffer2 = 0;
    outEmission = 0;
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
	 outShadowMask = 1;
#endif
    return;
#endif
    UNITY_SETUP_INSTANCE_ID(i);
#if UNITY_VERSION >= 201711
    UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);
#endif

    FRAGMENT_SETUP(s);
    

	// no analytic lights in this pass
    UnityLight dummyLight = DummyLight();
    half atten = 1;

	// only GI
    half occlusion = Occlusion(i.tex.xy);
#if UNITY_ENABLE_REFLECTION_BUFFERS
	bool sampleReflectionsInDeferred = false;
#else
    bool sampleReflectionsInDeferred = true;
#endif

    UnityGI gi = FragmentGI(s, occlusion, i.ambientOrLightmapUV, atten, dummyLight, sampleReflectionsInDeferred);

    half3 emissiveColor = UNITY_BRDF_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;

#ifdef _EMISSION
	emissiveColor += Emission (i.tex.xy);
#endif

#ifndef UNITY_HDR_ON
    emissiveColor.rgb = exp2(-emissiveColor.rgb);
#endif

    UnityStandardData data;
    data.diffuseColor = s.diffColor;
    data.occlusion = occlusion;
    data.specularColor = s.specColor;
    data.smoothness = s.smoothness;
    data.normalWorld = s.normalWorld;

    UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

	// Emissive lighting buffer
    outEmission = half4(emissiveColor, 1);

	// Baked direct lighting occlusion if any
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
	outShadowMask = UnityGetRawBakedOcclusions(i.ambientOrLightmapUV.xy, IN_WORLDPOS(i));
#endif
}

#endif