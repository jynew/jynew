Shader "Hovl/Particles/Electricity"
{
	Properties
	{
		_MainTexture("Main Texture", 2D) = "white" {}
		_Dissolveamount("Dissolve amount", Range( 0 , 1)) = 0.332
		_Mask("Mask", 2D) = "white" {}
		_Color("Color", Color) = (0.5,0.5,0.5,1)
		_Emission("Emission", Float) = 6
		_RemapXYFresnelZW("Remap XY/Fresnel ZW", Vector) = (-10,10,2,2)
		_Speed("Speed", Vector) = (0.189,0.225,-0.2,-0.05)
		_Opacity("Opacity", Range( 0 , 1)) = 1
		[MaterialToggle] _Usedepth ("Use depth?", Float ) = 0
        _Depth ("Depth", Float ) = 0.15
		[Enum(Cull Off,0, Cull Front,1, Cull Back,2)] _CullMode("Culling", Float) = 2
	}


	Category 
	{
		SubShader
		{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull[_CullMode]
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
					float3 ase_normal : NORMAL;
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
					float4 ase_texcoord4 : TEXCOORD4;
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _Mask;
				uniform float _Dissolveamount;
				uniform sampler2D _MainTexture;
				uniform float4 _Speed;
				uniform float4 _MainTexture_ST;
				uniform float4 _RemapXYFresnelZW;
				uniform float4 _Color;
				uniform float _Emission;
				uniform float _Opacity;
				uniform float _Depth;
				uniform fixed _Usedepth;

				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.ase_texcoord3.xyz = ase_worldPos;
					float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
					o.ase_texcoord4.xyz = ase_worldNormal;
					
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord3.w = 0;
					o.ase_texcoord4.w = 0;

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
					float lp = 1;
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate ((sceneZ-partZ) / _Depth);
						lp *= lerp(1, fade, _Usedepth);
						i.color.a *= lp;
					#endif

					float temp_output_66_0 = (-0.65 + ((1.0 + (_Dissolveamount - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) - 0.0) * (0.65 - -0.65) / (1.0 - 0.0));
					float2 appendResult21 = (float2(_Speed.x , _Speed.y));
					float2 uv0_MainTexture = i.texcoord.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
					float2 appendResult22 = (float2(_Speed.z , _Speed.w));
					float2 appendResult74 = (float2((1.0 + (saturate( (_RemapXYFresnelZW.x + (( ( temp_output_66_0 + tex2D( _MainTexture, ( ( appendResult21 * _Time.y ) + uv0_MainTexture ) ).r ) * ( temp_output_66_0 + tex2D( _MainTexture, ( uv0_MainTexture + ( _Time.y * appendResult22 ) ) ).r ) ) - 0.0) * (_RemapXYFresnelZW.y - _RemapXYFresnelZW.x) / (1.0 - 0.0)) ) - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) , 0.0));
					float temp_output_120_0 = saturate( tex2D( _Mask, appendResult74 ).r );
					float3 ase_worldPos = i.ase_texcoord3.xyz;
					float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
					ase_worldViewDir = normalize(ase_worldViewDir);
					float3 ase_worldNormal = i.ase_texcoord4.xyz;
					float fresnelNdotV83 = dot( ase_worldNormal, ase_worldViewDir );
					float fresnelNode83 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV83, _RemapXYFresnelZW.z ) );
					float clampResult78 = clamp( ( _RemapXYFresnelZW.w * fresnelNode83 ) , 0.0 , 1.0 );
					float4 appendResult116 = (float4((( temp_output_120_0 * _Color * i.color * clampResult78 * _Emission )).rgb , ( temp_output_120_0 * _Color.a * i.color.a * clampResult78 * _Opacity )));
					

					fixed4 col = appendResult116;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	
	
	
}
/*ASEBEGIN
Version=16800
7;87;1906;946;2012.513;546.5009;1.463181;True;False
Node;AmplifyShaderEditor.Vector4Node;15;-3913.605,-171.8944;Float;False;Property;_Speed;Speed;6;0;Create;True;0;0;False;0;0.189,0.225,-0.2,-0.05;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;17;-3582.083,-143.3319;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;21;-3552.2,-233.9622;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-3549.659,-11.68275;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-3162.334,-519.1004;Float;False;Property;_Dissolveamount;Dissolve amount;1;0;Create;True;0;0;False;0;0.332;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;-3336.797,-118.1515;Float;False;0;63;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-3335.527,-232.621;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-3333.084,34.1566;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;63;-3030.421,-182.4538;Float;True;Property;_MainTexture;Main Texture;0;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-3003.552,8.265821;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-3001.65,-282.4343;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;79;-2883.354,-708.1322;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-2728.789,-288.6685;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;66;-2706.937,-518.742;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.65;False;4;FLOAT;0.65;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-2735.894,-64.63338;Float;True;Property;_Noise;Noise;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;68;-2351.046,-162.7447;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;67;-2355.082,-301.6194;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-2188.392,-291.7206;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;71;-2350.525,-37.19666;Float;False;Property;_RemapXYFresnelZW;Remap XY/Fresnel ZW;5;0;Create;True;0;0;False;0;-10,10,2,2;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;70;-2007.525,-299.1967;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.65;False;4;FLOAT;0.65;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;119;-1822.089,-298.6183;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;80;-1662.117,-305.1185;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;74;-1452.87,-297.0031;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FresnelNode;83;-1570.88,299.8762;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-1292.371,-323.9884;Float;True;Property;_Mask;Mask;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-1198.092,221.5643;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;32;-995.74,46.40682;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;31;-1053.741,-123.5932;Float;False;Property;_Color;Color;3;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;52;-978.3959,355.0093;Float;False;Property;_Emission;Emission;4;0;Create;True;0;0;False;0;6;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;120;-971.9929,-299.7244;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;78;-977.2617,218.1655;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-685.2213,-62.59716;Float;False;5;5;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1095.473,461.8331;Float;False;Property;_Opacity;Opacity;7;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;117;-484.4033,-49.63982;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-687.8826,208.5222;Float;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;116;-7.303206,92.0602;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;65;-2876.867,-512.4763;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;115;203.4624,51.85008;Float;False;True;2;Float;;0;7;Hovl/Particles/Electricity;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;21;0;15;1
WireConnection;21;1;15;2
WireConnection;22;0;15;3
WireConnection;22;1;15;4
WireConnection;24;0;21;0
WireConnection;24;1;17;2
WireConnection;23;0;17;2
WireConnection;23;1;22;0
WireConnection;27;0;29;0
WireConnection;27;1;23;0
WireConnection;26;0;24;0
WireConnection;26;1;29;0
WireConnection;79;0;64;0
WireConnection;13;0;63;0
WireConnection;13;1;26;0
WireConnection;66;0;79;0
WireConnection;14;0;63;0
WireConnection;14;1;27;0
WireConnection;68;0;66;0
WireConnection;68;1;14;1
WireConnection;67;0;66;0
WireConnection;67;1;13;1
WireConnection;69;0;67;0
WireConnection;69;1;68;0
WireConnection;70;0;69;0
WireConnection;70;3;71;1
WireConnection;70;4;71;2
WireConnection;119;0;70;0
WireConnection;80;0;119;0
WireConnection;74;0;80;0
WireConnection;83;3;71;3
WireConnection;33;1;74;0
WireConnection;77;0;71;4
WireConnection;77;1;83;0
WireConnection;120;0;33;1
WireConnection;78;0;77;0
WireConnection;51;0;120;0
WireConnection;51;1;31;0
WireConnection;51;2;32;0
WireConnection;51;3;78;0
WireConnection;51;4;52;0
WireConnection;117;0;51;0
WireConnection;61;0;120;0
WireConnection;61;1;31;4
WireConnection;61;2;32;4
WireConnection;61;3;78;0
WireConnection;61;4;62;0
WireConnection;116;0;117;0
WireConnection;116;3;61;0
WireConnection;65;0;64;0
WireConnection;115;0;116;0
ASEEND*/
//CHKSM=7EADAA8A742546B260302387DE2F8033B770B2D8