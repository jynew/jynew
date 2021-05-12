// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/StaticBakeObj_backup"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0.04
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_EmissionTexture("Emission Texture", 2D) = "white" {}
		_MainTex("MainTex", 2D) = "white" {}
		_MainColor("MainColor", Color) = (1,1,1,0)
		_Normal("Normal", 2D) = "bump" {}
		_EmissionColor("EmissionColor", Color) = (1,1,1,0)
		_FlashScale("FlashScale", Range( 0 , 1)) = 0.5
		_TheFlashColor("TheFlashColor", Color) = (0,0,0,0)
		_FlashIntensity("FlashIntensity", Range( 0 , 1)) = 0.6
		_FlashSpeedX("FlashSpeedX", Range( -5 , 5)) = 0.5
		_FlashSpeedY("FlashSpeedY", Range( -5 , 5)) = 0
		_Visibility("Visibility", Range( 0 , 1)) = 1
		_FlashTex("FlashTex", 2D) = "white" {}
		_Specular("Specular", Range( 0 , 1)) = 0
		_Gloss("Gloss", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		
		
		struct Input {
			half filler;
		};
		uniform half4 _ASEOutlineColor;
		uniform half _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		LOD 700
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			half2 uv_texcoord;
			float4 screenPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform half _FlashIntensity;
		uniform half _Visibility;
		uniform sampler2D _FlashTex;
		uniform half _FlashScale;
		uniform half _FlashSpeedX;
		uniform half _FlashSpeedY;
		uniform half4 _TheFlashColor;
		uniform half4 _MainColor;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _EmissionTexture;
		uniform float4 _EmissionTexture_ST;
		uniform half4 _EmissionColor;
		uniform half _Specular;
		uniform half _Gloss;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 appendResult54 = (half2(ase_screenPosNorm.x , ase_screenPosNorm.y));
			float2 appendResult47 = (half2(( ( appendResult54 * _FlashScale ).x + ( _FlashSpeedX * _Time.y ) ) , ( 0.0 + ( _Time.y * _FlashSpeedY ) )));
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			o.Albedo = ( ( ( _FlashIntensity * _Visibility * tex2D( _FlashTex, appendResult47 ).a ) * _TheFlashColor ) + ( _MainColor * tex2D( _MainTex, uv_MainTex ) ) ).rgb;
			float2 uv_EmissionTexture = i.uv_texcoord * _EmissionTexture_ST.xy + _EmissionTexture_ST.zw;
			o.Emission = ( tex2D( _EmissionTexture, uv_EmissionTexture ) * _EmissionColor ).rgb;
			o.Specular = _Specular;
			o.Gloss = _Gloss;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
2059;127;1774;914;1089.096;477.0021;1;True;True
Node;AmplifyShaderEditor.ScreenPosInputsNode;55;-2136.843,-798.8043;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;54;-1900.746,-929.9678;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1702.199,-781.9439;Float;False;Property;_FlashScale;FlashScale;5;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;42;-1644.622,-606.6732;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1376.18,-889.7894;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1444.342,-692.8036;Float;False;Property;_FlashSpeedX;FlashSpeedX;8;0;Create;True;0;0;False;0;0.5;0.5;-5;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1449.342,-448.8047;Float;False;Property;_FlashSpeedY;FlashSpeedY;9;0;Create;True;0;0;False;0;0;0;-5;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1139.623,-530.6737;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1139.623,-631.6729;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;39;-1230.622,-889.6728;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-903.6223,-653.6729;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-910.6222,-848.6728;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;51;-1699.13,-1244.957;Float;True;Property;_FlashTex;FlashTex;11;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DynamicAppendNode;47;-749.058,-759.8304;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-572.5515,-980.6188;Float;False;Property;_FlashIntensity;FlashIntensity;7;0;Create;True;0;0;False;0;0.6;0.6;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;16;-581.9925,-786.8751;Float;True;Property;_sampleflash;sampleflash;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;37;-569.4451,-894.8147;Float;False;Property;_Visibility;Visibility;10;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-504.187,-383.4447;Float;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;33;-234.9213,-721.0696;Float;False;Property;_TheFlashColor;TheFlashColor;6;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-182.299,-895.7656;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-503.2785,-560.3404;Float;False;Property;_MainColor;MainColor;2;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;10;-449.8413,392.1671;Float;False;Property;_EmissionColor;EmissionColor;4;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-105.9709,-394.5078;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-3.127998,-803.0344;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;9;-497.0579,165.2984;Float;True;Property;_EmissionTexture;Emission Texture;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-502.1275,-187.2163;Float;True;Property;_Normal;Normal;3;0;Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;57;-484.0964,11.99792;Half;False;Property;_Specular;Specular;12;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;50;133.1259,-542.0714;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-115.6898,324.2215;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;53;-2122.72,-950.147;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;58;-486.0964,86.99792;Half;False;Property;_Gloss;Gloss;13;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;13;86.2947,-123.0772;Half;False;True;2;Half;ASEMaterialInspector;700;0;Lambert;Custom/StaticBakeObj_backup;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;7;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;True;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.04;0,0,0,0;VertexOffset;False;False;Cylindrical;False;Relative;700;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;54;0;55;1
WireConnection;54;1;55;2
WireConnection;38;0;54;0
WireConnection;38;1;32;0
WireConnection;44;0;42;2
WireConnection;44;1;36;0
WireConnection;43;0;35;0
WireConnection;43;1;42;2
WireConnection;39;0;38;0
WireConnection;46;1;44;0
WireConnection;45;0;39;0
WireConnection;45;1;43;0
WireConnection;47;0;45;0
WireConnection;47;1;46;0
WireConnection;16;0;51;0
WireConnection;16;1;47;0
WireConnection;48;0;34;0
WireConnection;48;1;37;0
WireConnection;48;2;16;4
WireConnection;5;0;4;0
WireConnection;5;1;1;0
WireConnection;49;0;48;0
WireConnection;49;1;33;0
WireConnection;50;0;49;0
WireConnection;50;1;5;0
WireConnection;11;0;9;0
WireConnection;11;1;10;0
WireConnection;13;0;50;0
WireConnection;13;1;3;0
WireConnection;13;2;11;0
WireConnection;13;3;57;0
WireConnection;13;4;58;0
ASEEND*/
//CHKSM=6F15FD2351EF5471C35F8991AC64943D822FD63D