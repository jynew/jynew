// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Water/SeaWave" {
	Properties {
		_WaterTex ("WaterTex", 2D) = "black" {} 
		_WaveTex ("WaveTex", 2D) = "black" {} //海浪
		_BumpTex ("BumpTex", 2D) = "bump" {} 
		_GTex ("Gradient", 2D) = "white" {} //海水渐变
		_NoiseTex ("Noise", 2D) = "white" {} //海浪躁波
		_WaterSpeed("WaterSpeed", float) = 0.74  //海水速度
		_WaveSpeed("WaveSpeed", float) = -12.64 //海浪速度
		_WaveRange("WaveRange", float) = 0.3
		_NoiseRange("NoiseRange", float) = 6.43
		_WaveDelta("WaveDelta", float) = 2.43
		_Refract("Refract", float) = 0.07
		_Specular("Specular", float) = 1.86
		_Gloss("Gloss", float) = 0.71
		_SpecColor("SpecColor", color) = (1, 1, 1, 1)
		_LightDir("LightDir", vector) = (0, 50, 30, 0)
		_Range ("Range", vector) = (0.13, 1.53, 0.37, 0.78)
	}

	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200

		zwrite off
		
		CGPROGRAM
		#pragma surface surf WaterLight vertex:vert alpha noshadow
		#pragma target 3.0

		sampler2D _GTex;

		sampler2D _WaterTex;
		sampler2D _BumpTex;
		sampler2D _CameraDepthTexture;
		
		sampler2D _NoiseTex;
		sampler2D _WaveTex;

		float4 _Range;

		half _WaterSpeed;
		
		half _WaveSpeed;
		fixed _WaveDelta;
		half _WaveRange;
		fixed _Refract;
		half _Specular;
		fixed _Gloss;
		float4 _LightDir;
		half _NoiseRange;

		float4 _WaterTex_TexelSize;

		struct Input {
			float2 uv_WaterTex;
			float2 uv_NoiseTex;
			float4 proj;
			float3 viewDir;
		};

		fixed4 LightingWaterLight(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
			half3 halfVector = normalize(_LightDir + viewDir);
			float diffFactor = max(0, dot(normalize(_LightDir), s.Normal)) * 0.8 + 0.2;
			float nh = max(0, dot(halfVector, s.Normal));
			float spec = pow(nh, s.Specular * 128.0) * s.Gloss;
			fixed4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diffFactor + _SpecColor.rgb * spec * _LightColor0.rgb) * (atten);
			c.a = s.Alpha + spec * _SpecColor.a;
			return c;
		}

		void vert (inout appdata_full v, out Input i) {
			UNITY_INITIALIZE_OUTPUT(Input, i);

			i.proj = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
			COMPUTE_EYEDEPTH(i.proj.z);
		}

		void surf (Input IN, inout SurfaceOutput o) {
			float2 uv = IN.proj.xy/IN.proj.w;
			#if UNITY_UV_STARTS_AT_TOP
			if(_WaterTex_TexelSize.y<0)
			uv.y = 1 - uv.y;
			#endif
			fixed4 water = (tex2D(_WaterTex, IN.uv_WaterTex + float2(_WaterSpeed*_Time.x,0))+tex2D(_WaterTex, float2(1-IN.uv_WaterTex.y,IN.uv_WaterTex.x) + float2(_WaterSpeed*_Time.x,0)))/2;
			float4 offsetColor = (tex2D(_BumpTex, IN.uv_WaterTex + float2(_WaterSpeed*_Time.x,0))+tex2D(_BumpTex, float2(1-IN.uv_WaterTex.y,IN.uv_WaterTex.x) + float2(_WaterSpeed*_Time.x,0)))/2;
			half2 offset = UnpackNormal(offsetColor).xy * _Refract;
			half m_depth = LinearEyeDepth(tex2Dproj (_CameraDepthTexture, IN.proj).r);
			half deltaDepth = m_depth - IN.proj.z;

			fixed4 noiseColor = tex2D(_NoiseTex, IN.uv_NoiseTex);

			fixed4 waterColor = tex2D(_GTex, float2(min(_Range.y, deltaDepth)/_Range.y,1));
			
			fixed4 waveColor = tex2D(_WaveTex, float2(1-min(_Range.z, deltaDepth)/_Range.z+_WaveRange*sin(_Time.x*_WaveSpeed+noiseColor.r*_NoiseRange),1)+offset);
			waveColor.rgb *= (1-(sin(_Time.x*_WaveSpeed+noiseColor.r*_NoiseRange)+1)/2)*noiseColor.r;
			fixed4 waveColor2 = tex2D(_WaveTex, float2(1-min(_Range.z, deltaDepth)/_Range.z+_WaveRange*sin(_Time.x*_WaveSpeed+_WaveDelta+noiseColor.r*_NoiseRange),1)+offset);
			waveColor2.rgb *= (1-(sin(_Time.x*_WaveSpeed+_WaveDelta+noiseColor.r*_NoiseRange)+1)/2)*noiseColor.r;
			
			half water_A = 1-min(_Range.z, deltaDepth)/_Range.z;
			half water_B = min(_Range.w, deltaDepth)/_Range.w;
			float4 bumpColor = (tex2D(_BumpTex, IN.uv_WaterTex+offset + float2(_WaterSpeed*_Time.x,0))+tex2D(_BumpTex, float2(1-IN.uv_WaterTex.y,IN.uv_WaterTex.x)+offset + float2(_WaterSpeed*_Time.x,0)))/2;

			o.Normal = UnpackNormal(bumpColor).xyz;
			
			o.Specular = _Specular;
			o.Gloss = _Gloss;
			o.Albedo = (1 - water_B) + waterColor.rgb * water_B;
			o.Albedo = o.Albedo * (1 - water.a*water_A) + water.rgb * water.a*water_A;
			o.Albedo += (waveColor.rgb+waveColor2.rgb) * water_A; 
			
			o.Alpha = min(_Range.x, deltaDepth)/_Range.x;
		}
		ENDCG
	} 
	//FallBack "Diffuse"
}
