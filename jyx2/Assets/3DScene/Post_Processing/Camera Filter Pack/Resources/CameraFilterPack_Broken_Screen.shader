// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Broken_Screen" {
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
uniform float _Fade;
uniform float _Shadow;
uniform float2 _MainTex_TexelSize;

struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
float2 texcoord2 : TEXCOORD1;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float2 texcoord2  : TEXCOORD1;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0) IN.texcoord.y = 1 - IN.texcoord.y;
#endif
OUT.texcoord2 = IN.texcoord;
return OUT; 
}


half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uvst2 = uvst;

float2 uv = uvst.xy;
float2 uv2 = uvst2.xy;
float4 txtd = tex2D(_MainTex2, uv2);
uv2 = float2(txtd.r, 1-txtd.g);
uv2 = lerp(uv, uv2, _Fade);
float4 txt = tex2D(_MainTex, uv2);

txt -= txtd.b*_Fade*_Shadow;


return txt;
}

ENDCG
}

}
}