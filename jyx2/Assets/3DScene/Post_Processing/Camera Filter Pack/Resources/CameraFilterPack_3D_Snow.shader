// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/3D_Snow" { 
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
}
SubShader
{
Pass
{
Cull Off ZWrite Off ZTest Always
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0
#pragma glsl
#include "UnityCG.cginc"
uniform sampler2D _MainTex;
uniform sampler2D Texture2;
uniform float _Visualize;
uniform sampler2D _CameraDepthTexture;
uniform float _TimeX;
uniform float _Value;
uniform float _Value2;

uniform float _Value4;
uniform float _Value5;

uniform float _Value7;
uniform float _Value8;

uniform float Drop_Near;
uniform float Drop_Far;
uniform float Drop_With_Obj;


uniform float _FixDistance;


uniform float4 _ScreenResolution;
uniform float2 _MainTex_TexelSize;
struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;


};
struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
float4 projPos : TEXCOORD1; 
float2 texcoord2 : TEXCOORD2;
};

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
OUT.projPos = ComputeScreenPos(OUT.vertex);
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
	IN.texcoord.y = 1 - IN.texcoord.y;
#endif
OUT.texcoord2 = IN.texcoord;
return OUT;
}


float rdepth(v2f i,float step)
{
float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
depth /= _FixDistance * 10;
depth = saturate(depth);
depth = smoothstep(Drop_Near+ step, Drop_Near+ step +0.01, depth);
return depth;
}

float snow(float2 uv, float dp, float depth,float speed)
{
float s = 1;
uv *= dp*_Value5;
uv.x += 0.1+sin(_Time*16)*speed*0.2;
uv.y += _TimeX*speed;
s = tex2D(Texture2, uv).r;
s = lerp(0, s*0.2, depth);
return s;
}

half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;
float2 uv2 = i.texcoord2.xy;
_TimeX *= _Value4;
float4 txt = tex2D(_MainTex, uv);
float s = 0;
float depth = rdepth(i,0.5);
s= snow(uv, 10, depth, 0.2);
depth = rdepth(i, 0.3); s+= snow(uv2 + float2(0.1,0), 8, depth,0.4);
depth = rdepth(i, 0.2); s+= snow(uv2 + float2(0.4, 0), 6, depth, 0.6);
depth = rdepth(i, 0.1); s+= snow(uv2 + float2(0.8, 0), 4, depth, 0.8);
depth = rdepth(i, 0); s+= snow(uv2 + float2(0.5, 0), 2, depth, 1);
depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
depth /= _FixDistance * 10;
depth = saturate(depth);
depth = smoothstep(Drop_Near, Drop_Far, depth);
if (_Visualize == 1) return depth;
txt= lerp(txt, txt+s*_Value2, depth *Drop_With_Obj);
return  txt;
}
ENDCG
}
}
}
