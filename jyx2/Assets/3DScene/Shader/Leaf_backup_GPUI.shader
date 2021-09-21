// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GPUInstancer/Custom/Leaf_backup"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainColor("MainColor", Color) = (1,1,1,0)
		_MainTex("MainTex", 2D) = "white" {}
		_EmissionColor("EmissionColor", Color) = (1,1,1,0)
		_EmissionTexture("EmissionTexture", 2D) = "white" {}
		_FlashScale("FlashScale", Range( 0 , 1)) = 0.5
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
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			half2 uv_texcoord;
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
		uniform float4 _MainTex_ST;
		uniform half4 _EmissionColor;
		uniform sampler2D _EmissionTexture;
		uniform float4 _EmissionTexture_ST;
		uniform float _Cutoff = 0.5;


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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float simplePerlin3D6 = snoise( ase_vertexNormal );
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float temp_output_85_0 = ( ase_worldPos.x * 0.2 );
			float4 appendResult110 = (half4(( _GlobalWind * v.color.r * ase_objectScale * _OffsetScale * _WindMain * ( ( ( cos( ( ( ( ase_vertexNormal.y + _Time.y ) * _PulseFrequency ) + temp_output_85_0 ) ) - sin( ( ase_vertexNormal.z + _Time.z + temp_output_85_0 ) ) ) * 0.45 ) + 0.55 ) ) , 0.0));
			v.vertex.xyz += ( half4( ( ase_vertexNormal * sin( ( simplePerlin3D6 * _SpeedTurbulence * v.color.b * _Time.y * 20.0 ) ) * v.color.g * _Turbulence * 0.01 * _GlobalWind ) , 0.0 ) + mul( unity_WorldToObject, appendResult110 ) ).xyz;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 appendResult77 = (half2(ase_screenPosNorm.x , ase_screenPosNorm.y));
			float2 appendResult68 = (half2(( ( appendResult77 * _FlashScale ).x + ( _FlashSpeedX * _Time.y ) ) , ( 0.0 + ( _Time.y * _FlashSpeedY ) )));
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			half4 tex2DNode3 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( ( ( _FlashIntensity * _Visibility * tex2D( _FlashTex, appendResult68 ).a ) * _TheFlashColor ) + ( _MainColor * tex2DNode3 ) ).rgb;
			float2 uv_EmissionTexture = i.uv_texcoord * _EmissionTexture_ST.xy + _EmissionTexture_ST.zw;
			o.Emission = ( _EmissionColor * tex2D( _EmissionTexture, uv_EmissionTexture ) ).rgb;
			o.Alpha = 1;
			clip( tex2DNode3.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
2189.6;4;1774;1034;4841.886;3248.698;5.183733;True;True
Node;AmplifyShaderEditor.TimeNode;121;-1858.441,1153.224;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;94;-1861.052,995.7859;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;83;-1833.624,1311.173;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;84;-1809.222,1471.196;Float;False;Constant;_Float0;Float 0;17;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;95;-1643.052,1008.786;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-1650.639,1187.856;Float;False;Property;_PulseFrequency;PulseFrequency;16;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;78;-1957.387,-980.842;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-1473.481,1039.089;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-1622.756,1416.708;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;99;-1309.716,1078.141;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;77;-1710.4,-1115.752;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-1571.518,-970.5946;Float;False;Property;_FlashScale;FlashScale;5;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;102;-1316.716,1233.141;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-1313.661,-881.4543;Float;False;Property;_FlashSpeedX;FlashSpeedX;8;0;Create;True;0;0;False;0;0.5;0;-5;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;100;-1155.716,1078.141;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1245.499,-1078.44;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1318.661,-637.4554;Float;False;Property;_FlashSpeedY;FlashSpeedY;9;0;Create;True;0;0;False;0;0;0;-5;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;59;-1513.941,-795.3239;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;103;-1168.716,1233.141;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-1008.716,1279.141;Float;False;Constant;_Float2;Float 2;18;0;Create;True;0;0;False;0;0.45;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-1008.942,-820.3236;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;64;-1099.941,-1078.323;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-1008.942,-719.3244;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;104;-1004.716,1132.141;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-819.7155,1332.141;Float;False;Constant;_Float3;Float 3;18;0;Create;True;0;0;False;0;0.55;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-772.9415,-842.3236;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;67;-779.9414,-1037.323;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;11;-942.7629,-12.9262;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-834.7155,1182.141;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-674.7366,1109.236;Float;False;Property;_WindMain;WindMain;15;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-487.71,403.7186;Float;False;Constant;_Float4;Float 4;18;0;Create;True;0;0;False;0;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;35;-690.6451,219.2309;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;56;-1559.348,-1363.407;Float;True;Property;_FlashTex;FlashTex;11;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DynamicAppendNode;68;-618.3772,-948.4811;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector3Node;89;-695.8694,964.5548;Float;False;Property;_OffsetScale;OffsetScale;17;0;Create;True;0;0;False;0;0,0,0;0.1,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NoiseGeneratorNode;6;-703.6547,58.08059;Float;False;Simplex3D;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-700.8823,144.6862;Float;False;Property;_SpeedTurbulence;SpeedTurbulence;14;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectScaleNode;82;-696.1071,824.3132;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TimeNode;120;-726.27,406.5113;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;108;-665.7155,1223.141;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;86;-698.2908,655.7939;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;81;-799.4518,575.2371;Float;False;Property;_GlobalWind;GlobalWind;12;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-411.3244,807.1459;Float;False;6;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;69;-451.3117,-975.5258;Float;True;Property;_sampleflash;sampleflash;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;71;-441.8707,-1169.27;Float;False;Property;_FlashIntensity;FlashIntensity;7;0;Create;True;0;0;False;0;0.6;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-443.5016,108.2539;Float;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-374.7694,1011.814;Float;False;Constant;_Float1;Float 1;18;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-438.7643,-1083.465;Float;False;Property;_Visibility;Visibility;10;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-450.7515,-391.9805;Float;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;False;0;None;fb4ea04431284bf4c9a518b6d03bac42;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;39;-304.8699,227.7952;Float;False;Property;_Turbulence;Turbulence;13;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-355.7515,-565.9805;Float;False;Property;_MainColor;MainColor;1;0;Create;True;0;0;False;0;1,1,1,0;0.8018868,0.8018868,0.8018868,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;74;-104.2405,-909.7203;Float;False;Property;_TheFlashColor;TheFlashColor;6;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;117;-261.71,328.7186;Float;False;Constant;_Float5;Float 5;18;0;Create;True;0;0;False;0;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;36;-305.0663,94.7501;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldToObjectMatrix;112;-247.3699,686.8147;Float;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.DynamicAppendNode;110;-199.2698,812.9138;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-51.61821,-1084.416;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;127.5528,-991.6851;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;42;-792.6224,-244.493;Float;True;Property;_EmissionTexture;EmissionTexture;4;0;Create;True;0;0;False;0;None;fb4ea04431284bf4c9a518b6d03bac42;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;41;-697.6221,-418.4937;Float;False;Property;_EmissionColor;EmissionColor;3;0;Create;True;0;0;False;0;1,1,1,0;0.1320754,0.1320754,0.1320754,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-124.3465,33.92128;Float;False;6;6;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;29.53006,734.9147;Float;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;8.248549,-328.9805;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-333.622,-181.4925;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;76;-1932.374,-1135.932;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;118;70.29004,195.7186;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;75;263.8067,-730.7221;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;251,-230;Half;False;True;2;Half;ASEMaterialInspector;1000;0;Lambert;GPUInstancer/Custom/Leaf_backup;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;1000;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;95;0;94;2
WireConnection;95;1;121;2
WireConnection;92;0;95;0
WireConnection;92;1;79;0
WireConnection;85;0;83;1
WireConnection;85;1;84;0
WireConnection;99;0;92;0
WireConnection;99;1;85;0
WireConnection;77;0;78;1
WireConnection;77;1;78;2
WireConnection;102;0;94;3
WireConnection;102;1;121;3
WireConnection;102;2;85;0
WireConnection;100;0;99;0
WireConnection;60;0;77;0
WireConnection;60;1;58;0
WireConnection;103;0;102;0
WireConnection;65;0;61;0
WireConnection;65;1;59;2
WireConnection;64;0;60;0
WireConnection;63;0;59;2
WireConnection;63;1;62;0
WireConnection;104;0;100;0
WireConnection;104;1;103;0
WireConnection;66;1;63;0
WireConnection;67;0;64;0
WireConnection;67;1;65;0
WireConnection;106;0;104;0
WireConnection;106;1;105;0
WireConnection;68;0;67;0
WireConnection;68;1;66;0
WireConnection;6;0;11;0
WireConnection;108;0;106;0
WireConnection;108;1;107;0
WireConnection;87;0;81;0
WireConnection;87;1;86;1
WireConnection;87;2;82;0
WireConnection;87;3;89;0
WireConnection;87;4;80;0
WireConnection;87;5;108;0
WireConnection;69;0;56;0
WireConnection;69;1;68;0
WireConnection;33;0;6;0
WireConnection;33;1;34;0
WireConnection;33;2;35;3
WireConnection;33;3;120;2
WireConnection;33;4;116;0
WireConnection;36;0;33;0
WireConnection;110;0;87;0
WireConnection;110;3;111;0
WireConnection;72;0;71;0
WireConnection;72;1;70;0
WireConnection;72;2;69;4
WireConnection;73;0;72;0
WireConnection;73;1;74;0
WireConnection;38;0;11;0
WireConnection;38;1;36;0
WireConnection;38;2;35;2
WireConnection;38;3;39;0
WireConnection;38;4;117;0
WireConnection;38;5;81;0
WireConnection;113;0;112;0
WireConnection;113;1;110;0
WireConnection;5;0;1;0
WireConnection;5;1;3;0
WireConnection;40;0;41;0
WireConnection;40;1;42;0
WireConnection;118;0;38;0
WireConnection;118;1;113;0
WireConnection;75;0;73;0
WireConnection;75;1;5;0
WireConnection;0;0;75;0
WireConnection;0;2;40;0
WireConnection;0;10;3;4
WireConnection;0;11;118;0
ASEEND*/
//CHKSM=F4D508DEDC0A6F7E19D8D5793690D785740483B8
