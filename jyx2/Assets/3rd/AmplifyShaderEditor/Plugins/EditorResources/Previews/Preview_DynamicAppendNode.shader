Shader "Hidden/DynamicAppendNode"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
		_B ("_B", 2D) = "white" {}
		_C ("_C", 2D) = "white" {}
		_D ("_D", 2D) = "white" {}
		_Mask("_Mask", Vector) = (0,0,0,0)
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
			sampler2D _D;

			float4 _Mask;
		ENDCG

		Pass //0
		{
			Name "1111"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				float4 b = tex2D(_B, i.uv);
				float4 c = tex2D(_C, i.uv);
				float4 d = tex2D(_D, i.uv);
				return float4(a.x,b.x,c.x,d.x)*_Mask;
			}
			ENDCG
		}

		Pass //1
		{
			Name "1120"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				float4 b = tex2D(_B, i.uv);
				float4 c = tex2D(_C, i.uv);
				
				return float4(a.x,b.x,c.xy)*_Mask;
			}
			ENDCG
		}

		Pass //2
		{
			Name "1201"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				float4 b = tex2D(_B, i.uv);
				float4 d = tex2D(_D, i.uv);
				return float4(a.x,b.xy,d.x)*_Mask;
			}
			ENDCG
		}

		Pass //3
		{
			Name "1300"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				float4 b = tex2D(_B, i.uv);
				return float4(a.x,b.xyz)*_Mask;
			}
			ENDCG
		}

		Pass //4
		{
			Name "2011"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				float4 c = tex2D(_C, i.uv);
				float4 d = tex2D(_D, i.uv);
				return float4(a.xy,c.x,d.x)*_Mask;
			}
			ENDCG
		}

		Pass //5
		{
			Name "2020"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				float4 c = tex2D(_C, i.uv);
				return float4(a.xy,c.xy)*_Mask;
			}
			ENDCG
		}

		Pass //6
		{
			Name "3001"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				float4 d = tex2D(_D, i.uv);
				return float4(a.xyz,d.x)*_Mask;
			}
			ENDCG
		}

		Pass //7
		{
			Name "4000"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return a*_Mask;
			}
			ENDCG
		}
	}
}
