Shader "Hidden/DiffuseAndSpecularFromMetallicNode"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
		_B ("_B", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
		
			float4 frag( v2f_img i ) : SV_Target
			{
				float4 albedo = tex2D( _A, i.uv );
				float metallic = tex2D( _A, i.uv ).r;
				float3 specColor = 0;
				float oneMinusReflectivity;
				float3 albedoFinal = DiffuseAndSpecularFromMetallic(albedo,metallic,specColor,oneMinusReflectivity);
				return float4( albedoFinal , 1 );
			}
			ENDCG
		}
	}
}
