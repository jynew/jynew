Shader "ErbGameArt/Particles/Blend_CenterGlow" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _Emission ("Emission", Float ) = 2
        _SpeedMainTexUVNoiseZW ("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
        _Opacity ("Opacity", Range(0, 1)) = 1
        [MaterialToggle] _Usedepth ("Use depth?", Float ) = 0
        _Depthpower ("Depth power", Float ) = 1
        [MaterialToggle] _Usecenterglow ("Use center glow?", Float ) = 0
        _Mask ("Mask", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
			"PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Emission;
            uniform fixed _Usecenterglow;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float4 _SpeedMainTexUVNoiseZW;
            uniform float _Opacity;
            uniform fixed _Usedepth;
            uniform float _Depthpower;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float2 ttt = (((_Time.g*float2(_SpeedMainTexUVNoiseZW.r,_SpeedMainTexUVNoiseZW.g))+i.uv0));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(ttt, _MainTex));
                float2 tt = (((_Time.g*float2(_SpeedMainTexUVNoiseZW.b,_SpeedMainTexUVNoiseZW.a))+i.uv0));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(tt, _Noise));
                float3 maincolor = ((_MainTex_var.rgb*_Noise_var.rgb)*i.vertexColor.rgb*_TintColor.rgb);
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 emissive = (_Emission*lerp( maincolor, (maincolor*saturate((_Mask_var.rgb*saturate((_Mask_var.rgb-(i.uv0.b*-1.0+1.0)))))), _Usecenterglow ));
                float ff = (_MainTex_var.a*_Noise_var.a*i.vertexColor.a*_TintColor.a);
                fixed4 finalRGBA = fixed4(emissive,(lerp( ff, (ff*saturate((sceneZ-partZ)/_Depthpower)), _Usedepth )*_Opacity));
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
}
