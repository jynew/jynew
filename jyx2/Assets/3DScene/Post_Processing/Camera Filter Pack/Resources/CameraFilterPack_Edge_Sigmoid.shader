// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Edge_Sigmoid" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Gain ("_Gain", Range(1.0, 10.0)) = 3.0
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
uniform float _Distortion;
uniform float4 _ScreenResolution;
uniform float _Gain;

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
#define tex2D(sampler,uvs)  tex2Dlod( sampler , float4( ( uvs ) , 0.0f , 0.0f) )

half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;

float2 p = 1.0 / _ScreenResolution.xy;

float4 colorSum = float4(0,0,0,0);

for(int a = -1.0 ; a <= 1.0 ; a ++) {
for(int b = -1.0 ; b <= 1.0 ; b ++) {
if(b == 0.0 && a == 0.0) colorSum += tex2D(_MainTex, uv + p * float2(b,a)) * 8.0;
else colorSum += tex2D(_MainTex, uv + p * float2(b,a)) * -1.0;
}
}

float3 c = colorSum.rgb;
float avg = (c.r+c.g+c.b)/ (10- _Gain); 
float v = (1.0 - (1.0 / (1.0 + exp( -9.0 * (avg-0.18) )))) * 2.0;
return float4(v,v,v,1.0);
}

ENDCG
}

}
}