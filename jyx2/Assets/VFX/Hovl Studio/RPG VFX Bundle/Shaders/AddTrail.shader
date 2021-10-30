Shader "Hovl/Particles/AddTrail"
{
	Properties
	{
		_MainTexture("MainTexture", 2D) = "white" {}
		_SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_StartColor("StartColor", Color) = (1,0,0,1)
		_EndColor("EndColor", Color) = (1,1,0,1)
		_Colorpower("Color power", Float) = 1
		_Colorrange("Color range", Float) = 1
		_Noise("Noise", 2D) = "white" {}
		_Emission("Emission", Float) = 2
		[Toggle]_Usedark("Use dark", Float) = 1
		[Toggle]_Mask("Mask", Float) = 0
		_Maskpower("Mask power", Float) = 10
		[MaterialToggle] _Usedepth ("Use depth?", Float ) = 0
		_Depthpower("Depth power", Float) = 1
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
					
				};	
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform float4 _StartColor;
				uniform float4 _EndColor;
				uniform float _Colorrange;
				uniform float _Colorpower;
				uniform float _Emission;
				uniform float _Usedark;
				uniform sampler2D _MainTexture;
				uniform float4 _SpeedMainTexUVNoiseZW;
				uniform float4 _MainTexture_ST;
				uniform sampler2D _Noise;
				uniform float4 _Noise_ST;
				uniform float _Mask;
				uniform float _Maskpower;
				uniform fixed _Usedepth;
				uniform float _Depthpower;

				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

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
						float fade = saturate ((sceneZ-partZ) / _Depthpower);
						lp *= lerp(1, fade, _Usedepth);
						i.color.a *= lp;
					#endif

					float2 uv01 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float U6 = uv01.x;
					float4 lerpResult3 = lerp( _StartColor , _EndColor , saturate( pow( ( U6 * _Colorrange ) , _Colorpower ) ));
					float4 temp_cast_0 = (1.0).xxxx;
					float2 appendResult32 = (float2(_SpeedMainTexUVNoiseZW.x , _SpeedMainTexUVNoiseZW.y));
					float3 uv0_MainTexture = i.texcoord.xyz;
					uv0_MainTexture.xy = i.texcoord.xyz.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
					float4 Main57 = tex2D( _MainTexture, ( float3( ( appendResult32 * _Time.y ) ,  0.0 ) + uv0_MainTexture + uv0_MainTexture.z ).xy );
					float2 uv0_Noise = i.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
					float2 appendResult29 = (float2(_SpeedMainTexUVNoiseZW.z , _SpeedMainTexUVNoiseZW.w));
					float clampResult44 = clamp( ( pow( ( 1.0 - U6 ) , 0.8 ) * 1.0 ) , 0.2 , 0.6 );
					float4 temp_cast_3 = (U6).xxxx;
					float4 Dissolve49 = saturate( ( ( tex2D( _Noise, ( uv0_Noise + ( _Time.y * appendResult29 ) ) ) + clampResult44 ) - temp_cast_3 ) );
					float V17 = uv01.y;
					float4 temp_output_51_0 = ( i.color.a * Main57 * Dissolve49 * saturate( ( (1.0 + (U6 - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) * (1.0 + (V17 - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) * V17 * 6.0 * lerp(1.0,( U6 * _Maskpower ),_Mask) ) ) );
					float4 appendResult92 = (float4((( ( lerpResult3 * i.color * _Emission ) * lerp(temp_cast_0,temp_output_51_0,_Usedark) )).rgb , temp_output_51_0.r));
					
					fixed4 col = appendResult92;
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
856;287;1326;846;1922.457;-197.5046;1.279268;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-3546.753,397.129;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;28;-3814.819,57.31149;Float;False;Property;_SpeedMainTexUVNoiseZW;Speed MainTex U/V + Noise Z/W;1;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;6;-3298.518,431.3827;Float;False;U;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;29;-3450.873,217.5233;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;30;-3483.297,85.87398;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;42;-3082.569,407.1339;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;40;-3096.845,146.033;Float;False;0;27;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;60;-2904.368,406.8205;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-3096.073,262.2383;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-2730.11,397.7697;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;36;-2845.31,226.1788;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;44;-2577.53,387.1384;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.2;False;2;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;27;-2717.816,199.2953;Float;True;Property;_Noise;Noise;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;32;-3453.414,-4.756103;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;-2349.029,414.1044;Float;False;6;U;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-1689.098,482.2165;Float;False;6;U;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-1674.071,725.4624;Float;False;Property;_Maskpower;Mask power;10;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-2367.688,205.1529;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;56;-3140.282,2.254051;Float;False;0;2;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-3138.373,-91.66324;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-3299.711,498.4168;Float;False;V;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-1480.753,709.4359;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;-1454.562,549.0349;Float;False;17;V;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;7;-1522.872,-236.9327;Float;False;6;U;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;-2854.639,-77.83686;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1510.567,-163.0431;Float;False;Property;_Colorrange;Color range;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;47;-2142.562,207.4056;Float;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-1204.717,610.8511;Float;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;100;-1254.167,681.9261;Float;False;Property;_Mask;Mask;9;0;Create;True;0;0;False;0;0;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1302.603,-154.2804;Float;False;Property;_Colorpower;Color power;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;97;-1934.302,205.2418;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;21;-1237.045,450.0201;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-1275.883,-237.93;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;19;-1240.395,291.7538;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2714.319,-84.08413;Float;True;Property;_MainTexture;MainTexture;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;-2390.7,-83.00887;Float;False;Main;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-985.4469,403.7144;Float;True;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;10;-1114.394,-237.9301;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-1781.332,201.958;Float;False;Dissolve;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-768.0624,220.6713;Float;False;57;Main;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;15;-707.296,-286.4251;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;4;-1099.159,-592.153;Float;False;Property;_StartColor;StartColor;2;0;Create;True;0;0;False;0;1,0,0,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;96;-751.6993,398.7313;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;98;-940.9728,-239.1694;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;5;-1095.667,-412.8965;Float;False;Property;_EndColor;EndColor;3;0;Create;True;0;0;False;0;1,1,0,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;50;-769.4026,297.0116;Float;False;49;Dissolve;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-501.945,204.2904;Float;True;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;3;-765.0906,-495.5406;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-680.9393,-131.6054;Float;False;Property;_Emission;Emission;7;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-404.9611,48.36441;Float;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;99;-247.7693,54.92614;Float;False;Property;_Usedark;Use dark;8;0;Create;True;0;0;False;0;1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-436.6116,-130.9249;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-22.73198,-30.14677;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;93;263.301,96.88125;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;92;495.0742,154.525;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;38;-3298.052,358.0942;Float;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;94;769.7286,153.8613;Float;False;True;2;Float;;0;11;Hovl/Particles/AddTrail;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;6;0;1;1
WireConnection;29;0;28;3
WireConnection;29;1;28;4
WireConnection;42;0;6;0
WireConnection;60;0;42;0
WireConnection;35;0;30;2
WireConnection;35;1;29;0
WireConnection;59;0;60;0
WireConnection;36;0;40;0
WireConnection;36;1;35;0
WireConnection;44;0;59;0
WireConnection;27;1;36;0
WireConnection;32;0;28;1
WireConnection;32;1;28;2
WireConnection;45;0;27;0
WireConnection;45;1;44;0
WireConnection;33;0;32;0
WireConnection;33;1;30;2
WireConnection;17;0;1;2
WireConnection;103;0;20;0
WireConnection;103;1;101;0
WireConnection;37;0;33;0
WireConnection;37;1;56;0
WireConnection;37;2;56;3
WireConnection;47;0;45;0
WireConnection;47;1;41;0
WireConnection;100;1;103;0
WireConnection;97;0;47;0
WireConnection;21;0;18;0
WireConnection;9;0;7;0
WireConnection;9;1;8;0
WireConnection;19;0;20;0
WireConnection;2;1;37;0
WireConnection;57;0;2;0
WireConnection;22;0;19;0
WireConnection;22;1;21;0
WireConnection;22;2;18;0
WireConnection;22;3;23;0
WireConnection;22;4;100;0
WireConnection;10;0;9;0
WireConnection;10;1;12;0
WireConnection;49;0;97;0
WireConnection;96;0;22;0
WireConnection;98;0;10;0
WireConnection;51;0;15;4
WireConnection;51;1;58;0
WireConnection;51;2;50;0
WireConnection;51;3;96;0
WireConnection;3;0;4;0
WireConnection;3;1;5;0
WireConnection;3;2;98;0
WireConnection;99;0;53;0
WireConnection;99;1;51;0
WireConnection;14;0;3;0
WireConnection;14;1;15;0
WireConnection;14;2;16;0
WireConnection;54;0;14;0
WireConnection;54;1;99;0
WireConnection;93;0;54;0
WireConnection;92;0;93;0
WireConnection;92;3;51;0
WireConnection;38;0;1;0
WireConnection;94;0;92;0
ASEEND*/
//CHKSM=3132C082A0FC43B6B258894D65CC5B29C8204273