	// Copyright 2016-2018 Kronnect - All Rights Reserved.
	
	#include "UnityCG.cginc"

	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_CompareTex);

	uniform float4 _MainTex_TexelSize;
	uniform float4 _MainTex_ST;
	uniform fixed4 _ColorBoost; // x = Brightness, y = Contrast, z = Saturate, w = Daltonize;
	uniform fixed4 _Sharpen;
    uniform float4 _CompareParams;
		
    struct appdata {
    	float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    
	struct v2f {
	    float4 pos : SV_POSITION;
	    float2 uv: TEXCOORD0;
	    float2 uvN: TEXCOORD1;
	    float2 uvS: TEXCOORD2;
	    float2 uvW: TEXCOORD3;
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

    	float3 uvInc = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0);
    	o.uvN = o.uv + uvInc.zy;
    	o.uvS = o.uv - uvInc.zy;
    	o.uvW = o.uv - uvInc.xz;
    	return o;
	}

	inline fixed getLuma(fixed3 rgb) { 
		const fixed3 lum = fixed3(0.299, 0.587, 0.114);
		return dot(rgb, lum);
	}
		
	void beautifyPassFast(v2f i, inout fixed3 rgbM) {
   	    fixed3 rgbN       = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uvN).rgb;
		fixed3 rgbS       = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uvS).rgb;
	    fixed3 rgbW       = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uvW).rgb;
		fixed  lumaM      = getLuma(rgbM);
    	fixed  lumaN      = getLuma(rgbN);
    	fixed  lumaW      = getLuma(rgbW);
    	fixed  lumaS      = getLuma(rgbS);
    	fixed  maxLuma    = max(lumaN,lumaS);
    	       maxLuma    = max(maxLuma, lumaW);
	    fixed  minLuma    = min(lumaN,lumaS);
	           minLuma    = min(minLuma, lumaW) - 0.000001;
	    fixed  lumaPower  = 2 * lumaM - minLuma - maxLuma;
		fixed  lumaAtten  = saturate(_Sharpen.w / (maxLuma - minLuma));
		       rgbM      *= 1.0 + clamp(lumaPower * lumaAtten * _Sharpen.x, -_Sharpen.z, _Sharpen.z);
		fixed3 maxComponent = max(rgbM.r, max(rgbM.g, rgbM.b));
 		fixed3 minComponent = min(rgbM.r, min(rgbM.g, rgbM.b));
 		fixed  sat          = saturate(maxComponent - minComponent);
		      rgbM         *= 1.0 + _ColorBoost.z * (1.0 - sat) * (rgbM - getLuma(rgbM));
  			  rgbM       = (rgbM - 0.5.xxx) * _ColorBoost.y + 0.5.xxx;
  			  rgbM      *= _ColorBoost.x;
   	}

	fixed4 fragBeautifyFast (v2f i) : SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
   		fixed4 pixel = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);
   		beautifyPassFast(i, pixel.rgb);
   		return pixel;
	}
	
	fixed4 fragCompareFast (v2fCompare i) : SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);

		// separator line + antialias
		float2 dd     = i.uvNonStereo - 0.5.xx;
		float  co     = dot(_CompareParams.xy, dd);
		float  dist   = distance( _CompareParams.xy * co, dd );
		float4 aa     = saturate( (_CompareParams.w - dist) / abs(_MainTex_TexelSize.y) );

		fixed4 pixel  = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);
		fixed4 pixelNice = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CompareTex, i.uv);
		
		// are we on the beautified side?
		fixed t       = dot(dd, _CompareParams.yz) > 0;
		pixel         = lerp(pixel, pixelNice, t);
		return pixel + aa;

	}
