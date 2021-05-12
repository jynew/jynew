Shader "Hidden/SwizzleNode"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
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
		///////////////////////////////////////////////////////////////////////////////
		Pass 
		{
			Name "xxxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xxxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xxxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xxxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xxyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.y,a.x)*_Mask;
			}
			ENDCG
		}


		Pass 
		{
			Name "xxyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xxyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xxyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xxzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xxzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xxzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xxzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xxwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xxwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xxwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xxww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.x,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xyxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xyxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xyxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xyxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xyyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.y,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xyyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xyyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xyyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xyzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xyzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xyzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xyzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xywx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xywy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xywz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xyww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.y,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xzxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.y,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xzyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xzzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xzwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xzww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.z,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xwxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.y,a.x)*_Mask;
			}
			ENDCG
		}


		Pass 
		{
			Name "xwyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xwzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "xwwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "xwww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.x,a.w,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		///////////////////////////////////////////////////////////////////////////////
		Pass 
		{
			Name "yxxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "yxxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yxxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yxxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yxyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.y,a.x)*_Mask;
			}
			ENDCG
		}


		Pass 
		{
			Name "yxyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yxyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yxyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yxzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "yxzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yxzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yxzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yxwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "yxwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yxwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yxww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.x,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yyxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "yyxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yyxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yyxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yyyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.y,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "yyyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yyyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yyyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yyzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "yyzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yyzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yyzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yywx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "yywy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yywz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yyww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.y,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "yzxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.y,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "yzyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "yzzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "yzwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "yzww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.z,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "ywxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.y,a.x)*_Mask;
			}
			ENDCG
		}


		Pass 
		{
			Name "ywyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "ywzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "ywwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "ywww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.y,a.w,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		///////////////////////////////////////////////////////////////////////////////
		Pass 
		{
			Name "zxxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zxxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zxxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zxxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zxyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.y,a.x)*_Mask;
			}
			ENDCG
		}


		Pass 
		{
			Name "zxyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zxyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zxyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zxzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zxzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zxzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zxzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zxwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zxwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zxwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zxww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.x,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zyxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zyxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zyxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zyxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zyyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.y,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zyyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zyyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zyyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zyzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zyzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zyzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zyzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zywx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zywy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zywz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zyww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.y,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zzxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.y,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zzyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zzzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zzwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zzww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.z,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zwxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.y,a.x)*_Mask;
			}
			ENDCG
		}


		Pass 
		{
			Name "zwyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zwzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "zwwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "zwww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.z,a.w,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		///////////////////////////////////////////////////////////////////////////////
		Pass 
		{
			Name "wxxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wxxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wxxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wxxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wxyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.y,a.x)*_Mask;
			}
			ENDCG
		}


		Pass 
		{
			Name "wxyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wxyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wxyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wxzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wxzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wxzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wxzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wxwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wxwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wxwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wxww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.x,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wyxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wyxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wyxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wyxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wyyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.y,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wyyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wyyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wyyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wyzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wyzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wyzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wyzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wywx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wywy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wywz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wyww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.y,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wzxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.y,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wzyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wzzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wzwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wzww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.z,a.w,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwxx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.x,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wwxy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.x,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwxz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.x,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwxw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.x,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwyx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.y,a.x)*_Mask;
			}
			ENDCG
		}


		Pass 
		{
			Name "wwyy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.y,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwyz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.y,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwyw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.y,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwzx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.z,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wwzy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.z,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwzz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.z,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwzw"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.z,a.w)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwwx"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.w,a.x)*_Mask;
			}
			ENDCG
		}

		Pass 
		{
			Name "wwwy"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.w,a.y)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwwz"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.w,a.z)*_Mask;
			}
			ENDCG
		}
		
		Pass 
		{
			Name "wwww"
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				return float4(a.w,a.w,a.w,a.w)*_Mask;
			}
			ENDCG
		}
	}
}
