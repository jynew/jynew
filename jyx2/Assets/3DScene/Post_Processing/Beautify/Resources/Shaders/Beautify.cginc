	// Copyright 2016, 2017, 2018 Kronnect - All Rights Reserved.
	
	#include "UnityCG.cginc"
	#include "BeautifyAdvancedParams.cginc"
	#include "BeautifyOrtho.cginc"
	#if defined(BEAUTIFY_ACES_FITTED)
	#include "BeautifyACESFitted.cginc"
	#define tmoFilmicACES ACESFitted
	#endif

	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	uniform sampler2D       _OverlayTex;
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_ScreenLum);
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_BloomTex);
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_EALumSrc);
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_EAHist);
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_CompareTex);
	uniform sampler2D       _LUTTex;
	
	uniform float4 _MainTex_TexelSize;
	uniform float4 _MainTex_ST;
	uniform float4 _BloomTex_TexelSize;
	uniform float4 _ColorBoost; // x = Brightness, y = Contrast, z = Saturate, w = Daltonize;
	uniform float4 _Sharpen;
	uniform float4 _Dither;
	uniform float4 _FXColor;
	uniform float4 _TintColor;
	uniform float4 _Outline;
	uniform float4 _Dirt;		// x = brightness based, y = intensity, z = threshold, w = bloom contribution
    uniform float4 _Bloom;
    uniform float4 _CompareParams;

    #if BEAUTIFY_VIGNETTING || BEAUTIFY_VIGNETTING_MASK
	uniform float4 _Vignetting;
    uniform float _VignettingAspectRatio;
    uniform sampler2D _VignettingMask;
    #endif

    #if BEAUTIFY_FRAME || BEAUTIFY_FRAME_MASK
    uniform float4 _Frame;
    uniform sampler2D _FrameMask;
    #endif

   	uniform float4 _BokehData;
	uniform float4 _BokehData2;
	uniform float4 _EyeAdaptation;
	uniform float3 _Purkinje;

	#if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT || BEAUTIFY_DEPTH_OF_FIELD
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_DoFTex);
	uniform float4 _DoFTex_TexelSize;
	#endif
	#if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
	uniform sampler2D _DepthTexture;
	uniform sampler2D _DofExclusionTexture;
	#endif

    struct appdata {
    	float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    
	struct v2f {
	    float4 pos : SV_POSITION;
	    float2 uv: TEXCOORD0;
    	float4 depthUV : TEXCOORD1;	    
    	#if BEAUTIFY_DIRT || BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT || BEAUTIFY_VIGNETTING || BEAUTIFY_VIGNETTING_MASK || BEAUTIFY_FRAME || BEAUTIFY_FRAME_MASK
	    float2 uvNonStereo: TEXCOORD2;
	    #endif
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};

	struct v2fCompare {
		float4 pos : SV_POSITION;
		float2 uv: TEXCOORD0;
		float2 uvNonStereo: TEXCOORD1;
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2fCompare vertCompare(appdata v) {
		v2fCompare o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
		o.uvNonStereo = v.texcoord;
		return o;
	}


	v2f vert(appdata v) {
    	v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    	o.pos = UnityObjectToClipPos(v.vertex);
   		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
    	o.depthUV = float4(o.uv, 0, 0);
    	#if BEAUTIFY_DIRT || BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT || BEAUTIFY_VIGNETTING || BEAUTIFY_VIGNETTING_MASK || BEAUTIFY_FRAME || BEAUTIFY_FRAME_MASK
   		o.uvNonStereo = v.texcoord;
   		#endif
    	#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Depth texture is inverted WRT the main texture
    	    o.depthUV.y = 1.0 - o.depthUV.y;
			#if BEAUTIFY_DIRT || BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT || BEAUTIFY_VIGNETTING || BEAUTIFY_VIGNETTING_MASK || BEAUTIFY_FRAME || BEAUTIFY_FRAME_MASK
			o.uvNonStereo.y = 1.0 - o.uvNonStereo.y;
			#endif
		}
    	#endif
    	return o;
	}
		
	inline float getLuma(float3 rgb) {
		const float3 lum = float3(0.299, 0.587, 0.114);
		return dot(rgb, lum);
	}
	
	float3 getNormal(float depth, float depth1, float depth2, float2 offset1, float2 offset2) {
  		float3 p1 = float3(offset1, depth1 - depth);
  		float3 p2 = float3(offset2, depth2 - depth);
  		float3 normal = cross(p1, p2);
	  	return normalize(normal);
	}

	float getRandom(float2 uv) {
		return frac(sin(_Time.y + dot(uv, float2(12.9898, 78.233)))* 43758.5453);
	}



	float getCoc(v2f i) {
	#if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
	    float depthTex = DecodeFloatRGBA(tex2Dlod(_DepthTexture, float4(i.uvNonStereo, 0, 0)));
	    float exclusionDepth = DecodeFloatRGBA(tex2Dlod(_DofExclusionTexture, float4(i.uvNonStereo, 0, 0)));
		float depth  = Linear01Depth(UNITY_SAMPLE_DEPTH(SAMPLE_RAW_DEPTH_TEXTURE_LOD(_CameraDepthTexture, i.depthUV)));
		depth = min(depth, depthTex);
		if (exclusionDepth < depth) return 0;
	    depth *= _ProjectionParams.z;
	#else
		float depth  = LinearEyeDepth(UNITY_SAMPLE_DEPTH(SAMPLE_RAW_DEPTH_TEXTURE_LOD(_CameraDepthTexture, i.depthUV)));
	#endif
		float xd     = abs(depth - _BokehData.x) - _BokehData2.x * (depth < _BokehData.x);
		return 0.5 * _BokehData.y * xd/depth;	// radius of CoC
	}

	#if !defined(BEAUTIFY_ACES_FITTED)
   	inline float3 tmoFilmicACES(float3 x) {
		const float A = 2.51;
		const float B = 0.03;
		const float C = 2.43;
		const float D = 0.59;
		const float E = 0.14;
		return (x * (A * x + B) ) / (x * (C * x + D) + E);
	}
	#endif

	void beautifyPass(v2f i, inout float3 rgbM) {
		
		const float3 halves = float3(0.5, 0.5, 0.5);

		float3 uvInc      = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0);
		float  depthS     = Linear01Depth(UNITY_SAMPLE_DEPTH(SAMPLE_RAW_DEPTH_TEXTURE_LOD(_CameraDepthTexture, i.depthUV - uvInc.zyzz)));
		float  depthW     = Linear01Depth(UNITY_SAMPLE_DEPTH(SAMPLE_RAW_DEPTH_TEXTURE_LOD(_CameraDepthTexture, i.depthUV - uvInc.xzzz)));
		float  depthE     = Linear01Depth(UNITY_SAMPLE_DEPTH(SAMPLE_RAW_DEPTH_TEXTURE_LOD(_CameraDepthTexture, i.depthUV + uvInc.xzzz)));		
		float  depthN     = Linear01Depth(UNITY_SAMPLE_DEPTH(SAMPLE_RAW_DEPTH_TEXTURE_LOD(_CameraDepthTexture, i.depthUV + uvInc.zyzz)));
		float  lumaM      = getLuma(rgbM);

		#if !BEAUTIFY_NIGHT_VISION && !BEAUTIFY_THERMAL_VISION
		#if !defined(BEAUTIFY_DITHER_FINAL) && defined(BEAUTIFY_ENABLE_DITHER)
		float3 dither     = dot(float2(171.0, 231.0), i.uv * _ScreenParams.xy).xxx;
		       dither     = frac(dither / float3(103.0, 71.0, 97.0)) - halves;
		       rgbM      *= 1.0 + step(_Dither.y, depthW) * _Dither.x * dither;
        #endif

		#if BEAUTIFY_DALTONIZE
		float3 rgb0       = 1.0.xxx - saturate(rgbM.rgb);
		       rgbM.r    *= 1.0 + rgbM.r * rgb0.g * rgb0.b * _ColorBoost.w;
			   rgbM.g    *= 1.0 + rgbM.g * rgb0.r * rgb0.b * _ColorBoost.w;
			   rgbM.b    *= 1.0 + rgbM.b * rgb0.r * rgb0.g * _ColorBoost.w;	
			   rgbM      *= lumaM / (getLuma(rgbM) + 0.0001);
		#endif

		float  maxDepth   = max(depthN, depthS);
		       maxDepth   = max(maxDepth, depthW);
		       maxDepth   = max(maxDepth, depthE);
		float  minDepth   = min(depthN, depthS);
		       minDepth   = min(minDepth, depthW);
		       minDepth   = min(minDepth, depthE);
		float  dDepth     = maxDepth - minDepth + 0.00001;
		
		float  lumaDepth  = saturate(_Sharpen.y / dDepth);
		float3 rgbS       = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv - uvInc.zy).rgb;
	    float3 rgbW       = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv - uvInc.xz).rgb;
	    float3 rgbE       = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv + uvInc.xz).rgb;
	    float3 rgbN       = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv + uvInc.zy).rgb;
	    
    	float  lumaN      = getLuma(rgbN);
    	float  lumaE      = getLuma(rgbE);
    	float  lumaW      = getLuma(rgbW);
    	float  lumaS      = getLuma(rgbS);
		
    	float  maxLuma    = max(lumaN,lumaS);
    	       maxLuma    = max(maxLuma, lumaW);
    	       maxLuma    = max(maxLuma, lumaE);
	    float  minLuma    = min(lumaN,lumaS);
	           minLuma    = min(minLuma, lumaW);
	           minLuma    = min(minLuma, lumaE) - 0.000001;
	    float  lumaPower  = 2.0 * lumaM - minLuma - maxLuma;
		float  lumaAtten  = saturate(_Sharpen.w / (maxLuma - minLuma));
		float  depthClamp = abs(depthW - _Dither.z) < _Dither.w;		
		       #if defined(BEAUTIFY_ENABLE_CORE_EFFECTS)
		       rgbM      *= 1.0 + clamp(lumaPower * lumaAtten * lumaDepth * _Sharpen.x, -_Sharpen.z, _Sharpen.z) * depthClamp;
		       #endif

		#if BEAUTIFY_DEPTH_OF_FIELD || BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
		float4 dofPix     = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_DoFTex, i.uv);
   		#if UNITY_COLORSPACE_GAMMA
			   dofPix.rgb = LinearToGammaSpace(dofPix.rgb);
		#endif
		if (_DoFTex_TexelSize.z < _MainTex_TexelSize.z) {
			float  CoC = getCoc(i);
		       dofPix.a   = lerp(CoC, dofPix.a, _DoFTex_TexelSize.z / _MainTex_TexelSize.z);
		}
		       rgbM       = lerp(rgbM, dofPix.rgb, saturate(dofPix.a));	
		#endif

  		#endif	// night & thermal vision exclusion

  		#if BEAUTIFY_NIGHT_VISION
			float depth       = Linear01Depth(UNITY_SAMPLE_DEPTH(SAMPLE_RAW_DEPTH_TEXTURE_LOD(_CameraDepthTexture, i.depthUV)));
