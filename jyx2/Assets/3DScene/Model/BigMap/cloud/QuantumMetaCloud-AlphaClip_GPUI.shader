Shader "GPUInstancer/QuantumTheory/QuantumMetaCloud-AlphaClip"
{
	Properties 
	{
_CloudTexture("Quantum MetaCloud Texture", 2D) = "black" {}
_AmbientColor("Ambient Color of Clouds", Color) = (0.2078431,0.2588235,0.2980392,1)
_SunColor("Sun Color of Clouds", Color) = (0.977612,0.9254484,0.882769,1)
_CloudContrast("Cloud Contrast", Range(0.1,3) ) = 1
_Bias("Cloud Density", Range(0.25,0.95) ) = 0.45
_AlphaCutoff("Alpha Clip Value", Range(1,0.1) ) = 0.1

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="True"
"RenderType"="TransparentCutout"

		}

		
Cull Off
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
Mode Off
}


		CGPROGRAM
#include "UnityCG.cginc"
#include "./../../../../3rd/GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc"
#pragma instancing_options procedural:setupGPUI
#pragma multi_compile_instancing
#pragma surface surf BlinnPhongEditor  softvegetation noambient nolightmap vertex:vert
#pragma target 2.0


sampler2D _CloudTexture;
float4 _AmbientColor;
float4 _SunColor;
float _CloudContrast;
float _Bias;
float _AlphaCutoff;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float2 uv_CloudTexture;

			};

			void vert (inout appdata_full v, out Input o) {
			 UNITY_INITIALIZE_OUTPUT(Input,o);
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Tex2D0=tex2D(_CloudTexture,(IN.uv_CloudTexture.xyxy).xy);
float4 Split0=Tex2D0;
float4 Subtract0=float4( Split0.x, Split0.x, Split0.x, Split0.x) - float4( 0.5,0.5,0.5,0.5 );
float4 Max0=max(_CloudContrast.xxxx,float4( 0,0,0,0 ));
float4 Multiply1=Subtract0 * Max0;
float4 Add1=Multiply1 + float4( 0.5,0.5,0.5,0.5 );
float4 Saturate0=saturate(Add1);
float4 Log1=log(float4( 0.5,0.5,0.5,0.5 ));
float4 Log0=log(_Bias.xxxx);
float4 Divide0=Log1 / Log0;
float4 Pow0=pow(Saturate0,Divide0);
float4 Saturate1=saturate(Pow0);
float4 Lerp0=lerp(_AmbientColor,_SunColor,Saturate1);
float4 Subtract1=float4( Split0.y, Split0.y, Split0.y, Split0.y) - _AlphaCutoff.xxxx;
float4 Master0_0_NoInput = float4(0,0,0,0);
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
clip( Subtract1 );
o.Emission = Lerp0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}
