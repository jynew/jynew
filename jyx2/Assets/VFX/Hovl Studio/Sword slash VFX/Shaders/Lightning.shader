Shader "Hovl/Particles/Lightning"
{
	Properties
	{	
		_MainTexture("Main Texture", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_FlowMap("Flow Map", 2D) = "white" {}
		_VFlowSpeed("V Flow Speed", Float) = 2
		_UFlowSpeed("U Flow Speed", Float) = 4
		_FlowStrength("Flow Strength", Float) = 0.1
		_Color("Color", Color) = (1,1,1,1)
		_Emission("Emission", Float) = 2
		_ShinnySpeed("Shinny Speed", Float) = 30
		[Toggle]_UseShinny("Use Shinny", Float) = 0
		[MaterialToggle] _Usedepth ("Use depth?", Float ) = 0
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	}


	Category 
	{
		SubShader
		{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					float4 ase_texcoord1 : TEXCOORD1;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					float4 ase_texcoord3 : TEXCOORD3;
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform float _InvFade;
				uniform sampler2D _Noise;
				uniform sampler2D _MainTexture;
				uniform float4 _MainTexture_ST;
				uniform sampler2D _FlowMap;
				uniform float _UFlowSpeed;
				uniform float _VFlowSpeed;
				uniform float _FlowStrength;
				uniform float _UseShinny;
				uniform float _ShinnySpeed;
				uniform float4 _Color;
				uniform float _Emission;
				uniform fixed _Usedepth;

				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					o.ase_texcoord3 = v.ase_texcoord1;

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 uv0_MainTexture = i.texcoord.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
					float2 UV23 = uv0_MainTexture;
					float4 uv159 = i.ase_texcoord3;
					uv159.xy = i.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
					float T79 = uv159.w;
					float2 temp_output_81_0 = ( uv0_MainTexture + T79 );
					float2 panner2 = ( sin( ( _UFlowSpeed * _Time.y ) ) * float2( 1,0 ) + temp_output_81_0);
					float2 panner3 = ( ( _VFlowSpeed * _Time.y ) * float2( -0.1,1 ) + temp_output_81_0);
					float2 panner4 = ( ( ( _VFlowSpeed * 0.25 ) * _Time.y ) * float2( 0.1,0.75 ) + temp_output_81_0);
					float2 appendResult20 = (float2(tex2D( _FlowMap, panner2 ).r , ( ( tex2D( _FlowMap, panner3 ).g + tex2D( _FlowMap, panner4 ).g ) / 2.0 )));
					float4 tex2DNode27 = tex2D( _MainTexture, ( UV23 + ( appendResult20 * _FlowStrength ) ) );
					float temp_output_33_0 = ( tex2D( _Noise, UV23 ).g * tex2DNode27.g );
					float clampResult35 = clamp( ( temp_output_33_0 * 16.0 * temp_output_33_0 ) , 0.0 , 1.0 );
					float V24 = uv0_MainTexture.y;
					float clampResult56 = clamp( ( ( V24 + (-1.0 + (uv159.x - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) * 2.0 ) , 0.0 , 1.0 );
					float W75 = uv159.z;
					float temp_output_45_0 = ( _Time.y * ( _ShinnySpeed - W75 ) );
					float blendOpSrc50 = sin( temp_output_45_0 );
					float blendOpDest50 = sin( ( temp_output_45_0 / 3.0 ) );
					float clampResult52 = clamp( ( ( saturate( ( blendOpSrc50 + blendOpDest50 ) )) + 0.2 ) , 0.0 , 1.0 );
					float temp_output_54_0 = ( tex2DNode27.r * clampResult56 * lerp(1.0,clampResult52,_UseShinny) );
					float2 panner66 = ( 1.0 * _Time.y * float2( 0,0.2 ) + ( UV23 + T79 ));
					float clampResult73 = clamp( pow( ( ( tex2DNode27.g * ( 1.0 - tex2DNode27.g ) ) + ( tex2DNode27.g * tex2D( _Noise, panner66 ).r ) ) , uv159.y ) , 0.0 , 1.0 );
					float4 appendResult43 = (float4((( ( clampResult35 + ( ( tex2DNode27.r + 0.1 ) * 6.0 ) + temp_output_54_0 ) * _Color * i.color * _Emission )).rgb , ( _Color.a * i.color.a * temp_output_54_0 * clampResult73 )));
					

					fixed4 col = appendResult43;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	
	
	
}
/*ASEBEGIN
Version=17000
7;29;1906;1004;2430.084;817.2708;2.275535;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;59;-692.6807,1172.693;Float;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-335.4295,961.748;Float;False;T;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1979.956,49.42834;Float;False;Property;_VFlowSpeed;V Flow Speed;3;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1811.553,-268.2933;Float;False;0;27;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;80;-1760.335,-139.4714;Float;False;79;T;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-1765.956,186.4283;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;11;-2064.956,401.4283;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-329.7014,1251.581;Float;False;W;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;76;-2264.101,1042.045;Float;False;75;W;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;81;-1510.027,-141.9742;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1560.956,187.4283;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-2263.158,920.874;Float;False;Property;_ShinnySpeed;Shinny Speed;8;0;Create;True;0;0;False;0;30;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1565.956,54.42834;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1980.956,-65.57166;Float;False;Property;_UFlowSpeed;U Flow Speed;4;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;14;-1393.076,301.9138;Float;True;Property;_FlowMap;Flow Map;2;0;Create;True;0;0;False;0;cd460ee4ac5c1e746b7a734cc7cc64dd;cd460ee4ac5c1e746b7a734cc7cc64dd;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;3;-1287.031,25.29276;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;77;-2033.404,929.8829;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1748.956,-57.57166;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;4;-1284.353,137.7755;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.75;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1839.405,915.7187;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;17;-1031.432,168.2302;Float;True;Property;_TextureSample2;Texture Sample 2;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-1035.332,-15.06997;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;6;-1556.8,-44.97375;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;48;-1611.405,1026.719;Float;False;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;2;-1285.693,-94.93975;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-687.974,36.39175;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;49;-1456.405,1027.719;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;19;-533.1742,34.39166;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;15;-1031.433,-199.67;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;47;-1614.405,915.7187;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-352.5511,-171.7755;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-1456.556,-369.3494;Float;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-362.7513,-53.64629;Float;False;Property;_FlowStrength;Flow Strength;5;0;Create;True;0;0;False;0;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;50;-1315.405,912.7187;Float;False;LinearDodge;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-1457.825,-298.3492;Float;False;V;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-257.8503,-343.1044;Float;False;23;UV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-1058.405,917.7187;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-144.3515,-171.9464;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;67;-272.4731,745.1285;Float;False;23;UV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;60;-259.2249,462.7086;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;52;-902.4053,917.7187;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;31;-55.92051,-21.59654;Float;True;Property;_Noise;Noise;1;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleAddOpNode;78;-85.8739,737.2612;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-258.2249,369.7086;Float;False;24;V;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;53.87817,-194.9075;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;32;215.6332,-447.6553;Float;True;Property;_TextureSample3;Texture Sample 3;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;27;205.6946,-222.9074;Float;True;Property;_MainTexture;Main Texture;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;66;77.05768,733.7287;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ToggleSwitchNode;53;-692.4053,889.7186;Float;False;Property;_UseShinny;Use Shinny;9;0;Create;True;0;0;False;0;0;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;45.77515,376.7086;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;70;-211.5025,671.2339;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;209.0107,374.3527;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;65;280.9352,709.6451;Float;True;Property;_TextureSample4;Texture Sample 4;9;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;584.8867,-349.7775;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;62;654.5898,513.7711;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;838.2484,488.113;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;609.3632,-194.7039;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;56;398.6074,375.1491;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;635.8929,620.467;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;783.0154,-370.4595;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;16;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;69;309.5685,597.4816;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;844.9501,322.4195;Float;False;3;3;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;68;1026.209,595.8459;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;785.3632,-193.7039;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;6;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;35;961.3632,-370.7039;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;1166.705,231.8248;Float;False;Property;_Emission;Emission;7;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;41;1144.762,67.83511;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;40;1127.001,-93.92087;Float;False;Property;_Color;Color;6;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;36;1149.387,-265.9435;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;72;1196.69,784.9392;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;73;1382.504,782.6417;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;1404.353,-107.4755;Float;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;44;1556.453,-115.2753;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;1586.52,291.4774;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0.3;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;43;1850.255,-108.7753;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;2125.314,-100.5251;Float;False;True;2;Float;;0;11;Hovl/Particles/Lightning;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;79;0;59;4
WireConnection;9;0;13;0
WireConnection;75;0;59;3
WireConnection;81;0;5;0
WireConnection;81;1;80;0
WireConnection;8;0;9;0
WireConnection;8;1;11;0
WireConnection;7;0;13;0
WireConnection;7;1;11;0
WireConnection;3;0;81;0
WireConnection;3;1;7;0
WireConnection;77;0;46;0
WireConnection;77;1;76;0
WireConnection;10;0;12;0
WireConnection;10;1;11;0
WireConnection;4;0;81;0
WireConnection;4;1;8;0
WireConnection;45;0;11;0
WireConnection;45;1;77;0
WireConnection;17;0;14;0
WireConnection;17;1;4;0
WireConnection;16;0;14;0
WireConnection;16;1;3;0
WireConnection;6;0;10;0
WireConnection;48;0;45;0
WireConnection;2;0;81;0
WireConnection;2;1;6;0
WireConnection;18;0;16;2
WireConnection;18;1;17;2
WireConnection;49;0;48;0
WireConnection;19;0;18;0
WireConnection;15;0;14;0
WireConnection;15;1;2;0
WireConnection;47;0;45;0
WireConnection;20;0;15;1
WireConnection;20;1;19;0
WireConnection;23;0;5;0
WireConnection;50;0;47;0
WireConnection;50;1;49;0
WireConnection;24;0;5;2
WireConnection;51;0;50;0
WireConnection;21;0;20;0
WireConnection;21;1;22;0
WireConnection;60;0;59;1
WireConnection;52;0;51;0
WireConnection;78;0;67;0
WireConnection;78;1;79;0
WireConnection;26;0;25;0
WireConnection;26;1;21;0
WireConnection;32;0;31;0
WireConnection;32;1;25;0
WireConnection;27;1;26;0
WireConnection;66;0;78;0
WireConnection;53;1;52;0
WireConnection;57;0;58;0
WireConnection;57;1;60;0
WireConnection;70;0;53;0
WireConnection;55;0;57;0
WireConnection;65;0;31;0
WireConnection;65;1;66;0
WireConnection;33;0;32;2
WireConnection;33;1;27;2
WireConnection;62;0;27;2
WireConnection;63;0;27;2
WireConnection;63;1;62;0
WireConnection;37;0;27;1
WireConnection;56;0;55;0
WireConnection;64;0;27;2
WireConnection;64;1;65;1
WireConnection;34;0;33;0
WireConnection;34;2;33;0
WireConnection;69;0;70;0
WireConnection;54;0;27;1
WireConnection;54;1;56;0
WireConnection;54;2;69;0
WireConnection;68;0;63;0
WireConnection;68;1;64;0
WireConnection;38;0;37;0
WireConnection;35;0;34;0
WireConnection;36;0;35;0
WireConnection;36;1;38;0
WireConnection;36;2;54;0
WireConnection;72;0;68;0
WireConnection;72;1;59;2
WireConnection;73;0;72;0
WireConnection;39;0;36;0
WireConnection;39;1;40;0
WireConnection;39;2;41;0
WireConnection;39;3;42;0
WireConnection;44;0;39;0
WireConnection;74;0;40;4
WireConnection;74;1;41;4
WireConnection;74;2;54;0
WireConnection;74;3;73;0
WireConnection;43;0;44;0
WireConnection;43;3;74;0
WireConnection;1;0;43;0
ASEEND*/
//CHKSM=ED07BCB7BFF344C0FBEDB2C95A2C57E3562B2E32