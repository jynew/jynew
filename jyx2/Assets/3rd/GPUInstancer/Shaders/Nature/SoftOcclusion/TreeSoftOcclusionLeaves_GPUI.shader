Shader "GPUInstancer/Nature/Tree Soft Occlusion Leaves" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {  }
        _Cutoff ("Alpha cutoff", Range(0.25,0.9)) = 0.5
        _BaseLight ("Base Light", Range(0, 1)) = 0.35
        _AO ("Amb. Occlusion", Range(0, 10)) = 2.4
        _Occlusion ("Dir Occlusion", Range(0, 20)) = 7.5
    }

    SubShader {
        Tags {
            "Queue" = "AlphaTest"
            "IgnoreProjector"="True"
            "RenderType" = "TreeTransparentCutout"
            "DisableBatching"="True"
        }
        Cull Off
        ColorMask RGB

        Pass {
            Lighting On

            CGPROGRAM
		
			#include "UnityCG.cginc"
			#include "../../Include/GPUInstancerInclude.cginc"
            #pragma vertex leaves_gpui
            #pragma fragment frag
            #pragma multi_compile_fog
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:setupGPUI
            #include "UnityBuiltin2xTreeLibrary.cginc"

			sampler2D _MainTex;
            fixed _Cutoff;

			v2f leaves_gpui(appdata_tree v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				//TerrainAnimateTree(v.vertex, v.color.w);

				float3 viewpos = UnityObjectToViewPos(v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;

				float4 lightDir = 0;
				float4 lightColor = 0;
				lightDir.w = _AO;

				float4 light = UNITY_LIGHTMODEL_AMBIENT;
				for (int i = 0; i < 4; i++) {
					float atten = 1.0;

					float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					toLight.z *= -1.0;
					lightDir.xyz = mul( (float3x3)unity_CameraToWorld, normalize(toLight) );
					float lengthSq = dot(toLight, toLight);
					atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);

					lightColor.rgb = unity_LightColor[i].rgb;

					lightDir.xyz *= _Occlusion;
					float occ =  dot (v.tangent, lightDir);
					occ = max(0, occ);
					occ += _BaseLight;
					light += lightColor * (occ * atten);
				}
				
				float4 col = frac((unity_ObjectToWorld._m03_m13_m23.x + unity_ObjectToWorld._m03_m13_m23.z)) * 0.4 + 0.6;
				col.a = 1;
				o.color = light * _Color * col;
				o.color.a = 0.5 * _HalfOverCutoff;

				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag(v2f input) : SV_Target
            {
                fixed4 c = tex2D( _MainTex, input.uv.xy);
                c.rgb *= input.color.rgb;

                clip (c.a - _Cutoff);
                UNITY_APPLY_FOG(input.fogCoord, c);
                return c;
            }
            ENDCG
        }

        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            CGPROGRAM
			#include "UnityCG.cginc"
			#include "../../Include/GPUInstancerInclude.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:setupGPUI
            #include "UnityCG.cginc"
            #include "TerrainEngine.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
                float4 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            v2f vert( appdata v )
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                //TerrainAnimateTree(v.vertex, v.color.w);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                o.uv = v.texcoord;
                return o;
            }

            sampler2D _MainTex;
            fixed _Cutoff;

            float4 frag( v2f i ) : SV_Target
            {
                fixed4 texcol = tex2D( _MainTex, i.uv );
                clip( texcol.a - _Cutoff );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }

    // This subshader is never actually used, but is only kept so
    // that the tree mesh still assumes that normals are needed
    // at build time (due to Lighting On in the pass). The subshader
    // above does not actually use normals, so they are stripped out.
    // We want to keep normals for backwards compatibility with Unity 4.2
    // and earlier.
    SubShader {
        Tags {
            "Queue" = "AlphaTest"
            "IgnoreProjector"="True"
            "RenderType" = "TransparentCutout"
        }
        Cull Off
        ColorMask RGB
        Pass {
            Tags { "LightMode" = "Vertex" }
            AlphaTest GEqual [_Cutoff]
            Lighting On
            Material {
                Diffuse [_Color]
                Ambient [_Color]
            }
            SetTexture [_MainTex] { combine primary * texture DOUBLE, texture }
        }
    }
}
