	// Copyright 2016-2018 Kronnect - All Rights Reserved.
	
	#include "UnityCG.cginc"
	#include "BeautifyAdvancedParams.cginc"
	#include "BeautifyOrtho.cginc"

	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	uniform sampler2D _DepthTexture;
	uniform sampler2D _DofExclusionTexture;	
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	uniform float4    _MainTex_TexelSize;
	uniform float4    _MainTex_ST;
	uniform float4	  _BokehData;
	uniform float4    _BokehData2;
	uniform float     _BokehData3;
    uniform float     _BlurScale;
	
    struct appdata {
    	float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    
	struct v2f {
	    float4 pos : SV_POSITION;
	    float2 uv: TEXCOORD0;
		float2 depthUV : TEXCOORD1;	    		
		float2 uvNonStereo: TEXCOORD2;
		float2 uvNonStereoInv: TEXCOORD3;
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};


 struct v2fCross {
        float4 pos : SV_POSITION;
        float2 uv: TEXCOORD0;
        float2 uv1: TEXCOORD1;
        float2 uv2: TEXCOORD2;
        float2 uv3: TEXCOORD3;
        float2 uv4: TEXCOORD4;
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO	
    };
    
	v2f vert(appdata v) {
    	v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    	o.pos = UnityObjectToClipPos(v.vertex);
		o.uvNonStereo = v.texcoord;
		o.uvNonStereoInv = v.texcoord;
		o.uv  = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
		o.depthUV = o.uv;
		
    	#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
    	    o.uv.y = 1.0 - o.uv.y;
			o.uvNonStereoInv.y = 1.0 - o.uvNonStereoInv.y;
    	}
    	#endif  	
    	return o;
	}

	float getCoc(v2f i) {
	#if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
	    float depthTex = DecodeFloatRGBA(tex2Dlod(_DepthTexture, float4(i.uvNonStereo, 0, 0)));
	    float exclusionDepth = DecodeFloatRGBA(tex2Dlod(_DofExclusionTexture, float4(i.uvNonStereo, 0, 0)));
		float depth  = Linear01Depth(UNITY_SAMPLE_DEPTH(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, i.depthUV)));
		depth = min(depth, depthTex);
		if (exclusionDepth < depth) return 0;
	    depth *= _ProjectionParams.z;
	#else
		float depth  = LinearEyeDepth(UNITY_SAMPLE_DEPTH(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, i.depthUV)));
	#endif
		float xd     = abs(depth - _BokehData.x) - _BokehData2.x * (depth < _BokehData.x);
		return 0.5 * _BokehData.y * xd/depth;	// radius of CoC
	}
				
	float4 fragCoC (v2f i) : SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
		float4 pixel  = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);
		pixel         = clamp(pixel, 0.0.xxxx, _BokehData3.xxxx);
		#if UNITY_COLORSPACE_GAMMA
		pixel.rgb     = GammaToLinearSpace(pixel.rgb);
		#endif
   		return float4(pixel.rgb, getCoc(i));
   	}	
	
	float4 fragCoCDebug (v2f i) : SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
		float4 pixel  = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);
		pixel         = clamp(pixel, 0.0.xxxx, _BokehData3.xxxx);
		float  CoC    = getCoc(i);
		pixel.a       = min(CoC, pixel.a);
		return pixel.aaaa;
   	}

	float4 fragBlur (v2f i): SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
		float4 sum     = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv );
		float  samples = ceil(sum.a);
		float4 dir     = float4(_BokehData.zw * _MainTex_TexelSize.xy, 0, 0);
		#if UNITY_SINGLE_PASS_STEREO
		dir.x *= 2.0;
		#endif
		       dir    *= max(1.0, samples / _BokehData2.y);
		float  jitter  = dot(float2(2.4084507, 3.2535211), i.uv * _MainTex_TexelSize.zw);
		float2 disp0   = dir.xy * (frac(jitter) + 0.5);
		float4 disp1   = float4(i.uvNonStereoInv + disp0, 0, 0);
		float4 disp2   = float4(i.uvNonStereoInv - disp0, 0, 0);
		float  w       = 1.0;

		const int sampleCount = (int)min(_BokehData2.y, samples);
		UNITY_UNROLL
		for (int k=1;k<16;k++) {
			if (k<sampleCount) {
				#if UNITY_SINGLE_PASS_STEREO
					   disp1.xy    = saturate(disp1.xy);
				float4 pixel1	   = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, float4(UnityStereoScreenSpaceUVAdjust(disp1.xy, _MainTex_ST), 0, 0)); // was tex2Dlod
				#else
				float4 pixel1	   = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, disp1);
				#endif
				float  bt1         = pixel1.a > k;
				       pixel1.rgb += _BokehData2.www * max(pixel1.rgb - _BokehData2.zzz, 0.0.xxx);
					   sum        += pixel1 * bt1;
					   w 	      += bt1;
					   disp1      += dir;
				#if UNITY_SINGLE_PASS_STEREO
					   disp2.xy    = saturate(disp2.xy);
				float4 pixel2      = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, float4(UnityStereoScreenSpaceUVAdjust(disp2.xy, _MainTex_ST), 0, 0));
				#else
				float4 pixel2	   = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, disp2);
				#endif
					   float  bt2  = pixel2.a > k;
				       pixel2.rgb += _BokehData2.www * max(pixel2.rgb - _BokehData2.zzz, 0.0.xxx);
					   sum        += pixel2 * bt2;
					   w          += bt2;
					   disp2      -= dir;
			}
		}
		return sum / w;
	}

	float4 fragBlurNoBokeh (v2f i): SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
		float4 sum     = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv );
		float samples  = ceil(sum.a);
		float4 dir     = float4(_BokehData.zw * _MainTex_TexelSize.xy, 0, 0);
		#if UNITY_SINGLE_PASS_STEREO
		dir.x *= 0.5;
		#endif
		       dir    *= max(1.0, samples / _BokehData2.y);
		float  jitter  = dot(float2(2.4084507, 3.2535211), i.uv * _MainTex_TexelSize.zw);
		float2 disp0   = dir.xy * (frac(jitter) + 0.5);
		float4 disp1   = float4(i.uvNonStereoInv + disp0, 0, 0);
		float4 disp2   = float4(i.uvNonStereoInv - disp0, 0, 0);
		float  w       = 1.0;

		const int sampleCount = (int)min(_BokehData2.y, samples);
		UNITY_UNROLL
		for (int k=1;k<16;k++) {
			if (k<sampleCount) {
				#if UNITY_SINGLE_PASS_STEREO
				float4 pixel1      = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, float4(UnityStereoScreenSpaceUVAdjust(disp1.xy, _MainTex_ST), 0, 0));
				#else
				float4 pixel1      = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, disp1);
				#endif
				float  bt1         = pixel1.a > k;
					   sum        += bt1 * pixel1;
					   w 	      += bt1;
					   disp1      += dir;
				#if UNITY_SINGLE_PASS_STEREO
				float4 pixel2      = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, float4(UnityStereoScreenSpaceUVAdjust(disp2.xy, _MainTex_ST), 0, 0));
				#else
				float4 pixel2      = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, disp2);
				#endif
				float  bt2         = pixel2.a > k;
					   sum        += bt2 * pixel2;
					   w          += bt2;
					   disp2      -= dir;
			}
		}
		return sum / w;
	}



    v2fCross vertBlurH(appdata v) {
        v2fCross o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv  = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
        #if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0) {
            o.uv.y = 1.0 - o.uv.y;
        }
        #endif      
        half2 inc = half2(_MainTex_TexelSize.x * 1.3846153846 * _BlurScale, 0); 

