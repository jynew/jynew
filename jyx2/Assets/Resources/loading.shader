Shader "Custom/loading"
{
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _Color1("Color1", Color) = (1,0,0,0)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Clip("Clip", float) = 0.5//-0.5f（全变色）-->0.5f（全不变色）
        [KeywordEnum(None, Left, Up, Forward)]_Mode("Mode", Float) = 0//左到右，上到下，前到后
    }
        SubShader{
        Tags{ "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // 基于物理的标准照明模型，并允许所有光线类型的阴影
#pragma surface surf Standard fullforwardshadows vertex:vert

        // 使用着色器模型3.0目标，以获得更好的照明
#pragma target 3.0

    sampler2D _MainTex;

    struct Input {
        float2 uv_MainTex;
        float4 localPos;
    };

    half _Glossiness;
    half _Metallic;
    fixed4 _Color;
    float _Clip;
    float _Mode;
    float4 _Color1;
    void vert(inout appdata_full i, out Input o)
    {
        UNITY_INITIALIZE_OUTPUT(Input, o);
        o.localPos = i.vertex;
    }

    void surf(Input i, inout SurfaceOutputStandard o) {

        /*if (i.localPos.x >= _Clip && _Mode == 1 || i.localPos.y >= _Clip && _Mode == 2 || i.localPos.z >= _Clip && _Mode == 3)
        {
            clip(-1);
        }*/

        //老铁啊Albedo是一种颜色纹理哟
        fixed4 c = tex2D(_MainTex, i.uv_MainTex) * _Color;
        if (i.localPos.x >= _Clip && _Mode == 1 || i.localPos.y >= _Clip && _Mode == 2 || i.localPos.z >= _Clip && _Mode == 3)
        {
            //clip(-1);
            o.Albedo = _Color1;
        }
        else
        {
            o.Albedo = c.rgb;
        }
    
        // 金属感和平滑度滑动条条
        o.Metallic = _Metallic;
        o.Smoothness = _Glossiness;
        o.Alpha = c.a;
    }
    ENDCG
    }
        FallBack "Diffuse"
}
