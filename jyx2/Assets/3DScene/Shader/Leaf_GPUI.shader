Shader "GPUInstancer/Custom/Leaf"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainColor("MainColor", Color) = (1,1,1,0)
		_MainTex("MainTex", 2D) = "white" {}
		_EmissionColor("EmissionColor", Color) = (1,1,1,0)
		_EmissionTexture("EmissionTexture", 2D) = "white" {}
		[Space(40)]_FlashScale("FlashScale", Range( 0 , 1)) = 0.5
		_TheFlashColor("TheFlashColor", Color) = (0,0,0,0)
		_FlashIntensity("FlashIntensity", Range( 0 , 1)) = 0.6
		_FlashSpeedX("FlashSpeedX", Range( -5 , 5)) = 0.5
		_FlashSpeedY("FlashSpeedY", Range( -5 , 5)) = 0
		_Visibility("Visibility", Range( 0 , 1)) = 1
		_FlashTex("FlashTex", 2D) = "white" {}
		_GlobalWind("GlobalWind", Range( 0 , 1)) = 0.5
		_Turbulence("Turbulence", Float) = 1
		_SpeedTurbulence("SpeedTurbulence", Float) = 0.5
		_WindMain("WindMain", Float) = 2
		_PulseFrequency("PulseFrequency", Float) = 3
		_OffsetScale("OffsetScale", Vector) = (0,0,0,0)
		[Space(40)]
		[KeywordEnum(OFF, ON)] _DISSOLVE("Dissolve switch", Float) = 0
		_DissolveNoiseTex("DissolveNoiseTex",2D) = "while" {}
		_DissolveRatio("_DissolveRatio",Range(0,1)) = 0
		_WorkDistance("Work Distance", Float) = 5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		LOD 1000
		Cull Off
		CGPROGRAM
