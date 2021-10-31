Shader "ErbGameArt/Particles/Blend_TwoSides"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("Main Tex", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_FrontFacesColor("Front Faces Color", Color) = (0,0.2313726,1,1)
		_BackFacesColor("Back Faces Color", Color) = (0.1098039,0.4235294,1,1)
		_Emission("Emission", Float) = 2
		[Toggle(_USEFRESNEL_ON)] _UseFresnel("Use Fresnel?", Float) = 1
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		_Fresnel("Fresnel", Float) = 1
		_FresnelEmission("Fresnel Emission", Float) = 1
		[Toggle(_USECUSTOMDATA_ON)] _UseCustomData("Use Custom Data?", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent" "IsEmissive" = "true" "PreviewType"="Plane" }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		//#pragma target 3.0
		#pragma shader_feature _USEFRESNEL_ON
		#pragma shader_feature _USECUSTOMDATA_ON
		#pragma surface surf Unlit keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float3 viewDir;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 uv_tex4coord;
		};

		uniform float4 _FrontFacesColor;
		uniform float _Fresnel;
		uniform float _FresnelEmission;
		uniform float4 _FresnelColor;
		uniform float4 _BackFacesColor;
		uniform float _Emission;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _SpeedMainTexUVNoiseZW;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform sampler2D _Noise;
		uniform float4 _Noise_ST;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV95 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode95 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV95, _Fresnel ) );
			#ifdef _USEFRESNEL_ON
				float4 staticSwitch101 = ( ( _FrontFacesColor * ( 1.0 - fresnelNode95 ) ) + ( _FresnelEmission * _FresnelColor * fresnelNode95 ) );
			#else
				float4 staticSwitch101 = _FrontFacesColor;
			#endif
			float dotResult87 = dot( ase_worldNormal , i.viewDir );
			float4 lerpResult91 = lerp( staticSwitch101 , _BackFacesColor , (1.0 + (sign( dotResult87 ) - -1.0) * (0.0 - 1.0) / (1.0 - -1.0)));
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 appendResult21 = (float2(_SpeedMainTexUVNoiseZW.x , _SpeedMainTexUVNoiseZW.y));
			o.Emission = ( lerpResult91 * _Emission * i.vertexColor * tex2D( _MainTex, ( uv_MainTex + ( appendResult21 * _Time.y ) ) ) * i.vertexColor.a).rgb;
			o.Alpha = 1;
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 uv_Noise = i.uv_tex4coord;
			uv_Noise.xy = i.uv_tex4coord.xy * _Noise_ST.xy + _Noise_ST.zw;
			float2 appendResult22 = (float2(_SpeedMainTexUVNoiseZW.z , _SpeedMainTexUVNoiseZW.w));
			#ifdef _USECUSTOMDATA_ON
				float staticSwitch103 = uv_Noise.z;
			#else
				float staticSwitch103 = 1.0;
			#endif
			clip( ( tex2D( _Mask, uv_Mask ) * tex2D( _Noise, ( (uv_Noise).xy + ( _Time.y * appendResult22 ) ) ) * staticSwitch103 ).r - _Cutoff );
		}

		ENDCG
	}
}