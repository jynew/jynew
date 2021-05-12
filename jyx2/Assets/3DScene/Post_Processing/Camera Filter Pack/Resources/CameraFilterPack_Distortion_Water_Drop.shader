// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Distortion_Water_Drop" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_CenterX ("_CenterX", Range(-1.0, 1.0)) = 0
_CenterY ("_CenterY", Range(-1.0, 1.0)) = 0
_WaveIntensity ("_WaveIntensity", Range(0, 10)) = 0
_NumberOfWaves ("_NumberOfWaves", Range(0, 10)) = 0
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
uniform float _CenterX;
uniform float _CenterY;
uniform float _WaveIntensity;
uniform int _NumberOfWaves;

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

float genWave(float len, float PI, float time)
{
float wave = sin(8.0f * PI * len + time);
wave = (wave + 1.0) * 0.5; 
wave -= 0.3;
wave *= wave * wave;
return wave;
}

float scene(float len, float PI, float time)
{
return genWave(len, PI, time) * _WaveIntensity/3;
}

float2 normal(float len, float PI, float time) 
{
float3 eps = float3(0.01, 0.0, 0.0);
float tg = (scene(len + eps.x, PI, time) - scene(len, PI, time)) / eps.x;
return normalize(float2(-tg, 1.0));
}

half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float PI = 3 + _NumberOfWaves;
float time = -_TimeX * 5.0;

float2 so = float2((0.5 + _CenterX * 0.5) , (0.5 - _CenterY * 0.5));
float2 pos2 = float2(uvst.xy - so);

float len = length(pos2);
float wave = scene(len, PI, time); 

float2 uv2 = -normalize(pos2) * wave/(1.0 + 5.0 * len);

return tex2D(_MainTex, uvst.xy + uv2);
}

ENDCG
}

}
}