// Upgrade NOTE: upgraded instancing buffer 'MTEStandardExperimentalPacked8Textures' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MTE/Standard/Experimental/Packed/8Textures"
{
	Properties
	{
		_PackedSplat0("PackedSplat0", 2D) = "white" {}
		_PackedSplat3("PackedSplat3", 2D) = "white" {}
		_PackedSplat1("PackedSplat1", 2D) = "white" {}
		_PackedSplat4("PackedSplat4", 2D) = "white" {}
		_PackedSplat5("PackedSplat5", 2D) = "white" {}
		_PackedSplat2("PackedSplat2", 2D) = "white" {}
		_PackedHeightMap("PackedHeightMap", 2D) = "white" {}
		_PackedHeightMapExtra("PackedHeightMapExtra", 2D) = "white" {}
		_Control("Control", 2D) = "white" {}
		_ControlExtra("ControlExtra", 2D) = "white" {}
		_Metallic("Metallic", Vector) = (0,0,0,0)
		_MetallicExtra("MetallicExtra", Vector) = (0,0,0,0)
		_Smoothness("Smoothness", Vector) = (0,0,0,0)
		_SmoothnessExtra("SmoothnessExtra", Vector) = (0,0,0,0)
		_HeightWeightExtra("HeightWeightExtra", Vector) = (0.3,0.3,0.3,0.3)
		_HeightWeight("HeightWeight", Vector) = (0.3,0.3,0.3,0.3)
		_NormalStrength("NormalStrength", Vector) = (-1,-1,-1,-1)
		_NormalStrengthExtra("NormalStrengthExtra", Vector) = (-1,-1,-1,-1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Control;
		uniform sampler2D _PackedHeightMap;
		uniform float4 _PackedHeightMap_ST;
		float4 _PackedHeightMap_TexelSize;
		uniform float4 _NormalStrength;
		uniform sampler2D _ControlExtra;
		uniform sampler2D _PackedHeightMapExtra;
		uniform float4 _PackedHeightMapExtra_ST;
		float4 _PackedHeightMapExtra_TexelSize;
		uniform float4 _NormalStrengthExtra;
		uniform sampler2D _PackedSplat0;
		uniform sampler2D _PackedSplat1;
		uniform sampler2D _PackedSplat2;
		uniform sampler2D _PackedSplat3;
		uniform sampler2D _PackedSplat4;
		uniform sampler2D _PackedSplat5;

		UNITY_INSTANCING_BUFFER_START(MTEStandardExperimentalPacked8Textures)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Control_ST)
#define _Control_ST_arr MTEStandardExperimentalPacked8Textures
			UNITY_DEFINE_INSTANCED_PROP(float4, _HeightWeight)
#define _HeightWeight_arr MTEStandardExperimentalPacked8Textures
			UNITY_DEFINE_INSTANCED_PROP(float4, _ControlExtra_ST)
#define _ControlExtra_ST_arr MTEStandardExperimentalPacked8Textures
			UNITY_DEFINE_INSTANCED_PROP(float4, _HeightWeightExtra)
#define _HeightWeightExtra_arr MTEStandardExperimentalPacked8Textures
			UNITY_DEFINE_INSTANCED_PROP(float4, _Metallic)
#define _Metallic_arr MTEStandardExperimentalPacked8Textures
			UNITY_DEFINE_INSTANCED_PROP(float4, _MetallicExtra)
#define _MetallicExtra_arr MTEStandardExperimentalPacked8Textures
			UNITY_DEFINE_INSTANCED_PROP(float4, _Smoothness)
#define _Smoothness_arr MTEStandardExperimentalPacked8Textures
			UNITY_DEFINE_INSTANCED_PROP(float4, _SmoothnessExtra)
#define _SmoothnessExtra_arr MTEStandardExperimentalPacked8Textures
		UNITY_INSTANCING_BUFFER_END(MTEStandardExperimentalPacked8Textures)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _Control_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Control_ST_arr, _Control_ST);
			float2 uv_Control = i.uv_texcoord * _Control_ST_Instance.xy + _Control_ST_Instance.zw;
			float4 tex2DNode256 = tex2D( _Control, uv_Control );
			float4 _HeightWeight_Instance = UNITY_ACCESS_INSTANCED_PROP(_HeightWeight_arr, _HeightWeight);
			float2 uv_PackedHeightMap = i.uv_texcoord * _PackedHeightMap_ST.xy + _PackedHeightMap_ST.zw;
			float4 temp_output_258_0 = ( tex2DNode256 * tex2D( _PackedHeightMap, uv_PackedHeightMap ) );
			float4 break259 = temp_output_258_0;
			float temp_output_262_0 = max( max( max( break259.r , break259.g ) , break259.b ) , break259.a );
			float4 appendResult263 = (float4(temp_output_262_0 , temp_output_262_0 , temp_output_262_0 , temp_output_262_0));
			float4 temp_output_268_0 = ( tex2DNode256 * max( ( _HeightWeight_Instance + ( temp_output_258_0 - appendResult263 ) ) , float4( 0,0,0,0 ) ) );
			float dotResult271 = dot( temp_output_268_0 , half4(1,1,1,1) );
			float4 temp_output_277_0 = ( temp_output_268_0 / max( dotResult271 , 0.0 ) );
			float2 temp_output_94_0_g259 = uv_PackedHeightMap;
			float2 appendResult99_g259 = (float2(_PackedHeightMap_TexelSize.x , 0.0));
			float4 tex2DNode97_g259 = tex2D( _PackedHeightMap, ( temp_output_94_0_g259 + appendResult99_g259 ) );
			float4 tex2DNode100_g259 = tex2D( _PackedHeightMap, temp_output_94_0_g259 );
			float temp_output_21_0_g260 = tex2DNode100_g259.r;
			float temp_output_128_0_g259 = _NormalStrength.x;
			float temp_output_22_0_g260 = temp_output_128_0_g259;
			float3 appendResult11_g260 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g259.r - temp_output_21_0_g260 ) * temp_output_22_0_g260 )));
			float2 appendResult101_g259 = (float2(0.0 , _PackedHeightMap_TexelSize.y));
			float4 tex2DNode98_g259 = tex2D( _PackedHeightMap, ( temp_output_94_0_g259 + appendResult101_g259 ) );
			float3 appendResult19_g260 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g259.r - temp_output_21_0_g260 ) * temp_output_22_0_g260 )));
			float3 normalizeResult15_g260 = normalize( cross( appendResult11_g260 , appendResult19_g260 ) );
			float temp_output_21_0_g263 = tex2DNode100_g259.g;
			float temp_output_22_0_g263 = temp_output_128_0_g259;
			float3 appendResult11_g263 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g259.g - temp_output_21_0_g263 ) * temp_output_22_0_g263 )));
			float3 appendResult19_g263 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g259.g - temp_output_21_0_g263 ) * temp_output_22_0_g263 )));
			float3 normalizeResult15_g263 = normalize( cross( appendResult11_g263 , appendResult19_g263 ) );
			float temp_output_21_0_g261 = tex2DNode100_g259.b;
			float temp_output_22_0_g261 = temp_output_128_0_g259;
			float3 appendResult11_g261 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g259.b - temp_output_21_0_g261 ) * temp_output_22_0_g261 )));
			float3 appendResult19_g261 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g259.b - temp_output_21_0_g261 ) * temp_output_22_0_g261 )));
			float3 normalizeResult15_g261 = normalize( cross( appendResult11_g261 , appendResult19_g261 ) );
			float temp_output_21_0_g262 = tex2DNode100_g259.a;
			float temp_output_22_0_g262 = temp_output_128_0_g259;
			float3 appendResult11_g262 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g259.a - temp_output_21_0_g262 ) * temp_output_22_0_g262 )));
			float3 appendResult19_g262 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g259.a - temp_output_21_0_g262 ) * temp_output_22_0_g262 )));
			float3 normalizeResult15_g262 = normalize( cross( appendResult11_g262 , appendResult19_g262 ) );
			float4 weightedBlendVar287 = temp_output_277_0;
			float3 weightedAvg287 = ( ( weightedBlendVar287.x*( ( normalizeResult15_g260 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) + weightedBlendVar287.y*( ( normalizeResult15_g263 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) + weightedBlendVar287.z*( ( normalizeResult15_g261 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) + weightedBlendVar287.w*( ( normalizeResult15_g262 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) )/( weightedBlendVar287.x + weightedBlendVar287.y + weightedBlendVar287.z + weightedBlendVar287.w ) );
			float4 _ControlExtra_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_ControlExtra_ST_arr, _ControlExtra_ST);
			float2 uv_ControlExtra = i.uv_texcoord * _ControlExtra_ST_Instance.xy + _ControlExtra_ST_Instance.zw;
			float4 tex2DNode291 = tex2D( _ControlExtra, uv_ControlExtra );
			float4 _HeightWeightExtra_Instance = UNITY_ACCESS_INSTANCED_PROP(_HeightWeightExtra_arr, _HeightWeightExtra);
			float2 uv_PackedHeightMapExtra = i.uv_texcoord * _PackedHeightMapExtra_ST.xy + _PackedHeightMapExtra_ST.zw;
			float4 temp_output_293_0 = ( tex2DNode291 * tex2D( _PackedHeightMapExtra, uv_PackedHeightMapExtra ) );
			float4 break294 = temp_output_293_0;
			float temp_output_297_0 = max( max( max( break294.r , break294.g ) , break294.b ) , break294.a );
			float4 appendResult298 = (float4(temp_output_297_0 , temp_output_297_0 , temp_output_297_0 , temp_output_297_0));
			float4 temp_output_303_0 = ( tex2DNode291 * max( ( _HeightWeightExtra_Instance + ( temp_output_293_0 - appendResult298 ) ) , float4( 0,0,0,0 ) ) );
			float dotResult306 = dot( temp_output_303_0 , half4(1,1,1,1) );
			float4 temp_output_312_0 = ( temp_output_303_0 / max( dotResult306 , 0.0 ) );
			float2 temp_output_94_0_g254 = uv_PackedHeightMapExtra;
			float2 appendResult99_g254 = (float2(_PackedHeightMapExtra_TexelSize.x , 0.0));
			float4 tex2DNode97_g254 = tex2D( _PackedHeightMapExtra, ( temp_output_94_0_g254 + appendResult99_g254 ) );
			float4 tex2DNode100_g254 = tex2D( _PackedHeightMapExtra, temp_output_94_0_g254 );
			float temp_output_21_0_g255 = tex2DNode100_g254.r;
			float temp_output_128_0_g254 = _NormalStrengthExtra.x;
			float temp_output_22_0_g255 = temp_output_128_0_g254;
			float3 appendResult11_g255 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g254.r - temp_output_21_0_g255 ) * temp_output_22_0_g255 )));
			float2 appendResult101_g254 = (float2(0.0 , _PackedHeightMapExtra_TexelSize.y));
			float4 tex2DNode98_g254 = tex2D( _PackedHeightMapExtra, ( temp_output_94_0_g254 + appendResult101_g254 ) );
			float3 appendResult19_g255 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g254.r - temp_output_21_0_g255 ) * temp_output_22_0_g255 )));
			float3 normalizeResult15_g255 = normalize( cross( appendResult11_g255 , appendResult19_g255 ) );
			float temp_output_21_0_g258 = tex2DNode100_g254.g;
			float temp_output_22_0_g258 = temp_output_128_0_g254;
			float3 appendResult11_g258 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g254.g - temp_output_21_0_g258 ) * temp_output_22_0_g258 )));
			float3 appendResult19_g258 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g254.g - temp_output_21_0_g258 ) * temp_output_22_0_g258 )));
			float3 normalizeResult15_g258 = normalize( cross( appendResult11_g258 , appendResult19_g258 ) );
			float temp_output_21_0_g256 = tex2DNode100_g254.b;
			float temp_output_22_0_g256 = temp_output_128_0_g254;
			float3 appendResult11_g256 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g254.b - temp_output_21_0_g256 ) * temp_output_22_0_g256 )));
			float3 appendResult19_g256 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g254.b - temp_output_21_0_g256 ) * temp_output_22_0_g256 )));
			float3 normalizeResult15_g256 = normalize( cross( appendResult11_g256 , appendResult19_g256 ) );
			float temp_output_21_0_g257 = tex2DNode100_g254.a;
			float temp_output_22_0_g257 = temp_output_128_0_g254;
			float3 appendResult11_g257 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g254.a - temp_output_21_0_g257 ) * temp_output_22_0_g257 )));
			float3 appendResult19_g257 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g254.a - temp_output_21_0_g257 ) * temp_output_22_0_g257 )));
			float3 normalizeResult15_g257 = normalize( cross( appendResult11_g257 , appendResult19_g257 ) );
			float4 weightedBlendVar322 = temp_output_312_0;
			float3 weightedAvg322 = ( ( weightedBlendVar322.x*( ( normalizeResult15_g255 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) + weightedBlendVar322.y*( ( normalizeResult15_g258 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) + weightedBlendVar322.z*( ( normalizeResult15_g256 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) + weightedBlendVar322.w*( ( normalizeResult15_g257 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) )/( weightedBlendVar322.x + weightedBlendVar322.y + weightedBlendVar322.z + weightedBlendVar322.w ) );
			o.Normal = ( weightedAvg287 + weightedAvg322 );
			float4 tex2DNode272 = tex2D( _PackedSplat0, uv_PackedHeightMap );
			float3 appendResult279 = (float3(tex2DNode272.r , tex2DNode272.g , tex2DNode272.b));
			float4 tex2DNode270 = tex2D( _PackedSplat1, uv_PackedHeightMap );
			float3 appendResult282 = (float3(tex2DNode270.r , tex2DNode270.g , tex2DNode270.b));
			float4 tex2DNode273 = tex2D( _PackedSplat2, uv_PackedHeightMap );
			float3 appendResult284 = (float3(tex2DNode273.r , tex2DNode273.g , tex2DNode273.b));
			float3 appendResult274 = (float3(tex2DNode272.a , tex2DNode270.a , tex2DNode273.a));
			half3 gammaToLinear281 = appendResult274;
			gammaToLinear281 = half3( GammaToLinearSpaceExact(gammaToLinear281.r), GammaToLinearSpaceExact(gammaToLinear281.g), GammaToLinearSpaceExact(gammaToLinear281.b) );
			float4 weightedBlendVar285 = temp_output_277_0;
			float3 weightedAvg285 = ( ( weightedBlendVar285.x*appendResult279 + weightedBlendVar285.y*appendResult282 + weightedBlendVar285.z*appendResult284 + weightedBlendVar285.w*gammaToLinear281 )/( weightedBlendVar285.x + weightedBlendVar285.y + weightedBlendVar285.z + weightedBlendVar285.w ) );
			float4 tex2DNode307 = tex2D( _PackedSplat3, uv_PackedHeightMapExtra );
			float3 appendResult314 = (float3(tex2DNode307.r , tex2DNode307.g , tex2DNode307.b));
			float4 tex2DNode305 = tex2D( _PackedSplat4, uv_PackedHeightMapExtra );
			float3 appendResult317 = (float3(tex2DNode305.r , tex2DNode305.g , tex2DNode305.b));
			float4 tex2DNode308 = tex2D( _PackedSplat5, uv_PackedHeightMapExtra );
			float3 appendResult319 = (float3(tex2DNode308.r , tex2DNode308.g , tex2DNode308.b));
			float3 appendResult309 = (float3(tex2DNode307.a , tex2DNode305.a , tex2DNode308.a));
			half3 gammaToLinear316 = appendResult309;
			gammaToLinear316 = half3( GammaToLinearSpaceExact(gammaToLinear316.r), GammaToLinearSpaceExact(gammaToLinear316.g), GammaToLinearSpaceExact(gammaToLinear316.b) );
			float4 weightedBlendVar320 = temp_output_312_0;
			float3 weightedAvg320 = ( ( weightedBlendVar320.x*appendResult314 + weightedBlendVar320.y*appendResult317 + weightedBlendVar320.z*appendResult319 + weightedBlendVar320.w*gammaToLinear316 )/( weightedBlendVar320.x + weightedBlendVar320.y + weightedBlendVar320.z + weightedBlendVar320.w ) );
			o.Albedo = ( weightedAvg285 + weightedAvg320 );
			float4 _Metallic_Instance = UNITY_ACCESS_INSTANCED_PROP(_Metallic_arr, _Metallic);
			float dotResult286 = dot( temp_output_277_0 , _Metallic_Instance );
			float4 _MetallicExtra_Instance = UNITY_ACCESS_INSTANCED_PROP(_MetallicExtra_arr, _MetallicExtra);
			float dotResult321 = dot( temp_output_312_0 , _MetallicExtra_Instance );
			o.Metallic = ( dotResult286 + dotResult321 );
			float4 _Smoothness_Instance = UNITY_ACCESS_INSTANCED_PROP(_Smoothness_arr, _Smoothness);
			float dotResult288 = dot( temp_output_277_0 , _Smoothness_Instance );
			float4 _SmoothnessExtra_Instance = UNITY_ACCESS_INSTANCED_PROP(_SmoothnessExtra_arr, _SmoothnessExtra);
			float dotResult323 = dot( temp_output_312_0 , _SmoothnessExtra_Instance );
			o.Smoothness = ( dotResult288 + dotResult323 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "MTE.MTEPackedShaderGUI"
}
/*ASEBEGIN
Version=18927
586;134;1920;989;3539.202;51.7067;1.782257;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;254;-3125.933,350.3575;Inherit;True;Property;_PackedHeightMap;PackedHeightMap;6;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;289;-3252.193,1993.479;Inherit;True;Property;_PackedHeightMapExtra;PackedHeightMapExtra;7;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;255;-2766.753,309.8344;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;290;-2893.012,1952.956;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;257;-2498.818,104.844;Inherit;True;Property;_Normal1;Packed Heightmap;5;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;292;-2625.078,1747.965;Inherit;True;Property;_Normal2;Packed Heightmap;5;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;256;-2496.099,-175.3622;Inherit;True;Property;_Control;Control;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;291;-2622.359,1467.759;Inherit;True;Property;_ControlExtra;ControlExtra;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;293;-2092.178,1722.638;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;258;-1965.918,79.51642;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;259;-1888.299,306.8306;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;294;-2014.559,1949.952;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMaxOpNode;260;-1630.6,255.5295;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;295;-1756.86,1898.651;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;261;-1491.1,288.4292;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;296;-1617.36,1931.551;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;297;-1515.66,2032.451;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;262;-1389.4,389.3298;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;298;-1441.696,1879.33;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;263;-1315.436,236.2085;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;265;-1143.039,171.308;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;299;-1343.269,1430.981;Float;False;InstancedProperty;_HeightWeightExtra;HeightWeightExtra;14;0;Create;True;0;0;0;True;0;False;0.3,0.3,0.3,0.3;0.3,0.3,0.3,0.3;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;264;-1217.009,-212.14;Float;False;InstancedProperty;_HeightWeight;HeightWeight;15;0;Create;True;0;0;0;True;0;False;0.3,0.3,0.3,0.3;0.3,0.3,0.3,0.3;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;300;-1269.299,1814.429;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;301;-1102.927,1758.975;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;266;-976.6672,115.8533;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;302;-978.564,1823.226;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;267;-852.3042,180.1046;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;303;-863.6425,1691.339;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;268;-737.3828,48.21796;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;269;-519.7125,321.4084;Half;False;Constant;_Vector0;Vector 0;14;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;304;-645.9719,1964.53;Half;False;Constant;_Vector2;Vector 2;14;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;305;-1331.585,2336.837;Inherit;True;Property;_PackedSplat4;PackedSplat4;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;308;-1331.31,2530.846;Inherit;True;Property;_PackedSplat5;PackedSplat5;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;272;-1206.381,499.9949;Inherit;True;Property;_PackedSplat0;PackedSplat0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;306;-373.0718,1896.93;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;271;-246.8124,253.8085;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;307;-1332.641,2143.116;Inherit;True;Property;_PackedSplat3;PackedSplat3;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;270;-1205.325,693.7162;Inherit;True;Property;_PackedSplat1;PackedSplat1;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;273;-1205.05,887.7249;Inherit;True;Property;_PackedSplat2;PackedSplat2;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;275;-678.2846,1133.752;Float;False;Property;_NormalStrength;NormalStrength;16;0;Create;True;0;0;0;False;0;False;-1,-1,-1,-1;80,80,80,80;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMaxOpNode;276;-118.6356,321.7178;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;309;-916.2792,2556.065;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;311;-244.895,1964.839;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;310;-804.5443,2776.873;Float;False;Property;_NormalStrengthExtra;NormalStrengthExtra;17;0;Create;True;0;0;0;False;0;False;-1,-1,-1,-1;80,80,80,80;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;274;-790.0195,912.9438;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;314;-911.8095,2169.536;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;284;-779.0401,792.8408;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;283;0.2777071,1203.771;Inherit;False;InstancedProperty;_Metallic;Metallic;10;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;280;-379.4978,1041.629;Inherit;False;GetNormalsFromPackedHeight;-1;;259;e838e220bfb1a834390a94fca7b1804b;0;3;128;FLOAT;3;False;102;SAMPLER2D;0;False;94;FLOAT2;0,0;False;4;FLOAT3;124;FLOAT3;50;FLOAT3;109;FLOAT3;110
Node;AmplifyShaderEditor.Vector4Node;278;2.134215,1424.364;Inherit;False;InstancedProperty;_Smoothness;Smoothness;12;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;318;-125.9816,2846.892;Inherit;False;InstancedProperty;_MetallicExtra;MetallicExtra;11;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GammaToLinearNode;316;-729.3514,2555.344;Inherit;False;1;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;313;-124.1252,3067.485;Inherit;False;InstancedProperty;_SmoothnessExtra;SmoothnessExtra;13;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;277;30.92413,151.4152;Inherit;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;282;-788.042,661.7494;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GammaToLinearNode;281;-603.092,912.2233;Inherit;False;1;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;279;-785.5498,526.4153;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;319;-905.2999,2435.962;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;315;-505.7572,2684.75;Inherit;False;GetNormalsFromPackedHeight;-1;;254;e838e220bfb1a834390a94fca7b1804b;0;3;128;FLOAT;3;False;102;SAMPLER2D;0;False;94;FLOAT2;0,0;False;4;FLOAT3;124;FLOAT3;50;FLOAT3;109;FLOAT3;110
Node;AmplifyShaderEditor.DynamicAppendNode;317;-914.3017,2304.87;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;312;-95.33528,1794.536;Inherit;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WeightedBlendNode;285;385.9409,352.5837;Inherit;True;5;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;288;358.3879,1377.536;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;321;316.1667,2610.92;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;286;427.2906,1114.109;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WeightedBlendNode;322;256.892,2325.401;Inherit;True;5;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;323;338.0771,2796.144;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WeightedBlendNode;320;259.6815,1995.705;Inherit;True;5;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WeightedBlendNode;287;383.1513,682.2805;Inherit;True;5;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;325;755.7846,1835.284;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;324;757.5389,1746.669;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;326;756.7846,1926.284;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;327;756.7846,2016.284;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;129;981.7219,1843.476;Float;False;True;-1;2;MTE.MTEPackedShaderGUI;0;0;Standard;MTE/Standard/Experimental/Packed/8Textures;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;255;2;254;0
WireConnection;290;2;289;0
WireConnection;257;0;254;0
WireConnection;257;1;255;0
WireConnection;292;0;289;0
WireConnection;292;1;290;0
WireConnection;293;0;291;0
WireConnection;293;1;292;0
WireConnection;258;0;256;0
WireConnection;258;1;257;0
WireConnection;259;0;258;0
WireConnection;294;0;293;0
WireConnection;260;0;259;0
WireConnection;260;1;259;1
WireConnection;295;0;294;0
WireConnection;295;1;294;1
WireConnection;261;0;260;0
WireConnection;261;1;259;2
WireConnection;296;0;295;0
WireConnection;296;1;294;2
WireConnection;297;0;296;0
WireConnection;297;1;294;3
WireConnection;262;0;261;0
WireConnection;262;1;259;3
WireConnection;298;0;297;0
WireConnection;298;1;297;0
WireConnection;298;2;297;0
WireConnection;298;3;297;0
WireConnection;263;0;262;0
WireConnection;263;1;262;0
WireConnection;263;2;262;0
WireConnection;263;3;262;0
WireConnection;265;0;258;0
WireConnection;265;1;263;0
WireConnection;300;0;293;0
WireConnection;300;1;298;0
WireConnection;301;0;299;0
WireConnection;301;1;300;0
WireConnection;266;0;264;0
WireConnection;266;1;265;0
WireConnection;302;0;301;0
WireConnection;267;0;266;0
WireConnection;303;0;291;0
WireConnection;303;1;302;0
WireConnection;268;0;256;0
WireConnection;268;1;267;0
WireConnection;305;1;290;0
WireConnection;308;1;290;0
WireConnection;272;1;255;0
WireConnection;306;0;303;0
WireConnection;306;1;304;0
WireConnection;271;0;268;0
WireConnection;271;1;269;0
WireConnection;307;1;290;0
WireConnection;270;1;255;0
WireConnection;273;1;255;0
WireConnection;276;0;271;0
WireConnection;309;0;307;4
WireConnection;309;1;305;4
WireConnection;309;2;308;4
WireConnection;311;0;306;0
WireConnection;274;0;272;4
WireConnection;274;1;270;4
WireConnection;274;2;273;4
WireConnection;314;0;307;1
WireConnection;314;1;307;2
WireConnection;314;2;307;3
WireConnection;284;0;273;1
WireConnection;284;1;273;2
WireConnection;284;2;273;3
WireConnection;280;128;275;0
WireConnection;280;102;254;0
WireConnection;280;94;255;0
WireConnection;316;0;309;0
WireConnection;277;0;268;0
WireConnection;277;1;276;0
WireConnection;282;0;270;1
WireConnection;282;1;270;2
WireConnection;282;2;270;3
WireConnection;281;0;274;0
WireConnection;279;0;272;1
WireConnection;279;1;272;2
WireConnection;279;2;272;3
WireConnection;319;0;308;1
WireConnection;319;1;308;2
WireConnection;319;2;308;3
WireConnection;315;128;310;0
WireConnection;315;102;289;0
WireConnection;315;94;290;0
WireConnection;317;0;305;1
WireConnection;317;1;305;2
WireConnection;317;2;305;3
WireConnection;312;0;303;0
WireConnection;312;1;311;0
WireConnection;285;0;277;0
WireConnection;285;1;279;0
WireConnection;285;2;282;0
WireConnection;285;3;284;0
WireConnection;285;4;281;0
WireConnection;288;0;277;0
WireConnection;288;1;278;0
WireConnection;321;0;312;0
WireConnection;321;1;318;0
WireConnection;286;0;277;0
WireConnection;286;1;283;0
WireConnection;322;0;312;0
WireConnection;322;1;315;124
WireConnection;322;2;315;50
WireConnection;322;3;315;109
WireConnection;322;4;315;110
WireConnection;323;0;312;0
WireConnection;323;1;313;0
WireConnection;320;0;312;0
WireConnection;320;1;314;0
WireConnection;320;2;317;0
WireConnection;320;3;319;0
WireConnection;320;4;316;0
WireConnection;287;0;277;0
WireConnection;287;1;280;124
WireConnection;287;2;280;50
WireConnection;287;3;280;109
WireConnection;287;4;280;110
WireConnection;325;0;287;0
WireConnection;325;1;322;0
WireConnection;324;0;285;0
WireConnection;324;1;320;0
WireConnection;326;0;286;0
WireConnection;326;1;321;0
WireConnection;327;0;288;0
WireConnection;327;1;323;0
WireConnection;129;0;324;0
WireConnection;129;1;325;0
WireConnection;129;3;326;0
WireConnection;129;4;327;0
ASEEND*/
//CHKSM=E9B6D6F43856DDA5AD5C2DECF8FF8236640195C4