Shader "MTE/Unlit/3 Textures (no realtime shadow, baked lightmap)"
{
    Properties
    {
        _Control("Control (RGBA)", 2D) = "red" {}
        _Splat0("Layer 1", 2D) = "white" {}
        _Splat1("Layer 2", 2D) = "white" {}
        _Splat2("Layer 3", 2D) = "white" {}
    }

    CGINCLUDE
    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_fog
    #pragma multi_compile _ LIGHTMAP_ON
    #include "UnityCG.cginc"

    //#define DEBUG_LIGHTMAP
        
	sampler2D _Control;
	float4 _Control_ST;
    sampler2D _Splat0,_Splat1,_Splat2;
    float4 _Splat0_ST,_Splat1_ST,_Splat2_ST;

    struct v2f
    {
        float4 pos : SV_POSITION;
        float4 tc_Control : TEXCOORD0;
        float4 tc_Splat01 : TEXCOORD1;
        float2 tc_Splat2  : TEXCOORD2;
	    UNITY_FOG_COORDS(3)
    };

    struct appdata_lightmap
    {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
        float2 texcoord1 : TEXCOORD1;
        float2 fogCoord : TEXCOORD2;
    };

    v2f vert(appdata_lightmap v)
    {
        v2f o;
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        o.pos = UnityObjectToClipPos(v.vertex);
        o.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
#ifdef LIGHTMAP_ON
        o.tc_Control.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;//save lightmap uv in tc_Control.zw
#endif
        o.tc_Splat01.xy = TRANSFORM_TEX(v.texcoord, _Splat0);
        o.tc_Splat01.zw = TRANSFORM_TEX(v.texcoord, _Splat1);
        o.tc_Splat2.xy = TRANSFORM_TEX(v.texcoord, _Splat2);
		UNITY_TRANSFER_FOG(o, o.pos);
        return o;
    }

    fixed4 frag(v2f i) : SV_Target
    {
        half4 splat_control = tex2D(_Control, i.tc_Control.xy);

        fixed4 mixedDiffuse = 0.0f;
        mixedDiffuse += splat_control.r * tex2D(_Splat0, i.tc_Splat01.xy);
        mixedDiffuse += splat_control.g * tex2D(_Splat1, i.tc_Splat01.zw);
        mixedDiffuse += splat_control.b * tex2D(_Splat2, i.tc_Splat2.xy);
        fixed4 color = mixedDiffuse;
        
#ifdef LIGHTMAP_ON
        color.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.tc_Control.zw));
#endif

	    UNITY_APPLY_FOG(i.fogCoord, color);
        
#ifdef DEBUG_LIGHTMAP
        color *=
#ifdef LIGHTMAP_ON
            fixed4(0,1.0f,0,1.0f);
#else
            fixed4(1.0f,0,0,1.0f);
#endif
#endif

        return color;
    }
    ENDCG

    SubShader
    {
        Tags { "LIGHTMODE" = "VertexLM" "RenderType" = "Opaque" }
        // Without baked lightmap
        Pass {
            Tags{ "LIGHTMODE" = "Vertex" }
            Lighting Off
            CGPROGRAM
            ENDCG
        }
        // With baked lightmap
        Pass
        {
            Tags{ "LIGHTMODE" = "VertexLM" "RenderType" = "Opaque" }
            CGPROGRAM
            ENDCG
        }
    }
    Fallback "Diffuse"
    CustomEditor "MTE.MTEShaderGUI"
}
