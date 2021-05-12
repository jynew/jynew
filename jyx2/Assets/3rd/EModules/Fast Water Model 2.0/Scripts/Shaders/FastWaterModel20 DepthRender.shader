
Shader "Hidden/EM-X/DepthRender" {
	Properties{
		//Y_POS("Y_POS", Vector) = (0,0,0,0)
	}
		SubShader{
		Pass {
		Lighting Off Fog { Mode Off }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		//#pragma fragmentoption ARB_precision_hint_nicest
		//float Y_POS
		 
		struct appdata {
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
		};
		struct v2f {
			float4 pos : POSITION;
			float4 uv : TEXCOORD0;
			float4 screen : TEXCOORD2;
		};



		v2f vert(appdata v ) {
			v2f o;
			float4 oPos = UnityObjectToClipPos(v.vertex);
			o.pos = oPos;
			o.screen = ComputeScreenPos(o.pos);
			o.uv = v.texcoord;
			return o;
		}
		float4 frag(v2f i) : SV_Target{
			//float refZ = UnityObjectToClipPos(Y_POS);
			
			float D = 256;
			float S = 4;

			float READ_Z;




			/*float ZP = (i.screen.z);
#if !defined(UNITY_REVERSED_Z)
			ZP = (1.0f - ZP) / 2;
#endif
			READ_Z = max(0, ZP - 0.5) * 2;*/
			//return ZP;
			/*float ZP = (i.screen.z);
#if !defined(UNITY_REVERSED_Z)
			ZP = (1.0f - ZP) / 2;
#endif
			READ_Z = ZP;*/

#if defined(UNITY_REVERSED_Z)
			READ_Z = (1 - (i.screen.z));
			READ_Z = max(0, READ_Z - 0.5) * 2;
#else
			READ_Z = ((i.screen.z));
#endif	
			

			//A = i.pos.x / 2024;

			/*float2 R = max(0, abs(i.uv.xy - 0.5) - 0.11);
			return float4(R, R, R, 1);
			if (R.x == 0 || R.y == 0 ) return float4(0, 0, 0, 1);*/

			float fullZ = max(0, READ_Z * (D ) );

			float A = saturate(fullZ);
			//float fullZ = max(0, READ_Z * (D + 1) - 1);
			//float fullZ = max(0, (i.pos.z) * (D + 1) - 1);
			float r = frac(fullZ / S);
			float GG = (fullZ / S) - r;
			float g = frac(GG / S);
			float b = ((GG / S) - g) / S;


			return float4(r, g, b, A);
		}
		ENDCG
		}
	}
}


