// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Atmosphere_Rain_Pro_3D" { 
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
uniform sampler2D _CameraDepthTexture;
uniform float _TimeX;
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
uniform float _Value4;
uniform float _Value5;
uniform float _Value6;
uniform float _Value7;
uniform float _Value8;
uniform float _Visualize;
uniform float Drop_Near;
uniform float Drop_Far;
uniform float Drop_With_Obj;
uniform float Drop_Floor_Fluid;
uniform float Myst;
uniform float4 Myst_Color;
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
};

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
OUT.color = IN.color;
OUT.projPos = ComputeScreenPos(OUT.vertex);
return OUT;
}

float4 getPixel(in int x, in int y, float2 uvst)
{
return tex2D(_MainTex, ((uvst.xy * _ScreenResolution.xy) + float2(x, y)) / _ScreenResolution.xy);
}

float4 Edge(float2 uvst)
{
float extrax=2;
float extray=4;
float4 sum = abs(getPixel(extrax, extray, uvst) - getPixel(extrax, -extray, uvst));
sum += abs(getPixel(extrax, extray, uvst) - getPixel(-extrax, extray, uvst));
sum = abs(getPixel(extrax, extray, uvst) - getPixel(extrax, -extray, uvst));
sum += abs(getPixel(extrax, extray, uvst) - getPixel(-extrax, extray, uvst));
sum *= 1.00;
return length(sum);			
}
half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
uv.y = 1-uv.y;
#endif

float depth = LinearEyeDepth  (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
depth/=_FixDistance*10;
depth=saturate(depth);
depth = smoothstep(Drop_Near,Drop_Far,depth);
if (_Visualize == 1) return depth;

float4 result=float4(1-depth,1-depth,1-depth,1);
float dp = 1-depth;
float2 uv3 = uv;

_Value3=lerp(_Value3,0,Drop_Floor_Fluid*dp);

float2 uv2 = uv*_Value5;
_TimeX*=_Value4;
uv2.x+=_Value3*uv2.y;
_Value2*=0.25;
uv2*=3;
uv2.x+=0.1;
uv2.y+=_TimeX*0.8;
float4 txt =tex2D(Texture2, uv2).r*0.3*_Value2;
uv2*=0.65;
uv2.x+=0.1;
uv2.y+=_TimeX;
txt+=tex2D(Texture2, uv2).r*0.5*_Value2;
uv2*=0.65;
uv2.x+=0.1;
uv2.y+=_TimeX*1.2;
txt+=tex2D(Texture2, uv2).r*0.7*_Value2;
uv2*=0.5;
uv2.x+=0.1;
uv2.y+=_TimeX*1.2;
txt+=tex2D(Texture2, uv2).r*0.9*_Value2;
uv2*=0.4;
uv2.x+=0.1;
uv2.y+=_TimeX*1.2;
txt+=tex2D(Texture2, uv2).r*0.9*_Value2;
txt=lerp(txt,0,dp*Drop_With_Obj);
uv = uvst.xy;
float4 old=tex2D(_MainTex, uv);
_Value6*=depth;
uv+=float2(txt.r*_Value6,txt.r*_Value6);
float4 nt=tex2D(_MainTex, uv)+txt;
txt=lerp(old,nt,_Value);
uv = uvst.xy*0.001;
uv.x+=_TimeX*0.2;
uv.y=_TimeX*0.01;
nt=lerp(txt,txt+tex2D(Texture2, uv).g*2*_Value2,_Value7);
float4 t2=nt;
txt = Edge(uvst);
float tmp=txt;
uv = uv3+(float2(0.5,0.5)*0.8);
uv.x*=2;
float dpx=dp;
dpx=floor(dpx*16)/16;
uv/=dpx;
uv.x += floor(_TimeX*64)/64;
uv.y += floor(_TimeX*64)/16;
uv.x -= _TimeX*0.55*_Value3;
uv.y -= _TimeX*0.15;
txt = tex2D(Texture2, uv).b;
txt = lerp(float4(0,0,0,0),txt,tmp);
txt = lerp(t2,t2+txt*0.25,_Value8);
txt=lerp(old,txt,_Value);
txt=lerp(txt,Myst_Color,(1-dp)*Myst);
return  txt;
}
ENDCG
}
}
}
