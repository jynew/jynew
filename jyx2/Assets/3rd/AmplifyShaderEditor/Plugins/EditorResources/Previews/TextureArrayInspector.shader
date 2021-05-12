Shader "Hidden/TextureArrayEditor"
{
	Properties
	{
		_MainTex ("_MainTex", 2DArray) = "white" {}
		_Index ("_Index", Int) = 0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.5
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"

			uniform UNITY_DECLARE_TEX2DARRAY( _MainTex );
			int _Index;

			float4 frag( v2f_img i ) : SV_Target
			{
				//return UNITY_SAMPLE_TEX2DARRAY_LOD( _MainTex, float3( i.uv, _Index), 0 );
				return UNITY_SAMPLE_TEX2DARRAY( _MainTex, float3( i.uv, _Index) );
			}
			ENDCG
		}
	}
}
