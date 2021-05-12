// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////
Shader "CameraFilterPack/3D_Myst" 
{
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
uniform sampler2D _MainTex2;
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
float4 txt = tex2D(_MainTex,uv);

#if SHADER_API_D3D9
if (_MainTex_TexelSize.y < 0)
uv.y = 1-uv.y;
#endif
float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
float yOff = _WorldSpaceCameraPos.y;
depth/=_FixDistance*10;
float ss=smoothstep(_Near, saturate(_Near+_Far), depth);
float depth2 = ss;
depth =  ss * (1 - ss);
depth = depth+depth2;

if (_Visualize == 1) return depth;

uv.y+=yOff*0.2;
uv.x+=_Time*2;
float4 txt2 = tex2D(_MainTex2,uv);
uv.x+=_Time*2;
uv.y+=0.05*depth*8;
uv/=1.5;
float4 txt3 = tex2D(_MainTex2,uv);
uv.y+=0.15*depth*8;
uv/=2.5;
float4 txt4 = tex2D(_MainTex2,uv);
txt2.rgb=lerp(txt2.r,txt3.r,txt4.b);
txt2 = txt2.r+txt3.g+txt4.r;
txt2 = txt2*0.33;
txt = lerp(txt,txt+txt2,depth);
return txt;
}

ENDCG
}

}
}