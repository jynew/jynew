Shader "SkillEffect/Character" 
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white"{}
		_Color("Main Color",color) = (1,1,1,1)
		_ColorValue("Factor",range(0,5)) = 1
		_ToonEffect("Toon Effect",range(0,1)) = 0.5
		_Outline("Thick of Outline",range(0,0.1)) = 0.01//挤出描边的粗细
		_Factor("Factor",range(0,1)) = 0.5//挤出多远
		_OutColor("OutColor",color) = (0,0,0,0)
		_ColorPower("ColorPower", Range(0.000001, 3.0)) = 1.5
		_Steps("Steps of toon",range(0,9)) = 3
		_RimColor("RimColor", Color) = (1,1,1,1)
		_RimPower("RimPower", Range(0.000001, 3.0)) = 0.3
		//遮挡部分rimlight参数
		//_XRayFactor("XRayFactor", Range(0, 1)) = 1
		//_XRayColor("XRayColor", Color) = (0,0.5,1,1)
		//_XRayPower("XRayPower", Range(0.000001, 3.0)) = 1
	}

	SubShader
	{
		Tags{ "Queue" = "Geometry+10" "RenderType" = "Opaque" }
		//渲染X光效果的Pass  
		/*
		Pass
		{
			Tags{ "ForceNoShadowCasting" = "true" }
			Blend SrcAlpha One
			ZWrite Off
			ZTest Greater
			CGPROGRAM
			#include "Lighting.cginc"  

			float4 _XRayColor;
			float _XRayPower;
			float _XRayFactor;
			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : normal;
				float3 viewDir : TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.viewDir = ObjSpaceViewDir(v.vertex);
				o.normal = v.normal;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 normal = normalize(i.normal);
				float3 viewDir = normalize(i.viewDir);
				float rim = 1 - max(0, dot(normal, viewDir));
				float4 rst = _XRayColor * pow(rim, 1 / _XRayPower);
				if(_XRayFactor==0)
					rst.a = 0;
				return rst;
			}
			#pragma vertex vert  
			#pragma fragment frag  
			ENDCG
		}
		*/
		pass {//处理光照前的pass渲染，描边
		Tags{ "LightMode" = "Always"   "ForceNoShadowCasting" = "true" }
		Cull Front
		ZWrite On
		CGPROGRAM
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
			v.vertex.xyz += dir * _Outline * v.color.r;
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
		#include "UnityPBSLighting.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Cartoon fullforwardshadows //vertex:vert
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_NormalTex;
			float3 normal:TEXCOORD1;
		};

		fixed4 _Color;
		float _Steps;
		float _ToonEffect;
		sampler2D _MainTex;
		float _RimPower;
		float _ToonRimStep;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
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
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}