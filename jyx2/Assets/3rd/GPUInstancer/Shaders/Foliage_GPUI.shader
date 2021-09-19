// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GPUInstancer/Foliage"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_WindWaveNormalTexture("Wind Wave Normal Texture", 2D) = "bump" {}
		_WindWaveSize("Wind Wave Size", Range( 0 , 1)) = 0.5
		_DryColor("Dry Color", Color) = (1,0,0,0)
		_HealthyColor("Healthy Color", Color) = (0,1,0.2137935,0)
		_MainTex("MainTex", 2D) = "white" {}
		_AmbientOcclusion("Ambient Occlusion", Range( 0 , 1)) = 0.5
		_GradientPower("Gradient Power", Range( 0 , 1)) = 0.3
		_NoiseSpread("Noise Spread", Float) = 0.1
		_NormalMap("Normal Map", 2D) = "bump" {}
		_WindVector("Wind Vector", Vector) = (0.4,0.8,0,0)
		[Toggle]_IsBillboard("IsBillboard", Float) = 0
		_WindWaveTintColor("Wind Wave Tint Color", Color) = (0.07586241,0,1,0)
		_HealthyDryNoiseTexture("Healthy Dry Noise Texture", 2D) = "white" {}
		[Toggle]_WindWavesOn("Wind Waves On", Float) = 0
		_WindWaveTint("Wind Wave Tint", Range( 0 , 1)) = 0.5
		_WindWaveSway("Wind Wave Sway", Range( 0 , 1)) = 0.5
		_WindIdleSway("Wind Idle Sway", Range( 0 , 1)) = 0.6
		[Toggle(_BILLBOARDFACECAMPOS_ON)] _BillboardFaceCamPos("BillboardFaceCamPos", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "GPUIFoliage"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile __ _BILLBOARDFACECAMPOS_ON
		#include "UnityCG.cginc"
		#include "Include/GPUInstancerInclude.cginc"
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setupGPUI
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float vertexLocalY;
		};

		uniform float _IsBillboard;
		uniform float _WindWavesOn;
		uniform sampler2D _WindWaveNormalTexture;
		uniform float2 _WindVector;
		uniform float _WindWaveSize;
		uniform float _WindIdleSway;
		uniform float _WindWaveSway;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _GradientPower;
		uniform float4 _HealthyColor;
		uniform float4 _DryColor;
		uniform sampler2D _HealthyDryNoiseTexture;
		uniform float _NoiseSpread;
		uniform float4 _WindWaveTintColor;
		uniform float _WindWaveTint;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _AmbientOcclusion;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float BillboardOn261 = lerp(0.0,1.0,_IsBillboard);
			float4 ase_vertex4Pos = v.vertex;
			float4x4 break301 = unity_ObjectToWorld;
			float3 appendResult302 = (float3(break301[ 0 ][ 0 ] , break301[ 1 ][ 0 ] , break301[ 2 ][ 0 ]));
			float3 appendResult306 = (float3(break301[ 0 ][ 1 ] , break301[ 1 ][ 1 ] , break301[ 2 ][ 1 ]));
			float3 appendResult307 = (float3(break301[ 0 ][ 2 ] , break301[ 1 ][ 2 ] , break301[ 2 ][ 2 ]));
			float4 appendResult303 = (float4(( ase_vertex4Pos.x * length( appendResult302 ) ) , ( ase_vertex4Pos.y * length( appendResult306 ) ) , ( ase_vertex4Pos.z * length( appendResult307 ) ) , ase_vertex4Pos.w));
			float4x4 break278 = UNITY_MATRIX_V;
			float3 appendResult287 = (float3(break278[ 0 ][ 0 ] , break278[ 0 ][ 1 ] , break278[ 0 ][ 2 ]));
			float3 normalizeResult288 = normalize( appendResult287 );
			float3 appendResult295 = (float3(normalizeResult288));
			float3 appendResult314 = (float3(break301[ 0 ][ 3 ] , break301[ 1 ][ 3 ] , break301[ 2 ][ 3 ]));
			float3 normalizeResult504 = normalize( cross( float3(0,1,0) , ( appendResult314 - _WorldSpaceCameraPos ) ) );
			#ifdef _BILLBOARDFACECAMPOS_ON
				float3 staticSwitch496 = normalizeResult504;
			#else
				float3 staticSwitch496 = appendResult295;
			#endif
			float3 appendResult279 = (float3(break278[ 1 ][ 0 ] , break278[ 1 ][ 1 ] , break278[ 1 ][ 2 ]));
			float3 normalizeResult283 = normalize( appendResult279 );
			float3 appendResult296 = (float3(normalizeResult283));
			float temp_output_416_0 = (appendResult296).y;
			float3 break419 = appendResult296;
			float4 appendResult420 = (float4(break419.x , ( temp_output_416_0 * -1.0 ) , break419.z , 0.0));
			#ifdef _BILLBOARDFACECAMPOS_ON
				float4 staticSwitch498 = float4(0,1,0,0);
			#else
				float4 staticSwitch498 = (( temp_output_416_0 > 0.0 ) ? float4( appendResult296 , 0.0 ) :  appendResult420 );
			#endif
			float3 appendResult281 = (float3(break278[ 2 ][ 0 ] , break278[ 2 ][ 1 ] , break278[ 2 ][ 2 ]));
			float3 normalizeResult284 = normalize( appendResult281 );
			float3 appendResult297 = (float3(( normalizeResult284 * -1.0 )));
			float3 appendResult322 = (float3(mul( appendResult303, float4x4(float4( staticSwitch496 , 0.0 ), staticSwitch498, float4( appendResult297 , 0.0 ), float4( 0,0,0,0 )) ).xyz));
			float4 appendResult323 = (float4(( appendResult322 + appendResult314 ) , ase_vertex4Pos.w));
			float4 transform327 = mul(unity_WorldToObject,appendResult323);
			float4 BillboardedVertexPos320 = transform327;
			float WindWavesOn112 = lerp(0.0,1.0,_WindWavesOn);
			float2 WindDirVector29 = _WindVector;
			float mulTime407 = _Time.y * ( length( WindDirVector29 ) * 0.01 );
			float WindWaveSize128 = _WindWaveSize;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 panner36 = ( mulTime407 * WindDirVector29 + ( ( 1.0 - (0.0 + (WindWaveSize128 - 0.0) * (0.9 - 0.0) / (1.0 - 0.0)) ) * 0.003 * (ase_worldPos).xz ));
			float3 tex2DNode2 = UnpackNormal( tex2Dlod( _WindWaveNormalTexture, float4( panner36, 0, 0.0) ) );
			float3 FinalWindVectors181 = tex2DNode2;
			float3 break185 = FinalWindVectors181;
			float3 appendResult186 = (float3(break185.x , 0.0 , break185.y));
			float WindIdleSway197 = _WindIdleSway;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 lerpResult230 = lerp( float3( 0,0,0 ) , ( appendResult186 * WindIdleSway197 ) , saturate( ase_vertex3Pos.y ));
			float3 WindIdleSwayCalculated218 = lerpResult230;
			float WindWaveSway191 = _WindWaveSway;
			float WindWaveNoise126 = saturate( tex2DNode2.g );
			float2 break206 = ( WindWaveNoise126 * WindDirVector29 );
			float3 appendResult205 = (float3(break206.x , 0.0 , break206.y));
			float3 lerpResult229 = lerp( float3( 0,0,0 ) , ( WindWaveSway191 * 20.0 * appendResult205 ) , saturate( ase_vertex3Pos.y ));
			float3 WindWaveSwayCalculated220 = lerpResult229;
			float3 lerpResult215 = lerp( WindIdleSwayCalculated218 , ( WindIdleSwayCalculated218 + ( WindWaveSwayCalculated220 * -1.0 ) ) , WindWaveNoise126);
			float4 transform233 = mul(unity_WorldToObject,float4( (( WindWavesOn112 > 0.0 ) ? lerpResult215 :  WindIdleSwayCalculated218 ) , 0.0 ));
			float4 WindVertexOffset183 = transform233;
			float4 FinalVertexPos336 = (( BillboardOn261 > 0.0 ) ? ( BillboardedVertexPos320 + WindVertexOffset183 ) :  ( WindVertexOffset183 + ase_vertex4Pos ) );
			v.vertex.xyz = FinalVertexPos336.xyz;
			v.normal = float3(0,1,0);

			o.vertexLocalY = mul( unity_WorldToObject, float4( ase_worldPos , 1 ) ).y;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			//float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float GradientPower161 = _GradientPower;
			float GradientColor160 = (( 1.0 - GradientPower161 ) + (saturate( i.vertexLocalY ) - 0.0) * (1.0 - ( 1.0 - GradientPower161 )) / (1.0 - 0.0));
			float WindWavesOn112 = lerp(0.0,1.0,_WindWavesOn);
			float4 HealthyColor116 = _HealthyColor;
			float4 DryColor118 = _DryColor;
			float3 ase_worldPos = i.worldPos;
			float NoiseSpread351 = _NoiseSpread;
			float div364=256.0/float((int)32.0);
			float4 posterize364 = ( floor( tex2D( _HealthyDryNoiseTexture, ( ( (ase_worldPos).xz * 0.05 ) * NoiseSpread351 ) ) * div364 ) / div364 );
			float4 break365 = posterize364;
			float HealthyDryNoise140 = saturate( sqrt( ( break365.r * break365.g * break365.b ) ) );
			float4 lerpResult59 = lerp( HealthyColor116 , DryColor118 , HealthyDryNoise140);
			float4 HealthyDryTint60 = lerpResult59;
			float4 WindWaveTintColor122 = _WindWaveTintColor;
			float2 WindDirVector29 = _WindVector;
			float mulTime407 = _Time.y * ( length( WindDirVector29 ) * 0.01 );
			float WindWaveSize128 = _WindWaveSize;
			float2 panner36 = ( mulTime407 * WindDirVector29 + ( ( 1.0 - (0.0 + (WindWaveSize128 - 0.0) * (0.9 - 0.0) / (1.0 - 0.0)) ) * 0.003 * (ase_worldPos).xz ));
			float3 tex2DNode2 = UnpackNormal( tex2D( _WindWaveNormalTexture, panner36 ) );
			float WindWaveNoise126 = saturate( tex2DNode2.g );
			float WindWaveTintPower135 = _WindWaveTint;
			float4 lerpResult82 = lerp( HealthyDryTint60 , WindWaveTintColor122 , saturate( ( WindWaveNoise126 * WindWaveTintPower135 * 5.0 ) ));
			float4 WindWaveTint137 = lerpResult82;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode166 = tex2D( _MainTex, uv_MainTex );
			float4 FinalAlbedo152 = ( GradientColor160 * ( (( WindWavesOn112 > 0.0 ) ? WindWaveTint137 :  HealthyDryTint60 ) * tex2DNode166 ) );
			o.Albedo = FinalAlbedo152.rgb;
			float AmbientOcclusionPower163 = _AmbientOcclusion;
			float clampResult148 = clamp( ( saturate( i.vertexLocalY ) * AmbientOcclusionPower163 ) , 0.0 , 1.0 );
			float lerpResult150 = lerp( 1.0 , clampResult148 , AmbientOcclusionPower163);
			float AmbientOcclusion151 = lerpResult150;
			o.Occlusion = AmbientOcclusion151;
			o.Alpha = 1;
			float AlphaCutoff167 = tex2DNode166.a;
			clip( AlphaCutoff167 - _Cutoff );
		}

		ENDCG
	}
	//Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15600
