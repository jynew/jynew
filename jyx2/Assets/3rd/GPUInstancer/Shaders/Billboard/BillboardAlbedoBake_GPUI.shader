Shader "Hidden/GPUInstancer/Billboard/AlbedoBake"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Cutoff("Cutoff" , Range(0,1)) = 0.3
		_GPUIBillboardBrightness("Brightness", Range(0, 1)) = 0.5
		_GPUIBillboardCutoffOverride("Cutoff Override", Range(0, 1)) = 0.0
		_IsLinearSpace("Add Gama Correction", Float) = 0.0
	}
	SubShader
	{
		Cull Off
        
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "../Include/GPUIBillboardInclude.cginc"

			sampler2D _MainTex;
			float _Cutoff;
			float4 _Color;
			float _GPUIBillboardBrightness;
			float _GPUIBillboardCutoffOverride;
			float _IsLinearSpace;
						
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (float4 vertex : POSITION, float2 uv : TEXCOORD0)
			{


				v2f o;
				o.vertex = UnityObjectToClipPos(vertex);
				o.uv = uv.xy;
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				if (_GPUIBillboardCutoffOverride > 0.0){
					_Cutoff = _GPUIBillboardCutoffOverride;
				}

				half4 c = tex2D(_MainTex, i.uv);
				clip(c.a-_Cutoff); // discard if below cutoff
				if (_Color.r > 0 || _Color.g > 0 || _Color.b > 0){
					c.rgb = saturate(c * _Color).rgb;
				}

				c.a = 1; // set the non-discarded pixels back to full alpha


				// Adjust brightness
				float brightness = _GPUIBillboardBrightness * 2 - 1;

				//float channelValue = max(0, sign(brightness));
				//c.rgb = lerp (c.rgb, channelValue, abs(brightness));
				
				c.rgb += c.rgb * brightness; // more like contrast.
				
				// Account fot gamma correction if necessary
				return c * (1 - _IsLinearSpace) + (_IsLinearSpace * half4(LinearToGamma(c.rgb), 1));
			}
			ENDCG
		}
	}
}
