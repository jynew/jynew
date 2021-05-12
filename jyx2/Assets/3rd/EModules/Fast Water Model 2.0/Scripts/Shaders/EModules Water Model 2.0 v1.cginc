


//#include "EModules Water Model 2.0 uniform.cginc"

#if defined(USE_SHADOWS) && defined(SHADER_API_GLES)
#define HAS_USE_SHADOWS = 1
#endif

#if !defined(SKIP_FRESNEL_CALCULATION) 

#if !defined(HAS_REFRACTION_Z_BLEND_AND_RRFRESNEL) &&  !defined(RRFRESNEL)  &&  !defined(RIM) &&  !defined(HAS_REFRACTION_Z_BLEND_AND_RRFRESNEL) && defined(REFLECTION_NONE)
#define SKIP_FRESNEL_CALCULATION = 1
#endif

#endif
		 
		//STRUCTS
		struct appdata {
			MYFLOAT4 vertex : POSITION;
//#if !defined(WAW3D_NORMAL_CALCULATION)
			MYFIXED3 normal : NORMAL;
//#endif
			MYFIXED4 color : COLOR;
			MYFIXED4 tangent : TANGENT;
			MYFLOAT4 texcoord : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};
		//* appdata *//

		struct v2f {
//#if defined(USE_WPOS)
			MYFLOAT4 wPos : TEXCOORD10;
//#endif
			MYFLOAT4 vfoam : TEXCOORD7; //z - sinuv ; y - cosuv ; z - stretched uv foam

			MYFLOAT4 pos : SV_POSITION;
			MYFLOAT4 uv : TEXCOORD0;
#if defined(REFRACTION_GRABPASS)
			MYFIXED4 grabPos : TEXCOORD1;
#endif
			//MYFIXED2 texcoord : TEXCOORD15;
			MYFLOAT4 uvscroll : TEXCOORD2;
			MYFIXED4 helpers : TEXCOORD9; //xy - inputnormals ; x - vertex alpha ; w - up sun factor

			MYFIXED3 tspace0 : TEXCOORD3; // tangent.x, bitangent.x, normal.x
			MYFIXED3 tspace1 : TEXCOORD4; // tangent.y, bitangent.y, normal.y
			MYFIXED3 tspace2 : TEXCOORD5; // tangent.z, bitangent.z, normal.z
#if !defined(SKIP_FLAT_SPECULAR) && !defined(SKIP_3DVERTEX_ANIMATION)
										//MYFIXED3 simplen : TEXCOORD11;
#endif

#if defined(USE_CAUSTIC)
			//MYFIXED4 screenCaust : TEXCOORD14;
#endif

			MYFIXED4 screen : TEXCOORD6;
			//MYFIXED3 wViewDir : TEXCOORD7;
/*#if !defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR) || !defined(SKIP_FLAT_SPECULAR)
			MYFIXED3 wLightDir : TEXCOORD8;
#endif*/

#if defined(HAS_ROTATION)
			MYFIXED4
#else
			MYFIXED2
#endif
			 detileuv : TEXCOORD8;
			//MYFIXED4 _utils : TEXCOORD9;

			//MYFIXED3 wLightPos : TEXCOORD9;

			//MYFIXED3 diff : COLOR0;
			MYFIXED4 VER_TEX : TEXCOORD12;


#if !defined(SKIP_FOG)
#ifdef ORTO_CAMERA_ON
			MYFIXED4 fogCoord : TEXCOORD11;
#else
			UNITY_FOG_COORDS(11)
#endif
#endif

#if defined(USE_VERTEX_OUTPUT_STEREO)
				UNITY_VERTEX_OUTPUT_STEREO
#endif


				//SHADOW
#if defined(USE_SHADOWS)
				SHADOW_COORDS(13)
#endif
				//MYFIXED3 ambient : COLOR0;


#if !defined(SKIP_BLEND_ANIMATION)
				MYFIXED4 blend_index : TEXCOORD13;
			MYFIXED4 blend_time : TEXCOORD14;
#endif

		};
		//* v2f *//
		//STRUCTS

		#include "EModules Water Model Utils.cginc"


		//( uvscrluv,   uvscrlsp)





		 
		v2f vert( appdata v)
		{

			const float4 v4_one_2 = float4(-1, 1, 1, -1);
			const float3 v3_one = float3(1, 1, 1);
			const float4 v4_one = float4(1, 1, 1, 1);


			v2f o;
#if defined(USE_GPU_INSTANCE_FEATURE)
			UNITY_SETUP_INSTANCE_ID(v);
#endif
			UNITY_INITIALIZE_OUTPUT(v2f, o);


#if defined(USE_VERTEX_OUTPUT_STEREO)
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
#endif

			MYFIXED3 wpos = mul(unity_ObjectToWorld, v.vertex);

			//MYFIXED2 _VertexLength = _VertexSize.zw - _VertexSize.xy;
			
			o.uv.zw = 1 - (v.vertex.xz - _VertexSize.xy) / _VertexSize.zw;
			//o.texcoord.zw = wpos.xz;
			//o.texcoord = v.texcoord;

			  
			MYFIXED TX = -(wpos.x / 1000 * _MainTex_ST.x + _MainTex_ST.z);
			MYFIXED TY = -(wpos.z / 1000 * _MainTex_ST.y + _MainTex_ST.w);


#if defined(HAS_ROTATION)
			MYFIXED a_x = cos(_MainTexAngle);
			MYFIXED a_y = sin(_MainTexAngle);
			MYFIXED b_x = -a_y;
			MYFIXED b_y = a_x;

			o.detileuv.z = a_x;
			o.detileuv.w = a_y;

			MYFIXED2 rNX = MYFIXED2(a_x, a_y);
			MYFIXED2 rNY = MYFIXED2(b_x, b_y);
			MYFIXED2 texcoord = rNX * TX + rNY *TY;
#else
			MYFIXED2 texcoord = MYFIXED2( TX ,TY );
#endif
			
			//texcoord = o.texcoord;
#if defined(SIN_OFFSET)
			texcoord.y += sin(texcoord.x *_sinFriq) * _sinAmount / 10;
#endif

#if !defined(SKIP_BLEND_ANIMATION) 
			BLEND_ANIMATION(o);
#endif

//#if defined(WAW3D_NORMAL_CALCULATION)  
			MYFIXED3 n = v.normal;
//#endif


			//o.VER_TEX = v.vertex;
			//VERTEX
#if !defined(SKIP_3DVERTEX_ANIMATION)
			VERTEX_MOVER( v,  o, texcoord,  n);
			o.pos = UnityObjectToClipPos(v.vertex);
#else 
			o.pos = UnityObjectToClipPos(v.vertex);

#endif


		//	o.screen = ComputeScreenPos(o.pos);

			o.screen = ComputeScreenPos(o.pos);

#if !defined(SKIP_3DVERTEX_ANIMATION)
			wpos = mul(unity_ObjectToWorld, v.vertex);
			 o.uv.zw = 1 - (v.vertex.xz - _VertexSize.xy) / _VertexSize.zw;
#endif

//#if defined(USE_WPOS) 
			//float4 clip = float4(o.pos.xy, 0.0, 1.0);
			//o.worldDirection = 
			o.wPos.xyz = wpos;
			/*#if defined(HAS_CAMERA_DEPTH)
						o.wPos = wpos;
			#elif defined(HAS_BAKED_DEPTH)
						o.wPos.xy = texcoord.xy;
			#endif*/
			//o.wPos = mul(_ClipToWorld, clip) - _WorldSpaceCameraPos;

//#endif
			
			o.wPos.w = distance(_WorldSpaceCameraPos.xyz, o.wPos.xyz);



#if defined(USE_VERTEX_OUTPUT_STEREO)
			COMPUTE_EYEDEPTH(o.screen.z);
#endif

			//VERTEX





			/*float3 posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;
			o.pos = mul(UNITY_MATRIX_VP, float4(posWorld, 1.0));*/

		
		
			o.uv.xy = texcoord.xy * _WaterTextureTiling.xy;
			/*o.uv.x += (sin(o.uv.x * 5 + o.uv.y*5)) / _WaterTextureTiling.x / 2;
			o.uv.y += (sin(o.uv.y * 5)) / _WaterTextureTiling.y / 2 ;*/

#if defined(USE_LIGHTMAPS)
			//o._utils.zw = v.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			//o.uv1 = IN.uv1.xy * unity_Lightmap_ST.xy + unity_Lightmap_ST.zw;
#endif

#if defined(REFRACTION_GRABPASS)
			o.grabPos = ComputeGrabScreenPos(o.pos);
#endif


			TANGENT_SPACE_ROTATION; // Unity macro that creates a rotation matrix called "rotation" that transforms vectors from object to tangent space.
			o.VER_TEX.xyz = normalize(mul(rotation, ObjSpaceViewDir(v.vertex))); // Get tangent space view dir from object space view dir.
			o.helpers.z = max(0,v.color.a * 10 - 9);

			//o.wViewDir = normalize(UnityWorldSpaceViewDir(wpos));
			//o.wLightPos = normalize(_LightPos - wpos);
			//o.wLightDir = UnityObjectToWorldDir(_LightDir.xyz);
/*#if !defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR) || !defined(SKIP_FLAT_SPECULAR)
			o.wLightDir = _LightDir;
#endif*/

			MYFIXED3 wNormal = UnityObjectToWorldNormal(n);
/*#if defined(WAW3D_NORMAL_CALCULATION)
#else
			MYFIXED3 wNormal = MYFIXED3(0, 1, 0);
#endif*/

			MYFIXED3 wTangent = v.tangent.xyz;
#if defined(HAS_ROTATION)
			//tng.xy = MYFIXED2(a_x, a_y) * tng.x + MYFIXED2(-b_x, b_y)*tng.y;
#endif
			//MYFIXED3 wTangent = UnityObjectToWorldDir(tng); 

			MYFIXED tangentSign = v.tangent.w * unity_WorldTransformParams.w;
			MYFIXED3 wBitangent = cross(wNormal, wTangent) * tangentSign;

			

			o.tspace0 = MYFIXED3(wTangent.x, wBitangent.x, wNormal.x);
			o.tspace1 = MYFIXED3(wTangent.y, wBitangent.y, wNormal.y);
			o.tspace2 = MYFIXED3(wTangent.z, wBitangent.z, wNormal.z);

			//MYFIXED2 uvsp = MYFIXED2(_WaterTextureTiling.z, _WaterTextureTiling.w) / 2;

			//1 source
			//o.uvscroll = UVSCROLL;
			//
#if defined(WAVES_MAP_CROSS)
			o.uvscroll = MYFIXED4(
		 (_Time.x / 2 + _CosTime.x * 0.02)	* _WaterTextureTiling.z,
		 (_Time.x / 2 + _SinTime.x * 0.03)	* _WaterTextureTiling.z,
		 (_Time.x / 2 + _SinTime.x * 0.05)	* _WaterTextureTiling.z,
		 (_Time.x / 2 + _CosTime.x * 0.04)* _WaterTextureTiling.z
	);
#else
			o.uvscroll = MYFIXED4( \
		 (_Time.x / 2 + _CosTime.x * 0.04)	* _WaterTextureTiling.z,
		 (_Time.x / 2 + _SinTime.x * 0.05)	* _WaterTextureTiling.z,
		 (_Time.x / 2 + _CosTime.x * 0.02)	* _WaterTextureTiling.z,
		 (_Time.x / 2 + _SinTime.x * 0.03 + 0.5) * _WaterTextureTiling.z
	);
#endif
			/*o.uvscroll = MYFIXED4(
				o.uv.x + (_Time.x / 4 + _CosTime.x * 0.02) * _WaterTextureTiling.z ,
				o.uv.y + (_Time.x / 4 + _SinTime.x * 0.03) * _WaterTextureTiling.z,
#if defined(WAVES_MAP_CROSS)
				o.uv.y - (_Time.x / 4 + _SinTime.y * 0.05)* _WaterTextureTiling.z				,
				o.uv.x - (_Time.x / 4 + _CosTime.y * 0.04) * _WaterTextureTiling.z
#else
				o.uv.x + (_Time.x / 8 + _CosTime.y * 0.04) * _WaterTextureTiling.z,
				o.uv.y - (_Time.x / 8 + _SinTime.y * 0.05 + 0.5)* _WaterTextureTiling.z
#endif
			);*/


#if defined(DETILE_LQ)
			MYFIXED DX = (sin((o.uv.x + o.uv.y)* _DetileFriq)) * _DetileAmount;
			MYFIXED DY = (sin(o.uv.y * _DetileFriq)) * _DetileAmount;
			/*o.uvscroll.x += DX;
			o.uvscroll.y += DY;
			o.uvscroll.z += DX;
			o.uvscroll.w += DY;*/
			o.detileuv.x = DX;
			o.detileuv.y = DY;
#endif

#if !defined(SKIP_FOAM)
			
#endif




#if !defined(SKIP_FOG)
#ifdef ORTO_CAMERA_ON
			/*MYFIXED3 eyePos = UnityObjectToViewPos(v.vertex);
			MYFIXED fogCoord = length(eyePos.xyz);
			UNITY_CALC_FOG_FACTOR_RAW(fogCoord);
			o.fogCoord = saturate(unityFogFactor);*/
			o.fogCoord = v.vertex;
#else
			UNITY_TRANSFER_FOG(o, o.pos);
#endif
#endif

			//o.ambient = ShadeSH9(MYFIXED4(wNormal, 1));
			//SHADOW
#if defined(USE_SHADOWS)
			TRANSFER_SHADOW(o)
#endif

				return o;
		}//!vert






#if defined(HAS_ROTATION)
#define ROT_NRM(nrm) 
//nrm.xy = MYFIXED2( i.detileuv.z ,  i.detileuv.w ) * nrm.x  + MYFIXED2( i.detileuv.w,  -i.detileuv.z )*nrm.y; 
#else
#define	ROT_NRM(nrm)
#endif



