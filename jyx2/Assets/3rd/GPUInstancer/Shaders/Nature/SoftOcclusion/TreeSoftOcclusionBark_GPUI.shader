Shader "GPUInstancer/Nature/Tree Soft Occlusion Bark" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,0)
        _MainTex ("Main Texture", 2D) = "white" {}
        _BaseLight ("Base Light", Range(0, 1)) = 0.35
        _AO ("Amb. Occlusion", Range(0, 10)) = 2.4
    }

    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "RenderType" = "TreeOpaque"
            "DisableBatching"="True"
        }

        Pass {
            Lighting On

            CGPROGRAM
            #include "UnityCG.cginc"
			#include "../../Include/GPUInstancerInclude.cginc"
			#pragma vertex bark_gpui
            #pragma fragment frag
            #pragma multi_compile_fog
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:setupGPUI
            #include "UnityBuiltin2xTreeLibrary.cginc"

            sampler2D _MainTex;

			v2f bark_gpui(appdata_tree v)
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

					float diffuse = dot (v.normal, lightDir.xyz);
					diffuse = max(0, diffuse);
					diffuse *= _AO * v.tangent.w + _BaseLight;
					light += lightColor * (diffuse * atten);
				}

				light.a = 1;
				float4 col = frac((unity_ObjectToWorld._m03_m13_m23.x + unity_ObjectToWorld._m03_m13_m23.z)) * 0.4 + 0.6;
				col.a = 1;
				o.color = light * _Color * col;


				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 col = input.color;
                col.rgb *= tex2D( _MainTex, input.uv.xy).rgb;
                UNITY_APPLY_FOG(input.fogCoord, col);
                UNITY_OPAQUE_ALPHA(col.a);
                return col;
            }
            ENDCG
        }

        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:setupGPUI
            #include "UnityCG.cginc"
			#include "../../Include/GPUInstancerInclude.cginc"
            #include "TerrainEngine.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert( appdata v )
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                //TerrainAnimateTree(v.vertex, v.color.w);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag( v2f i ) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}