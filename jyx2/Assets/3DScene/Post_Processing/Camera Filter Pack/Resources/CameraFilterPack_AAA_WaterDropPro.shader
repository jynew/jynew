// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/AAA_WaterDropPro" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_MainTex2 ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_SizeX ("SizeX", Range(0.0, 1.0)) = 1.0
_SizeY ("SizeY", Range(0.0, 1.0)) = 1.0
_Speed ("Speed", Range(0.0, 10.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.87
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
uniform sampler2D _MainTex2;
uniform float _TimeX;
uniform float _SizeX;
uniform float _Speed;
uniform float _SizeY;
uniform float _Distortion;
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
};   

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;

return OUT;
}

float nrand(float2 n) {

return frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453);
}

half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
_Speed = 1-_Speed;
#endif
float3 raintex = tex2D(_MainTex2,float2(uv.x*1.3*_SizeX,(uv.y*_SizeY*1.4)+_TimeX*_Speed*0.125)).rgb/_Distortion;
float3 raintex2 = tex2D(_MainTex2,float2(uv.x*1.15*_SizeX-0.1,(uv.y*_SizeY*1.1)+_TimeX*_Speed*0.225)).rgb/_Distortion;
float3 raintex3 = tex2D(_MainTex2,float2(uv.x*_SizeX-0.2,(uv.y*_SizeY)+_TimeX*_Speed*0.025)).rgb/_Distortion;
float2 where = uv.xy-(raintex.xy-raintex2.xy-raintex3.xy)/3;
float3 col = tex2D(_MainTex,float2(where.x,where.y)).rgb;


return float4(col, 1.0);
}

ENDCG
}

}
}