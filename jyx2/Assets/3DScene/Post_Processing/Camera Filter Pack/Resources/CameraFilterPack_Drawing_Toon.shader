// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Drawing_Toon" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_DotSize ("_DotSize", Range(0.0, 1.0)) = 0
_ColorLevel ("_ColorLevel", Range(0.0, 10.0)) = 7
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
uniform float _DotSize;
uniform float _ColorLevel;

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


inline float3 RecolorForeground(float3 color)
{
if(color.g > (color.r + color.b)*12.8)
{
color.rgb = float3(0.,0.,0.);
}


color.rgb = 0.2126*color.rrr + 0.7152*color.ggg + 0.0722*color.bbb;

if(color.r > 0.95)
{

}
else if(color .r > 0.75)
{
color.r *= 0.9;
}
else if(color.r > 0.5)
{
color.r *= 0.7;
color.g *=0.9;
}
else if (color.r > 0.25)
{
color.r *=0.5;
color.g *=0.75;
}
else
{
color.r *= 0.25;
color.g *= 0.5;
}


return color;
}

inline float3 Posterize(float3 color)
{
color = pow(color, float3(0.65, 0.65, 0.65));
color = floor(color * 5.)/5.;
color = pow(color, 1.53846154);
return color.rgb;
}

inline float3 Outline(float2 uv)
{
float4 lines= float4(0.30, 0.59, 0.11, 1.0);

lines.rgb = lines.rgb * 1.5;
lines /= 4.0;

float S512 = (1.0 / 512.)*_DotSize;

float s11 = dot(tex2D(_MainTex, uv + float2(-S512, -S512)), lines);  
float s12 = s11;            
float s13 = dot(tex2D(_MainTex, uv + float2(S512, -S512)), lines);   
float s21 = dot(tex2D(_MainTex, uv + float2(-S512, 0.0)), lines); 
float s23 = s21;
float s31 = dot(tex2D(_MainTex, uv + float2(-S512, S512)), lines);
float s32 = dot(tex2D(_MainTex, uv + float2(0, S512)), lines); 
float s33 = dot(tex2D(_MainTex, uv + float2(S512, S512)), lines);

float t1 = s13 + s33 + (2.0 * s23) - s11 - (2.0 * s21) - s31;
float t2 = s31 + (2.0 * s32) + s33 - s11 - (2.0 * s12) - s13;

float3 col;

if (((t1 * t1) + (t2* t2)) > 0.04) 
{
col = float3(-1.,-1.,-1.);
}
else
{
col = float3(0.,0.,0.);
}

return col;
}


half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv 			= uvst.xy;
float3 color 		= normalize(tex2D(_MainTex,uv)).rgb*2.5;	
color 				= Posterize(color)*_Distortion;
color.rgb 		   += Outline(uv);
color 				= RecolorForeground(color);

return float4(color,1.);



}

ENDCG
}

}
}