#if defined(MULTI_OCTAVES)

		/*MYFIXED3 GET_GRAD(MYFIXED a) {
			MYFIXED2 LS = uvsp / (a / 5 + 1);
			MYFIXED4 uv = UVSCROLL(o.uv, LS);
			return (tex2Dgrad(_BumpMap, uv.xy * (a + 1), 0, 0).rgb + tex2Dgrad(_BumpMap, uv.zw* (a + 1), 0, 0).rgb) * 0.5;
		}*/

		/* uvsp *= _MultyOctavesSpeedOffset; \
innerUV *= _MultyOctavesTileOffset;\
uv = UVSCROLL(innerUV, uvsp); \*/

		void MultyOctaves(in MYFIXED2 IN_UVS, in MYFIXED4 IN_UVS_DIR, in MYFIXED2 IN_SPD, inout MYFIXED3 temp_tile) {
			MYFIXED2 uvsp = MYFIXED2(_WaterTextureTiling.z, _WaterTextureTiling.w) ;
			MYFIXED2 innerUV = IN_UVS;
			MYFIXED2 tileFactor = MYFIXED2(1,1);
			MYFIXED2 rUv;



			//res = (UnpackNormal(nms1) + UnpackNormal(nms2))*0.5;
#if defined(MULTI_OCTAVES_ROTATE)
			MYFIXED MOR = _MOR_Base;
#endif
			MYFIXED3 r;
			MYFIXED3 advance = temp_tile.rgb;
			MYFIXED rightFade = 1 - _FadingFactor;
			//GET_GRAD(0, advance);
			//MYFIXED R = _MultyOctaveNormals;
			for (MYFIXED a = 1; a < _MultyOctaveNormals; a++ ) {


				uvsp *= _MultyOctavesSpeedOffset; 
				tileFactor *= _MultyOctavesTileOffset; 
				UVS2X(IN_UVS, IN_UVS_DIR, uvsp, rUv);
				rUv *= tileFactor; 
				/*MYFIXED4 nms = (tex2Dgrad(_BumpMap, uv.xy * (a + 1),0,0) + tex2Dgrad(_BumpMap, uv.zw* (a + 1),0,0)) * 0.5;*/\
				/*MYFIXED4 nms = (tex2D(_BumpMap, uv.xy * (a + 1)) + tex2D(_BumpMap, uv.zw* (a + 1))) * 0.5;*/\
				MYFIXED4 nms1 = TEX2DGRAD(_BumpMap, rUv);

#if !defined(SKIP_MO_TOWARD)
				//a += 1;
				uvsp *= _MultyOctavesSpeedOffset; 
				tileFactor *= _MultyOctavesTileOffset;
#endif
				UVS2Z(IN_UVS, IN_UVS_DIR, uvsp, rUv);
				rUv *= tileFactor; 
				MYFIXED4 nms2 = TEX2DGRAD(_BumpMap, rUv.xy );

/*#if defined(WAVES_MAP_BLENDMAX)
				r = lerp(UnpackNormal(nms1), UnpackNormal(nms2), nms2.z);
#else
				r = UnpackNormal((nms1+ nms2)/2);
#endif*/
				MYFIXED3 NN1 = UnpackNormal(nms1);
				MYFIXED3 NN2 = UnpackNormal(nms2);

#if defined(MULTI_OCTAVES_ROTATE)
#if defined(MULTI_OCTAVES_ROTATE_TILE)
				MYFIXED2 RX = MYFIXED2(sin((_Time.z * 2 + rUv.x * _MOR_Tile)* MOR/*+ _MOR_Offset * a*3*/), cos((_Time.z * 2 + rUv.y * _MOR_Tile) * MOR/* + _MOR_Offset * a*3*/));
#else
				MYFIXED2 RX = MYFIXED2(sin((_Time.z * 2)* MOR/*+ _MOR_Offset * a*3*/), cos((_Time.z * 2) * MOR/* + _MOR_Offset * a*3*/));
#endif
				MOR *= _MOR_Offset;
				MYFIXED2 RY = MYFIXED2(-RX.y, RX.x);
				NN1.xy = RX * NN1.x + RY * NN1.y;
				NN2.xy = RX * NN2.x + RY * NN2.y;
#endif

#if defined(WAVES_MAP_BLENDLERP)
				r = lerp(NN1, NN2, nms2.z);
#else
				r = NN1 + NN2;
#endif



			
				/*MYFIXED3 NN1 = UnpackNormal(nms1);
				MYFIXED3 NN2= NN1;
				NN2.x = -NN1.y;
				NN2.y = NN1.x;
				r = lerp(NN2, NN1, sin(_Time.z * 2 * uvsp.x)*0.5+0.5);*/

				//r = normalize(UnpackNormal(nms1)* UnpackNormal(nms2));

				/*if (nms1.z < nms2.z) r = nms1;
				else r = nms2;*/


				//MYFIXED4(_Time.x / 4 + _CosTime.x * 0.02) * _WaterTextureTiling.z
				//advance = (advance + (tex2D(_BumpMap, UVSCROLL.xy * (a + 1)) + tex2D(_BumpMap, UVSCROLL.zw* (a + 1))) * 0.5) / 2;
				/*MYFIXED4 uv = UVSCROLL(o.uv, _WaterTextureTiling.z / (a/5 + 1));
				MYFIXED advance = (tex2Dgrad(_BumpMap, uv.xy * (a + 1), 0, 0) + tex2Dgrad(_BumpMap, uv.zw* (a + 1), 0, 0)) * 0.5;*/
				//GET_GRAD( a, r );
				//advance += r;
				//advance = (advance * _FadingFactor + r * rightFade);
#if defined(WAVES_MAP_BLENDLERP)
				advance = lerp(advance, r, (r.z) * rightFade);
#else
				advance = lerp(advance * 2, r * 2, rightFade);
#endif
				//advance = (advance* r) *4;

				//if (advance.z > r.z) advance = r;
				//else r = nms2;
			}
#if defined(WAVES_MAP_BLENDLERP)
			temp_tile.rgb = advance.rgb;
#else
			temp_tile.rgb = advance.rgb / _MultyOctaveNormals;
#endif
			//temp_tile.rgb = (temp_tile.rgb *0.3 + advance * 0.7);
		}
#endif

		/* vec3 dist = p - ori;
    float d = length(dist);
    vec3 n = getNormal(p, d*d*.0003);// Or whatever value that suits.*/


		/*MYFIXED GER2(MYFIXED2 uv) {
			MYFIXED2 GS_TILE = MYFIXED2(5, 100);
			MYFIXED2 gs_offset = uv.xy;

			MYFIXED3 GS_DIR = MYFIXED3(0.9, 0.1, 3.5);
			GS_DIR.z * GS_TILE.x;
			MYFIXED2 GS_SPEED = MYFIXED2(0.01, 0.01);
			MYFIXED GS_AMOUNT = 1;
			MYFIXED DIV = 10 ; //+ _SinTime.w / 10

			MYFIXED octres;
			MYFIXED w;
			for (int i = 0; i < 10; i++) {
				 w += 0.07;
				MYFIXED gs_x = DIV - abs(gs_offset.x - GS_DIR.x * GS_DIR.z * GS_AMOUNT * sin((GS_DIR.z * gs_offset.x - GS_SPEED.x * _Time.x) * GS_TILE.x + w)) / DIV;
				MYFIXED gs_y1 = GS_AMOUNT * cos((GS_DIR.y * gs_offset.y - GS_SPEED.y * _Time.x) * GS_TILE.y + gs_x * GS_DIR.z - DIV) + 1;
				MYFIXED gs_y2 = GS_AMOUNT * cos((GS_DIR.y * gs_offset.y - GS_SPEED.y * _Time.x) * GS_TILE.y - gs_x * GS_DIR.z + DIV) + 1;
				MYFIXED gs_y = 1 - (gs_y1 * gs_y2);
				gs_offset *= w + 1;
				octres += gs_y;
				GS_SPEED *= w + 1;
				DIV *= w + 1 + 0.2;
			}
			octres /= 10;

			return octres;
		}*/
		 
#if defined(WAVES_GERSTNER)
#define TEX_SIZE 128
		MYFIXED CLASSIC_SAMPLE(MYFIXED3 p)
		{
			p.z = frac(p.z) * TEX_SIZE;
			MYFIXED p_int = floor(p.z);
			MYFIXED2 a_off = MYFIXED2(23.0, 29.0)*(p_int) / TEX_SIZE;
			MYFIXED2 b_off = MYFIXED2(23.0, 29.0)*(p_int + 1) / TEX_SIZE;


			MYFIXED uv01 = frac(p.xy + a_off);
			MYFIXED uv02 = frac(p.xy + b_off);
			MYFIXED a = tex2Dgrad(_NoiseHQ, uv01).r;
			MYFIXED b = tex2Dgrad(_NoiseHQ, uv02).r;
			return lerp(a, b, p.z - p_int) - 0.5;
		}
		
	

		MYFIXED3 CLASSIC_PASS(MYFIXED2 uv) {
//#define CP_POW(i)  pow(_CLASNOISE_PW, i)
#define CP_POW(i)  _CLASNOISE_PW
#define SN_UNWRAP(i) p * (2.0*i + 1.0)

			MYFIXED zadd = uv.x / 100.0;
			MYFIXED timeadd = _Time.x / 40.0;
			MYFIXED3 p = MYFIXED3(uv , 0);

			p.z = timeadd * _CN_SPEED  + zadd;
			MYFIXED R1 = CLASSIC_SAMPLE(SN_UNWRAP(0)) * CP_POW(0);
			p.z = timeadd * _CN_SPEED / 4 + zadd;
			MYFIXED R2 = CLASSIC_SAMPLE(SN_UNWRAP(2)) * CP_POW(1);
			//p.z = timeadd * _CN_SPEED  + zadd;
			//MYFIXED R3 = CLASSIC_SAMPLE(SN_UNWRAP(4)) * CP_POW(2);
			//p.z = timeadd * _CN_SPEED + zadd;
			//MYFIXED R4 = CLASSIC_SAMPLE(SN_UNWRAP(6)) * CP_POW(3);

			MYFIXED v = 0.0;
			v += R1 * R1;
			v += R2 * R2;
			//v += R3 * R3;
			/*v += pow(R1, _CLASNOISE_PW);
			v += pow(R2, _CLASNOISE_PW);
			v += pow(R3, _CLASNOISE_PW);*/
			//v += pow( CLASSIC_SAMPLE(SN_UNWRAP(3)) * SN_POW(3), POW);

			return v * _CN_AMOUNT * _CLASNOISE_PW *10;
		}

		MYFIXED3 CLASSIC(MYFIXED2 uv)
		{

			MYFIXED2 texel = _CN_TEXEL / _CN_TILING;
			uv *= _CN_TILING;

			MYFIXED C = CLASSIC_PASS(uv + MYFIXED2(-texel.x, -texel.y));
#if defined(_CN_DEBUG)
			return C.rrr;
#endif
			//MYFIXED H0 = CLASSIC_PASS(uv + MYFIXED2(0, -texel.y));
			//MYFIXED H1 = CLASSIC_PASS(uv + MYFIXED2(-texel.x, 0));
			MYFIXED H2 = CLASSIC_PASS(uv + MYFIXED2(texel.x, 0));
			MYFIXED H3 = CLASSIC_PASS(uv + MYFIXED2(0, texel.y));

			MYFIXED3 n;
			//n.x = H0 - H3;
			//n.y = H1 - H2;
			n.y = -C + H3;
			n.x = C - H2;
			//n /= 2;
			n.z = 2;

			//return MYFIXED3(0,0 , 1);
			return normalize(n);
		}

