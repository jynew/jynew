// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Drawing_Paper" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_MainTex2 ("Base (RGB)", 2D) = "white" {}
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
uniform sampler2D _MainTex2;
uniform float4 _PColor;
uniform float4 _PColor2;
uniform float _TimeX;
uniform float _Value1;
uniform float _Value2;
uniform float _Value3;
uniform float _Value4;
uniform float _Value5;
uniform float _Value6;
uniform float _Value7;


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
float4 f = tex2D(_MainTex, uvst);
float3 paper = tex2D(_MainTex2,uvst).rgb;
float ce = 1;
float4 tex1[4];
float4 tex2[4]; 
float tex=_Value1;
float tt=_TimeX*_Value4;
float s=floor(sin(tt*10)*0.02)/12;
float c=floor(cos(tt*10)*0.02)/12;
float2 dist=float2(c+paper.b*0.02,s+paper.b*0.02);
float3 paper2 = tex2D(_MainTex2,uvst+dist).rgb;
tex2[0] = tex2D(_MainTex, uvst+float2(tex,0)+dist/128);
tex2[1] = tex2D(_MainTex, uvst+float2(-tex,0)+dist/128);
tex2[2] = tex2D(_MainTex, uvst+float2(0,tex)+dist/128);
tex2[3] = tex2D(_MainTex, uvst+float2(0,-tex)+dist/128);

for(int i = 0; i < 4; i++) 
{
tex1[i] = saturate(1.0-distance(tex2[i].r, f.r));
tex1[i] *= saturate(1.0-distance(tex2[i].g, f.g));
tex1[i] *= saturate(1.0-distance(tex2[i].b, f.b));
tex1[i] = pow(tex1[i], _Value2*25);
ce *= dot(tex1[i], 1.0);
}

ce=saturate(ce);
float l = 1-ce;
float3 ax = l; 
ax*=paper2.b;
ax=lerp(float3(0.0,0.0,0.0),ax*_Value3*1.5,1);
float gg=lerp(1-paper.g,0,1-_Value5);
ax=lerp(ax,float3(0.0,0.0,0.0),gg);
paper=lerp(float3(paper.r,paper.r,paper.r),_PColor2.rgb,_Value6); 
paper=lerp(paper,_PColor.rgb,ax*_Value3);
paper = lerp(f,paper,_Value7);
return float4(paper, 1.0);

}

ENDCG
}

}
}