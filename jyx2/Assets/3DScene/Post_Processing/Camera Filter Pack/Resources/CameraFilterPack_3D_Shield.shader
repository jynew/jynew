// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////
Shader "CameraFilterPack/3D_Shield" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.00)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_ColorRGB ("_ColorRGB", Color) = (1,1,1,1)

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
uniform float _Visualize;
uniform float _TimeX;
uniform float _Distortion;
uniform float4 _ScreenResolution;
uniform float4 _ColorRGB;
uniform float _Near;
uniform float _Far;
uniform float _FarCamera;
uniform sampler2D _CameraDepthTexture;
uniform float _FixDistance;
uniform float _LightIntensity;
uniform float _FadeShield;
uniform float2 _MainTex_TexelSize;
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
uniform float _Value4;

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
float4 projPos : TEXCOORD1; 
};   

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
OUT.projPos = ComputeScreenPos(OUT.vertex);

return OUT;
}

float col(float2 coord)
{
float time = _TimeX*1.3;
float delta_theta = 0.89759790102565521;
float col = 0.0;
float theta = 0.0;
for (int i = 0; i < 8; i++)
{
float2 adjc = coord;
theta = delta_theta*float(i);
adjc.x += cos(theta)*time*_Value + time * _Value2;
adjc.y -= sin(theta)*time*_Value - time * _Value3;
col = col + cos((adjc.x*cos(theta) - adjc.y*sin(theta))*6.0)*_Value4;
}
return cos(col);
}

float4 Fx(float2 uv)
{
float2 p = uv, c1 = p, c2 = p;
float cc1 = col(c1);
c2.x += 8.53;
float dx = 0.50*(cc1 - col(c2)) / 60;
c2.x = p.x;
c2.y += 8.53;
float dy = 0.50*(cc1 - col(c2)) / 60;
c1.x += dx*2.;
c1.y = (c1.y + dy*2.);
float alpha = 1. + dot(dx,dy) * 700;
float ddx = dx - 0.012;
float ddy = dy - 0.012;
if (ddx > 0. && ddy > 0.) alpha = pow(alpha, ddx*ddy * 200000);
float4 col = tex2D(_MainTex,c1)*(alpha);
return  col;
}



half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;


float depth = LinearEyeDepth  (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);

depth/=_FixDistance*10;
float ss=smoothstep(_Near, saturate(_Near+_Far), depth);
depth = ss;
if (_Visualize == 1) return depth;

float4 txt = tex2D(_MainTex, uv);
float4 txt2 = Fx(uv)*_LightIntensity;
txt2 = lerp(txt, txt2, _FadeShield);
//_FadeShield

txt = lerp(txt,txt2,depth);
return txt;
}

ENDCG
}

}
}