#endif

		// , UNITY_VPOS_TYPE screenPos : VPOS
		MYFIXED4 frag( v2f i) : SV_Target
		{
				MYFIXED3 tex;
		MYFIXED zdepth;
		MYFIXED raw_zdepth;
MYFIXED2 tnormal_GRAB;
MYFIXED2 tnormal_TEXEL;
MYFIXED3 tnormal;
MYFIXED3 worldNormal;
fixed rd;
MYFIXED3 wViewDir;
MYFIXED2 DFACTOR;
float2 DETILEUV = i.uv.xy;
float4 UVSCROLL = i.uvscroll;
MYFIXED4 color = fixed4(0, 0, 0, 1);
MYFIXED APS;
MYFIXED3 reflectionColor;
#if defined(HAS_REFLECTION)
#endif
#if defined(HAS_REFRACTION)
MYFIXED3 refractionColor;
MYFIXED lerped_refr;
#endif
MYFIXED3 unionFog;

	#include "EModules Water Model Sampling.cginc"
#if defined(WRONG_DEPTH)
			MYFIXED f = min(0.1, frac(i.screen.x / 10)) * 10;
		return MYFIXED4(f, f, f, 1);
#endif



		/*i.uv.x += (sin(i.uv.x * 5 + i.uv.y * 5)) / _WaterTextureTiling.x / 2;
		i.uv.y += (sin(i.uv.y * 5)) / _WaterTextureTiling.y / 2;*/

		MYFIXED4 UVS_DIR = i.uvscroll;
		MYFIXED2 UVS_SPD = MYFIXED2(_WaterTextureTiling.z, _WaterTextureTiling.w);
		UVS(i.uv, UVS_DIR, UVS_SPD, UVSCROLL)

#if defined(DETILE_LQ)
			MYFIXED DX = i.detileuv.x;
		MYFIXED DY = i.detileuv.y;
#elif defined(DETILE_HQ)
			MYFIXED DX = (sin((i.uv.x + i.uv.y)* _DetileFriq))  * _DetileAmount;
		MYFIXED DY = (sin(i.uv.y * _DetileFriq))  * _DetileAmount;
#else 
		MYFIXED DX = 0;
		MYFIXED DY = 0;
#endif

#if defined(DETILE_LQ) || defined(DETILE_HQ)
		UVSCROLL.x += DX;
		UVSCROLL.y += DY;
		UVSCROLL.z += DX;
		UVSCROLL.w += DY;
		DETILEUV.x += DX;
		DETILEUV.y += DY;
#endif

		

		


			//MYFIXED2 gs_offset = MYFIXED2(0, 0);
		



			//BUMP


#if defined(TILINGMASK)
		MYFIXED TILINGMASK_VALUE = min(_TILINGMASK_max, ((tex2Dgrad(_MainTex, DETILEUV* _TILINGMASK_Tile + _TILINGMASK_offset, 0, 0).r - 0.5) * _TILINGMASK_Amount + _TILINGMASK_min)) ; // / 1000

#if defined(TILE_MASK_DEBUG)
		return float4(TILINGMASK_VALUE.rrr, 1);
#endif
		//_tv += saturate( tex2D(_MainTex, DETILEUV* _TILINGMASK_Tile).r - 0.5) * 0.5;
		//DETILEUV += _tv;

#if defined(TILEMASKTYPE_OFFSET)|| defined(TILEMASKTYPE_OFFSETTILE)
		MYFIXED TMSOF = TILINGMASK_VALUE / 10;
		DX += TMSOF;
		DY += TMSOF;
		DETILEUV += TMSOF;
		UVSCROLL += TMSOF;
#endif 
#endif
		//MYFIXED4 mixed = lerp(MYFIXED4(0.5,0.5,1,1), (bump1 + bump2) * 0.5 , abs(bump1.g - bump2.g));
		// mixed = (bump1 + bump2) * 0.5;
		//mixed = MYFIXED4(0.5, 1, 0.5, 1);
		//MYFIXED4 mixed = lerp (bump1, bump2, (bump1.r - bump2.r)/2+0.5);
			 //mixed = (mixed + (bump1 + bump2) * 0.5) * 0.5;
		//mixed = 0.5 - abs((1 - mixed - 0.5)) + 0.5;
		//mixed = -abs(mixed - 0.5) + 0.5;
		//mixed.rb = 1;

//#if defined(WAVES_GERSTNER) 



		/*MYFIXED2 wgnuv = i.uv.xy / _WaterTextureTiling.xy;
		 tnormal = mister_gerstner(wgnuv);
#if defined(TILINGMASK)
#if defined(TILEMASKTYPE_TILE)|| defined(TILEMASKTYPE_OFFSETTILE)
		tnormal = lerp(tnormal, mister_gerstner(wgnuv * (5)), TILINGMASK_VALUE);
#endif
#endif*/
//#else///defined(WAVES_GERSTNER)

#if defined(SKIP_BLEND_ANIMATION)

		fixed3 tnormal1 = UnpackNormal(tex2D(_BumpMap, UVSCROLL.xy));
		fixed3 tnormal2 = UnpackNormal(tex2D(_BumpMap, UVSCROLL.zw));

#else
			float2 t1uv = UVSCROLL.xy + float2(i.blend_index.z, i.blend_index.z / 2);
		fixed4 t1tex = tex2D(_BumpMap, t1uv);
		fixed3 t1 = UnpackNormal(t1tex);

		float2 t2uv = UVSCROLL.zw + float2(i.blend_index.w, i.blend_index.w / 2);
		fixed4 t2tex = tex2D(_BumpMap, t2uv);
		fixed3 t2 = UnpackNormal(t2tex);

		float2 t3uv = UVSCROLL.xy + float2(i.blend_index.x, i.blend_index.x / 2);
		fixed4 t3tex = tex2D(_BumpMap, t3uv);
		fixed3 t3 = UnpackNormal(t3tex);

		float2 t4uv = UVSCROLL.zw + float2(i.blend_index.y, i.blend_index.y / 2);
		fixed4 t4tex = tex2D(_BumpMap, t4uv);
		fixed3 t4 = UnpackNormal(t4tex);
#if defined(USE_4X_BUMP)
		fixed4 t1texA = tex2D(_BumpMap, float2( t1uv.y , t1uv.x ) );
		fixed3 t1A = UnpackNormal(t1texA);
		fixed4 t2texA = tex2D(_BumpMap, float2(t2uv.y, t2uv.x) );
		fixed3 t2A = UnpackNormal(t2texA);					   
		fixed4 t3texA = tex2D(_BumpMap, float2(t3uv.y, t3uv.x) );
		fixed3 t3A = UnpackNormal(t3texA);					   
		fixed4 t4texA = tex2D(_BumpMap, float2(t4uv.y, t4uv.x) );
		fixed3 t4A = UnpackNormal(t4texA);
		//t1 = (t1 + t1A)* 0.5;
		t1 = lerp(t1, t1A, 0.5);
		//t2 = (t2 + t2A)* 0.5;
		t2 = lerp(t2, t2A, 0.5);
		//t3 = (t3 + t3A)* 0.5;
		t3 = lerp(t3, t3A, 0.5);
		//t4 = (t4 + t4A)* 0.5;
		t4 = lerp(t4, t4A, 0.5);
#endif

		t1 *= (i.blend_time.x);
		t2 *= (i.blend_time.y);
		t3 *= (i.blend_time.z);
		t4 *= (i.blend_time.w);

		fixed3 tnormal1 = (t1 + t3);
		fixed3 tnormal2 = (t2 + t4);

		//MYFIXED4 mixed = (t2 + t4 + t1 + t3)*0.5;
#endif



		fixed3 avt2 = lerp(tnormal1, tnormal2, 0.5);

#if defined(ALTERNATIVE_NORMALS_MIX)
		fixed sum = (tnormal1.z + tnormal2.z);
		fixed av15 = 1 - ((sum)) / _BumpMixAmount; //ues blend value and pop //used saturate
		fixed3 avt1 = fixed3(0, 0, 1);
		tnormal = lerp(avt1, avt2, av15);
		//tnormal = (tnormal* 0.3 + avt2 * 0.7);
		//tnormal = (tnormal + avt2 )* 0.5;
		tnormal = lerp(tnormal, avt2, 0.5);
		//tnormal = (tnormal* 0.7 + avt2*0.3) ;
#else
		//fixed av15 = ((sum - 1.95)) / 0.05; //ues blend value and pop
		tnormal = avt2;
#endif


		MYFIXED3 huge_NRM = tnormal;
			

#if defined(MULTI_OCTAVES)
		MultyOctaves(DETILEUV, UVS_DIR,UVSCROLL, tnormal);
		/*MYFIXED4 advance;
		for (MYFIXED a = 0; a < 5; a++) {
			advance = (advance + (tex2D(_BumpMap, UVSCROLL.xy * (a  + 1)) + tex2D(_BumpMap, UVSCROLL.zw* (a  + 1))) * 0.5) / 2;
		}
		mixed = (mixed *0.3 + advance *0.7);*/
#endif

#if defined(TILINGMASK)
#if defined(TILEMASKTYPE_TILE)|| defined(TILEMASKTYPE_OFFSETTILE)
		fixed4 bump1 = (tex2D(_BumpMap, UVSCROLL.xy * (_TILINGMASK_factor)));
		fixed4 bump2 = (tex2D(_BumpMap, UVSCROLL.zw * (_TILINGMASK_factor)));
/*#if defined(WAVES_MAP_BLENDMAX)
		MYFIXED4 temp_tile = max(bump1, bump2);
#else
		MYFIXED4 temp_tile = (bump1 + bump2) * 0.5;
#endif*/
		MYFIXED4 temp_tile = (bump1 + bump2) * 0.5;
		MYFIXED3 temp_normal = UnpackNormal(temp_tile);
#if defined(MULTI_OCTAVES)
		//MultyOctaves(DETILEUV * (5), UVS_DIR, UVSCROLL, tnormal);
		//MultyOctaves(i,UVSCROLL * (5), temp_normal);
#endif

		tnormal = lerp(tnormal, temp_normal, TILINGMASK_VALUE);
#endif
#endif


//#endif///defined(WAVES_GERSTNER)

		

#if defined(WAVES_MAP_BLENDMAX)
		//tnormal.xy *= abs(bump1.g - bump2.g) * 40;
		//tnormal.xy *= 3;
		//tnormal.z = abs(tnormal.z);
		//tnormal = normalize(tnormal);
#endif

		MYFIXED3 rawtnormal = tnormal;

		tnormal.xy *= _BumpAmount;

#if defined(AMOUNTMASK)
		MYFIXED amount_mask = min(_AMOUNTMASK_max, (tex2Dgrad(_MainTex, DETILEUV * _AMOUNTMASK_Tile + _AMOUNTMASK_offset,0,0).r * _AMOUNTMASK_Amount + _AMOUNTMASK_min));
#if defined(AMOUNT_MASK_DEBUG)
		return float4(amount_mask.rrr, 1);
#endif
		tnormal.xy *= amount_mask;
#endif


		// DISABLED
/*#if defined(WAVES_GERSTNER)
		MYFIXED check = distance(_WorldSpaceCameraPos, i.wPos) / _CN_DISTANCE;

#if defined(_CN_DEBUG)
		if (check > 1) return float4(0, 0, 0, 1);
		else return float4(lerp(CLASSIC(DETILEUV).rrr, float3(0, 0, 0), saturate(check * 2 - 1))    ,1);
#endif

		//return check;
		tnormal = normalize(tnormal);
		if (check < 1) tnormal = ( lerp(normalize( CLASSIC(DETILEUV) + tnormal), (tnormal), saturate( check * 2 - 1 ) ));
#else 
		tnormal = normalize(tnormal);
#endif*/

/*#if defined(USE_SURFACE_GRADS)



		MYFIXED v = (tnormal.x + tnormal.y) / tnormal.z * _WaveGradTopOffset;

#if defined(DEBUG_TOP_GRAD)
		return float4(v.rrr, 1);
#endif

		MYFIXED I_WaveGradMidOffset = 1 - _WaveGradMidOffset;
		MYFIXED3 top_grad_color = lerp(_WaveGrad1*1.5, _WaveGrad0 * 2, saturate((v - _WaveGradMidOffset) / I_WaveGradMidOffset));
		MYFIXED3 bottom_grad_color = lerp(_WaveGrad2, top_grad_color, saturate(v / _WaveGradMidOffset));

		MYFIXED3 surface_grad = bottom_grad_color - 0.5;
		//return float4(surface_grad, 1);
#endif*/



		 wViewDir = (_WorldSpaceCameraPos.xyz - i.wPos.xyz) / i.wPos.w;



		ROT_NRM(tnormal)

			MYFIXED2 NRMXY = MYFIXED2(rawtnormal.x, rawtnormal.y);

		
		

#if !defined(HAS_Z_AFFECT_BUMP)
		worldNormal.x = dot(i.tspace0, tnormal);
		worldNormal.y = dot(i.tspace1, tnormal);
		worldNormal.z = dot(i.tspace2, tnormal);
#endif






/////////////////////////////
/////////////////////////////
/////////////////////////////
#if defined(DEPTH_NONE)
		 zdepth = 1;
#else


#if defined(HAS_CAMERA_DEPTH)
	#ifdef ORTO_CAMERA_ON
			MYFIXED4 UV = i.screen;
	#else
			MYFIXED4 UV = (i.screen);
	#endif
#else
		MYFIXED2 UV = i.uv.zw;
		

#endif


	#if defined(FOAM_FINE_REFRACTIOND_DOSTORT)
		//////////////////////////////////////)//////////////////////////////////////
		//////////////////////////////////////)//////////////////////////////////////
			#if defined(HAS_CAMERA_DEPTH)//////////////////////////////////////


#define HAS_BEFORE_REALTIME_DEPTH = 1
		MYFIXED before_zdepth_raw = GET_Z(i, UV);
			#else
#define HAS_BEFORE_BAKED_DEPTH = 1
		MYFIXED before_zdepth_raw = GET_BAKED_Z(UV);
#endif
		
		 raw_zdepth = before_zdepth_raw;

		
		//////////////////////////////////////////////////////
		//////////////////////////////////////)//////////////////////////////////////
		//////////////////////////////////////)//////////////////////////////////////
		MYFIXED before_zdepth = saturate(before_zdepth_raw / _FoamLength / _FixMulty);
	#endif
		
	#if defined(HAS_REFRACTION)
	
		#if !defined(FOAM_FINE_REFRACTIOND_DOSTORT) && !defined(SKIP_ZDEPTH_REFRACTION_DISTORTIONCORRECTION)
		//////////////////////////////////////)//////////////////////////////////////
		//////////////////////////////////////)//////////////////////////////////////
			#if defined(HAS_CAMERA_DEPTH)//////////////////////////////////////
			/*#ifdef ORTO_CAMERA_ON
					MYFIXED beforetcdprj = tex2Dgrad(_CameraDepthTexture, UV,0,0).r;
			#else
					MYFIXED beforetcdprj = tex2Dgrad(_CameraDepthTexture, UV).r;
			#endif*/
				#define HAS_BEFORE_REALTIME_DEPTH = 1
				MYFIXED before_zdepth_raw = GET_Z(i, UV); //enabled after comments over
			#else

				#define HAS_BEFORE_BAKED_DEPTH = 1
				MYFIXED before_zdepth_raw = GET_BAKED_Z(UV);
			//MYFIXED before_zdepth_raw = GET_BAKED_Z(UV);
			#endif//////////////////////////////////////////////////////
		//////////////////////////////////////)//////////////////////////////////////
		//////////////////////////////////////)//////////////////////////////////////
		#endif
	

#if !defined(DEPTH_NONE) && !defined(HAS_CAMERA_DEPTH)  && !defined(SKIP_REFRACTION_CALC_DEPTH_FACTOR) && !defined(SKIP_ADDITIONAL_DEEP_COLOR_CALC)
		//MYFIXED3 pp2 = (i.VER_TEX.xyz);
			MYFIXED3 pp2 = (i.VER_TEX.xyz);

			MYFIXED  DEEPzdepth = GET_BAKED_Z_WITHOUTWRITE(UV - pp2.xy / (pp2.z * 0.8 + 0.2) * (saturate(
#if defined(HAS_BEFORE_BAKED_DEPTH)
				before_zdepth_raw
#else
				GET_BAKED_Z(UV)
#endif
				/ _RefrDeepFactor)));
#define HAS_ADDITIONAL_DEEP_COLOR_CALC = 1;
#endif



			MYFIXED2 M = NRMXY * _RefrDistortionZ * 30;
		//MYFIXED M = (NRMXY * _RefrDistortion);
#if !defined(HAS_CAMERA_DEPTH)
		M /= 100;
#endif

		#ifdef ORTO_CAMERA_ON//////////////////////////////////////
				//M = (M *ORTO_PROJ_DIVIDER) ;
		#else//////////////////////////////////////////////////////
#if defined(HAS_CAMERA_DEPTH)
		//M = (M );
#endif
		#endif//////////////////////////////////////////////////////
	
		#if defined(FOAM_FINE_REFRACTIOND_DOSTORT)
				M *= before_zdepth;
		#endif
		UV.xy += M / 10;
	#endif

//NEW//--------------------------------------------------
	#if defined(USE_YADD)
			#if defined(FOAM_FINE_REFRACTIOND_DOSTORT)
					Y_ADD *= before_zdepth;
			#endif
			UV.y += Y_ADD;
	#endif

			//UV = saturate(UV);

//NEW//--------------------------------------------------

		//--------------------------------------------------
		//MAIN
		//-----------------------------------------------------------
#if defined(HAS_CAMERA_DEPTH)
		//	return float4(GET_Z(i, UV).rrr, 1);


		/*#ifdef ORTO_CAMERA_ON//////////////////////////////////////
			//MYFIXED tcdprj = tex2Dgrad(_CameraDepthTexture, UV, 0, 0).r;
			MYFIXED tcdprj = tex2Dgrad(_CameraDepthTexture, UV,0,0).r;
		#else//////////////////////////////////////////////////////
					//MYFIXED tcdprj = tex2Dproj(_CameraDepthTexture, UV).r;
					MYFIXED tcdprj = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UV);

					
		#endif*/
		 zdepth = GET_Z(i, ( UV));

		#if defined(HAS_REFRACTION) && !defined(SKIP_ZDEPTH_REFRACTION_DISTORTIONCORRECTION)
					//tcdprj = min(beforetcdprj, tcdprj).r;
				zdepth = max(before_zdepth_raw, zdepth);
		#endif
		//MYFIXED zdepth = GET_Z(i, tcdprj);
#else

#if defined(HAS_REFRACTION) && !defined(SKIP_ZDEPTH_REFRACTION_DISTORTIONCORRECTION)
			 zdepth = max(before_zdepth_raw, GET_BAKED_Z_WITHOUTWRITE(UV));
			//zdepth = before_zdepth_raw;
		#else
#if !defined(HAS_BEFORE_BAKED_DEPTH)
			 zdepth = GET_BAKED_Z(UV);
#else
			 zdepth = GET_BAKED_Z_WITHOUTWRITE(UV);
#endif
		#endif
	
#endif
		//MYFIXED f = GET_Z_ORTO(i);
		//MYFIXED f = zdepth;
		//return MYFIXED4(f, f, f, 1);
		// zdepth = i.pos.z;
		//	return float4(saturate((zdepth*2.5) / 20).xxx, 1);

		#if defined(FOAM_FINE_REFRACTIOND_DOSTORT)
				/*MYFIXED C = 0.1;
				MYFIXED dif = max(0, abs(zdepth - before_zdepth_raw) - C);
				zdepth -= dif;
				Y_ADD *= dif/C;*/
		#endif

/////////////////////////////
/////////////////////////////
/////////////////////////////
//#else
/////////////////////////////
/////////////////////////////
/////////////////////////////




/*MYFIXED M = (rawtnormal.y * _RefrDistortion) / 100;
#ifdef ORTO_CAMERA_ON//////////////////////////////////////
		M = saturate(M / 20) ;
#else//////////////////////////////////////////////////////
		M = (M );
#endif//////////////////////////////////////////////////////
		zUV += M / 10;


		 zdepth = GET_BAKED_Z(zUV);*/
		





/////////////////////////////
/////////////////////////////
/////////////////////////////
//#endif ////////// END ZDEPTH
/////////////////////////////
/////////////////////////////
/////////////////////////////

#endif

#if !defined(DEPTH_NONE)  
	#if defined(DEGUB_Z)
	return MYFIXED4(zdepth/10, zdepth / 10, zdepth / 10, 1);
	#endif
	

#if defined(HAS_Z_AFFECT_BUMP)
	tnormal.xy *= saturate((zdepth - _BumpZFade) / _BumpZOffset + _BumpZFade );
	//tnormal.xy *= saturate((zdepth+_BumpZFade) * _BumpZOffset);
	tnormal = normalize(tnormal);
	worldNormal.x = dot(i.tspace0, tnormal);
	worldNormal.y = dot(i.tspace1, tnormal);
	worldNormal.z = dot(i.tspace2, tnormal);
		 //NRMXY = MYFIXED2(rawtnormal.x , rawtnormal.y);

#endif
#endif

	
	
#if defined(SURFACE_WAVES)
//MYFIXED text = tex2D(_NoiseHQ, MYFIXED2(NOISEUV.x, NOISEUV.y + _Time.x / 4 * 1.4) * 2).r - 0.2;
//MYFIXED text = tex2D(_NoiseHQ, MYFIXED2(NOISEUV.x, NOISEUV.y + _Time.x * 0.35 * _PRCHQ_speed) * 2).r - 0.2;
	
	//MYFIXED2 unpacked_sef_uv = i.wPos.xz / 1000;
	MYFIXED2 swf_speed = (_SFW_Dir1.xz * _SFW_Speed.x + MYFIXED2(-_SFW_Dir1.z, _SFW_Dir1.x) * _SFW_Speed.y);
	MYFIXED2 unpacked_sef_uv = MYFIXED2(DETILEUV.x, DETILEUV.y ) - _Time.x * 0.05 * swf_speed;

		unpacked_sef_uv = _SFW_Dir.xz * unpacked_sef_uv.x + MYFIXED2(-_SFW_Dir.z, _SFW_Dir.x) * unpacked_sef_uv.y + ( NRMXY)/100 * _SFW_Distort;
	MYFIXED unpacked_swf_tex = tex2D(_MainTex, unpacked_sef_uv* _SFW_Tile.zw).r - 0.2;

	//MYFIXED surface_noise = GET_NOISE(i, 0.4 * _PRCHQ_tileWaves, -_PRCHQ_speedWaves, DETILEUV) * text * 4 * text * _PRCHQ_amount;
	//MYFIXED GET_NOISE(v2f i, MYFIXED _tile, MYFIXED speed, MYFIXED2 UV) {
	MYFIXED2 swf_speeded_uv = DETILEUV + +_Time.z * swf_speed;

	MYFIXED2 swf_uv1 = (_SFW_Dir1.xz * swf_speeded_uv.x + MYFIXED2(-_SFW_Dir1.z, _SFW_Dir1.x) * swf_speeded_uv.y) * 13 * _SFW_Tile.xy;
	swf_uv1.x += sin(swf_uv1.x);
	MYFIXED dir1 = swf_uv1.x * 3 + swf_uv1.y / 1.9;
	MYFIXED surface_noise1 = saturate((cos(dir1) / 2 + 0.2));

	MYFIXED2 swf_uv = (_SFW_Dir.xz * swf_speeded_uv.x + MYFIXED2(-_SFW_Dir.z, _SFW_Dir.x) * swf_speeded_uv.y) * 13 * _SFW_Tile.xy;
	MYFIXED dirS = swf_uv.x * 3 + swf_uv.y / 1.9 ;
	MYFIXED surface_noise = saturate( (sin(dirS ) / 2 +0.2));

	
	//return float4(surface_noise.rrr, 1);
	surface_noise *= surface_noise1 * unpacked_swf_tex;
	surface_noise *= surface_noise* surface_noise * _SFW_Amount *10;

#if defined(DEBUG_SURFACE_WAVES)
	return float4(surface_noise.rrr,1);
#endif


#if !defined(SKIP_SURFACE_WAVES_NORMALEFFECT)
	tnormal += MYFIXED3(-surface_noise.x, -surface_noise.x, 0.5) * _SFW_NrmAmount;
	tnormal = normalize(tnormal);
#endif
#endif



#ifdef ORTO_CAMERA_ON//////////////////////////////////////
		  MYFIXED fresnelFac = 0;
#else//////////////////////////////////////////////////////
#if defined(SKIP_FRESNEL_CALCULATION) 
		MYFIXED fresnelFac = 0;
#else
		 MYFIXED YLERP_target = (wViewDir.y+ worldNormal.y)/2;
		 MYFIXED YLERP1 = lerp(wViewDir.y, YLERP_target, _FresnelFade*2);
		 MYFIXED YLERP2 = lerp(worldNormal.y, YLERP_target, _FresnelFade*2);
		 MYFIXED UP_SUNF = ( dot(MYFIXED3(0, 1, 0), wViewDir)) ;
		 MYFIXED UP_SUNF_POW = (pow(UP_SUNF + 1, _FresnelPow ));
		// UP_SUNF = UP_SUNF * UP_SUNF;
		// return UP_SUNF;
		 MYFIXED temp_fresnelFac =  1- dot( normalize( MYFIXED3(wViewDir.x, YLERP1 / UP_SUNF_POW, wViewDir.z)), normalize( MYFIXED3(worldNormal.x,  YLERP2 / UP_SUNF_POW, worldNormal.z)));


		 //fix;
		 temp_fresnelFac =  temp_fresnelFac *(1 - UP_SUNF * 0.7) + UP_SUNF/2;
		 //return temp_fresnelFac;//

		 // MYFIXED temp_fresnelFac = (1 - dot(wViewDir, worldNormal));
/*#if defined(SKIP_FRESNEL_GRADIENT)
		 //MYFIXED temp_fresnelFac = temp_fresnelFac;
#else
		 temp_fresnelFac = tex2Dgrad(_Utility, MYFIXED2((temp_fresnelFac), 0), 0, 0).r;
#endif*/
		 temp_fresnelFac *= _FresnelAmount;

//#if defined(USE_FRESNEL_POW)
		 temp_fresnelFac = (pow(temp_fresnelFac, _FresnelPow * _FresnelFade) ) ;

		// return float4(temp_fresnelFac.rrr	,1);
		 //MYFIXED3 fresnelFac = _MainTexColor.xyz * (1 - temp_fresnelFac) + temp_fresnelFac;
		 MYFIXED  fresnelFac = temp_fresnelFac;
		 
#if defined(FRESNEL_INVERCE)
		    fresnelFac = 1 - saturate(fresnelFac);
#else
		     fresnelFac = saturate(fresnelFac);
#endif

#if defined(DEBUG_FRESNEL)
		  return float4(fresnelFac.rrr, 1);
#endif
		 //fresnelFac = (1 - dot(wViewDir, worldNormal) / _FresnelFade);
		 //fresnelFac = saturate(_FresnelAmount + (1.0 - _FresnelAmount) * pow(fresnelFac, _FresnelPow));
//#endif
#endif

#endif////ORTO_CAMERA_ON//////////////////////////////////////////////////



//return MYFIXED4(zdepth, zdepth, zdepth, 1);
	//MYFIXED NRMXY = (tnormal.x + tnormal.z);


#if defined(HAS_REFRACTION)
 lerped_refr = saturate((
#if defined(HAS_ADDITIONAL_DEEP_COLOR_CALC)
	(DEEPzdepth)
#else
	(zdepth)
#endif
	- _RefrZOffset  )/ 
	_RefrZFallOff
#ifdef ORTO_CAMERA_ON//////////////////////////////////////
	 * ( dot(wViewDir, MYFIXED3(0, 1, 0))*5 + 1 ) 
#endif

	+ _RefrZOffset);


#endif


		//
#if defined(HAS_REFRACTION)
		//	zdepth += worldNormal.z * _RefrDistortion;
#endif
		//return MYFIXED4(zdepth, zdepth, zdepth, 1);


		/*#if defined(USE_YADD)
		Y_ADD *= saturate(zdepth/ 50);
#endif*/

#if defined(HAS_REFRACTION)
		/*	Y_ADD *= saturate(zdepth );
		_RefrDistortion *= saturate(zdepth );*/
#endif
		//DEPTH


		//REFLECTION////
#if defined(REFLECTION_NONE)
		 reflectionColor = MYFIXED3(0, 0, 0);
#else

		 reflectionColor = MYFIXED3(0, 0, 0);
#if defined(REFLECTION_BLUR) || !defined(REFLECTION_2D) && !defined(REFLECTION_PLANAR)
		MYFIXED _REFL_BLUR = _ReflectionBlurRadius;

#if !defined(SKIP_REFLECTION_BLUR_ZDEPENDS) && (defined(HAS_CAMERA_DEPTH) || defined(HAS_BAKED_DEPTH))
		MYFIXED refllerped_refr =  saturate(
#if defined(HAS_ADDITIONAL_DEEP_COLOR_CALC)
			(DEEPzdepth
#else
			(zdepth
#endif
			/20));

		
		
		_REFL_BLUR *= saturate(refllerped_refr + _ReflectionBlurZOffset);



		//return  fresnelFac;
		//_REFL_BLUR *=  saturate(zdepth + _ReflectionBlurZOffset);
#endif

#if !defined(SKIP_FRES_BLUR) && !defined(SKIP_FRESNEL_CALCULATION) 
//_REFL_BLUR += saturate(0.7 - fresnelFac) * FRES_BLUR_AMOUNT;// pow((fresnelFac), FRES_BLUR_AMOUNT);
		_REFL_BLUR += saturate(fresnelFac - FRES_BLUR_OFF) * FRES_BLUR_AMOUNT;// pow((fresnelFac), FRES_BLUR_AMOUNT);
#endif

#endif
 
		MYFIXED BRD = baked_ReflectionTex_distortion * saturate(zdepth);




#if defined(REFLECTION_2D) || defined(REFLECTION_PLANAR)
		MYFIXED4 uv1;
		uv1.x = (tnormal.x) * 10 * BRD;
		uv1.y = -abs(tnormal.y) * 10 * BRD;
		uv1.zw = 0;
#ifdef ORTO_CAMERA_ON
		uv1.xy /= ORTO_PROJ_DIVIDER;
#endif
		uv1 += i.screen;

#ifdef ORTO_CAMERA_ON
#else
		uv1 = UNITY_PROJ_COORD(uv1);
#endif


#if defined(REFLECTION_2D)
#ifdef ORTO_CAMERA_ON
#define REFLTEX(uv) tex2Dgrad(_ReflectionTex, (uv),0,0).rgb
#else
#define REFLTEX(uv) tex2Dproj(_ReflectionTex, (uv)).rgb
#endif
		//sampler2D targetTexture = _ReflectionTex;
#else
		//return tex2Dproj(_ReflectionTex_temp, UNITY_PROJ_COORD(uv1));
#ifdef ORTO_CAMERA_ON
#define REFLTEX(uv) tex2Dgrad(_ReflectionTex_temp, (uv),0,0).rgb
#else
#define REFLTEX(uv) tex2Dproj(_ReflectionTex_temp, (uv)).rgb
#endif
		//sampler2D targetTexture = _ReflectionTex_temp;
#endif

#if  defined(REFLECTION_BLUR)
		//MYFIXED3 REFL_BLUR4_PROJ(MYFIXED4 refrUv, MYFIXED RefrBlur) {
#define REFL_BLUR4_PROJ(refrUv, ReflBlur)\
			REFL_BLUR_result = REFLTEX(refrUv + MYFIXED4(0, ReflBlur, 0, 0));\
			REFL_BLUR_result += REFLTEX(refrUv + MYFIXED4(ReflBlur, 0, 0, 0));\
			REFL_BLUR_result += REFLTEX(refrUv + MYFIXED4(0, -ReflBlur, 0, 0));\
			REFL_BLUR_result += REFLTEX(refrUv + MYFIXED4(-ReflBlur, 0, 0, 0));\
			REFL_BLUR_result /= 4
			//return REFL_BLUR_result;
		//}
		//MYFIXED3 REFL_BLUR9_PROJ(MYFIXED4 refrUv, MYFIXED ReflBlur) {
#define REFL_BLUR9_PROJ(refrUv, ReflBlur)\
			REFL_BLUR_result = REFLTEX(refrUv + MYFIXED4(0, ReflBlur, 0, 0));\
			REFL_BLUR_result += REFLTEX(refrUv + MYFIXED4(ReflBlur * 0.5, ReflBlur * 0.5, 0, 0));\
			REFL_BLUR_result += REFLTEX(refrUv + MYFIXED4(ReflBlur, 0, 0, 0));\
			REFL_BLUR_result += REFLTEX(refrUv + MYFIXED4(ReflBlur * 0.5, -ReflBlur * 0.5, 0, 0));\
			REFL_BLUR_result += REFLTEX(refrUv + MYFIXED4(0, -ReflBlur, 0, 0));\
			REFL_BLUR_result += REFLTEX(refrUv + MYFIXED4(-ReflBlur * 0.5, -ReflBlur * 0.5, 0, 0));\
			REFL_BLUR_result += REFLTEX(refrUv + MYFIXED4(-ReflBlur, 0, 0, 0));\
			REFL_BLUR_result += REFLTEX(refrUv + MYFIXED4(-ReflBlur * 0.5, -ReflBlur * 0.5, 0, 0));\
			REFL_BLUR_result /= 9
		//return result / 9;
		//}
#endif


		/*reflectionColor = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(uv1));
		#elif defined(REFLECTION_PLANAR)
		MYFIXED4 uv1 = i.screen; 
		uv1.xy += tnormal * 10 * baked_ReflectionTex_distortion;
		reflectionColor = tex2Dproj(_ReflectionTex_temp, UNITY_PROJ_COORD(uv1));*/

/*#define ASD  ZXC + 0.5
#define ZXC 0.5
ZXC 0.0*/


		reflectionColor = REFLTEX(uv1);
		//return float4(reflectionColor.rgb / 3, 1);
		//REFLTEX
		//return float4(reflectionColor, 1);
#if defined(REFLECTION_BLUR)
		//MYFIXED _REFL_BLUR = _ReflectionBlurRadius / 100;
		

#ifdef ORTO_CAMERA_ON
		_REFL_BLUR /= ORTO_PROJ_DIVIDER;
#endif


		MYFIXED3 REFL_BLUR_result;

#if defined(REFLECTION_BLUR_3)
		REFL_BLUR4_PROJ(uv1, _REFL_BLUR);
		MYFIXED3 bluredRefl = REFL_BLUR_result;
		_REFL_BLUR += _REFL_BLUR;
		REFL_BLUR9_PROJ(uv1, _REFL_BLUR );
		MYFIXED3 bluredRefl9 = REFL_BLUR_result;
		_REFL_BLUR += _REFL_BLUR;
		REFL_BLUR9_PROJ(uv1, _REFL_BLUR);
		MYFIXED3 bluredRefl92 = REFL_BLUR_result;
		reflectionColor = (reflectionColor* 0.1 + bluredRefl * 0.15 + bluredRefl9 * 0.25 + bluredRefl92 * 0.5);
#elif defined(REFLECTION_BLUR_2)
		REFL_BLUR4_PROJ(uv1, _REFL_BLUR);
		MYFIXED3 bluredRefl = REFL_BLUR_result;
		_REFL_BLUR += _REFL_BLUR;
		REFL_BLUR9_PROJ(uv1,  _REFL_BLUR);
		MYFIXED3 bluredRefl9 = REFL_BLUR_result;
		reflectionColor = (reflectionColor * 0.15 + bluredRefl * 0.25 + bluredRefl9 * 0.6);
#else
		REFL_BLUR4_PROJ(uv1, _REFL_BLUR);
		MYFIXED3 bluredRefl = REFL_BLUR_result;
		reflectionColor = bluredRefl;
#endif
#endif

#else
		////////////////////////////////////////worldNormal
		MYFIXED3 worldRefl = reflect(-wViewDir , MYFIXED3(worldNormal.x * BRD/10 , worldNormal.y, worldNormal.z * BRD / 10) * (1 + _ReflectionYOffset));

		//_REFL_BLUR = _ReflectionBlurRadius * saturate(zdepth/5);

#if  !defined(REFLECTION_BLUR)
		 _REFL_BLUR = 0;
#endif


#if defined(REFLECTION_USER)
		reflectionColor = UNITY_SAMPLE_TEXCUBE_LOD(_ReflectionUserCUBE, worldRefl, _REFL_BLUR);
#endif
#if defined(REFLECTION_PROBE)
		reflectionColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, worldRefl, _REFL_BLUR);
#endif
#if defined(REFLECTION_PROBE_AND_INTENSITY)
		reflectionColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, worldRefl, _REFL_BLUR) * unity_SpecCube0_HDR.x;
