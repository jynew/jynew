// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Drawing_Crosshatch" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 10.0)) = 1.0
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
#include "UnityCG.cginc"


uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Distortion;
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

inline float mod(float x,float modu) {
return x - floor(x * (1.0 / modu)) * modu;
}   


inline float lookup(float2 p, float dx, float dy,float2 resStep)
{

float4 c	= tex2D(_MainTex, (p.xy + float2(dx, dy)) * resStep  );

return 0.2126*c.r + 0.7152*c.g + 0.0722*c.b;
}



half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 resStep = 1. / _ScreenResolution.xy;
float width = _Distortion;

float3 res 		= float3(1.0, 1.0, 1.0);
float4 tex 		= tex2D(_MainTex, uvst.xy);
float  brightness = (0.2126*tex.x) + (0.7152*tex.y) + (0.0722*tex.z);

float dimmestChannel 	= min( min( tex.r, tex.g ), tex.b );
float brightestChannel 	= max( max( tex.r, tex.g ), tex.b );
float delta 			= brightestChannel - dimmestChannel;
if ( delta > 0.1 )
tex = tex * ( 1.0 / brightestChannel );
else
tex.rgb = float3(1.0,1.0,1.0);


float2 iText = uvst.xy * _ScreenResolution.xy;
float iTextA = iText.x  + iText.y;
float iTextS = iText.x  - iText.y;

if (brightness < 0.8) 
{
if (mod(iTextA, 10.) <= width)
{
res = float3(tex.rgb * 0.8);
}
}
if (brightness < 0.6) 
{
if (mod(iTextS, 10.) <= width)
{

res = float3(tex.rgb * 0.6);
}
}
if (brightness < 0.3) 
{
if (mod(iTextA - 5., 10.) <= width)
{
res = float3(tex.rgb * 0.3);
}
}
if (brightness < 0.15) 
{
if (mod(iTextS - 5., 10.) <= width)
{
res = float3(0.,0.,0.);
}
}

float2 p = iText;

float gx = 0.0;
float gy = 0.0;

float precalLookup = lookup(p, -1.0, -1.0,resStep);
gx += -precalLookup;
gy += -precalLookup;

precalLookup = lookup(p, -1.0,  0.0,resStep);
gx += -2.0 * precalLookup;
gx += -precalLookup;

precalLookup = lookup(p,  1.0, -1.0,resStep);
gx +=  precalLookup;
gy += -precalLookup;

gx +=  2.0 * lookup(p,  1.0,  0.0,resStep);
gx +=  lookup(p,  1.0,  1.0,resStep);
gy +=  lookup(p,  1.0,  1.0,resStep);

gy += -2.0 * lookup(p,  0.0, -1.0,resStep);
gy +=  lookup(p, -1.0,  1.0,resStep);
gy +=  2.0 * lookup(p,  0.0,  1.0,resStep);


float g = gx*gx + gy*gy;
res *= (1.0-g);

return float4(res, 1.0);

}

ENDCG
}

}
}