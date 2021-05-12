// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|custl-3591-OUT,alpha-2550-OUT;n:type:ShaderForge.SFN_Tex2d,id:4933,x:31851,y:32809,ptovrint:False,ptlb:1_TEX,ptin:_1_TEX,varname:node_7302,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2369-UVOUT;n:type:ShaderForge.SFN_Multiply,id:3877,x:32417,y:32815,varname:node_3877,prsc:2|A-2085-OUT,B-9146-RGB,C-9146-A;n:type:ShaderForge.SFN_Color,id:9146,x:32218,y:32962,ptovrint:False,ptlb:1_TEX_Color,ptin:_1_TEX_Color,varname:node_7153,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:3591,x:32817,y:32812,varname:node_3591,prsc:2|A-3877-OUT,B-9312-OUT,C-5327-RGB;n:type:ShaderForge.SFN_ValueProperty,id:9312,x:32417,y:32758,ptovrint:False,ptlb:1_TEX_LD,ptin:_1_TEX_LD,varname:node_1967,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Rotator,id:2369,x:31113,y:32809,varname:node_2369,prsc:2|UVIN-3067-OUT,ANG-1906-OUT;n:type:ShaderForge.SFN_Add,id:3067,x:30864,y:32809,varname:node_3067,prsc:2|A-4072-UVOUT,B-1001-OUT;n:type:ShaderForge.SFN_TexCoord,id:4072,x:29786,y:33243,varname:node_4072,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1001,x:30623,y:32867,varname:node_1001,prsc:2|A-5421-T,B-1778-OUT;n:type:ShaderForge.SFN_Time,id:5421,x:29786,y:33401,varname:node_5421,prsc:2;n:type:ShaderForge.SFN_Append,id:1778,x:30415,y:32893,varname:node_1778,prsc:2|A-9572-X,B-9572-Y;n:type:ShaderForge.SFN_Vector4Property,id:9572,x:30233,y:32876,ptovrint:False,ptlb:1_TEX_UVspeed,ptin:_1_TEX_UVspeed,varname:node_9928,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Add,id:1906,x:30892,y:33115,varname:node_1906,prsc:2|A-8581-OUT,B-7218-OUT;n:type:ShaderForge.SFN_Multiply,id:8581,x:30602,y:33116,varname:node_8581,prsc:2|A-2640-OUT,B-5125-OUT;n:type:ShaderForge.SFN_RemapRange,id:2640,x:30417,y:33042,varname:node_2640,prsc:2,frmn:0,frmx:360,tomn:0,tomx:2|IN-6122-OUT;n:type:ShaderForge.SFN_Slider,id:6122,x:30071,y:33045,ptovrint:False,ptlb:1_TEX_UVrotator,ptin:_1_TEX_UVrotator,varname:node_4434,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:360;n:type:ShaderForge.SFN_Multiply,id:7218,x:30646,y:33306,varname:node_7218,prsc:2|A-5421-T,B-9022-OUT;n:type:ShaderForge.SFN_Slider,id:9022,x:30260,y:33327,ptovrint:False,ptlb:1_TEX_UVrotator_speed,ptin:_1_TEX_UVrotator_speed,varname:_1_TEX_UVrotator_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-30,cur:0,max:30;n:type:ShaderForge.SFN_Desaturate,id:393,x:32034,y:32845,varname:node_393,prsc:2|COL-4933-RGB;n:type:ShaderForge.SFN_SwitchProperty,id:2085,x:32218,y:32815,ptovrint:False,ptlb:1_TEX_QS,ptin:_1_TEX_QS,varname:node_9896,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-4933-RGB,B-393-OUT;n:type:ShaderForge.SFN_Pi,id:5125,x:29819,y:33547,varname:node_5125,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1464,x:32413,y:33082,varname:node_1464,prsc:2|A-9146-A,B-4933-A,C-5327-A;n:type:ShaderForge.SFN_Tex2d,id:3015,x:32217,y:33182,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_8019,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1820-UVOUT;n:type:ShaderForge.SFN_Power,id:694,x:32413,y:33233,varname:node_694,prsc:2|VAL-3015-R,EXP-2666-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2666,x:32217,y:33360,ptovrint:False,ptlb:Mask_QD,ptin:_Mask_QD,varname:node_5608,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Rotator,id:1820,x:31995,y:33182,varname:node_1820,prsc:2|UVIN-4072-UVOUT,ANG-3088-OUT;n:type:ShaderForge.SFN_Multiply,id:3088,x:31791,y:33283,varname:node_3088,prsc:2|A-6542-OUT,B-5125-OUT;n:type:ShaderForge.SFN_RemapRange,id:6542,x:31606,y:33209,varname:node_6542,prsc:2,frmn:0,frmx:360,tomn:0,tomx:2|IN-2399-OUT;n:type:ShaderForge.SFN_Slider,id:2399,x:31260,y:33212,ptovrint:False,ptlb:Mask_UVrotator,ptin:_Mask_UVrotator,varname:_1_Nois_UVrotator_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:360;n:type:ShaderForge.SFN_Multiply,id:9210,x:32599,y:33149,varname:node_9210,prsc:2|A-1464-OUT,B-694-OUT,C-5327-A;n:type:ShaderForge.SFN_SwitchProperty,id:2550,x:32819,y:33128,ptovrint:False,ptlb:Mask_switch,ptin:_Mask_switch,varname:node_5023,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-1464-OUT,B-9210-OUT;n:type:ShaderForge.SFN_VertexColor,id:5327,x:32522,y:32925,varname:node_5327,prsc:2;proporder:2085-9146-9312-4933-6122-9022-9572-2550-3015-2666-2399;pass:END;sub:END;*/

