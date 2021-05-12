// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Glow_Glow_Color" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_MainTex2 ("Blurred (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Amount ("_Amount", Range(0.0, 20.0)) = 5.0
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
uniform sampler2D _MainTex2;
uniform float _TimeX;
uniform float _Amount;
uniform float _Value1;
uniform float _Value2;
uniform float _Value3;

struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 uv_MainTex : TEXCOORD0;
};

struct v2f
{
float2 uv_MainTex  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};   

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.uv_MainTex = IN.uv_MainTex;
OUT.color = IN.color;

return OUT;
}

float4 frag (v2f i) : COLOR
{
float stepU = _Amount/_ScreenParams.x;
float3 result = tex2D(_MainTex,i.uv_MainTex - float2(0, stepU)).rgb;
result += tex2D(_MainTex,i.uv_MainTex).rgb;
result += tex2D(_MainTex,i.uv_MainTex + float2(0, stepU)).rgb;
result /= 3;
return float4(result,1.0);	
}

ENDCG
}

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
uniform sampler2D _MainTex2;
uniform float _TimeX;
uniform float _Value1;
uniform float _Value2;
uniform float _Value3;
uniform float _Amount;
uniform float4 _GlowColor;

struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 uv_MainTex : TEXCOORD0;
};

struct v2f
{
float2 uv_MainTex  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};   

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.uv_MainTex = IN.uv_MainTex;
OUT.color = IN.color;

return OUT;
}

float4 frag (v2f i) : COLOR
{
float3 result=tex2D(_MainTex,i.uv_MainTex.xy).rgb;
float3 cm=tex2D(_MainTex2,i.uv_MainTex.xy).rgb;
float c=cm*_Value2;
result += c*_GlowColor.rgb;
return float4(result,1.0);	
}

ENDCG
}

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
uniform sampler2D _MainTex2;
uniform float _TimeX;
uniform float _Amount;
uniform float _Value1;
uniform float _Value2;
uniform float _Value3;

struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 uv_MainTex : TEXCOORD0;
};

struct v2f
{
float2 uv_MainTex  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};   

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.uv_MainTex = IN.uv_MainTex;
OUT.color = IN.color;

return OUT;
}

float4 frag (v2f i) : COLOR
{
float stepU = _Amount/_ScreenParams.x;
float3 result = tex2D(_MainTex,i.uv_MainTex - float2(stepU, 0)).rgb;
result += tex2D(_MainTex,i.uv_MainTex).rgb;
result += tex2D(_MainTex,i.uv_MainTex + float2(stepU, 0)).rgb;
result /= 3;
return float4(result,1.0);	
}

ENDCG
}
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
uniform sampler2D _MainTex2;
uniform float _TimeX;
uniform float _Amount;
uniform float _Value1;
uniform float _Value2;
uniform float _Value3;

struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 uv_MainTex : TEXCOORD0;
};

struct v2f
{
float2 uv_MainTex  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};   

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.uv_MainTex = IN.uv_MainTex;
OUT.color = IN.color;

return OUT;
}

float4 frag (v2f i) : COLOR
{
float stepU = _Amount/_ScreenParams.x;
float3 result = tex2D(_MainTex,i.uv_MainTex - float2(stepU, 0)).rgb;
float l = dot(result,0.33);
result = smoothstep(_Value1, _Value1+_Value3, l);
return float4(result,1.0);	
}

ENDCG
}

}
}