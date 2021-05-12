Shader "Hidden/LayeredBlendNode"
{
	Properties
	{
		_A ( "_Weights", 2D) = "white" {}
		_B ( "_LayerBase", 2D) = "white" {}
		_C ( "_Layer1", 2D) = "white" {}
		_D ( "_Layer2", 2D ) = "white" {}
		_E ( "_Layer3", 2D ) = "white" {}
		_F ( "_Layer4", 2D ) = "white" {}
	}
	SubShader
	{

		CGINCLUDE
		
		#include "UnityCG.cginc"
		#pragma vertex vert_img
		#pragma fragment frag
		sampler2D _A;
		sampler2D _B;
		sampler2D _C;
		
		ENDCG

		Pass
		{
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 Weights = tex2D( _A, i.uv );
				float4 LayerBase = tex2D( _B, i.uv );
				float4 Layer1 = tex2D( _C, i.uv );
				return lerp ( LayerBase, Layer1, Weights.x );
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			sampler2D _D;
			float4 frag ( v2f_img i ) : SV_Target
			{
				float4 Weights = tex2D ( _A, i.uv );
				float4 LayerBase = tex2D ( _B, i.uv );
				float4 Layer1 = tex2D ( _C, i.uv );
				float4 Layer2 = tex2D ( _D, i.uv );
				return lerp ( lerp ( LayerBase, Layer1, Weights.x ), Layer2, Weights.y );
			}
			ENDCG
		}
		
		Pass
		{
			CGPROGRAM
			sampler2D _D;
			sampler2D _E;
			float4 frag ( v2f_img i ) : SV_Target
			{
				float4 Weights = tex2D ( _A, i.uv );
				float4 LayerBase = tex2D ( _B, i.uv );
				float4 Layer1 = tex2D ( _C, i.uv );
				float4 Layer2 = tex2D ( _D, i.uv );
				float4 Layer3 = tex2D ( _E, i.uv );
				return lerp ( lerp ( lerp ( LayerBase, Layer1, Weights.x ), Layer2, Weights.y ), Layer3, Weights.z );
			}
			ENDCG
		}
		
		Pass
		{
			CGPROGRAM
			sampler2D _D;
			sampler2D _E;
			sampler2D _F;
			float4 frag ( v2f_img i ) : SV_Target
			{
				float4 Weights = tex2D ( _A, i.uv );
				float4 LayerBase = tex2D ( _B, i.uv );
				float4 Layer1 = tex2D ( _C, i.uv );
				float4 Layer2 = tex2D ( _D, i.uv );
				float4 Layer3 = tex2D ( _E, i.uv );
				float4 Layer4 = tex2D ( _F, i.uv );
				return lerp ( lerp ( lerp ( lerp ( LayerBase, Layer1, Weights.x ), Layer2, Weights.y ), Layer3, Weights.z ), Layer4, Weights.w );
			}
			ENDCG
		}
	}
}
