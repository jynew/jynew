Shader "Hidden/WeightedBlendNode"
{
	Properties
	{
		_A ( "_Weights", 2D) = "white" {}
		_B ( "_Layer1", 2D) = "white" {}
		_C ( "_Layer2", 2D ) = "white" {}
		_D ( "_Layer3", 2D ) = "white" {}
		_E ( "_Layer4", 2D ) = "white" {}
	}

	SubShader
	{

		CGINCLUDE
		
		#include "UnityCG.cginc"
		#pragma vertex vert_img
		#pragma fragment frag
		sampler2D _A;
		sampler2D _B;
		
		ENDCG

		Pass
		{
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 Layer1 = tex2D( _B, i.uv );
				return Layer1;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			sampler2D _C;
			float4 frag ( v2f_img i ) : SV_Target
			{
				float4 Weights = tex2D ( _A, i.uv );
				float4 Layer1 = tex2D ( _B, i.uv );
				float4 Layer2 = tex2D ( _C, i.uv );
				return ( Weights.x*Layer1 + Weights.y*Layer2 ) / ( Weights.x + Weights.y );
			}
			ENDCG
		}
		
		Pass
		{
			CGPROGRAM
			sampler2D _C;
			sampler2D _D;
			float4 frag ( v2f_img i ) : SV_Target
			{
				float4 Weights = tex2D( _A, i.uv );
				float4 Layer1 = tex2D( _B, i.uv );
				float4 Layer2 = tex2D( _C, i.uv );
				float4 Layer3 = tex2D( _D, i.uv );
				return ( Weights.x*Layer1 + Weights.y*Layer2 + Weights.z*Layer3 ) / ( Weights.x + Weights.y + Weights.z );
			}
			ENDCG
		}
		
		Pass
		{
			CGPROGRAM
			sampler2D _C;
			sampler2D _D;
			sampler2D _E;
			float4 frag ( v2f_img i ) : SV_Target
			{
				float4 Weights = tex2D ( _A, i.uv );
				float4 Layer1 = tex2D ( _B, i.uv );
				float4 Layer2 = tex2D ( _C, i.uv );
				float4 Layer3 = tex2D ( _D, i.uv );
				float4 Layer4 = tex2D ( _E, i.uv );
				return ( Weights.x*Layer1 + Weights.y*Layer2 + Weights.z*Layer3 + Weights.w*Layer4 ) / ( Weights.x + Weights.y + Weights.z + Weights.w );
			}
			ENDCG
		}
	}
}
