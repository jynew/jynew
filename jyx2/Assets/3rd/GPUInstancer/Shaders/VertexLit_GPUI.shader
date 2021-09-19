// GPUInstancer enabled version of Unity built-in shader "Vertexlit"

Shader "GPUInstancer/VertexLit" {
Properties {
    _MainTex ("Main Texture", 2D) = "white" {  }
}
SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 200

	CGPROGRAM
	#include "UnityCG.cginc"
	#include "Include/GPUInstancerInclude.cginc"
	#pragma surface surf Lambert
	#pragma multi_compile_instancing
	#pragma instancing_options procedural:setupGPUI

	sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
		fixed4 color : COLOR;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}

	ENDCG
}
SubShader {
    Tags { "RenderType"="Opaque" }
    Pass {
        Tags { "LightMode" = "Vertex" }
        ColorMaterial AmbientAndDiffuse
        Lighting On
        SetTexture [_MainTex] {
            constantColor (1,1,1,1)
            combine texture * primary DOUBLE, constant // UNITY_OPAQUE_ALPHA_FFP
        }
    }

    // Lightmapped
    Pass
    {
        Tags{ "LIGHTMODE" = "VertexLM" "RenderType" = "Opaque" }

        CGPROGRAM
		#include "UnityCG.cginc"
		#include "Include/GPUInstancerInclude.cginc"
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setupGPUI
        #pragma multi_compile_fog

        #define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))

        float4 _MainTex_ST;

        struct appdata
        {
            float3 pos : POSITION;
            float3 uv1 : TEXCOORD1;
            float3 uv0 : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float2 uv0 : TEXCOORD0;
            float2 uv1 : TEXCOORD1;
        #if USING_FOG
            fixed fog : TEXCOORD2;
        #endif
            float4 pos : SV_POSITION;

            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert(appdata IN)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(IN);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            o.uv0 = IN.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            o.uv1 = IN.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;

        #if USING_FOG
            float3 eyePos = UnityObjectToViewPos(IN.pos);
            float fogCoord = length(eyePos.xyz);
            UNITY_CALC_FOG_FACTOR_RAW(fogCoord);
            o.fog = saturate(unityFogFactor);
        #endif

            o.pos = UnityObjectToClipPos(IN.pos);
            return o;
        }

        sampler2D _MainTex;

        fixed4 frag(v2f IN) : SV_Target
        {
            fixed4 col;
            fixed4 tex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.uv0.xy);
            half3 bakedColor = DecodeLightmap(tex);

            tex = tex2D(_MainTex, IN.uv1.xy);
            col.rgb = tex.rgb * bakedColor;
            col.a = 1.0f;

            #if USING_FOG
                col.rgb = lerp(unity_FogColor.rgb, col.rgb, IN.fog);
            #endif

        return col;

        }

    ENDCG
    }
}
}
