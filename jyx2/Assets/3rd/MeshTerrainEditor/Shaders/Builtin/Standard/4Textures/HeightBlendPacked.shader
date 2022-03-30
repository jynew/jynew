// Upgrade NOTE: upgraded instancing buffer 'MTEStandard4TexturesHeightBlendPacked' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MTE/Standard/4 Textures/Height Blend (Packed)"
{
	Properties
	{
		_PackedSplat0("PackedSplat0", 2D) = "white" {}
		_PackedSplat1("PackedSplat1", 2D) = "white" {}
		_PackedSplat2("PackedSplat2", 2D) = "white" {}
		_PackedHeightMap("PackedHeightMap", 2D) = "white" {}
		_Control("Control", 2D) = "white" {}
		_HeightWeight("HeightWeight", Vector) = (0.3,0.3,0.3,0.3)
		_NormalStrength("NormalStrength", Vector) = (-1,-1,-1,-1)
		_Metallic("Metallic", Vector) = (0,0,0,0)
		_Smoothness("Smoothness", Vector) = (0,0,0,0)
		_Tilling("Tilling", Vector) = (1,1,0,0)
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
		uniform float2 _Tilling;
		float4 _PackedHeightMap_TexelSize;
		uniform float4 _NormalStrength;
		uniform sampler2D _PackedSplat0;
		uniform sampler2D _PackedSplat1;
		uniform sampler2D _PackedSplat2;

		UNITY_INSTANCING_BUFFER_START(MTEStandard4TexturesHeightBlendPacked)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Control_ST)
#define _Control_ST_arr MTEStandard4TexturesHeightBlendPacked
			UNITY_DEFINE_INSTANCED_PROP(float4, _HeightWeight)
#define _HeightWeight_arr MTEStandard4TexturesHeightBlendPacked
			UNITY_DEFINE_INSTANCED_PROP(float4, _Metallic)
#define _Metallic_arr MTEStandard4TexturesHeightBlendPacked
			UNITY_DEFINE_INSTANCED_PROP(float4, _Smoothness)
#define _Smoothness_arr MTEStandard4TexturesHeightBlendPacked
		UNITY_INSTANCING_BUFFER_END(MTEStandard4TexturesHeightBlendPacked)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _Control_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Control_ST_arr, _Control_ST);
			float2 uv_Control = i.uv_texcoord * _Control_ST_Instance.xy + _Control_ST_Instance.zw;
			float4 tex2DNode6 = tex2D( _Control, uv_Control );
			float4 _HeightWeight_Instance = UNITY_ACCESS_INSTANCED_PROP(_HeightWeight_arr, _HeightWeight);
			float2 uv_PackedHeightMap = i.uv_texcoord * _PackedHeightMap_ST.xy + _PackedHeightMap_ST.zw;
			float2 temp_output_164_0 = ( uv_PackedHeightMap * _Tilling );
			float4 temp_output_74_0 = ( tex2DNode6 * tex2D( _PackedHeightMap, temp_output_164_0 ) );
			float4 break75 = temp_output_74_0;
			float temp_output_73_0 = max( max( max( break75.r , break75.g ) , break75.b ) , break75.a );
			float4 appendResult78 = (float4(temp_output_73_0 , temp_output_73_0 , temp_output_73_0 , temp_output_73_0));
			float4 temp_output_80_0 = ( tex2DNode6 * max( ( _HeightWeight_Instance + ( temp_output_74_0 - appendResult78 ) ) , float4( 0,0,0,0 ) ) );
			float dotResult145 = dot( temp_output_80_0 , half4(1,1,1,1) );
			float4 temp_output_81_0 = ( temp_output_80_0 / max( dotResult145 , 0.0 ) );
			float2 temp_output_94_0_g102 = temp_output_164_0;
			float2 appendResult99_g102 = (float2(_PackedHeightMap_TexelSize.x , 0.0));
			float4 tex2DNode97_g102 = tex2D( _PackedHeightMap, ( temp_output_94_0_g102 + appendResult99_g102 ) );
			float4 tex2DNode100_g102 = tex2D( _PackedHeightMap, temp_output_94_0_g102 );
			float temp_output_21_0_g103 = tex2DNode100_g102.r;
			float temp_output_128_0_g102 = _NormalStrength.x;
			float temp_output_22_0_g103 = temp_output_128_0_g102;
			float3 appendResult11_g103 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g102.r - temp_output_21_0_g103 ) * temp_output_22_0_g103 )));
			float2 appendResult101_g102 = (float2(0.0 , _PackedHeightMap_TexelSize.y));
			float4 tex2DNode98_g102 = tex2D( _PackedHeightMap, ( temp_output_94_0_g102 + appendResult101_g102 ) );
			float3 appendResult19_g103 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g102.r - temp_output_21_0_g103 ) * temp_output_22_0_g103 )));
			float3 normalizeResult15_g103 = normalize( cross( appendResult11_g103 , appendResult19_g103 ) );
			float temp_output_21_0_g106 = tex2DNode100_g102.g;
			float temp_output_22_0_g106 = temp_output_128_0_g102;
			float3 appendResult11_g106 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g102.g - temp_output_21_0_g106 ) * temp_output_22_0_g106 )));
			float3 appendResult19_g106 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g102.g - temp_output_21_0_g106 ) * temp_output_22_0_g106 )));
			float3 normalizeResult15_g106 = normalize( cross( appendResult11_g106 , appendResult19_g106 ) );
			float temp_output_21_0_g104 = tex2DNode100_g102.b;
			float temp_output_22_0_g104 = temp_output_128_0_g102;
			float3 appendResult11_g104 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g102.b - temp_output_21_0_g104 ) * temp_output_22_0_g104 )));
			float3 appendResult19_g104 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g102.b - temp_output_21_0_g104 ) * temp_output_22_0_g104 )));
			float3 normalizeResult15_g104 = normalize( cross( appendResult11_g104 , appendResult19_g104 ) );
			float temp_output_21_0_g105 = tex2DNode100_g102.a;
			float temp_output_22_0_g105 = temp_output_128_0_g102;
			float3 appendResult11_g105 = (float3(1.0 , 0.0 , ( ( tex2DNode97_g102.a - temp_output_21_0_g105 ) * temp_output_22_0_g105 )));
			float3 appendResult19_g105 = (float3(0.0 , 0.5 , ( ( tex2DNode98_g102.a - temp_output_21_0_g105 ) * temp_output_22_0_g105 )));
			float3 normalizeResult15_g105 = normalize( cross( appendResult11_g105 , appendResult19_g105 ) );
			float4 weightedBlendVar92 = temp_output_81_0;
			float3 weightedAvg92 = ( ( weightedBlendVar92.x*( ( normalizeResult15_g103 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) + weightedBlendVar92.y*( ( normalizeResult15_g106 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) + weightedBlendVar92.z*( ( normalizeResult15_g104 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) + weightedBlendVar92.w*( ( normalizeResult15_g105 * 0.5 ) + float3( 0.5,0.5,0.5 ) ) )/( weightedBlendVar92.x + weightedBlendVar92.y + weightedBlendVar92.z + weightedBlendVar92.w ) );
			o.Normal = weightedAvg92;
			float4 tex2DNode5 = tex2D( _PackedSplat0, temp_output_164_0 );
			float3 appendResult125 = (float3(tex2DNode5.r , tex2DNode5.g , tex2DNode5.b));
			float4 tex2DNode67 = tex2D( _PackedSplat1, temp_output_164_0 );
			float3 appendResult126 = (float3(tex2DNode67.r , tex2DNode67.g , tex2DNode67.b));
			float4 tex2DNode68 = tex2D( _PackedSplat2, temp_output_164_0 );
			float3 appendResult127 = (float3(tex2DNode68.r , tex2DNode68.g , tex2DNode68.b));
			float3 appendResult128 = (float3(tex2DNode5.a , tex2DNode67.a , tex2DNode68.a));
			half3 gammaToLinear152 = appendResult128;
			gammaToLinear152 = half3( GammaToLinearSpaceExact(gammaToLinear152.r), GammaToLinearSpaceExact(gammaToLinear152.g), GammaToLinearSpaceExact(gammaToLinear152.b) );
			float4 weightedBlendVar70 = temp_output_81_0;
			float3 weightedAvg70 = ( ( weightedBlendVar70.x*appendResult125 + weightedBlendVar70.y*appendResult126 + weightedBlendVar70.z*appendResult127 + weightedBlendVar70.w*gammaToLinear152 )/( weightedBlendVar70.x + weightedBlendVar70.y + weightedBlendVar70.z + weightedBlendVar70.w ) );
			o.Albedo = weightedAvg70;
			float4 _Metallic_Instance = UNITY_ACCESS_INSTANCED_PROP(_Metallic_arr, _Metallic);
			float dotResult141 = dot( temp_output_81_0 , _Metallic_Instance );
			o.Metallic = dotResult141;
			float4 _Smoothness_Instance = UNITY_ACCESS_INSTANCED_PROP(_Smoothness_arr, _Smoothness);
			float dotResult140 = dot( temp_output_81_0 , _Smoothness_Instance );
			o.Smoothness = dotResult140;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "MTE.MTEPackedShaderGUI"
}
/*ASEBEGIN
Version=18927
-7;18;1920;1001;3137.778;842.3022;1.3;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;83;-2633.47,-255.8694;Inherit;True;Property;_PackedHeightMap;PackedHeightMap;3;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;85;-2285.29,-277.3926;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;165;-2259.041,-143.5288;Inherit;False;Property;_Tilling;Tilling;9;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;-2000.041,-257.5288;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;6;-1970.636,-759.5891;Inherit;True;Property;_Control;Control;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-1973.355,-479.3829;Inherit;True;Property;_Normal0;Packed Heightmap;5;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1440.455,-504.7105;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;75;-1362.836,-277.3963;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMaxOpNode;71;-1105.137,-328.6974;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;72;-965.637,-295.7977;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;73;-863.937,-194.8971;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;78;-789.9724,-348.0184;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector4Node;90;-691.5453,-796.3668;Float;False;InstancedProperty;_HeightWeight;HeightWeight;5;0;Create;True;0;0;0;True;0;False;0.3,0.3,0.3,0.3;0.3,0.3,0.3,0.3;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;77;-617.5756,-412.9189;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;89;-451.204,-468.3737;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;76;-326.841,-404.1223;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-211.9196,-536.009;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;144;5.750707,-262.8185;Half;False;Constant;_Vector0;Vector 0;14;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;68;-679.5864,303.4979;Inherit;True;Property;_PackedSplat2;PackedSplat2;2;0;Create;True;0;0;0;False;0;False;68;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-680.9175,-84.2321;Inherit;True;Property;_PackedSplat0;PackedSplat0;0;0;Create;True;0;0;0;False;0;False;67;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;67;-679.8618,109.4893;Inherit;True;Property;_PackedSplat1;PackedSplat1;1;0;Create;True;0;0;0;False;0;False;67;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;145;278.6508,-330.4185;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;120;406.8276,-262.5091;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;128;-264.5563,328.7169;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;149;-152.8214,549.5253;Float;False;Property;_NormalStrength;NormalStrength;6;0;Create;True;0;0;0;False;0;False;-1,-1,-1,-1;80,80,80,80;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;126;-262.5788,77.52241;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;127;-253.5769,208.6139;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GammaToLinearNode;152;-77.62876,327.9963;Inherit;False;1;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;155;525.7409,619.5445;Inherit;False;InstancedProperty;_Metallic;Metallic;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;156;527.5974,840.1368;Inherit;False;InstancedProperty;_Smoothness;Smoothness;8;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;125;-260.0866,-57.8117;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;81;556.3873,-432.8118;Inherit;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;154;145.9654,457.4018;Inherit;False;GetNormalsFromPackedHeight;-1;;102;e838e220bfb1a834390a94fca7b1804b;0;3;128;FLOAT;3;False;102;SAMPLER2D;0;False;94;FLOAT2;0,0;False;4;FLOAT3;124;FLOAT3;50;FLOAT3;109;FLOAT3;110
Node;AmplifyShaderEditor.WeightedBlendNode;70;911.4041,-231.6432;Inherit;True;5;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WeightedBlendNode;92;908.6145,98.05365;Inherit;True;5;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;141;952.7538,529.8822;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;140;883.8511,793.309;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;166;1258.334,-173.4329;Float;False;True;-1;2;MTE.MTEPackedShaderGUI;0;0;Standard;MTE/Standard/4 Textures/Height Blend (Packed);False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;85;2;83;0
WireConnection;164;0;85;0
WireConnection;164;1;165;0
WireConnection;16;0;83;0
WireConnection;16;1;164;0
WireConnection;74;0;6;0
WireConnection;74;1;16;0
WireConnection;75;0;74;0
WireConnection;71;0;75;0
WireConnection;71;1;75;1
WireConnection;72;0;71;0
WireConnection;72;1;75;2
WireConnection;73;0;72;0
WireConnection;73;1;75;3
WireConnection;78;0;73;0
WireConnection;78;1;73;0
WireConnection;78;2;73;0
WireConnection;78;3;73;0
WireConnection;77;0;74;0
WireConnection;77;1;78;0
WireConnection;89;0;90;0
WireConnection;89;1;77;0
WireConnection;76;0;89;0
WireConnection;80;0;6;0
WireConnection;80;1;76;0
WireConnection;68;1;164;0
WireConnection;5;1;164;0
WireConnection;67;1;164;0
WireConnection;145;0;80;0
WireConnection;145;1;144;0
WireConnection;120;0;145;0
WireConnection;128;0;5;4
WireConnection;128;1;67;4
WireConnection;128;2;68;4
WireConnection;126;0;67;1
WireConnection;126;1;67;2
WireConnection;126;2;67;3
WireConnection;127;0;68;1
WireConnection;127;1;68;2
WireConnection;127;2;68;3
WireConnection;152;0;128;0
WireConnection;125;0;5;1
WireConnection;125;1;5;2
WireConnection;125;2;5;3
WireConnection;81;0;80;0
WireConnection;81;1;120;0
WireConnection;154;128;149;0
WireConnection;154;102;83;0
WireConnection;154;94;164;0
WireConnection;70;0;81;0
WireConnection;70;1;125;0
WireConnection;70;2;126;0
WireConnection;70;3;127;0
WireConnection;70;4;152;0
WireConnection;92;0;81;0
WireConnection;92;1;154;124
WireConnection;92;2;154;50
WireConnection;92;3;154;109
WireConnection;92;4;154;110
WireConnection;141;0;81;0
WireConnection;141;1;155;0
WireConnection;140;0;81;0
WireConnection;140;1;156;0
WireConnection;166;0;70;0
WireConnection;166;1;92;0
WireConnection;166;3;141;0
WireConnection;166;4;140;0
ASEEND*/
//CHKSM=D5DF2542856A8A5327958BD4518DF8053A6A12AD