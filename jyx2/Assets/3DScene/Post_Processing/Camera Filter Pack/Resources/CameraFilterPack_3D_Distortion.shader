// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/3D_Distortion" {
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
uniform float _DistortionLevel;
uniform float _DistortionSize;
uniform float _LightIntensity;
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


half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;
#if SHADER_API_D3D9
if (_MainTex_TexelSize.y < 0)
uv.y = 1-uv.y;
#endif

float depth = LinearEyeDepth  (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);

depth/=_FixDistance*10;
float ss=smoothstep(_Near, saturate(_Near+_Far), depth);
depth =  ss * (1 - ss);

if (_Visualize == 1) return depth;


float4 result=float4(1-depth,1-depth,1-depth,1);


float2 uv2;

uv2=uv;
uv2.x=uv2.x+sin( _Time*256+uv.y*_DistortionLevel)/_DistortionSize;
uv2.y=uv2.y+cos( _Time*256+uv.x*_DistortionLevel)/_DistortionSize;

uv = lerp(uv,uv2,depth);
float4 txt = tex2D(_MainTex,uv)*(1-depth);
float4 txt2 = tex2D(_MainTex,uv)*(1-depth)*_LightIntensity;

txt = lerp(txt,txt2,depth);
return txt;
}

ENDCG
}

}
}