#include "UnityCG.cginc"
#include "./../../3rd/GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc"
#pragma instancing_options procedural:setupGPUI
#pragma multi_compile_instancing
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		#pragma multi_compile _DISSOLVE_OFF _DISSOLVE_ON

		struct Input
		{
			half2 uv_texcoord;
			half4 screenPos;
			half3 worldPos;
		};

		uniform half _SpeedTurbulence;
		uniform half _Turbulence;
		uniform half _GlobalWind;
		uniform half3 _OffsetScale;
		uniform half _WindMain;
		uniform half _PulseFrequency;
		uniform half _FlashIntensity;
		uniform half _Visibility;
		uniform sampler2D _FlashTex;
		uniform half _FlashScale;
		uniform half _FlashSpeedX;
		uniform half _FlashSpeedY;
		uniform half4 _TheFlashColor;
		uniform half4 _MainColor;
		uniform sampler2D _MainTex;
		uniform half4 _MainTex_ST;
		uniform half4 _EmissionColor;
		uniform sampler2D _EmissionTexture;
		uniform half4 _EmissionTexture_ST;
		uniform half _Cutoff = 0.5;
		uniform half _DissolveRatio;
		uniform sampler2D _DissolveNoiseTex;
		uniform half _WorkDistance;
		half4 _PlayerPos;


		half3 mod3D289(half3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		half4 mod3D289(half4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		half4 permute(half4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		half4 taylorInvSqrt(half4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		half snoise(half3 v )
		{
			const half2 C = half2( 1.0 / 6.0, 1.0 / 3.0 );
			half3 i = floor( v + dot( v, C.yyy ) );
			half3 x0 = v - i + dot( i, C.xxx );
			half3 g = step( x0.yzx, x0.xyz );
			half3 l = 1.0 - g;
			half3 i1 = min( g.xyz, l.zxy );
			half3 i2 = max( g.xyz, l.zxy );
			half3 x1 = x0 - i1 + C.xxx;
			half3 x2 = x0 - i2 + C.yyy;
			half3 x3 = x0 - 0.5;
			i = mod3D289( i);
			half4 p = permute( permute( permute( i.z + half4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + half4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + half4( 0.0, i1.x, i2.x, 1.0 ) );
			half4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			half4 x_ = floor( j / 7.0 );
			half4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			half4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			half4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			half4 h = 1.0 - abs( x ) - abs( y );
			half4 b0 = half4( x.xy, y.xy );
			half4 b1 = half4( x.zw, y.zw );
			half4 s0 = floor( b0 ) * 2.0 + 1.0;
			half4 s1 = floor( b1 ) * 2.0 + 1.0;
			half4 sh = -step( h, 0.0 );
			half4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			half4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			half3 g0 = half3( a0.xy, h.x );
			half3 g1 = half3( a0.zw, h.y );
			half3 g2 = half3( a1.xy, h.z );
			half3 g3 = half3( a1.zw, h.w );
			half4 norm = taylorInvSqrt( half4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			half4 m = max( 0.6 - half4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			half4 px = half4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			half3 ase_vertexNormal = v.normal.xyz;
			half simplePerlin3D6 = snoise( ase_vertexNormal );
			half3 ase_objectScale = half3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			half3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half temp_output_85_0 = ( ase_worldPos.x * 0.2 );
			half4 appendResult110 = (half4(( _GlobalWind * v.color.r * ase_objectScale * _OffsetScale * _WindMain * ( ( ( cos( ( ( ( ase_vertexNormal.y + _Time.y ) * _PulseFrequency ) + temp_output_85_0 ) ) - sin( ( ase_vertexNormal.z + _Time.z + temp_output_85_0 ) ) ) * 0.45 ) + 0.55 ) ) , 0.0));
			//v.vertex.xyz += ( half4( ( ase_vertexNormal * sin( ( simplePerlin3D6 * _SpeedTurbulence * v.color.b * _Time.y * 20.0 ) ) * v.color.g * _Turbulence * 0.01 * _GlobalWind ) , 0.0 ) + mul( unity_WorldToObject, appendResult110 ) ).xyz;
		}

		void surf(Input i, inout SurfaceOutput o)
		{
#if _DISSOLVE_ON
				half toCamera = distance(i.worldPos, _WorldSpaceCameraPos.xyz);
				half playerToCamera = distance(_PlayerPos.xyz, _WorldSpaceCameraPos.xyz);
				half2 wcoord = (i.screenPos.xy / i.screenPos.w);
				half gradient = tex2D(_DissolveNoiseTex, i.uv_texcoord).r;
				if (toCamera < playerToCamera)
					clip(gradient - _DissolveRatio + (toCamera - _WorkDistance) / _WorkDistance);
#endif

			half4 ase_screenPos = half4(i.screenPos.xyz, i.screenPos.w + 0.00000000001);
			half4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = (UNITY_NEAR_CLIP_VALUE >= 0) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			half2 appendResult77 = (half2(ase_screenPosNorm.x, ase_screenPosNorm.y));
			half2 appendResult68 = (half2(((appendResult77 * _FlashScale).x + (_FlashSpeedX * _Time.y)), (0.0 + (_Time.y * _FlashSpeedY))));
			half2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			half4 tex2DNode3 = tex2D(_MainTex, uv_MainTex);
			o.Albedo = (((_FlashIntensity * _Visibility * tex2D(_FlashTex, appendResult68).a) * _TheFlashColor) + (_MainColor * tex2DNode3)).rgb;
			half2 uv_EmissionTexture = i.uv_texcoord * _EmissionTexture_ST.xy + _EmissionTexture_ST.zw;
			o.Emission = (_EmissionColor * tex2D(_EmissionTexture, uv_EmissionTexture)).rgb;
			o.Alpha = 1;
			clip(tex2DNode3.a - _Cutoff);
		}

		ENDCG
	}


	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		LOD 0
		Cull Off
		CGPROGRAM
#include "UnityCG.cginc"
#include "./../../3rd/GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc"
#pragma instancing_options procedural:setupGPUI
#pragma multi_compile_instancing
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			half4 screenPos;
			half2 uv_texcoord;
		};

		uniform half _FlashIntensity;
		uniform half _Visibility;
		uniform sampler2D _FlashTex;
		uniform half _FlashScale;
		uniform half _FlashSpeedX;
		uniform half _FlashSpeedY;
		uniform half4 _TheFlashColor;
		uniform half4 _MainColor;
		uniform sampler2D _MainTex;
		uniform half4 _MainTex_ST;
		uniform half4 _EmissionColor;
		uniform sampler2D _EmissionTexture;
		uniform half4 _EmissionTexture_ST;
		uniform half _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutput o )
		{
			half4 ase_screenPos = half4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			half4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			half2 appendResult77 = (half2(ase_screenPosNorm.x , ase_screenPosNorm.y));
			half2 appendResult68 = (half2(( ( appendResult77 * _FlashScale ).x + ( _FlashSpeedX * _Time.y ) ) , ( 0.0 + ( _Time.y * _FlashSpeedY ) )));
			half2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			half4 tex2DNode3 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( ( ( _FlashIntensity * _Visibility * tex2D( _FlashTex, appendResult68 ).a ) * _TheFlashColor ) + ( _MainColor * tex2DNode3 ) ).rgb;
			half2 uv_EmissionTexture = i.uv_texcoord * _EmissionTexture_ST.xy + _EmissionTexture_ST.zw;
			o.Emission = ( _EmissionColor * tex2D( _EmissionTexture, uv_EmissionTexture ) ).rgb;
			o.Alpha = 1;
			clip( tex2DNode3.a - _Cutoff );
		}

		ENDCG
	}

	Fallback "Diffuse"
}
