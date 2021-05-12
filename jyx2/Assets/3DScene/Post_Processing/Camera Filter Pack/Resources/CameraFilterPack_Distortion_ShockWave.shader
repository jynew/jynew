// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Distortion_ShockWave" { 
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
float time=_TimeX*_Value3;
float offset = (time- floor(time))/time;
float CurrentTime = (time)*(offset);    
float3 WaveParams = float3(10.0, 0.8, 0.1); 
float2 WaveCentre = float2(_Value, _Value2);
float2 texCoord = uvst.xy ;      
float Dist = distance(texCoord, WaveCentre);
float ScaleDiff = 1.0;
float Change=0;
if ((Dist <= ((CurrentTime) + (WaveParams.z))) && 
(Dist >= ((CurrentTime) - (WaveParams.z)))) 
{
float Diff = (Dist - CurrentTime); 
ScaleDiff = (1.0 - pow(abs(Diff * WaveParams.x), WaveParams.y)); 
float DiffTime = (Diff  * ScaleDiff);       
float2 DiffTexCoord = normalize(texCoord - WaveCentre);  
Change=1;     
texCoord += ((DiffTexCoord * DiffTime) / (CurrentTime * Dist * 40.0));
}  
float4 Color = tex2D(_MainTex, texCoord); 
Color += ((Color * ScaleDiff) / (CurrentTime * Dist * 40.0))*Change; 
return Color; 
}
ENDCG
}
}
}
