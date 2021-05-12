// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32794,y:32684,varname:node_3138,prsc:2|emission-3178-OUT,custl-7666-OUT,alpha-6644-OUT;n:type:ShaderForge.SFN_Tex2d,id:3755,x:30923,y:33091,ptovrint:False,ptlb:T_mask,ptin:_T_mask,varname:node_3755,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6699-OUT;n:type:ShaderForge.SFN_If,id:5746,x:31505,y:32951,varname:node_5746,prsc:2|A-1423-OUT,B-3755-R,GT-6272-OUT,EQ-6272-OUT,LT-3073-OUT;n:type:ShaderForge.SFN_Vector1,id:6272,x:31244,y:33096,varname:node_6272,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:3073,x:31244,y:33221,varname:node_3073,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2d,id:5620,x:31889,y:32523,ptovrint:False,ptlb:Diffuse Texture,ptin:_DiffuseTexture,varname:_node_3755_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6699-OUT;n:type:ShaderForge.SFN_Multiply,id:6731,x:32197,y:32904,varname:node_6731,prsc:2|A-5620-A,B-6450-OUT;n:type:ShaderForge.SFN_Color,id:7519,x:32099,y:32351,ptovrint:False,ptlb:Color&Alpha,ptin:_ColorAlpha,varname:node_7519,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:3808,x:32384,y:32437,varname:node_3808,prsc:2|A-7519-RGB,B-5620-RGB;n:type:ShaderForge.SFN_Multiply,id:6644,x:32467,y:32846,varname:node_6644,prsc:2|A-7519-A,B-6731-OUT;n:type:ShaderForge.SFN_ValueProperty,id:329,x:31009,y:32846,ptovrint:False,ptlb:N_mask,ptin:_N_mask,varname:node_329,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.3;n:type:ShaderForge.SFN_If,id:5217,x:31525,y:33205,varname:node_5217,prsc:2|A-1423-OUT,B-2022-OUT,GT-6272-OUT,EQ-6272-OUT,LT-3073-OUT;n:type:ShaderForge.SFN_Add,id:2022,x:31106,y:33159,varname:node_2022,prsc:2|A-3755-R,B-5828-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5828,x:30875,y:33257,ptovrint:False,ptlb:N_BY_KD,ptin:_N_BY_KD,varname:node_5828,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.01;n:type:ShaderForge.SFN_Subtract,id:1274,x:31735,y:33102,varname:node_1274,prsc:2|A-5746-OUT,B-5217-OUT;n:type:ShaderForge.SFN_Color,id:9508,x:31735,y:33271,ptovrint:False,ptlb:C_BYcolor,ptin:_C_BYcolor,varname:node_9508,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:7346,x:31929,y:33178,varname:node_7346,prsc:2|A-1274-OUT,B-9508-RGB;n:type:ShaderForge.SFN_ValueProperty,id:8447,x:31735,y:33450,ptovrint:False,ptlb:N_BY_QD,ptin:_N_BY_QD,varname:node_8447,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:7666,x:32226,y:33231,varname:node_7666,prsc:2|A-7346-OUT,B-8447-OUT;n:type:ShaderForge.SFN_Add,id:6450,x:31967,y:33031,varname:node_6450,prsc:2|A-5746-OUT,B-1274-OUT;n:type:ShaderForge.SFN_VertexColor,id:3457,x:31064,y:32653,varname:node_3457,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1423,x:31247,y:32802,varname:node_1423,prsc:2|A-3457-A,B-329-OUT;n:type:ShaderForge.SFN_Multiply,id:3178,x:32607,y:32614,varname:node_3178,prsc:2|A-3808-OUT,B-3457-RGB;n:type:ShaderForge.SFN_Tex2d,id:3416,x:29954,y:33024,ptovrint:False,ptlb:FlowMap,ptin:_FlowMap,varname:node_9162,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e3b13026bb0b65547b97b3903067cb46,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ComponentMask,id:6782,x:30216,y:33029,varname:node_6782,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-3416-RGB;n:type:ShaderForge.SFN_TexCoord,id:1477,x:30336,y:32834,varname:node_1477,prsc:2,uv:2,uaff:False;n:type:ShaderForge.SFN_Lerp,id:6699,x:30553,y:33055,varname:node_6699,prsc:2|A-6782-OUT,B-1477-UVOUT,T-4399-OUT;n:type:ShaderForge.SFN_VertexColor,id:6085,x:29988,y:33292,varname:node_6085,prsc:2;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:4399,x:30264,y:33352,varname:node_4399,prsc:2|IN-6085-A,IMIN-2448-OUT,IMAX-8961-OUT,OMIN-2062-OUT,OMAX-6663-OUT;n:type:ShaderForge.SFN_Vector1,id:2448,x:29635,y:33308,varname:node_2448,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:8961,x:29658,y:33446,varname:node_8961,prsc:2,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2062,x:29604,y:33545,ptovrint:False,ptlb:MinDist,ptin:_MinDist,varname:node_1962,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:6663,x:29675,y:33641,ptovrint:False,ptlb:MaxDist,ptin:_MaxDist,varname:node_9396,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:4316,x:30745,y:32755,ptovrint:False,ptlb:FlowMap_copy,ptin:_FlowMap_copy,varname:_FlowMap_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e3b13026bb0b65547b97b3903067cb46,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ComponentMask,id:7508,x:31007,y:32760,varname:node_7508,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-4316-RGB;n:type:ShaderForge.SFN_TexCoord,id:4061,x:31127,y:32565,varname:node_4061,prsc:2,uv:2,uaff:False;n:type:ShaderForge.SFN_Lerp,id:9102,x:31344,y:32786,varname:node_9102,prsc:2|A-7508-OUT,B-4061-UVOUT,T-3212-OUT;n:type:ShaderForge.SFN_VertexColor,id:3360,x:30779,y:33023,varname:node_3360,prsc:2;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:3212,x:31055,y:33083,varname:node_3212,prsc:2|IN-3360-A,IMIN-3176-OUT,IMAX-7139-OUT,OMIN-8784-OUT,OMAX-888-OUT;n:type:ShaderForge.SFN_Vector1,id:3176,x:30711,y:33195,varname:node_3176,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:7139,x:30734,y:33333,varname:node_7139,prsc:2,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:8784,x:30680,y:33432,ptovrint:False,ptlb:MinDist_copy,ptin:_MinDist_copy,varname:_MinDist_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:888,x:30751,y:33528,ptovrint:False,ptlb:MaxDist_copy,ptin:_MaxDist_copy,varname:_MaxDist_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:7519-5620-329-3755-9508-8447-5828-3416-2062-6663;pass:END;sub:END;*/

