// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BigMap" {
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_MainTexTwo("MainTex2", 2D) = "white" {}
		_ThreValue("ThreValue", range(0,1)) = 0
		_ToonEffect("Toon Effect",range(0,1)) = 0.5
		_Steps("Steps of toon",range(0,9)) = 3
		//_BumpMap ("Normal Map", 2D) = "bump" {}
		//_BumpScale ("Bump Scale", Range(0.0, 1.0)) = 0.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }

		LOD 150

		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Cartoon fullforwardshadows //vertex:vert
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		//#pragma surface surf Lambert noforwardadd

		sampler2D _MainTex;
		sampler2D _MainTexTwo;
		fixed _ThreValue;
		float _ToonEffect;
		float _Steps;
		//sampler2D _BumpMap;
		//float _BumpScale;

		struct Input 
		{
			float2 uv_MainTex;
		};
		
		inline float4 LightingCartoon(SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			float difLight = max(0, dot(normalize(s.Normal), normalize(lightDir)));
			//difLight = (difLight + 1) / 2;//做亮化处理
			//difLight = smoothstep(0, 1, difLight);//使颜色平滑的在[0,1]范围之内
			float toon = floor(difLight * _Steps) / _Steps;//把颜色做离散化处理，把diffuse颜色限制在_Steps种（_Steps阶颜色），简化颜色，这样的处理使色阶间能平滑的显示
			difLight = lerp(difLight, toon, _ToonEffect);//根据外部我们可控的卡通化程度值_ToonEffect，调节卡通与现实的比重
			float4 col;
			col.rgb = s.Albedo * _LightColor0.rgb * difLight * atten;
			col.a = s.Alpha;
			return col;
		}

		void surf(Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c2 = tex2D(_MainTexTwo, IN.uv_MainTex);
			c = c * (1 - _ThreValue) + c2 * _ThreValue;
			//c = fixed4(0,0,1,1);
			o.Albedo = c.rgb;
			o.Alpha = c.a;

			//fixed4 c3 = tex2D(_BumpMap, IN.uv_MainTex);
			//o.Normal = c3.rgb * 2.0 - 1.0;
			//o.Normal.z = _BumpScale;

		}
		ENDCG
	}

	//Fallback "Specular"
	Fallback "Mobile/VertexLit"
}

