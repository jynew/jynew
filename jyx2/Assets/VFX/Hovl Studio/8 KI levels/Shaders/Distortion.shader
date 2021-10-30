Shader "Hovl/Particles/Distortion"
{
	Properties
	{
		_NormalMap("Normal Map", 2D) = "bump" {}
		_Distortionpower("Distortion power", Float) = 0.05
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	}

	Category 
	{
		SubShader
		{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting Off 
			ZWrite Off
			Fog { Mode Off}
			GrabPass{ }

			Pass {		
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_particles
				#include "UnityCG.cginc"
				uniform sampler2D_float _CameraDepthTexture;
				uniform float _InvFade;
				uniform sampler2D _GrabTexture;
				uniform sampler2D _NormalMap;
				uniform float4 _NormalMap_ST;
				uniform float _Distortionpower;	
				uniform float4 _GrabTexture_TexelSize;	

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;				
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD1;
					float2 texcoord2 : TEXCOORD2;
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD3;
					#endif
				};			

				v2f vert ( appdata_t v  )
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					
					#if UNITY_UV_STARTS_AT_TOP
					half scale = -1.0;
					#else
					half scale = 1.0;
					#endif
					o.texcoord.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
					o.texcoord.zw = o.vertex.w;					
					#if UNITY_SINGLE_PASS_STEREO
					o.texcoord.xy = TransformStereoScreenSpaceTex(o.texcoord.xy, o.texcoord.w);
					#endif
					o.texcoord.z /= distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					o.texcoord2 = TRANSFORM_TEX( v.texcoord, _NormalMap );
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

					half3 tex2DNode14 = UnpackNormal(tex2D( _NormalMap, i.texcoord2));
					half2 screenColor29 = tex2DNode14.rg;
					half clampResult89 = (abs(tex2DNode14.r) + abs(tex2DNode14.g) * 30) - 0.03;
					screenColor29 = screenColor29 * _GrabTexture_TexelSize.xy * _Distortionpower * i.color.a;
					i.texcoord.xy = screenColor29 * i.texcoord.z + i.texcoord.xy;
					half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.texcoord));
					col.a = saturate(col.a * clampResult89);
					return col;
				}
				ENDCG 
			}
		}	
	}	
}
/*ASEBEGIN
Version=15401
764;92;877;655;1734.839;1034.898;1.545907;True;True
Node;AmplifyShaderEditor.SamplerNode;14;-1463.593,-612.4862;Float;True;Property;_NormalMap;Normal Map;1;0;Create;True;0;0;False;0;c77ad51b9c5e4a440b6c122953ce0dfc;c77ad51b9c5e4a440b6c122953ce0dfc;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;79;-1125.91,-499.2636;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;101;-1130.615,-262.5767;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-837.6768,-190.3634;Float;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;False;0;30;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-897.4952,-501.049;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-654.9815,-422.7797;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-613.9226,-177.3736;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;81;-396.7346,-359.6836;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;36;-480.6686,-614.95;Float;True;True;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-240.5315,-514.8548;Float;False;Property;_Distortionpower;Distortion power;0;0;Create;True;0;0;False;0;0.05;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;89;-164.2401,-354.9835;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;85;-763.2303,-813.5652;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;3.543352,-587.3339;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;86;-430.4041,-832.9164;Float;True;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;87;163.4362,-654.8883;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;104;501.661,-611.1464;Float;False;True;True;True;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScreenColorNode;29;751.0516,-829.6149;Float;False;Global;_GrabScreen0;Grab Screen 0;4;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;49;750.7663,-657.1718;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;1001.457,-721.0015;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;69;1175.819,-706.8605;Float;False;True;2;Float;ASEMaterialInspector;0;6;Distortion2;0b6a9f8b4f707c74ca64c0be8e590de0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent;IgnoreProjector=True;RenderType=Transparent;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;True;0;0;;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;79;0;14;1
WireConnection;101;0;14;2
WireConnection;80;0;79;0
WireConnection;80;1;101;0
WireConnection;102;0;80;0
WireConnection;102;1;84;0
WireConnection;81;0;102;0
WireConnection;81;1;82;0
WireConnection;36;0;14;0
WireConnection;89;0;81;0
WireConnection;91;0;36;0
WireConnection;91;1;12;0
WireConnection;91;2;89;0
WireConnection;86;0;85;0
WireConnection;87;0;86;0
WireConnection;87;1;91;0
WireConnection;104;0;87;0
WireConnection;29;0;104;0
WireConnection;97;0;29;0
WireConnection;97;1;49;0
WireConnection;69;0;97;0
ASEEND*/
//CHKSM=E071BF2A900CD0332D04C068E64CF91BB363761C