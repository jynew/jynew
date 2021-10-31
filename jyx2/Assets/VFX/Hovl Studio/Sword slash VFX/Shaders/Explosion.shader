Shader "Hovl/Particles/Explosion"
{
	Properties
	{
		_Noise("Noise", 2D) = "white" {}
		_FinalEmission("Final Emission", Float) = 1
		_Color("Color", Color) = (1,1,1,1)
		_GlowColor("Glow Color", Color) = (1,1,0,1)
		_Opacity("Opacity", Range( 0 , 1)) = 1
		_NoisespeedXYNoisepowerZGlowpowerW("Noise speed XY Noise power Z Glow power W", Vector) = (0.314,0.427,0.001,4)
		_MotionVector("MotionVector", 2D) = "white" {}
		_MainTex("MainTex", 2D) = "white" {}
		_TilingXY("Tiling XY", Vector) = (8,8,0,0)
		_MotionAmount("MotionAmount", Float) = 0.001
		[MaterialToggle] _Usedepth ("Use depth?", Float ) = 0
        _Depthpower ("Depth power", Float ) = 1
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
				uniform float4 _MainTex_ST;
				uniform float4 _GlowColor;
				uniform sampler2D _Noise;
				uniform float4 _Noise_ST;
				uniform float4 _NoisespeedXYNoisepowerZGlowpowerW;
				uniform sampler2D _MotionVector;
				uniform float4 _TilingXY;
				uniform float _MotionAmount;
				uniform float4 _Color;
				uniform float _FinalEmission;
				uniform float _Opacity;
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

					float4 uv0_Noise = i.texcoord;
					uv0_Noise.xy = i.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
					float Emission57 = uv0_Noise.z;
					float2 appendResult1 = (float2(_NoisespeedXYNoisepowerZGlowpowerW.x , _NoisespeedXYNoisepowerZGlowpowerW.y));
					float4 temp_output_9_0 = ( tex2D( _Noise, ( uv0_Noise + float4( ( _Time.y * appendResult1 ), 0.0 , 0.0 ) ).xy ) * _NoisespeedXYNoisepowerZGlowpowerW.z );
					float T104 = uv0_Noise.w;
					float Fract113 = frac( T104 );
					float2 uv0_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					float temp_output_89_0 = floor( T104 );
					// *** BEGIN Flipbook UV Animation vars ***
					// Total tiles of Flipbook Texture
					float fbtotaltiles107 = _TilingXY.x * _TilingXY.y;
					// Offsets for cols and rows of Flipbook Texture
					float fbcolsoffset107 = 1.0f / _TilingXY.x;
					float fbrowsoffset107 = 1.0f / _TilingXY.y;
					// Speed of animation
					float fbspeed107 = _Time[ 1 ] * 0.0;
					// UV Tiling (col and row offset)
					float2 fbtiling107 = float2(fbcolsoffset107, fbrowsoffset107);
					// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
					// Calculate current tile linear index
					float fbcurrenttileindex107 = round( fmod( fbspeed107 + temp_output_89_0, fbtotaltiles107) );
					fbcurrenttileindex107 += ( fbcurrenttileindex107 < 0) ? fbtotaltiles107 : 0;
					// Obtain Offset X coordinate from current tile linear index
					float fblinearindextox107 = round ( fmod ( fbcurrenttileindex107, _TilingXY.x ) );
					// Multiply Offset X by coloffset
					float fboffsetx107 = fblinearindextox107 * fbcolsoffset107;
					// Obtain Offset Y coordinate from current tile linear index
					float fblinearindextoy107 = round( fmod( ( fbcurrenttileindex107 - fblinearindextox107 ) / _TilingXY.x, _TilingXY.y ) );
					// Reverse Y to get tiles from Top to Bottom
					fblinearindextoy107 = (int)(_TilingXY.y-1) - fblinearindextoy107;
					// Multiply Offset Y by rowoffset
					float fboffsety107 = fblinearindextoy107 * fbrowsoffset107;
					// UV Offset
					float2 fboffset107 = float2(fboffsetx107, fboffsety107);
					// Flipbook UV
					half2 fbuv107 = uv0_MainTex * fbtiling107 + fboffset107;
					// *** END Flipbook UV Animation vars ***
					float4 temp_cast_2 = (_MotionAmount).xxxx;
					float4 temp_cast_3 = (( _MotionAmount * -1.0 )).xxxx;
					float4 temp_output_93_0 = (temp_cast_2 + (tex2D( _MotionVector, fbuv107 ) - float4( 0,0,0,0 )) * (temp_cast_3 - temp_cast_2) / (float4( 1,1,1,1 ) - float4( 0,0,0,0 )));
					float4 tex2DNode14 = tex2D( _MainTex, ( temp_output_9_0 + ( Fract113 * temp_output_93_0 ) + float4( fbuv107, 0.0 , 0.0 ) ).rg );
					float4 temp_cast_8 = (_MotionAmount).xxxx;
					float4 temp_cast_9 = (( _MotionAmount * -1.0 )).xxxx;
					float fbtotaltiles103 = _TilingXY.x * _TilingXY.y;
					float fbcolsoffset103 = 1.0f / _TilingXY.x;
					float fbrowsoffset103 = 1.0f / _TilingXY.y;
					float fbspeed103 = _Time[ 1 ] * 0.0;
					float2 fbtiling103 = float2(fbcolsoffset103, fbrowsoffset103);
					float fbcurrenttileindex103 = round( fmod( fbspeed103 + ( temp_output_89_0 + 1.0 ), fbtotaltiles103) );
					fbcurrenttileindex103 += ( fbcurrenttileindex103 < 0) ? fbtotaltiles103 : 0;
					float fblinearindextox103 = round ( fmod ( fbcurrenttileindex103, _TilingXY.x ) );
					float fboffsetx103 = fblinearindextox103 * fbcolsoffset103;
					float fblinearindextoy103 = round( fmod( ( fbcurrenttileindex103 - fblinearindextox103 ) / _TilingXY.x, _TilingXY.y ) );
					fblinearindextoy103 = (int)(_TilingXY.y-1) - fblinearindextoy103;
					float fboffsety103 = fblinearindextoy103 * fbrowsoffset103;
					float2 fboffset103 = float2(fboffsetx103, fboffsety103);
					half2 fbuv103 = uv0_MainTex * fbtiling103 + fboffset103;
					float4 tex2DNode60 = tex2D( _MainTex, ( temp_output_9_0 + ( ( Fract113 + -1.0 ) * temp_output_93_0 ) + float4( fbuv103, 0.0 , 0.0 ) ).rg );
					float4 lerpResult63 = lerp( tex2DNode14 , tex2DNode60 , Fract113);
					float4 temp_cast_12 = (_NoisespeedXYNoisepowerZGlowpowerW.w).xxxx;
					float4 temp_cast_13 = (10000.0).xxxx;
					float4 clampResult132 = clamp( ( _GlowColor * Emission57 * pow( abs( lerpResult63 ) , temp_cast_12 ) ) , float4( 0,0,0,0 ) , temp_cast_13 );
					float lerpResult65 = lerp( tex2DNode14.a , tex2DNode60.a , Fract113);
					float4 appendResult136 = (float4(( ( clampResult132 + lerpResult63 ) * _Color * i.color * _FinalEmission ).rgb , ( ( lerpResult65 * _Color.a * i.color.a ) * _Opacity )));
					

					fixed4 col = appendResult136;
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
7;29;1906;1004;2518.312;587.7371;1.915742;True;False
Node;AmplifyShaderEditor.CommentaryNode;130;-4097.669,-692.5529;Float;False;1464.114;475.897;Add some distortion to smoke texture;11;3;104;1;2;4;5;7;9;57;30;129;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-3563.88,-637.6884;Float;False;0;7;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;128;-4113.345,-177.4462;Float;False;2004.502;997.3094;Sequence Motion;20;68;12;105;89;76;75;107;94;92;95;54;113;102;93;103;99;119;106;91;100;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;104;-3270.074,-566.9968;Float;False;T;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;30;-4047.669,-465.1935;Float;False;Property;_NoisespeedXYNoisepowerZGlowpowerW;Noise speed XY Noise power Z Glow power W;5;0;Create;True;0;0;False;0;0.314,0.427,0.001,4;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;105;-4063.344,69.96631;Float;False;104;T;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;1;-3710.976,-448.5145;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;75;-3781.37,423.3419;Float;False;Property;_TilingXY;Tiling XY;8;0;Create;True;0;0;False;0;8,8,0,0;8,8,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;2;-3737.947,-515.9386;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;89;-3852.499,665.8519;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;76;-3829.952,304.9428;Float;False;0;59;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;107;-3471.058,407.8905;Float;False;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-3480.89,-468.1634;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;92;-3832.93,70.94185;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-3377.297,253.5844;Float;False;Property;_MotionAmount;MotionAmount;9;0;Create;True;0;0;False;0;0.001;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;102;-3676.005,668.168;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-3082.169,330.9825;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;113;-3058.5,-9.40364;Float;False;Fract;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;54;-3091.831,76.12077;Float;True;Property;_MotionVector;MotionVector;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-3271.588,-488.5103;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;103;-3468.042,594.863;Float;False;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;99;-2747.617,58.88704;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-3127.459,-485.8347;Float;True;Property;_Noise;Noise;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;93;-2770.184,194.9624;Float;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-2528.97,174.2508;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-2549.433,-56.81462;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-2802.555,-467.8086;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;119;-2614.76,375.4799;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;106;-2630.997,509.0002;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;59;-2314.822,-429.6734;Float;True;Property;_MainTex;MainTex;7;0;Create;True;0;0;False;0;1279422b5d9799842bf3006b54b27214;1279422b5d9799842bf3006b54b27214;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleAddOpNode;68;-2263.843,162.2813;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-2268.99,-127.4462;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-1895.344,-28.77689;Float;False;113;Fract;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-1989.088,-219.7894;Float;True;Property;_sg;sg;0;0;Create;True;0;0;False;0;1279422b5d9799842bf3006b54b27214;1279422b5d9799842bf3006b54b27214;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;60;-1996.264,48.83716;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;1279422b5d9799842bf3006b54b27214;1279422b5d9799842bf3006b54b27214;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;134;-1493.765,-591.5585;Float;False;757.167;421.8759;Make fire color brighter;7;58;31;22;131;19;133;132;;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;63;-1623.175,-76.8437;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.5;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;129;-3125.216,-271.9521;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;-3270.566,-642.5529;Float;False;Emission;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;131;-1408.701,-307.0374;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;31;-1443.765,-541.5585;Float;False;Property;_GlowColor;Glow Color;3;0;Create;True;0;0;False;0;1,1,0,1;1,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;19;-1288.673,-305.8751;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-1422.781,-377.8625;Float;False;57;Emission;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-1115.206,-284.869;Float;False;Constant;_Float0;Float 0;13;0;Create;True;0;0;False;0;10000;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-1111.88,-467.8062;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;132;-911.598,-325.6826;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;10000,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;17;-1858.125,423.5552;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;16;-1916.124,253.5551;Float;False;Property;_Color;Color;2;0;Create;True;0;0;False;0;1,1,1,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;65;-1623.395,84.79343;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-685.7427,-143.0471;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-999.3977,387.8671;Float;False;Property;_Opacity;Opacity;4;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-884.2829,218.7032;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-766.4157,81.56993;Float;False;Property;_FinalEmission;Final Emission;1;0;Create;True;0;0;False;0;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-663.315,218.4495;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-531.0883,-106.9828;Float;False;4;4;0;COLOR;1,1,1,1;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;136;-274.0238,-108.6356;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;135;16.64722,-109.1369;Float;False;True;2;Float;;0;11;Hovl/Particles/Explosion;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;104;0;3;4
WireConnection;1;0;30;1
WireConnection;1;1;30;2
WireConnection;89;0;105;0
WireConnection;107;0;76;0
WireConnection;107;1;75;1
WireConnection;107;2;75;2
WireConnection;107;4;89;0
WireConnection;4;0;2;0
WireConnection;4;1;1;0
WireConnection;92;0;105;0
WireConnection;102;0;89;0
WireConnection;95;0;94;0
WireConnection;113;0;92;0
WireConnection;54;1;107;0
WireConnection;5;0;3;0
WireConnection;5;1;4;0
WireConnection;103;0;76;0
WireConnection;103;1;75;1
WireConnection;103;2;75;2
WireConnection;103;4;102;0
WireConnection;99;0;113;0
WireConnection;7;1;5;0
WireConnection;93;0;54;0
WireConnection;93;3;94;0
WireConnection;93;4;95;0
WireConnection;100;0;99;0
WireConnection;100;1;93;0
WireConnection;91;0;113;0
WireConnection;91;1;93;0
WireConnection;9;0;7;0
WireConnection;9;1;30;3
WireConnection;119;0;107;0
WireConnection;106;0;103;0
WireConnection;68;0;9;0
WireConnection;68;1;100;0
WireConnection;68;2;106;0
WireConnection;12;0;9;0
WireConnection;12;1;91;0
WireConnection;12;2;119;0
WireConnection;14;0;59;0
WireConnection;14;1;12;0
WireConnection;60;0;59;0
WireConnection;60;1;68;0
WireConnection;63;0;14;0
WireConnection;63;1;60;0
WireConnection;63;2;114;0
WireConnection;129;0;30;4
WireConnection;57;0;3;3
WireConnection;131;0;63;0
WireConnection;19;0;131;0
WireConnection;19;1;129;0
WireConnection;22;0;31;0
WireConnection;22;1;58;0
WireConnection;22;2;19;0
WireConnection;132;0;22;0
WireConnection;132;2;133;0
WireConnection;65;0;14;4
WireConnection;65;1;60;4
WireConnection;65;2;114;0
WireConnection;27;0;132;0
WireConnection;27;1;63;0
WireConnection;21;0;65;0
WireConnection;21;1;16;4
WireConnection;21;2;17;4
WireConnection;29;0;21;0
WireConnection;29;1;24;0
WireConnection;28;0;27;0
WireConnection;28;1;16;0
WireConnection;28;2;17;0
WireConnection;28;3;26;0
WireConnection;136;0;28;0
WireConnection;136;3;29;0
WireConnection;135;0;136;0
ASEEND*/
//CHKSM=B4CA10D76F8C775E68F9990E516A6ADE525C21AB