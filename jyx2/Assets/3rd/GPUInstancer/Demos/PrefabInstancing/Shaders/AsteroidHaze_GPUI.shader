// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GPUInstancer/AsteroidHaze"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_HazeColor("HazeColor", Color) = (0.772549,0.7176471,0.8509804,1)
		_FadeDistance("FadeDistance", Range( 0.1 , 250)) = 100
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
#include "UnityCG.cginc"
#include "./../../../Shaders/Include/GPUInstancerInclude.cginc"
#pragma instancing_options procedural:setupGPUI
#pragma multi_compile_instancing
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow nofog nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float4 _HazeColor;
		uniform sampler2D _TextureSample0;
		uniform float _FadeDistance;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 appendResult108 = (float3(unity_CameraToWorld[ 2 ][ 0 ] , unity_CameraToWorld[ 2 ][ 1 ] , unity_CameraToWorld[ 2 ][ 2 ]));
			float3 normalizeResult110 = normalize( appendResult108 );
			float3 ReverseCameraViewVector117 = normalizeResult110;
			v.normal = ReverseCameraViewVector117;
			//Calculate new billboard vertex position and normal;
			float3 upCamVec = normalize ( UNITY_MATRIX_V._m10_m11_m12 );
			float3 forwardCamVec = -normalize ( UNITY_MATRIX_V._m20_m21_m22 );
			float3 rightCamVec = normalize( UNITY_MATRIX_V._m00_m01_m02 );
			float4x4 rotationCamMatrix = float4x4( rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1 );
			v.normal = normalize( mul( float4( v.normal , 0 ), rotationCamMatrix ));
			v.vertex.x *= length( unity_ObjectToWorld._m00_m10_m20 );
			v.vertex.y *= length( unity_ObjectToWorld._m01_m11_m21 );
			v.vertex.z *= length( unity_ObjectToWorld._m02_m12_m22 );
			v.vertex = mul( v.vertex, rotationCamMatrix );
			v.vertex.xyz += unity_ObjectToWorld._m03_m13_m23;
			//Need to nullify rotation inserted by generated surface shader;
			v.vertex = mul( unity_WorldToObject, v.vertex );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord9 = i.uv_texcoord * float2( 0.8,0.8 ) + float2( 0.1,0.1 );
			float4 transform95 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float cos6 = cos( ( ( _Time.y * 0.05 ) + radians( ( transform95.x * transform95.y * transform95.z ) ) ) );
			float sin6 = sin( ( ( _Time.y * 0.05 ) + radians( ( transform95.x * transform95.y * transform95.z ) ) ) );
			float2 rotator6 = mul( uv_TexCoord9 - float2( 0.5,0.5 ) , float2x2( cos6 , -sin6 , sin6 , cos6 )) + float2( 0.5,0.5 );
			float2 RotationOverTime53 = rotator6;
			float4 tex2DNode2 = tex2D( _TextureSample0, RotationOverTime53 );
			float4 temp_output_4_0 = ( _HazeColor * tex2DNode2 );
			o.Albedo = temp_output_4_0.rgb;
			o.Emission = temp_output_4_0.rgb;
			float3 ase_worldPos = i.worldPos;
			float clampResult35 = clamp( distance( ase_worldPos , _WorldSpaceCameraPos ) , 0 , _FadeDistance );
			float DistanceFade41 = (0 + (clampResult35 - 0) * (1 - 0) / (_FadeDistance - 0));
			o.Alpha = ( tex2DNode2.a * DistanceFade41 );
		}

		ENDCG
	}
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
137;194;1959;872;3985.258;1684.966;3.190953;True;False
Node;AmplifyShaderEditor.CommentaryNode;98;-2780.3,-940.6519;Float;False;1448.903;779.0632;Rotation over time (Pseudo random with object world position);13;44;46;92;45;67;97;9;7;6;53;95;121;122;Rotation over time;1,1,1,1;0;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;95;-2716.934,-365.8854;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;44;-2730.3,-618.7479;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;43;-2498.185,-29.99123;Float;False;1165.296;540.3787;Fade based on camera distance;7;32;33;31;36;35;37;41;Distance Fade;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-2465.812,-373.2414;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-2704.47,-457.4828;Float;False;Constant;_RotationSpeed;RotationSpeed;2;0;Create;True;0;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;67;-2284.65,-411.6772;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;121;-2630.754,-899.3826;Float;False;Constant;_Vector1;Vector 1;3;0;Create;True;0;0.8,0.8;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;119;-2486.914,975.0244;Float;False;1268.679;481.5909;Reverse camera view vector for normal calculation;5;117;114;116;110;108;Reverse Camera View Vector;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-2309.413,-595.4139;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;32;-2448.185,187.6077;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;33;-2410.298,20.00874;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;122;-2641.754,-777.3826;Float;False;Constant;_Vector2;Vector 2;3;0;Create;True;0;0.1,0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DistanceOpNode;31;-2132.686,138.264;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-2388.475,-890.6519;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.5,0.5;False;1;FLOAT2;0.25,0.25;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;7;-2343.131,-768.8588;Float;False;Constant;_Vector0;Vector 0;2;0;Create;True;0;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;36;-2337.257,395.3869;Float;False;Property;_FadeDistance;FadeDistance;2;0;Create;True;0;100;250;0.1;250;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;97;-2100.24,-459.4821;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CameraToWorldMatrix;116;-2436.915,1025.024;Float;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.ClampOpNode;35;-1964.097,203.7068;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;6;-1911.925,-665.8408;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;114;-2185.896,1026.87;Float;False;FLOAT4x4;1;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;54;-863.9086,-8.14459;Float;False;53;0;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;37;-1803.265,223.5188;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;108;-1884.542,1211.246;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;-1603.397,-671.7018;Float;False;RotationOverTime;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;41;-1575.889,218.0198;Float;False;DistanceFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-570.044,-30.29569;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;ec89eb58be74afb47a32870060361505;e7fab2a279474df4c8f2190089924d56;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;99;-581.0316,-256.8717;Float;False;Property;_HazeColor;HazeColor;1;0;Create;True;0;0.772549,0.7176471,0.8509804,1;0.5980331,0.7633312,0.8396226,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;110;-1715.682,1210.124;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-305.5647,193.2276;Float;False;41;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;118;-392.3446,293.0783;Float;False;117;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-37.8497,114.3007;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-36.59314,-45.44698;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;-1524.592,1202.656;Float;False;ReverseCameraViewVector;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
WireConnection;92;0;95;1
WireConnection;92;1;95;2
WireConnection;92;2;95;3
WireConnection;67;0;92;0
WireConnection;45;0;44;2
WireConnection;45;1;46;0
WireConnection;31;0;33;0
WireConnection;31;1;32;0
WireConnection;9;0;121;0
WireConnection;9;1;122;0
WireConnection;97;0;45;0
WireConnection;97;1;67;0
WireConnection;35;0;31;0
WireConnection;35;2;36;0
WireConnection;6;0;9;0
WireConnection;6;1;7;0
WireConnection;6;2;97;0
WireConnection;114;0;116;0
WireConnection;37;0;35;0
WireConnection;37;2;36;0
WireConnection;108;0;114;8
WireConnection;108;1;114;9
WireConnection;108;2;114;10
WireConnection;53;0;6;0
WireConnection;41;0;37;0
WireConnection;2;1;54;0
WireConnection;110;0;108;0
WireConnection;40;0;2;4
WireConnection;40;1;42;0
WireConnection;4;0;99;0
WireConnection;4;1;2;0
WireConnection;117;0;110;0
WireConnection;101;0;4;0
WireConnection;101;2;4;0
WireConnection;101;9;40;0
WireConnection;101;12;118;0
ASEEND*/
//CHKSM=265EA107C59137D6E786D976370A26F94F545294