6.285715;6.857143;2182;1160;1818.801;2051.375;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;130;-4776.251,-469.1541;Float;False;813.948;1066.223;Wind related parameters;13;191;135;128;122;66;190;132;17;113;197;196;29;28;Wind Parameters;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;48;-3792.662,-258.5315;Float;False;2253.782;581.4224;Wind waves noise map calculation;17;402;476;406;404;403;3;8;36;407;381;379;431;31;181;126;454;2;Wind Waves Noise;0.2640571,0.4734817,0.8161765,1;0;0
Node;AmplifyShaderEditor.Vector2Node;28;-4730.966,-416.8831;Float;False;Property;_WindVector;Wind Vector;10;0;Create;True;0;0;False;0;0.4,0.8;0,-1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;17;-4726.808,290.4856;Float;False;Property;_WindWaveSize;Wind Wave Size;2;0;Create;True;0;0;False;0;0.5;0.359;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;335;-3782.486,-3856.065;Float;False;3635.969;1958.271;Billboarding matrix calculations. Native ASE billboarding solution does not allow billboard toggling.;52;503;502;500;501;314;496;498;425;499;492;417;420;286;284;288;421;285;419;287;281;416;283;279;278;271;320;323;319;422;315;322;312;424;303;311;305;423;309;310;304;299;308;333;306;302;307;301;300;332;330;331;505;Spherical Billboarding;0.9448276,1,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;402;-3768.618,-201.8272;Float;False;128;WindWaveSize;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;128;-4345.342,290.4856;Float;False;WindWaveSize;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-3478.771,48.08096;Float;False;29;WindDirVector;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-4346.821,-415.7327;Float;False;WindDirVector;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;476;-3501.358,-201.0782;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;379;-3266.625,232.4436;Float;False;Constant;_Float13;Float 13;20;0;Create;True;0;0;False;0;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewMatrixNode;271;-3703.007,-2442.498;Float;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.WorldPosInputsNode;3;-3766.468,-23.727;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LengthOpNode;431;-3230.005,148.1057;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;381;-3080.107,144.3363;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;8;-3487.779,-31.29717;Float;False;FLOAT2;0;2;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;404;-3277.4,-79.08368;Float;False;Constant;_Float0;Float 0;20;0;Create;True;0;0;False;0;0.003;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;406;-3270.251,-160.5732;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;278;-3549.711,-2443.538;Float;False;FLOAT4x4;1;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;279;-3134.316,-2285.431;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;407;-2938.271,142.419;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;49;-3788.507,-853.5095;Float;False;2130.659;501.2978;Healthy/dry color noise map calculation;14;367;140;364;365;353;368;61;65;63;352;53;52;51;50;Healthy/Dry Noise;0.2627451,0.4745098,0.8156863,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;403;-2937.443,-69.18651;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ObjectToWorldMatrixNode;300;-3382.255,-3602.899;Float;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.CommentaryNode;331;-2726.662,-2336.055;Float;False;223;160;UpCamVec;1;296;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;36;-2743.686,28.39046;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;121;-4778.988,-1874.603;Float;False;650.0684;815.7568;Color related parameters and AO;10;163;145;161;155;116;118;119;57;351;64;Color Parameters;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;50;-3762.304,-777.8906;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;283;-2978.624,-2285.588;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;2;-2463.012,-188.7563;Float;True;Property;_WindWaveNormalTexture;Wind Wave Normal Texture;1;0;Create;True;0;0;False;0;78384f2a63e207744a3b1821e032ee03;78384f2a63e207744a3b1821e032ee03;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;296;-2676.662,-2286.055;Float;False;FLOAT3;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;301;-3143.544,-3601.687;Float;False;FLOAT4x4;1;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SwizzleNode;51;-3559.911,-783.0955;Float;False;FLOAT2;0;2;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-4727.625,-1206.72;Float;False;Property;_NoiseSpread;Noise Spread;8;0;Create;True;0;0;False;0;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-3563.869,-624.9954;Float;False;Constant;_Float1;Float 1;0;0;Create;True;0;0;False;0;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;454;-2104.865,-111.5891;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;281;-3214.304,-2139.984;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;500;-2820.988,-2832.468;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;314;-2729.535,-3163.162;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;224;-3816.575,992.6206;Float;False;1812.334;469.6074;Wind wave sway vertex offset calculation;12;460;211;210;220;229;192;208;205;206;199;204;195;Wind Wave Sway;0.1647059,0.5803922,0.1098039,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-3359.272,-707.178;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;351;-4410.191,-1206.771;Float;False;NoiseSpread;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;352;-3371.629,-591.4212;Float;False;351;NoiseSpread;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;416;-2410.457,-2385.654;Float;False;FLOAT;1;1;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;287;-3140.774,-2458.427;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;126;-1941.614,-60.31606;Float;False;WindWaveNoise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;204;-3768.537,1230.474;Float;False;29;WindDirVector;0;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;195;-3773.284,1140.805;Float;False;126;WindWaveNoise;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;284;-3045.444,-2141.106;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;421;-2192.242,-2271.506;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;288;-2994.716,-2458.427;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;285;-3049.156,-2064.004;Float;False;Constant;_Float9;Float 9;18;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;419;-2457.462,-2213.921;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-3163.857,-654.8336;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;330;-2724.442,-2510.669;Float;False;223;160;RightCamVec;1;295;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector3Node;502;-2342.076,-3048.509;Float;False;Constant;_Vector2;Vector 2;18;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;501;-2482.622,-2918.287;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;420;-2020.113,-2216.79;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;286;-2871.156,-2142.004;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;505;-1872.738,-2980.478;Float;False;236.2856;159.7144;CamPosRightVec;1;504;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CrossProductOpNode;503;-2120.728,-2934.482;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;199;-3444.645,1187.885;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-2928.52,-477.6389;Float;False;Constant;_Float2;Float 2;8;0;Create;True;0;0;False;0;32;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;61;-3020.064,-684.1801;Float;True;Property;_HealthyDryNoiseTexture;Healthy Dry Noise Texture;13;0;Create;True;0;0;False;0;5d221d423a7e56d4d8e9bff0c4a73886;5d221d423a7e56d4d8e9bff0c4a73886;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;332;-2727.366,-2161.76;Float;False;223;160;ForwardCamVec;1;297;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;223;-3793.596,391.8932;Float;False;1562.368;497.6409;Wind idle sway vertex offset calculation;9;182;185;186;194;198;187;218;228;230;Wind Idle Sway;0.1661347,0.5808823,0.111051,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;295;-2674.442,-2460.669;Float;False;FLOAT3;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;492;-1623.725,-2265.632;Float;False;Constant;_Vector1;Vector 1;19;0;Create;True;0;0;False;0;0,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;499;-2108.68,-2652.138;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;181;-1822.424,-187.6183;Float;False;FinalWindVectors;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;182;-3786.596,491.4998;Float;False;181;FinalWindVectors;0;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCCompareGreater;417;-1842.971,-2390.484;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-4727.42,476.6104;Float;False;Property;_WindWaveSway;Wind Wave Sway;16;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosterizeNode;364;-2717.182,-583.5996;Float;False;1;2;1;COLOR;0,0,0,0;False;0;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;206;-3223.668,1184.09;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.NormalizeNode;504;-1822.738,-2930.478;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;297;-2677.366,-2111.761;Float;False;FLOAT3;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;208;-2999.599,1317.661;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;307;-2732.147,-3311.017;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;333;-1091.653,-2705.52;Float;False;310;229;RotationCamMatrix;1;292;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;302;-2740.745,-3586.998;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;460;-2865.781,1110.94;Float;False;Constant;_Float12;Float 12;19;0;Create;True;0;0;False;0;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;185;-3479.431,494.4323;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;306;-2740.758,-3455.263;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;191;-4345.42,476.6104;Float;False;WindWaveSway;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;498;-1477.998,-2607.716;Float;False;Property;_BillboardFaceCamPos;BillboardFaceCamPos;18;0;Create;True;0;0;False;0;1;0;0;True;;Toggle;2;Key0;Key1;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;425;-1434.565,-2063.344;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;365;-2541.298,-598.0021;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;192;-2934.962,1032.316;Float;False;191;WindWaveSway;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;496;-1476.998,-2704.716;Float;False;Property;_BillboardFaceCamPos;BillboardFaceCamPos;19;0;Create;True;0;0;False;0;1;0;0;True;;Toggle;2;Key0;Key1;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;196;-4727.118,-281.2434;Float;False;Property;_WindIdleSway;Wind Idle Sway;17;0;Create;True;0;0;False;0;0.6;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;205;-2941.096,1189.462;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;211;-2660.728,1128.048;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;304;-2619.814,-3806.066;Float;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;228;-3135.394,728.7408;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;197;-4341.978,-281.2434;Float;False;WindIdleSway;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;299;-2551.462,-3584.584;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;308;-2544.819,-3455.95;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;210;-2719.327,1363.523;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.MatrixFromVectors;292;-1041.653,-2655.52;Float;False;FLOAT4x4;True;4;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;367;-2300.131,-598.0877;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;198;-3268.898,632.6165;Float;False;197;WindIdleSway;0;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;310;-2546.153,-3311.216;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;186;-3219.063,491.7997;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;423;-779.3868,-3127.796;Float;False;1;0;FLOAT4x4;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SaturateNode;194;-2896.383,735.2308;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;222;-2078.04,417.6245;Float;False;1733.328;456.6582;Final wind sway vertex offset calculation;11;183;348;215;232;217;253;219;251;221;252;378;Final Wind Sway;0.1647059,0.5803922,0.1098039,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;305;-2242.24,-3694.543;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;229;-2492.24,1183.714;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;187;-2897.352,561.38;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SqrtOpNode;368;-2162.728,-604.8068;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;311;-2254.627,-3342.732;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;309;-2252.042,-3487.556;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;230;-2695.262,543.3464;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;113;-4740.985,-182.2288;Float;False;733.7993;257;Wind waves on/of switch;4;111;109;112;110;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;220;-2299.941,1177.225;Float;False;WindWaveSwayCalculated;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;303;-1995.092,-3530.697;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;221;-2050.811,668.71;Float;False;220;WindWaveSwayCalculated;0;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;424;-1650.563,-3231.397;Float;False;1;0;FLOAT4x4;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4x4;0
Node;AmplifyShaderEditor.RangedFloatNode;252;-1936.819,758.6528;Float;False;Constant;_Float3;Float 3;18;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;57;-4733.84,-1788.214;Float;False;Property;_HealthyColor;Healthy Color;4;0;Create;True;0;0;False;0;0,1,0.2137935,0;0,1,0.2137935,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;142;-2520.286,-1743.023;Float;False;829.4101;337.406;Comment;5;117;120;59;60;141;Healthy/Dry Color Tint;1,0,0,1;0;0
Node;AmplifyShaderEditor.SaturateNode;353;-2042.691,-606.5488;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;139;-3795.29,-1743.729;Float;False;1195.445;658.8863;Wind wave tint calculation;9;471;137;82;467;68;469;75;475;472;Wind Waves Color Tint;1,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;132;-4726.808,386.4854;Float;False;Property;_WindWaveTint;Wind Wave Tint;15;0;Create;True;0;0;False;0;0.5;2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;119;-4729.389,-1604.005;Float;False;Property;_DryColor;Dry Color;3;0;Create;True;0;0;False;0;1,0,0,0;1,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;219;-2038.496,487.0963;Float;False;218;WindIdleSwayCalculated;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-4692.985,-134.2289;Half;False;Constant;_Float4;Float 4;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;141;-2470.285,-1509.009;Float;False;140;HealthyDryNoise;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;116;-4410.214,-1787.986;Float;False;HealthyColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;218;-2520.634,533.9471;Float;False;WindIdleSwayCalculated;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-4692.985,-54.229;Float;False;Constant;_Float5;Float 5;11;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;-1744.372,708.584;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;312;-1778.719,-3487.959;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4x4;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;-3735.535,-1434.635;Float;False;126;WindWaveNoise;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;117;-2450.93,-1681.415;Float;False;116;HealthyColor;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;120;-2450.097,-1592.172;Float;False;118;DryColor;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;475;-3647.967,-1245.409;Float;False;Constant;_Float6;Float 6;17;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;472;-3740.936,-1342.278;Float;False;135;WindWaveTintPower;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;118;-4410.13,-1603.9;Float;False;DryColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;140;-1893.319,-610.5497;Float;False;HealthyDryNoise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;135;-4344.342,385.4854;Float;False;WindWaveTintPower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;217;-1602.061,785.7782;Float;False;126;WindWaveNoise;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;253;-1605.896,634.92;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;109;-4516.985,-134.2289;Float;False;Property;_WindWavesOn;Wind Waves On;14;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;322;-1585.408,-3485.572;Float;False;FLOAT3;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;59;-2173.735,-1641.303;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;469;-3407.759,-1407.631;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;66;-4724.844,104.6633;Float;False;Property;_WindWaveTintColor;Wind Wave Tint Color;12;0;Create;True;0;0;False;0;0.07586241,0,1,0;0.1586208,0,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;422;-1868.041,-3799.718;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;215;-1362.406,601.9896;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;315;-1415.394,-3422.75;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;143;-1577.114,-781.0618;Float;False;1175.44;314.7707;Simple Ambient Occlusion calculation from vertex position;7;151;150;148;147;146;144;164;Ambient Occlusion;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;112;-4260.985,-134.2289;Float;False;WindWavesOn;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;348;-1408.643,456.849;Float;False;112;WindWavesOn;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;68;-3692.525,-1669.134;Float;False;60;HealthyDryTint;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;155;-4731.271,-1309.276;Float;False;Property;_GradientPower;Gradient Power;7;0;Create;True;0;0;False;0;0.3;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;60;-1942.877,-1646.374;Float;False;HealthyDryTint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;471;-3240.305,-1423.247;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;467;-3693.434,-1573.069;Float;False;122;WindWaveTintColor;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;122;-4350.951,104.6633;Float;False;WindWaveTintColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;154;-2526.687,-1302.728;Float;False;977.8249;329.1819;Gradient color calculation from vertex position;6;160;157;162;159;158;156;Gradient Color Tint;1,0,0,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;323;-1215.322,-3558.619;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TFHCCompareGreater;378;-1144.652,486.5858;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;265;-4774.624,-918.828;Float;False;724;207.9999;Billboarding on/off switch;4;261;262;264;263;Billboard Switch;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;144;-1558.667,-720.8851;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;232;-904.5095,478.0109;Float;False;238.5411;234.6374;Ignore local rotation;1;233;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;319;-979.3839,-3598.204;Float;False;429.1592;208.9326;Nullify billboard rotation;1;327;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;161;-4411.19,-1309.714;Float;False;GradientPower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;174;-1482.725,-269.8707;Float;False;1619.745;594.8293;Final albedo input calculation;10;152;167;169;165;168;124;166;138;79;115;Final Albedo;1,0,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;-2480.034,-1091.541;Float;False;161;GradientPower;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;145;-4731.818,-1413.143;Float;False;Property;_AmbientOcclusion;Ambient Occlusion;6;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;82;-3053.61,-1617.269;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;156;-2510.799,-1239.076;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;164;-1553.687,-571.7743;Float;False;163;AmbientOcclusionPower;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;327;-864.2399,-3559.773;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldToObjectTransfNode;233;-871.5095,528.0109;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;337;-1947.016,999.465;Float;False;1208.167;517.2274;Final vertex position calculation including billboarding;8;336;508;329;507;260;328;321;184;Final Vertex Position;0.945098,1,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;263;-4724.624,-868.828;Half;False;Constant;_Float7;Float 7;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;163;-4410.496,-1413.83;Float;False;AmbientOcclusionPower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;146;-1353.961,-706.3902;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;157;-2214.558,-1087.742;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;158;-2311.094,-1192.581;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;-1436.339,-55.21833;Float;False;137;WindWaveTint;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;-1437.37,50.03017;Float;False;60;HealthyDryTint;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;264;-4724.624,-788.8282;Float;False;Constant;_Float8;Float 8;11;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;137;-2877.77,-1622.622;Float;False;WindWaveTint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;-1437.938,-171.5748;Float;False;112;WindWavesOn;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;183;-607.6154,520.9398;Float;False;WindVertexOffset;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;-1198.426,-706.3402;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;184;-1885.938,1239.871;Float;False;183;WindVertexOffset;0;1;FLOAT4;0
Node;AmplifyShaderEditor.PosVertexDataNode;328;-1909.833,1335.326;Float;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;321;-1899.559,1143.273;Float;False;320;BillboardedVertexPos;0;1;FLOAT4;0
Node;AmplifyShaderEditor.TFHCRemapNode;159;-1989.005,-1248.63;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;124;-889.1823,-172.5284;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;166;-976.9665,107.9831;Float;True;Property;_MainTex;MainTex;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;320;-441.2839,-3563.046;Float;False;BillboardedVertexPos;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ToggleSwitchNode;262;-4548.624,-868.828;Float;False;Property;_IsBillboard;IsBillboard;11;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;261;-4292.624,-868.828;Float;False;BillboardOn;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;329;-1573.375,1313.019;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;148;-1033.728,-706.1402;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;260;-1559.46,1160.964;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;160;-1758.388,-1251.918;Float;False;GradientColor;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;168;-593.7906,-162.1037;Float;False;160;GradientColor;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;507;-1841.535,1059.113;Float;False;261;BillboardOn;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;-625.121,-54.31267;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;150;-854.926,-664.4952;Float;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;508;-1287.955,1116.056;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-348.091,-77.4103;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;257;-1234.46,-1619.727;Float;True;Property;_NormalMap;Normal Map;9;0;Create;True;0;0;False;0;None;c6607b9d4c0768e40916d8784d721f09;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;176;-916.3837,-1257.178;Float;False;Constant;_VegetationNormal;Vegetation Normal;19;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;338;-933.9393,-1343.697;Float;False;336;FinalVertexPos;0;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;153;-953.7034,-1705.352;Float;False;151;AmbientOcclusion;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;175;-933.5609,-1806.337;Float;False;152;FinalAlbedo;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;167;-573.6011,199.7374;Float;False;AlphaCutoff;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-111.85,-83.31852;Float;False;FinalAlbedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-889.9078,-1441.975;Float;False;167;AlphaCutoff;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;336;-1000.557,1111.408;Float;False;FinalVertexPos;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;151;-650.64,-670.0631;Float;False;AmbientOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-616.888,-1637.674;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;GPUInstancer/Foliage;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;False;Custom;GPUIFoliage;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;4;Include;UnityCG.cginc;Include;Include/GPUInstancerInclude.cginc;Pragma;multi_compile_instancing;Pragma;instancing_options procedural:setupGPUI;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;128;0;17;0
WireConnection;29;0;28;0
WireConnection;476;0;402;0
WireConnection;431;0;31;0
WireConnection;381;0;431;0
WireConnection;381;1;379;0
WireConnection;8;0;3;0
WireConnection;406;0;476;0
WireConnection;278;0;271;0
WireConnection;279;0;278;4
WireConnection;279;1;278;5
WireConnection;279;2;278;6
WireConnection;407;0;381;0
WireConnection;403;0;406;0
WireConnection;403;1;404;0
WireConnection;403;2;8;0
WireConnection;36;0;403;0
WireConnection;36;2;31;0
WireConnection;36;1;407;0
WireConnection;283;0;279;0
WireConnection;2;1;36;0
WireConnection;296;0;283;0
WireConnection;301;0;300;0
WireConnection;51;0;50;0
WireConnection;454;0;2;2
WireConnection;281;0;278;8
WireConnection;281;1;278;9
WireConnection;281;2;278;10
WireConnection;314;0;301;3
WireConnection;314;1;301;7
WireConnection;314;2;301;11
WireConnection;53;0;51;0
WireConnection;53;1;52;0
WireConnection;351;0;64;0
WireConnection;416;0;296;0
WireConnection;287;0;278;0
WireConnection;287;1;278;1
WireConnection;287;2;278;2
WireConnection;126;0;454;0
WireConnection;284;0;281;0
WireConnection;421;0;416;0
WireConnection;288;0;287;0
WireConnection;419;0;296;0
WireConnection;63;0;53;0
WireConnection;63;1;352;0
WireConnection;501;0;314;0
WireConnection;501;1;500;0
WireConnection;420;0;419;0
WireConnection;420;1;421;0
WireConnection;420;2;419;2
WireConnection;286;0;284;0
WireConnection;286;1;285;0
WireConnection;503;0;502;0
WireConnection;503;1;501;0
WireConnection;199;0;195;0
WireConnection;199;1;204;0
WireConnection;61;1;63;0
WireConnection;295;0;288;0
WireConnection;499;0;295;0
WireConnection;181;0;2;0
WireConnection;417;0;416;0
WireConnection;417;2;296;0
WireConnection;417;3;420;0
WireConnection;364;1;61;0
WireConnection;364;0;65;0
WireConnection;206;0;199;0
WireConnection;504;0;503;0
WireConnection;297;0;286;0
WireConnection;307;0;301;2
WireConnection;307;1;301;6
WireConnection;307;2;301;10
WireConnection;302;0;301;0
WireConnection;302;1;301;4
WireConnection;302;2;301;8
WireConnection;185;0;182;0
WireConnection;306;0;301;1
WireConnection;306;1;301;5
WireConnection;306;2;301;9
WireConnection;191;0;190;0
WireConnection;498;1;417;0
WireConnection;498;0;492;0
WireConnection;425;0;297;0
WireConnection;365;0;364;0
WireConnection;496;1;499;0
WireConnection;496;0;504;0
WireConnection;205;0;206;0
WireConnection;205;2;206;1
WireConnection;211;0;192;0
WireConnection;211;1;460;0
WireConnection;211;2;205;0
WireConnection;197;0;196;0
WireConnection;299;0;302;0
WireConnection;308;0;306;0
WireConnection;210;0;208;2
WireConnection;292;0;496;0
WireConnection;292;1;498;0
WireConnection;292;2;425;0
WireConnection;367;0;365;0
WireConnection;367;1;365;1
WireConnection;367;2;365;2
WireConnection;310;0;307;0
WireConnection;186;0;185;0
WireConnection;186;2;185;1
WireConnection;423;0;292;0
WireConnection;194;0;228;2
WireConnection;305;0;304;1
WireConnection;305;1;299;0
WireConnection;229;1;211;0
WireConnection;229;2;210;0
WireConnection;187;0;186;0
WireConnection;187;1;198;0
WireConnection;368;0;367;0
WireConnection;311;0;304;3
WireConnection;311;1;310;0
WireConnection;309;0;304;2
WireConnection;309;1;308;0
WireConnection;230;1;187;0
WireConnection;230;2;194;0
WireConnection;220;0;229;0
WireConnection;303;0;305;0
WireConnection;303;1;309;0
WireConnection;303;2;311;0
WireConnection;303;3;304;4
WireConnection;424;0;423;0
WireConnection;353;0;368;0
WireConnection;116;0;57;0
WireConnection;218;0;230;0
WireConnection;251;0;221;0
WireConnection;251;1;252;0
WireConnection;312;0;303;0
WireConnection;312;1;424;0
WireConnection;118;0;119;0
WireConnection;140;0;353;0
WireConnection;135;0;132;0
WireConnection;253;0;219;0
WireConnection;253;1;251;0
WireConnection;109;0;110;0
WireConnection;109;1;111;0
WireConnection;322;0;312;0
WireConnection;59;0;117;0
WireConnection;59;1;120;0
WireConnection;59;2;141;0
WireConnection;469;0;75;0
WireConnection;469;1;472;0
WireConnection;469;2;475;0
WireConnection;422;0;304;4
WireConnection;215;0;219;0
WireConnection;215;1;253;0
WireConnection;215;2;217;0
WireConnection;315;0;322;0
WireConnection;315;1;314;0
WireConnection;112;0;109;0
WireConnection;60;0;59;0
WireConnection;471;0;469;0
WireConnection;122;0;66;0
WireConnection;323;0;315;0
WireConnection;323;3;422;0
WireConnection;378;0;348;0
WireConnection;378;2;215;0
WireConnection;378;3;219;0
WireConnection;161;0;155;0
WireConnection;82;0;68;0
WireConnection;82;1;467;0
WireConnection;82;2;471;0
WireConnection;327;0;323;0
WireConnection;233;0;378;0
WireConnection;163;0;145;0
WireConnection;146;0;144;2
WireConnection;157;0;162;0
WireConnection;158;0;156;2
WireConnection;137;0;82;0
WireConnection;183;0;233;0
WireConnection;147;0;146;0
WireConnection;147;1;164;0
WireConnection;159;0;158;0
WireConnection;159;3;157;0
WireConnection;124;0;115;0
WireConnection;124;2;138;0
WireConnection;124;3;79;0
WireConnection;320;0;327;0
WireConnection;262;0;263;0
WireConnection;262;1;264;0
WireConnection;261;0;262;0
WireConnection;329;0;184;0
WireConnection;329;1;328;0
WireConnection;148;0;147;0
WireConnection;260;0;321;0
WireConnection;260;1;184;0
WireConnection;160;0;159;0
WireConnection;165;0;124;0
WireConnection;165;1;166;0
WireConnection;150;1;148;0
WireConnection;150;2;164;0
WireConnection;508;0;507;0
WireConnection;508;2;260;0
WireConnection;508;3;329;0
WireConnection;169;0;168;0
WireConnection;169;1;165;0
WireConnection;167;0;166;4
WireConnection;152;0;169;0
WireConnection;336;0;508;0
WireConnection;151;0;150;0
WireConnection;0;0;175;0
WireConnection;0;1;257;0
WireConnection;0;5;153;0
WireConnection;0;10;173;0
WireConnection;0;11;338;0
WireConnection;0;12;176;0
ASEEND*/
//CHKSM=F0E22E11D4FB50BDE2ED8C40C6E61712B5E4B909