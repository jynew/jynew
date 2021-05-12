// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33192,y:32988,varname:node_4795,prsc:2|emission-6145-OUT,alpha-5022-OUT;n:type:ShaderForge.SFN_Fresnel,id:1648,x:30944,y:33080,varname:node_1648,prsc:2|EXP-391-OUT;n:type:ShaderForge.SFN_Slider,id:391,x:30569,y:33066,ptovrint:False,ptlb:W_fangwei,ptin:_W_fangwei,varname:node_391,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:3245,x:31269,y:33209,varname:node_3245,prsc:2|A-1648-OUT,B-7226-RGB,C-8892-OUT;n:type:ShaderForge.SFN_Color,id:7226,x:30894,y:33309,ptovrint:False,ptlb:W_Color,ptin:_W_Color,varname:node_7226,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1,c2:0.6,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:6145,x:31664,y:33299,varname:node_6145,prsc:2|A-3245-OUT,B-2223-RGB;n:type:ShaderForge.SFN_OneMinus,id:4630,x:31630,y:33803,varname:node_4630,prsc:2|IN-4524-A;n:type:ShaderForge.SFN_Subtract,id:2474,x:31912,y:33772,varname:node_2474,prsc:2|A-7226-A,B-4630-OUT;n:type:ShaderForge.SFN_Smoothstep,id:5950,x:32215,y:34023,varname:node_5950,prsc:2|A-3441-OUT,B-8990-OUT,V-2474-OUT;n:type:ShaderForge.SFN_Slider,id:8990,x:31760,y:34207,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_6508,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.213675,max:5;n:type:ShaderForge.SFN_ConstantClamp,id:5022,x:32494,y:33808,varname:node_5022,prsc:2,min:0,max:1|IN-5950-OUT;n:type:ShaderForge.SFN_VertexColor,id:4524,x:31301,y:33700,varname:node_4524,prsc:2;n:type:ShaderForge.SFN_Vector1,id:3441,x:32063,y:33626,varname:node_3441,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:8892,x:31129,y:33022,ptovrint:False,ptlb:Glow,ptin:_Glow,varname:node_8892,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.776699,max:10;n:type:ShaderForge.SFN_Color,id:2223,x:31427,y:33406,ptovrint:False,ptlb:N_Color,ptin:_N_Color,varname:node_2223,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;proporder:7226-2223-391-8990-8892;pass:END;sub:END;*/

Shader "WJL_Wjl_Miaobian" {
    Properties {
        _W_Color ("W_Color", Color) = (0.1,0.6,1,1)
        _N_Color ("N_Color", Color) = (0.5,0.5,0.5,1)
        _W_fangwei ("W_fangwei", Range(0, 10)) = 1
        _Alpha ("Alpha", Range(0, 5)) = 1.213675
        _Glow ("Glow", Range(0, 10)) = 0.776699
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 3.0
            uniform float _W_fangwei;
            uniform float4 _W_Color;
            uniform float _Alpha;
            uniform float _Glow;
            uniform float4 _N_Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float3 emissive = ((pow(1.0-max(0,dot(normalDirection, viewDirection)),_W_fangwei)*_W_Color.rgb*_Glow)+_N_Color.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,clamp(smoothstep( 0.0, _Alpha, (_W_Color.a-(1.0 - i.vertexColor.a)) ),0,1));
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0.5,0.5,0.5,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
