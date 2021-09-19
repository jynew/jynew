Shader "GPUInstancer/Billboard/2DRendererSoftOcclusion" {
    Properties {
		_AlbedoAtlas ("Albedo Atlas", 2D) = "white" {}
		_NormalAtlas("Normal Atlas", 2D) = "white" {}
		_Cutoff ("Cutoff", Range(0,1)) = 0.3
		_FrameCount("_FrameCount", Float) = 8
        
		_Color ("Main Color", Color) = (1,1,1,1)
        //_BaseLight ("Base Light", Range(0, 1)) = 0.35
        _AO ("Amb. Occlusion", Range(0, 10)) = 2.4
        _Occlusion ("Dir Occlusion", Range(0, 20)) = 7.5

		[Toggle(_BILLBOARDFACECAMPOS_ON)] _BillboardFaceCamPos("BillboardFaceCamPos", Float) = 0
    }

    SubShader {
		Tags {
			"Queue" = "AlphaTest"
            "IgnoreProjector"="True"
            "RenderType" = "TransparentCutout"
            "DisableBatching"="True"
        }
        Cull Off
        ColorMask RGB
        Pass {
            Lighting On
			
            CGPROGRAM

		
			#include "UnityCG.cginc"
			#include "../Include/GPUInstancerInclude.cginc"
			#include "../Include/GPUIBillboardInclude.cginc"
            #pragma vertex leaves_gpui
            #pragma fragment frag
            #pragma multi_compile_fog
			#pragma multi_compile __ _BILLBOARDFACECAMPOS_ON
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:setupGPUI
            #include "UnityBuiltin2xTreeLibrary.cginc"

			#pragma target 3.5

			sampler2D _AlbedoAtlas;
			sampler2D _NormalAtlas;
			float _Cutoff;
			float _FrameCount;

			struct appdata_tree_gpui {
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f_gpui {
				float4 pos : SV_POSITION;
				float2 atlasUV : TEXCOORD0;
				half4 color : TEXCOORD1;

				float3 tangentWorld : TEXCOORD2;
				float3 bitangentWorld : TEXCOORD3;
				float3 normalWorld : TEXCOORD4;


				UNITY_FOG_COORDS(5)
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f_gpui leaves_gpui(appdata_tree_gpui v)
			{
				v2f_gpui o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				//TerrainAnimateTree(v.vertex, v.color.w);

				float3 viewpos = UnityObjectToViewPos(v.vertex);
				
				GPUIBillboardVertex(v.vertex, v.normal, v.tangent, o.tangentWorld, o.bitangentWorld, o.normalWorld, v.texcoord, o.atlasUV, _FrameCount);
				o.pos = UnityObjectToClipPos(v.vertex);

				float4 lightDir = 0;
				float4 lightColor = 0;
				lightDir.w = _AO;

				float4 light = UNITY_LIGHTMODEL_AMBIENT;
				for (int i = 0; i < 4; i++) {
					lightColor.rgb = unity_LightColor[i].rgb * (1 - unity_LightPosition[i].w);
					light += lightColor;
				}
				
				float4 col = frac((unity_ObjectToWorld._m03_m13_m23.x + unity_ObjectToWorld._m03_m13_m23.z)) * 0.4 + 0.6;
				col.a = 1;
				o.color = light * col;

				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag(v2f_gpui input) : SV_Target
            {
				half4 c = tex2D (_AlbedoAtlas, input.atlasUV);
				c.rgb *= input.color.rgb;
				clip(c.a - _Cutoff);
				c.a = 1;
				UNITY_APPLY_FOG(input.fogCoord, c);

				return c;

				/*
				half4 normalDepth = GPUIBillboardNormals(_NormalAtlas, input.atlasUV, _FrameCount, input.tangentWorld, input.bitangentWorld, input.normalWorld);
				fixed diff = min (1, dot (-normalDepth.xyz, _WorldSpaceLightPos0.xyz));
				return half4(c.rgb * (1 - diff), c.a);
				*/
				
            }
            ENDCG
        }
	}
}