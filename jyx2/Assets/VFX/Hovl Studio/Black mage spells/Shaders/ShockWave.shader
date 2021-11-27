Shader "Hovl/Particles/ShockWave"
{
	Properties
	{
		[MaterialToggle] _Usedepth ("Use depth?", Float ) = 0
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_MainTexture("Main Texture", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Flow("Flow", 2D) = "white" {}
		_DistortionSpeedXYPowerZ("Distortion Speed XY Power Z", Vector) = (0,0,0,0)
		_NoiseSpeedXYPowerZ("Noise Speed XY Power Z", Vector) = (0,0,1,0)
		_Emission("Emission", Float) = 2
		_Mask("Mask", 2D) = "white" {}
		_NoiseOpacityLerp("Noise Opacity Lerp", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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

				uniform fixed _Usedepth;
				uniform float _InvFade;
				uniform float _Emission;
				uniform sampler2D _Noise;
				uniform float4 _NoiseSpeedXYPowerZ;
				uniform float4 _Noise_ST;
				uniform float _NoiseOpacityLerp;
				uniform sampler2D _MainTexture;
				uniform sampler2D _Flow;
				uniform float4 _DistortionSpeedXYPowerZ;
				uniform float4 _Flow_ST;
				uniform float4 _MainTexture_ST;
				uniform sampler2D _Mask;
				uniform float4 _Mask_ST;

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
					float lp = 1;
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						lp *= lerp(1, fade, _Usedepth);
						i.color.a *= lp;
					#endif

					float2 appendResult49 = (float2(_NoiseSpeedXYPowerZ.x , _NoiseSpeedXYPowerZ.y));
					float2 uv0_Noise = i.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
					float2 panner50 = ( 1.0 * _Time.y * appendResult49 + (uv0_Noise).xy);
					float4 tex2DNode45 = tex2D( _Noise, panner50 );
					float lerpResult81 = lerp( tex2DNode45.r , 1.0 , _NoiseOpacityLerp);
					float temp_output_66_0 = (( 1.0 - _NoiseSpeedXYPowerZ.z ) + (lerpResult81 - 0.0) * (_NoiseSpeedXYPowerZ.z - ( 1.0 - _NoiseSpeedXYPowerZ.z )) / (1.0 - 0.0));
					float2 appendResult29 = (float2(_DistortionSpeedXYPowerZ.x , _DistortionSpeedXYPowerZ.y));
					float4 uv1_Flow = i.ase_texcoord3;
					uv1_Flow.xy = i.ase_texcoord3.xy * _Flow_ST.xy + _Flow_ST.zw;
					float2 panner31 = ( 1.0 * _Time.y * appendResult29 + (uv1_Flow).xy);
					float Flowpower33 = _DistortionSpeedXYPowerZ.z;
					float4 uv0_MainTexture = i.texcoord;
					uv0_MainTexture.xy = i.texcoord.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
					float2 appendResult27 = (float2(uv0_MainTexture.z , uv0_MainTexture.w));
					float2 temp_output_11_0 = ( (uv0_MainTexture).xy + appendResult27 );
					float4 tex2DNode2 = tex2D( _MainTexture, ( ( (tex2D( _Flow, panner31 )).rg * Flowpower33 ) + temp_output_11_0 ) );
					float T75 = uv1_Flow.w;
					float4 temp_cast_0 = (T75).xxxx;
					float2 break15 = temp_output_11_0;
					float ifLocalVar18 = 0;
					if( 1.0 == ceil( break15.x ) )
					ifLocalVar18 = 0.0;
					else
					ifLocalVar18 = 1.0;
					float ifLocalVar19 = 0;
					if( 1.0 == ceil( break15.y ) )
					ifLocalVar19 = 0.0;
					else
					ifLocalVar19 = 1.0;
					float2 uv_Mask = i.texcoord.xy * _Mask_ST.xy + _Mask_ST.zw;
					float W70 = uv1_Flow.z;
					float4 appendResult3 = (float4(( _Emission * ( temp_output_66_0 * saturate( pow( tex2DNode2 , temp_cast_0 ) ) ) * i.color * tex2DNode45 ).rgb , saturate( ( ( tex2DNode2.a * ( 1.0 - max( ifLocalVar18 , ifLocalVar19 ) ) * i.color.a * tex2D( _Mask, uv_Mask ).a * temp_output_66_0 ) * W70 ) )));
					
					fixed4 col = appendResult3;
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
669;82;1906;1004;1549.157;1064.764;1.3;True;False
Node;AmplifyShaderEditor.Vector4Node;28;-2996.185,-380.9642;Float;False;Property;_DistortionSpeedXYPowerZ;Distortion Speed XY Power Z;3;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;39;-2939.424,-544.927;Float;False;1;32;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;30;-2612.881,-431.1774;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;29;-2542.003,-360.5725;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-2165.69,-99.53895;Float;False;0;2;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;31;-2368.475,-401.1438;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;27;-1878.088,-25.08011;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;26;-1919.608,-102.1762;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;46;-2011.202,-636.227;Float;False;Property;_NoiseSpeedXYPowerZ;Noise Speed XY Power Z;4;0;Create;True;0;0;False;0;0,0,1,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;-2037.707,-752.6404;Float;False;0;45;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-2614.632,-267.7177;Float;False;Flowpower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;32;-2182.217,-429.697;Float;True;Property;_Flow;Flow;2;0;Create;True;0;0;False;0;None;61c0b9c0523734e0e91bc6043c72a490;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-1696.929,-98.51338;Float;True;2;2;0;FLOAT2;-0.5,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;48;-1716.95,-686.6091;Float;False;True;True;False;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;49;-1646.071,-616.0042;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;15;-1479.739,-98.09904;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;36;-1835.169,-343.2113;Float;False;33;Flowpower;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;35;-1840.025,-430.5372;Float;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1619.096,-431.5284;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-987.0012,-130.7295;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;50;-1460.412,-680.8436;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-988.0012,-59.72949;Float;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;17;-1184.57,-31.67378;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;16;-1180.116,-246.2539;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-2616.226,-502.9784;Float;False;T;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-1178.645,-821.423;Float;False;Property;_NoiseOpacityLerp;Noise Opacity Lerp;7;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;45;-1253.117,-743.7463;Float;True;Property;_Noise;Noise;1;0;Create;True;0;0;False;0;None;4cdecdfbc6f2fa34689b6691b5d55879;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;19;-709.0012,-51.72949;Float;True;False;5;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;38;-1399.431,-430.4391;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ConditionalIfNode;18;-714.7638,-265.8079;Float;True;False;5;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;67;-1119.448,-552.2353;Float;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;81;-929.8432,-670.223;Float;False;3;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1262.116,-458.7045;Float;True;Property;_MainTexture;Main Texture;0;0;Create;True;0;0;False;0;None;4cdecdfbc6f2fa34689b6691b5d55879;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;76;-970.1829,-384.421;Float;False;75;T;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;23;-405.2362,-162.1472;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;25;-174.4099,-160.1449;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;40;-69.31241,-428.1146;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;66;-756.352,-656.7134;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;74;-728.9283,-453.3492;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-2621.827,-583.4098;Float;False;W;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;43;-400.7015,68.87635;Float;True;Property;_Mask;Mask;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;78;-563.4968,-454.7295;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;158.5059,-62.45157;Float;False;70;W;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;136.674,-273.2871;Float;True;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;434.0134,-273.0399;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-60.36459,-542.7811;Float;False;Property;_Emission;Emission;5;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-386.063,-501.2477;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;218.4014,-519.3336;Float;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;73;640.913,-270.2352;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;3;830.7225,-433.4673;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1076.553,-434.6862;Float;False;True;2;Float;;0;11;Hovl/Particles/ShockWave;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;30;0;39;0
WireConnection;29;0;28;1
WireConnection;29;1;28;2
WireConnection;31;0;30;0
WireConnection;31;2;29;0
WireConnection;27;0;4;3
WireConnection;27;1;4;4
WireConnection;26;0;4;0
WireConnection;33;0;28;3
WireConnection;32;1;31;0
WireConnection;11;0;26;0
WireConnection;11;1;27;0
WireConnection;48;0;47;0
WireConnection;49;0;46;1
WireConnection;49;1;46;2
WireConnection;15;0;11;0
WireConnection;35;0;32;0
WireConnection;37;0;35;0
WireConnection;37;1;36;0
WireConnection;50;0;48;0
WireConnection;50;2;49;0
WireConnection;17;0;15;1
WireConnection;16;0;15;0
WireConnection;75;0;39;4
WireConnection;45;1;50;0
WireConnection;19;0;22;0
WireConnection;19;1;17;0
WireConnection;19;2;22;0
WireConnection;19;3;21;0
WireConnection;19;4;22;0
WireConnection;38;0;37;0
WireConnection;38;1;11;0
WireConnection;18;0;22;0
WireConnection;18;1;16;0
WireConnection;18;2;22;0
WireConnection;18;3;21;0
WireConnection;18;4;22;0
WireConnection;67;1;46;3
WireConnection;81;0;45;1
WireConnection;81;2;82;0
WireConnection;2;1;38;0
WireConnection;23;0;18;0
WireConnection;23;1;19;0
WireConnection;25;0;23;0
WireConnection;66;0;81;0
WireConnection;66;3;67;0
WireConnection;66;4;46;3
WireConnection;74;0;2;0
WireConnection;74;1;76;0
WireConnection;70;0;39;3
WireConnection;78;0;74;0
WireConnection;24;0;2;4
WireConnection;24;1;25;0
WireConnection;24;2;40;4
WireConnection;24;3;43;4
WireConnection;24;4;66;0
WireConnection;72;0;24;0
WireConnection;72;1;71;0
WireConnection;44;0;66;0
WireConnection;44;1;78;0
WireConnection;41;0;42;0
WireConnection;41;1;44;0
WireConnection;41;2;40;0
WireConnection;41;3;45;0
WireConnection;73;0;72;0
WireConnection;3;0;41;0
WireConnection;3;3;73;0
WireConnection;1;0;3;0
ASEEND*/
//CHKSM=38AEE92864EC09BF4AEE18CCE8F3670E44F02816