#endif
#ifdef SKIP_PLANNAR
		reflectionColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, worldRefl, _REFL_BLUR) * unity_SpecCube0_HDR.x;
#endif


#endif

//#if defined(RRFRESNEL) //defined(HAS_REFRACTION_Z_BLEND_AND_RRFRESNEL) ||
		reflectionColor *= (1 - fresnelFac) * _ReflectionAmount;
/*#if defined(HAS_REFRACTION_Z_BLEND) //defined(HAS_REFRACTION_Z_BLEND_AND_RRFRESNEL) ||
		reflectionColor *=  _ReflectionAmount;
#else
		reflectionColor *= (1 - fresnelFac) * _ReflectionAmount;
#endif*/

		
		//reflectionColor = saturate(reflectionColor * fresnelFac) * _ReflectionAmount;

#if defined(DESATURATE_REFL)
		reflectionColor = lerp(reflectionColor, (reflectionColor.b + reflectionColor.g) / 2, _ReflectionDesaturate);
#endif
		//reflectionColor = reflectionColor * fresnelFac + _ReflectionAmount;
#endif
		//REFLECTION////

		//return MYFIXED4(reflectionColor, 1);




		 


#if !defined(SKIP_FOAM)

#if defined(FOAM_FINE_REFRACTIOND_DOSTORT)
		MYFIXED lengthFactor = saturate((zdepth) / _FoamLength);
