// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Blur_Radial" { 
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
uniform float _TimeX;
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
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
half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 center = float2(_Value2,_Value3);
float2 uv = uvst.xy;
uv -= center;
float4 color = float4(0.0,0.0,0.0,0.0);
float scale;
_Value*=0.075;
scale = 1 + (float(0*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(1*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(2*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(3*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(4*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(5*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(6*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(7*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(8*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(9*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(10*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(11*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(12*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(13*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(14*_Value));
color += tex2D(_MainTex, uv * scale + center);
scale = 1 + (float(15*_Value));
color += tex2D(_MainTex, uv * scale + center);
color /= float(16);
return  color;
}
ENDCG
}
}
}
