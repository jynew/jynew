Shader "GPUInstancer/Billboard/2DRendererTreeCreator" {
	Properties {
		_AlbedoAtlas ("Albedo Atlas", 2D) = "white" {}
		_NormalAtlas("Normal Atlas", 2D) = "white" {}
		_Cutoff ("Cutoff", Range(0,1)) = 0.3
		_FrameCount("FrameCount", Float) = 8

		_TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1)
		_TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
		_ShadowStrength("Shadow Strength", Range(0,1)) = 0.8

		[Toggle(_BILLBOARDFACECAMPOS_ON)] _BillboardFaceCamPos("BillboardFaceCamPos", Float) = 0
	}
	SubShader {
		Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" "DisableBatching"="True" } //"ForceNoShadowCasting" = "True" }
		LOD 400
		CGPROGRAM

		sampler2D _AlbedoAtlas;
		sampler2D _NormalAtlas;
		float _Cutoff;
		float _FrameCount;

		fixed3 _TranslucencyColor;
		fixed _TranslucencyViewDependency;
		half _ShadowStrength;

		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "../Include/GPUInstancerInclude.cginc"
		#include "../Include/GPUIBillboardInclude.cginc"

		#pragma multi_compile __ _BILLBOARDFACECAMPOS_ON
		#pragma instancing_options procedural:setupGPUI
		#pragma surface surf TreeLeaf vertex:vert nolightmap noforwardadd addshadow //exclude_path:deferred
		#pragma multi_compile _ LOD_FADE_CROSSFADE
		#pragma target 4.5

		struct Input {
			float2 atlasUV;
			float3 tangentWorld;
            float3 bitangentWorld;
            float3 normalWorld;
		};

		struct LeafSurfaceOutput {
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			fixed Translucency;
			fixed Alpha;
			float Depth;
		};
		
		inline half4 LightingTreeLeaf(LeafSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 h = normalize (lightDir + viewDir);

			half nl = dot (s.Normal, lightDir);

			// view dependent back contribution for translucency
			fixed backContrib = saturate(dot(viewDir, -lightDir));

			// normally translucency is more like -nl, but looks better when it's view dependent
			backContrib = lerp(saturate(-nl), backContrib, _TranslucencyViewDependency);

			fixed3 translucencyColor = backContrib * s.Translucency * _TranslucencyColor;

			// wrap-around diffuse
			nl = max(0, nl * 0.6 + 0.4);

			fixed4 c;
			c.rgb = s.Albedo * (translucencyColor * 2 + nl);
			c.rgb = c.rgb * _LightColor0.rgb;// + spec;
			c.rgb = lerp (c.rgb, float3(0,0,0), s.Depth * 1.75);

			// For directional lights, apply less shadow attenuation
			// based on shadow strength parameter.
			#if defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)
			c.rgb *= lerp(1, atten, _ShadowStrength);
			#else
			c.rgb *= atten;
			#endif

			c.a = s.Alpha;

			return c;
		}

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
		
			GPUIBillboardVertex(v.vertex, v.normal, v.tangent, o.tangentWorld, o.bitangentWorld, o.normalWorld, v.texcoord, o.atlasUV, _FrameCount);
		}

		void surf (Input IN, inout LeafSurfaceOutput o) {

			half4 c = tex2D (_AlbedoAtlas, IN.atlasUV);
			clip(c.a - 0.5);
			
			half4 normalDepth = GPUIBillboardNormals(_NormalAtlas, IN.atlasUV, _FrameCount, IN.tangentWorld, IN.bitangentWorld, IN.normalWorld);
			o.Normal = -normalDepth.xyz;
			o.Albedo = c.rgb;
			o.Depth = normalDepth.w;

			o.Translucency = 1; //TO-DO: use as property
			o.Alpha = c.a;
		}
		ENDCG
	}

}