//   		float3  normalNW  = getNormal(depth, depthN, depthW, uvInc.zy, -uvInc.xz);	// According to Unity QA this causes issues in Mali G71 GPU due to a hardware problem
   			float3  normalNW  = getNormal(depth, depthN, depthW, uvInc.zy, float2(-uvInc.x, -uvInc.z));
   		#endif

   		#if BEAUTIFY_OUTLINE
			#if !defined(BEAUTIFY_OUTLINE_SOBEL)
		   		#if !BEAUTIFY_NIGHT_VISION
					float depth       = Linear01Depth(UNITY_SAMPLE_DEPTH(SAMPLE_RAW_DEPTH_TEXTURE_LOD(_CameraDepthTexture, i.depthUV)));
	//   			float3 normalNW   = getNormal(depth, depthN, depthW, uvInc.zy, -uvInc.xz); 	// According to Unity QA this causes issues in Mali G71 GPU due to a hardware problem
   					float3  normalNW  = getNormal(depth, depthN, depthW, uvInc.zy, float2(-uvInc.x, -uvInc.z));
   				#endif
   				float3 normalSE   = getNormal(depth, depthS, depthE, -uvInc.zy,  uvInc.xz);
				float  dnorm      = dot(normalNW, normalSE);
   				rgbM              = lerp(rgbM, _Outline.rgb, (float)(dnorm  < _Outline.a));
   			#else
	   			float4 uv4 = float4(i.uv, 0, 0);
   				#if BEAUTIFY_NIGHT_VISION || BEAUTIFY_THERMAL_VISION
		   			float3 rgbS       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.zyzz).rgb;
		    		float3 rgbN       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + uvInc.zyzz).rgb;
			    	float3 rgbW       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.xzzz).rgb;
	    			float3 rgbE       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + uvInc.xzzz).rgb;
		    	#endif
				float3 rgbSW = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.xyzz).rgb;	// was tex2Dlod
				float3 rgbNE = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + uvInc.xyzz).rgb;
				float3 rgbSE = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + float4( uvInc.x, -uvInc.y, 0, 0)).rgb;
				float3 rgbNW = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + float4(-uvInc.x,  uvInc.y, 0, 0)).rgb;
				float3 gx  = rgbSW * -1.0;
 				       gx += rgbSE *  1.0;
			       	   gx += rgbW  * -2.0;
				       gx += rgbE  *  2.0;
				       gx += rgbNW * -1.0;
				       gx += rgbNE *  1.0;
				float3 gy  = rgbSW * -1.0;
				       gy += rgbS  * -2.0;
				       gy += rgbSE * -1.0;
				       gy += rgbNW *  1.0;
				       gy += rgbN  *  2.0;
				       gy += rgbNE *  1.0;
				float olColor = (length(gx * gx + gy * gy) - _Outline.a) > 0.0;
				rgbM = lerp(rgbM, _Outline.rgb, olColor); 
   			#endif
	 	#endif

   		#if UNITY_COLORSPACE_GAMMA
		rgbM = GammaToLinearSpace(rgbM);
		#endif
		
		#if BEAUTIFY_BLOOM
		rgbM += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_BloomTex, i.uv).rgb * _Bloom.xxx;
		#endif
		
   	 	#if BEAUTIFY_DIRT
			float3 scrLum = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ScreenLum, i.uv).rgb;
			#if BEAUTIFY_BLOOM
			scrLum *= _Dirt.www;
			#endif
   	 		float4 dirt = tex2D(_OverlayTex, i.uvNonStereo);
   	 	    rgbM       += saturate(halves.xxx - _Dirt.zzz + scrLum) * dirt.rgb * _Dirt.y; 
		#endif

   	 	#if BEAUTIFY_NIGHT_VISION
   	 	       lumaM      = getLuma(rgbM);	// updates luma
   		float  nvbase     = saturate(normalNW.z - 0.8); // minimum ambient self radiance (useful for pitch black)
   			   nvbase    += lumaM;						// adds current lighting
   			   nvbase    *= nvbase * (0.5 + nvbase);	// increase contrast
   			   rgbM	      = nvbase * _FXColor.rgb;
   			   rgbM      *= frac(floor(i.uv.y*_ScreenParams.y)*0.25)>0.4;	// scan lines
   			   rgbM	     *= 1.0 + getRandom(i.uv) * 0.3 - 0.15;				// noise
	 	#endif
	 	
   	 	#if BEAUTIFY_THERMAL_VISION
   	 	       lumaM      = getLuma(rgbM);	// updates luma
    	float3 tv0 	      = lerp(float3(0.0,0.0,1.0), float3(1.0,1.0,0.0), lumaM * 2.0);
    	float3 tv1  	  = lerp(float3(1.0,1.0,0.0), float3(1.0,0.0,0.0), lumaM * 2.0 - 1.0);
    		   rgbM       = lerp(tv0, tv1, (float)(lumaM >= 0.5));
   			   rgbM      *= 0.2 + frac(floor(i.uv.y*_ScreenParams.y)*0.25)>0.4;	// scan lines
   			   rgbM		 *= 1.0 + getRandom(i.uv) * 0.2 - 0.1;					// noise
	 	#endif

  		#if BEAUTIFY_EYE_ADAPTATION || BEAUTIFY_PURKINJE
   		float4 avgLum = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_EAHist, 0.5.xx);
   		#endif

		#if BEAUTIFY_EYE_ADAPTATION
   			float srcLum  = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_EALumSrc, 0.5.xx).r;
   			float  diff   = srcLum / (avgLum.r + 0.0001);
   			#if defined(BEAUTIFY_EYE_ADAPTATION_DYNAMIC_RANGE)
				float pixLum  = log(1.0 + getLuma(rgbM));
		      	diff    = pow(pixLum / (avgLum.r + 0.0001), abs(diff-1.0));
			#endif
   		    diff   = clamp(diff, _EyeAdaptation.x, _EyeAdaptation.y);
   			rgbM   = rgbM * diff;
   		#endif

		#if BEAUTIFY_TONEMAP_ACES
			 rgbM   *= _ColorBoost.x;
   			 rgbM    = tmoFilmicACES(rgbM);
   		#endif

	   	#if UNITY_COLORSPACE_GAMMA
			 rgbM    = LinearToGammaSpace(rgbM);
		#endif

		#if defined(BEAUTIFY_USE_PROCEDURAL_SEPIA)
   			float3 sepia      = float3(
   		            	   			dot(rgbM, float3(0.393, 0.769, 0.189)),
               						dot(rgbM, float3(0.349, 0.686, 0.168)),
               						dot(rgbM, float3(0.272, 0.534, 0.131))
               					  );
               	rgbM      = lerp(rgbM, sepia, _FXColor.a);
        #elif BEAUTIFY_LUT
			#if !UNITY_COLORSPACE_GAMMA
			  rgbM = LinearToGammaSpace(rgbM);
			#endif
			const float3 lutST = float3(1.0/1024, 1.0/32, 32-1);
			float3 lookUp = saturate(rgbM) * lutST.zzz;
    		lookUp.xy = lutST.xy * (lookUp.xy + 0.5);
    		float slice = floor(lookUp.z);
    		lookUp.x += slice * lutST.y;
    		float2 lookUpNextSlice = float2(lookUp.x + lutST.y, lookUp.y);
    		float3 lut = lerp(tex2D(_LUTTex, lookUp.xy).rgb, tex2D(_LUTTex, lookUpNextSlice).rgb, lookUp.z - slice);
    		rgbM = lerp(rgbM, lut, _FXColor.a);
        #endif

