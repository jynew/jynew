// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Vision_Aura" { 
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Value2 ("_ColorRGB", Color) = (1,1,1,1)
_Value5 ("Speed", Range(0.0, 1.0)) = 1.0
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
uniform float _TimeX; 
uniform float _Value;
uniform float4 _Value2;
uniform float _Value3;
uniform float _Value4;
uniform float _Value5;
uniform float4 _ScreenResolution;
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
};
v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}
float voronoi(in float2 uv) 
{
float2 lp = abs(uv)*10.;
float2 sp = frac(lp)-.5;
lp = floor(lp);

float d = 1.;

for (float x = -1; x < 2; x++) {
for (float y = -1; y < 2; y++) {

float2 mp = float2(float(x),float(y));
float2 p = lp+mp;

d = min(d,length(sp+(cos(p.x+_TimeX)+cos(p.y+_TimeX))*.3-mp));

}
}

return d;
}

half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy  - float2(_Value3,_Value4);

float time=_TimeX*_Value5;

float ang = atan2(uv.y,uv.x);
float dst = length(uv)*_Value;
float cfade = clamp(dst*40.-3.+cos(ang*1.+cos(ang*6.)*1.+time*2.)*.68,0.,1.);

float a = 0.;
for (int q= 3; q < 6; q++) 
{ 
float fi = float(q);
float2 luv = uv+cos((ang-dst)*fi+time+uv+fi)*.2;
a += voronoi(luv)*(.7+(cos(luv.x*14.234)+cos(luv.y*16.234))*.4);
}

float3 color = _Value2.rgb;

float4 v=tex2D(_MainTex,uvst.xy);
color=color*a*cfade*_Value2.a;

color+=v.rgb;

return  float4(color,1.);
}
ENDCG
}
}
}
