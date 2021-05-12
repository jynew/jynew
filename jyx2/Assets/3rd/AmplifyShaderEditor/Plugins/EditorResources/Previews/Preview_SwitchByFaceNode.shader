Shader "Hidden/SwitchByFaceNode"
{
	Properties
	{
		_A ("_Front", 2D) = "white" {}
		_B ("_Back", 2D) = "white" {}
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
			sampler2D _B;

			float4 frag( v2f_img i, half ase_vface : VFACE ) : SV_Target
			{
				float4 front = tex2D( _A, i.uv );
				float4 back = tex2D( _B, i.uv );
				return ( ( ase_vface > 0 ) ? ( front ) : ( back ) );
			}
			ENDCG
		}
	}
}
