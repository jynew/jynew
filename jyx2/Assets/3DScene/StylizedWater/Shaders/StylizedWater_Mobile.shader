// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "StylizedWater/Mobile"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_WaterColor("Water Color", Color) = (0.1176471,0.6348885,1,0)
		_WaterShallowColor("WaterShallowColor", Color) = (0.4191176,0.7596349,1,0)
		_Wavetint("Wave tint", Range( -1 , 1)) = 0
		_RimColor("Rim Color", Color) = (1,1,1,0.5019608)
		_NormalStrength("NormalStrength", Range( 0 , 1)) = 0.25
		_Transparency("Transparency", Range( 0 , 1)) = 0.75
		_Glossiness("Glossiness", Range( 0 , 1)) = 0.85
		_Worldspacetiling("Worldspace tiling", Float) = 1
		_NormalTiling("NormalTiling", Range( 0 , 1)) = 0.9
		_EdgeFade("EdgeFade", Range( 0.01 , 3)) = 0.2448298
		_RimSize("Rim Size", Range( 0 , 20)) = 5
		_Rimfalloff("Rim falloff", Range( 0.1 , 50)) = 3
		_Rimtiling("Rim tiling", Float) = 0.5
		_FoamOpacity("FoamOpacity", Range( -1 , 1)) = 0.05
		_FoamSpeed("FoamSpeed", Range( 0 , 1)) = 0.1
		_FoamSize("FoamSize", Float) = 0
		_FoamTiling("FoamTiling", Float) = 0.05
		_Depth("Depth", Range( 0 , 100)) = 30
		_Wavesspeed("Waves speed", Range( 0 , 10)) = 0.75
		_WaveHeight("Wave Height", Range( 0 , 1)) = 0.05
		_WaveFoam("Wave Foam", Range( 0 , 10)) = 0
		_WaveSize("Wave Size", Range( 0 , 10)) = 0.1
		_WaveDirection("WaveDirection", Vector) = (1,0,0,0)
		[Toggle]_USE_VC_INTERSECTION("USE_VC_INTERSECTION", Float) = 0
		[Toggle]_UseIntersectionFoam("UseIntersectionFoam", Int) = 0
		[Toggle]_ENABLE_VC("ENABLE_VC", Float) = 0
		[Toggle]_LIGHTING("LIGHTING", Int) = 0
		[NoScaleOffset][Normal]_Normals("Normals", 2D) = "bump" {}
		[Toggle]_Unlit("Unlit", Float) = 0
		[NoScaleOffset]_Shadermap("Shadermap", 2D) = "black" {}
		[Toggle]_NORMAL_MAP("NORMAL_MAP", Int) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		LOD 200
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature _USEINTERSECTIONFOAM_ON
		#pragma multi_compile __ _NORMAL_MAP_ON
		#pragma multi_compile __ _LIGHTING_ON
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma exclude_renderers xbox360 psp2 n3ds wiiu 
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha noshadow nolightmap  nodynlightmap nodirlightmap nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
			float2 data713;
			float2 texcoord_0;
			float2 data714;
			float4 vertexColor : COLOR;
			float3 worldPos;
			float3 worldRefl;
			float3 data746;
			float2 texcoord_1;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform half4 _WaterShallowColor;
		uniform half4 _WaterColor;
		uniform sampler2D_float _CameraDepthTexture;
		uniform float _Depth;
		uniform sampler2D _Shadermap;
		uniform float _Worldspacetiling;
		uniform float _WaveSize;
		uniform float _Wavesspeed;
		uniform float4 _WaveDirection;
		uniform fixed _Wavetint;
		uniform float4 _RimColor;
		uniform float _USE_VC_INTERSECTION;
		uniform float _ENABLE_VC;
		uniform half _Rimfalloff;
		uniform float _Rimtiling;
		uniform half _RimSize;
		uniform fixed _FoamOpacity;
		uniform float _FoamTiling;
		uniform float _FoamSpeed;
		uniform half _FoamSize;
		uniform float _WaveFoam;
		uniform sampler2D _Normals;
		uniform float _NormalTiling;
		uniform fixed _NormalStrength;
		uniform fixed _Glossiness;
		uniform float _Unlit;
		uniform fixed _Transparency;
		uniform half _EdgeFade;
		uniform fixed _WaveHeight;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			o.data713 = lerp(( -20.0 * o.texcoord_0 ),( (ase_worldPos).xz * float2( 0.1,0.1 ) ),_Worldspacetiling);
			float2 appendResult500 = (float2(_WaveDirection.x , _WaveDirection.z));
			o.data714 = ( ( _Wavesspeed * _Time.x ) * appendResult500 );
			o.data746 = _LightColor0.rgb;
			float3 ase_vertexNormal = v.normal.xyz;
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float2 Tiling21 = lerp(( -20.0 * o.texcoord_1 ),( (ase_worldPos).xz * float2( 0.1,0.1 ) ),_Worldspacetiling);
			float2 WaveSpeed40 = ( ( _Wavesspeed * _Time.x ) * appendResult500 );
			float2 HeightmapUV581 = ( ( ( Tiling21 * _WaveSize ) * float2( 0.1,0.1 ) ) + ( WaveSpeed40 * float2( 0.5,0.5 ) ) );
			float4 tex2DNode94 = tex2Dlod( _Shadermap, float4( HeightmapUV581, 0.0 , 0.0 ) );
			float temp_output_95_0 = ( _WaveHeight * tex2DNode94.g );
			float3 Displacement100 = ( ase_vertexNormal * temp_output_95_0 );
			v.vertex.xyz += Displacement100;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			SurfaceOutputStandard s733 = (SurfaceOutputStandard ) 0;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			//Stylized Water custom depth
			float screenDepth643 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth643 = abs( ( screenDepth643 - LinearEyeDepth( ase_screenPosNorm.z ) ) / (  lerp( 1.0 , ( 1.0 / _ProjectionParams.z ) , unity_OrthoParams.w) ) );
			float DepthTexture494 = distanceDepth643;
			float Depth479 = saturate( ( DepthTexture494 / _Depth ) );
			float3 lerpResult478 = lerp( (_WaterShallowColor).rgb , (_WaterColor).rgb , Depth479);
			float3 WaterColor350 = lerpResult478;
			float2 Tiling21 = i.data713;
			float2 WaveSpeed40 = i.data714;
			float2 HeightmapUV581 = ( ( ( Tiling21 * _WaveSize ) * float2( 0.1,0.1 ) ) + ( WaveSpeed40 * float2( 0.5,0.5 ) ) );
			float4 tex2DNode94 = tex2D( _Shadermap, HeightmapUV581 );
			float Heightmap99 = tex2DNode94.g;
			float3 temp_cast_0 = (( Heightmap99 * _Wavetint )).xxx;
			float3 RimColor102 = (_RimColor).rgb;
			float4 VertexColors729 = lerp(float4( 0.0,0,0,0 ),i.vertexColor,_ENABLE_VC);
			float2 temp_output_24_0 = ( Tiling21 * _Rimtiling );
			float temp_output_30_0 = ( tex2D( _Shadermap, ( ( 0.5 * temp_output_24_0 ) + WaveSpeed40 ) ).b * tex2D( _Shadermap, ( temp_output_24_0 + ( 1.0 - WaveSpeed40 ) ) ).b );
			float Intersection42 = saturate( ( _RimColor.a * ( 1.0 - ( ( ( lerp(DepthTexture494,( 1.0 - (VertexColors729).r ),_USE_VC_INTERSECTION) / _Rimfalloff ) * temp_output_30_0 ) + ( lerp(DepthTexture494,( 1.0 - (VertexColors729).r ),_USE_VC_INTERSECTION) / _RimSize ) ) ) ) );
			float3 lerpResult61 = lerp( ( WaterColor350 - temp_cast_0 ) , ( RimColor102 * 3.0 ) , Intersection42);
			float2 temp_output_634_0 = ( WaveSpeed40 * _FoamSpeed );
			float4 tex2DNode67 = tex2D( _Shadermap, ( ( _FoamTiling * Tiling21 ) + temp_output_634_0 + ( Heightmap99 * 0.1 ) ) );
			#ifdef _USEINTERSECTIONFOAM_ON
				float staticSwitch725 = ( 1.0 - tex2DNode67.b );
			#else
				float staticSwitch725 = saturate( ( 1000.0 * ( ( tex2D( _Shadermap, ( ( _FoamTiling * ( Tiling21 * float2( 0.5,0.5 ) ) ) + temp_output_634_0 ) ).r - tex2DNode67.r ) - _FoamSize ) ) );
			#endif
			float Foam73 = ( _FoamOpacity * staticSwitch725 );
			float3 temp_cast_1 = (2.0).xxx;
			float FoamTex244 = staticSwitch725;
			float WaveFoam221 = saturate( ( pow( ( tex2DNode94.g * _WaveFoam ) , 2.0 ) * FoamTex244 ) );
			float3 lerpResult223 = lerp( ( lerpResult61 + Foam73 ) , temp_cast_1 , WaveFoam221);
			float3 FinalColor114 = lerpResult223;
			s733.Albedo = FinalColor114;
			fixed3 _BlankNormal = fixed3(0,0,1);
			float2 temp_output_705_0 = ( _NormalTiling * Tiling21 );
			#ifdef _NORMAL_MAP_ON
				float2 staticSwitch760 = ( ( float2( 0.25,0.25 ) * temp_output_705_0 ) + WaveSpeed40 );
			#else
				float2 staticSwitch760 = float2( 0.0,0 );
			#endif
			#ifdef _NORMAL_MAP_ON
				float2 staticSwitch761 = ( temp_output_705_0 + ( 1.0 - WaveSpeed40 ) );
			#else
				float2 staticSwitch761 = float2( 0.0,0 );
			#endif
			#ifdef _NORMAL_MAP_ON
				float3 staticSwitch763 = ( ( UnpackNormal( tex2D( _Normals, staticSwitch760 ) ) + UnpackNormal( tex2D( _Normals, staticSwitch761 ) ) ) / float3( 2,2,2 ) );
			#else
				float3 staticSwitch763 = _BlankNormal;
			#endif
			float3 lerpResult621 = lerp( _BlankNormal , staticSwitch763 , _NormalStrength);
			float3 NormalMap52 = lerpResult621;
			s733.Normal = WorldNormalVector( i, NormalMap52);
			s733.Emission = float3( 0,0,0 );
			s733.Metallic = 0.0;
			float GlossParam754 = _Glossiness;
			s733.Smoothness = GlossParam754;
			s733.Occlusion = 1.0;

			gi.light.ndotl = LambertTerm( s733.Normal, gi.light.dir );
			data.light = gi.light;

			UnityGI gi733 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g733;
			g733.roughness = 1 - s733.Smoothness;
			g733.reflUVW = reflect( -data.worldViewDir, s733.Normal );
			gi733 = UnityGlobalIllumination( data, s733.Occlusion, s733.Normal, g733 );
			#endif

			float3 surfResult733 = LightingStandard ( s733, viewDir, gi733 ).rgb;
			surfResult733 += s733.Emission;

			float3 ase_worldPos = i.worldPos;
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float dotResult741 = dot( ase_worldlightDir , WorldReflectionVector( i , NormalMap52 ) );
			#ifdef _LIGHTING_ON
				float3 staticSwitch769 = float3( 0.0,0,0 );
			#else
				float3 staticSwitch769 = ( pow( max( 0.0 , dotResult741 ) , ( GlossParam754 * 64.0 ) ) + lerp(( i.data746 * FinalColor114 ),FinalColor114,_Unlit) );
			#endif
			float3 CustomLighting753 = staticSwitch769;
			#ifdef _LIGHTING_ON
				float3 staticSwitch734 = surfResult733;
			#else
				float3 staticSwitch734 = CustomLighting753;
			#endif
			float Opacity121 = saturate( ( ( DepthTexture494 / _EdgeFade ) * ( Intersection42 + saturate( ( Depth479 + _WaterShallowColor.a ) ) ) ) );
			c.rgb = staticSwitch734;
			c.a = ( ( _Transparency * Opacity121 ) - (VertexColors729).g );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=13108
