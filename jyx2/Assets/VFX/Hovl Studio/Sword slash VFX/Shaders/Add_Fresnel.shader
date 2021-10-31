Shader "Hovl/Particles/Add_Fresnel"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Color("Color", Color) = (0.5,0.5,0.5,1)
		_Emission("Emission", Float) = 2
		_SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_Flow("Flow", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_Distortionpower("Distortion power", Float) = 0.2
		_Fresnelscale("Fresnel scale", Float) = 3
		_Fresnelpower("Fresnel power", Float) = 3
		_Depthpower("Depth power", Float) = 0.2
		[Toggle]_Useonlycolor("Use only color", Float) = 0
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
				//#pragma target 2.0
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
					UNITY_VERTEX_OUTPUT_STEREO
					float4 ase_texcoord3 : TEXCOORD3;
					float4 ase_texcoord4 : TEXCOORD4;
					//float4 ase_texcoord5 : TEXCOORD5;
				};
				
				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform sampler2D_float _CameraDepthTexture;
				uniform float _Useonlycolor;
				uniform float4 _SpeedMainTexUVNoiseZW;
				uniform sampler2D _Mask;
				uniform float4 _Mask_ST;
				uniform sampler2D _Flow;
				uniform float4 _Flow_ST;
				uniform float _Distortionpower;
				uniform sampler2D _Noise;
				uniform float4 _Noise_ST;
				uniform float4 _Color;
				uniform float _Emission;
				uniform float _Fresnelscale;
				uniform float _Fresnelpower;
				uniform float _Depthpower;

				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.ase_texcoord3.xyz = ase_worldPos;
					float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
					o.ase_texcoord4.xyz = ase_worldNormal;
					float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
					float4 screenPos = ComputeScreenPos(ase_clipPos);
					//o.ase_texcoord5 = screenPos;
					
					
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

				fixed4 frag ( v2f i , half ase_vface : VFACE ) : SV_Target
				{
					float fade = 1;
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						fade = saturate (_Depthpower * (sceneZ-partZ));
					#endif
					
					float2 appendResult186 = (float2(_SpeedMainTexUVNoiseZW.x , _SpeedMainTexUVNoiseZW.y));
					float2 uv_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					float2 uv_Mask = i.texcoord.xy * _Mask_ST.xy + _Mask_ST.zw;
					float2 appendResult177 = (float2(_SpeedMainTexUVNoiseZW.z , _SpeedMainTexUVNoiseZW.w));
					float3 uv_Flow = i.texcoord.xyz;
					uv_Flow.xy = i.texcoord.xyz.xy * _Flow_ST.xy + _Flow_ST.zw;
					float4 tex2DNode203 = tex2D( _MainTex, ( ( ( appendResult186 * _Time.y ) + uv_MainTex ) - ( (( tex2D( _Mask, uv_Mask ) * tex2D( _Flow, ( ( _Time.y * appendResult177 ) + (uv_Flow).xy ) ) )).rg * _Distortionpower ) ) );
					float2 uv_Noise = i.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
					float4 tex2DNode211 = tex2D( _Noise, uv_Noise );
					float w199 = (1.0 + (uv_Flow.z - 0.0) * (128.0 - 1.0) / (1.0 - 0.0));
					float4 temp_cast_3 = (tex2DNode203.a).xxxx;
					float div207=256.0/float((int)w199);
					float4 posterize207 = ( floor( temp_cast_3 * div207 ) / div207 );
					float opac215 = (posterize207).a;
					float3 ase_worldPos = i.ase_texcoord3.xyz;
					float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
					ase_worldViewDir = normalize(ase_worldViewDir);
					float3 ase_worldNormal = i.ase_texcoord4.xyz;
					float fresnelNdotV187 = dot( ase_worldNormal, ase_worldViewDir );
					float fresnelNode187 = ( 0.0 + _Fresnelscale * pow( 1.0 - fresnelNdotV187, _Fresnelpower ) );
					float clampResult193 = clamp( fresnelNode187 , 0.0 , 1.0 );
					float switchResult206 = (((ase_vface>0)?(clampResult193):(0.0)));
					float clampResult202 = clamp( fade , 0.0 , 1.0 );			
					float clampResult214 = clamp( ( (1.0 + (clampResult202 - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) - switchResult206 ) , 0.0 , 1.0 );
					float4 appendResult224 = (float4(( lerp(float4( (( tex2DNode203 * tex2DNode211 * _Color * i.color )).rgb , 0.0 ),_Color,_Useonlycolor) * _Emission ).rgb , ( opac215 * tex2DNode211.a * _Color.a * i.color.a * ( switchResult206 + clampResult214 ) )));
					fixed4 col = appendResult224;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
}
/*ASEBEGIN
Version=16700
7;29;1906;1004;2628.474;817.7966;2.32459;True;False
Node;AmplifyShaderEditor.Vector4Node;175;-3403.278,-187.8897;Float;False;Property;_SpeedMainTexUVNoiseZW;Speed MainTex U/V + Noise Z/W;4;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;179;-3128.091,133.8754;Float;False;0;182;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;176;-3071.756,-159.3273;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;177;-3039.331,-27.67758;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-2822.756,18.16158;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;222;-2854.699,123.9574;Float;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;180;-2590.137,56.08243;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;182;-2450.287,36.03077;Float;True;Property;_Flow;Flow;5;0;Create;True;0;0;False;0;61c0b9c0523734e0e91bc6043c72a490;61c0b9c0523734e0e91bc6043c72a490;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;181;-2436.15,-164.1162;Float;True;Property;_Mask;Mask;6;0;Create;True;0;0;False;0;98480d5f929f6b44aa9f0bef5e6aa216;98480d5f929f6b44aa9f0bef5e6aa216;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;186;-3041.873,-249.9572;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;185;-2094.413,-6.293035;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;189;-2825.2,-248.616;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;191;-1970.021,94.15653;Float;False;Property;_Distortionpower;Distortion power;7;0;Create;True;0;0;False;0;0.2;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;190;-1942.127,-12.67462;Float;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;188;-2824.302,-160.4125;Float;False;0;203;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;184;-1832.816,612.7309;Float;False;Property;_Fresnelscale;Fresnel scale;8;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;183;-1837.439,702.0196;Float;False;Property;_Fresnelpower;Fresnel power;9;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;195;-2558.478,-247.3405;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;194;-1721.198,-13.66589;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;192;-1905.483,966.1779;Float;False;Property;_Depthpower;Depth power;10;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;196;-2576.335,238.1291;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;128;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;187;-1603.648,590.0786;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;200;-1558.277,-232.7691;Float;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-2264.895,294.6912;Float;False;w;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;197;-1700.775,950.5247;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;193;-1322.607,587.9484;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;202;-1413.214,950.8426;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;203;-1242.024,-223.369;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;16d9a7ba675ceeb4c843fd998345edd5;16d9a7ba675ceeb4c843fd998345edd5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;201;-1052.624,-464.7238;Float;False;199;w;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;208;-1157.103,177.5223;Float;False;Property;_Color;Color;2;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;210;-1115.114,347.522;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;211;-1230.057,-22.91238;Float;True;Property;_Noise;Noise;1;0;Create;True;0;0;False;0;cbc94c83de2b026488aa1d32c6cc4cab;cbc94c83de2b026488aa1d32c6cc4cab;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwitchByFaceNode;206;-1070.486,549.6138;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;205;-1243.115,941.5295;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosterizeNode;207;-836.0663,-495.9696;Float;False;1;2;1;COLOR;0,0,0,0;False;0;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;212;-803.8646,594.939;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;209;-648.4307,-495.6043;Float;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;213;-822.2797,-128.8466;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;214;-634.4662,573.429;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;215;-336.3258,-489.0232;Float;False;opac;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;223;-663.7763,-119.8957;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;225;-437.2037,-75.52232;Float;False;Property;_Useonlycolor;Use only color;11;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;216;-390.4986,112.0732;Float;False;215;opac;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;220;-374.2722,30.65194;Float;False;Property;_Emission;Emission;3;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;218;-429.3195,373.2848;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;221;-192.7166,149.6884;Float;False;5;5;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;-176.8772,9.083834;Float;False;2;2;0;COLOR;1,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;224;39.51221,99.70026;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;123;264.0503,171.3129;Float;False;True;2;Float;;0;11;Hovl/Particles/Add_Fresnell;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-4;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;177;0;175;3
WireConnection;177;1;175;4
WireConnection;178;0;176;2
WireConnection;178;1;177;0
WireConnection;222;0;179;0
WireConnection;180;0;178;0
WireConnection;180;1;222;0
WireConnection;182;1;180;0
WireConnection;186;0;175;1
WireConnection;186;1;175;2
WireConnection;185;0;181;0
WireConnection;185;1;182;0
WireConnection;189;0;186;0
WireConnection;189;1;176;2
WireConnection;190;0;185;0
WireConnection;195;0;189;0
WireConnection;195;1;188;0
WireConnection;194;0;190;0
WireConnection;194;1;191;0
WireConnection;196;0;179;3
WireConnection;187;2;184;0
WireConnection;187;3;183;0
WireConnection;200;0;195;0
WireConnection;200;1;194;0
WireConnection;199;0;196;0
WireConnection;197;0;192;0
WireConnection;193;0;187;0
WireConnection;202;0;197;0
WireConnection;203;1;200;0
WireConnection;206;0;193;0
WireConnection;205;0;202;0
WireConnection;207;1;203;4
WireConnection;207;0;201;0
WireConnection;212;0;205;0
WireConnection;212;1;206;0
WireConnection;209;0;207;0
WireConnection;213;0;203;0
WireConnection;213;1;211;0
WireConnection;213;2;208;0
WireConnection;213;3;210;0
WireConnection;214;0;212;0
WireConnection;215;0;209;0
WireConnection;223;0;213;0
WireConnection;225;0;223;0
WireConnection;225;1;208;0
WireConnection;218;0;206;0
WireConnection;218;1;214;0
WireConnection;221;0;216;0
WireConnection;221;1;211;4
WireConnection;221;2;208;4
WireConnection;221;3;210;4
WireConnection;221;4;218;0
WireConnection;219;0;225;0
WireConnection;219;1;220;0
WireConnection;224;0;219;0
WireConnection;224;3;221;0
WireConnection;123;0;224;0
ASEEND*/
//CHKSM=D4AB6591B7C5F48A19328108EB7F0EB6A4DCCE88
