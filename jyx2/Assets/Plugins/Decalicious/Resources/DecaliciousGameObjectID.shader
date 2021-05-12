Shader "Hidden/Decalicious Game Object ID"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		ZWrite Off
		ZTest LEqual
		Offset -1,-1

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
	
			#include "UnityCG.cginc"

			int _ID;

			float4 vert(float4 vertex : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}

			float4 frag(float4 vertex : SV_POSITION) : SV_Target
			{
				// sample the texture
				return _ID;
			}
			ENDCG
		}
	}
}
