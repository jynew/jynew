// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MTE/Standard/TextureArray_DisplayNormal"
{
	Properties
	{
		_Control0("Control0", 2D) = "white" {}
		_Control1("Control1", 2D) = "white" {}
		_Control2("Control2", 2D) = "white" {}
		_TextureArray1("TextureArray1", 2DArray) = "white" {}
		_UVScaleOffset("UVScaleOffset", Vector) = (0,0,0,0)
		_NormalIntensity("Normal Intensity", Range( 0.01 , 10)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#include "Assets/MeshTerrainEditor/Shaders/MTECommonPBR.hlsl"
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D_ARRAY(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D_ARRAY(tex,samplertex,coord) tex2DArray(tex,coord)
		#endif//ASE Sampling Macros

		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
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


		float4 WeightedBlend424( half4 Weight, float4 Layer1, float4 Layer2, float4 Layer3, float4 Layer4 )
		{
			return Layer1 * Weight.r + Layer2 * Weight.g + Layer3 * Weight.b + Layer4 * Weight.a;
		}


		inline float3 RestoreNormal3( float2 NormalXY, float Intensity )
		{
			return RestoreNormal(NormalXY, Intensity);
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float localGetMax4WeightLayers11 = ( 0.0 );
			float2 uv_Control0 = i.uv_texcoord * _Control0_ST.xy + _Control0_ST.zw;
			float4 Control011 = tex2D( _Control0, uv_Control0 );
			float2 uv_Control1 = i.uv_texcoord * _Control1_ST.xy + _Control1_ST.zw;
			float4 Control111 = tex2D( _Control1, uv_Control1 );
			float2 uv_Control2 = i.uv_texcoord * _Control2_ST.xy + _Control2_ST.zw;
			float4 Control211 = tex2D( _Control2, uv_Control2 );
			float4 Weights11 = float4( 1,0,0,0 );
			float4 Indices11 = float4( 1,0,0,0 );
			{
			Max4WeightLayer(Control011, Control111, Control211, Weights11, Indices11);
			}
			float4 Weight24 = Weights11;
			float4 appendResult7 = (float4(_UVScaleOffset.x , _UVScaleOffset.y , 0.0 , 0.0));
			float4 appendResult14 = (float4(_UVScaleOffset.z , _UVScaleOffset.w , 0.0 , 0.0));
			float2 uv_TexCoord27 = i.uv_texcoord * appendResult7.xy + appendResult14.xy;
			float2 TransformedUV8 = uv_TexCoord27;
			float4 break12 = Indices11;
			float4 Layer124 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV8,break12.x) );
			float4 Layer224 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV8,break12.y) );
			float4 Layer324 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV8,break12.z) );
			float4 Layer424 = SAMPLE_TEXTURE2D_ARRAY( _TextureArray1, sampler_TextureArray1, float3(TransformedUV8,break12.w) );
			float4 localWeightedBlend424 = WeightedBlend424( Weight24 , Layer124 , Layer224 , Layer324 , Layer424 );
			float2 NormalXY3 = (localWeightedBlend424).yz;
			float Intensity3 = _NormalIntensity;
			float3 localRestoreNormal3 = RestoreNormal3( NormalXY3 , Intensity3 );
			o.Emission = (localRestoreNormal3*0.5 + 0.5);
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18927
-7;42;1920;977;2533.086;950.7214;2.114292;True;False
Node;AmplifyShaderEditor.CommentaryNode;1;-1806.953,-440.9245;Inherit;False;1213.657;671.5775;;10;28;26;23;22;20;19;18;17;16;11;Fetch max 4 weighted layers;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;22;-1754.651,-390.9245;Float;True;Property;_Control0;Control0;0;0;Create;True;0;0;0;False;0;False;None;d4a3fce0688bfaa4690f0cb15c1432c0;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;16;-1756.953,-206.4633;Float;True;Property;_Control1;Control1;1;0;Create;True;0;0;0;False;0;False;None;885bb82c6a7699943bbdd51920249b59;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;20;-1751.651,-2.086884;Float;True;Property;_Control2;Control2;2;0;Create;True;0;0;0;False;0;False;None;8535d203bfa2a5045a777b5e97de6d81;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.Vector4Node;15;-1354.976,367.8181;Inherit;False;Property;_UVScaleOffset;UVScaleOffset;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;7;-1130.976,335.8181;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-1500.451,-323.3874;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-1500.45,63.45023;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;18;-1505.753,-140.9262;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;14;-1130.976,479.8181;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;28;-1253.083,-388.4087;Inherit;True;Property;_TextureSample4;Texture Sample 4;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;27;-986.976,383.8181;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;26;-1240.977,0.6530151;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;17;-1246.279,-203.7234;Inherit;True;Property;_ControlEx;ControlEx;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;11;-847.2961,-257.6745;Inherit;False;Max4WeightLayer(Control0, Control1, Control2, Weights, Indices)@;7;Create;5;True;Control0;FLOAT4;1,0,0,0;In;;Inherit;False;True;Control1;FLOAT4;0,0,0,0;In;;Inherit;False;True;Control2;FLOAT4;0,0,0,0;In;;Inherit;False;True;Weights;FLOAT4;1,0,0,0;Out;;Half;False;True;Indices;FLOAT4;1,0,0,0;Out;;Half;False;GetMax4WeightLayers;False;False;0;;False;6;0;FLOAT;0;False;1;FLOAT4;1,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;1,0,0,0;False;5;FLOAT4;1,0,0,0;False;3;FLOAT;0;FLOAT4;5;FLOAT4;6
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-773.976,378.8181;Inherit;False;TransformedUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;2;-471.9545,-87.01297;Inherit;False;863.0828;860.0196;;7;25;21;12;10;9;6;5;Sample normal from TextureArray2D;1,1,1,1;0;0
Node;AmplifyShaderEditor.BreakToComponentsNode;12;-439.9545,216.987;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TexturePropertyNode;5;-439.9545,24.98703;Inherit;True;Property;_TextureArray1;TextureArray1;3;0;Create;True;0;0;0;False;0;False;None;4465edeec0229e147b0603d8cd6d48e0;False;white;LockedToTexture2DArray;Texture2DArray;-1;0;2;SAMPLER2DARRAY;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;25;-423.9545,376.987;Inherit;False;8;TransformedUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;10;72.04542,-39.01297;Inherit;True;Property;_Layer0;Layer0;3;0;Fetch;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;72.04542,360.987;Inherit;True;Property;_TextureSample2;Texture Sample 2;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;21;72.04542,552.987;Inherit;True;Property;_TextureSample6;Texture Sample 6;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;72.04542,152.987;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2DArray;8;0;SAMPLER2DARRAY;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;24;637.3619,-235.5545;Float;False;return Layer1 * Weight.r + Layer2 * Weight.g + Layer3 * Weight.b + Layer4 * Weight.a@;4;Create;5;True;Weight;FLOAT4;0,0,0,0;In;float[5];Half;False;True;Layer1;FLOAT4;0,0,0,0;In;;Float;False;True;Layer2;FLOAT4;0,0,0,0;In;;Float;False;True;Layer3;FLOAT4;0,0,0,0;In;;Float;False;True;Layer4;FLOAT4;0,0,0,0;In;;Float;False;Weighted Blend 4;True;False;0;;False;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;4;779.578,-7.895782;Inherit;False;Property;_NormalIntensity;Normal Intensity;5;0;Create;True;0;0;0;False;0;False;1;1;0.01;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;29;822.2319,-239.4698;Inherit;False;False;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CustomExpressionNode;3;1049.725,-234.5718;Inherit;False;RestoreNormal(NormalXY, Intensity);3;Create;2;True;NormalXY;FLOAT2;0,0;In;;Inherit;False;True;Intensity;FLOAT;1;In;;Inherit;False;RestoreNormal;True;False;0;;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;13;1263.353,-233.1767;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;31;1454.829,-279.3793;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;MTE/Standard/TextureArray_DisplayNormal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;1;Include;;True;fa510ed2a5a3b2d4a9ef902d0fbdd6e2;Custom;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;15;1
WireConnection;7;1;15;2
WireConnection;23;2;22;0
WireConnection;19;2;20;0
WireConnection;18;2;16;0
WireConnection;14;0;15;3
WireConnection;14;1;15;4
WireConnection;28;0;22;0
WireConnection;28;1;23;0
WireConnection;27;0;7;0
WireConnection;27;1;14;0
WireConnection;26;0;20;0
WireConnection;26;1;19;0
WireConnection;17;0;16;0
WireConnection;17;1;18;0
WireConnection;11;1;28;0
WireConnection;11;2;17;0
WireConnection;11;3;26;0
WireConnection;8;0;27;0
WireConnection;12;0;11;6
WireConnection;10;0;5;0
WireConnection;10;1;25;0
WireConnection;10;6;12;0
WireConnection;9;0;5;0
WireConnection;9;1;25;0
WireConnection;9;6;12;2
WireConnection;21;0;5;0
WireConnection;21;1;25;0
WireConnection;21;6;12;3
WireConnection;6;0;5;0
WireConnection;6;1;25;0
WireConnection;6;6;12;1
WireConnection;24;0;11;5
WireConnection;24;1;10;0
WireConnection;24;2;6;0
WireConnection;24;3;9;0
WireConnection;24;4;21;0
WireConnection;29;0;24;0
WireConnection;3;0;29;0
WireConnection;3;1;4;0
WireConnection;13;0;3;0
WireConnection;31;2;13;0
ASEEND*/
//CHKSM=A9DB26861B4DFB8CD58AAD6427FA263E92ED129D