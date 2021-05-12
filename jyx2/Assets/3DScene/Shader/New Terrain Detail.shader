// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/New Terrain Detail"
{
	Properties
	{
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_ControlTexture1("Control Texture1", 2D) = "white" {}
		_ControlTexture2("Control Texture2", 2D) = "white" {}
		_GrassTex("GrassTex", 2D) = "white" {}
		_SpecularGrass("SpecularGrass", Range( 0 , 1)) = 0
		_GlossGrass("GlossGrass", Range( 0 , 1)) = 0
		_Texture1("Texture 1", 2D) = "white" {}
		_Normal1("Normal 1", 2D) = "bump" {}
		_Specular1("Specular1", Range( 0 , 1)) = 0
		_Gloss1("Gloss1", Range( 0 , 1)) = 0
		_Texture2("Texture 2", 2D) = "white" {}
		_Normal2("Normal 2", 2D) = "bump" {}
		_Specular2("Specular2", Range( 0 , 1)) = 0
		_Gloss2("Gloss2", Range( 0 , 1)) = 0
		_Texture3("Texture 3", 2D) = "white" {}
		_Normal3("Normal 3", 2D) = "bump" {}
		_Specular3("Specular3", Range( 0 , 1)) = 0
		_Gloss3("Gloss3", Range( 0 , 1)) = 0
		_Texture4("Texture 4", 2D) = "white" {}
		_Normal4("Normal 4", 2D) = "bump" {}
		_Specular4("Specular4", Range( 0 , 1)) = 0
		_Gloss4("Gloss4", Range( 0 , 1)) = 0
		_Texture5("Texture 5", 2D) = "white" {}
		_Normal5("Normal 5", 2D) = "bump" {}
		_Specular5("Specular5", Range( 0 , 1)) = 0
		_Gloss5("Gloss5", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf BlinnPhong keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _ControlTexture1;
		uniform float4 _ControlTexture1_ST;
		uniform sampler2D _Normal1;
		uniform float4 _Normal1_ST;
		uniform sampler2D _Normal2;
		uniform float4 _Normal2_ST;
		uniform sampler2D _Normal3;
		uniform float4 _Normal3_ST;
		uniform sampler2D _ControlTexture2;
		uniform float4 _ControlTexture2_ST;
		uniform sampler2D _Normal4;
		uniform float4 _Normal4_ST;
		uniform sampler2D _Normal5;
		uniform float4 _Normal5_ST;
		uniform sampler2D _GrassTex;
		uniform float4 _GrassTex_ST;
		uniform sampler2D _Texture1;
		uniform float4 _Texture1_ST;
		uniform sampler2D _Texture2;
		uniform float4 _Texture2_ST;
		uniform sampler2D _Texture3;
		uniform float4 _Texture3_ST;
		uniform sampler2D _Texture4;
		uniform float4 _Texture4_ST;
		uniform sampler2D _Texture5;
		uniform float4 _Texture5_ST;
		uniform float _SpecularGrass;
		uniform float _Specular1;
		uniform float _Specular2;
		uniform float _Specular3;
		uniform float _Specular4;
		uniform float _Specular5;
		uniform float _GlossGrass;
		uniform float _Gloss1;
		uniform float _Gloss2;
		uniform float _Gloss3;
		uniform float _Gloss4;
		uniform float _Gloss5;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_ControlTexture1 = i.uv_texcoord * _ControlTexture1_ST.xy + _ControlTexture1_ST.zw;
			float4 tex2DNode1 = tex2D( _ControlTexture1, uv_ControlTexture1 );
			float2 uv_Normal1 = i.uv_texcoord * _Normal1_ST.xy + _Normal1_ST.zw;
			float2 uv_Normal2 = i.uv_texcoord * _Normal2_ST.xy + _Normal2_ST.zw;
			float2 uv_Normal3 = i.uv_texcoord * _Normal3_ST.xy + _Normal3_ST.zw;
			float2 uv_ControlTexture2 = i.uv_texcoord * _ControlTexture2_ST.xy + _ControlTexture2_ST.zw;
			float4 tex2DNode11 = tex2D( _ControlTexture2, uv_ControlTexture2 );
			float2 uv_Normal4 = i.uv_texcoord * _Normal4_ST.xy + _Normal4_ST.zw;
			float2 uv_Normal5 = i.uv_texcoord * _Normal5_ST.xy + _Normal5_ST.zw;
			float4 color60 = IsGammaSpace() ? float4(0.5019608,0.5019608,1,0) : float4(0.2158605,0.2158605,1,0);
			o.Normal = ( float4( ( tex2DNode1.g * UnpackNormal( tex2D( _Normal1, uv_Normal1 ) ) ) , 0.0 ) + float4( ( tex2DNode1.b * UnpackNormal( tex2D( _Normal2, uv_Normal2 ) ) ) , 0.0 ) + float4( ( tex2DNode1.a * UnpackNormal( tex2D( _Normal3, uv_Normal3 ) ) ) , 0.0 ) + float4( ( tex2DNode11.r * UnpackNormal( tex2D( _Normal4, uv_Normal4 ) ) ) , 0.0 ) + float4( ( tex2DNode11.g * UnpackNormal( tex2D( _Normal5, uv_Normal5 ) ) ) , 0.0 ) + ( tex2DNode1.r * color60 ) ).rgb;
			float2 uv_GrassTex = i.uv_texcoord * _GrassTex_ST.xy + _GrassTex_ST.zw;
			float2 uv_Texture1 = i.uv_texcoord * _Texture1_ST.xy + _Texture1_ST.zw;
			float2 uv_Texture2 = i.uv_texcoord * _Texture2_ST.xy + _Texture2_ST.zw;
			float2 uv_Texture3 = i.uv_texcoord * _Texture3_ST.xy + _Texture3_ST.zw;
			float2 uv_Texture4 = i.uv_texcoord * _Texture4_ST.xy + _Texture4_ST.zw;
			float2 uv_Texture5 = i.uv_texcoord * _Texture5_ST.xy + _Texture5_ST.zw;
			o.Albedo = ( ( tex2DNode1.r * tex2D( _GrassTex, uv_GrassTex ) ) + ( tex2DNode1.g * tex2D( _Texture1, uv_Texture1 ) ) + ( tex2DNode1.b * tex2D( _Texture2, uv_Texture2 ) ) + ( tex2DNode1.a * tex2D( _Texture3, uv_Texture3 ) ) + ( tex2DNode11.r * tex2D( _Texture4, uv_Texture4 ) ) + ( tex2DNode11.g * tex2D( _Texture5, uv_Texture5 ) ) ).rgb;
			o.Specular = ( ( tex2DNode1.r * _SpecularGrass ) + ( tex2DNode1.g * _Specular1 ) + ( tex2DNode1.b * _Specular2 ) + ( tex2DNode1.a * _Specular3 ) + ( tex2DNode11.r * _Specular4 ) + ( tex2DNode11.g * _Specular5 ) );
			o.Gloss = ( ( tex2DNode1.r * _GlossGrass ) + ( tex2DNode1.g * _Gloss1 ) + ( tex2DNode1.b * _Gloss2 ) + ( tex2DNode1.a * _Gloss3 ) + ( tex2DNode11.r * _Gloss4 ) + ( tex2DNode11.g * _Gloss5 ) );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
2023;109;1774;1010;1369.499;597.8644;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;35;-928.8671,-1608.241;Float;False;874.4577;1290.469;main texture;13;5;4;6;3;14;12;8;7;2;13;9;15;10;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;34;-906.7091,-300.1644;Float;False;895.0651;1311.878;normal;13;25;23;26;17;18;19;22;30;29;27;28;60;61;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;83;122.949,335.9055;Float;False;Property;_Gloss3;Gloss3;17;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-874.8671,-1169.242;Float;True;Property;_Texture2;Texture 2;10;0;Create;True;0;0;False;0;None;7a7a22e947ce5884a9e874840bd7fda5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1866.207,-494.0554;Float;True;Property;_ControlTexture1;Control Texture1;1;0;Create;True;0;0;False;0;None;a445460f121382845ae5d39541c50680;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;82;126.949,222.9068;Float;False;Property;_Gloss2;Gloss2;13;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;113.949,445.9059;Float;False;Property;_Gloss4;Gloss4;21;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-873.8671,-1364.242;Float;True;Property;_Texture1;Texture 1;6;0;Create;True;0;0;False;0;None;7849bff7bc43efd47a6132702917b3ac;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;81;104.9492,131.9068;Float;False;Property;_Gloss1;Gloss1;9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;130.949,549.9067;Float;False;Property;_Gloss5;Gloss5;25;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-862.6024,-547.7715;Float;True;Property;_Texture5;Texture 5;22;0;Create;True;0;0;False;0;None;e4500b480523f044da39d9457eb6d8d2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;63;421.7871,-1456.731;Float;False;Property;_Specular1;Specular1;8;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-872.9117,-969.0103;Float;True;Property;_Texture3;Texture 3;14;0;Create;True;0;0;False;0;None;8be9eed7d3bfad347a69e1b47df3ea43;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;-864.402,-759.9745;Float;True;Property;_Texture4;Texture 4;18;0;Create;True;0;0;False;0;None;82412f6bfdccaaf4aa0550f6c47c85f0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;80;101.8872,20.67083;Float;False;Property;_GlossGrass;GlossGrass;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-878.8671,-1558.241;Float;True;Property;_GrassTex;GrassTex;3;0;Create;True;0;0;False;0;None;06722b99c6c6a454398b7908b1b27b53;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;29;-846.1539,573.5778;Float;True;Property;_Normal4;Normal 4;19;0;Create;True;0;0;False;0;None;5fb3c07e70472444a9efe2596d28803e;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;64;443.7871,-1365.732;Float;False;Property;_Specular2;Specular2;12;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;25;-860.1992,175.3535;Float;True;Property;_Normal2;Normal 2;11;0;Create;True;0;0;False;0;None;d8eebcee6ac9e254684b026f6b2d89c1;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;-858.7979,-31.49096;Float;True;Property;_Normal1;Normal 1;7;0;Create;True;0;0;False;0;None;468f01f2b92a7bd4da77cfed34005c60;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;11;-1859.58,-252.8821;Float;True;Property;_ControlTexture2;Control Texture2;2;0;Create;True;0;0;False;0;None;c6d45a9aea9ac0f4387e51de5da1afa7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;56;418.7253,-1567.967;Float;False;Property;_SpecularGrass;SpecularGrass;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;60;-851.6528,-237.1532;Float;False;Constant;_Color0;Color 0;16;0;Create;True;0;0;False;0;0.5019608,0.5019608,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;26;-851.7032,371.8778;Float;True;Property;_Normal3;Normal 3;15;0;Create;True;0;0;False;0;None;2457e62486a076f46b0ee475a04976a3;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;67;430.7871,-1142.731;Float;False;Property;_Specular4;Specular4;20;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;447.7871,-1038.731;Float;False;Property;_Specular5;Specular5;24;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;439.7871,-1252.731;Float;False;Property;_Specular3;Specular3;16;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-854.2936,805.5721;Float;True;Property;_Normal5;Normal 5;23;0;Create;True;0;0;False;0;None;437747e6b50124b45b3e7f7a996e6f62;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;500.7617,334.8105;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-443.1455,-907.767;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;817.5997,-1253.827;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-442.1817,-1149.148;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;813.5997,-1043.826;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;816.7874,-1571.731;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;496.7617,544.8116;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-435.9456,-793.4018;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-445.1306,-1264.312;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;818.5997,-1364.827;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-445.4029,-1375.333;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;500.7617,436.8108;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-427.6821,191.7728;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-426.3985,579.1782;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;815.5997,-1466.827;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-417.6196,27.7925;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;817.5997,-1151.827;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-427.1749,389.2846;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-427.9441,821.5722;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-457.4394,-209.5789;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;499.9495,16.90691;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;498.7617,121.8117;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-443.5471,-1025.025;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;501.7617,223.8117;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-209.4094,-1226.333;Float;False;6;6;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-149.5265,223.9335;Float;False;6;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;87;1089.641,-1392.658;Float;False;6;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;749.3713,133.7792;Float;False;6;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;593.9528,-652.6146;Float;False;True;2;Float;ASEMaterialInspector;0;0;BlinnPhong;Custom/New Terrain Detail;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.1;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;0;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;77;0;1;4
WireConnection;77;1;83;0
WireConnection;13;0;11;1
WireConnection;13;1;12;0
WireConnection;71;0;1;4
WireConnection;71;1;65;0
WireConnection;8;0;1;3
WireConnection;8;1;5;0
WireConnection;73;0;11;2
WireConnection;73;1;66;0
WireConnection;68;0;1;1
WireConnection;68;1;56;0
WireConnection;79;0;11;2
WireConnection;79;1;85;0
WireConnection;15;0;11;2
WireConnection;15;1;14;0
WireConnection;7;0;1;2
WireConnection;7;1;4;0
WireConnection;70;0;1;3
WireConnection;70;1;64;0
WireConnection;2;0;1;1
WireConnection;2;1;3;0
WireConnection;78;0;11;1
WireConnection;78;1;84;0
WireConnection;18;0;1;3
WireConnection;18;1;25;0
WireConnection;27;0;11;1
WireConnection;27;1;29;0
WireConnection;69;0;1;2
WireConnection;69;1;63;0
WireConnection;19;0;1;2
WireConnection;19;1;23;0
WireConnection;72;0;11;1
WireConnection;72;1;67;0
WireConnection;17;0;1;4
WireConnection;17;1;26;0
WireConnection;28;0;11;2
WireConnection;28;1;30;0
WireConnection;61;0;1;1
WireConnection;61;1;60;0
WireConnection;74;0;1;1
WireConnection;74;1;80;0
WireConnection;75;0;1;2
WireConnection;75;1;81;0
WireConnection;9;0;1;4
WireConnection;9;1;6;0
WireConnection;76;0;1;3
WireConnection;76;1;82;0
WireConnection;10;0;2;0
WireConnection;10;1;7;0
WireConnection;10;2;8;0
WireConnection;10;3;9;0
WireConnection;10;4;13;0
WireConnection;10;5;15;0
WireConnection;22;0;19;0
WireConnection;22;1;18;0
WireConnection;22;2;17;0
WireConnection;22;3;27;0
WireConnection;22;4;28;0
WireConnection;22;5;61;0
WireConnection;87;0;68;0
WireConnection;87;1;69;0
WireConnection;87;2;70;0
WireConnection;87;3;71;0
WireConnection;87;4;72;0
WireConnection;87;5;73;0
WireConnection;86;0;74;0
WireConnection;86;1;75;0
WireConnection;86;2;76;0
WireConnection;86;3;77;0
WireConnection;86;4;78;0
WireConnection;86;5;79;0
WireConnection;0;0;10;0
WireConnection;0;1;22;0
WireConnection;0;3;87;0
WireConnection;0;4;86;0
ASEEND*/
//CHKSM=E20DFA22CF7E529BBB9A866EB21AC55A9E7A2362