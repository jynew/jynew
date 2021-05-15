// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASE/ToonTerrain"
{
	Properties
	{
		_OutlineWidth("Outline Width", Range( 0 , 0.5)) = 0.03
		_OutlineColor("Outline Color", Color) = (0,0,0,0)
		_controlTex("controlTex", 2D) = "white" {}
		_controlTex_2("controlTex_2", 2D) = "white" {}
		_Tex_01("Tex_01", 2D) = "gray" {}
		_TextureSample0("Texture Sample 0", 2D) = "gray" {}
		_Tex_02("Tex_02", 2D) = "gray" {}
		_Tex_03("Tex_03", 2D) = "gray" {}
		_Tex_04("Tex_04", 2D) = "gray" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		
		
		struct Input
		{
			half filler;
		};
		uniform float _OutlineWidth;
		uniform float4 _OutlineColor;
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float outlineVar = _OutlineWidth;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _OutlineColor.rgb;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Tex_01;
		uniform float4 _Tex_01_ST;
		uniform sampler2D _controlTex;
		uniform float4 _controlTex_ST;
		uniform sampler2D _Tex_02;
		uniform float4 _Tex_02_ST;
		uniform sampler2D _Tex_03;
		uniform float4 _Tex_03_ST;
		uniform sampler2D _Tex_04;
		uniform float4 _Tex_04_ST;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform sampler2D _controlTex_2;
		uniform float4 _controlTex_2_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += 0;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Tex_01 = i.uv_texcoord * _Tex_01_ST.xy + _Tex_01_ST.zw;
			float2 uv_controlTex = i.uv_texcoord * _controlTex_ST.xy + _controlTex_ST.zw;
			float4 tex2DNode23 = tex2D( _controlTex, uv_controlTex );
			float2 uv_Tex_02 = i.uv_texcoord * _Tex_02_ST.xy + _Tex_02_ST.zw;
			float2 uv_Tex_03 = i.uv_texcoord * _Tex_03_ST.xy + _Tex_03_ST.zw;
			float2 uv_Tex_04 = i.uv_texcoord * _Tex_04_ST.xy + _Tex_04_ST.zw;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float2 uv_controlTex_2 = i.uv_texcoord * _controlTex_2_ST.xy + _controlTex_2_ST.zw;
			float4 temp_output_31_0 = ( ( tex2D( _Tex_01, uv_Tex_01 ) * tex2DNode23.r ) + ( tex2D( _Tex_02, uv_Tex_02 ) * tex2DNode23.g ) + ( tex2D( _Tex_03, uv_Tex_03 ) * tex2DNode23.b ) + ( tex2D( _Tex_04, uv_Tex_04 ) * tex2DNode23.a ) + ( tex2D( _TextureSample0, uv_TextureSample0 ) * tex2D( _controlTex_2, uv_controlTex_2 ).r ) );
			o.Albedo = temp_output_31_0.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
315;32;1568;889;2587.11;717.7767;2.010388;True;True
Node;AmplifyShaderEditor.SamplerNode;26;-1462.24,617.5281;Float;True;Property;_Tex_04;Tex_04;9;0;Create;True;0;0;False;0;None;50e393550fcc6f6408cd838487fee852;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;25;-1477.84,405.6281;Float;True;Property;_Tex_03;Tex_03;8;0;Create;True;0;0;False;0;None;50e393550fcc6f6408cd838487fee852;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;24;-1467.44,200.228;Float;True;Property;_Tex_02;Tex_02;7;0;Create;True;0;0;False;0;None;6e73a8334e092d745af1b3ba50749da1;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;22;-823.6911,598.6088;Float;False;710.089;357.0004;Comment;3;7;8;11;outline;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;34;-1435.243,898.0063;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;0;0;False;0;None;50e393550fcc6f6408cd838487fee852;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;20;-1467.357,-11.50662;Float;True;Property;_Tex_01;Tex_01;5;0;Create;True;0;0;False;0;None;50e393550fcc6f6408cd838487fee852;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;33;-1841.084,920.5997;Float;True;Property;_controlTex_2;controlTex_2;4;0;Create;True;0;0;False;0;None;b6e8b72479933a24291cb40b2fe30a0d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;-1804.193,127.806;Float;True;Property;_controlTex;controlTex;3;0;Create;True;0;0;False;0;None;b6e8b72479933a24291cb40b2fe30a0d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1069.739,575.928;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1069.22,913.3997;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;21;-1927.36,-694.961;Float;False;1252.272;640.8292;Comment;9;16;5;19;4;12;13;14;18;2;toon;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-773.6911,839.6093;Float;False;Property;_OutlineWidth;Outline Width;1;0;Create;True;0;0;False;0;0.03;0.225;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-712.0367,648.6088;Float;False;Property;_OutlineColor;Outline Color;2;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1083.94,72.828;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1078.739,244.428;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-1049.439,397.828;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ObjSpaceLightDirHlpNode;13;-1877.36,-237.1318;Float;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OutlineNode;11;-366.6021,755.5331;Float;False;0;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1161.603,-415.1042;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;14;-1847.269,-375.3716;Float;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-468.0864,89.66095;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;2;-1692.936,-411.9634;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-807.1395,265.228;Float;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;5;-995.088,-473.7178;Float;True;Property;_Ramp;Ramp;0;0;Create;True;0;0;False;0;ff05cd32e25b92d4f88fa0efe0e12363;ff05cd32e25b92d4f88fa0efe0e12363;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;4;-1438.465,-446.0556;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;19;-1296.849,-538.3611;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;12;-1679.366,-644.961;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;18;-1865.438,-537.6284;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;ASE/ToonTerrain;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;30;0;26;0
WireConnection;30;1;23;4
WireConnection;35;0;34;0
WireConnection;35;1;33;1
WireConnection;27;0;20;0
WireConnection;27;1;23;1
WireConnection;28;0;24;0
WireConnection;28;1;23;2
WireConnection;29;0;25;0
WireConnection;29;1;23;3
WireConnection;11;0;8;0
WireConnection;11;1;7;0
WireConnection;16;0;4;0
WireConnection;32;0;5;0
WireConnection;32;1;31;0
WireConnection;31;0;27;0
WireConnection;31;1;28;0
WireConnection;31;2;29;0
WireConnection;31;3;30;0
WireConnection;31;4;35;0
WireConnection;5;1;19;0
WireConnection;4;0;18;0
WireConnection;4;1;2;0
WireConnection;19;0;4;0
WireConnection;0;0;31;0
WireConnection;0;11;11;0
ASEEND*/
//CHKSM=53D7EB32F8CCBE1BC1CC14E73F726D8181F54696