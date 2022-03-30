// alpha-cutout billboard shader

Shader "MTE/Grass/Billboard"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Grass Texture", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}

	CGINCLUDE
		#pragma surface surf Lambert vertex:vert alphatest:_Cutoff addshadow

		uniform sampler2D _MainTex;
		uniform fixed4 _Color;

		struct VertexInput
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
		};

		struct Input
		{
			float4 position : SV_POSITION;
			float2 uv_MainTex;
		};

		void vert(inout VertexInput v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			// apply object scale
			v.vertex.xy *= float2(length(unity_ObjectToWorld._m00_m10_m20), length(unity_ObjectToWorld._m01_m11_m21));

			// get the camera basis vectors
			float3 forward = -normalize(UNITY_MATRIX_V._m20_m21_m22);
			float3 up = normalize(UNITY_MATRIX_V._m10_m11_m12);
			float3 right = normalize(UNITY_MATRIX_V._m00_m01_m02);

			// rotate to face camera
			float4x4 rotationMatrix = float4x4(right, 0,
											   up, 0,
											   forward, 0,
											   0, 0, 0, 1);
			v.vertex = mul(v.vertex, rotationMatrix);
			v.normal = mul(v.normal, (float3x3)rotationMatrix);

			// undo object to world transform surface shader will apply
			v.vertex.xyz = mul((float3x3)unity_WorldToObject, v.vertex.xyz);
			v.normal = mul(v.normal, (float3x3)unity_ObjectToWorld);

			o.uv_MainTex = v.texcoord;
			o.position = v.vertex;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
	ENDCG

	Category
	{
		Tags
		{
			"Queue" = "AlphaTest"
			"IgnoreProjector" = "True"
			"DisableBatching" = "True"
			"RenderType" = "TransparentCutout"
		}
		SubShader//for target 3.0+
		{
			CGPROGRAM
				#pragma target 3.0
			ENDCG
		}
		SubShader//for target 2.5
		{
			CGPROGRAM
				#pragma target 2.5
			ENDCG
		}
		SubShader//for target 2.0
		{
			CGPROGRAM
				#pragma target 2.0
			ENDCG
		}
	}
}
