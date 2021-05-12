Shader "Easy Decal/Legacy/Reflect Bumped Specular Cut" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_Light ("Light Intesity", Range (0, 1)) = 0.5
		_ReflectColor ("Reflec. Color (RGB) Reflec. Intensity (A)", Color) = (1,1,1,0.5)
		_MainTex ("Base (RGB) Transparent (A)", 2D) = "white" {}
		_Cube ("Reflection Cubemap", Cube) = "" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_SpecMap ("Reflection (RGB) Spec (A) ", 2D) = "white" {}
		_Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
	}

	SubShader 
	{
		Offset -1,-1		

		LOD 400
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 3.0


		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _SpecMap;
		samplerCUBE _Cube;

		half _Light;
		fixed4 _Color;
		fixed4 _ReflectColor;
		half _Shininess;
		half _Cutoff;

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldRefl;
			INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 spec = tex2D(_SpecMap, IN.uv_MainTex);
			fixed4 c = tex * _Color;
			fixed3 cc = _LightColor0;

			clip(c.a - _Cutoff);

			if(_Light <= .001)
			{
				cc = 1;
			}
			else
			{
				cc = _LightColor0 * (1-_Light);
			}

			o.Albedo = c.rgb;
			o.Gloss = spec.a;
			o.Specular = _Shininess;	
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

			float3 worldRefl = WorldReflectionVector (IN, o.Normal);
			fixed4 reflcol = texCUBE (_Cube, worldRefl);


			reflcol *= _ReflectColor.a;

			o.Emission = ((c.rgb * cc) + (reflcol.rgb * _ReflectColor.rgb * (1 - spec.r)));
		}
		ENDCG
	}

	//FallBack "Reflective/Bumped Diffuse"
	FallBack "Transparent/VertexLit"
}