Shader "Skilleffect/00_TEX_Mask_Alpha" {
    Properties {
        [MaterialToggle] _1_TEX_QS ("1_TEX_QS", Float ) = 0
        _1_TEX_Color ("1_TEX_Color", Color) = (1,1,1,1)
        _1_TEX_LD ("1_TEX_LD", Float ) = 1
        _1_TEX ("1_TEX", 2D) = "white" {}
        _1_TEX_UVrotator ("1_TEX_UVrotator", Range(0, 360)) = 0
        _1_TEX_UVrotator_speed ("1_TEX_UVrotator_speed", Range(-30, 30)) = 0
        _1_TEX_UVspeed ("1_TEX_UVspeed", Vector) = (0,0,0,0)
        [MaterialToggle] _Mask_switch ("Mask_switch", Float ) = 0
        _Mask ("Mask", 2D) = "white" {}
        _Mask_QD ("Mask_QD", Float ) = 3
        _Mask_UVrotator ("Mask_UVrotator", Range(0, 360)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 2.0
            uniform sampler2D _1_TEX; uniform float4 _1_TEX_ST;
            uniform float4 _1_TEX_Color;
            uniform float _1_TEX_LD;
            uniform float4 _1_TEX_UVspeed;
            uniform float _1_TEX_UVrotator;
            uniform float _1_TEX_UVrotator_speed;
            uniform fixed _1_TEX_QS;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Mask_QD;
            uniform float _Mask_UVrotator;
            uniform fixed _Mask_switch;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
                float node_5125 = 3.141592654;
                float4 node_5421 = _Time;
                float node_2369_ang = (((_1_TEX_UVrotator*0.005555556+0.0)*node_5125)+(node_5421.g*_1_TEX_UVrotator_speed));
                float node_2369_spd = 1.0;
                float node_2369_cos = cos(node_2369_spd*node_2369_ang);
                float node_2369_sin = sin(node_2369_spd*node_2369_ang);
                float2 node_2369_piv = float2(0.5,0.5);
                float2 node_2369 = (mul((i.uv0+(node_5421.g*float2(_1_TEX_UVspeed.r,_1_TEX_UVspeed.g)))-node_2369_piv,float2x2( node_2369_cos, -node_2369_sin, node_2369_sin, node_2369_cos))+node_2369_piv);
                float4 _1_TEX_var = tex2D(_1_TEX,TRANSFORM_TEX(node_2369, _1_TEX));
                float3 finalColor = ((lerp( _1_TEX_var.rgb, dot(_1_TEX_var.rgb,float3(0.3,0.59,0.11)), _1_TEX_QS )*_1_TEX_Color.rgb*_1_TEX_Color.a)*_1_TEX_LD*i.vertexColor.rgb);
                float node_1464 = (_1_TEX_Color.a*_1_TEX_var.a*i.vertexColor.a);
                float node_1820_ang = ((_Mask_UVrotator*0.005555556+0.0)*node_5125);
                float node_1820_spd = 1.0;
                float node_1820_cos = cos(node_1820_spd*node_1820_ang);
                float node_1820_sin = sin(node_1820_spd*node_1820_ang);
                float2 node_1820_piv = float2(0.5,0.5);
                float2 node_1820 = (mul(i.uv0-node_1820_piv,float2x2( node_1820_cos, -node_1820_sin, node_1820_sin, node_1820_cos))+node_1820_piv);
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(node_1820, _Mask));
                return fixed4(finalColor,lerp( node_1464, (node_1464*pow(_Mask_var.r,_Mask_QD)*i.vertexColor.a), _Mask_switch ));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
