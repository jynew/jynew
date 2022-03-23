// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CustomTerrain"
{
	Properties
	{
		_ControlTex("ControlTex", 2D) = "white" {}
		_ControlTex2("ControlTex2", 2D) = "white" {}
		_Texture1("Texture1", 2D) = "white" {}
		[Normal]_Normal1("Normal1", 2D) = "bump" {}
		_Normal1Strength("Normal1Strength", Float) = 0
		_Texture2("Texture2", 2D) = "white" {}
		[Normal]_Normal2("Normal2", 2D) = "bump" {}
		_Normal2Strength("Normal2Strength", Float) = 0
		_Texture3("Texture3", 2D) = "white" {}
		[Normal]_Normal3("Normal3", 2D) = "bump" {}
		_Normal3Strength("Normal3Strength", Float) = 0
		_Texture4("Texture4", 2D) = "white" {}
		[Normal]_Normal4("Normal4", 2D) = "bump" {}
		_Normal4Strength("Normal4Strength", Float) = 0
		_Texture5("Texture5", 2D) = "white" {}
		[Normal]_Normal5("Normal5", 2D) = "bump" {}
		_Normal5Strength("Normal5Strength", Float) = 0
		_Texture6("Texture6", 2D) = "white" {}
		_Normal6Strength("Normal6Strength", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal1;
		uniform float4 _Normal1_ST;
		uniform sampler2D _ControlTex;
		uniform float4 _ControlTex_ST;
		uniform float _Normal1Strength;
		uniform sampler2D _Normal2;
		uniform float4 _Normal2_ST;
		uniform float _Normal2Strength;
		uniform sampler2D _Normal3;
		uniform float4 _Normal3_ST;
		uniform float _Normal3Strength;
		uniform sampler2D _Normal4;
		uniform float4 _Normal4_ST;
		uniform float _Normal4Strength;
		uniform sampler2D _ControlTex2;
		uniform float4 _ControlTex2_ST;
		uniform float _Normal6Strength;
		uniform sampler2D _Normal5;
		uniform float4 _Normal5_ST;
		uniform float _Normal5Strength;
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
		uniform sampler2D _Texture6;
		uniform float4 _Texture6_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal1 = i.uv_texcoord * _Normal1_ST.xy + _Normal1_ST.zw;
			float2 uv_ControlTex = i.uv_texcoord * _ControlTex_ST.xy + _ControlTex_ST.zw;
			float4 tex2DNode1 = tex2D( _ControlTex, uv_ControlTex );
			float2 uv_Normal2 = i.uv_texcoord * _Normal2_ST.xy + _Normal2_ST.zw;
			float2 uv_Normal3 = i.uv_texcoord * _Normal3_ST.xy + _Normal3_ST.zw;
			float2 uv_Normal4 = i.uv_texcoord * _Normal4_ST.xy + _Normal4_ST.zw;
			float3 tex2DNode18 = UnpackNormal( tex2D( _Normal4, uv_Normal4 ) );
			float4 color26 = IsGammaSpace() ? float4(0,0,1,0) : float4(0,0,1,0);
			float2 uv_ControlTex2 = i.uv_texcoord * _ControlTex2_ST.xy + _ControlTex2_ST.zw;
			float4 tex2DNode33 = tex2D( _ControlTex2, uv_ControlTex2 );
			float2 uv_Normal5 = i.uv_texcoord * _Normal5_ST.xy + _Normal5_ST.zw;
			o.Normal = saturate( ( float4( ( UnpackNormal( tex2D( _Normal1, uv_Normal1 ) ) * tex2DNode1.r * _Normal1Strength ) , 0.0 ) + float4( ( UnpackNormal( tex2D( _Normal2, uv_Normal2 ) ) * tex2DNode1.g * _Normal2Strength ) , 0.0 ) + float4( ( UnpackNormal( tex2D( _Normal3, uv_Normal3 ) ) * tex2DNode1.b * _Normal3Strength ) , 0.0 ) + float4( ( tex2DNode18 * tex2DNode1.a * _Normal4Strength ) , 0.0 ) + color26 + float4( ( tex2DNode18 * tex2DNode33.g * _Normal6Strength ) , 0.0 ) + float4( ( UnpackNormal( tex2D( _Normal5, uv_Normal5 ) ) * tex2DNode33.r * _Normal5Strength ) , 0.0 ) ) ).rgb;
			float2 uv_Texture1 = i.uv_texcoord * _Texture1_ST.xy + _Texture1_ST.zw;
			float2 uv_Texture2 = i.uv_texcoord * _Texture2_ST.xy + _Texture2_ST.zw;
			float2 uv_Texture3 = i.uv_texcoord * _Texture3_ST.xy + _Texture3_ST.zw;
			float2 uv_Texture4 = i.uv_texcoord * _Texture4_ST.xy + _Texture4_ST.zw;
			float2 uv_Texture5 = i.uv_texcoord * _Texture5_ST.xy + _Texture5_ST.zw;
			float2 uv_Texture6 = i.uv_texcoord * _Texture6_ST.xy + _Texture6_ST.zw;
			o.Albedo = ( ( tex2D( _Texture1, uv_Texture1 ) * tex2DNode1.r ) + ( tex2D( _Texture2, uv_Texture2 ) * tex2DNode1.g ) + ( tex2D( _Texture3, uv_Texture3 ) * tex2DNode1.b ) + ( tex2D( _Texture4, uv_Texture4 ) * tex2DNode1.a ) + ( tex2D( _Texture5, uv_Texture5 ) * tex2DNode33.r ) + ( tex2D( _Texture6, uv_Texture6 ) * tex2DNode33.g ) ).rgb;
			float temp_output_28_0 = 0.0;
			o.Metallic = temp_output_28_0;
			o.Smoothness = temp_output_28_0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
756;69;1002;626;1657.905;1419.607;1.763076;True;True
Node;AmplifyShaderEditor.SamplerNode;16;-1020.907,545.8455;Float;True;Property;_Normal2;Normal2;6;1;[Normal];Create;True;0;0;False;0;None;db0bef606d00d854599028a957e5c330;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;38;-918.6558,1210.398;Float;True;Property;_Normal5;Normal5;15;1;[Normal];Create;True;0;0;False;0;None;b760ab70be7756a4bb47d4a5dc7615cd;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;43;-534.293,1461.738;Float;False;Property;_Normal6Strength;Normal6Strength;18;0;Create;True;0;0;False;0;0;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-654.6846,921.1453;Float;False;Property;_Normal3Strength;Normal3Strength;10;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1845.696,32.08175;Float;True;Property;_ControlTex;ControlTex;0;0;Create;True;0;0;False;0;None;b6e8b72479933a24291cb40b2fe30a0d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;41;-521.3213,1283.759;Float;False;Property;_Normal5Strength;Normal5Strength;16;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-726.0483,243.473;Float;False;Property;_Normal1Strength;Normal1Strength;4;0;Create;True;0;0;False;0;0;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-1820.646,298.1606;Float;True;Property;_ControlTex2;ControlTex2;1;0;Create;True;0;0;False;0;None;b6e8b72479933a24291cb40b2fe30a0d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;30;-709.0833,685.9451;Float;False;Property;_Normal2Strength;Normal2Strength;7;0;Create;True;0;0;False;0;0;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;-903.2284,1000.188;Float;True;Property;_Normal4;Normal4;12;1;[Normal];Create;True;0;0;False;0;None;00af2faeabd9ff6478ba642e9dbc20bd;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;17;-929.2419,760.7883;Float;True;Property;_Normal3;Normal3;9;1;[Normal];Create;True;0;0;False;0;None;f160a67640adbfd48b34df63ed2bd544;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;32;-542.5096,1035.923;Float;False;Property;_Normal4Strength;Normal4Strength;13;0;Create;True;0;0;False;0;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;15;-957.9752,330.861;Float;True;Property;_Normal1;Normal1;3;1;[Normal];Create;True;0;0;False;0;None;adcb2e3be6d6b0a4b82edb1a2438519c;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-203.201,956.4764;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;2;-937.9875,-1071.436;Float;True;Property;_Texture1;Texture1;2;0;Create;True;0;0;False;0;None;4c9f8a1eb258f7e46bd799dce0d9aad2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;26;-339.2773,589.5631;Float;False;Constant;_Color0;Color 0;9;0;Create;True;0;0;False;0;0,0,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;34;-955.0604,-241.0989;Float;True;Property;_Texture5;Texture5;14;0;Create;True;0;0;False;0;None;6d3445ea818ff254b82f7afd84e13e67;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-944.8499,-658.0518;Float;True;Property;_Texture3;Texture3;8;0;Create;True;0;0;False;0;None;ef8f10a83e19589478e6ec761aec1ef0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-325.7878,834.6749;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-269.2299,1157.37;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;35;-964.1541,-33.89616;Float;True;Property;_Texture6;Texture6;17;0;Create;True;0;0;False;0;None;6278e331fa930df4fac06445a1663735;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-520.6783,549.4307;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;3;-946.0932,-863.0989;Float;True;Property;_Texture2;Texture2;5;0;Create;True;0;0;False;0;None;f4261ca8ea0a8924e8f212941c672e52;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-279.1302,1420.92;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;5;-955.5334,-448.3192;Float;True;Property;_Texture4;Texture4;11;0;Create;True;0;0;False;0;None;b7c70c6e4b4014c49a00716c84b33fc0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-532.7156,388.5004;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-558.0712,-635.5035;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-497.0179,-31.36279;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-81.42817,435.123;Float;False;7;7;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;COLOR;0,0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-532.123,-1026.93;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-554.4978,-440.6412;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-534.4607,-807.5369;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-505.7643,-221.7535;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;58.4151,-295.8976;Float;False;6;6;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;29;128.7986,425.7278;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;28;185.0938,114.1543;Float;False;Constant;_Float1;Float 1;10;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1540.564,-263.7321;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;44;-1796.213,-302.6825;Float;True;Simplex2D;1;0;FLOAT2;1,500;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;335.8727,36.57453;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;CustomTerrain;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;18;0
WireConnection;22;1;1;4
WireConnection;22;2;32;0
WireConnection;21;0;17;0
WireConnection;21;1;1;3
WireConnection;21;2;31;0
WireConnection;40;0;38;0
WireConnection;40;1;33;1
WireConnection;40;2;41;0
WireConnection;20;0;16;0
WireConnection;20;1;1;2
WireConnection;20;2;30;0
WireConnection;42;0;18;0
WireConnection;42;1;33;2
WireConnection;42;2;43;0
WireConnection;19;0;15;0
WireConnection;19;1;1;1
WireConnection;19;2;27;0
WireConnection;12;0;4;0
WireConnection;12;1;1;3
WireConnection;37;0;35;0
WireConnection;37;1;33;2
WireConnection;23;0;19;0
WireConnection;23;1;20;0
WireConnection;23;2;21;0
WireConnection;23;3;22;0
WireConnection;23;4;26;0
WireConnection;23;5;42;0
WireConnection;23;6;40;0
WireConnection;10;0;2;0
WireConnection;10;1;1;1
WireConnection;13;0;5;0
WireConnection;13;1;1;4
WireConnection;11;0;3;0
WireConnection;11;1;1;2
WireConnection;36;0;34;0
WireConnection;36;1;33;1
WireConnection;14;0;10;0
WireConnection;14;1;11;0
WireConnection;14;2;12;0
WireConnection;14;3;13;0
WireConnection;14;4;36;0
WireConnection;14;5;37;0
WireConnection;29;0;23;0
WireConnection;45;0;44;0
WireConnection;45;1;1;1
WireConnection;0;0;14;0
WireConnection;0;1;29;0
WireConnection;0;3;28;0
WireConnection;0;4;28;0
ASEEND*/
//CHKSM=112411AF4DB27B8EB5663726400B0807AD103767