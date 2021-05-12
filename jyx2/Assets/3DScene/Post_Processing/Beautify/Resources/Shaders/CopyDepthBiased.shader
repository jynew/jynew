Shader "Beautify/CopyDepthBiased" {
	Properties{
		_MainTex("", 2D) = "white" {}
	}

		SubShader{
			Tags{ "RenderType" = "Transparent" }
			Pass{
			CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		struct v2f {
			float4 pos : SV_POSITION;
			float depth01 : TEXCOORD0;
		};

		float _BeautifyDepthBias;

		v2f vert(appdata_base v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.depth01 = COMPUTE_DEPTH_01;
			o.depth01 *= _BeautifyDepthBias;
			return o;
		}
		float4 frag(v2f i) : SV_Target{
			return EncodeFloatRGBA(i.depth01);
		}
		ENDCG
		}
	}
}
