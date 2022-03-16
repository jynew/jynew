// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SkillEffect/GhostShadow"
{
	Properties
	{
		_RimColor("RimColor", Color) = (0, 0, 1, 1)
		_RimIntensity("Intensity", Range(0, 2)) = 1
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Opaque" }

		LOD 200
		
		// 写入深度，修复龙透明效果不正确问题
		Pass
		{
		    ColorMask 0
		}
		
		Pass
		{
		Blend SrcAlpha One//打开混合模式
		ZWrite off
		Lighting off

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal:Normal;
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
		};

		fixed4 _RimColor;
		float _RimIntensity;

		v2f vert(appdata v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));//计算出顶点到相机的向量
			float val = 1 - saturate(dot(v.normal, viewDir));//计算点乘值
			o.color = _RimColor * val * (1 + _RimIntensity);//计算强度
			return o;
		}

		fixed4 frag(v2f i) : COLOR
		{
			return i.color;
		}
		ENDCG
	}
	}
}