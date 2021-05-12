Shader "Hidden/EM-X/ChannerRemover"
{
	Properties{
		_MainTex("Texture", Any) = "white" {}
	_Mask("Mask", Color) = (1,1,1,1)
	}
		
		CGINCLUDE
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
	
#include "UnityCG.cginc"

		struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float2 clipUV : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	uniform float4 _Mask;
	uniform float4 _MainTex_ST;
	uniform fixed4 _Color;
	uniform float4x4 unity_GUIClipTextureMatrix;

	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.vertex = UnityObjectToClipPos(v.vertex);
		float3 eyePos = UnityObjectToViewPos(v.vertex);
		o.clipUV = mul(unity_GUIClipTextureMatrix, float4(eyePos.xy, 0, 1.0));
		o.color = v.color;
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	uniform bool _ManualTex2SRGB;
	sampler2D _MainTex;
	sampler2D _GUIClipTexture;

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 colTex = tex2D(_MainTex, i.texcoord);
	if (_ManualTex2SRGB)
		colTex.rgb = LinearToGammaSpace(colTex.rgb);
	fixed4 col;
	col.rgb = colTex.rgb * i.color.rgb;
	fixed alpha = i.color.a * tex2D(_GUIClipTexture, i.clipUV).a;
	
	if (_Mask.r == 0 || _Mask.g == 0 || _Mask.b == 0 || _Mask.a == 0) {
		fixed t = 0;
		if (_Mask.r != 0) t = colTex.r;
		else if (_Mask.g != 0) t = colTex.g;
		else if (_Mask.b != 0) t = colTex.b;
		else if (_Mask.a != 0) t = colTex.a;
t *= i.color.rgb;
		return fixed4(t, t, t, alpha);
	}
	return float4(col.rgb, alpha);
	//return float4(col.rgb, tex2D(_GUIClipTexture, i.texcoord).a * alpha);
	}
		ENDCG

		SubShader {
		Tags{ "ForceSupported" = "True" }

			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha, One One
			Cull Off
			ZWrite Off
			ZTest Always

			Pass{
			CGPROGRAM
			ENDCG
		}
	}

	SubShader {
		Tags{ "ForceSupported" = "True" }

			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZWrite Off
			ZTest Always

			Pass{
			CGPROGRAM
			ENDCG
		}
	}
}
