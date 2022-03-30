// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MTE/Standard/TextureArray"
{
	Properties
	{
		_Control0("Control0", 2D) = "white" {}
		_Control1("Control1", 2D) = "white" {}
		_Control2("Control2", 2D) = "white" {}
		[NoScaleOffset]_TextureArray0("TextureArray0", 2DArray) = "white" {}
		[NoScaleOffset]_TextureArray1("TextureArray1", 2DArray) = "white" {}
		_UVScaleOffset("UVScaleOffset", Vector) = (0,0,0,0)
		_NormalIntensity("Normal Intensity", Range( 0.01 , 10)) = 1
		[Toggle(ENABLE_NORMAL_INTENSITY)] ENABLE_NORMAL_INTENSITY("Normal Intensity", Float) = 1
		[Toggle(_HasWeightMap1)] _HasWeightMap1("HasWeightMap1", Float) = 0
		[Toggle(_HasWeightMap2)] _HasWeightMap2("HasWeightMap2", Float) = 0
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
		#pragma shader_feature_local ENABLE_NORMAL_INTENSITY
		#include "Assets/MeshTerrainEditor/Shaders/MTECommonPBR.hlsl"
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D_ARRAY(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D_ARRAY(tex,samplertex,coord) tex2DArray(tex,coord)
		#endif//ASE Sampling Macros

		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
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
		uniform float4 _UVScaleOffset;
		SamplerState sampler_TextureArray1;
		uniform float _NormalIntensity;
		UNITY_DECLARE_TEX2DARRAY_NOSAMPLER(_TextureArray0);
		SamplerState sampler_TextureArray0;


		float4 WeightedBlend4264( half4 Weight, float4 Layer1, float4 Layer2, float4 Layer3, float4 Layer4 )
		{
			return Layer1 * Weight.r + Layer2 * Weight.g + Layer3 * Weight.b + Layer4 * Weight.a;
		}


		inline float3 RestoreNormal266( float2 NormalXY, float Intensity )
		{
			return RestoreNormal(NormalXY, Intensity);
		}


		float4 WeightedBlend4254( half4 Weight, float4 Layer1, float4 Layer2, float4 Layer3, float4 Layer4 )
		{
			return Layer1 * Weight.r + Layer2 * Weight.g + Layer3 * Weight.b + Layer4 * Weight.a;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 localGetMax4WeightLayers245 = ( float4( 0,0,0,0 ) );
			float2 uv_Control0 = i.uv_texcoord * _Control0_ST.xy + _Control0_ST.zw;
			float4 Control0245 = tex2D( _Control0, uv_Control0 );
			float4 _Vector0 = float4(0,0,0,0);
			float2 uv_Control1 = i.uv_texcoord * _Control1_ST.xy + _Control1_ST.zw;
			#ifdef _HasWeightMap1
				float4 staticSwitch241 = tex2D( _Control1, uv_Control1 );
			#else
				float4 staticSwitch241 = _Vector0;
			#endif
			float4 Control1245 = staticSwitch241;
			float2 uv_Control2 = i.uv_texcoord * _Control2_ST.xy + _Control2_ST.zw;
			#ifdef _HasWeightMap2
				float4 staticSwitch242 = tex2D( _Control2, uv_Control2 );
			#else
				float4 staticSwitch242 = _Vector0;
			#endif
			float4 Control2245 = staticSwitch242;
			float4 Weights245 = float4( 1,0,0,0 );
			float4 Indices245 = float4( 1,0,0,0 );
			{
			Max4WeightLayer(Control0245, Control1245, Control2245, Weights245, Indices245);
			}
			float4 Weight264 = Weights245;
			float4 appendResult234 = (float4(_UVScaleOffset.x , _UVScaleOffset.y , 0.0 , 0.0));
			float4 appendResult237 = (float4(_UVScaleOffset.z , _UVScaleOffset.w , 0.0 , 0.0));
			float2 uv_TexCoord243 = i.uv_texcoord * appendResult234.xy + appendResult237.xy;
			float2 TransformedUV246 = uv_TexCoord243;
			float4 break262 = Indices245;
			float4 Layer1264 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV246,break262.x) );
			float4 Layer2264 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV246,break262.y) );
			float4 Layer3264 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV246,break262.z) );
			float4 Layer4264 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV246,break262.w) );
			float4 localWeightedBlend4264 = WeightedBlend4264( Weight264 , Layer1264 , Layer2264 , Layer3264 , Layer4264 );
			float2 NormalXY266 = (localWeightedBlend4264).yz;
			#ifdef ENABLE_NORMAL_INTENSITY
				float staticSwitch274 = _NormalIntensity;
			#else
				float staticSwitch274 = 1.0;
			#endif
			float Intensity266 = staticSwitch274;
			float3 localRestoreNormal266 = RestoreNormal266( NormalXY266 , Intensity266 );
			o.Normal = localRestoreNormal266;
			float4 Weight254 = Weights245;
			float4 break248 = Indices245;
			float4 Layer1254 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray0, sampler_TextureArray0, float3(TransformedUV246,break248.x) );
			float4 Layer2254 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray0, sampler_TextureArray0, float3(TransformedUV246,break248.y) );
			float4 Layer3254 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray0, sampler_TextureArray0, float3(TransformedUV246,break248.z) );
			float4 Layer4254 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray0, sampler_TextureArray0, float3(TransformedUV246,break248.w) );
			float4 localWeightedBlend4254 = WeightedBlend4254( Weight254 , Layer1254 , Layer2254 , Layer3254 , Layer4254 );
			o.Albedo = (localWeightedBlend4254).xyz;
			o.Smoothness = ( 1.0 - (localWeightedBlend4264).x );
			o.Occlusion = (localWeightedBlend4264).w;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "MTE.MTETextureArrayShaderGUI"
}
/*ASEBEGIN
Version=18927
-7;174;1920;845;1239.682;738.593;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;227;-821.3513,-713.6486;Inherit;False;1549.657;668.5775;;13;245;242;241;240;239;238;236;235;233;232;231;229;228;Fetch max 4 weighted layers;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;228;-794.0494,-264.8109;Float;True;Property;_Control2;Control2;2;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;229;-799.3513,-469.1873;Float;True;Property;_Control1;Control1;1;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;232;-548.1508,-403.6502;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;233;-542.8482,-199.2738;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;230;-270.9001,184.9758;Inherit;False;Property;_UVScaleOffset;UVScaleOffset;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;12.6,12.6,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;231;-797.0493,-653.6486;Float;True;Property;_Control0;Control0;0;0;Create;True;0;0;0;False;0;False;None;a1cac4206712b884fbfa27d39a00d437;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;238;-283.3751,-262.071;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;237;-48.16223,298.1642;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector4Node;236;-2.410011,-424.2649;Inherit;False;Constant;_Vector0;Vector 0;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;234;-48.729,156.3446;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;239;-288.6776,-466.4474;Inherit;True;Property;_ControlEx;ControlEx;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;235;-542.8488,-586.1115;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;241;192.1346,-494.4469;Inherit;False;Property;_HasWeightMap1;HasWeightMap1;8;0;Create;False;0;0;0;False;0;False;0;0;0;True;_HasWeightMap1;Toggle;2;Key0;Key1;Create;True;False;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;242;193.7366,-387.4635;Inherit;False;Property;_HasWeightMap2;HasWeightMap2;9;0;Create;False;0;0;0;False;0;False;0;0;0;True;_HasWeightMap2;Toggle;2;Key0;Key1;Create;True;False;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;243;103.2012,202.7012;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;240;-295.4816,-651.1327;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;245;507.1738,-530.3986;Inherit;False;Max4WeightLayer(Control0, Control1, Control2, Weights, Indices)@;7;Create;5;True;Control0;FLOAT4;1,0,0,0;In;;Inherit;False;True;Control1;FLOAT4;0,0,0,0;In;;Inherit;False;True;Control2;FLOAT4;0,0,0,0;In;;Inherit;False;True;Weights;FLOAT4;1,0,0,0;Out;;Inherit;False;True;Indices;FLOAT4;1,0,0,0;Out;;Half;False;GetMax4WeightLayers;False;False;0;;False;6;0;FLOAT4;0,0,0,0;False;1;FLOAT4;1,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;1,0,0,0;False;5;FLOAT4;1,0,0,0;False;3;FLOAT4;0;FLOAT4;5;FLOAT4;6
Node;AmplifyShaderEditor.CommentaryNode;255;807.0278,532.6627;Inherit;False;863.0828;860.0196;By default, R is smoothness, GB is normal and A is Ambient Occlusion.;7;270;269;268;262;259;258;256;Sample from second TextureArray2D;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;246;308.459,197.6686;Inherit;False;TransformedUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;262;840.7726,849.4911;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TexturePropertyNode;259;849.5729,656.7631;Inherit;True;Property;_TextureArray1;TextureArray1;4;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;None;False;white;LockedToTexture2DArray;Texture2DArray;-1;0;2;SAMPLER2DARRAY;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.CommentaryNode;244;802.7973,-385.0916;Inherit;False;872.7976;840.5895;By default, RGB is Albedo.;7;253;252;251;250;249;248;247;Sample from first TextureArray2D;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;258;859.7509,1007.657;Inherit;False;246;TransformedUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;268;1347.385,779.8341;Inherit;True;Property;_TextureSample5;Texture Sample 5;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;256;1352.531,585.6627;Inherit;True;Property;_Layer0;Layer0;3;0;Fetch;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;249;880.58,119.6512;Inherit;False;246;TransformedUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;269;1350.11,1173.997;Inherit;True;Property;_TextureSample6;Texture Sample 6;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;248;846.257,-68.26346;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TexturePropertyNode;247;845.0573,-262.2738;Inherit;True;Property;_TextureArray0;TextureArray0;3;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;None;False;white;LockedToTexture2DArray;Texture2DArray;-1;0;2;SAMPLER2DARRAY;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;270;1348.81,978.9967;Inherit;True;Property;_TextureSample7;Texture Sample 7;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;250;1351.016,-335.0916;Inherit;True;Property;_Layer0;Layer0;3;0;Fetch;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;273;2217.663,-250.6196;Inherit;False;332;155;Required by MTE shader UI. Don't Delete.;1;274;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;253;1352.87,-137.9205;Inherit;True;Property;_TextureSample2;Texture Sample 2;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;257;1926.306,-167.6959;Inherit;False;Property;_NormalIntensity;Normal Intensity;6;0;Create;True;0;0;0;False;0;False;1;1;0.01;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;252;1354.295,61.24222;Inherit;True;Property;_TextureSample3;Texture Sample 3;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;275;2028.189,-261.8654;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;251;1355.595,256.2424;Inherit;True;Property;_TextureSample4;Texture Sample 4;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;264;1958.964,-508.2785;Float;False;return Layer1 * Weight.r + Layer2 * Weight.g + Layer3 * Weight.b + Layer4 * Weight.a@;4;Create;5;True;Weight;FLOAT4;0,0,0,0;In;float[5];Half;False;True;Layer1;FLOAT4;0,0,0,0;In;;Float;False;True;Layer2;FLOAT4;0,0,0,0;In;;Float;False;True;Layer3;FLOAT4;0,0,0,0;In;;Float;False;True;Layer4;FLOAT4;0,0,0,0;In;;Float;False;Weighted Blend 4;True;False;0;;False;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;260;2151.959,-429.2699;Inherit;False;False;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CustomExpressionNode;254;1954.158,-690.9645;Float;False;return Layer1 * Weight.r + Layer2 * Weight.g + Layer3 * Weight.b + Layer4 * Weight.a@;4;Create;5;True;Weight;FLOAT4;0,0,0,0;In;float[5];Half;False;True;Layer1;FLOAT4;0,0,0,0;In;;Float;False;True;Layer2;FLOAT4;0,0,0,0;In;;Float;False;True;Layer3;FLOAT4;0,0,0,0;In;;Float;False;True;Layer4;FLOAT4;0,0,0,0;In;;Float;False;Weighted Blend 4;True;False;0;;False;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;274;2244.714,-205.1112;Inherit;False;Property;ENABLE_NORMAL_INTENSITY;Normal Intensity;7;0;Create;False;0;0;0;False;0;False;0;1;1;True;ENABLE_NORMAL_INTENSITY;Toggle;2;Key0;Key1;Create;True;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;267;2152.959,-534.2699;Inherit;False;True;False;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;261;2154.959,-344.2699;Inherit;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;265;2406.283,-527.8861;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;263;2171.997,-696.3516;Inherit;False;True;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomExpressionNode;266;2379.453,-424.3719;Inherit;False;RestoreNormal(NormalXY, Intensity);3;Create;2;True;NormalXY;FLOAT2;0,0;In;;Inherit;False;True;Intensity;FLOAT;1;In;;Inherit;False;RestoreNormal;True;False;0;;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;216;2616.188,-692.3808;Float;False;True;-1;2;MTE.MTETextureArrayShaderGUI;0;0;Standard;MTE/Standard/TextureArray;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;1;Include;;True;fa510ed2a5a3b2d4a9ef902d0fbdd6e2;Custom;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;232;2;229;0
WireConnection;233;2;228;0
WireConnection;238;0;228;0
WireConnection;238;1;233;0
WireConnection;237;0;230;3
WireConnection;237;1;230;4
WireConnection;234;0;230;1
WireConnection;234;1;230;2
WireConnection;239;0;229;0
WireConnection;239;1;232;0
WireConnection;235;2;231;0
WireConnection;241;1;236;0
WireConnection;241;0;239;0
WireConnection;242;1;236;0
WireConnection;242;0;238;0
WireConnection;243;0;234;0
WireConnection;243;1;237;0
WireConnection;240;0;231;0
WireConnection;240;1;235;0
WireConnection;245;1;240;0
WireConnection;245;2;241;0
WireConnection;245;3;242;0
WireConnection;246;0;243;0
WireConnection;262;0;245;6
WireConnection;268;0;259;0
WireConnection;268;1;258;0
WireConnection;268;6;262;1
WireConnection;256;0;259;0
WireConnection;256;1;258;0
WireConnection;256;6;262;0
WireConnection;269;0;259;0
WireConnection;269;1;258;0
WireConnection;269;6;262;3
WireConnection;248;0;245;6
WireConnection;270;0;259;0
WireConnection;270;1;258;0
WireConnection;270;6;262;2
WireConnection;250;0;247;0
WireConnection;250;1;249;0
WireConnection;250;6;248;0
WireConnection;253;0;247;0
WireConnection;253;1;249;0
WireConnection;253;6;248;1
WireConnection;252;0;247;0
WireConnection;252;1;249;0
WireConnection;252;6;248;2
WireConnection;251;0;247;0
WireConnection;251;1;249;0
WireConnection;251;6;248;3
WireConnection;264;0;245;5
WireConnection;264;1;256;0
WireConnection;264;2;268;0
WireConnection;264;3;270;0
WireConnection;264;4;269;0
WireConnection;260;0;264;0
WireConnection;254;0;245;5
WireConnection;254;1;250;0
WireConnection;254;2;253;0
WireConnection;254;3;252;0
WireConnection;254;4;251;0
WireConnection;274;1;275;0
WireConnection;274;0;257;0
WireConnection;267;0;264;0
WireConnection;261;0;264;0
WireConnection;265;0;267;0
WireConnection;263;0;254;0
WireConnection;266;0;260;0
WireConnection;266;1;274;0
WireConnection;216;0;263;0
WireConnection;216;1;266;0
WireConnection;216;4;265;0
WireConnection;216;5;261;0
ASEEND*/
//CHKSM=542A8A3B41D4FF6FF440AC922CD9CA3C32E5B36E