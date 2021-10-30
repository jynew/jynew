Shader "Hovl/Particles/Blend_TwoSides"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("Main Tex", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_FrontFacesColor("Front Faces Color", Color) = (0,0.2313726,1,1)
		_BackFacesColor("Back Faces Color", Color) = (0.1098039,0.4235294,1,1)
		_Emission("Emission", Float) = 2
		[Toggle]_UseFresnel("Use Fresnel?", Float) = 1
		[Toggle]_SeparateFresnel("SeparateFresnel", Float) = 0
		_SeparateEmission("Separate Emission", Float) = 2
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		_Fresnel("Fresnel", Float) = 1
		_FresnelEmission("Fresnel Emission", Float) = 1
		[Toggle]_UseCustomData("Use Custom Data?", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  "PreviewType"="Plane" }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha	
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		//#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float3 viewDir;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 uv_tex4coord;
		};

		uniform float _SeparateFresnel;
		uniform float _UseFresnel;
		uniform float4 _FrontFacesColor;
		uniform float _Fresnel;
		uniform float _FresnelEmission;
		uniform float4 _FresnelColor;
		uniform float4 _BackFacesColor;
		uniform float _Emission;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _SpeedMainTexUVNoiseZW;
		uniform float _SeparateEmission;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform sampler2D _Noise;
		uniform float4 _Noise_ST;
		uniform float _UseCustomData;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV95 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode95 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV95, _Fresnel ) );
			float dotResult87 = dot( ase_worldNormal , i.viewDir );
			float4 lerpResult91 = lerp( lerp(_FrontFacesColor,( ( _FrontFacesColor * ( 1.0 - fresnelNode95 ) ) + ( _FresnelEmission * _FresnelColor * fresnelNode95 ) ),_UseFresnel) , _BackFacesColor , (1.0 + (sign( dotResult87 ) - -1.0) * (0.0 - 1.0) / (1.0 - -1.0)));
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 appendResult21 = (float2(_SpeedMainTexUVNoiseZW.x , _SpeedMainTexUVNoiseZW.y));
			float4 tex2DNode105 = tex2D( _MainTex, ( uv0_MainTex + ( appendResult21 * _Time.y ) ) );
			o.Emission = lerp(( lerpResult91 * _Emission * i.vertexColor * i.vertexColor.a * tex2DNode105 ),( ( lerpResult91 + ( _FresnelColor * tex2DNode105 * _SeparateEmission ) ) * _Emission * i.vertexColor * i.vertexColor.a ),_SeparateFresnel).rgb;
			o.Alpha = 1;
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 uv0_Noise = i.uv_tex4coord;
			uv0_Noise.xy = i.uv_tex4coord.xy * _Noise_ST.xy + _Noise_ST.zw;
			float2 appendResult22 = (float2(_SpeedMainTexUVNoiseZW.z , _SpeedMainTexUVNoiseZW.w));
			clip( ( tex2D( _Mask, uv_Mask ) * tex2D( _Noise, ( (uv0_Noise).xy + ( _Time.y * appendResult22 ) + uv0_Noise.w ) ) * lerp(1.0,uv0_Noise.z,_UseCustomData) ).r - _Cutoff );
		}
		ENDCG
	}
}
/*ASEBEGIN
Version=17000
754;92;807;655;1574.913;1223.073;3.37946;True;False
Node;AmplifyShaderEditor.RangedFloatNode;96;-1332.484,-144.2664;Float;False;Property;_Fresnel;Fresnel;12;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;95;-1174.625,-220.7157;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;15;-1416.635,615.4911;Float;False;Property;_SpeedMainTexUVNoiseZW;Speed MainTex U/V + Noise Z/W;4;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;86;-880.5408,-104.7736;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;85;-855.3208,63.65365;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TimeNode;17;-1085.113,644.0539;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;21;-1055.23,553.4234;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;126;-879.6107,-189.0835;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;31;-899.9604,-500.3966;Float;False;Property;_FrontFacesColor;Front Faces Color;5;0;Create;True;0;0;False;0;0,0.2313726,1,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;93;-1145.177,-388.2928;Float;False;Property;_FresnelColor;Fresnel Color;11;0;Create;True;0;0;False;0;1,1,1,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;98;-1150.683,-471.3495;Float;False;Property;_FresnelEmission;Fresnel Emission;13;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;106;-612.1661,450.9655;Float;False;0;105;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-871.0923,-334.3178;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-838.5569,554.7645;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;87;-622.2883,-9.652705;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;-649.0439,-325.096;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SignOpNode;88;-454.7349,1.959959;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-312.7807,532.4018;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;123;-490.7369,-183.1635;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;143;-865.423,223.1553;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;-842.2249,645.2516;Float;False;0;14;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;22;-1052.689,775.7031;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;105;-176.5121,163.5146;Float;True;Property;_MainTex;Main Tex;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;133;-324.6653,-335.8928;Float;False;Property;_UseFresnel;Use Fresnel?;8;0;Create;True;0;0;False;0;1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;89;-292.2313,-16.20071;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;92;-327.1501,-194.7354;Float;False;Property;_BackFacesColor;Back Faces Color;6;0;Create;True;0;0;False;0;0.1098039,0.4235294,1,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;142;-94.93386,351.3196;Float;False;Property;_SeparateEmission;Separate Emission;10;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;144;-255.5379,138.241;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;91;-25.98829,-276.804;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-836.114,821.5425;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;59;-589.8,683.1187;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;252.8173,97.90491;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;136;411.426,-123.3316;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-16.89706,-122.9107;Float;False;Property;_Emission;Emission;7;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-265.9941,869.9862;Float;False;Constant;_Float0;Float 0;13;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;32;-46.08871,-45.01809;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-264.7057,745.7369;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;13;-117.0276,524.4487;Float;True;Property;_Mask;Mask;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;130;-98.24423,989.1903;Float;False;Property;_UseCustomData;Use Custom Data?;14;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-117.4146,709.8865;Float;True;Property;_Noise;Noise;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;274.6301,239.1062;Float;False;5;5;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;566.1781,-39.63671;Float;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;333.5393,643.4429;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;134;765.065,222.8378;Float;False;Property;_SeparateFresnel;SeparateFresnel;9;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;129;1092.768,257.209;Float;False;True;2;Float;;0;0;Unlit;Hovl/Particles/Blend_TwoSides;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;TransparentCutout;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;1;PreviewType=Plane;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;95;3;96;0
WireConnection;21;0;15;1
WireConnection;21;1;15;2
WireConnection;126;0;95;0
WireConnection;97;0;98;0
WireConnection;97;1;93;0
WireConnection;97;2;95;0
WireConnection;24;0;21;0
WireConnection;24;1;17;2
WireConnection;87;0;86;0
WireConnection;87;1;85;0
WireConnection;127;0;31;0
WireConnection;127;1;126;0
WireConnection;88;0;87;0
WireConnection;26;0;106;0
WireConnection;26;1;24;0
WireConnection;123;0;127;0
WireConnection;123;1;97;0
WireConnection;143;0;93;0
WireConnection;22;0;15;3
WireConnection;22;1;15;4
WireConnection;105;1;26;0
WireConnection;133;0;31;0
WireConnection;133;1;123;0
WireConnection;89;0;88;0
WireConnection;144;0;143;0
WireConnection;91;0;133;0
WireConnection;91;1;92;0
WireConnection;91;2;89;0
WireConnection;23;0;17;2
WireConnection;23;1;22;0
WireConnection;59;0;29;0
WireConnection;141;0;144;0
WireConnection;141;1;105;0
WireConnection;141;2;142;0
WireConnection;136;0;91;0
WireConnection;136;1;141;0
WireConnection;27;0;59;0
WireConnection;27;1;23;0
WireConnection;27;2;29;4
WireConnection;130;0;104;0
WireConnection;130;1;29;3
WireConnection;14;1;27;0
WireConnection;51;0;91;0
WireConnection;51;1;52;0
WireConnection;51;2;32;0
WireConnection;51;3;32;4
WireConnection;51;4;105;0
WireConnection;140;0;136;0
WireConnection;140;1;52;0
WireConnection;140;2;32;0
WireConnection;140;3;32;4
WireConnection;70;0;13;0
WireConnection;70;1;14;0
WireConnection;70;2;130;0
WireConnection;134;0;51;0
WireConnection;134;1;140;0
WireConnection;129;2;134;0
WireConnection;129;10;70;0
ASEEND*/
//CHKSM=91A18620E85D389A5025600A91D9217F3130E590