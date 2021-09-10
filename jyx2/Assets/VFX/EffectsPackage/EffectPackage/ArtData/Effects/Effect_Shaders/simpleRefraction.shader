// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ztgame/Refraction/simple_Refraction"
{
	Properties
	{
	 	//_Intensity ("Refraction Intensity", Range(0, 0.5)) = 0.5299146
		_Refraction ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100
		GrabPass{ }

		Pass
		{	
			Name "FORWARD"

			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
            ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				fixed4 vertex : POSITION;
				fixed2 uv : TEXCOORD0; 
				float4 vertexColor : COLOR;
			};

			struct v2f
			{
				fixed2 uv : TEXCOORD0;
				fixed4 pos : SV_POSITION;
				fixed4 screenPos : TEXCOORD1;
				float4 vertexColor : COLOR;
			};

			sampler2D _GrabTexture;
			sampler2D _Refraction; float4 _Refraction_ST;
//			fixed _Intensity;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex );
				o.uv = TRANSFORM_TEX(v.uv, _Refraction);
			  	o.screenPos = o.pos;
				o.vertexColor = v.vertexColor;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//DX和OpenGl的差异宏
				#if UNITY_UV_STARTS_AT_TOP
                    fixed grabSign = -_ProjectionParams.x;
                #else
                    fixed grabSign = _ProjectionParams.x;
				#endif
                i.screenPos = fixed4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y*=_ProjectionParams.x;

                fixed4 _RefCol=tex2D(_Refraction,i.uv);
          
                fixed2 sceneUVs=fixed2(1,grabSign)*i.screenPos.xy*0.5 +0.5+_RefCol.rg*i.vertexColor.a;

				fixed4 col = tex2D(_GrabTexture, sceneUVs);
				return col;
			}
			ENDCG
		}
	}
}
