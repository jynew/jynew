Shader "Hidden/TFHCFlipBookUVAnimation"
{
	Properties
	{
		_A ("_UV", 2D) = "white" {}
		_B ("_Columns", 2D) = "white" {}
		_C ("_Rows", 2D) = "white" {}
		_D ("_Speed", 2D) = "white" {}
		_E ("_StartFrame", 2D) = "white" {}
		_F ("_Speed", 2D) = "white" {}
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
			sampler2D _E;
			sampler2D _F;
			float _EditorTime;
		ENDCG

		//Time port disconnected
		Pass
		{
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float2 uv = tex2D( _A, i.uv ).rg;
				float col = tex2D( _B, i.uv ).r;
				float row = tex2D( _C, i.uv ).r;
				float spd = tex2D( _D, i.uv ).r;
				float str = tex2D( _E, i.uv ).r;
		
				float fbtotaltiles = col * row;
				float fbcolsoffset = 1.0f / col;
				float fbrowsoffset = 1.0f / row;
				float fbspeed = _EditorTime * spd;
				float2 fbtiling = float2(fbcolsoffset, fbrowsoffset);
				float fbcurrenttileindex = round( fmod( fbspeed + str, fbtotaltiles) );
				fbcurrenttileindex += ( fbcurrenttileindex < 0) ? fbtotaltiles : 0;
				float fblinearindextox = round ( fmod ( fbcurrenttileindex, col ) );
				float fboffsetx = fblinearindextox * fbcolsoffset;
				float fblinearindextoy = round( fmod( ( fbcurrenttileindex - fblinearindextox ) / col, row ) );
				fblinearindextoy = (int)(row-1) - fblinearindextoy;
				float fboffsety = fblinearindextoy * fbrowsoffset;
				float2 fboffset = float2(fboffsetx, fboffsety);
				float2 fbuv = float4( uv, 0.0 , 0.0 ) * fbtiling + fboffset;
				return float4(fbuv, 0 , 0);
			}
			ENDCG
		}
		
		//Time port connected
		Pass
		{
			CGPROGRAM
			float4 frag(v2f_img i) : SV_Target
			{
				float2 uv = tex2D( _A, i.uv ).rg;
				float col = tex2D( _B, i.uv ).r;
				float row = tex2D( _C, i.uv ).r;
				float spd = tex2D( _D, i.uv ).r;
				float str = tex2D( _E, i.uv ).r;
				float time = tex2D( _F, i.uv ).r;
				float fbtotaltiles = col * row;
				float fbcolsoffset = 1.0f / col;
				float fbrowsoffset = 1.0f / row;
				float fbspeed = time * spd;
				float2 fbtiling = float2(fbcolsoffset, fbrowsoffset);
				float fbcurrenttileindex = round( fmod( fbspeed + str, fbtotaltiles) );
				fbcurrenttileindex += ( fbcurrenttileindex < 0) ? fbtotaltiles : 0;
				float fblinearindextox = round ( fmod ( fbcurrenttileindex, col ) );
				float fboffsetx = fblinearindextox * fbcolsoffset;
				float fblinearindextoy = round( fmod( ( fbcurrenttileindex - fblinearindextox ) / col, row ) );
				fblinearindextoy = (int)(row-1) - fblinearindextoy;
				float fboffsety = fblinearindextoy * fbrowsoffset;
				float2 fboffset = float2(fboffsetx, fboffsety);
				float2 fbuv = float4( uv, 0.0 , 0.0 ) * fbtiling + fboffset;
				return float4(fbuv, 0 , 0);
			}
			ENDCG
		}
	}
}
