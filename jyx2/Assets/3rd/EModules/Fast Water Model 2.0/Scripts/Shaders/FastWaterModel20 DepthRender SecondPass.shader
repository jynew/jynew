Shader "Hidden/EM-X/DepthRender SecondPass" {
	Properties{
		_MainTex("_MainTex ", any) = "white"
		BLUR_R("BLUR_R", VECTOR) = (0.1,0.1,0.1,0.1)
		STEPS("STEPS", FLOAT) = 1
		Q("Q", FLOAT) = 2
		SHORE_SIM_RAD("SHORE_SIM_RAD", FLOAT) = 2
		SHORE_BORDERS("SHORE_BORDERS", FLOAT) = 1
	} 
		SubShader{
		 
		Pass {
		//ZTest Always Cull Off ZWrite Off
		Lighting Off Fog { Mode Off }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		 //#pragma fragmentoption ARB_precision_hint_nicest

		struct appdata {
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
		};
		struct v2f {
			float4 pos : POSITION;
			float4 uv : TEXCOORD2;
		};
		
			uniform float SHORE_BORDERS;
		uniform float STEPS;
		uniform float2 BLUR_R;
		uniform float Q;
		uniform float SHORE_SIM_RAD;
	uniform sampler2D_float _MainTex;
	 
	uniform float MISTERGAUSS[49];

	float OctaveCalc(float2 uv, float octaves, float OC) {
		
		float STEPS_1 = STEPS + SHORE_SIM_RAD; //O FIX TO REMOVE +1
		float _L = -floor(STEPS / 2);
		float _R = ceil(STEPS / 2) + 1;
		float LENGTH = _R - _L;

		float result = 0;
		float lrp = octaves / (OC - 1);
		float2 coord = lerp(BLUR_R / OC, BLUR_R, saturate(lrp)) / STEPS;
		 
#if !defined (SHADER_API_GLES)
#define UNPACK( _MainTex,  UUVV)  tex2Dgrad(_MainTex, UUVV,0,0).a 
#else
#define UNPACK( _MainTex,  UUVV)  tex2D(_MainTex, UUVV).a 
#endif

		/*float2 testuv = saturate(uv + float2((0 + LENGTH) * coord, (0 + LENGTH) * coord));
		return  UNPACK(_MainTex, testuv)* MISTERGAUSS[50];*/
		float sinTime = 0;
		if (SHORE_BORDERS == 1) {
			for (int x2 = 0.0; x2 < LENGTH; x2++)
				for (int y2 = 0.0; y2 < LENGTH; y2++) {
					float2 dif = float2((x2 + _L) , (y2 + _L));
					float rx = cos(sinTime);
					float ry = sin(sinTime);
					dif = (float2(rx, ry) * dif.x + float2(-ry, rx) * dif.y)* coord;
					float2 UUVV = saturate(uv + dif);
					float2 R = max(0, abs(UUVV - 0.5) - 0.4999);
					if (R.x == 0.0 && R.y == 0.0)
					{

						//float unack = tex2Dgrad(_MainTex, UUVV, 0, 0).a;
						result += saturate( UNPACK(_MainTex, UUVV) ) * MISTERGAUSS[ (x2  + y2 * STEPS_1) % 49 ];
					}
					sinTime += 2.13;
				}
		}
		else {
			for (int x = 0.0; x < LENGTH; x++)
				for (int y = 0.0; y < LENGTH; y++) {
					//float2 UUVV = saturate(uv + float2(x * coord,  y * coord));
					//float2 UUVV = saturate(uv + float2((x + _L) * coord, (y + _L) * coord));

					float2 dif = float2((x + _L), (y + _L) );
					float rx = cos(sinTime);
					float ry = sin(sinTime);
					dif = (float2(rx,ry) * dif.x + float2(-ry,rx) * dif.y)* coord;
					float2 UUVV = (uv + dif);
					//UNPACK(_MainTex, UUVV, x, y);
					//float2 gausCoord = float2(rx, ry) * x + float2(-ry, rx) * y;
					float2 gausCoord = float2(x, y) ;
					//gausCoord = clamp(gausCoord, 0, 48);
					result += saturate(UNPACK(_MainTex, UUVV)) * MISTERGAUSS[(gausCoord.x + gausCoord.y * STEPS_1) % 49] ;
						/*float unack = tex2Dgrad(_MainTex, UUVV, 0, 0).a;
					result += saturate(unack * 1000) * MISTERGAUSS[x - _L + (y - _L) * STEPS_1];*/
					sinTime += 2.13;
				}
		}
		//float3 unack = tex2Dgrad(_MainTex, UUVV, 0, 0);
			//result += saturate((unack.r + unack.g + unack.b) * 1000) * MISTERGAUSS[x - _L + (y - _L) * STEPS_1];
			//result += saturate((unack.r - unack.g)*10) * MISTERGAUSS[x - _L + (y - _L) * STEPS_1];
		 
		result /= 1000;
		//return saturate(result * result* result* result)	;
		//result *= 3.0;
		float OCL = lerp(0.4, 0.8, lrp);
		//float OCL = lerp(0.4, 1, lrp);
		return saturate((result - OCL) / (1 - OCL));
	}

		float mistergaussian(float2 uv) {
			//return 1;
			//return tex2D(_MainTex, uv).a;
			
			
			//

			/*float result = 0;
			float coord = BLUR_R / STEPS;

			for (int x = _L; x < _R; x++)
				for (int y = _L; y < _R; y++) {
					float2 UUVV = uv + float2(x * coord, y * coord);
					float3 unack = tex2D(_MainTex, UUVV);
					result += saturate((unack.r + unack.g + unack.b) * 1000) * MISTERGAUSS[x - _L + (y - _L) * STEPS_1];
				}
			result *= 3;
			return saturate(result - 0.5) * 2;*/
			 
			float OC;
			if (Q == 3) OC = 30;
			else if (Q == 1) OC = 5;
			else if (Q == 4) OC = 50;
			else OC = 10;

			//OC = 5;
			float OC_RESULT = 0;

			for (int octaves = 0; octaves < OC; octaves++) OC_RESULT += OctaveCalc(uv, octaves, OC);

			return OC_RESULT / OC;
			
		}


		v2f vert(in appdata v) {
			v2f o;
			UNITY_INITIALIZE_OUTPUT(v2f, o);
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
		}
		float4 frag(v2f i) : SV_Target{
			float blur = mistergaussian(i.uv.xy);
		//return tex2D(_MainTex, i.uv.xy);
			
		//tex2D(_MainTex, uv).rgb

			return float4(tex2D(_MainTex, i.uv).rgb, blur * tex2D(_MainTex, i.uv).a);
		}
		ENDCG
		}
	}
}