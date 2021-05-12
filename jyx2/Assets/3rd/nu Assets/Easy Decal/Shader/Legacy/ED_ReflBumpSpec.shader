Shader "Easy Decal/Legacy/Reflect Bumped Specular [Depracated]" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_ReflectColor ("Reflec. Color (RGB) Reflec. Intensity (A)", Color) = (1,1,1,0.5)
		_MainTex ("Base (RGB) Transparent (A)", 2D) = "white" {}
		_Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_SpecMap ("Reflection (RGB) Spec (A) ", 2D) = "white" {}
	}

	SubShader 
	{
		Cull Off
		Blend DstColor Zero
		Offset -1,-1
		ZTest LEqual

		Tags { Queue = Transparent}
		LOD 400

		CGPROGRAM
		#pragma surface surf BlinnPhong alpha
		#pragma target 3.0
		#pragma exclude_renderers d3d11_9x
		#pragma multi_compile_fwdadd_fullshadows

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _SpecMap;
		samplerCUBE _Cube;

		fixed4 _Color;
		fixed4 _ReflectColor;
		half _Shininess;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldRefl;
			INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float3 worldRefl = WorldReflectionVector (IN, o.Normal);

			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 spec = tex2D(_SpecMap, IN.uv_MainTex);
			fixed4 reflcol = texCUBE (_Cube, worldRefl);

			fixed4 c = tex * _Color;


			o.Albedo = c.rgb;	
			o.Gloss = spec.a;
			o.Specular = _Shininess;	
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Alpha = tex.a;
	

			reflcol *= _ReflectColor.a;
			o.Emission = ((tex.rgb * (1 - spec.r)) + ((reflcol.rgb * _ReflectColor.rgb) * (1 - spec.r)));

		}
		ENDCG
	}


	FallBack "Transparent/VertexLit"
}
