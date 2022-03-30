// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MTE/Surface/TextureArray/BlinnPhong"
{
	Properties
	{
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_Control0("Control0", 2D) = "white" {}
		_Shininess("Shininess", Range( 0.01 , 1)) = 0
		_Control1("Control1", 2D) = "white" {}
		_Gloss("Gloss", Range( 0 , 1)) = 0.5
		_Control2("Control2", 2D) = "white" {}
		[NoScaleOffset]_TextureArray0("TextureArray0", 2DArray) = "white" {}
		[NoScaleOffset]_TextureArray1("TextureArray1", 2DArray) = "bump" {}
		_UVScaleOffset("UVScaleOffset", Vector) = (0,0,0,0)
		[Toggle(_HasWeightMap1)] _HasWeightMap1("HasWeightMap1", Float) = 1
		[Toggle(_HasWeightMap2)] _HasWeightMap2("HasWeightMap2", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _HasWeightMap1
		#pragma shader_feature_local _HasWeightMap2
		#include "Assets/MeshTerrainEditor/Shaders/MTECommonPBR.hlsl"
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D_ARRAY(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D_ARRAY(tex,samplertex,coord) tex2DArray(tex,coord)
		#endif//ASE Sampling Macros

		#pragma surface surf BlinnPhong keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Control0;
		uniform float4 _Control0_ST;
		uniform sampler2D _Control1;
		uniform float4 _Control1_ST;
		uniform sampler2D _Control2;
		uniform float4 _Control2_ST;
		UNITY_DECLARE_TEX2DARRAY_NOSAMPLER(_TextureArray1);
		uniform half4 _UVScaleOffset;
		SamplerState sampler_TextureArray1;
		UNITY_DECLARE_TEX2DARRAY_NOSAMPLER(_TextureArray0);
		SamplerState sampler_TextureArray0;
		uniform half _Shininess;
		uniform half _Gloss;


		float4 WeightedBlend4265( half4 Weight, float4 Layer1, float4 Layer2, float4 Layer3, float4 Layer4 )
		{
			return Layer1 * Weight.r + Layer2 * Weight.g + Layer3 * Weight.b + Layer4 * Weight.a;
		}


		float4 WeightedBlend4254( half4 Weight, float4 Layer1, float4 Layer2, float4 Layer3, float4 Layer4 )
		{
			return Layer1 * Weight.r + Layer2 * Weight.g + Layer3 * Weight.b + Layer4 * Weight.a;
		}


		void surf( Input i , inout SurfaceOutput o )
		{
			half4 localGetMax4WeightLayers245 = ( float4( 0,0,0,0 ) );
			float2 uv_Control0 = i.uv_texcoord * _Control0_ST.xy + _Control0_ST.zw;
			half4 Control0245 = tex2D( _Control0, uv_Control0 );
			half4 _Vector0 = half4(0,0,0,0);
			float2 uv_Control1 = i.uv_texcoord * _Control1_ST.xy + _Control1_ST.zw;
			#ifdef _HasWeightMap1
				half4 staticSwitch241 = tex2D( _Control1, uv_Control1 );
			#else
				half4 staticSwitch241 = _Vector0;
			#endif
			half4 Control1245 = staticSwitch241;
			float2 uv_Control2 = i.uv_texcoord * _Control2_ST.xy + _Control2_ST.zw;
			#ifdef _HasWeightMap2
				half4 staticSwitch242 = tex2D( _Control2, uv_Control2 );
			#else
				half4 staticSwitch242 = _Vector0;
			#endif
			half4 Control2245 = staticSwitch242;
			half4 Weights245 = float4( 1,0,0,0 );
			half4 Indices245 = float4( 1,0,0,0 );
			{
			Max4WeightLayer(Control0245, Control1245, Control2245, Weights245, Indices245);
			}
			float4 Weight265 = Indices245;
			half4 appendResult234 = (half4(_UVScaleOffset.x , _UVScaleOffset.y , 0.0 , 0.0));
			half4 appendResult237 = (half4(_UVScaleOffset.z , _UVScaleOffset.w , 0.0 , 0.0));
			float2 uv_TexCoord243 = i.uv_texcoord * appendResult234.xy + appendResult237.xy;
			half2 TransformedUV266 = uv_TexCoord243;
			half4 break260 = Indices245;
			float4 Layer1265 = float4( UnpackNormal( SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV266,break260.x) ) ) , 0.0 );
			float4 Layer2265 = float4( UnpackNormal( SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV266,break260.y) ) ) , 0.0 );
			float4 Layer3265 = float4( UnpackNormal( SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV266,break260.z) ) ) , 0.0 );
			float4 Layer4265 = float4( UnpackNormal( SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV266,break260.w) ) ) , 0.0 );
			float4 localWeightedBlend4265 = WeightedBlend4265( Weight265 , Layer1265 , Layer2265 , Layer3265 , Layer4265 );
			o.Normal = localWeightedBlend4265.xyz;
			float4 Weight254 = Weights245;
			half4 break248 = Indices245;
			float4 Layer1254 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray0, sampler_TextureArray0, float3(TransformedUV266,break248.x) );
			float4 Layer2254 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray0, sampler_TextureArray0, float3(TransformedUV266,break248.y) );
			float4 Layer3254 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray0, sampler_TextureArray0, float3(TransformedUV266,break248.z) );
			float4 Layer4254 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray0, sampler_TextureArray0, float3(TransformedUV266,break248.w) );
			float4 localWeightedBlend4254 = WeightedBlend4254( Weight254 , Layer1254 , Layer2254 , Layer3254 , Layer4254 );
			o.Albedo = localWeightedBlend4254.xyz;
			o.Specular = _Shininess;
			o.Gloss = _Gloss;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Nature/Terrain/Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18927
