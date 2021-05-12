// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Drawing_NewCellShading" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Threshold ("_Threshold", Range(0.0, 1.0)) = 0

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
uniform float _Threshold;

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


inline float GetTexture(v2f i,float x, float y)
{
return tex2D(_MainTex, float2(x,y) / _ScreenResolution.xy).x;
}

half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);

float4 color = 0;

float x = uvst.x * _ScreenResolution.x;
float y = uvst.y * _ScreenResolution.y;

float xValue = -GetTexture(i,x-1.0, y-1.0) - 2.0*GetTexture(i,x-1.0, y) - GetTexture(i,x-1.0, y+1.0)
+ GetTexture(i,x+1.0, y-1.0) + 2.0*GetTexture(i,x+1.0, y) + GetTexture(i,x+1.0, y+1.0);
float yValue = GetTexture(i,x-1.0, y-1.0) + 2.0*GetTexture(i,x, y-1.0) + GetTexture(i,x+1.0, y-1.0)
- GetTexture(i,x-1.0, y+1.0) - 2.0*GetTexture(i,x, y+1.0) - GetTexture(i,x+1.0, y+1.0);

if(length(float2(xValue, yValue)) > _Threshold)
{
color = float4(0,0,0,0);
}
else
{
float2 uv = float2(x, y) / _ScreenResolution.xy;
color = tex2D(_MainTex, uv);
}

return color;	
}

ENDCG
}

}
}