1927;29;1906;1004;5782.065;2066.579;4.055762;True;False
Node;AmplifyShaderEditor.CommentaryNode;347;-5301.821,-2139.555;Float;False;1499.929;585.7002;Comment;9;21;713;15;703;13;12;17;14;16;UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;16;-5223.622,-1759.556;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;14;-5214.021,-2089.556;Float;False;Constant;_Float0;Float 0;4;0;-20;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;17;-4998.623,-1765.556;Float;False;FLOAT2;0;2;2;2;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-5251.821,-1963.256;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;348;-5266.808,-1271.853;Float;False;1410.655;586.9989;Comment;8;40;337;39;500;38;35;320;714;Speed/direction;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-4959.623,-1992.955;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;703;-4807.515,-1705.867;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.1,0.1;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector4Node;320;-5140.667,-905.3926;Float;False;Property;_WaveDirection;WaveDirection;22;0;1,0,0,0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TimeNode;38;-5156.112,-1095.253;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;35;-5216.808,-1221.853;Float;False;Property;_Wavesspeed;Waves speed;18;0;0.75;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;15;-4610.623,-1842.556;Float;False;Property;_Worldspacetiling;Worldspace tiling;7;0;1;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-4869.8,-1148.653;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;500;-4905.578,-869.3902;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;337;-4663.672,-1020.493;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;629;-4041.018,2886.475;Float;False;1369.069;675.6616;Comment;8;581;344;92;88;90;301;87;302;Heightmap UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexToFragmentNode;713;-4287.368,-1833.505;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.GetLocalVarNode;87;-3884.539,2936.475;Float;False;21;0;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;302;-3991.018,3053.557;Float;False;Property;_WaveSize;Wave Size;21;0;0.1;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-4051.474,-1840.346;Float;False;Tiling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.VertexToFragmentNode;714;-4435.124,-1031.767;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-4175.821,-1024.427;Float;False;WaveSpeed;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.GetLocalVarNode;90;-3633.135,3269.07;Float;False;40;0;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;301;-3619.018,2987.557;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-3379.885,3068.508;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.1,0.1;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-3377.135,3292.07;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;630;-4266.805,41.43127;Float;False;1022.648;667.8272;Comment;9;22;23;24;355;354;41;356;648;649;Intersection UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;344;-3149.219,3179.451;Float;False;2;2;0;FLOAT2;0.0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;23;-4163.003,367.1515;Float;False;Property;_Rimtiling;Rim tiling;12;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;407;-2542.945,2823.105;Float;False;1786.026;846.3284;Comment;16;218;100;98;95;97;221;96;230;231;229;232;219;220;99;94;652;Heightmap;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;723;1808.2,-1169.643;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;22;-4189.418,265.4469;Float;False;21;0;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;638;-4682.635,1006.549;Float;False;1529.241;677.0369;Comment;12;637;636;64;634;65;604;632;63;633;628;676;677;SF UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;581;-2994.683,3182.493;Float;False;HeightmapUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;349;-3035.07,-106.9404;Float;False;2778.636;874.3949;Comment;24;42;10;102;426;425;420;497;496;3;5;495;444;222;233;30;29;28;686;709;710;712;732;731;767;Intersection;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;94;-2468.452,3199.244;Float;True;Property;_TextureSample6;Texture Sample 6;30;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;41;-3984.751,510.3699;Float;False;40;0;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;355;-3957.068,164.5515;Float;False;Constant;_Float13;Float 13;34;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-3937.519,289.1349;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ToggleSwitchNode;726;2042.612,-1200.198;Float;False;Property;_ENABLE_VC;ENABLE_VC;27;1;[Toggle];0;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;628;-4633.827,1214.088;Float;False;21;0;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;504;-3030.266,-1050.05;Float;False;1001.34;484.8032;Comment;7;494;643;479;642;646;104;647;Depth;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;729;2354.973,-1172.303;Float;False;VertexColors;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;63;-4337.898,1065.557;Float;False;Property;_FoamTiling;FoamTiling;16;0;0.05;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;99;-1934.15,3240.64;Float;False;Heightmap;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;731;-2991.032,81.64807;Float;False;729;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;604;-4228.341,1181.917;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2
Node;AmplifyShaderEditor.OneMinusNode;649;-3675.09,515.2559;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;633;-4103.035,1443.05;Float;False;Property;_FoamSpeed;FoamSpeed;14;0;0.1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;632;-4045.489,1354.466;Float;False;40;0;1;FLOAT2
Node;AmplifyShaderEditor.GetLocalVarNode;676;-4038.467,1535.054;Float;False;99;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;354;-3670.949,251.0045;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.SWS_Depth;643;-2642.008,-715.7141;Float;False;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;648;-3438.396,309.2903;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;677;-3741.903,1541.684;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.1;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-3755.193,1130.693;Float;False;2;2;0;FLOAT;0,0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.SwizzleNode;732;-2756.032,80.64807;Float;False;FLOAT;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;634;-3756.968,1370.898;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;356;-3443.304,466.8142;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;381;-3964.163,1959.834;Float;False;966.723;492.0804;Comment;8;47;659;342;46;341;339;705;18;Normals UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-3751.245,1242.404;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;311;-3042.07,1036.015;Float;False;2790.748;696.8248;Comment;13;73;624;244;71;721;673;720;670;69;66;67;722;725;Surface highlights;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;29;-2921.188,498.8604;Float;True;Property;_TextureSample1;Texture Sample 1;30;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;637;-3439.664,1376.837;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;2;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;636;-3447.636,1134.132;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;28;-2923.574,303.1307;Float;True;Property;_TextureSample0;Texture Sample 0;30;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;494;-2325.962,-728.4429;Float;False;DepthTexture;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;712;-2596.796,86.3121;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;18;-3930.826,2023.59;Float;False;Property;_NormalTiling;NormalTiling;8;0;0.9;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;46;-3919.359,2134.635;Float;False;21;0;1;FLOAT2
Node;AmplifyShaderEditor.GetLocalVarNode;495;-2833.993,-44.65842;Float;False;494;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;66;-2897.47,1109.127;Float;True;Property;_TextureSample4;Texture Sample 4;30;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-2480.348,451.9202;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-2333.595,101.7485;Half;False;Property;_Rimfalloff;Rim falloff;11;0;3;0.1;50;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;705;-3544.359,2132.163;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.GetLocalVarNode;47;-3930.563,2316.432;Float;False;40;0;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;67;-2910.186,1325.706;Float;True;Property;_TextureSample5;Texture Sample 5;30;0;None;True;0;False;white;Auto;False;Instance;27;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;710;-2401.259,-43.75025;Float;False;Property;_USE_VC_INTERSECTION;USE_VC_INTERSECTION;25;1;[Toggle];0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;647;-2935.302,-939.4899;Float;False;494;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;670;-2475.899,1133.251;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;496;-1931.477,76.0475;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;709;-1801.515,319.9915;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;-2338.022,229.7638;Half;False;Property;_RimSize;Rim Size;10;0;5;0;20;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;69;-2428.874,1251.174;Half;False;Property;_FoamSize;FoamSize;15;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;659;-3495.744,2325.258;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;342;-3346.247,2027.306;Float;False;2;2;0;FLOAT2;0.25,0.25;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;104;-2985.575,-825.7819;Float;False;Property;_Depth;Depth;17;0;30;0;100;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;720;-2171.777,1140.326;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;646;-2667.876,-898.7989;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;503;-3059.798,-1737.311;Float;False;939.0676;526.2443;Comment;7;350;478;477;482;60;765;766;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;497;-1923.585,198.9694;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;341;-3170.445,2110.015;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;339;-3176.647,2251.115;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;420;-1688.83,85.64011;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;642;-2494.417,-898.4079;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;425;-1474.603,154.4005;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;477;-3009.798,-1687.31;Half;False;Property;_WaterShallowColor;WaterShallowColor;1;0;0.4191176,0.7596349,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;721;-1963.777,1116.389;Float;False;2;2;0;FLOAT;1000.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StaticSwitch;760;-2883.452,2038.439;Float;False;Property;_NORMAL_MAP;NORMAL_MAP;28;0;1;False;True;;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;502;-2522.438,1980.171;Float;False;1882.752;558.178;Comment;9;52;621;128;622;661;660;50;45;763;Small Wave Normals;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;60;-2998.926,-1491.732;Half;False;Property;_WaterColor;Water Color;0;0;0.1176471,0.6348885,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StaticSwitch;761;-2913.921,2233.657;Float;False;Property;_NORMAL_MAP;NORMAL_MAP;28;0;1;False;True;;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.OneMinusNode;673;-2429.768,1384.268;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;220;-2340.323,3450.925;Float;False;Property;_WaveFoam;Wave Foam;20;0;0;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;426;-1174.303,160.3223;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;765;-2776.609,-1673.693;Float;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SaturateNode;722;-1764.92,1139.873;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;479;-2302.576,-901.9539;Float;False;Depth;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;352;-3067.381,4011.095;Float;False;2891.381;468.422;Comment;16;114;223;315;224;625;79;61;475;141;62;476;103;111;351;112;110;Master lerp;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;50;-2472.438,2257.07;Float;True;Property;_TextureSample3;Texture Sample 3;29;0;None;True;0;False;white;Auto;True;Instance;43;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;766;-2751.609,-1528.693;Float;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;482;-2974.599,-1316.611;Float;False;479;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;45;-2469.642,2030.171;Float;True;Property;_TextureSample2;Texture Sample 2;29;0;None;True;0;False;white;Auto;True;Instance;43;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;10;-1177.727,-39.95768;Float;False;Property;_RimColor;Rim Color;3;0;1,1,1,0.5019608;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;478;-2544.895,-1559.009;Float;False;3;0;FLOAT3;0.0,0,0,0;False;1;FLOAT3;0,0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;110;-2992.428,4167.476;Float;False;99;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;232;-1983.623,3559.724;Fixed;False;Constant;_Float7;Float 7;25;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;112;-3017.381,4318.741;Fixed;False;Property;_Wavetint;Wave tint;2;0;0;-1;1;0;1;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;767;-844.6658,-30.29321;Float;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;444;-874.3505,111.0203;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;660;-2075.106,2186.271;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;-1985.122,3368.425;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StaticSwitch;725;-1505.115,1302.903;Float;False;Property;_UseIntersectionFoam;UseIntersectionFoam;26;0;0;False;True;;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;103;-2343.248,4199.134;Float;False;102;0;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;71;-1334.413,1109.331;Fixed;False;Property;_FoamOpacity;FoamOpacity;13;0;0.05;-1;1;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;351;-2662.558,4061.095;Float;False;350;0;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-516.3961,-52.14754;Float;False;RimColor;-1;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;350;-2341.23,-1551.045;Float;False;WaterColor;-1;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;229;-1768.523,3546.525;Float;False;244;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;231;-1778.023,3372.025;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-2627.316,4156.233;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;686;-683.2039,105.8938;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;622;-1807.969,2028.126;Fixed;False;Constant;_BlankNormal;BlankNormal;36;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;244;-980.6179,1365.102;Float;False;FoamTex;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;476;-2342.437,4297.366;Fixed;False;Constant;_Float2;Float 2;34;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;661;-1896.977,2185.114;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;2,2,2;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;128;-1795.855,2332.987;Fixed;False;Property;_NormalStrength;NormalStrength;4;0;0.25;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;62;-2130.268,4348.27;Float;False;42;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;42;-504.6126,92.63463;Float;False;Intersection;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-1449.523,3376.724;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StaticSwitch;763;-1554.723,2176.267;Float;False;Property;_NORMAL_MAP;NORMAL_MAP;30;0;1;False;True;;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;475;-2095.437,4208.366;Float;False;2;2;0;FLOAT3;0,0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;624;-958.9775,1155.225;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;141;-2386.586,4073.144;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;621;-1211.594,2132.534;Float;False;3;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;79;-1771.724,4351.879;Float;False;73;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;61;-1727.146,4076.186;Float;False;3;0;FLOAT3;0,0,0,0;False;1;FLOAT3;1,1,1,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.CommentaryNode;380;-3073.8,-2416.792;Float;False;2197.571;521.1841;Comment;11;121;657;134;119;687;498;489;480;2;694;695;Opacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;764;-3072.667,4644.836;Float;False;1862.706;736.2534;Comment;17;739;753;769;751;768;748;749;745;744;741;755;747;746;738;740;742;758;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-735.7233,1163.7;Float;False;Foam;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;652;-1253.644,3383.992;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;758;-3005.049,4801.563;Float;False;52;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;625;-1453.824,4079.357;Float;False;2;2;0;FLOAT3;0,0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;224;-1341.485,4376.117;Float;False;221;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;480;-2993.971,-2042.603;Float;False;479;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-907.3911,2119.4;Float;False;NormalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;221;-1029.021,3376.225;Float;False;WaveFoam;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;315;-1297.257,4235.156;Fixed;False;Constant;_Float9;Float 9;32;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;223;-971.7388,4078.296;Float;False;3;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;738;-3022.667,4694.836;Float;False;1;0;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;694;-2680.013,-2002.087;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldReflectionVector;740;-2763.121,4805.972;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LightColorNode;742;-2993.092,5172.526;Float;False;0;3;COLOR;FLOAT3;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;56;1110.652,-247.9698;Fixed;False;Property;_Glossiness;Glossiness;6;0;0.85;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;2;-2985.028,-2246.265;Half;False;Property;_EdgeFade;EdgeFade;9;0;0.2448298;0.01;3;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;119;-2494.39,-2239.521;Float;False;42;0;1;FLOAT
Node;AmplifyShaderEditor.VertexToFragmentNode;746;-2780.771,5184.248;Float;False;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;114;-771.7118,4072.191;Float;False;FinalColor;-1;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;747;-2774.481,5288.084;Float;False;114;0;1;FLOAT3
Node;AmplifyShaderEditor.DotProductOpNode;741;-2492.403,4744.969;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;754;1418.436,-246.6733;Float;False;GlossParam;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;755;-2554.393,4974.859;Float;False;754;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;489;-2941.714,-2358.903;Float;False;494;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;695;-2526.013,-2013.087;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;134;-2174.178,-2183.131;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;498;-2640.725,-2337.706;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;749;-2518.679,5208.56;Float;False;2;2;0;FLOAT3;0.0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;744;-2307.534,4978.774;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;64.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;745;-2333.309,4722.092;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;687;-1928.494,-2299.849;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;768;-2199.795,5193.049;Float;False;Property;_Unlit;Unlit;29;1;[Toggle];0;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.PowerNode;748;-2064.552,4977.665;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;512.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;657;-1620.808,-2304.757;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;96;-2143.252,2949.544;Fixed;False;Property;_WaveHeight;Wave Height;19;0;0.05;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;751;-1836.979,5103.686;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;639;1511.193,43.50829;Float;False;121;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;117;1524.936,-54.92926;Fixed;False;Property;_Transparency;Transparency;5;0;0.75;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;53;1430.341,-340.943;Float;False;52;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1808.653,3074.04;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;730;1478.936,146.8509;Float;False;729;0;1;COLOR
Node;AmplifyShaderEditor.StaticSwitch;769;-1676.233,5082.75;Float;False;Property;_LIGHTING;LIGHTING;30;0;1;False;True;;2;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;121;-1453.981,-2309.958;Float;False;Opacity;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;97;-1805.996,2891.624;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;756;1436.547,-425.2564;Float;False;114;0;1;FLOAT3
Node;AmplifyShaderEditor.CustomStandardSurface;733;1795.785,-370.4509;Float;False;False;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;1.0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;753;-1503.72,5083.599;Float;False;CustomLighting;-1;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;115;1801.9,-156.475;Float;False;753;0;1;FLOAT3
Node;AmplifyShaderEditor.SwizzleNode;728;1734.936,135.8509;Float;False;FLOAT;1;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-1490.751,2940.04;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;702;1847.942,-26.74405;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;101;2035.883,198.4911;Float;False;100;0;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;43;2233.069,-970.2504;Float;True;Property;_Normals;Normals;23;2;[NoScaleOffset];[Normal];None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.VertexToFragmentNode;739;-2749.297,4694.884;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;100;-1057.476,2932.911;Float;False;Displacement;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;233;-2231.797,587.783;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StaticSwitch;734;2161.471,-251.5607;Float;False;Property;_LIGHTING;LIGHTING;28;0;1;False;True;;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;724;2044.316,47.10138;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;27;1820.076,-971.6935;Float;True;Property;_Shadermap;Shadermap;24;1;[NoScaleOffset];None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;222;-1968.1,563.5;Float;False;IntersectionTex;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;218;-1561.298,3137.152;Float;False;WaveHeight;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2385.9,-238;Float;False;True;2;Float;;200;0;CustomLighting;StylizedWater/Mobile;False;False;False;False;False;False;True;True;True;False;True;True;False;False;True;True;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;False;True;True;False;False;False;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;200;;-1;-1;-1;-1;0;0;1;fragmentoption ARB_precision_hint_fastest;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;16;0
WireConnection;13;0;14;0
WireConnection;13;1;12;0
WireConnection;703;0;17;0
WireConnection;15;0;13;0
WireConnection;15;1;703;0
WireConnection;39;0;35;0
WireConnection;39;1;38;1
WireConnection;500;0;320;1
WireConnection;500;1;320;3
WireConnection;337;0;39;0
WireConnection;337;1;500;0
WireConnection;713;0;15;0
WireConnection;21;0;713;0
WireConnection;714;0;337;0
WireConnection;40;0;714;0
WireConnection;301;0;87;0
WireConnection;301;1;302;0
WireConnection;88;0;301;0
WireConnection;92;0;90;0
WireConnection;344;0;88;0
WireConnection;344;1;92;0
WireConnection;581;0;344;0
WireConnection;94;1;581;0
WireConnection;24;0;22;0
WireConnection;24;1;23;0
WireConnection;726;1;723;0
WireConnection;729;0;726;0
WireConnection;99;0;94;2
WireConnection;604;0;628;0
WireConnection;649;0;41;0
WireConnection;354;0;355;0
WireConnection;354;1;24;0
WireConnection;648;0;354;0
WireConnection;648;1;41;0
WireConnection;677;0;676;0
WireConnection;64;0;63;0
WireConnection;64;1;604;0
WireConnection;732;0;731;0
WireConnection;634;0;632;0
WireConnection;634;1;633;0
WireConnection;356;0;24;0
WireConnection;356;1;649;0
WireConnection;65;0;63;0
WireConnection;65;1;628;0
WireConnection;29;1;356;0
WireConnection;637;0;65;0
WireConnection;637;1;634;0
WireConnection;637;2;677;0
WireConnection;636;0;64;0
WireConnection;636;1;634;0
WireConnection;28;1;648;0
WireConnection;494;0;643;0
WireConnection;712;0;732;0
WireConnection;66;1;636;0
WireConnection;30;0;28;3
WireConnection;30;1;29;3
WireConnection;705;0;18;0
WireConnection;705;1;46;0
WireConnection;67;1;637;0
WireConnection;710;0;495;0
WireConnection;710;1;712;0
WireConnection;670;0;66;1
WireConnection;670;1;67;1
WireConnection;496;0;710;0
WireConnection;496;1;5;0
WireConnection;709;0;30;0
WireConnection;659;0;47;0
WireConnection;342;1;705;0
WireConnection;720;0;670;0
WireConnection;720;1;69;0
WireConnection;646;0;647;0
WireConnection;646;1;104;0
WireConnection;497;0;710;0
WireConnection;497;1;3;0
WireConnection;341;0;342;0
WireConnection;341;1;47;0
WireConnection;339;0;705;0
WireConnection;339;1;659;0
WireConnection;420;0;496;0
WireConnection;420;1;709;0
WireConnection;642;0;646;0
WireConnection;425;0;420;0
WireConnection;425;1;497;0
WireConnection;721;1;720;0
WireConnection;760;0;341;0
WireConnection;761;0;339;0
WireConnection;673;0;67;3
WireConnection;426;0;425;0
WireConnection;765;0;477;0
WireConnection;722;0;721;0
WireConnection;479;0;642;0
WireConnection;50;1;761;0
WireConnection;766;0;60;0
WireConnection;45;1;760;0
WireConnection;478;0;765;0
WireConnection;478;1;766;0
WireConnection;478;2;482;0
WireConnection;767;0;10;0
WireConnection;444;0;10;4
WireConnection;444;1;426;0
WireConnection;660;0;45;0
WireConnection;660;1;50;0
WireConnection;219;0;94;2
WireConnection;219;1;220;0
WireConnection;725;0;673;0
WireConnection;725;1;722;0
WireConnection;102;0;767;0
WireConnection;350;0;478;0
WireConnection;231;0;219;0
WireConnection;231;1;232;0
WireConnection;111;0;110;0
WireConnection;111;1;112;0
WireConnection;686;0;444;0
WireConnection;244;0;725;0
WireConnection;661;0;660;0
WireConnection;42;0;686;0
WireConnection;230;0;231;0
WireConnection;230;1;229;0
WireConnection;763;0;661;0
WireConnection;763;1;622;0
WireConnection;475;0;103;0
WireConnection;475;1;476;0
WireConnection;624;0;71;0
WireConnection;624;1;725;0
WireConnection;141;0;351;0
WireConnection;141;1;111;0
WireConnection;621;0;622;0
WireConnection;621;1;763;0
WireConnection;621;2;128;0
WireConnection;61;0;141;0
WireConnection;61;1;475;0
WireConnection;61;2;62;0
WireConnection;73;0;624;0
WireConnection;652;0;230;0
WireConnection;625;0;61;0
WireConnection;625;1;79;0
WireConnection;52;0;621;0
WireConnection;221;0;652;0
WireConnection;223;0;625;0
WireConnection;223;1;315;0
WireConnection;223;2;224;0
WireConnection;694;0;480;0
WireConnection;694;1;477;4
WireConnection;740;0;758;0
WireConnection;746;0;742;1
WireConnection;114;0;223;0
WireConnection;741;0;738;0
WireConnection;741;1;740;0
WireConnection;754;0;56;0
WireConnection;695;0;694;0
WireConnection;134;0;119;0
WireConnection;134;1;695;0
WireConnection;498;0;489;0
WireConnection;498;1;2;0
WireConnection;749;0;746;0
WireConnection;749;1;747;0
WireConnection;744;0;755;0
WireConnection;745;1;741;0
WireConnection;687;0;498;0
WireConnection;687;1;134;0
WireConnection;768;0;749;0
WireConnection;768;1;747;0
WireConnection;748;0;745;0
WireConnection;748;1;744;0
WireConnection;657;0;687;0
WireConnection;751;0;748;0
WireConnection;751;1;768;0
WireConnection;95;0;96;0
WireConnection;95;1;94;2
WireConnection;769;1;751;0
WireConnection;121;0;657;0
WireConnection;733;0;756;0
WireConnection;733;1;53;0
WireConnection;733;4;754;0
WireConnection;753;0;769;0
WireConnection;728;0;730;0
WireConnection;98;0;97;0
WireConnection;98;1;95;0
WireConnection;702;0;117;0
WireConnection;702;1;639;0
WireConnection;739;0;738;0
WireConnection;100;0;98;0
WireConnection;233;0;30;0
WireConnection;734;0;733;0
WireConnection;734;1;115;0
WireConnection;724;0;702;0
WireConnection;724;1;728;0
WireConnection;222;0;233;0
WireConnection;218;0;95;0
WireConnection;0;2;734;0
WireConnection;0;9;724;0
WireConnection;0;11;101;0
ASEEND*/
//CHKSM=63FE132E2B215A9DEFA10BAD3311A4C152952CC4