#else
		MYFIXED lengthFactor = saturate((zdepth) / _FoamLength);
#endif
		lengthFactor = saturate(lengthFactor + _FoamDistortionFade / 20);
		MYFIXED foamfade = saturate( abs(lengthFactor) * _WaterfrontFade) ;
		//foamfade = 1;
		MYFIXED invlengFactor = 1 - lengthFactor;

		//FOAM//
		
		MYFIXED2 foamDistortUV = NRMXY * _FoamDistortion * (1 - invlengFactor * _FoamDistortionFade);


		MYFIXED2 fUV;
		fUV = 0.01 * i.wPos.xz; //0.01 * wpos.xz + 
		fUV.x += _FoamOffsetSpeed * 0.05 * _SinTime.w;

#if defined(GRAD_B)
		MYFIXED foamGradient = 0.8 - tex2D(_Utility, MYFIXED2(invlengFactor - _Time.y * _FoamWavesSpeed + _FoamOffset, foamDistortUV.x * 2) + fUV * _FoamDirection).b;
#else
		MYFIXED foamGradient = 0.8 - tex2D(_Utility, MYFIXED2(invlengFactor - _Time.y * _FoamWavesSpeed + _FoamOffset, foamDistortUV.x * 2) + fUV * _FoamDirection).g;
#endif

#if defined(NEED_FOAM_UNPACK)
		//MYFIXED2 foamDistortUV = tex2D(_FoamTexture, _FoamTextureTiling * i.uv.zw /*+ foamDistortUV*/ ).r * _FoamDistortion;
		MYFIXED3 unpackedFoam = tex2D(_FoamTexture, _FoamTextureTiling * (fUV /*+ foamDistortUV*/ + foamDistortUV / 10 * _FoamDistortionTexture)).r * _FoamColor;
#else
		//	MYFIXED2 foamDistortUV = MYFIXED2(0, 0);
		MYFIXED3 unpackedFoam = _FoamColor;
#endif
		MYFIXED4 foamColor;
		foamColor.rgb = unpackedFoam;
		// return MYFIXED4(foamGradient, 1);

		MYFIXED foamL = invlengFactor * foamfade * _FoamAmount;
		foamColor.rgb = foamGradient * foamL * foamColor.rgb ;
		
#if defined(FOAM_COAST_ALPHA_V2)
		MYFIXED asd = (foamColor.r * foamColor.g *  invlengFactor * 2 - _FoamAlpha2Amount) * 2 * (1 - _FoamAlpha2Amount/2);
		asd = (asd * asd) + 0.1;
		foamColor.a = asd * foamfade + 1 - saturate(invlengFactor);
#elif !defined(SKIP_FOAM_COAST_ALPHA)
		//foamColor.a = foamColor.b *foamColor.r * foamColor.g * (1 - invlengFactor) + 1 - saturate(invlengFactor) + 0.2;
		foamColor.a = foamColor.r * foamColor.g * invlengFactor * 2 + 1 - saturate(invlengFactor);
		// 

#else
		foamColor.a = 1;
#endif


#else 
		MYFIXED4 foamColor = MYFIXED4(0, 0, 0, 1);
#endif
		//FOAM//
		// return foamColor;
		


		//return float4(lengthFactor.xxx,1);

#if defined(SHORE_WAVES) && !defined(DEPTH_NONE)

#if defined(SKIP_SECOND_DEPTH)
		MYFIXED shore_z = saturate(zdepth/10);
#else
		MYFIXED shore_z = GET_SHORE(i.uv.zw);
#define HAS_SHORE_Z = 1;
#if defined(DEGUB_Z_SHORE)
		return MYFIXED4(shore_z, shore_z, shore_z, 1);
#endif
#endif

		MYFIXED lengthFactor_SW = saturate((shore_z) / _FoamLength_SW);
		//lengthFactor_SW = saturate(lengthFactor_SW + _FoamDistortionFade_SW / 20);
		//MYFIXED foamfade_SW = min(1, abs(lengthFactor_SW) * _WaterfrontFade_SW);
		//MYFIXED foamfade_SW =1;
		MYFIXED invlengFactor_SW = 1 - lengthFactor_SW;

#if defined(INVERCE_FADE_DISTORTION)
		MYFIXED2 foamDistortUV_SW = NRMXY * _FoamDistortion_SW * (invlengFactor_SW * _FoamDistortionFade_SW);
#else
		MYFIXED2 foamDistortUV_SW = NRMXY * _FoamDistortion_SW * (1 - invlengFactor_SW * _FoamDistortionFade_SW);
#endif

		//MYFIXED2 nrm = tex2D(_FoamTexture_SW, _FoamTextureTiling_SW * 100).rg - 0.8;
		// foamDistortUV_SW = nrm * _FoamDistortion_SW * (1 - invlengFactor_SW * _FoamDistortionFade_SW);

		MYFIXED2 uv_SW = i.wPos.xz;
		MYFIXED2 shorewaves_tile = MYFIXED2(_FoamLShoreWavesTileX_SW, _FoamLShoreWavesTileY_SW) / 100;


		//WAVES TILITN ACCORDING GLOBLA TILE
		
		//
#if defined(NEED_SHORE_WAVES_UNPACK)
		MYFIXED3 unpackedFoam_SW = tex2D(_FoamTexture_SW, uv_SW * (_FoamTextureTiling_SW * 0.01 + foamDistortUV_SW / 90 * _ShoreDistortionTexture)).r * _FoamColor_SW;
#else
		MYFIXED3 unpackedFoam_SW = _FoamColor_SW;
#endif
		//unpackedFoam_SW = _FoamColor_SW;

		MYFIXED2 uv_SW_directed = uv_SW * _FoamDirection_SW / 20;
		//uv_SW *= shorewaves_tile;
		//MYFIXED2 invuvsw = MYFIXED2(uv_SW.y, -uv_SW.x);
		//return abs(_FoamDirection_SW);
		//uv_SW = lerp(-uv_SW, uv_SW, _FoamDirection_SW / 10) ;
		//uv_SW = lerp(0, invuvsw, saturate(_FoamDirection_SW)) ;
		//uv_SW /= 10;
		//uv_SW.y *= _FoamDirection_SW / 10;
		//uv_SW = normalize(uv_SW);

		MYFIXED shore_mask = tex2D(_Utility, MYFIXED2(invlengFactor_SW, _FoamMaskOffset_SW)  ).g;
		//MYFIXED shore_mask = 1;
		shore_mask = saturate(shore_mask - _WaterfrontFade_SW) / (1 - _WaterfrontFade_SW);
		//MYFIXED3 foamGradient_SW = tex2Dgrad(_ShoreWavesGrad, MYFIXED2(invlengFactor_SW - _Time.y * _FoamWavesSpeed_SW, foamDistortUV_SW.x * 2) + uv_SW).b;
		MYFIXED foamGradient_SW = tex2D(_ShoreWavesGrad, (MYFIXED2( invlengFactor_SW - _Time.x * _FoamWavesSpeed_SW, 0) + uv_SW * shorewaves_tile + uv_SW_directed + foamDistortUV_SW) ).b;

#if defined(USE_SHORE_POW_APPEAR)
		MYFIXED aa = saturate(pow(4 - shore_mask * 3, 2));
		foamGradient_SW = pow((foamGradient_SW), aa) * aa * aa* aa;
#endif

		foamGradient_SW *= shore_mask;
		//foamGradient_SW = shore_mask;

		MYFIXED foamL_UW = invlengFactor_SW * _FoamAmount_SW;
		//return foamL_UW + 0.5;
		foamColor.rgb += unpackedFoam_SW * foamGradient_SW * foamL_UW;
#endif



		// MYFIXED3 lT = MYFIXED3(i.tspace0.x, i.tspace1.x, i.tspace2.x);
		// worldNormal += lT * foamColor.r * 1 * (1- invlengFactor);

		//MYFIXED3 LLN = tnormal.xzy;
		MYFIXED3 LLN = worldNormal;

		
		//LIGHTING
#if !defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR) ||  !defined(SKIP_FLAT_SPECULAR)
		
#if defined(USE_LIGHTIN_NORMALS)
		MYFIXED3 LWN = worldNormal;
		MYFIXED3 COMP =  (abs(MYFIXED3(i.tspace0.x, i.tspace1.x, i.tspace2.x) + MYFIXED3(i.tspace0.y, i.tspace1.y, i.tspace2.y)));
		LWN += LWN * COMP * (_LightNormalsFactor);
		LLN += LLN * COMP * (_LightNormalsFactor);
		LWN = normalize(LWN);
		LLN = normalize(LLN);
#else 
		MYFIXED3 LWN = worldNormal;
		//MYFIXED3 LWN =normalize( worldNormal);
#endif
		


		MYFIXED nl = saturate( dot(LWN, -_LightDir));
		//return nl;
		//ROUND SPEC
		//MYFIXED specularLight = pow(max(dot(worldRefl, i.wLightDir), 0.0), _SpecularShininess) * _SpecularAmount;

#if !defined(SKIP_SPECULAR) || !defined(SKIP_FLAT_SPECULAR)
		MYFIXED specularLight = 0;
/*#if !defined(LOW_NORMALS)
		MYFIXED3 FIXEDAngleVector = normalize(normalize(wViewDir) + normalize(-_LightDir));
#else*/
		 MYFIXED3 ava = wViewDir - _LightDir; //optimize
		ava.z /= 2;
		MYFIXED3 FIXEDAngleVector = normalize(ava);

//#endif
		//FIXEDAngleVector = normalize(-i.wLightDir);
		//FIXEDAngleVector =  normalize(wViewDir);
																						//MYFIXED3 FIXEDAngleVector = normalize(wViewDir) + normalize(i.wLightDir); //optimize
		//MYFIXED3 FIXEDAngleVector = normalize((i.wLightDir + wViewDir)); //optimize
		MYFIXED UP_SUN = (1 - dot(MYFIXED3(0, 1, 0), -_LightDir))*0.65 + 0.35;
		//UP_SUN = (UP_SUN * UP_SUN) * 3;
#if !defined(SKIP_SPECULAR)
		//worldNormal.z /= 10;
		//specularLight += pow(max(dot(FIXEDAngleVector, (NworldNormal + worldNormal) / 2), 0.0), _SpecularShininess * 3) * _SpecularAmount;
		MYFIXED SPEC_DOT = saturate(dot(FIXEDAngleVector, LWN));
		
		//specularLight += pow(saturate(SPEC_DOT - 0.8) * 5, _SpecularShininess * 3) * _SpecularAmount;
		specularLight += pow(SPEC_DOT, _SpecularShininess * 30) * _SpecularAmount;
#if !defined(SKIP_SPECULAR_GLOW)
		specularLight += pow(SPEC_DOT, _SpecularShininess * 0.8333) * _SpecularAmount / 5;
#endif
	
		specularLight *= UP_SUN;
#endif
		nl *= UP_SUN;

#if !defined(SKIP_FLAT_SPECULAR)


#if defined(FLAT_AS_STARS)
		MYFIXED4 fix = (i.uv.y * _FlatFriqY + i.uv.x * _FlatFriqX) * 4;
#else
		MYFIXED4 fix = 0;
#endif
		//fix = 0;
		
	
#if defined(USE_FLAT_OWN)
		MYFIXED4 flat_bump1 = (tex2D(_BumpMap, UVSCROLL.xy * 5 * MYFIXED2(_FlatFriqX, _FlatFriqY) - fix));
		MYFIXED4 flat_bump2 = tex2D(_BumpMap, UVSCROLL.zw * 2 * MYFIXED2(_FlatFriqX, _FlatFriqY) + fix);


#if defined(USE_FLAT_HQ)
		MYFIXED3 flat_tnormal1 = UnpackNormal(flat_bump1);
		MYFIXED3 flat_tnormal2 = UnpackNormal(flat_bump2);
		MYFIXED flat_lerp = saturate((-flat_tnormal2.z + flat_tnormal1.z) * FLAT_HQ_OFFSET);
		MYFIXED3 flat_tnormal = flat_tnormal1 * flat_lerp + flat_tnormal2 * (1 - flat_lerp);

