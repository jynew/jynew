Shader "Hovl/Particles/Scroll"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Flow("Flow", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_DistortionSpeedXYPowerZ("Distortion Speed XY Power Z", Vector) = (0,0,0,0)
		_Emission("Emission", Float) = 2
		_Color("Color", Color) = (0.5,0.5,0.5,1)
		_Opacity("Opacity", Range( 0 , 1)) = 1
		_PathSet0ifyouuseinPS("Path(Set 0 if you use in PS)", Range( 0 , 1)) = 0
		_Noisedistortpower("Noise distort power", Float) = 1
		[Toggle]_UsePScustomdataW("Use PS custom data W", Float) = 1
		[MaterialToggle] _Usedepth ("Use depth?", Float ) = 0
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
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform float4 _SpeedMainTexUVNoiseZW;
				uniform sampler2D _Flow;
				uniform float4 _DistortionSpeedXYPowerZ;
				uniform float4 _Flow_ST;
				uniform sampler2D _Noise;
				uniform float4 _Noise_ST;
				uniform float _PathSet0ifyouuseinPS;
				uniform float4 _Color;
				uniform float _Emission;
				uniform float _Noisedistortpower;
				uniform float _UsePScustomdataW;
				uniform float _Opacity;
				uniform sampler2D _Mask;
				uniform float4 _Mask_ST;
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
						float fade = saturate (_InvFade * (sceneZ-partZ));
						lp *= lerp(1, fade, _Usedepth);
						i.color.a *= lp;
					#endif

					float2 appendResult13 = (float2(_SpeedMainTexUVNoiseZW.x , _SpeedMainTexUVNoiseZW.y));
					float2 uv0_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					float2 panner21 = ( 1.0 * _Time.y * appendResult13 + uv0_MainTex);
					float2 appendResult4 = (float2(_DistortionSpeedXYPowerZ.x , _DistortionSpeedXYPowerZ.y));
					float4 uv0_Flow = i.texcoord;
					uv0_Flow.xy = i.texcoord.xy * _Flow_ST.xy + _Flow_ST.zw;
					float2 panner6 = ( 1.0 * _Time.y * appendResult4 + (uv0_Flow).xy);
					float Flowpower11 = _DistortionSpeedXYPowerZ.z;
					float2 temp_output_18_0 = ( (tex2D( _Flow, panner6 )).rg * Flowpower11 );
					float2 appendResult17 = (float2(_SpeedMainTexUVNoiseZW.z , _SpeedMainTexUVNoiseZW.w));
					float2 uv0_Noise = i.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
					float2 panner22 = ( 1.0 * _Time.y * appendResult17 + uv0_Noise);
					float W113 = uv0_Flow.z;
					float2 appendResult147 = (float2((panner22).x , (0.0 + ((panner22).y - ( _PathSet0ifyouuseinPS + W113 )) * (1.0 - 0.0) / (1.0 - ( _PathSet0ifyouuseinPS + W113 )))));
					float4 tex2DNode26 = tex2D( _Noise, ( appendResult147 - temp_output_18_0 ) );
					float4 temp_cast_0 = (0.1).xxxx;
					float4 clampResult58 = clamp( ( tex2DNode26 - temp_cast_0 ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
					float2 uv093 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float clampResult115 = clamp( ( 1.0 - pow( ( 1.0 - uv093.y ) , 40.0 ) ) , 0.0 , 1.0 );
					float clampResult57 = clamp( ( tex2DNode26.a - 0.1 ) , 0.0 , 1.0 );
					float TT130 = uv0_Flow.w;
					float clampResult133 = clamp( TT130 , 0.0 , 1.0 );
					float clampResult48 = clamp( pow( clampResult57 , ( _Noisedistortpower + lerp(0.0,(0.0 + (clampResult133 - 0.0) * (10.0 - 0.0) / (1.0 - 0.0)),_UsePScustomdataW) ) ) , 0.0 , 1.0 );
					float4 _Vector0 = float4(0,1,-2,5);
					float clampResult49 = clamp( (_Vector0.z + (clampResult48 - _Vector0.x) * (_Vector0.w - _Vector0.z) / (_Vector0.y - _Vector0.x)) , 0.0 , 1.0 );
					float2 uv_Mask = i.texcoord.xy * _Mask_ST.xy + _Mask_ST.zw;
					float4 appendResult40 = (float4(( (( tex2D( _MainTex, ( panner21 - temp_output_18_0 ) ) * clampResult58 * _Color * i.color * clampResult115 )).rgb * _Emission ) , ( ( clampResult49 * clampResult115 ) * _Color.a * i.color.a * _Opacity * tex2D( _Mask, uv_Mask ).a )));			

					fixed4 col = appendResult40;
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
204;92;886;655;3977.575;637.5205;2.263846;True;False
Node;AmplifyShaderEditor.CommentaryNode;148;-3832.236,581.5789;Float;False;1583.562;520.7532;Texture distortion;12;14;16;18;2;3;4;5;113;6;8;11;130;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;9;-3860.321,-462.1631;Float;False;1037.896;533.6285;Textures movement;7;22;21;19;17;15;13;10;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;10;-3810.321,-233.1204;Float;False;Property;_SpeedMainTexUVNoiseZW;Speed MainTex U/V + Noise Z/W;4;0;Create;True;0;0;False;0;0,0,0,0;0,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;2;-3782.236,684.564;Float;False;Property;_DistortionSpeedXYPowerZ;Distortion Speed XY Power Z;5;0;Create;True;0;0;False;0;0,0,0,0;0,0.5,0.4,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-3727.586,879.468;Float;False;0;8;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;17;-3225.773,-61.53451;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-3320.909,-186.76;Float;False;0;26;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;4;-3349.425,719.585;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;113;-3368.596,915.8895;Float;False;W;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;5;-3397.546,631.5789;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;22;-3036.289,-153.3571;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-2911.905,348.8269;Float;False;113;W;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-3009.954,269.9953;Float;False;Property;_PathSet0ifyouuseinPS;Path(Set 0 if you use in PS);9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;6;-3153.139,661.6124;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-3370.888,813.1251;Float;False;Flowpower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-2966.881,633.0593;Float;True;Property;_Flow;Flow;2;0;Create;True;0;0;False;0;2d2569483a93cff42a128d794be03e25;04a40b50e9e63ed43af8af28f2ba4f86;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;145;-2739.669,210.1873;Float;False;False;True;False;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;117;-2697.622,280.0678;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;144;-2524.128,223.7823;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;146;-2530.645,145.0207;Float;False;True;False;False;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-3373.79,987.3322;Float;False;TT;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;14;-2655.628,636.8116;Float;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;16;-2650.772,724.1374;Float;False;11;Flowpower;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;-2046.871,698.9725;Float;False;130;TT;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-2417.674,637.712;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;147;-2303.147,188.3481;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;133;-1851.05,704.2316;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;41;-2040.186,189.8058;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;26;-1846.326,58.1031;Float;True;Property;_Noise;Noise;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;136;-1686.811,704.1088;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;137;-1727.876,948.4697;Float;False;981.1573;251.4055;Smooth gradient;5;93;124;111;125;115;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;51;-1177.099,375.468;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;143;-1481.194,675.7449;Float;False;Property;_UsePScustomdataW;Use PS custom data W;11;0;Create;True;0;0;False;0;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1231.297,492.8675;Float;False;Property;_Noisedistortpower;Noise distort power;10;0;Create;True;0;0;False;0;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;93;-1677.876,998.4697;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;124;-1435.35,1043.038;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;57;-1013.703,382.2813;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;13;-3237.552,-284.898;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-3313.759,-412.1631;Float;False;0;27;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;135;-987.7557,496.3241;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;43;-844.0231,389.0872;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-1516.792,131.5289;Float;False;Constant;_01;0.1;12;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;21;-3029.426,-371.2292;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;111;-1263.131,1044.239;Float;False;2;0;FLOAT;0;False;1;FLOAT;40;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;56;-1350.569,56.84396;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.2;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;48;-685.9332,389.5527;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;125;-1102.236,1045.929;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;23;-2031.431,-102.8944;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;47;-499.1326,453.6958;Float;False;Constant;_Vector0;Vector 0;11;0;Create;True;0;0;False;0;0,1,-2,5;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;42;-250.1441,393.5109;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-4;False;4;FLOAT;7;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;115;-921.7191,1043.875;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;29;-1712.359,424.9394;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;28;-1770.359,254.9394;Float;False;Property;_Color;Color;7;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;58;-1124.935,56.40707;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;27;-1847.238,-131.9327;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;2d2569483a93cff42a128d794be03e25;2d2569483a93cff42a128d794be03e25;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;49;74.55065,424.0493;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-380.5268,68.4642;Float;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,1;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;129;-900.4711,656.2642;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;32;-206.931,62.58289;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;7;79.95734,782.2888;Float;True;Property;_Mask;Mask;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;128;-1060.619,626.0744;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;98.02649,695.1618;Float;False;Property;_Opacity;Opacity;8;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-156.1111,164.702;Float;False;Property;_Emission;Emission;6;0;Create;True;0;0;False;0;2;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;274.7189,469.027;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;454.4367,511.9257;Float;False;5;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;4.139219,67.40697;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;40;672.4033,199.4106;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;142;838.0772,201.5348;Float;False;True;2;Float;;0;11;ERB/Particles/Scroll;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;17;0;10;3
WireConnection;17;1;10;4
WireConnection;4;0;2;1
WireConnection;4;1;2;2
WireConnection;113;0;3;3
WireConnection;5;0;3;0
WireConnection;22;0;19;0
WireConnection;22;2;17;0
WireConnection;6;0;5;0
WireConnection;6;2;4;0
WireConnection;11;0;2;3
WireConnection;8;1;6;0
WireConnection;145;0;22;0
WireConnection;117;0;94;0
WireConnection;117;1;114;0
WireConnection;144;0;145;0
WireConnection;144;1;117;0
WireConnection;146;0;22;0
WireConnection;130;0;3;4
WireConnection;14;0;8;0
WireConnection;18;0;14;0
WireConnection;18;1;16;0
WireConnection;147;0;146;0
WireConnection;147;1;144;0
WireConnection;133;0;131;0
WireConnection;41;0;147;0
WireConnection;41;1;18;0
WireConnection;26;1;41;0
WireConnection;136;0;133;0
WireConnection;51;0;26;4
WireConnection;143;1;136;0
WireConnection;124;0;93;2
WireConnection;57;0;51;0
WireConnection;13;0;10;1
WireConnection;13;1;10;2
WireConnection;135;0;45;0
WireConnection;135;1;143;0
WireConnection;43;0;57;0
WireConnection;43;1;135;0
WireConnection;21;0;15;0
WireConnection;21;2;13;0
WireConnection;111;0;124;0
WireConnection;56;0;26;0
WireConnection;56;1;120;0
WireConnection;48;0;43;0
WireConnection;125;0;111;0
WireConnection;23;0;21;0
WireConnection;23;1;18;0
WireConnection;42;0;48;0
WireConnection;42;1;47;1
WireConnection;42;2;47;2
WireConnection;42;3;47;3
WireConnection;42;4;47;4
WireConnection;115;0;125;0
WireConnection;58;0;56;0
WireConnection;27;1;23;0
WireConnection;49;0;42;0
WireConnection;30;0;27;0
WireConnection;30;1;58;0
WireConnection;30;2;28;0
WireConnection;30;3;29;0
WireConnection;30;4;115;0
WireConnection;129;0;29;4
WireConnection;32;0;30;0
WireConnection;128;0;28;4
WireConnection;116;0;49;0
WireConnection;116;1;115;0
WireConnection;38;0;116;0
WireConnection;38;1;128;0
WireConnection;38;2;129;0
WireConnection;38;3;35;0
WireConnection;38;4;7;4
WireConnection;39;0;32;0
WireConnection;39;1;36;0
WireConnection;40;0;39;0
WireConnection;40;3;38;0
WireConnection;142;0;40;0
ASEEND*/
//CHKSM=6D671574CEE9E2FA8619FED22F9D6689228C832F