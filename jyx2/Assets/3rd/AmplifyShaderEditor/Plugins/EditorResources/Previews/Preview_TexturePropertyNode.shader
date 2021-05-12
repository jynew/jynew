Shader "Hidden/TexturePropertyNode"
{
	Properties
	{
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

			UNITY_DECLARE_TEX2DARRAY( _Array );
			samplerCUBE _Cube;
			sampler2D _Sampler;
			sampler3D _Sampler3D;
			int _Default;
			int _Type;

			float4 frag( v2f_img i ) : SV_Target
			{
				if(_Default == 1)
				{
					return 1;
				}
				else if(_Default == 2)
				{
					return 0;
				} 
				else if(_Default == 3)
				{
					return 0.5f;
				}
				else if(_Default == 4)
				{
					return float4(0.5,0.5,1,1);
				}
				else 
				{
					if( _Type == 4 )
						return UNITY_SAMPLE_TEX2DARRAY( _Array, float3( i.uv, 0 ) );
					else if( _Type == 3 )
						return texCUBE( _Cube, float3(i.uv,0) );
					else if( _Type == 2 )
						return tex3D( _Sampler3D, float3(i.uv,0) );
					else
						return tex2D( _Sampler, i.uv);
				}
			}
			ENDCG
		}
	}
}