#else

		MYFIXED4 flat_mixed = (flat_bump1 + flat_bump2) * 0.53;
		//MYFIXED4 flat_mixed = max(flat_bump1 , flat_bump2);
	//flat_mixed = mixed;
	MYFIXED3 flat_tnormal = UnpackNormal(flat_mixed);
#endif
#else
		MYFIXED3 flat_tnormal = tnormal;

#endif
	//ROT_NRM(flat_tnormal)


		//MYFIXED3 flat_tnormal = tnormal;

		MYFIXED DD = saturate(dot(flat_tnormal, MYFIXED3(0.1, 0.98, 0.05)) * _FlatSpecularAmount);
		//MYFIXED DD = 1 - (flat_tnormal.x + flat_tnormal.y) / 2;
		MYFIXED flat_result = saturate((pow(DD, _FlatSpecularShininess))) * _FlatSpecularAmount;

#if !defined(SKIP_FLAT_SPECULAR_CLAMP)
		MYFIXED3 CCC = LWN;
		MYFIXED FLAT_NL = pow(max(dot(FIXEDAngleVector, CCC), 0.0), _FlatSpecularClamp);
		flat_result *= FLAT_NL;
		//specularLight += saturate(pow(, _FlatSpecularShininess * 300))* _FlatSpecularAmount;

#endif

		specularLight += flat_result;




		/* OLD
		MYFIXED3 NNN = (MYFIXED3(i.tspace0.z, i.tspace1.z, i.tspace2.z));
		//MYFIXED3 CCC = NworldNormal * 0.25 + worldNormal * 0.75;
		MYFIXED MS = 40;
		//MYFIXED3 CCC = normalize(MYFIXED3(sin(worldNormal.x * MS), sin(worldNormal.y * MS), sin(worldNormal.z * MS)));
		//MYFIXED3 CCC2 = normalize(MYFIXED3(sin(worldNormal.x * MS), sin(worldNormal.y * MS), (worldNormal.z)));
		//MYFIXED3 CCC = normalize(worldNormal* abs(worldNormal.z));
		//MYFIXED3 CCC = normalize(NworldNormal * MYFIXED3(1, sin(i.uv.x * 10), 1));
		//MYFIXED3 CCC = normalize(NworldNormal * MYFIXED3(1, sin(i.uv.x * 10)*0.5 + 2, 1));
		//MYFIXED3 CCC = normalize(NworldNormal * MYFIXED3(1, sin(i.uv.x * 10 )*0.5 + 2 + tnormal.y * 50, 1));
		
		MYFIXED3 CCC = worldNormal;
		//CCC.y = lerp(CCC.y, NNN.y, sin(i.uv.x * 10) );

		//CCC = -(UnpackNormal(bump1 - bump2 / 1.2 + cos(i.uv.y * 20 + sin(i.uv.x * 20 + i.uv.y*4))  )  );

#if !defined(SKIP_FLAT_SPECULAR_CLAMP)
		//MYFIXED ddd = (1 + dot(worldNormal, -wViewDir)) / 2;
		//specularLight = ddd;
		//* min(_FlatSpecularClamp , max(0,3 * (-0.0 + ddd )))
		MYFIXED ddd = dot(NNN, wViewDir);
		ddd *= ddd;

		specularLight += saturate( pow(max(dot(lerp(NNN, FIXEDAngleVector, 0.0001 * _FlatSpecularClamp * ddd), CCC),0.0), _FlatSpecularShininess * 300) )* _FlatSpecularAmount;
#else
		//float p =1 - ( (pow((dot(NNN, CCC)), 2) - 0.5)-0.4)*20 + 0.9;
		//specularLight += p * p * 20;

		//MYFIXED d = (dot(NNN, CCC) - 0.9) * 10;
		//float p = pow(d, 200);
		//specularLight += saturate(p * p * p)* _FlatSpecularAmount;

		//MYFIXED DD =  sin( (dot(NNN, CCC) * 0.5 + 0.5) * 4 );
		//MYFIXED DD = max(dot(NNN, CCC), 0.0);
		//40 *
		//MYFIXED DD = frac((NNN.x * CCC.x + NNN.y * CCC.y+ NNN.z * CCC.z) *  (( 1000-distance(_WorldSpaceCameraPos , i.wPos)) / 200000 + 1 ) * 40 ) / 1.1;

		//MYFIXED4 fix = (i.uv.x /3) * (i.uv.y *i.uv.y) /2;
		
		//MYFIXED DD = 1 - (flat_tnormal.x + flat_tnormal.y) / 2;
		specularLight += saturate((pow(DD, _FlatSpecularShininess ))) * _FlatSpecularAmount;
#endif*/
#endif


		//UP_SUN = (UP_SUN * 1.5);
#else
		MYFIXED3 FIXEDAngleVector;
		MYFIXED specularLight = 0;
#endif

#if !defined(SKIP_SURFACE_FOAMS)
		//0, 99458534073250848412034271523232
		//MYFIXED3 ASDASD = normalize(MYFIXED3(0.12, 0.98, 0.12));
			//MYFIXED3 ASDASD = (MYFIXED3(0.12065329648999294186660041025867, 0.98533525466827569191057001711249, 0.12065329648999294186660041025867));
		
		//ssspecularLight += saturate(pow(dot(_SUrfaceFoamVector, LWN), _SurfaceFoamContrast * 3)) * _SurfaceFoamAmount;
		specularLight += saturate(pow(dot(_SUrfaceFoamVector, LLN), _SurfaceFoamContrast * 3)) * _SurfaceFoamAmount;
#endif

#if !defined(USE_FAKE_LIGHTING)
		MYFIXED3 NL_MULT = nl * _LightColor0;
#else
		MYFIXED3 NL_MULT = nl * _LightColor0Fake;
#endif	
		specularLight *= NL_MULT;
#if !defined(SKIP_LIGHTING)
		FIXEDAngleVector = ( _LightAmount * nl * 2 * NL_MULT);
#else
		FIXEDAngleVector = MYFIXED3(0,0,0);
#endif

/*#if !defined(USE_FAKE_LIGHTING)
#if !defined(SKIP_LIGHTING)
		FIXEDAngleVector = (_LightAmount * nl * 2 + specularLight) * nl *_LightColor0;
#else
		FIXEDAngleVector = (specularLight)* nl *_LightColor0;
#endif	   
#else
#if !defined(SKIP_LIGHTING)
		FIXEDAngleVector = (_LightAmount * nl * 2 + specularLight) * nl *_LightColor0Fake;
#else
		FIXEDAngleVector = (specularLight)* nl *_LightColor0Fake;
#endif
#endif*/

#endif
		//LIGHTING
		//return MYFIXED4(FIXEDAngleVector, 1);


	//	return float4(specularLight.rrr, 1);

#if defined(HAS_REFRACTION)


		MYFIXED RefrDist = _RefrDistortion * (lerped_refr*0.9 + 0.1);
		//MYFIXED RefrDist = _RefrDistortion ;

	

#if defined(REFRACTION_ONLYZCOLOR)
		// refractionColor = MYFIXED3(0, 0, 0);
#elif defined(HAS_BAKED_REFRACTION)


#ifdef ORTO_CAMERA_ON
		RefrDist = RefrDist / ORTO_PROJ_DIVIDER / 300;
#else
		RefrDist = RefrDist / 300;
#endif

		/////////
		MYFIXED2 refrUv = i.uv.zw + rawtnormal.xy * RefrDist;
		/* D = D / i.pos.w;
		D = -normalize(D);
		MYFIXED3 asd = normalize(UnityWorldSpaceViewDir(i.wPos));*/
		//MYFIXED3 pp = ObjSpaceViewDir(MYFIXED4(i.wPos, 1)).xyz;
		//MYFIXED3 vn = COMPUTE_VIEW_NORMAL;
		//MYFIXED3 pp = ObjSpaceViewDir(i.VER_TEX).xyz;
		//pp = mul(rotation, MYFIXED4(pp,1)).xyz;
		// pp = normalize(pp);
		//MYFIXED3 pp = mul(UNITY_MATRIX_MV, float4(i.pos.xyz, 1)).xyz;

		/* mul(UNITY_MATRIX_MV, float4(pos, 1.0)).xyz
		D = ObjSpaceViewDir(i.wPos).xyz;*/
		//asd = -asd;
		// refrUv -= ParallaxGen(zdepth / 100, pp);

#if !defined(DEPTH_NONE) && !defined(SKIP_REFRACTION_CALC_DEPTH_FACTOR)
		MYFIXED3 pp = (i.VER_TEX.xyz);
		refrUv -= pp.xy / (pp.z * 0.8 + 0.2) * (((zdepth) / _RefrDeepFactor));
#endif



#if defined(ADDITIONAL_ANGLE_DISTORTION) 
		refrUv.xy -= ((rawtnormal.xy)* i.VER_TEX.xy) / (i.VER_TEX.z) * RefrDist;
#endif
#if defined(USE_YADD)
		refrUv.y += Y_ADD;
#endif
#if !defined(SKIP_FOAM) && defined(ADDITIONAL_FOAM_DISTORTION)
		//MYFIXED AM_OFF = saturate(frac(foamColor.b+0.4)-0.4) * 15;
		//refrUv.xy -= i.VER_TEX.xy / (i.VER_TEX.z * 0.8 + 0.2) * AM_OFF ;
		MYFIXED amp = (1 - saturate(foamColor.b));
		MYFIXED ampam = (amp - 0.8)/300;
#ifdef ORTO_CAMERA_ON
		//ampam /= ORTO_PROJ_DIVIDER;
#endif
		refrUv.xy -= (ampam)  * i.VER_TEX.xy * amp  * ADDITIONAL_FOAM_DISTORTION_AMOUNT;
#endif



#if defined(REFRACTION_BAKED_FROM_TEXTURE) 
#define REFRTEX(uva) TEX2DGRAD(_RefractionTex, (uva)).rgb
		//sampler2D targetTextureRefr = _RefractionTex;
#else
#define REFRTEX(uva) TEX2DGRAD(_RefractionTex_temp, (uva)).rgb
		//sampler2D targetTextureRefr = _RefractionTex_temp;
#endif


#if defined(REFRACTION_BLUR)
	//	MYFIXED3 REFR_BLUR4(MYFIXED2 refrUv, MYFIXED inRefrBlur) {
#define REFR_BLUR4(inrefrUv, inRefrBlur)\
			REFR_BLUR4_result = REFRTEX(inrefrUv + MYFIXED2(0, inRefrBlur));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED2(inRefrBlur, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED2(0, -inRefrBlur));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED2(-inRefrBlur, 0));\
			REFR_BLUR4_result /= 4
		//}
		//MYFIXED3 REFR_BLUR9(MYFIXED2 refrUv, MYFIXED inRefrBlur) {
#define REFR_BLUR9(inrefrUv, inRefrBlur)\
			REFR_BLUR4_result = REFRTEX(inrefrUv + MYFIXED2(0, inRefrBlur));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED2(inRefrBlur * 0.5, inRefrBlur * 0.5));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED2(inRefrBlur, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED2(inRefrBlur * 0.5, -inRefrBlur * 0.5));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED2(0, -inRefrBlur));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED2(-inRefrBlur * 0.5, -inRefrBlur * 0.5));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED2(-inRefrBlur, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED2(-inRefrBlur * 0.5, -inRefrBlur * 0.5));\
			REFR_BLUR4_result /= 9
		//}
#endif



		
		 refractionColor = REFRTEX( refrUv );
/*#if defined(REFRACTION_BAKED_FROM_TEXTURE) 
		MYFIXED3 refractionColor = tex2D(targetTextureRefr, refrUv);
#else
		MYFIXED3 refractionColor = tex2D(targetTextureRefr, refrUv);
#endif*/
	//	return MYFIXED4(refractionColor, 1);
#if defined(REFRACTION_BLUR)
		MYFIXED _REFR_BLUR = _RefrBlur / 100;

#if !defined(SKIP_REFRACTION_BLUR_ZDEPENDS) && (defined(HAS_CAMERA_DEPTH) || defined(HAS_BAKED_DEPTH))
		_REFR_BLUR *= saturate(  lerped_refr + _RefractionBlurZOffset);
		//_REFR_BLUR *= zdepth;
#endif
		MYFIXED3 REFR_BLUR4_result;
#if defined(REFRACTION_BLUR_3)
		REFR_BLUR4(refrUv, _REFR_BLUR);
		MYFIXED3 blured = REFR_BLUR4_result;
		_REFR_BLUR += _REFR_BLUR;
			REFR_BLUR9(, refrUv, _REFR_BLUR);
		MYFIXED3 blured9 = REFR_BLUR4_result;
		_REFR_BLUR += _REFR_BLUR;
		REFR_BLUR9(refrUv, _REFR_BLURUR);
		MYFIXED3 blured92 = REFR_BLUR4_result;
		refractionColor = (refractionColor* 0.1 + blured * 0.15 + blured9 * 0.25 + blured92 * 0.5);
#elif defined(REFRACTION_BLUR_2)
		REFR_BLUR4(refrUv, _REFR_BLUR);
		MYFIXED3 blured = REFR_BLUR4_result;
		_REFR_BLUR += _REFR_BLUR;
		REFR_BLUR9(refrUv, _REFR_BLUR);
		MYFIXED3 blured9 = REFR_BLUR4_result;
		refractionColor = (refractionColor * 0.15 + blured * 0.25 + blured9 * 0.6);
#else
		REFR_BLUR4(refrUv, _REFR_BLUR);
		MYFIXED3 blured2 = REFR_BLUR4_result;
		refractionColor = blured2;
		// refractionColor = (refractionColor * 0.0 + blured * 1);
#endif
#endif

		//return MYFIXED4(refractionColor, 1);
		/////////
#elif defined(REFRACTION_GRABPASS)



		/////////
		// MYFIXED3 pp = ObjSpaceViewDir(MYFIXED4(i.wPos, 1)).xyz;
		// i.screen.y = i.grabPos.y + (1 - dot(wViewDir, worldNormal)) * 30;
		// i.screen.y = UNITY_PROJ_COORD(i.screen).y + (1 - dot(wViewDir , worldNormal) ) * 30 ;
MYFIXED4 refrUv = i.grabPos;

#ifdef ORTO_CAMERA_ON
	RefrDist /= ORTO_PROJ_DIVIDER;
		//refrUv.xy +=  (rawtnormal.xy)* RefrDist / ORTO_PROJ_DIVIDER;
#else
		//refrUv.xy +=  (rawtnormal.xy)* RefrDist;
#endif
refrUv.xy += (rawtnormal.xy)* RefrDist;





#if defined(ADDITIONAL_ANGLE_DISTORTION) 
refrUv.xy -= ((rawtnormal.xy )* i.VER_TEX.xy  ) / (i.VER_TEX.z  ) * RefrDist;
#endif
#if defined(USE_YADD)
		refrUv.y += Y_ADD;
#endif
#if !defined(SKIP_FOAM) && defined(ADDITIONAL_FOAM_DISTORTION)
		//MYFIXED AM_OFF = saturate(frac(foamColor.b+0.4)-0.4) * 15;
		//refrUv.xy -= i.VER_TEX.xy / (i.VER_TEX.z * 0.8 + 0.2) * AM_OFF ;
		MYFIXED amp = 1- saturate(foamColor.b);
