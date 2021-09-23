// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "GPUInstancer/Custom/Bigmap_Items" {
	Properties {
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MainTex("MainTex", 2D) = "white"{}
		_Color("Main Color",color) = (1,1,1,1)
		_Outline("Thick of Outline",range(0,0.1)) = 0.03//挤出描边的粗细
		_Factor("Factor",range(0,1)) = 0.5//挤出多远
		_OutColor("OutColor",color) = (0,0,0,0)
		_ToonEffect("Toon Effect",range(0,1)) = 0.5
		_Steps("Steps of toon",range(0,9)) = 3
		_RimPower("RimPower",range(0,1)) = 1
		_ToonRimStep("ToonRimStep",range(0,5)) = 1
		_IllumBool("IllumBool",range(0,1)) = 0
		_Illum("Illumin (A)", 2D) = "white" { }
	}
	SubShader {
		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" "DisableBatching"="LODFading"}

		pass {//处理光照前的pass渲染
			Tags{ "LightMode" = "Always" }
			Cull Front
			ZWrite On
			CGPROGRAM
#include "UnityCG.cginc"
#include "./../../3rd/GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc"
#pragma instancing_options procedural:setupGPUI
#pragma multi_compile_instancing
			#pragma multi_compile_fog
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _Outline;
			float _Factor;
			fixed4 _OutColor;
			struct v2f {
				float4 pos:SV_POSITION;
				UNITY_FOG_COORDS(0)
			};

			v2f vert(appdata_full v) {
				v2f o;
				float3 dir = normalize(v.vertex.xyz);
				float3 dir2 = v.normal;
				float D = dot(dir,dir2);
				dir = dir * sign(D);
				dir = dir * _Factor + dir2 * (1 - _Factor);
				v.vertex.xyz += dir * _Outline;
				o.pos = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}
			float4 frag(v2f i) :COLOR
			{
				float4 c = _OutColor;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}

		CGPROGRAM
#include "UnityCG.cginc"
#include "./../../3rd/GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc"
#pragma instancing_options procedural:setupGPUI
#pragma multi_compile_instancing
		#include "UnityPBSLighting.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Cartoon fullforwardshadows //vertex:vert
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_Illum;
			float3 normal:TEXCOORD1;
		};

		fixed4 _Color;
		float _Steps;
		float _ToonEffect;
		sampler2D _MainTex;
		sampler2D _Illum;
		float _RimPower;
		float _ToonRimStep;
		fixed _IllumBool;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		inline float4 LightingCartoon(SurfaceOutputStandard s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			//暂时光的方向反了，使用了-normalize(lightDir)
			float difLight = max(0, dot(normalize(s.Normal), normalize(lightDir)));
			difLight = (difLight + 1) / 2;//做亮化处理
			difLight = smoothstep(0, 1, difLight);//使颜色平滑的在[0,1]范围之内
			float toon = floor(difLight * _Steps) / _Steps;//把颜色做离散化处理，把diffuse颜色限制在_Steps种（_Steps阶颜色），简化颜色，这样的处理使色阶间能平滑的显示
			difLight = lerp(difLight, toon, _ToonEffect);//根据外部我们可控的卡通化程度值_ToonEffect，调节卡通与现实的比重
			float4 col;
			col.rgb = s.Albedo * _LightColor0.rgb * difLight * atten;
			col.a = s.Alpha;
			return col;
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			if (_IllumBool > 0.5) 
			{
				o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a;
			}
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
