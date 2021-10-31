Shader "Hovl/Particles/Blend_Normals"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_Emission("Emission", Float) = 2
		_Color("Color", Color) = (0.5,0.5,0.5,1)
		[MaterialToggle] _Usedepth ("Use depth?", Float ) = 0
		_Depthpower("Depth power", Float) = 1
		[MaterialToggle] _Usecenterglow("Use center glow?", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		_Opacity("Opacity", Range( 0 , 1)) = 1
		_NormalMap("Normal Map", 2D) = "white" {}
		_NormalScale("Normal Scale", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		//#pragma target 3.0
		#pragma surface surf Lambert alpha:fade keepalpha noshadow 
		#pragma multi_compile _ SOFTPARTICLES_ON
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 screenPos;
		};

		uniform float _NormalScale;
		uniform sampler2D _NormalMap;
		uniform float4 _SpeedMainTexUVNoiseZW;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Noise;
		uniform float4 _Noise_ST;
		uniform float4 _Color;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform float _Emission;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Depthpower;
		uniform fixed _Usedepth;
		uniform float _Opacity;
		uniform fixed _Usecenterglow;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 appendResult21 = (float2(_SpeedMainTexUVNoiseZW.x , _SpeedMainTexUVNoiseZW.y));
			float2 temp_output_24_0 = ( appendResult21 * _Time.y );
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _NormalMap, ( temp_output_24_0 + uv_NormalMap ) ), _NormalScale );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode13 = tex2D( _MainTex, ( temp_output_24_0 + uv_MainTex ) );
			float2 uv_Noise = i.uv_texcoord * _Noise_ST.xy + _Noise_ST.zw;
			float2 appendResult22 = (float2(_SpeedMainTexUVNoiseZW.z , _SpeedMainTexUVNoiseZW.w));
			float4 tex2DNode14 = tex2D( _Noise, ( uv_Noise + ( _Time.y * appendResult22 ) ) );
			float3 temp_output_30_0 = ( (tex2DNode13).rgb * (tex2DNode14).rgb * (_Color).rgb * (i.vertexColor).rgb );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float3 temp_output_58_0 = (tex2D( _Mask, uv_Mask )).rgb;
			float3 temp_cast_0 = ((1.0 + (0.0 - 0.0) * (0.0 - 1.0) / (1.0 - 0.0))).xxx;
			float3 clampResult38 = clamp( ( temp_output_58_0 - temp_cast_0 ) , float3( 0,0,0 ) , float3( 1,1,1 ) );
			float3 clampResult40 = clamp( ( temp_output_58_0 * clampResult38 ) , float3( 0,0,0 ) , float3( 1,1,1 ) );
			float3 staticSwitch46 = lerp(temp_output_30_0, ( temp_output_30_0 * clampResult40 ), _Usecenterglow);
			o.Albedo = staticSwitch46;
			o.Emission = ( staticSwitch46 * _Emission );
			float temp_output_60_0 = ( tex2DNode13.a * tex2DNode14.a * _Color.a * i.vertexColor.a );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth49 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth49 = abs( ( screenDepth49 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depthpower ) );
			float clampResult53 = clamp( distanceDepth49 , 0.0 , 1.0 );		
			float4 staticSwitch47 = temp_output_60_0;
			#ifdef SOFTPARTICLES_ON
				staticSwitch47 *= lerp(1, clampResult53, _Usedepth);
			#endif
			o.Alpha = ( staticSwitch47 * _Opacity );
		}

		ENDCG
	}
}