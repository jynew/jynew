// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Drawing_EnhancedComics" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_DotSize ("_DotSize", Range(0.0, 1.0)) = 0
_ColorRGB ("_ColorRGB", Color) = (1,0,0,1)
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
#include "UnityCG.cginc"


uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Distortion;
uniform float4 _ScreenResolution;
uniform float4 _ColorRGB;
uniform float _DotSize;
uniform float _ColorR;
uniform float _ColorG;
uniform float _ColorB;
uniform float _Blood;
uniform float _SmoothStart;
uniform float _SmoothEnd;

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
float3 color = tex2D(_MainTex,uvst);
float3 brtColor = color;
color = smoothstep(_DotSize+_SmoothStart, 0.1+_DotSize+_SmoothEnd,(color.r+color.g+color.b)/3);
float3 color2 = lerp(float3(1.0,_Blood,_Blood), lerp(float3(0, 0, 0), brtColor, 8.88), 8.39);
if ((color2.r>_ColorR) && (color2.g<_ColorG) && (color2.b<_ColorB)) color.rgb = _ColorRGB.rgb;

return float4(color,1.);
}

ENDCG
}

}
}