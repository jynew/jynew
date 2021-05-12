Shader "Hidden/VertexIdVariableNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			struct appdata_custom
			{
				float4 vertex : POSITION;
				uint vertexId : SV_VertexID;
			};

			struct v2f_custom
			{
				float4 pos : SV_POSITION;
				half vertexId : TEXCOORD0;
			};

			v2f_custom vert( appdata_custom v )
			{
				v2f_custom o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.vertexId = v.vertexId;
				return o;
			}

			float4 frag( v2f_custom i ) : SV_Target
			{
				return i.vertexId;
			}
			ENDCG
		}
	}
}
