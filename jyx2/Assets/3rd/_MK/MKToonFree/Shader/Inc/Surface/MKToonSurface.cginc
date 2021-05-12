//surface calculations
#ifndef MK_TOON_SURFACE
	#define MK_TOON_SURFACE

	#include "../Surface/MKToonSurfaceIO.cginc"

	void PreCalcParameters(inout MKToonSurface mkts)
	{
		mkts.Pcp.VdotN = max(0.0 , dot(mkts.Pcp.ViewDirection, mkts.Pcp.NormalDirection));
		mkts.Pcp.NdotL = max(0.0 , dot(mkts.Pcp.NormalDirection, mkts.Pcp.LightDirection)*T_V + T_V);
		mkts.Pcp.HV = normalize(mkts.Pcp.LightDirection + mkts.Pcp.ViewDirection);
		mkts.Pcp.NdotHV = max(0.0, dot(mkts.Pcp.NormalDirection, mkts.Pcp.HV));
	}

	//get surface color based on blendmode and color source
	void SurfaceColor(out fixed3 albedo, out fixed alpha, float2 uv)
	{
		fixed4 c = tex2D(_MainTex, uv) * _Color;
		albedo = c.rgb;
		alpha = c.a;
	}

	//only include initsurface when not meta pass
	#ifndef MK_TOON_META_PASS
		/////////////////////////////////////////////////////////////////////////////////////////////
		// INITIALIZE SURFACE
		/////////////////////////////////////////////////////////////////////////////////////////////
		MKToonSurface InitSurface(
			#if defined(MK_TOON_FWD_BASE_PASS) || defined(MK_TOON_FWD_ADD_PASS)
				in VertexOutputForward o
			#endif
		)
		{
			//Init Surface
			MKToonSurface mkts;
			UNITY_INITIALIZE_OUTPUT(MKToonSurface,mkts);

			//modified brightness based on light type
			_LightThreshold = _LightThreshold * T_V + T_V;
			_Brightness = 1.0 - T_H;

			//init surface color
			SurfaceColor(mkts.Color_Albedo, mkts.Alpha, o.uv_Main);

			//apply alpha if transparent blendmode
			mkts.Color_Out.a = mkts.Alpha;

			//basic normal input
			mkts.Pcp.NormalDirection = WorldNormal(_BumpMap, o.uv_Main, o.tangentWorld, o.binormalWorld, o.normalWorld);

			//view direction
			mkts.Pcp.ViewDirection = normalize(_WorldSpaceCameraPos - o.posWorld).xyz;

			//lightdirection and attenuation
			#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
				mkts.Pcp.LightDirection  = normalize(_WorldSpaceLightPos0.xyz - o.posWorld.xyz);
			#else
				mkts.Pcp.LightDirection = normalize(_WorldSpaceLightPos0.xyz);
			#endif

			UNITY_LIGHT_ATTENUATION(atten, o, o.posWorld.xyz);
			mkts.Pcp.LightAttenuation = atten;
			mkts.Pcp.LightColor = _LightColor0.rgb;
			mkts.Pcp.LightColorXAttenuation = mkts.Pcp.LightColor * mkts.Pcp.LightAttenuation;

			//init precalc
			PreCalcParameters(mkts);

			#if _MKTOON_EMISSION
				mkts.Color_Emission = _EmissionColor * mkts.Color_Albedo;
			#endif
			mkts.Color_Specular = 1.0;
			return mkts;
		}
	#endif
#endif