#if UNITY_SINGLE_PASS_STEREO
        inc.x *= 2.0;
#endif
        o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc, _MainTex_ST);  
        o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc, _MainTex_ST);  

        half2 inc2 = half2(_MainTex_TexelSize.x * 3.2307692308 * _BlurScale, 0);    

#if UNITY_SINGLE_PASS_STEREO
        inc2.x *= 2.0;
#endif
        o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc2, _MainTex_ST);
        o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc2, _MainTex_ST); 
        return o;
    }


    v2fCross vertBlurV(appdata v) {
        v2fCross o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv  = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
        #if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0) {
            o.uv.y = 1.0 - o.uv.y;
        }
        #endif      
        half2 inc = half2(0, _MainTex_TexelSize.y * 1.3846153846 * _BlurScale); 
        o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc, _MainTex_ST);  
        o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc, _MainTex_ST);  

        half2 inc2 = half2(0, _MainTex_TexelSize.y * 3.2307692308 * _BlurScale);    
        o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc2, _MainTex_ST);
        o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc2, _MainTex_ST); 
        return o;
    }


   float4 fragBlurCoC (v2fCross i): SV_Target {
        UNITY_SETUP_INSTANCE_ID(i);
        float depth   = LinearEyeDepth(UNITY_SAMPLE_DEPTH(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, i.uv)));
        float depth1  = LinearEyeDepth(UNITY_SAMPLE_DEPTH(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, i.uv1)));
        float depth2  = LinearEyeDepth(UNITY_SAMPLE_DEPTH(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, i.uv2)));
        float depth3  = LinearEyeDepth(UNITY_SAMPLE_DEPTH(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, i.uv3)));
        float depth4  = LinearEyeDepth(UNITY_SAMPLE_DEPTH(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, i.uv4)));

        const float f = 10;
        float w1      = saturate((depth - depth1)/f) * 0.3162162162; 
        float w2      = saturate((depth - depth2)/f) * 0.3162162162; 
        float w3      = saturate((depth - depth3)/f) * 0.0702702703; 
        float w4      = saturate((depth - depth4)/f) * 0.0702702703; 

        float coc1    = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1).a;
        float coc2    = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv2).a;
        float coc3    = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv3).a;
        float coc4    = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv4).a;

        float w0      = 0.2270270270;

        half4 pixel = tex2D(_MainTex, i.uv);

        float coc     = (pixel.a * w0 + coc1 * w1 + coc2 * w2 + coc3 * w3 + coc4 * w4) / (w0 + w1 + w2 + w3 + w4);
        pixel.a = coc;
        return pixel;
    }   



