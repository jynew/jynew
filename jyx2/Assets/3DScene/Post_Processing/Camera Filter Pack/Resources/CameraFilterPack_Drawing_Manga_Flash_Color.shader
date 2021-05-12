// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
/////////////////////////////////////////////


Shader "CameraFilterPack/Drawing_Manga_Flash_Color" { 
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
uniform float _Value4;
uniform float _Intensity;
uniform float4 Color;


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



float noise( float2 val ) 
{
return frac(sin(dot(val.xy ,float2(12.9898,78.233))) * 43758.5453);
}

float getFrameTime() 
{
float time = _TimeX;
return floor(time * _Value2) / _Value2;
}

half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst;
float2 vec = uvst - float2(_Value3,_Value4);
float PI = 3.141592653589793*_Value;
float l = length(vec) / length(float2(1.0,1.0) - float2(0.5,0.5));
float r = (atan2(vec.y, vec.x) + PI) / (2.0 * PI);
float t = getFrameTime();
t = max(t, 0.1);
float r2 = floor(r * 700.0) / 700.0 * t;
float ran = noise( float2(r2, r2) ) * 0.7 + 0.3;

float c = l > ran ? abs(l - ran) : 0.0;

float4 v=tex2D(_MainTex,uvst);
v=lerp(v,Color,c*_Intensity);
return  float4( v );
}
ENDCG
}
}
}
