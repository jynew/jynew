// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Distortion_BlackHole" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_Distortion2 ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_PositionX ("_PositionX", Range(-1.0, 1.0)) = 1.5
_PositionY ("_PositionY", Range(-1.0, 1.0)) = 30.0
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
uniform float _Distortion2;
uniform float4 _ScreenResolution;
uniform float _PositionX;
uniform float _PositionY;

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

half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;

float2 center = float2((0.5 + _PositionX / 2) * _ScreenResolution.x , (0.5 - _PositionY / 2) * _ScreenResolution.y);
float light = clamp(0.005*distance(center.xy, (uvst.xy * _ScreenResolution.xy)) - _Distortion, 0.0, 1.0);
float light2 = 1-clamp(0.002*distance(center.xy, (uvst.xy * _ScreenResolution.xy)) - _Distortion, 0.0, 1.0);
light= light-light2;
uv = uv + float2(1-light, 1-light);
return tex2D(_MainTex, uv) * light;

}

ENDCG
}

}
}