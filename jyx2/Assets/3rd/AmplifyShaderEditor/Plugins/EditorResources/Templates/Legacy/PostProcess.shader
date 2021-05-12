// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader /*ase_name*/ "Hidden/Templates/Legacy/PostProcess" /*end*/
{
	Properties
	{
		_MainTex ( "Screen", 2D ) = "black" {}
		/*ase_props*/
	}

	SubShader
	{
		Tags{ }
		
		ZTest Always
		Cull Off
		ZWrite Off

		/*ase_pass*/
		Pass
		{ 
			CGPROGRAM 

			#pragma vertex vert_img_custom 
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			/*ase_pragma*/

			struct appdata_img_custom
			{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				/*ase_vdata:p=p;uv0=tc0*/
			};

			struct v2f_img_custom
			{
				float4 pos : SV_POSITION;
				half2 uv   : TEXCOORD0;
				half2 stereoUV : TEXCOORD2;
		#if UNITY_UV_STARTS_AT_TOP
				half4 uv2 : TEXCOORD1;
				half4 stereoUV2 : TEXCOORD3;
		#endif
				/*ase_interp(4,):sp=sp.xyzw;uv0=tc0.xy;uv1=tc1;uv2=tc2;uv3=tc3*/
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			/*ase_globals*/

			v2f_img_custom vert_img_custom ( appdata_img_custom v /*ase_vert_input*/ )
			{
				v2f_img_custom o;
				/*ase_vert_code:v=appdata_img_custom;o=v2f_img_custom*/
				o.pos = UnityObjectToClipPos ( v.vertex );
				o.uv = float4( v.texcoord.xy, 1, 1 );

				#if UNITY_UV_STARTS_AT_TOP
					o.uv2 = float4( v.texcoord.xy, 1, 1 );
					o.stereoUV2 = UnityStereoScreenSpaceUVAdjust ( o.uv2, _MainTex_ST );

					if ( _MainTex_TexelSize.y < 0.0 )
						o.uv.y = 1.0 - o.uv.y;
				#endif
				o.stereoUV = UnityStereoScreenSpaceUVAdjust ( o.uv, _MainTex_ST );
				return o;
			}

			half4 frag ( v2f_img_custom i /*ase_frag_input*/) : SV_Target
			{
				#ifdef UNITY_UV_STARTS_AT_TOP
					half2 uv = i.uv2;
					half2 stereoUV = i.stereoUV2;
				#else
					half2 uv = i.uv;
					half2 stereoUV = i.stereoUV;
				#endif	
				
				half4 finalColor;

				// ase common template code
				/*ase_frag_code:i=v2f_img_custom*/

				finalColor = /*ase_frag_out:Frag Color;Float4*/half4( 1, 1, 1, 1 )/*end*/;

				return finalColor;
			} 
			ENDCG 
		}
	}
	CustomEditor "ASEMaterialInspector"
}
