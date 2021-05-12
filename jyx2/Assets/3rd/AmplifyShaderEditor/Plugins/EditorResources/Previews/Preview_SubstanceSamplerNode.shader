Shader "Hidden/SubstanceSamplerNode"
{
	Properties
	{
		_A ("_UV", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _GenTex;
			int _CustomUVs;

			float4 frag( v2f_img i ) : SV_Target
			{
				float2 uvs = i.uv;
				if( _CustomUVs == 1 )
					uvs = tex2D( _A, i.uv ).xy;
				float4 genTex = tex2D( _GenTex, uvs);
				return genTex;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _GenTex;
			int _CustomUVs;

			float4 frag( v2f_img i ) : SV_Target
			{
				float2 uvs = i.uv;
				if( _CustomUVs == 1 )
					uvs = tex2D( _A, i.uv ).xy;
				float3 genTex = UnpackNormal( tex2D( _GenTex, uvs ) );
				return float4( genTex, 0 );
			}
			ENDCG
		}
	}
}