Shader "WJL/WJL_dissolution_blend" {
    Properties {
        _ColorAlpha ("Color&Alpha", Color) = (1,1,1,1)
        _DiffuseTexture ("Diffuse Texture", 2D) = "white" {}
        _N_mask ("N_mask", Float ) = 0.3
        _T_mask ("T_mask", 2D) = "white" {}
        _C_BYcolor ("C_BYcolor", Color) = (1,0,0,1)
        _N_BY_QD ("N_BY_QD", Float ) = 3
        _N_BY_KD ("N_BY_KD", Float ) = 0.01
        _FlowMap ("FlowMap", 2D) = "white" {}
        _MinDist ("MinDist", Float ) = 0
        _MaxDist ("MaxDist", Float ) = 1
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
            Cull Off
            ZWrite on
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _T_mask; uniform float4 _T_mask_ST;
            uniform sampler2D _DiffuseTexture; uniform float4 _DiffuseTexture_ST;
            uniform float4 _ColorAlpha;
            uniform float _N_mask;
            uniform float _N_BY_KD;
            uniform float4 _C_BYcolor;
            uniform float _N_BY_QD;
            uniform sampler2D _FlowMap; uniform float4 _FlowMap_ST;
            uniform float _MinDist;
            uniform float _MaxDist;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _FlowMap_var = tex2D(_FlowMap,TRANSFORM_TEX(i.uv0, _FlowMap));
                float node_2448 = 0.0;
                float2 node_6699 = lerp(_FlowMap_var.rgb.rg,i.uv2,(_MinDist + ( (i.vertexColor.a - node_2448) * (_MaxDist - _MinDist) ) / (1.0 - node_2448)));
                float4 _DiffuseTexture_var = tex2D(_DiffuseTexture,TRANSFORM_TEX(node_6699, _DiffuseTexture));
                float3 emissive = ((_ColorAlpha.rgb*_DiffuseTexture_var.rgb)*i.vertexColor.rgb);
                float node_1423 = (i.vertexColor.a*_N_mask);
                float4 _T_mask_var = tex2D(_T_mask,TRANSFORM_TEX(node_6699, _T_mask));
                float node_5746_if_leA = step(node_1423,_T_mask_var.r);
                float node_5746_if_leB = step(_T_mask_var.r,node_1423);
                float node_3073 = 0.0;
                float node_6272 = 1.0;
                float node_5746 = lerp((node_5746_if_leA*node_3073)+(node_5746_if_leB*node_6272),node_6272,node_5746_if_leA*node_5746_if_leB);
                float node_5217_if_leA = step(node_1423,(_T_mask_var.r+_N_BY_KD));
                float node_5217_if_leB = step((_T_mask_var.r+_N_BY_KD),node_1423);
                float node_1274 = (node_5746-lerp((node_5217_if_leA*node_3073)+(node_5217_if_leB*node_6272),node_6272,node_5217_if_leA*node_5217_if_leB));
                float3 finalColor = emissive + ((node_1274*_C_BYcolor.rgb)*_N_BY_QD);
                return fixed4(finalColor,(_ColorAlpha.a*(_DiffuseTexture_var.a*(node_5746+node_1274))));
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
