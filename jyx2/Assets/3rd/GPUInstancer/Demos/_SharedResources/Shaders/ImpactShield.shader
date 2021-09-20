// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GPUInstancer/ImpactShield"
{
	Properties
	{
		_ImpactPosition("Impact Position", Vector) = (0,0,0,0)
		_ImpactTime("Impact Time", Float) = 0
		_ImpactColor("Impact Color", Color) = (0.378244,0.6964117,0.7794118,1)
		_ImpactSize("Impact Size", Float) = 0.2
		_ShieldImpactTexture("Shield Impact Texture", 2D) = "white" {}
		_IdleShieldTexture("Idle Shield Texture", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _ImpactTime;
		uniform float3 _ImpactPosition;
		uniform float _ImpactSize;
		uniform sampler2D _IdleShieldTexture;
		uniform float4 _IdleShieldTexture_ST;
		uniform float4 _ImpactColor;
		uniform sampler2D _ShieldImpactTexture;
		uniform float4 _ShieldImpactTexture_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float temp_output_22_0 = distance( ase_vertex3Pos , _ImpactPosition );
			float2 uv_IdleShieldTexture = i.uv_texcoord * _IdleShieldTexture_ST.xy + _IdleShieldTexture_ST.zw;
			float4 tex2DNode36 = tex2D( _IdleShieldTexture, uv_IdleShieldTexture );
			float2 uv_ShieldImpactTexture = i.uv_texcoord * _ShieldImpactTexture_ST.xy + _ShieldImpactTexture_ST.zw;
			float4 lerpResult23 = lerp( tex2DNode36 , ( ( _ImpactSize / temp_output_22_0 ) * ( _ImpactColor * tex2D( _ShieldImpactTexture, uv_ShieldImpactTexture ) ) ) , (0 + (_ImpactTime - 0) * (1 - 0) / (100 - 0)));
			float4 temp_output_18_0 = (( _ImpactTime > 0 ) ? (( temp_output_22_0 < _ImpactSize ) ? lerpResult23 :  tex2DNode36 ) :  tex2DNode36 );
			o.Emission = temp_output_18_0.rgb;
			o.Alpha = temp_output_18_0.r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
391;126;1653;883;3502.456;1009.494;2.337282;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;2;-2313.826,-277.3042;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;21;-2305.219,-93.00385;Float;False;Property;_ImpactPosition;Impact Position;0;0;Create;True;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;34;-2326.091,507.6712;Float;True;Property;_ShieldImpactTexture;Shield Impact Texture;4;0;Create;True;0;ec89eb58be74afb47a32870060361505;ec89eb58be74afb47a32870060361505;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceOpNode;22;-2102.865,-199.1562;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-2323.75,297.3729;Float;False;Property;_ImpactColor;Impact Color;2;0;Create;True;0;0.378244,0.6964117,0.7794118,1;0.1333333,0.3372549,0.1372549,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1900.948,480.7592;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;38;-2164.77,171.2971;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1994.307,-57.90455;Float;False;Property;_ImpactSize;Impact Size;3;0;Create;True;0;0.2;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;8;-1355.407,455.6845;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1599.909,5.330079;Float;False;Property;_ImpactTime;Impact Time;1;0;Create;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;40;-1576.416,588.3773;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;36;-1608.653,-212.9055;Float;True;Property;_IdleShieldTexture;Idle Shield Texture;5;0;Create;True;0;c3283ecb9e4f9004d89cb59a8745bb27;c3283ecb9e4f9004d89cb59a8745bb27;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1119.384,475.0891;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;9;-1264.276,290.6413;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;100;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;39;-1590.688,-442.429;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;23;-869.5189,244.3457;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;37;-1527.254,-491.5907;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareLower;13;-770.1561,-210.6235;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareGreater;18;-470.6603,19.93088;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-18.33554,-20.37283;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;GPUInstancer/ImpactShield;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;Off;0;0;False;0;0;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;2;0
WireConnection;22;1;21;0
WireConnection;35;0;4;0
WireConnection;35;1;34;0
WireConnection;38;0;22;0
WireConnection;8;0;3;0
WireConnection;8;1;38;0
WireConnection;40;0;35;0
WireConnection;10;0;8;0
WireConnection;10;1;40;0
WireConnection;9;0;17;0
WireConnection;39;0;3;0
WireConnection;23;0;36;0
WireConnection;23;1;10;0
WireConnection;23;2;9;0
WireConnection;37;0;22;0
WireConnection;13;0;37;0
WireConnection;13;1;39;0
WireConnection;13;2;23;0
WireConnection;13;3;36;0
WireConnection;18;0;17;0
WireConnection;18;2;13;0
WireConnection;18;3;36;0
WireConnection;0;2;18;0
WireConnection;0;9;18;0
ASEEND*/
//CHKSM=50EF3279FA52B4A35623576F83F61B0CF7E347E3