#ifdef ORTO_CAMERA_ON
		amp /= ORTO_PROJ_DIVIDER;
#endif
		refrUv.xy -= (amp - 0.8) * i.VER_TEX.xy * amp  * ADDITIONAL_FOAM_DISTORTION_AMOUNT;
#endif


		// refrUv.y += (dot(wViewDir, worldNormal)) * 10;
		 refractionColor = tex2Dproj(_GrabTexture, refrUv);

		// return tex2Dproj(_GrabTexture, refrUv);

#if defined(REFRACTION_BLUR)
		MYFIXED _REFR_BLUR = _RefrBlur;

#if !defined(SKIP_REFRACTION_BLUR_ZDEPENDS) && (defined(HAS_CAMERA_DEPTH) || defined(HAS_BAKED_DEPTH))
		_REFR_BLUR *= saturate( lerped_refr + _RefractionBlurZOffset);
		//_REFR_BLUR *= zdepth;
#endif
		//UNITY_PROJ_COORD
#define REFRTEX(uv) tex2Dproj(_GrabTexture, (uv)).rgb
		MYFIXED3 REFR_BLUR4_result;
		
#define REFR_BLUR4_PROJ(inrefrUv, inReflBlur)\
			REFR_BLUR4_result = REFRTEX(inrefrUv + MYFIXED4(0, inReflBlur, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED4(inReflBlur, 0, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED4(0, -inReflBlur, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED4(-inReflBlur, 0, 0, 0));\
			REFR_BLUR4_result /= 4
#define REFR_BLUR9_PROJ(inrefrUv, inReflBlur)\
			REFR_BLUR4_result = REFRTEX(inrefrUv + MYFIXED4(0, inReflBlur, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED4(inReflBlur * 0.5, inReflBlur * 0.5, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED4(inReflBlur, 0, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED4(inReflBlur * 0.5, -inReflBlur * 0.5, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED4(0, -inReflBlur, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED4(-inReflBlur * 0.5, -inReflBlur * 0.5, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED4(-inReflBlur, 0, 0, 0));\
			REFR_BLUR4_result += REFRTEX(inrefrUv + MYFIXED4(-inReflBlur * 0.5, -inReflBlur * 0.5, 0, 0));\
			REFR_BLUR4_result /= 9


#ifdef ORTO_CAMERA_ON
		_REFR_BLUR /= ORTO_PROJ_DIVIDER;
#endif

#if defined(REFRACTION_BLUR_3)
		REFR_BLUR4_PROJ( refrUv, _REFR_BLUR);
		MYFIXED3 blured = REFR_BLUR4_result;
		_REFR_BLUR += _REFR_BLUR;
		REFR_BLUR9_PROJ( refrUv, _REFR_BLUR);
		MYFIXED3 blured9 = REFR_BLUR4_result;
		_REFR_BLUR += _REFR_BLUR;
		REFR_BLUR9_PROJ( refrUv, _REFR_BLUR);
		MYFIXED3 blured92 = REFR_BLUR4_result;
		refractionColor = (refractionColor* 0.1 + blured * 0.15 + blured9 * 0.25 + blured92 * 0.5);
		// refractionColor = (refractionColor + (blured + (blured9 + blured92) / 2) / 2) / 2;
#elif defined(REFRACTION_BLUR_2)
		REFR_BLUR4_PROJ( refrUv, _REFR_BLUR);
		MYFIXED3 blured = REFR_BLUR4_result;
		_REFR_BLUR += _REFR_BLUR;
		REFR_BLUR9_PROJ( refrUv, _REFR_BLUR);
		MYFIXED3 blured9 = REFR_BLUR4_result;
		refractionColor = (refractionColor * 0.15 + blured * 0.25 + blured9 * 0.6);
		//refractionColor = (refractionColor + (blured + blured9) / 2) / 2;
#else
		REFR_BLUR4_PROJ( refrUv, _REFR_BLUR);
		MYFIXED3 blured = REFR_BLUR4_result;
		refractionColor = blured;
		//refractionColor = (refractionColor + blured) / 2;
#endif
		/*  refractionColor += tex2D(_GrabTexture, refrUv + MYFIXED2(0, _REFR_BLUR));
		refractionColor += tex2D(_GrabTexture, refrUv + MYFIXED2(_REFR_BLUR, 0));
		refractionColor += tex2D(_GrabTexture, refrUv + MYFIXED2(0, -_REFR_BLUR));
		refractionColor += tex2D(_GrabTexture, refrUv + MYFIXED2(-_REFR_BLUR, 0) );
		refractionColor /= 5;*/
#endif

		/////////

		//refractionColor *= (1 - _Transparency / 2);
		//reflectionColor *= (1 - fresnelFac);
#else 
		 refractionColor = MYFIXED3(0, 0, 0);
#endif







#endif

		//MAIN MIX
		//MAIN MIX
		//MAIN MIX




		///* tex2D(_NoiseHQ, i.uv / 10) * 3
#if !defined(SKIP_REFLECTION_MASK) && !defined(REFLECTION_NONE)
		MYFIXED4 rawtargetMask = TEX2DGRAD(_MainTex, i.uv.zw * _ReflectionMask_Tiling + _ReflectionMask_TexOffsetF);

		/*#if !defined(SKIP_MAINTEXTURE)
		targetMask = tex2D(_MainTex, i.uv * _ReflectionMask_Tiling);
#else

		targetMask = tex2D(_BumpMap, i.uv * _ReflectionMask_Tiling);
#endif*/


		MYFIXED4 t = min(_ReflectionMask_UpClamp,(rawtargetMask * _ReflectionMask_Amount + _ReflectionMask_Offset) );


#if defined(REFLECTION_MASK_R)
		MYFIXED mask = t.r;
		 reflectionColor *= t.r;
#elif defined(REFLECTION_MASK_G)
		MYFIXED mask = t.g;
		 reflectionColor *= t.g;
#elif defined(REFLECTION_MASK_B)
		MYFIXED mask = t.b;
		 reflectionColor *= t.b;
#else
		MYFIXED mask = t.a;
		 reflectionColor *= t.a;
#endif

#if defined(REFL_MASK_DEBUG)
		 return float4(mask.rrr, 1);
#endif

#else
		//color.rgb = reflectionColor;
#endif



#if defined(REFLECTION_DEBUG_RGB) && !defined(REFLECTION_NONE)
		return float4(reflectionColor.rgb / _ReflectionAmount, 1);
#endif




#ifdef WATER_DOWNSAMPLING_HARD
}
else { return TEX2DGRAD(_FrameBuffer, fb_wcoord); }if (cond) {
#endif





		//color.rgb = reflectionColor;

		//ALPHA
		//REFRACTION_ONLYZCOLOR
#if defined(HAS_REFRACTION)


		
#if defined(HAS_REFR_MASK)
		MYFIXED4 HAS_REFR_MASK_t = min(_REFR_MASK_max, (TEX2DGRAD(_MainTex, i.uv.zw * _REFR_MASK_Tile + _REFR_MASK_offset) * _REFR_MASK_Amount + _REFR_MASK_min));
#if defined(REFR_MASK_R)
		MYFIXED HAS_REFR_MASK_t_mask = HAS_REFR_MASK_t.r;
#elif defined(REFR_MASK_G)
		MYFIXED HAS_REFR_MASK_t_mask = HAS_REFR_MASK_t.g;
#elif defined(REFR_MASK_B)
		MYFIXED HAS_REFR_MASK_t_mask = HAS_REFR_MASK_t.b;
#else
		MYFIXED HAS_REFR_MASK_t_mask = HAS_REFR_MASK_t.a;
#endif
		lerped_refr *= saturate(_REFR_MASK_Amount - HAS_REFR_MASK_t_mask);

#if defined(REFR_MASK_DEBUG)
		return float4(saturate(_REFR_MASK_Amount - HAS_REFR_MASK_t_mask).rrr, 1);
#endif
#endif

	
		


#if defined(REFRACTION_DEBUG)
		return MYFIXED4(lerped_refr, lerped_refr, lerped_refr, 1);
#endif

		MYFIXED4 REF_C;
		//REF_C.rgb = ((_RefrTopZColor - _RefrZColor) *  lerped_refr + _RefrZColor) * _RefrAmount;
		//MYFIXED3 REF_C = ((1 - _RefrZColor) * lerped_refr + _RefrZColor) * _RefrAmount;
		//MYFIXED3 REF_C = (( _RefrZColor ) * (1-lerped_refr ) + _RefrTopZColor * lerped_refr) * _RefrAmount;

		MYFIXED3 RefrTopZColor = _RefrTopZColor;
		MYFIXED3 RefrZColor = _RefrZColor;

#if !defined(REFRACTION_ONLYZCOLOR)


		//todo MASK WORKS

		
		//color.rgb = lerp( color.rgb, REF_C, lerped_refr);
		//color.rgb = REF_C + color.rgb + refractionColor * (1 - lerped_refr);
		//color.rgb = REF_C + color.rgb;
		//color.rgb = lerp( color.rgb, _RefrZColor, lerped_refr);

		/*REF_C.a = lerped_refr;
		MYFIXED3 MLT_DEEP = refractionColor * REF_C.rgb;
		MYFIXED3 DEEP =  lerp(refractionColor, MLT_DEEP, _RefrRecover);
		MYFIXED3 FRONT = REF_C.rgb *(refractionColor + _RefrTextureFog) / (1 + _RefrTextureFog);
		REF_C.rgb = lerp(DEEP, FRONT, lerped_refr);*/
//BACK
		REF_C.a = zdepth;
		RefrTopZColor = lerp(refractionColor , MYFIXED3(1,1,1), _RefrTextureFog) * RefrTopZColor;
		RefrZColor = lerp(MYFIXED3(1, 1, 1), refractionColor , _RefrRecover) * RefrZColor;
		//REF_C.rgb = lerp(REF_C.rgb, REF_C.rgb *  (refractionColor + _RefrTextureFog) / (1 + _RefrTextureFog), lerped_refr + _RefrRecover);
		//		return float4(REF_C.rgb, 1);

		//	return MYFIXED4(REF_C, 1);
		//TOP_COLOR *= refractionColor * saturate(lerped_refr + _RefrRecover);
#else

		//color.rgb *= REF_C;
#endif

#if defined(USE_CAUSTIC)
#ifdef ORTO_CAMERA_ON
		MYFIXED3 sourcecausticWP = i.wPos;
#else
		//MYFIXED4 osp = ComputeScreenPos(i.pos);

#if !defined(HAS_CAMERA_DEPTH) 
		MYFIXED ld = (
#if defined(HAS_ADDITIONAL_DEEP_COLOR_CALC)
			(DEEPzdepth
#else
			(zdepth
#endif
				/ 20));
#else
		MYFIXED ld = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen)));// //
#endif

		MYFIXED3 wD = i.wPos - _WorldSpaceCameraPos;
		MYFIXED3 sourcecausticWP = ((wD / i.screen.w) * ld + _WorldSpaceCameraPos) / 1000;

#endif

#if !defined(SKIP_C_NOISE)

	
		MYFIXED3 causticWP = i.wPos / 1000;
		causticWP.xz = causticWP.xz  * _CAUSTIC_Tiling.x;
		causticWP.x *= _CAUSTIC_Tiling.y;
#ifdef ORTO_CAMERA_ON
		causticWP /= ORTO_PROJ_DIVIDER;
#endif
		//causticWP.x = _Time.x * 100 + causticWP.x;
		causticWP.y = _Time.x * _CAUSTIC_Speed + 1 * _CAUSTIC_Tiling.x; //sourcecausticWP.y
		MYFIXED2 causticUV1 = causticWP.xy + causticWP.yz;
		MYFIXED2 causticUV2 = causticWP.xz + causticWP.yx;
		MYFIXED2 causticUV3 = causticWP.zy + causticWP.xz;
		MYFIXED A1 = tex2D(_NoiseHQ, causticUV1).r;
		MYFIXED B1 = tex2D(_NoiseHQ, causticUV2).g;
		MYFIXED C1 = tex2D(_NoiseHQ, causticUV3).b;
		//MYFIXED caustic_DRES = A * B * C * 8;
		//causticWP.x = -_Time.x * 100 + causticWP.x;
		causticWP.y = -_Time.x *  _CAUSTIC_Speed + 1  * _CAUSTIC_Tiling.x; //sourcecausticWP.y
		//causticFrq = 500 * _CAUSTIC_Tiling;
		causticUV1 = causticWP.xy + causticWP.yz;
		causticUV2 = causticWP.xz + causticWP.yx;
		causticUV3 = causticWP.zy + causticWP.xz;
		MYFIXED A2 = tex2D(_NoiseHQ, causticUV1).r;
		MYFIXED B2 = tex2D(_NoiseHQ, causticUV2).g;
		MYFIXED C2 = tex2D(_NoiseHQ, causticUV3 ).b;
		//MYFIXED caustic_URES = A * B * C * 8;
		//MYFIXED D = tex2Dgrad(_MainTex, causticWP.xy / causticFrq + causticWP.yz / causticFrq,0,0).r;

		MYFIXED XXX = (A1 * B1 * B2 * C2 * A2  ) * _CAUSTIC_Offset.y - _CAUSTIC_Offset.x;
		MYFIXED YYY = (B1 * C1 * C2 * A2 * A1 ) * _CAUSTIC_Offset.y - _CAUSTIC_Offset.x;
#if defined(DEBUG_CAUSTIC_NOISE)
		return (XXX < 0 ? MYFIXED4(-1,0,0,-1) : MYFIXED4 (0,1,0,1))*XXX.xxxx;
#endif
#else
		MYFIXED XXX = 0;
			MYFIXED YYY = 0;
#endif

		
		MYFIXED2 C_TILE = _CAUSTIC_Tiling.zw
#ifdef ORTO_CAMERA_ON
			/ ORTO_PROJ_DIVIDER / 10
#endif
			;

#if defined(USE_FIXV_CAUSTIC)
		MYFIXED2 C_UV = ((sourcecausticWP.xy + sourcecausticWP.yz + sourcecausticWP.xz) / 2 + rawtnormal.xy * RefrDist / 1000) * C_TILE + MYFIXED2(XXX, YYY) * _CAUSTIC_Offset.z;
#else
		MYFIXED2 C_UV = (sourcecausticWP.xz + rawtnormal.xy * RefrDist / 1000) * C_TILE + MYFIXED2(XXX, YYY) * _CAUSTIC_Offset.z;
#endif

#if defined(USE_COLOR_CAUSTIC)
		MYFIXED3 causticRessult;
#define C_SAMPLE(uv) tex2D(_CAUSTIC_MAP,uv ).rgb
#else
		MYFIXED causticRessult;
#define C_SAMPLE(uv) tex2D(_CAUSTIC_MAP,uv ).b
#endif


#if defined(C_ANIM)
		MYFIXED CB = _C_BLUR_S * _Time.x;
		causticRessult = C_SAMPLE(C_UV + MYFIXED2(-CB, 0) * C_TILE);
		causticRessult += C_SAMPLE(C_UV + MYFIXED2(CB +0.25, 0.25) * C_TILE);
		causticRessult += C_SAMPLE(C_UV + MYFIXED2(0.5, CB +0.5) * C_TILE);
		causticRessult += C_SAMPLE(C_UV + MYFIXED2(0.75, -CB-0.25) * C_TILE);
		causticRessult /= 4;
#elif  defined(C_BLUR)
/*#if defined(CAUSTIC_BOTTOM)
		MYFIXED CL = (lerped_refr);
#elif defined(CAUSTIC_TOPBOTTOM)
		MYFIXED CL  1;
#else
		MYFIXED CL = (1 - lerped_refr);
#endif*/
		MYFIXED CB = _C_BLUR_R ;
		causticRessult = C_SAMPLE(C_UV);
		causticRessult += C_SAMPLE(C_UV + MYFIXED2(-CB, 0) * C_TILE);
		causticRessult += C_SAMPLE(C_UV + MYFIXED2(CB, 0) * C_TILE);
		causticRessult += C_SAMPLE(C_UV + MYFIXED2(0, CB) * C_TILE);
		causticRessult += C_SAMPLE(C_UV + MYFIXED2(0 ,-CB) * C_TILE);
		causticRessult /= 5;
#else
		causticRessult = C_SAMPLE(C_UV);
#endif

		causticRessult *= saturate(foamColor.a* foamColor.a) * _CAUSTIC_FOG_Amount;
/*#if defined(USE_COLOR_CAUSTIC)
		MYFIXED3 causticRessult = tex2D(_CAUSTIC_MAP, ).rgb * saturate(foamColor.a* foamColor.a) * _CAUSTIC_FOG_Amount;
#else
		MYFIXED causticRessult = tex2D(_CAUSTIC_MAP, (sourcecausticWP.xz + rawtnormal.xy * RefrDist / 1000) * _CAUSTIC_Tiling.zw
#ifdef ORTO_CAMERA_ON
			/ ORTO_PROJ_DIVIDER / 10
#endif	
			+ MYFIXED2(XXX, YYY) * _CAUSTIC_Offset.z).b * saturate(foamColor.a* foamColor.a) * _CAUSTIC_FOG_Amount;
#endif*/
		////
	
/*#define CSTCSAMPLER(uv) tex2Dgrad(_CAUSTIC_MAP, (uv),0,0)
		MYFIXED2 SOURCE_CUV = (sourcecausticWP.xz + rawtnormal.xy * RefrDist / 1000) * _CAUSTIC_PROC_Tiling.zw;
		MYFIXED2 CUV = SOURCE_CUV.xy + MYFIXED2(XXX, YYY);
		MYFIXED cstc_tex1 = CSTCSAMPLER(CUV).r;
		CUV.y = cstc_tex1 - _Time.x * _CAUSTIC_PROC_GlareSpeed * 1.4 * _CAUSTIC_PROC_Tiling.w;
		MYFIXED cstc_tex2 = CSTCSAMPLER(CUV).r;

		MYFIXED causticRessult = (max(cstc_tex2 - _CAUSTIC_PROC_Contrast, _CAUSTIC_PROC_BlackOffset))* _CAUSTIC_FOG_Amount;*/

		////
		causticRessult = pow(causticRessult, _CAUSTIC_FOG_Pow);

#if defined(DEBUG_CAUSTIC)
#if defined(USE_COLOR_CAUSTIC)
		float3 debugRes = ( causticRessult);
#else
		float3 debugRes = ( causticRessult.rrr);
#endif


#if defined(CAUSTIC_BOTTOM)
		return float4(( lerped_refr ) * debugRes,1);
#elif defined(CAUSTIC_TOPBOTTOM)
		return float4(debugRes, 1);
#else
		return float4((1 - lerped_refr) * debugRes, 1);
#endif
	
#endif
		//return float4(RefrZColor.rgb, 1);
		//return float4(RefrTopZColor.rgb* causticRessult, 1);
		//return float4(RefrTopZColor.rgb, 1);
		//_RefrTopZColor = max(_RefrTopZColor, causticRessult);
		//_RefrTopZColor += _RefrTopZColor * (1 - (saturate(abs((caustic_DRES * caustic_URES * _CAUSTIC_Offset.z) * _CAUSTIC_Offset.y - _CAUSTIC_Offset.x) * _CAUSTIC_Offset.y) ))* saturate( foamColor.a* foamColor.a)* _CAUSTIC_FOG_Amount;

#if defined(CAUSTIC_BOTTOM)

		RefrZColor.rgb += RefrZColor.rgb* causticRessult;
		REF_C.rgb = lerp(RefrTopZColor, RefrZColor, lerped_refr) * _RefrAmount;
#elif defined(CAUSTIC_TOPBOTTOM)
		REF_C.rgb = lerp(RefrTopZColor, RefrZColor, lerped_refr) ;
		REF_C.rgb += REF_C.rgb * causticRessult;
		REF_C.rgb *= _RefrAmount;
#else
		RefrTopZColor.rgb += RefrTopZColor.rgb* causticRessult;
		REF_C.rgb = lerp(RefrTopZColor, RefrZColor, lerped_refr) * _RefrAmount;
#endif


#else

		REF_C.rgb = lerp(RefrTopZColor, RefrZColor, lerped_refr) * _RefrAmount;

#endif





		//return float4(REF_C.rgb, 1);

/*#if !defined(SKIP_REFLECTION_MASK)
		REF_C *= mask;
#endif*/

		//DEBUG
		/* if (rawDepth > buffer[0].x) buffer[0].x = rawDepth;
		if (linearDepth > buffer[0].y) buffer[0].y = linearDepth;
		if (zdepth > buffer[0].z) buffer[0].z = zdepth;*/
		// if (_CameraFarClipPlane > buffer[0].z) buffer[0].z = _CameraFarClipPlane;


		/*#if !defined(SKIP_Z_WORLD_CALCULATION)
#else
		color.rgb *= saturate(_RefrZOffset - (zdepth) / _RefrZFallOff);
#endif*/

#if defined(DESATURATE_REFR)
		REF_C.rgb = lerp(REF_C.rgb, (REF_C.b + REF_C.g) / 2, _RefractionDesaturate);
#endif

		// color.rgb = 1 - zdepth / 30;
#else
		MYFIXED4 REF_C = MYFIXED4(0, 0, 0, 0);
#endif




#if defined(HAS_REFRACTION_Z_BLEND_AND_RRFRESNEL)
		MYFIXED blendamount;
#if defined(REFRACTION_Z_BLEND_INVERSE)
		blendamount = 1 - saturate(((REF_C.a - _RefractionBlendOffset) / _RefractionBlendFade + _RefractionBlendOffset));
#else
		blendamount = saturate(((REF_C.a - _RefractionBlendOffset) / _RefractionBlendFade + _RefractionBlendOffset));
#endif
		blendamount = min(saturate(pow(fresnelFac * _RefrBled_Fres_Amount, _RefrBled_Fres_Pow)), blendamount);
		//return saturate(pow(fresnelFac * _RefrBled_Fres_Amount, _RefrBled_Fres_Pow));
		color.rgb = lerp(reflectionColor, REF_C.rgb, blendamount);
#elif defined(HAS_REFRACTION_Z_BLEND)
		//MYFIXED3 refr2 = REF_C.rgb;
		//return float4(reflectionColor.rgb, 1);
#if defined(REFRACTION_Z_BLEND_INVERSE)
		color.rgb = lerp(REF_C.rgb, reflectionColor 
#if defined(HAS_REFRACTION)
			* _RefrZColor
#endif
			, saturate(((REF_C.a - _RefractionBlendOffset) / _RefractionBlendFade + _RefractionBlendOffset))) ;
#else
		color.rgb = lerp(reflectionColor 
#if defined(HAS_REFRACTION)
			* _RefrTopZColor
#endif
			, REF_C.rgb, saturate(((REF_C.a- _RefractionBlendOffset)/ _RefractionBlendFade + _RefractionBlendOffset)));
#endif
#elif defined(RRFRESNEL)
		color.rgb =  lerp(reflectionColor, REF_C.rgb, saturate( pow( fresnelFac * _RefrBled_Fres_Amount, _RefrBled_Fres_Pow)));
		//return float4(color.rgb / 7,1);
#elif defined(RRMULTIBLEND)
		color.rgb = (((REF_C.rgb+ _AverageOffset) * (reflectionColor + _AverageOffset)) - _AverageOffset) / 4;
#elif defined(RRMAXBLEND)
		color.rgb = max(REF_C.rgb, reflectionColor);
#else
		color.rgb = lerp(REF_C.rgb , reflectionColor, _AverageOffset);
#endif
		//return float4(color.rgb, 1);

#if defined(REFRACTION_DEBUG_RGB) && defined(HAS_REFRACTION)

		  

		/*MYFIXED ld = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen)));
		MYFIXED3 wD = i.wPos - _WorldSpaceCameraPos;
		MYFIXED3 worldspace = wD / (i.screen.w) * ld + _WorldSpaceCameraPos;
		worldspace /= 1000;
		MYFIXED val = tex2Dgrad(_MainTex, worldspace.xz, 0, 0).r;
		return 1 - saturate(abs((val.x) * 2 - 1) * 2);*/
	//return 1 - saturate(abs(((tnormal.x) + (tnormal.y)) * 20 - 1) * 2);


		return float4(REF_C.rgb / _RefrAmount, 1);
#endif

			//color.rgb = REF_C;
		//color.rgb = REF_C*0.9+ reflectionColor * 0.1;
		//
		//color.rgb = REF_C;
		//color.rgb = max(REF_C, color.rgb);

		//return ;
		//return MYFIXED4(color.rgb,1);
		//color.rgb = REF_C + reflectionColor;


#if !defined(SKIP_MAINTEXTURE)
		MYFIXED2 uvM = DETILEUV;
		uvM.x += _MainTexTile.z * _Time.x + rawtnormal.x * _MTDistortion;
		uvM.y += _MainTexTile.w * _Time.x + rawtnormal.y * _MTDistortion;
		MYFIXED3 maintexunpack = tex2D(_MainTex, (uvM)*_MainTexTile.xy).rgb * _MainTexColor.a;

#if defined(MAINTEXMASK)
		MYFIXED main_mask = min(_MAINTEXMASK_max, max(0, (TEX2DGRAD(_MainTex, DETILEUV * _MAINTEXMASK_Tile + _MAINTEXMASK_offset).r + _MAINTEXMASK_min)))  * _MAINTEXMASK_Amount ;
#if defined(MAINTEX_MASK_DEBUG)
		return float4(main_mask.rrr, 1);
#endif
		maintexunpack *= main_mask;
#endif

#if defined(MAINTEXMAXBLEND)
			color.rgb = max(color.rgb, maintexunpack) ;
#else
			color.rgb = (maintexunpack - color.rgb) *_MAINTEXMASK_Blend + color.rgb;
			//color.rgb += maintexunpack * 0.5;
#endif

#endif

#if defined(SURFACE_FOG)
			MYFIXED SRF_FOG_FRQ = 1000 * _SURFACE_FOG_Tiling;
			MYFIXED3 SRF_FOG_WP = i.wPos;
			SRF_FOG_WP.y = _Time.x * 100 * _SURFACE_FOG_Speed + SRF_FOG_WP.y;
			MYFIXED2 SRF_FOG_UV1 = SRF_FOG_WP.xy + SRF_FOG_WP.yz;
			MYFIXED2 SRF_FOG_UV2 = SRF_FOG_WP.xz + SRF_FOG_WP.yx;
			MYFIXED2 SRF_FOG_UV3 = SRF_FOG_WP.zy + SRF_FOG_WP.xz;
			MYFIXED A = tex2Dgrad(_MainTex, SRF_FOG_UV1 / SRF_FOG_FRQ).r;
			MYFIXED B = tex2Dgrad(_MainTex, SRF_FOG_UV2 / SRF_FOG_FRQ).g;
			MYFIXED C = tex2Dgrad(_MainTex, SRF_FOG_UV3 / SRF_FOG_FRQ).b;
			MYFIXED DRES = A * B * C * 8;
			//SRF_FOG_WP.x = -_Time.x * 100 + SRF_FOG_WP.x;
			SRF_FOG_WP.y = -_Time.x * 200 * _SURFACE_FOG_Speed + SRF_FOG_WP.y;
			SRF_FOG_UV1 = SRF_FOG_WP.xy + SRF_FOG_WP.yz;
			SRF_FOG_UV2 = SRF_FOG_WP.xz + SRF_FOG_WP.yx;
			SRF_FOG_UV3 = SRF_FOG_WP.zy + SRF_FOG_WP.xz;
			SRF_FOG_FRQ = 500 * _SURFACE_FOG_Tiling;
			A = tex2Dgrad(_MainTex, SRF_FOG_UV1 / SRF_FOG_FRQ).r;
			B = tex2Dgrad(_MainTex, SRF_FOG_UV2 / SRF_FOG_FRQ).g;
			C = tex2Dgrad(_MainTex, SRF_FOG_UV3 / SRF_FOG_FRQ).b;
			MYFIXED URES = A * B * C * 8;
			//MYFIXED D = tex2Dgrad(_MainTex, SRF_FOG_WP.xy / SRF_FOG_FRQ + SRF_FOG_WP.yz / SRF_FOG_FRQ,0,0).r;
			color.rgb += color.rgb * DRES * URES * _SURFACE_FOG_Amount;
			//color.rgb = lerp(color.rgb, 1,  DRES * URES * _SURFACE_FOG_Amount);
#endif




#if defined(SURFACE_WAVES)
			color.rgb +=   surface_noise;
#endif

		//color.rgb *= saturate((zdepth) / 10 * _Transparency);

		//color.a = lerp( 1 - tnormal.y * 5 - 1 + _Transparency, 1 , 1 - fresnelFac);

		//ALPHA

		//color.rgb = (color.rgb + refractionColor ) / 2;

		//color.rgb += tex2D(_MainTex, i.uv) / 3;
		color.rgb *= _MainTexColor.rgb;


#if !defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR) ||  !defined(SKIP_FLAT_SPECULAR)
		// FIXEDAngleVector *= MYFIXED3(0.7, 0.85, 1);

#if defined(APPLY_REFR_TO_SPECULAR) && defined(HAS_REFRACTION)
		//FIXEDAngleVector *= saturate(1 - lerped_refr);
		specularLight *= saturate( zdepth / 20 -  _APPLY_REFR_TO_SPECULAR_DISSOLVE);
#endif

#if defined(LIGHTING_BLEND_SIMPLE)
		color.rgb += FIXEDAngleVector + specularLight;
#elif  defined(LIGHTING_BLEND_COLOR)
		color.rgb += _BlendColor * (FIXEDAngleVector + specularLight);
#else
		color.rgb =  color.rgb * 0.7 + color.rgb * FIXEDAngleVector * 2 + specularLight;
#endif
#endif







#if defined(FOAM_BLEND_LUM)
		color.rgb = lerp(color.rgb, foamColor.rgb, saturate((foamColor.x + foamColor.y + foamColor.z) / 3 - _FoamBlendOffset));
#else
		color.rgb += foamColor;
#endif

		color.a = foamColor.a;
#include "EModules Water Model PostProcess.cginc"





		return color;
#ifdef WATER_DOWNSAMPLING_HARD
}
return 1;
#endif
#ifdef WATER_DOWNSAMPLING
}
else { return TEX2DGRAD(_FrameBuffer, fb_wcoord); }
return 1;
#endif
		}//!frag

	