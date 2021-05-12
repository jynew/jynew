Shader "Hidden/SamplerNode"
{
	Properties
	{
		_B ("_UVs", 2D) = "white" {}
		_C ("_Level", 2D) = "white" {}
		_F ("_NormalScale", 2D) = "white" {}
		_CustomUVs ("_CustomUVs", Int) = 0
		_Unpack ("_Unpack", Int) = 0
		_LodType ("_LodType", Int) = 0

		_Sampler ("_Sampler", 2D) = "white" {}
		_Sampler3D ("_Sampler3D", 3D) = "white" {}
		_Array ("_Array", 2DArray) = "white" {}
		_Cube( "_Cube", CUBE) = "white" {}
		_Default ("_Default", Int) = 0
		_Type ("_Type", Int) = 0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma exclude_renderers d3d9 
			#pragma target 3.5
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"

			sampler2D _B;
			sampler2D _C;
			sampler2D _F;
			int _CustomUVs;
			int _Unpack;
			int _LodType;

			UNITY_DECLARE_TEX2DARRAY( _Array );
			samplerCUBE _Cube;
			sampler2D _Sampler;
			sampler3D _Sampler3D;
			int _Default;
			int _Type;

			float4 frag( v2f_img i ) : SV_Target
			{
				if( _Default == 1 )
				{
					return 1;
				}
				else if( _Default == 2 )
				{
					return 0;
				} 
				else if( _Default == 3 )
				{
					return 0.5f;
				}
				else if( _Default == 4 )
				{
					float4 h = float4(0.5,0.5,1,1);
					if ( _Unpack == 1 ) {
						h.rgb = UnpackScaleNormal( h.xxyy, tex2D( _F, i.uv ).r );
					} 
					return h;
				}
				else 
				{
					if( _Type == 4 )
						return UNITY_SAMPLE_TEX2DARRAY( _Array, float3( i.uv, 0 ) );
					else if( _Type == 3 )
					{
						float3 uvs = float3(i.uv,0);

						if ( _CustomUVs == 1 )
							uvs = tex2D( _B, i.uv ).xyz;

						return texCUBE( _Cube, uvs );
					}
					else if( _Type == 2 )
					{
						return tex3D( _Sampler3D, float3(i.uv,0) );
					}
					else
					{
						float2 uvs = i.uv;
						float4 c = 0;

						if ( _CustomUVs == 1 )
							uvs = tex2D( _B, i.uv ).xy;
						
						if ( _LodType == 1 ) 
						{
							float lod = tex2D( _C, i.uv ).r;
							c = tex2Dlod( _Sampler, float4(uvs,0,lod) );
						}
						else if ( _LodType == 2 ) 
						{
							float bias = tex2D( _C, i.uv ).r;
							c = tex2Dbias( _Sampler, float4(uvs,0,bias) );
						}
						else 
						{
							c = tex2D( _Sampler, uvs );
						}

						if ( _Unpack == 1 ) {
							float nscale = tex2D( _F, i.uv ).r;
							c.rgb = UnpackScaleNormal( c, nscale );
						} 

						return c;
					}
				}
			}
			ENDCG
		}
	}
}
