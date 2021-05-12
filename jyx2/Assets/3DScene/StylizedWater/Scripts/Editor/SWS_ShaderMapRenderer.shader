// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

Shader "Hidden/SWS/ShaderMapRenderer"
{
	Properties
	{
		_RedInput("Red Channel", 2D) = "black" {}
		_GreenInput("Green Channel", 2D) = "black" {}
		_BlueInput("Blue Channel", 2D) = "black" {}
		//_AlphaInput ("Alpha Channel", 2D) = "black" {}
	}
	SubShader
	{

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _RedInput;
			sampler2D _GreenInput;
			sampler2D _BlueInput;
			sampler2D _AlphaInput;
			
			fixed4 frag (v2f_img IN) : SV_Target
			{
				float r = tex2D(_RedInput, IN.uv).rrrr;
				float g = tex2D(_GreenInput, IN.uv).rrrr;
				float b = tex2D(_BlueInput, IN.uv).rrrr;
				//float a = tex2D(_AlphaInput, IN.uv).rrrr;

				return float4(r, g, b, 1);
			}
			ENDCG
		}
	}
}