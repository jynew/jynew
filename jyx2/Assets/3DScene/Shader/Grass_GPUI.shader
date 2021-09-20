// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GPUInstancer/Custom/Grass"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainColor("Main Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_GlobalWind("Global Wind", Range( 0 , 1)) = 0.5
		_Turbulence("Turbulence", Float) = 1
		_SpeedTurbulence("Speed Turbulence", Float) = 1
		_WindMain("Wind Main", Float) = 2
		_PulseFrequency("Pulse Frequency", Float) = 3
		_ToonEffect("Toon Effect",range(0,1)) = 0.5
		_Steps("Steps of toon",range(0,9)) = 3
		_OffsetScale("Offset Scale", Vector) = (0.1,0,0,0)
	}

	SubShader
	{
		Tags{ "Queue" ="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" "IsEmissive" = "true" }
		
		LOD 500
		Cull off
		CGPROGRAM
#include "UnityCG.cginc"
#include "./../../3rd/GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc"
#pragma instancing_options procedural:setupGPUI
#pragma multi_compile_instancing
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf Cartoon vertex:vertexDataFunc alphatest:_Cutoff addshadow noshadow
		struct Input
		{
			float2 texcoord;
			float3 worldPos;
			float4 vertexColor : COLOR;
		};
		
		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		uniform float _SpeedTurbulence;
		uniform float _Turbulence;
		uniform float _GlobalWind;
		uniform float _WindMain;
		uniform float _PulseFrequency;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _MainColor;
		uniform float4 _OffsetScale;
		float _Steps;
		float _ToonEffect;

		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}

		void vertexDataFunc(inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float simplePerlin3D16 = snoise( ase_vertexNormal );
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float temp_output_323_0 = ( ase_worldPos.x * 0.2 );
			float4 transform57 = mul(unity_WorldToObject,float4( ( _GlobalWind * v.color.r * ase_objectScale * _OffsetScale.xyz * _WindMain * (0.1 + (( cos( ( ( _PulseFrequency * (_Time.y + v.normal.y) ) + temp_output_323_0 ) ) - sin( ( _Time.z + v.normal.z + temp_output_323_0 ) ) ) - -1.0) * (1.0 - 0.1) / (1.0 - -1.0)) ) , 0.0 ));
			float4 Wind390 = ( float4( ( 0.01 * sin( ( _Time.y * ( 20.0 * simplePerlin3D16 ) * _SpeedTurbulence * v.color.b ) ) * ( _Turbulence * ase_vertexNormal * v.color.g ) * _GlobalWind ) , 0.0 ) + transform57 );
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			v.vertex.xyz += Wind390.xyz;
		}

		inline float4 LightingCartoon(SurfaceOutputStandard s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			float difLight = max(0, dot(normalize(s.Normal), normalize(lightDir)));
			difLight = (difLight + 1) / 2;//做亮化处理
			difLight = smoothstep(0, 1, difLight);//使颜色平滑的在[0,1]范围之内
			float toon = floor(difLight * _Steps) / _Steps;//把颜色做离散化处理，把diffuse颜色限制在_Steps种（_Steps阶颜色），简化颜色，这样的处理使色阶间能平滑的显示
			difLight = lerp(difLight, toon, _ToonEffect);//根据外部我们可控的卡通化程度值_ToonEffect，调节卡通与现实的比重
			float4 col;
			col.rgb = s.Albedo * _LightColor0.rgb * difLight * atten;
			col.a = s.Alpha;
			return col;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 c = tex2D( _MainTex, i.texcoord) * _MainColor;
			o.Emission = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Transparent/Cutout/VertexLit"
}
