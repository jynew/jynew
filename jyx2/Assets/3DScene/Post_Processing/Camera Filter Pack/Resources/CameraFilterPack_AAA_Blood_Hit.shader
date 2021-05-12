// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/AAA_Blood_Hit" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_MainTex2 ("Base (RGB)", 2D) = "white" {}
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
uniform float _Speed;
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
uniform float _Value4;
uniform float _Value5;
uniform float _Value6;
uniform float _Value7;
uniform float _Value8;
uniform float _Value9;
uniform float _Value10;
uniform float _Value11;
uniform float _Value12;
uniform float _Value13;

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
	float2 texcoord2  : TEXCOORD1;
	float4 vertex   : SV_POSITION;
	float4 color : COLOR;
};

v2f vert(appdata_t IN)
{
	v2f OUT;
	OUT.vertex = UnityObjectToClipPos(IN.vertex);
	OUT.texcoord = IN.texcoord;
	#if UNITY_UV_STARTS_AT_TOP
	if (_MainTex_TexelSize.y < 0)
	IN.texcoord.y = 1 - IN.texcoord.y;
	#endif
	OUT.texcoord2 = IN.texcoord;
	OUT.color = IN.color;
	return OUT;
}


half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;
float2 uv2 = uv;
float2 uv4 = i.texcoord.xy;

float2 uv3 = uv4 * 0.5 + float2(0.0,0.5);
float3 col1 = (tex2D(_MainTex2,uv3).rgb);

uv3=uv*0.5+float2(0.5,0.5);
float3 col2 = (tex2D(_MainTex2,uv3).rgb);

uv3=uv*0.5+float2(0.0,0.0);
float3 col3 = (tex2D(_MainTex2,uv3).rgb);

uv3=uv*0.5+float2(0.5,0.0);
float3 col4 = (tex2D(_MainTex2,uv3).rgb);

float col=0;

col  = col1.r*_Value2;
col += col2.r*_Value3;
col += col3.r*_Value4;
col += col4.r*_Value5;

col += col1.g*_Value6;
col += col2.g*_Value7;
col += col3.g*_Value8;
col += col4.g*_Value9;

col += col1.b*_Value10;
col += col2.b*_Value11;
col += col3.b*_Value12;
col += col4.b*_Value13;

col*=_Value;
uv2+=float2(col,col)*0.0625;

float3 mcol = tex2D(_MainTex,uv2).rgb;

mcol=mcol+(col/16)*0.0625;
mcol.r=mcol.r+col;

return float4(mcol, 1.0);

}

ENDCG
}

}
}