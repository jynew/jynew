Shader "Hovl/Particles/Blend_LinePath"
{
	Properties
	{
		[Toggle] _Usedepth ("Use depth?", Float ) = 0
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_MainTex("MainTex", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Color("Color", Color) = (0.5,0.5,0.5,1)
		_Emission("Emission", Float) = 2
		_LenghtSet1ifyouuseinPS("Lenght(Set 1 if you use in PS)", Range( 0 , 1)) = 0
		_PathSet0ifyouuseinPS("Path(Set 0 if you use in PS)", Range( 0 , 1)) = 0
		[Toggle]_Movenoise("Move noise", Float) = 1
		_Opacity("Opacity", Range( 0 , 3)) = 1
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

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform float _PathSet0ifyouuseinPS;
				uniform float _LenghtSet1ifyouuseinPS;
				uniform sampler2D _Noise;
				uniform float _Movenoise;
				uniform float4 _Noise_ST;
				uniform float4 _Color;
				uniform float _Emission;
				uniform float _Opacity;
				uniform fixed _Usedepth;

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
						float fade = saturate ((sceneZ-partZ) / _InvFade);
						lp *= lerp(1, fade, _Usedepth);
						i.color.a *= lp;
					#endif

					float4 uv069 = i.texcoord;
					uv069.xy = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float temp_output_98_0 = (2.5 + (( _PathSet0ifyouuseinPS + uv069.z ) - 0.0) * (1.0 - 2.5) / (1.0 - 0.0));
					float temp_output_102_0 = (1.0 + (( uv069.w * _LenghtSet1ifyouuseinPS ) - 0.0) * (0.0 - 1.0) / (1.0 - 0.0));
					float clampResult107 = clamp( ( ( ( temp_output_98_0 * temp_output_98_0 * temp_output_98_0 * temp_output_98_0 ) * uv069.x ) - temp_output_102_0 ) , 0.0 , 1.0 );
					float2 appendResult109 = (float2(( clampResult107 * rsqrt( (1.0 + (temp_output_102_0 - 0.0) * (0.001 - 1.0) / (1.0 - 0.0)) ) ) , uv069.y));
					float2 clampResult85 = clamp( appendResult109 , float2( 0,0 ) , float2( 1,1 ) );
					float4 tex2DNode23 = tex2D( _MainTex, clampResult85 );
					float2 uv0_Noise = i.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
					float4 tex2DNode24 = tex2D( _Noise, lerp(uv0_Noise,( clampResult85 * uv0_Noise ),_Movenoise) );
					float4 appendResult110 = (float4(( (( tex2DNode23 * tex2DNode24 * _Color * i.color )).rgb * _Emission ) , ( ( tex2DNode23.a * tex2DNode24.a * _Opacity ) * _Color.a * i.color.a )));
					
					fixed4 col = appendResult110;
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
535;187;1019;673;2749.968;540.7341;1.3;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;69;-4134.208,-63.6925;Float;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;91;-4188.811,-193.4627;Float;False;Property;_PathSet0ifyouuseinPS;Path(Set 0 if you use in PS);5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;97;-3833.998,-185.869;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-4048.646,137.1324;Float;False;Property;_LenghtSet1ifyouuseinPS;Lenght(Set 1 if you use in PS);4;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;98;-3642.908,-227.0266;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;2.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-3695.988,60.25461;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-3444.471,-247.6052;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;102;-3434.708,32.9898;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-3287.028,-227.607;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;101;-3097.418,-231.9082;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;105;-3148.25,12.15276;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0.001;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;107;-2899.279,-233.9683;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RSqrtOpNode;106;-2913,13.0967;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;108;-2716.752,-171.0533;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;109;-2527.872,-138.1581;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;113;-2336.336,-42.11392;Float;False;0;24;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;85;-2307.945,-207.1241;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-2082.962,-118.5871;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ToggleSwitchNode;117;-1863.844,-45.40438;Float;False;Property;_Movenoise;Move noise;6;0;Create;True;0;0;False;0;1;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VertexColorNode;22;-1501.337,308.0427;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;24;-1635.304,-58.79362;Float;True;Property;_Noise;Noise;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;25;-1559.337,138.0429;Float;False;Property;_Color;Color;2;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;-1633.917,-244.2309;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;1310c389b00742944a196851ef347845;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;115;-1545.108,488.2652;Float;False;Property;_Opacity;Opacity;7;0;Create;True;0;0;False;0;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-968.3528,7.087863;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;112;-697.4847,1.551311;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-412.6612,110.3443;Float;False;Property;_Emission;Emission;3;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-965.6019,185.7662;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;-638.7333,300.2067;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-176.6617,14.34432;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;110;214.1178,87.42996;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;111;422.4361,82.68047;Float;False;True;2;Float;;0;11;Hovl/Particles/Blend_LinePath;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;97;0;91;0
WireConnection;97;1;69;3
WireConnection;98;0;97;0
WireConnection;104;0;69;4
WireConnection;104;1;103;0
WireConnection;99;0;98;0
WireConnection;99;1;98;0
WireConnection;99;2;98;0
WireConnection;99;3;98;0
WireConnection;102;0;104;0
WireConnection;100;0;99;0
WireConnection;100;1;69;1
WireConnection;101;0;100;0
WireConnection;101;1;102;0
WireConnection;105;0;102;0
WireConnection;107;0;101;0
WireConnection;106;0;105;0
WireConnection;108;0;107;0
WireConnection;108;1;106;0
WireConnection;109;0;108;0
WireConnection;109;1;69;2
WireConnection;85;0;109;0
WireConnection;114;0;85;0
WireConnection;114;1;113;0
WireConnection;117;0;113;0
WireConnection;117;1;114;0
WireConnection;24;1;117;0
WireConnection;23;1;85;0
WireConnection;30;0;23;0
WireConnection;30;1;24;0
WireConnection;30;2;25;0
WireConnection;30;3;22;0
WireConnection;112;0;30;0
WireConnection;26;0;23;4
WireConnection;26;1;24;4
WireConnection;26;2;115;0
WireConnection;116;0;26;0
WireConnection;116;1;25;4
WireConnection;116;2;22;4
WireConnection;21;0;112;0
WireConnection;21;1;33;0
WireConnection;110;0;21;0
WireConnection;110;3;116;0
WireConnection;111;0;110;0
ASEEND*/
//CHKSM=7ECB1E5498AFBA85CFE552B894B4AE53EA3C0263