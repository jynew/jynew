// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Camera Filter Pack - (c)VETASOFT 2014
Shader "CameraFilterPack/3D_Fog_Smoke" {
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
			uniform float4 _MatrixColor;
			uniform float _DepthLevel;
			uniform float _FarCamera;
			uniform sampler2D _CameraDepthTexture;
			uniform float _FixDistance;
			uniform float _LightIntensity;
			uniform sampler2D _MainTex2;
			uniform float _MatrixSize;
			uniform float _MatrixSpeed;
		
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
		uv.y=1-uv.y;
		
	    float depth = LinearEyeDepth  (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
		
		depth/=_FixDistance*10;
		depth = 1-depth;
        depth = saturate(depth);
		depth = lerp(0.5,depth,_DepthLevel);
		
		if (_Visualize == 1) return depth;
		
		float t=_Time*_MatrixSpeed;
		uv=uvst.xy;
		uv/=depth+0.2;
		uv.y+=t;
		uv*=_MatrixSize;
		float4 mx = tex2D(_MainTex2,uv).r;
		mx -=1-_MatrixColor;
		
		uv.y+=t*0.5;
		uv *=0.8*_MatrixSize; 
		mx += tex2D(_MainTex2,uv).r;
		uv.y+=t*0.5;
		uv *=0.8*_MatrixSize; 
		mx += tex2D(_MainTex2,uv).g;
		uv.y+=t*0.5;
		uv *=0.8*_MatrixSize;
		mx += tex2D(_MainTex2,uv).b;
		
		
		uv = uvst.xy;
		float4 txt = tex2D(_MainTex,uv);
		mx+=txt+depth*0.25*_LightIntensity;
		
		mx=lerp(txt,mx,_DepthLevel);
		return mx;
}
			
			ENDCG
		}
		
	}
}