#if defined(BEAUTIFY_ENABLE_CORE_EFFECTS)
			float3 maxComponent = max(rgbM.r, max(rgbM.g, rgbM.b));
			float3 minComponent = min(rgbM.r, min(rgbM.g, rgbM.b));
			float  sat = saturate(maxComponent - minComponent);
			rgbM *= 1.0 + _ColorBoost.z * (1.0 - sat) * (rgbM - getLuma(rgbM));
			rgbM = lerp(rgbM, rgbM * _TintColor.rgb, _TintColor.a);
			rgbM = (rgbM - halves) * _ColorBoost.y + halves;
#endif

		#if !BEAUTIFY_TONEMAP_ACES
  			   rgbM      *= _ColorBoost.x;
  		#endif

   		#if BEAUTIFY_PURKINJE
   			  lumaM    = getLuma(rgbM);
   		float3 shifted  = saturate(float3(lumaM / (1.0 + _Purkinje.x * 1.14), lumaM, lumaM * (1.0 + _Purkinje.x * 2.99)));
   			  rgbM     = lerp(shifted, rgbM, saturate(exp(avgLum.g) - _Purkinje.y));
   		#endif

#if BEAUTIFY_VIGNETTING
			float2 vd = float2(i.uvNonStereo.x - 0.5, (i.uvNonStereo.y - 0.5) * _VignettingAspectRatio);
			rgbM = lerp(rgbM, lumaM * _Vignetting.rgb, saturate(_Purkinje.z + _Vignetting.a * dot(vd, vd)));
