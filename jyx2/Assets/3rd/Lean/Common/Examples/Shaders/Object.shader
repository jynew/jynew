Shader "Lean/Common/Object"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Color1("Color 1", Color) = (1.0, 0.5, 0.5, 1.0)
		_Color2("Color 2", Color) = (0.5, 0.5, 1.0, 1.0)
		_Rim("Rim", Float) = 1.0
		_Shift("Shift", Float) = 1.0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Geometry"
			"PreviewType" = "Sphere"
			"DisableBatching" = "True"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			sampler2D _MainTex;
			float4    _Color;
			float4    _Color1;
			float4    _Color2;
			float     _Rim;
			float     _Shift;

			struct a2v
			{
				float4 vertex    : POSITION;
				float2 texcoord0 : TEXCOORD0;
				float3 normal    : NORMAL;
				float4 color     : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv     : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float4 color  : COLOR;
			};

			struct f2g
			{
				float4 color : SV_TARGET;
			};

			void Vert(a2v i, out v2f o)
			{
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv     = i.texcoord0;
				o.normal = mul((float3x3)UNITY_MATRIX_IT_MV, i.normal);
				o.color  = i.color * _Color;
			}

			void Frag(v2f i, out f2g o)
			{
				float rim = _Shift - pow(saturate(1.0f - normalize(i.normal).z), _Rim);

				o.color = tex2D(_MainTex, i.uv) * lerp(_Color1, _Color2, rim) * i.color;
			}
			ENDCG
		} // Pass
	} // SubShader
} // Shader