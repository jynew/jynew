Shader "Hidden/Preview_ConditionalIfNode"
{
	Properties
	{
		_A ( "_A", 2D) = "white" {}
		_B ( "_B", 2D ) = "white" {}
		_C ( "_AGreaterThanB", 2D ) = "white" {}
		_D ( "_AEqualToB", 2D ) = "white" {}
		_E ( "_ALessThanB", 2D ) = "white" {}
	}
	
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag
			
			uniform sampler2D _A;
			uniform sampler2D _B;
			uniform sampler2D _C;
			uniform sampler2D _D;
			uniform sampler2D _E;

			float4 frag ( v2f_img i ) : SV_Target
			{
				float aVal = tex2D ( _A, i.uv ).r;
				float bVal = tex2D ( _B, i.uv ).r;
				float4 aGreaterbVal = tex2D ( _C, i.uv );
				float4 aEqualbVal = tex2D ( _D, i.uv );
				float4 aLessbVal = tex2D ( _E, i.uv );
				
				if ( aVal > bVal )
					return aGreaterbVal;

				if ( aVal == bVal )
					return aEqualbVal;

				return aLessbVal;
				
			}
			ENDCG
		}
	}
}
