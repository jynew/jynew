Shader "Hovl/Particles/DissolveNoise"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_TextureNoise("Texture Noise", 2D) = "white" {}
		_Dissolvenoise("Dissolve noise", 2D) = "white" {}
		_NoisespeedXYEmissonZPowerW("Noise speed XY / Emisson Z / Power W", Vector) = (0.5,0,2,1)
		_DissolvespeedXY("Dissolve speed XY", Vector) = (0,0,0,0)
		_Maincolor("Main color", Color) = (0.7609469,0.8547776,0.9433962,1)
		_Noisecolor("Noise color", Color) = (0.2470588,0.3012382,0.3607843,1)
		_Dissolvecolor("Dissolve color", Color) = (1,1,1,1)
		[Toggle]_Usetexturecolor("Use texture color", Float) = 0
		[Toggle]_Usetexturedissolve("Use texture dissolve", Float) = 0
		_Opacity("Opacity", Range( 0 , 1)) = 1
		[Toggle] _Usedepth ("Use depth?", Float ) = 0
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
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

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform float _Usedepth;
				uniform float4 _NoisespeedXYEmissonZPowerW;
				uniform float _Usetexturecolor;
				uniform float4 _Maincolor;
				uniform float4 _Noisecolor;
				uniform sampler2D _TextureNoise;
				uniform sampler2D _Dissolvenoise;
				uniform float4 _Dissolvenoise_ST;
				uniform float4 _TextureNoise_ST;
				uniform float _Usetexturedissolve;
				uniform float4 _DissolvespeedXY;
				uniform float4 _Dissolvecolor;
				uniform float _Opacity;

				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					o.ase_texcoord3.xyz = v.ase_texcoord1.xyz;
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord3.w = 0;

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

					float Emission59 = _NoisespeedXYEmissonZPowerW.z;
					float2 appendResult38 = (float2(_NoisespeedXYEmissonZPowerW.x , _NoisespeedXYEmissonZPowerW.y));
					float3 uv1_Dissolvenoise = i.ase_texcoord3.xyz;
					uv1_Dissolvenoise.xy = i.ase_texcoord3.xyz.xy * _Dissolvenoise_ST.xy + _Dissolvenoise_ST.zw;
					float W120 = uv1_Dissolvenoise.z;
					float4 uv0_TextureNoise = i.texcoord;
					uv0_TextureNoise.xy = i.texcoord.xy * _TextureNoise_ST.xy + _TextureNoise_ST.zw;
					float2 panner39 = ( 1.0 * _Time.y * appendResult38 + ( W120 + float2( 0.2,0.4 ) + (uv0_TextureNoise).xy ));
					float Noisepower63 = _NoisespeedXYEmissonZPowerW.w;
					float4 temp_cast_0 = (Noisepower63).xxxx;
					float4 clampResult11 = clamp( ( pow( tex2D( _TextureNoise, panner39 ) , temp_cast_0 ) * Noisepower63 ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
					float4 lerpResult8 = lerp( _Maincolor , _Noisecolor , clampResult11);
					float2 appendResult109 = (float2(_DissolvespeedXY.x , _DissolvespeedXY.y));
					float2 panner111 = ( 1.0 * _Time.y * appendResult109 + ( (uv1_Dissolvenoise).xy + W120 ));
					float4 tex2DNode91 = tex2D( _Dissolvenoise, panner111 );
					float2 uv_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					float4 tex2DNode4 = tex2D( _MainTex, uv_MainTex );
					float mainTexr123 = tex2DNode4.r;
					float temp_output_88_0 = step( lerp(tex2DNode91.r,( tex2DNode91.r * mainTexr123 ),_Usetexturedissolve) , uv0_TextureNoise.z );
					float4 temp_output_93_0 = ( lerpResult8 * ( 1.0 - temp_output_88_0 ) );
					float clampResult87 = clamp( ( (-4.0 + (( (-0.65 + (( 1.0 - uv0_TextureNoise.z ) - 0.0) * (0.65 - -0.65) / (1.0 - 0.0)) + lerp(tex2DNode91.r,( tex2DNode91.r * mainTexr123 ),_Usetexturedissolve) ) - 0.0) * (7.0 - -4.0) / (1.0 - 0.0)) * 3.0 ) , 0.0 , 1.0 );
					float4 lerpResult92 = lerp( lerp(temp_output_93_0,( temp_output_93_0 * tex2DNode4 ),_Usetexturecolor) , lerp(_Dissolvecolor,( _Dissolvecolor * tex2DNode4 ),_Usetexturecolor) , ( clampResult87 * temp_output_88_0 ));
					float clampResult99 = clamp( (-15.0 + (( lerp(tex2DNode91.r,( tex2DNode91.r * mainTexr123 ),_Usetexturedissolve) + (-0.65 + (uv0_TextureNoise.w - 0.0) * (0.65 - -0.65) / (1.0 - 0.0)) ) - 0.0) * (15.0 - -15.0) / (1.0 - 0.0)) , 0.0 , 1.0 );
					float4 appendResult2 = (float4((( Emission59 * lerpResult92 * i.color )).rgb , ( i.color.a * tex2DNode4.a * clampResult99 * _Opacity )));
					
					fixed4 col = appendResult2;
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
452;190;1205;843;1417.795;416.7184;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;110;-3731.085,62.61617;Float;False;1;91;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;96;-4375.368,-929.1308;Float;False;2523.071;702.9789;Noise emission;17;37;40;38;65;39;63;64;3;9;12;6;11;7;8;59;119;121;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;120;-3497.433,135.6656;Float;False;W;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;40;-4325.368,-688.4302;Float;False;0;3;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;118;-3492.551,61.89781;Float;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;113;-3496.907,209.6091;Float;False;Property;_DissolvespeedXY;Dissolve speed XY;4;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;37;-4088.941,-624.0703;Float;False;Property;_NoisespeedXYEmissonZPowerW;Noise speed XY / Emisson Z / Power W;3;0;Create;True;0;0;False;0;0.5,0,2,1;0.5,0,1.5,3;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;65;-4007.662,-701.4861;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;-3976.682,-780.9033;Float;False;120;W;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;117;-3268.184,124.0004;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;109;-3284.167,219.7076;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;38;-3771.349,-612.1542;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;119;-3753.854,-740.6059;Float;False;3;3;0;FLOAT;0;False;1;FLOAT2;0.2,0.4;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;4;-1407.331,460.5456;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;52187efe2e15f22438ea18da0388faf9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;111;-3138.163,161.1251;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;-3765.858,-446.4271;Float;False;Noisepower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;39;-3590.439,-664.4094;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;123;-1081.101,483.4396;Float;False;mainTexr;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;124;-2849.281,366.0172;Float;False;123;mainTexr;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-3352.115,-661.3746;Float;True;Property;_TextureNoise;Texture Noise;1;0;Create;True;0;0;False;0;None;04a40b50e9e63ed43af8af28f2ba4f86;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;103;-3664.961,-99.41174;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;91;-2944.553,180.0912;Float;True;Property;_Dissolvenoise;Dissolve noise;2;0;Create;True;0;0;False;0;None;04a40b50e9e63ed43af8af28f2ba4f86;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;64;-3062.491,-360.0853;Float;False;63;Noisepower;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;-2655.502,278.2065;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;89;-2601.746,30.60856;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;9;-2834.707,-531.7649;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;84;-2426.78,29.58903;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.65;False;4;FLOAT;0.65;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-2626.456,-532.4448;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;122;-2512.364,201.0435;Float;False;Property;_Usetexturedissolve;Use texture dissolve;10;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;104;-3568.484,373.0246;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;88;-1897.113,360.4829;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;-2525.469,-710.7628;Float;False;Property;_Noisecolor;Noise color;6;0;Create;True;0;0;False;0;0.2470588,0.3012382,0.3607843,1;0.6084906,0.7078456,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;11;-2430.626,-539.9247;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;6;-2522.28,-879.1308;Float;False;Property;_Maincolor;Main color;5;0;Create;True;0;0;False;0;0.7609469,0.8547776,0.9433962,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;85;-2212.658,30.22906;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;8;-2036.292,-749.175;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;94;-1629.423,-205.6722;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;86;-2061.677,36.23553;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-4;False;4;FLOAT;7;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-1377.25,-220.3437;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;95;-1400.845,-10.83905;Float;False;Property;_Dissolvecolor;Dissolve color;7;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;102;-3549.196,605.6983;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-1832.965,37.59949;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;126;-1879.118,642.0746;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;87;-1650.216,36.16174;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1017.75,-93.77279;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-1011.594,84.85248;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;101;-1005.377,757.6221;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.65;False;4;FLOAT;0.65;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;75;-839.7263,-226.4786;Float;False;Property;_Usetexturecolor;Use texture color;9;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;76;-860.1625,-0.6898842;Float;False;Property;_Usetexturecolor;Use texture color;8;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-1421.17,230.6805;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;-766.1468,655.4361;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;59;-3766.757,-520.1385;Float;False;Emission;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;57;-49.6262,197.8381;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;60;-89.6088,-126.0067;Float;False;59;Emission;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;92;-408.2168,-28.06238;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;98;-559.383,652.7811;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-15;False;4;FLOAT;15;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;201.1051,-35.90836;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-261.7072,780.2939;Float;False;Property;_Opacity;Opacity;11;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;99;-276.761,649.803;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;5;387.0235,-14.18318;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;239.3105,376.0516;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;2;639.802,0.6767869;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;116;868.134,5.849471;Float;False;True;2;Float;;0;11;Hovl/Particles/DissolveNoise;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;120;0;110;3
WireConnection;118;0;110;0
WireConnection;65;0;40;0
WireConnection;117;0;118;0
WireConnection;117;1;120;0
WireConnection;109;0;113;1
WireConnection;109;1;113;2
WireConnection;38;0;37;1
WireConnection;38;1;37;2
WireConnection;119;0;121;0
WireConnection;119;2;65;0
WireConnection;111;0;117;0
WireConnection;111;2;109;0
WireConnection;63;0;37;4
WireConnection;39;0;119;0
WireConnection;39;2;38;0
WireConnection;123;0;4;1
WireConnection;3;1;39;0
WireConnection;103;0;40;3
WireConnection;91;1;111;0
WireConnection;125;0;91;1
WireConnection;125;1;124;0
WireConnection;89;0;103;0
WireConnection;9;0;3;0
WireConnection;9;1;64;0
WireConnection;84;0;89;0
WireConnection;12;0;9;0
WireConnection;12;1;64;0
WireConnection;122;0;91;1
WireConnection;122;1;125;0
WireConnection;104;0;40;3
WireConnection;88;0;122;0
WireConnection;88;1;104;0
WireConnection;11;0;12;0
WireConnection;85;0;84;0
WireConnection;85;1;122;0
WireConnection;8;0;6;0
WireConnection;8;1;7;0
WireConnection;8;2;11;0
WireConnection;94;0;88;0
WireConnection;86;0;85;0
WireConnection;93;0;8;0
WireConnection;93;1;94;0
WireConnection;102;0;40;4
WireConnection;97;0;86;0
WireConnection;126;0;122;0
WireConnection;87;0;97;0
WireConnection;74;0;93;0
WireConnection;74;1;4;0
WireConnection;80;0;95;0
WireConnection;80;1;4;0
WireConnection;101;0;102;0
WireConnection;75;0;93;0
WireConnection;75;1;74;0
WireConnection;76;0;95;0
WireConnection;76;1;80;0
WireConnection;90;0;87;0
WireConnection;90;1;88;0
WireConnection;100;0;126;0
WireConnection;100;1;101;0
WireConnection;59;0;37;3
WireConnection;92;0;75;0
WireConnection;92;1;76;0
WireConnection;92;2;90;0
WireConnection;98;0;100;0
WireConnection;58;0;60;0
WireConnection;58;1;92;0
WireConnection;58;2;57;0
WireConnection;99;0;98;0
WireConnection;5;0;58;0
WireConnection;41;0;57;4
WireConnection;41;1;4;4
WireConnection;41;2;99;0
WireConnection;41;3;127;0
WireConnection;2;0;5;0
WireConnection;2;3;41;0
WireConnection;116;0;2;0
ASEEND*/
//CHKSM=1CE1EE6B6A3D0D28DFE1537BFEC0A364D30C2704