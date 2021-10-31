// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hovl/Particles/BlendDistort"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Flow("Flow", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_NormalMap("NormalMap", 2D) = "bump" {}
		_Color("Color", Color) = (0.5,0.5,0.5,1)
		_Distortionpower("Distortion power", Float) = 0
		_SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_DistortionSpeedXYPowerZ("Distortion Speed XY Power Z", Vector) = (0,0,0,0)
		_Emission("Emission", Float) = 2
		_Opacity("Opacity", Range( 0 , 3)) = 1
		[Toggle]_Usedepth("Use depth?", Float) = 1
		[Toggle]_Softedges("Soft edges", Float) = 0
		_Depthpower("Depth power", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.5
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float4 uv_tex4coord;
			float4 vertexColor : COLOR;
			float3 worldNormal;
			float3 viewDir;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _Distortionpower;
		uniform sampler2D _NormalMap;
		uniform float4 _SpeedMainTexUVNoiseZW;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Flow;
		uniform float4 _DistortionSpeedXYPowerZ;
		uniform float4 _Flow_ST;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform sampler2D _Noise;
		uniform float4 _Noise_ST;
		uniform float4 _Color;
		uniform float _Emission;
		uniform float _Opacity;
		uniform float _Softedges;
		uniform float _Usedepth;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Depthpower;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 appendResult22 = (float2(_SpeedMainTexUVNoiseZW.z , _SpeedMainTexUVNoiseZW.w));
			float2 uv0_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float2 panner146 = ( 1.0 * _Time.y * appendResult22 + uv0_NormalMap);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor132 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( float4( UnpackScaleNormal( tex2D( _NormalMap, panner146 ), _Distortionpower ) , 0.0 ) + ase_grabScreenPosNorm ).xy);
			float3 temp_output_128_0 = (screenColor132).rgb;
			float2 appendResult21 = (float2(_SpeedMainTexUVNoiseZW.x , _SpeedMainTexUVNoiseZW.y));
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 panner107 = ( 1.0 * _Time.y * appendResult21 + uv0_MainTex);
			float2 appendResult100 = (float2(_DistortionSpeedXYPowerZ.x , _DistortionSpeedXYPowerZ.y));
			float4 uv0_Flow = i.uv_tex4coord;
			uv0_Flow.xy = i.uv_tex4coord.xy * _Flow_ST.xy + _Flow_ST.zw;
			float2 panner110 = ( 1.0 * _Time.y * appendResult100 + (uv0_Flow).xy);
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float Flowpower102 = _DistortionSpeedXYPowerZ.z;
			float4 tex2DNode13 = tex2D( _MainTex, ( float4( panner107, 0.0 , 0.0 ) - ( ( tex2D( _Flow, panner110 ) * tex2D( _Mask, uv_Mask ) ) * Flowpower102 ) ).rg );
			float2 uv0_Noise = i.uv_texcoord * _Noise_ST.xy + _Noise_ST.zw;
			float2 panner108 = ( 1.0 * _Time.y * appendResult22 + uv0_Noise);
			float2 appendResult160 = (float2(uv0_Flow.w , 0.0));
			float4 tex2DNode14 = tex2D( _Noise, ( panner108 + appendResult160 ) );
			float temp_output_88_0 = ( tex2DNode13.a * tex2DNode14.a * _Color.a * i.vertexColor.a * _Opacity );
			float3 temp_output_140_0 = (( ( tex2DNode13 * tex2DNode14 * _Color * i.vertexColor ) * _Emission * temp_output_88_0 )).rgb;
			float W158 = uv0_Flow.z;
			float3 lerpResult157 = lerp( ( temp_output_128_0 + temp_output_140_0 ) , ( temp_output_128_0 * temp_output_140_0 ) , W158);
			o.Emission = lerpResult157;
			float temp_output_151_0 = saturate( temp_output_88_0 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth134 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth134 = abs( ( screenDepth134 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depthpower ) );
			float3 ase_worldNormal = i.worldNormal;
			float dotResult163 = dot( ase_worldNormal , i.viewDir );
			float temp_output_185_0 = ( pow( dotResult163 , 3.0 ) * 5.0 );
			float dotResult171 = dot( ase_worldNormal , i.viewDir );
			float lerpResult181 = lerp( temp_output_185_0 , (0.0 + (temp_output_185_0 - 0.0) * (1.0 - 0.0) / (-1.0 - 0.0)) , (1.0 + (sign( dotResult171 ) - -1.0) * (0.0 - 1.0) / (1.0 - -1.0)));
			float clampResult186 = clamp( lerpResult181 , 0.0 , 1.0 );
			o.Alpha = lerp(lerp(temp_output_151_0,( temp_output_151_0 * saturate( distanceDepth134 ) ),_Usedepth),( lerp(temp_output_151_0,( temp_output_151_0 * saturate( distanceDepth134 ) ),_Usedepth) * clampResult186 ),_Softedges);
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=17000
847;143;1147;838;1246.973;-44.28143;2.743498;True;False
Node;AmplifyShaderEditor.CommentaryNode;104;-4130.993,490.5418;Float;False;1910.996;537.6462;Texture distortion;13;91;33;100;102;99;95;103;92;59;98;110;154;158;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;99;-3968.293,619.481;Float;False;Property;_DistortionSpeedXYPowerZ;Distortion Speed XY Power Z;8;0;Create;True;0;0;False;0;0,0,0,0;0,-0.3,-0.42,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;98;-3920.299,848.9976;Float;False;0;91;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;100;-3535.482,654.5021;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;59;-3583.603,566.496;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;109;-3401.27,-330.4436;Float;False;1037.896;533.6285;Textures movement;7;107;108;29;21;89;22;15;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;110;-3352.609,596.5295;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;15;-3351.27,-101.4007;Float;False;Property;_SpeedMainTexUVNoiseZW;Speed MainTex U/V + Noise Z/W;7;0;Create;True;0;0;False;0;0,0,0,0;0,0,-0.6,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;91;-3152.937,567.9764;Float;True;Property;_Flow;Flow;2;0;Create;True;0;0;False;0;None;3f1d4fadb37c37e4488860109d7dce4b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;154;-3241.786,814.8284;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-3146.373,763.0061;Float;True;Property;_Mask;Mask;3;0;Create;True;0;0;False;0;None;4b0225a5290cbe540bc56e26a8682db2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-3556.945,748.0421;Float;False;Flowpower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;89;-2861.858,-55.04038;Float;False;0;14;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;22;-2766.722,70.18491;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-2762.212,550.0183;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;21;-2778.501,-153.1786;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;-2856.788,-280.4436;Float;False;0;13;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;103;-2605.07,630.9626;Float;False;102;Flowpower;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;155;-3110.044,378.7794;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;107;-2570.374,-239.5098;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldNormalVector;167;-1534.985,863.8109;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;162;-1517.8,1016.647;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-2388.997,542.6455;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;160;-2463.807,241.8467;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;148;-2291.231,57.18929;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;108;-2577.237,-21.63752;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;163;-1289.017,881.0124;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;169;-1151.195,1305.252;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;170;-1155.524,1167.576;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;96;-1989.684,-41.77601;Float;False;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;152;-2247.959,174.557;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;147;-1304.489,-522.6365;Float;False;0;125;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;149;-2066.336,-256.5923;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;171;-899.1652,1172.812;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;164;-1061.509,881.1074;Float;True;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-826.0103,543.6755;Float;False;Property;_Opacity;Opacity;10;0;Create;True;0;0;False;0;1;3;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-1060.476,-272.4925;Float;False;Property;_Distortionpower;Distortion power;6;0;Create;True;0;0;False;0;0;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;146;-1041.952,-401.1633;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;135;-789.709,660.5037;Float;False;Property;_Depthpower;Depth power;13;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;32;-1670.612,486.0577;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;14;-1804.579,119.2214;Float;True;Property;_Noise;Noise;1;0;Create;True;0;0;False;0;None;7ae741e4c5226834db440bb3f58952b7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;31;-1728.612,316.0578;Float;False;Property;_Color;Color;5;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;13;-1803.192,-66.2159;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;60d224affeaecf84ead5a8b24c6c9995;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;125;-804.1512,-387.8453;Float;True;Property;_NormalMap;NormalMap;4;0;Create;True;0;0;False;0;None;9d5139686547cd24983e1c90ad7e4c33;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GrabScreenPosition;131;-740.1512,-179.8456;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;134;-571.1661,642.9752;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-445.0123,311.9933;Float;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;185;-766.309,876.9579;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SignOpNode;179;-755.3915,1168.656;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1135.791,-2.490838;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;151;-283.3048,312.2804;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;138;-307.0031,645.2684;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;178;-519.2654,939.6957;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;-1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-488.2215,149.8908;Float;False;Property;_Emission;Emission;9;0;Create;True;0;0;False;0;2;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;184;-569.9684,1152.042;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;126;-484.1472,-291.8456;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-285.8404,56.42584;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;-97.1164,420.5305;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;181;-268.7881,864.6698;Float;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;132;-338.2314,-366.4494;Float;False;Global;_GrabScreen0;Grab Screen 0;2;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;136;96.6973,310.8871;Float;False;Property;_Usedepth;Use depth?;11;0;Create;True;0;0;False;0;1;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;140;-127.7495,51.97886;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;128;-152.7932,-356.0084;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;186;-81.06185,864.351;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;158;-3580.125,842.1049;Float;False;W;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;133;267.4687,-177.1892;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;-0.1,-0.1,-0.1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;166;349.7334,390.7799;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;156;256.3983,18.15391;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;159;204.3189,152.461;Float;False;158;W;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;157;505.0637,-31.91299;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;165;508.7849,304.1036;Float;False;Property;_Softedges;Soft edges;12;0;Create;True;0;0;False;0;0;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;145;780.9699,35.32447;Float;False;True;3;Float;;0;0;Unlit;Hovl/Particles/BlendDistort;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;100;0;99;1
WireConnection;100;1;99;2
WireConnection;59;0;98;0
WireConnection;110;0;59;0
WireConnection;110;2;100;0
WireConnection;91;1;110;0
WireConnection;154;0;98;4
WireConnection;102;0;99;3
WireConnection;22;0;15;3
WireConnection;22;1;15;4
WireConnection;92;0;91;0
WireConnection;92;1;33;0
WireConnection;21;0;15;1
WireConnection;21;1;15;2
WireConnection;155;0;154;0
WireConnection;107;0;29;0
WireConnection;107;2;21;0
WireConnection;95;0;92;0
WireConnection;95;1;103;0
WireConnection;160;0;155;0
WireConnection;148;0;22;0
WireConnection;108;0;89;0
WireConnection;108;2;22;0
WireConnection;163;0;167;0
WireConnection;163;1;162;0
WireConnection;96;0;107;0
WireConnection;96;1;95;0
WireConnection;152;0;108;0
WireConnection;152;1;160;0
WireConnection;149;0;148;0
WireConnection;171;0;170;0
WireConnection;171;1;169;0
WireConnection;164;0;163;0
WireConnection;146;0;147;0
WireConnection;146;2;149;0
WireConnection;14;1;152;0
WireConnection;13;1;96;0
WireConnection;125;1;146;0
WireConnection;125;5;124;0
WireConnection;134;0;135;0
WireConnection;88;0;13;4
WireConnection;88;1;14;4
WireConnection;88;2;31;4
WireConnection;88;3;32;4
WireConnection;88;4;62;0
WireConnection;185;0;164;0
WireConnection;179;0;171;0
WireConnection;30;0;13;0
WireConnection;30;1;14;0
WireConnection;30;2;31;0
WireConnection;30;3;32;0
WireConnection;151;0;88;0
WireConnection;138;0;134;0
WireConnection;178;0;185;0
WireConnection;184;0;179;0
WireConnection;126;0;125;0
WireConnection;126;1;131;0
WireConnection;51;0;30;0
WireConnection;51;1;52;0
WireConnection;51;2;88;0
WireConnection;139;0;151;0
WireConnection;139;1;138;0
WireConnection;181;0;185;0
WireConnection;181;1;178;0
WireConnection;181;2;184;0
WireConnection;132;0;126;0
WireConnection;136;0;151;0
WireConnection;136;1;139;0
WireConnection;140;0;51;0
WireConnection;128;0;132;0
WireConnection;186;0;181;0
WireConnection;158;0;98;3
WireConnection;133;0;128;0
WireConnection;133;1;140;0
WireConnection;166;0;136;0
WireConnection;166;1;186;0
WireConnection;156;0;128;0
WireConnection;156;1;140;0
WireConnection;157;0;133;0
WireConnection;157;1;156;0
WireConnection;157;2;159;0
WireConnection;165;0;136;0
WireConnection;165;1;166;0
WireConnection;145;2;157;0
WireConnection;145;9;165;0
ASEEND*/
//CHKSM=8ABCA0CC8DDD786763F299020EF363A4DA03F01D