-7;198;1920;821;-1467.421;646.1973;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;227;-821.3513,-713.6486;Inherit;False;1549.657;668.5775;;13;245;242;241;240;239;238;236;235;233;232;231;229;228;Fetch max 4 weighted layers;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;229;-799.3513,-469.1873;Float;True;Property;_Control1;Control1;3;0;Create;True;0;0;0;False;0;False;None;82d54f2d5e16173409ed7b4cdc1f02dd;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;228;-794.0494,-264.8109;Float;True;Property;_Control2;Control2;5;0;Create;True;0;0;0;False;0;False;None;b65270a08b78d544b9a083f29cd6ef0d;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;231;-797.0493,-653.6486;Float;True;Property;_Control0;Control0;1;0;Create;True;0;0;0;False;0;False;None;6c994aeec150fba4c9cae3c391c29d76;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;232;-548.1508,-403.6502;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;233;-542.8482,-199.2738;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;230;-151.7085,188.7447;Inherit;False;Property;_UVScaleOffset;UVScaleOffset;8;0;Create;True;0;0;0;False;0;False;0,0,0,0;5,5,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;234;70.46341,160.1136;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector4Node;236;-2.410011,-424.2649;Inherit;False;Constant;_Vector0;Vector 0;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;238;-283.3751,-262.071;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;235;-542.8488,-586.1115;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;239;-288.6776,-466.4474;Inherit;True;Property;_ControlEx;ControlEx;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;237;71.03043,301.9337;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;243;260.8081,215.0065;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;242;193.7366,-387.4635;Inherit;False;Property;_HasWeightMap2;HasWeightMap2;10;0;Create;False;0;0;0;False;0;False;0;1;1;True;_HasWeightMap2;Toggle;2;Key0;Key1;Create;True;False;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;241;192.1346,-494.4469;Inherit;False;Property;_HasWeightMap1;HasWeightMap1;9;0;Create;False;0;0;0;False;0;False;0;1;1;True;_HasWeightMap1;Toggle;2;Key0;Key1;Create;True;False;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;240;-295.4816,-651.1327;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;245;507.1738,-530.3986;Inherit;False;Max4WeightLayer(Control0, Control1, Control2, Weights, Indices)@;7;Create;5;True;Control0;FLOAT4;1,0,0,0;In;;Inherit;False;True;Control1;FLOAT4;0,0,0,0;In;;Inherit;False;True;Control2;FLOAT4;0,0,0,0;In;;Inherit;False;True;Weights;FLOAT4;1,0,0,0;Out;;Inherit;False;True;Indices;FLOAT4;1,0,0,0;Out;;Half;False;GetMax4WeightLayers;False;False;0;;False;6;0;FLOAT4;0,0,0,0;False;1;FLOAT4;1,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;1,0,0,0;False;5;FLOAT4;1,0,0,0;False;3;FLOAT4;0;FLOAT4;5;FLOAT4;6
Node;AmplifyShaderEditor.CommentaryNode;257;822.0245,614.5428;Inherit;False;1016.435;880.2737;Normal;7;264;263;262;261;259;258;260;Sample from second TextureArray2D;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;244;802.7973,-385.0916;Inherit;False;1034.99;911.0148;Color;7;252;253;250;251;248;247;267;Sample from first TextureArray2D;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;266;475.93,208.876;Inherit;False;TransformedUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;247;845.0573,-262.2738;Inherit;True;Property;_TextureArray0;TextureArray0;6;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;83bf8c0c79dd17741ba3ecafc0843724;False;white;LockedToTexture2DArray;Texture2DArray;-1;0;2;SAMPLER2DARRAY;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.BreakToComponentsNode;248;846.257,-68.26346;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TexturePropertyNode;259;864.5693,738.6432;Inherit;True;Property;_TextureArray1;TextureArray1;7;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;707a22c11e49cfe46bf2fad4d562376c;False;bump;LockedToTexture2DArray;Texture2DArray;-1;0;2;SAMPLER2DARRAY;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;258;874.7474,1089.538;Inherit;False;266;TransformedUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;260;855.7691,931.3712;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;267;866.2388,105.5223;Inherit;False;266;TransformedUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;263;1486.159,658.8625;Inherit;True;Property;_Layer0;Layer0;3;0;Fetch;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;251;1504.983,254.1083;Inherit;True;Property;_TextureSample4;Texture Sample 4;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;264;1483.738,1247.197;Inherit;True;Property;_TextureSample7;Texture Sample 7;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;253;1502.258,-140.0546;Inherit;True;Property;_TextureSample2;Texture Sample 2;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;261;1481.013,853.0339;Inherit;True;Property;_TextureSample5;Texture Sample 5;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;262;1482.438,1052.197;Inherit;True;Property;_TextureSample8;Texture Sample 8;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;277;2026.872,-138.9338;Inherit;False;523.9191;225.3525;Specular;3;275;276;272;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;250;1500.404,-337.2257;Inherit;True;Property;_Layer0;Layer0;3;0;Fetch;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;252;1503.683,59.10812;Inherit;True;Property;_TextureSample3;Texture Sample 3;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;276;2253.637,-83.93378;Inherit;False;Property;_Shininess;Shininess;2;0;Create;True;0;0;0;False;0;False;0;0.5;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;254;2010.555,-501.1415;Float;False;return Layer1 * Weight.r + Layer2 * Weight.g + Layer3 * Weight.b + Layer4 * Weight.a@;4;Create;5;True;Weight;FLOAT4;0,0,0,0;In;float[5];Half;False;True;Layer1;FLOAT4;0,0,0,0;In;;Float;False;True;Layer2;FLOAT4;0,0,0,0;In;;Float;False;True;Layer3;FLOAT4;0,0,0,0;In;;Float;False;True;Layer4;FLOAT4;0,0,0,0;In;;Float;False;Weighted Blend 4;True;False;0;;False;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CustomExpressionNode;265;2016.264,-330.909;Float;False;return Layer1 * Weight.r + Layer2 * Weight.g + Layer3 * Weight.b + Layer4 * Weight.a@;4;Create;5;True;Weight;FLOAT4;0,0,0,0;In;float[5];Half;False;True;Layer1;FLOAT4;0,0,0,0;In;;Float;False;True;Layer2;FLOAT4;0,0,0,0;In;;Float;False;True;Layer3;FLOAT4;0,0,0,0;In;;Float;False;True;Layer4;FLOAT4;0,0,0,0;In;;Float;False;Weighted Blend 4;True;False;0;;False;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector3Node;272;2051.872,-84.5813;Inherit;False;Property;_SpecColor;SpecColor;0;1;[HideInInspector];Fetch;True;0;0;0;False;0;False;0.7843137,0.7843137,0.7843137;0.9433962,0.6630473,0.6630473;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;275;2254.791,-10.53986;Inherit;False;Property;_Gloss;Gloss;4;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;216;2619.232,-508.9881;Half;False;True;-1;2;ASEMaterialInspector;0;0;BlinnPhong;MTE/Surface/TextureArray/BlinnPhong;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;Nature/Terrain/Diffuse;-1;-1;-1;-1;0;False;0;0;False;-1;0;0;False;-1;1;Include;;True;fa510ed2a5a3b2d4a9ef902d0fbdd6e2;Custom;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;232;2;229;0
WireConnection;233;2;228;0
WireConnection;234;0;230;1
WireConnection;234;1;230;2
WireConnection;238;0;228;0
WireConnection;238;1;233;0
WireConnection;235;2;231;0
WireConnection;239;0;229;0
WireConnection;239;1;232;0
WireConnection;237;0;230;3
WireConnection;237;1;230;4
WireConnection;243;0;234;0
WireConnection;243;1;237;0
WireConnection;242;1;236;0
WireConnection;242;0;238;0
WireConnection;241;1;236;0
WireConnection;241;0;239;0
WireConnection;240;0;231;0
WireConnection;240;1;235;0
WireConnection;245;1;240;0
WireConnection;245;2;241;0
WireConnection;245;3;242;0
WireConnection;266;0;243;0
WireConnection;248;0;245;6
WireConnection;260;0;245;6
WireConnection;263;0;259;0
WireConnection;263;1;258;0
WireConnection;263;6;260;0
WireConnection;251;0;247;0
WireConnection;251;1;267;0
WireConnection;251;6;248;3
WireConnection;264;0;259;0
WireConnection;264;1;258;0
WireConnection;264;6;260;3
WireConnection;253;0;247;0
WireConnection;253;1;267;0
WireConnection;253;6;248;1
WireConnection;261;0;259;0
WireConnection;261;1;258;0
WireConnection;261;6;260;1
WireConnection;262;0;259;0
WireConnection;262;1;258;0
WireConnection;262;6;260;2
WireConnection;250;0;247;0
WireConnection;250;1;267;0
WireConnection;250;6;248;0
WireConnection;252;0;247;0
WireConnection;252;1;267;0
WireConnection;252;6;248;2
WireConnection;254;0;245;5
WireConnection;254;1;250;0
WireConnection;254;2;253;0
WireConnection;254;3;252;0
WireConnection;254;4;251;0
WireConnection;265;0;245;6
WireConnection;265;1;263;0
WireConnection;265;2;261;0
WireConnection;265;3;262;0
WireConnection;265;4;264;0
WireConnection;216;0;254;0
WireConnection;216;1;265;0
WireConnection;216;3;276;0
WireConnection;216;4;275;0
ASEEND*/
//CHKSM=B6E262D01E2E5C47A86981BBAEC40E395C599152