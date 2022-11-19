// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "JYX2/BigMapTree"{
    Properties {
        _MainTex ("Main Tex", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
    }
    SubShader {
        // Need to disable batching because of the vertex animation
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "DisableBatching"="True"}
        Cull off
            Pass { 
                Tags { "LightMode"="ForwardBase" }

                ZWrite On
                    Blend SrcAlpha OneMinusSrcAlpha
                    Cull Off

                    CGPROGRAM

#pragma vertex vert
#pragma fragment frag

#include "Lighting.cginc"

                    sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed4 _Color;

                struct a2v {
                    float4 vertex : POSITION;
                    float4 texcoord : TEXCOORD0;
                };

                struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                v2f vert (a2v v) {
                    v2f o;

                    float3 upCamVec = float3( 0, 1, 0 );
                    float3 forwardCamVec = -normalize( UNITY_MATRIX_V._m20_m21_m22 );
                    float3 rightCamVec = normalize( UNITY_MATRIX_V._m00_m01_m02 );
                    float4x4 rotationCamMatrix = float4x4( rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1 );
                    v.vertex.x *= length( unity_ObjectToWorld._m00_m10_m20 );
                    v.vertex.y *= length( unity_ObjectToWorld._m01_m11_m21 );
                    v.vertex.z *= length( unity_ObjectToWorld._m02_m12_m22 );
                    v.vertex = mul( v.vertex, rotationCamMatrix );
                    v.vertex.xyz += unity_ObjectToWorld._m03_m13_m23;
                    //Need to nullify rotation inserted by generated surface shader;
                    v.vertex = mul( unity_WorldToObject, v.vertex );

                    o.pos = UnityObjectToClipPos(v.vertex.xyz);
                    o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                    o.uv.x = 1 - o.uv.x;

                    return o;
                }

                fixed4 frag (v2f i) : SV_Target {
                    fixed4 c = tex2D (_MainTex, i.uv);
                    c.rgb *= _Color.rgb;

                    return c;
                }

                ENDCG
            }
    } 
    FallBack "Transparent/VertexLit"
}
