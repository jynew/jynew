// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/New Terrain"
{
	Properties
	{
		_ControlTexture1("Control Texture1", 2D) = "white" {}
		_ControlTexture2("Control Texture2", 2D) = "white" {}
		_Texture1("Texture 1", 2D) = "white" {}
		_Texture2("Texture 2", 2D) = "white" {}
		_Texture3("Texture 3", 2D) = "white" {}
		_Texture4("Texture 4", 2D) = "white" {}
		_Texture5("Texture 5", 2D) = "white" {}
		_Texture6("Texture 6", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _ControlTexture1;
		uniform float4 _ControlTexture1_ST;
		uniform sampler2D _Texture1;
		uniform float4 _Texture1_ST;
		uniform sampler2D _Texture2;
		uniform float4 _Texture2_ST;
		uniform sampler2D _Texture3;
		uniform float4 _Texture3_ST;
		uniform sampler2D _Texture4;
		uniform float4 _Texture4_ST;
		uniform sampler2D _ControlTexture2;
		uniform float4 _ControlTexture2_ST;
		uniform sampler2D _Texture5;
		uniform float4 _Texture5_ST;
		uniform sampler2D _Texture6;
		uniform float4 _Texture6_ST;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_ControlTexture1 = i.uv_texcoord * _ControlTexture1_ST.xy + _ControlTexture1_ST.zw;
			float4 tex2DNode1 = tex2D( _ControlTexture1, uv_ControlTexture1 );
			float2 uv_Texture1 = i.uv_texcoord * _Texture1_ST.xy + _Texture1_ST.zw;
			float2 uv_Texture2 = i.uv_texcoord * _Texture2_ST.xy + _Texture2_ST.zw;
			float2 uv_Texture3 = i.uv_texcoord * _Texture3_ST.xy + _Texture3_ST.zw;
			float2 uv_Texture4 = i.uv_texcoord * _Texture4_ST.xy + _Texture4_ST.zw;
			float2 uv_ControlTexture2 = i.uv_texcoord * _ControlTexture2_ST.xy + _ControlTexture2_ST.zw;
			float4 tex2DNode11 = tex2D( _ControlTexture2, uv_ControlTexture2 );
			float2 uv_Texture5 = i.uv_texcoord * _Texture5_ST.xy + _Texture5_ST.zw;
			float2 uv_Texture6 = i.uv_texcoord * _Texture6_ST.xy + _Texture6_ST.zw;
			o.Albedo = ( ( tex2DNode1.r * tex2D( _Texture1, uv_Texture1 ) ) + ( tex2DNode1.g * tex2D( _Texture2, uv_Texture2 ) ) + ( tex2DNode1.b * tex2D( _Texture3, uv_Texture3 ) ) + ( tex2DNode1.a * tex2D( _Texture4, uv_Texture4 ) ) + ( tex2DNode11.r * tex2D( _Texture5, uv_Texture5 ) ) + ( tex2DNode11.g * tex2D( _Texture6, uv_Texture6 ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
2075;261;1774;950;2998.441;1554.655;2.468631;True;True
Node;AmplifyShaderEditor.CommentaryNode;35;-1041.802,-856.2013;Float;False;874.4577;1290.469;main texture;13;5;4;6;3;14;12;8;7;2;13;9;15;10;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;6;-985.8467,-216.9695;Float;True;Property;_Texture4;Texture 4;5;0;Create;True;0;0;False;0;None;8be9eed7d3bfad347a69e1b47df3ea43;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-991.8021,-806.2014;Float;True;Property;_Texture1;Texture 1;2;0;Create;True;0;0;False;0;None;06722b99c6c6a454398b7908b1b27b53;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;11;-1859.58,-252.8821;Float;True;Property;_ControlTexture2;Control Texture2;1;0;Create;True;0;0;False;0;None;c6d45a9aea9ac0f4387e51de5da1afa7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;-977.337,-7.933914;Float;True;Property;_Texture5;Texture 5;6;0;Create;True;0;0;False;0;None;82412f6bfdccaaf4aa0550f6c47c85f0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-986.8021,-612.2018;Float;True;Property;_Texture2;Texture 2;3;0;Create;True;0;0;False;0;None;7849bff7bc43efd47a6132702917b3ac;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;14;-975.5374,204.2688;Float;True;Property;_Texture6;Texture 6;7;0;Create;True;0;0;False;0;None;e4500b480523f044da39d9457eb6d8d2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-987.8021,-417.2013;Float;True;Property;_Texture3;Texture 3;4;0;Create;True;0;0;False;0;None;7a7a22e947ce5884a9e874840bd7fda5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1866.207,-494.0554;Float;True;Property;_ControlTexture1;Control Texture1;0;0;Create;True;0;0;False;0;None;a445460f121382845ae5d39541c50680;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-555.116,-397.1073;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-556.0798,-155.7265;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-558.3372,-623.2928;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-548.8799,-41.3612;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-556.4814,-272.9841;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-558.0649,-512.2715;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-322.3438,-474.2924;Float;False;6;6;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-25.79091,-475.6824;Float;False;True;2;Float;ASEMaterialInspector;0;0;Lambert;Custom/New Terrain;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.1;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;1;3
WireConnection;8;1;5;0
WireConnection;13;0;11;1
WireConnection;13;1;12;0
WireConnection;2;0;1;1
WireConnection;2;1;3;0
WireConnection;15;0;11;2
WireConnection;15;1;14;0
WireConnection;9;0;1;4
WireConnection;9;1;6;0
WireConnection;7;0;1;2
WireConnection;7;1;4;0
WireConnection;10;0;2;0
WireConnection;10;1;7;0
WireConnection;10;2;8;0
WireConnection;10;3;9;0
WireConnection;10;4;13;0
WireConnection;10;5;15;0
WireConnection;0;0;10;0
ASEEND*/
//CHKSM=1369F222AD2224FDC2F3A00CB985021AFC1BD882