#elif BEAUTIFY_VIGNETTING_MASK
			float2 vd = float2(i.uvNonStereo.x - 0.5, (i.uvNonStereo.y - 0.5) * _VignettingAspectRatio);
			float  vmask = tex2D(_VignettingMask, i.uvNonStereo).a;
			rgbM = lerp(rgbM, lumaM * _Vignetting.rgb, saturate(_Purkinje.z + vmask * _Vignetting.a * dot(vd, vd)));
#endif

  		#if BEAUTIFY_FRAME
	    rgbM       = lerp(rgbM, _Frame.rgb, saturate( (max(abs(i.uvNonStereo.x - 0.5), abs(i.uvNonStereo.y - 0.5)) - _Frame.a) * 50.0));
	    #elif BEAUTIFY_FRAME_MASK
	    float4 frameMask  = tex2D(_FrameMask, i.uvNonStereo);
  		float3 frameColor     = lerp(_Frame.rgb, frameMask.rgb * _Frame.rgb, frameMask.a);
	    rgbM       = lerp(rgbM, frameColor, saturate( (max(abs(i.uvNonStereo.x - 0.5), abs(i.uvNonStereo.y - 0.5)) - _Frame.a) * 50.0));
   		#endif

   		#if !BEAUTIFY_NIGHT_VISION && !BEAUTIFY_THERMAL_VISION && defined(BEAUTIFY_DITHER_FINAL) && defined(BEAUTIFY_ENABLE_DITHER)
		float3 dither     = dot(float2(171.0, 231.0), i.uv * _ScreenParams.xy).xxx;
		       dither     = frac(dither / float3(103.0, 71.0, 97.0)) - halves;
		       rgbM      *= 1.0 + step(_Dither.y, depthW) * _Dither.x * dither;
        #endif
	}
	
	float4 fragBeautify (v2f i) : SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
		#if BEAUTIFY_THERMAL_VISION
		i.uv.x += 0.0009 * sin(_Time.z + i.uv.y * 20.0) / (1.0 + 10.0 * dot(i.uv - 0.5.xx, i.uv.xy - 0.5.xx));	// wave animation
		#endif
   		float4 pixel = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);
   		beautifyPass(i, pixel.rgb);
  		return pixel;
	}
	
	float4 fragCompare (v2fCompare i) : SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
		// separator line + antialias
		float2 dd     = i.uvNonStereo - 0.5.xx;
		float  co     = dot(_CompareParams.xy, dd);
		float  dist   = distance( _CompareParams.xy * co, dd );
		float4 aa     = saturate( (_CompareParams.w - dist) / abs(_MainTex_TexelSize.y) );

		float4 pixel  = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);
		float4 pixelNice = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CompareTex, i.uv);
		
		// are we on the beautified side?
		float t       = dot(dd, _CompareParams.yz) > 0;
		pixel         = lerp(pixel, pixelNice, t);
		return pixel + aa;
	}
