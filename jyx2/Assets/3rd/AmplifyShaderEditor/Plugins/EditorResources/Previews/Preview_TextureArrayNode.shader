Shader "Hidden/TextureArrayNode"
{
	Properties
	{
		_A ("_UVs", 2D) = "white" {}
		_B ("_Index", 2D) = "white" {}
		_C ("_Lod", 2D) = "white" {}
		_D ("_NormalScale", 2D) = "white" {}
		_G ("_Tex", 2D) = "white" {}
		_TexConnected ("_TexConnected", Int) = 0
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

			uniform UNITY_DECLARE_TEX2DARRAY( _Sampler );
			sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;
			sampler2D _G;
			float _CustomUVs;
			float _LodType;
			float _Unpack;
			int _TexConnected;

			float4 frag( v2f_img i ) : SV_Target
			{
				float2 uvs = i.uv;
				if ( _CustomUVs == 1 )
					uvs = tex2D( _A, i.uv ).xy;

				float n = tex2D( _D, i.uv ).r;
				float4 c = 0;
				if ( _LodType == 1 ) {
					float lod = tex2D( _C, i.uv ).r;
					c = UNITY_SAMPLE_TEX2DARRAY_LOD( _Sampler, float3( uvs, tex2D( _B, i.uv ).r ), lod );
				}
				else if( _TexConnected == 0) {
					c = UNITY_SAMPLE_TEX2DARRAY( _Sampler, float3( uvs, tex2D( _B, i.uv ).r ) );
				} 
				else {
					c = tex2D( _G, uvs );
				}

				if ( _Unpack == 1 ) 
				{
					c.rgb = UnpackScaleNormal(c, n);
				} 

				return c;
			}
			ENDCG
		}
	}
}
