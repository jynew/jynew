// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//vertex and fragment shader
#ifndef MK_TOON_FORWARD
	#define MK_TOON_FORWARD

	/////////////////////////////////////////////////////////////////////////////////////////////
	// VERTEX SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	VertexOutputForward vertfwd (VertexInputForward v)
	{
		UNITY_SETUP_INSTANCE_ID(v);
		VertexOutputForward o;
		UNITY_INITIALIZE_OUTPUT(VertexOutputForward, o);
		UNITY_TRANSFER_INSTANCE_ID(v,o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		//vertex positions
		o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		o.pos =  UnityObjectToClipPos(v.vertex);

		//texcoords
		o.uv_Main = TRANSFORM_TEX(v.texcoord0, _MainTex);
		
		//normal tangent binormal
		o.tangentWorld = normalize(mul(unity_ObjectToWorld, half4(v.tangent.xyz, 0.0)).xyz);
		o.normalWorld = normalize(mul(half4(v.normal, 0.0), unity_WorldToObject).xyz);
		o.binormalWorld = normalize(cross(o.normalWorld, o.tangentWorld) * v.tangent.w);

		#ifdef MK_TOON_FWD_BASE_PASS
			//lightmaps and ambient
			#ifdef DYNAMICLIGHTMAP_ON
				o.uv_Lm.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				o.uv_Lm.xy = 1;
			#endif
			#ifdef LIGHTMAP_ON
				o.uv_Lm.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				o.uv_Lm.zw = 1;
			#endif

			#ifdef MK_TOON_FWD_BASE_PASS
				#if UNITY_SHOULD_SAMPLE_SH
				//unity ambient light
					o.aLight = ShadeSH9 (half4(o.normalWorld,1.0));
				#else
					o.aLight = 0.0;
				#endif
				#ifdef VERTEXLIGHT_ON
					//vertexlight
					o.aLight += Shade4PointLights (
					unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
					unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
					unity_4LightAtten0, o.posWorld, o.normalWorld);
				#endif
			#endif
		#endif

		//vertex shadow
		#if UNITY_VERSION >= 201810
			UNITY_TRANSFER_LIGHTING(o, v.texcoord0);
		#else
			UNITY_TRANSFER_SHADOW(o, v.texcoord0);
		#endif

		#if SHADER_TARGET >= 30
			//vertex fog
			UNITY_TRANSFER_FOG(o,o.pos);
		#endif

		return o;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////
	// FRAGMENT SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	fixed4 fragfwd (VertexOutputForward o) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(o);
		//init surface struct for rendering
		MKToonSurface mkts = InitSurface(o);

		//apply lights, ambient and lightmap
		MKToonLightLMCombined(mkts, o);

		//Emission
		#if _MKTOON_EMISSION
			//apply rim lighting
			mkts.Color_Emission += RimDefault(_RimSize, mkts.Pcp.VdotN, _RimColor.rgb, _RimIntensity, _RimSmoothness);
			mkts.Color_Out.rgb += mkts.Color_Emission;
		#endif

		mkts.Color_Out.rgb = BControl(mkts.Color_Out.rgb, _Brightness);

		#if SHADER_TARGET >= 30
			//if enabled add some fog - forward rendering only
			UNITY_APPLY_FOG(o.fogCoord, mkts.Color_Out);
		#endif

		return mkts.Color_Out;
	}
#endif