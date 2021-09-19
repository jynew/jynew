// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/GPUInstancer/Internal-DepthNormalsTexture" {
	Properties{
		_MainTex("", 2D) = "white" {}
		_Cutoff("", Float) = 0.5
		_Color("", Color) = (1,1,1,1)
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			Pass {
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "Include/GPUInstancerInclude.cginc"
		#pragma instancing_options procedural:setupGPUI
		#pragma multi_compile_instancing
		#pragma vertex vert
		#pragma fragment frag
		struct v2f {
			float4 pos : SV_POSITION;
			float4 nz : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
		};
		v2f vert(appdata_base v) {
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			o.pos = UnityObjectToClipPos(v.vertex);
			o.nz.xyz = COMPUTE_VIEW_NORMAL;
			o.nz.w = COMPUTE_DEPTH_01;
			return o;
		}
		fixed4 frag(v2f i) : SV_Target {
			return EncodeDepthNormal(i.nz.w, i.nz.xyz);
		}
		ENDCG
			}
		}

			SubShader{
				Tags { "RenderType" = "TransparentCutout" }
				Pass {
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Include/GPUInstancerInclude.cginc"
			#pragma instancing_options procedural:setupGPUI
			#pragma multi_compile_instancing
			#pragma vertex vert
			#pragma fragment frag
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 nz : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			uniform float4 _MainTex_ST;
			v2f vert(appdata_base v) {
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.nz.xyz = COMPUTE_VIEW_NORMAL;
				o.nz.w = COMPUTE_DEPTH_01;
				return o;
			}
			uniform sampler2D _MainTex;
			uniform fixed _Cutoff;
			uniform fixed4 _Color;
			fixed4 frag(v2f i) : SV_Target {
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a*_Color.a - _Cutoff);
				return EncodeDepthNormal(i.nz.w, i.nz.xyz);
			}
			ENDCG
				}
		}

			SubShader{
				Tags { "RenderType" = "TreeBark" }
				Pass {
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Include/GPUInstancerInclude.cginc"
			#pragma instancing_options procedural:setupGPUI
			#pragma multi_compile_instancing
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			#include "UnityBuiltin3xTreeLibrary.cginc"
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 nz : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert(appdata_full v) {
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				TreeVertBark(v);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				o.nz.xyz = COMPUTE_VIEW_NORMAL;
				o.nz.w = COMPUTE_DEPTH_01;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				return EncodeDepthNormal(i.nz.w, i.nz.xyz);
			}
			ENDCG
				}
			}
				SubShader{
					Tags { "RenderType" = "TreeLeaf" }
					Pass {
				CGPROGRAM
				#include "UnityCG.cginc"
				#include "Include/GPUInstancerInclude.cginc"
				#pragma instancing_options procedural:setupGPUI
				#pragma multi_compile_instancing
				#pragma vertex vert
				#pragma fragment frag
				#include "Lighting.cginc"
				#include "UnityBuiltin3xTreeLibrary.cginc"
				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 nz : TEXCOORD1;
					UNITY_VERTEX_OUTPUT_STEREO
				};
				v2f vert(appdata_full v) {
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					TreeVertLeaf(v);

					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord.xy;
					o.nz.xyz = COMPUTE_VIEW_NORMAL;
					o.nz.w = COMPUTE_DEPTH_01;
					return o;
				}
				uniform sampler2D _MainTex;
				uniform fixed _Cutoff;
				fixed4 frag(v2f i) : SV_Target {
					half alpha = tex2D(_MainTex, i.uv).a;

					clip(alpha - _Cutoff);
					return EncodeDepthNormal(i.nz.w, i.nz.xyz);
				}
				ENDCG
					}
			}

				SubShader{
					Tags { "RenderType" = "TreeOpaque" "DisableBatching" = "True" }
					Pass {
				CGPROGRAM
				#include "UnityCG.cginc"
				#include "Include/GPUInstancerInclude.cginc"
				#pragma instancing_options procedural:setupGPUI
				#pragma multi_compile_instancing
				#pragma vertex vert
				#pragma fragment frag
				#include "TerrainEngine.cginc"
				struct v2f {
					float4 pos : SV_POSITION;
					float4 nz : TEXCOORD0;
					UNITY_VERTEX_OUTPUT_STEREO
				};
				struct appdata {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					fixed4 color : COLOR;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};
				v2f vert(appdata v) {
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					TerrainAnimateTree(v.vertex, v.color.w);
					o.pos = UnityObjectToClipPos(v.vertex);
					o.nz.xyz = COMPUTE_VIEW_NORMAL;
					o.nz.w = COMPUTE_DEPTH_01;
					return o;
				}
				fixed4 frag(v2f i) : SV_Target {
					return EncodeDepthNormal(i.nz.w, i.nz.xyz);
				}
				ENDCG
					}
				}

					SubShader{
						Tags { "RenderType" = "TreeTransparentCutout" "DisableBatching" = "True" }
						Pass {
							Cull Back
					CGPROGRAM
					#include "UnityCG.cginc"
					#include "Include/GPUInstancerInclude.cginc"
					#pragma instancing_options procedural:setupGPUI
					#pragma multi_compile_instancing
					#pragma vertex vert
					#pragma fragment frag
					#include "TerrainEngine.cginc"

					struct v2f {
						float4 pos : SV_POSITION;
						float2 uv : TEXCOORD0;
						float4 nz : TEXCOORD1;
						UNITY_VERTEX_OUTPUT_STEREO
					};
					struct appdata {
						float4 vertex : POSITION;
						float3 normal : NORMAL;
						fixed4 color : COLOR;
						float4 texcoord : TEXCOORD0;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};
					v2f vert(appdata v) {
						v2f o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						TerrainAnimateTree(v.vertex, v.color.w);
						o.pos = UnityObjectToClipPos(v.vertex);
						o.uv = v.texcoord.xy;
						o.nz.xyz = COMPUTE_VIEW_NORMAL;
						o.nz.w = COMPUTE_DEPTH_01;
						return o;
					}
					uniform sampler2D _MainTex;
					uniform fixed _Cutoff;
					fixed4 frag(v2f i) : SV_Target {
						half alpha = tex2D(_MainTex, i.uv).a;

						clip(alpha - _Cutoff);
						return EncodeDepthNormal(i.nz.w, i.nz.xyz);
					}
					ENDCG
						}
						Pass {
							Cull Front
					CGPROGRAM
					#include "UnityCG.cginc"
					#include "Include/GPUInstancerInclude.cginc"
					#pragma instancing_options procedural:setupGPUI
					#pragma multi_compile_instancing
					#pragma vertex vert
					#pragma fragment frag
					#include "TerrainEngine.cginc"

					struct v2f {
						float4 pos : SV_POSITION;
						float2 uv : TEXCOORD0;
						float4 nz : TEXCOORD1;
						UNITY_VERTEX_OUTPUT_STEREO
					};
					struct appdata {
						float4 vertex : POSITION;
						float3 normal : NORMAL;
						fixed4 color : COLOR;
						float4 texcoord : TEXCOORD0;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};
					v2f vert(appdata v) {
						v2f o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						TerrainAnimateTree(v.vertex, v.color.w);
						o.pos = UnityObjectToClipPos(v.vertex);
						o.uv = v.texcoord.xy;
						o.nz.xyz = -COMPUTE_VIEW_NORMAL;
						o.nz.w = COMPUTE_DEPTH_01;
						return o;
					}
					uniform sampler2D _MainTex;
					uniform fixed _Cutoff;
					fixed4 frag(v2f i) : SV_Target {
						fixed4 texcol = tex2D(_MainTex, i.uv);
						clip(texcol.a - _Cutoff);
						return EncodeDepthNormal(i.nz.w, i.nz.xyz);
					}
					ENDCG
						}

				}

					SubShader {
					Tags{ "RenderType" = "TreeBillboard" }
						Pass{
							Cull Off
					CGPROGRAM
					#include "UnityCG.cginc"
					#include "Include/GPUInstancerInclude.cginc"
					#pragma instancing_options procedural:setupGPUI
					#pragma multi_compile_instancing
					#pragma vertex vert
					#pragma fragment frag
					#include "TerrainEngine.cginc"
					struct v2f {
						float4 pos : SV_POSITION;
						float2 uv : TEXCOORD0;
						float4 nz : TEXCOORD1;
						UNITY_VERTEX_OUTPUT_STEREO
					};
					v2f vert(appdata_tree_billboard v) {
						v2f o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
						o.pos = UnityObjectToClipPos(v.vertex);
						o.uv.x = v.texcoord.x;
						o.uv.y = v.texcoord.y > 0;
						o.nz.xyz = float3(0,0,1);
						o.nz.w = COMPUTE_DEPTH_01;
						return o;
					}
					uniform sampler2D _MainTex;
					fixed4 frag(v2f i) : SV_Target {
						fixed4 texcol = tex2D(_MainTex, i.uv);
						clip(texcol.a - 0.001);
						return EncodeDepthNormal(i.nz.w, i.nz.xyz);
					}
					ENDCG
					}
				}

					SubShader{
						Tags { "RenderType" = "GrassBillboard" }
						Pass {
							Cull Off
					CGPROGRAM
					#include "UnityCG.cginc"
					#include "Include/GPUInstancerInclude.cginc"
					#pragma instancing_options procedural:setupGPUI
					#pragma multi_compile_instancing
					#pragma vertex vert
					#pragma fragment frag
					#include "TerrainEngine.cginc"

					struct v2f {
						float4 pos : SV_POSITION;
						fixed4 color : COLOR;
						float2 uv : TEXCOORD0;
						float4 nz : TEXCOORD1;
						UNITY_VERTEX_OUTPUT_STEREO
					};

					v2f vert(appdata_full v) {
						v2f o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						WavingGrassBillboardVert(v);
						o.color = v.color;
						o.pos = UnityObjectToClipPos(v.vertex);
						o.uv = v.texcoord.xy;
						o.nz.xyz = COMPUTE_VIEW_NORMAL;
						o.nz.w = COMPUTE_DEPTH_01;
						return o;
					}
					uniform sampler2D _MainTex;
					uniform fixed _Cutoff;
					fixed4 frag(v2f i) : SV_Target {
						fixed4 texcol = tex2D(_MainTex, i.uv);
						fixed alpha = texcol.a * i.color.a;
						clip(alpha - _Cutoff);
						return EncodeDepthNormal(i.nz.w, i.nz.xyz);
					}
					ENDCG
						}
					}

						SubShader{
							Tags { "RenderType" = "Grass" }
							Pass {
								Cull Off
						CGPROGRAM
						#include "UnityCG.cginc"
						#include "Include/GPUInstancerInclude.cginc"
						#pragma instancing_options procedural:setupGPUI
						#pragma multi_compile_instancing
						#pragma vertex vert
						#pragma fragment frag
						#include "TerrainEngine.cginc"
						struct v2f {
							float4 pos : SV_POSITION;
							fixed4 color : COLOR;
							float2 uv : TEXCOORD0;
							float4 nz : TEXCOORD1;
							UNITY_VERTEX_OUTPUT_STEREO
						};

						v2f vert(appdata_full v) {
							v2f o;
							UNITY_SETUP_INSTANCE_ID(v);
							UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
							WavingGrassVert(v);
							o.color = v.color;
							o.pos = UnityObjectToClipPos(v.vertex);
							o.uv = v.texcoord;
							o.nz.xyz = COMPUTE_VIEW_NORMAL;
							o.nz.w = COMPUTE_DEPTH_01;
							return o;
						}
						uniform sampler2D _MainTex;
						uniform fixed _Cutoff;
						fixed4 frag(v2f i) : SV_Target {
							fixed4 texcol = tex2D(_MainTex, i.uv);
							fixed alpha = texcol.a * i.color.a;
							clip(alpha - _Cutoff);
							return EncodeDepthNormal(i.nz.w, i.nz.xyz);
						}
						ENDCG
							}
						}

						SubShader{
						Tags { "RenderType" = "GPUIFoliage" }
						Pass {
							Cull Off
							CGPROGRAM
							#include "UnityCG.cginc"
							#include "Include/GPUInstancerInclude.cginc"
							#pragma instancing_options procedural:setupGPUI
							#pragma multi_compile_instancing
							#pragma vertex vert
							#pragma fragment frag
							#include "TerrainEngine.cginc"
							struct v2f {
								float4 pos : SV_POSITION;
								fixed4 color : COLOR;
								float2 uv : TEXCOORD0;
								float4 nz : TEXCOORD1;
								UNITY_VERTEX_OUTPUT_STEREO
							};

							uniform float _IsBillboard;
							uniform float _WindWavesOn;
							uniform sampler2D _WindWaveNormalTexture;
							uniform float2 _WindVector;
							uniform float _WindWaveSize;
							uniform float _WindIdleSway;
							uniform float _WindWaveSway;
							uniform sampler2D _MainTex;
							uniform float4 _MainTex_ST;
							uniform float _Cutoff = 0.5;

							void GPUIFoliageVert(inout appdata_full v)
							{
								float BillboardOn261 = lerp(0.0,1.0,_IsBillboard);
								float4 ase_vertex4Pos = v.vertex;
								float4x4 break301 = unity_ObjectToWorld;
								float3 appendResult302 = (float3(break301[ 0 ][ 0 ] , break301[ 1 ][ 0 ] , break301[ 2 ][ 0 ]));
								float3 appendResult306 = (float3(break301[ 0 ][ 1 ] , break301[ 1 ][ 1 ] , break301[ 2 ][ 1 ]));
								float3 appendResult307 = (float3(break301[ 0 ][ 2 ] , break301[ 1 ][ 2 ] , break301[ 2 ][ 2 ]));
								float4 appendResult303 = (float4(( ase_vertex4Pos.x * length( appendResult302 ) ) , ( ase_vertex4Pos.y * length( appendResult306 ) ) , ( ase_vertex4Pos.z * length( appendResult307 ) ) , ase_vertex4Pos.w));
								float4x4 break278 = UNITY_MATRIX_V;
								float3 appendResult287 = (float3(break278[ 0 ][ 0 ] , break278[ 0 ][ 1 ] , break278[ 0 ][ 2 ]));
								float3 normalizeResult288 = normalize( appendResult287 );
								float3 appendResult295 = (float3(normalizeResult288));
								float3 appendResult314 = (float3(break301[ 0 ][ 3 ] , break301[ 1 ][ 3 ] , break301[ 2 ][ 3 ]));
								float3 normalizeResult504 = normalize( cross( float3(0,1,0) , ( appendResult314 - _WorldSpaceCameraPos ) ) );
								#ifdef _BILLBOARDFACECAMPOS_ON
									float3 staticSwitch496 = normalizeResult504;
								#else
									float3 staticSwitch496 = appendResult295;
								#endif
								float3 appendResult279 = (float3(break278[ 1 ][ 0 ] , break278[ 1 ][ 1 ] , break278[ 1 ][ 2 ]));
								float3 normalizeResult283 = normalize( appendResult279 );
								float3 appendResult296 = (float3(normalizeResult283));
								float temp_output_416_0 = (appendResult296).y;
								float3 break419 = appendResult296;
								float4 appendResult420 = (float4(break419.x , ( temp_output_416_0 * -1.0 ) , break419.z , 0.0));
								#ifdef _BILLBOARDFACECAMPOS_ON
									float4 staticSwitch498 = float4(0,1,0,0);
								#else
									float4 staticSwitch498 = (( temp_output_416_0 > 0.0 ) ? float4( appendResult296 , 0.0 ) :  appendResult420 );
								#endif
								float3 appendResult281 = (float3(break278[ 2 ][ 0 ] , break278[ 2 ][ 1 ] , break278[ 2 ][ 2 ]));
								float3 normalizeResult284 = normalize( appendResult281 );
								float3 appendResult297 = (float3(( normalizeResult284 * -1.0 )));
								float3 appendResult322 = (float3(mul( appendResult303, float4x4(float4( staticSwitch496 , 0.0 ), staticSwitch498, float4( appendResult297 , 0.0 ), float4( 0,0,0,0 )) ).xyz));
								float4 appendResult323 = (float4(( appendResult322 + appendResult314 ) , ase_vertex4Pos.w));
								float4 transform327 = mul(unity_WorldToObject,appendResult323);
								float4 BillboardedVertexPos320 = transform327;
								float WindWavesOn112 = lerp(0.0,1.0,_WindWavesOn);
								float2 WindDirVector29 = _WindVector;
								float mulTime407 = _Time.y * ( length( WindDirVector29 ) * 0.01 );
								float WindWaveSize128 = _WindWaveSize;
								float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
								float2 panner36 = ( mulTime407 * WindDirVector29 + ( ( 1.0 - (0.0 + (WindWaveSize128 - 0.0) * (0.9 - 0.0) / (1.0 - 0.0)) ) * 0.003 * (ase_worldPos).xz ));
								float3 tex2DNode2 = UnpackNormal( tex2Dlod( _WindWaveNormalTexture, float4( panner36, 0, 0.0) ) );
								float3 FinalWindVectors181 = tex2DNode2;
								float3 break185 = FinalWindVectors181;
								float3 appendResult186 = (float3(break185.x , 0.0 , break185.y));
								float WindIdleSway197 = _WindIdleSway;
								float3 ase_vertex3Pos = v.vertex.xyz;
								float3 lerpResult230 = lerp( float3( 0,0,0 ) , ( appendResult186 * WindIdleSway197 ) , saturate( ase_vertex3Pos.y ));
								float3 WindIdleSwayCalculated218 = lerpResult230;
								float WindWaveSway191 = _WindWaveSway;
								float WindWaveNoise126 = saturate( tex2DNode2.g );
								float2 break206 = ( WindWaveNoise126 * WindDirVector29 );
								float3 appendResult205 = (float3(break206.x , 0.0 , break206.y));
								float3 lerpResult229 = lerp( float3( 0,0,0 ) , ( WindWaveSway191 * 20.0 * appendResult205 ) , saturate( ase_vertex3Pos.y ));
								float3 WindWaveSwayCalculated220 = lerpResult229;
								float3 lerpResult215 = lerp( WindIdleSwayCalculated218 , ( WindIdleSwayCalculated218 + ( WindWaveSwayCalculated220 * -1.0 ) ) , WindWaveNoise126);
								float4 transform233 = mul(unity_WorldToObject,float4( (( WindWavesOn112 > 0.0 ) ? lerpResult215 :  WindIdleSwayCalculated218 ) , 0.0 ));
								float4 WindVertexOffset183 = transform233;
								float4 FinalVertexPos336 = (( BillboardOn261 > 0.0 ) ? ( BillboardedVertexPos320 + WindVertexOffset183 ) :  ( WindVertexOffset183 + ase_vertex4Pos ) );
								v.vertex.xyz = FinalVertexPos336.xyz;
								v.normal = float3(0,1,0);
							}


							v2f vert(appdata_full v) {
								v2f o;
								UNITY_SETUP_INSTANCE_ID(v);
								UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
								GPUIFoliageVert(v);
								o.color = v.color;
								o.pos = UnityObjectToClipPos(v.vertex);
								o.uv = v.texcoord;
								o.nz.xyz = COMPUTE_VIEW_NORMAL;
								o.nz.w = COMPUTE_DEPTH_01;
								return o;
							}

							fixed4 frag(v2f i) : SV_Target {
								fixed4 texcol = tex2D(_MainTex, i.uv);
								fixed alpha = texcol.a * i.color.a;
								clip(alpha - _Cutoff);
								return EncodeDepthNormal(i.nz.w, i.nz.xyz);
							}
							ENDCG
						}
					}
						Fallback Off
}
