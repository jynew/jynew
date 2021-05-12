// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'



//#include "EModules Water Model 2.0 uniform.cginc"



struct appdata {
	MYFLOAT4 vertex : POSITION;
	fixed3 normal : NORMAL;
	fixed4 color : COLOR;
	fixed4 tangent : TANGENT;
	MYFLOAT4 texcoord : TEXCOORD0;
};

struct v2f {
	MYFLOAT4 pos : SV_POSITION;
	MYFLOAT4 uv : TEXCOORD0; // xy - world ; zw - object
	MYFLOAT4 uvscroll : TEXCOORD1;
	half4 screen : TEXCOORD2;
	fixed4 wPos : TEXCOORD3; //xyz - wpos // w - distance to camera
	fixed4 VER_TEX : TEXCOORD4;// xyz - face tangent view ; w - local vertex y pos 
	fixed4 helpers : TEXCOORD5; //xy - inputnormals ; x - vertex alpha ; w - up sun factor
	fixed4 vfoam : TEXCOORD6; //z - sinuv ; y - cosuv ; z - stretched uv foam
	fixed4 fogCoord : TEXCOORD7;

/*#if !defined(SKIP_FOG)
#ifdef ORTO_CAMERA_ON
	fixed4 fogCoord : TEXCOORD7;
#else
	UNITY_FOG_COORDS(7)
#endif
#endif*/

#if defined(USE_VERTEX_OUTPUT_STEREO)
	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
#endif

};

#include "EModules Water Model Utils.cginc"




float2 INIT_COORDS(inout appdata v, inout v2f o) {

	o.wPos = mul(unity_ObjectToWorld, v.vertex);

	o.uv.zw = 1 - (v.vertex.xz - _VertexSize.xy) /( _VertexSize.zw );

	MYFLOAT TX = -(o.wPos.x / 1000 * _MainTex_ST.x + _MainTex_ST.z);
	MYFLOAT TY = -(o.wPos.z / 1000 * _MainTex_ST.y + _MainTex_ST.w);

	return fixed2(TX, TY);
}


v2f vert(appdata v)
{
	const float4 v4_one_2 = fixed4(-1, 1, 1, -1);
	const float3 v3_one = fixed3(1, 1, 1);
	const float4 v4_one = fixed4(1, 1, 1, 1);




	v2f o;
	UNITY_INITIALIZE_OUTPUT(v2f, o);


#if defined(USE_VERTEX_OUTPUT_STEREO)
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
#endif

	MYFLOAT2 texcoord = INIT_COORDS(v, o);


#if defined(SIN_OFFSET)
	texcoord.y += sin(texcoord.x *_sinFriq) * _sinAmount / 10;
#endif

	fixed3 n = v.normal;
#if !defined(SKIP_3DVERTEX_ANIMATION)
	VERTEX_MOVER(v, o, texcoord, n);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.wPos = mul(unity_ObjectToWorld, v.vertex);
	o.uv.zw = 1 - (v.vertex.xz - _VertexSize.xy) / (_VertexSize.zw);
#else 
	o.pos = UnityObjectToClipPos(v.vertex);
	o.VER_TEX.w = 0.2;
#endif

	fixed wd_v01 = distance(_WorldSpaceCameraPos.xyz, o.wPos.xyz);
	fixed3 wDir = (_WorldSpaceCameraPos.xyz - o.wPos.xyz) / wd_v01;
	o.helpers.xy = wDir.xy;
	o.vfoam.z = wDir.z;
	o.screen = ComputeScreenPos(o.pos);

	o.uv.xy = texcoord.xy;
#if !defined(SKIP_3DVERTEX_ANIMATION)
	//fixed2 f = o.vfoam.xy / _VertexSize.zw;
	float2 f = o.vfoam.xy * _ObjectScale.xz;
#if defined(HAS_ROTATION)
	float a_x = cos(_MainTexAngle);
	float a_y = sin(_MainTexAngle);
	f = float2(a_x, a_y) * f.x + float2(-a_y, a_x) * f.y;
#endif
	fixed _vuv = 0.005 * (1 - _VertexToUv);
	o.uv.x = o.uv.x - f.y * _vuv;
	o.uv.y = o.uv.y + f.x * _vuv;
#endif
	o.uv.xy *=  _WaterTextureTiling.xy;


#if defined(USE_VERTEX_OUTPUT_STEREO)
	COMPUTE_EYEDEPTH(o.screen.z);
#endif


	TANGENT_SPACE_ROTATION; 
	o.VER_TEX.xyz = normalize(mul(rotation, ObjSpaceViewDir(v.vertex))) ;
	o.VER_TEX.xy = o.VER_TEX.xy / _VertexSize.wz * 150 / _ObjectScale.xz;
	o.VER_TEX.xy = o.VER_TEX.xy / (o.VER_TEX.z* 0.8 + 0.2);
//	o.VER_TEX.z = sqrt(1 - o.VER_TEX.x * o.VER_TEX.x - o.VER_TEX.y * o.VER_TEX.y);
	o.helpers.z = max(0, v.color.a * 10 - 9);

	//fixed2 vOff = o.vfoam.zw / _VertexSize.wz;
	//float2 fixedUV = o.uv.xy + vOff.xy * _WaterTextureTiling.xy  ;
	//float2 fixedUV = o.uv.xy ;



#if defined(WAVES_MAP_CROSS)
	o.uvscroll = float4(
		dot(float3(o.uv.x, _Frac_UVS_DIR.x, _Frac_WaterTextureTilingTime_m_AnimMove.x), v3_one),
		dot(float3(o.uv.y, _Frac_UVS_DIR.y, _Frac_WaterTextureTilingTime_m_AnimMove.y), v3_one),
		dot(float4(o.uv.y, 0.5, _Frac_UVS_DIR.z, _Frac_WaterTextureTilingTime_m_AnimMove.y), v4_one_2),
		dot(float4(o.uv.x, 0.5, _Frac_UVS_DIR.w, _Frac_WaterTextureTilingTime_m_AnimMove.x), v4_one_2)
		);
#else
	o.uvscroll = float4(
		dot(float3(o.uv.x, _Frac_UVS_DIR.x, _Frac_WaterTextureTilingTime_m_AnimMove.x), v3_one),
		dot(float3(o.uv.y, _Frac_UVS_DIR.y, _Frac_WaterTextureTilingTime_m_AnimMove.y), v3_one),
		dot(float4(o.uv.x, 0.5, _Frac_UVS_DIR.z, _Frac_WaterTextureTilingTime_m_AnimMove.x), v4_one_2),
		dot(float4(o.uv.y, 0.5, _Frac_UVS_DIR.w, _Frac_WaterTextureTilingTime_m_AnimMove.y), v4_one_2)
		);
#endif


/*#if defined(WAVES_MAP_CROSS)
	o.uvscroll = MYFLOAT4(
		fixedUV.x + _Frac_UVS_DIR.x + _Frac_WaterTextureTilingTime_m_AnimMove.x,
		fixedUV.y + _Frac_UVS_DIR.y + _Frac_WaterTextureTilingTime_m_AnimMove.y,
		-fixedUV.y+0.5 + _Frac_UVS_DIR.z - _Frac_WaterTextureTilingTime_m_AnimMove.y,
		-fixedUV.x+0.5 + _Frac_UVS_DIR.w - _Frac_WaterTextureTilingTime_m_AnimMove.x
	);
#else
	o.uvscroll = MYFLOAT4(
		fixedUV.x + _Frac_UVS_DIR.x + _Frac_WaterTextureTilingTime_m_AnimMove.x,
		fixedUV.y + _Frac_UVS_DIR.y + _Frac_WaterTextureTilingTime_m_AnimMove.y,
		-fixedUV.x+0.5 + _Frac_UVS_DIR.z - _Frac_WaterTextureTilingTime_m_AnimMove.x,
		-fixedUV.y+0.5 + _Frac_UVS_DIR.w - _Frac_WaterTextureTilingTime_m_AnimMove.y
	);
#endif*/



#if !defined(SKIP_FLAT_SPECULAR)
	o.vfoam.w = normalize(lerp(fixed3(0.1, 0.28, 0.55), fixed3(0.1, 0.98, 0.05), _Light_FlatSpecTopDir));
#endif


	//MYFLOAT2 LowDistUv = o.uv.xy * _LOW_DISTOR_Tile + _Frac2PITime * floor(_LOW_DISTOR_Speed);
	MYFLOAT2 LowDistUv2 = texcoord.xy * 20 * _LOW_DISTOR_Tile + _Frac2PITime * floor(_LOW_DISTOR_Speed);
	o.vfoam.x = (sin(LowDistUv2.x))*0.1;
	o.vfoam.y = (cos(LowDistUv2.y))*0.1;


#if !defined(SKIP_FOG)
#ifdef ORTO_CAMERA_ON
	//o.fogCoord = v.vertex;
	fixed3 eyePos = UnityObjectToViewPos(v.vertex);
	o.fogCoord.x = length(eyePos);
#else
	UNITY_TRANSFER_FOG(o, o.pos);
#endif
#endif

#if defined(USE_BLENDANIMATED_MAINTEX)  && !defined(SKIP_MAIN_TEXTURE)
	MYFLOAT2 LowDistUv = texcoord.xy * _LOW_DISTOR_MAINTEX_Tile + _Frac2PITime * floor(_LOW_DISTOR_MAINTEX_Speed * 2);
	o.fogCoord.z = (sin(LowDistUv.x))*0.1;
	o.fogCoord.w = (cos(LowDistUv.y))*0.1;
#endif


#if !defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR) ||  !defined(SKIP_FLAT_SPECULAR)
	o.helpers.w = (1 - dot(fixed3(0, 1, 0), -_LightDir))*0.65 + 0.35;
#endif


	return o;
}//!vert








fixed4 frag(in v2f i) : SV_Target
{
		fixed3 tex;
	fixed zdepth;
	fixed raw_zdepth;
fixed2 tnormal_GRAB;
fixed2 tnormal_TEXEL;
fixed3 tnormal;
		fixed3 worldNormal;
		fixed rd;
		fixed3 wViewDir;
fixed2 DFACTOR;
float2 DETILEUV = i.uv.xy;
float4 UVSCROLL = i.uvscroll;
fixed4 color = fixed4(0, 0, 0, 1);
fixed APS;
#if defined(HAS_REFLECTION)
fixed3 reflectionColor;
#endif
#if defined(HAS_REFRACTION)
fixed3 refractionColor;
fixed lerped_refr;
#endif
fixed3 unionFog;

	#include "EModules Water Model Sampling.cginc"
#if defined(WRONG_DEPTH)
			fixed f = min(0.1, frac(i.screen.x / 10)) * 10;
		return fixed4(f, f, f, 1);
#endif



		fixed4 bump1 = (tex2D(_BumpMap, UVSCROLL.xy));
		fixed4 bump2 = (tex2D(_BumpMap, UVSCROLL.zw));
		//fixed4 mixed = (bump1 + bump2) *0.5;
		/*fixed4 mixed = lerp(bump1, bump2, 0.5);
		 tnormal = UnpackNormal(mixed);*/
		fixed3 tn1 = UnpackNormal(bump1);
		fixed3 tn2 = UnpackNormal(bump2);
		tnormal = lerp(tn1, tn2, 0.5);
		tnormal.xy *= _BumpAmount;



	

#if defined(UF_AMOUNTMASK)
#if defined(_UF_NMASK_USE_MAINTEX)
#define MASK_TEX _MainTex
#else
#define MASK_TEX _UF_NMASK_Texture
#endif

		MYFLOAT2 amtile = DETILEUV * _UF_NMASK_Tile + _UF_NMASK_offset;
#if defined(_UF_NMASK_G)
		fixed amm = tex2D(MASK_TEX, amtile).g;
#elif defined(_UF_NMASK_B)
		fixed amm = tex2D(MASK_TEX, amtile).b;
#elif defined(_UF_NMASK_A)
		fixed amm = tex2D(MASK_TEX, amtile).a;
#else
		fixed amm = tex2D(MASK_TEX, amtile).r;
#endif
		fixed amount_mask = saturate(amm * _UF_NMASK_Contrast - _UF_NMASK_Contrast  / 2 + _UF_NMASK_Brightnes);
#if defined(AMOUNT_MASK_DEBUG)
		return float4(amount_mask.rrr, 1);
#endif
		tnormal.xy *= amount_mask;
#endif

		 tnormal_TEXEL = tnormal.xy / _VertexSize.zw / _ObjectScale.xz * 2;
		 tnormal_GRAB = tnormal.xy;

		///////////////////////////////////////////////////



		///////////////////////////////////////////////////

		 wViewDir = fixed3(i.helpers.xy, i.vfoam.z);
		 worldNormal = tnormal.xzy;
		 worldNormal = normalize(worldNormal);

		///////////////////////////////////////////////////

	#if defined(DEPTH_NONE)
			  zdepth = 1;
	#else
	#if defined(HAS_CAMERA_DEPTH)
	#ifdef ORTO_CAMERA_ON
			fixed4 UV = i.screen;
	#else
			fixed4 UV = (i.screen);
	#endif
			 zdepth = GET_Z(i, UV);
			 raw_zdepth = zdepth;
	#else
			fixed2 UV = i.uv.zw;

			 zdepth = GET_BAKED_Z(UV);
			 raw_zdepth = zdepth;

	#if !defined(SKIP_Z_CALC_DEPTH_FACTOR)
			fixed av10 = saturate(zdepth / _RefrDeepFactor) ;
			 DFACTOR = i.VER_TEX.xy * av10;
#define HAS_DEPTH_FACTOR = 1;
			zdepth = GET_BAKED_Z_WITHOUTWRITE(UV - DFACTOR);
	#endif
	#endif

	#if defined(DEGUB_Z)
			return fixed4(zdepth / 10, zdepth / 10, zdepth / 10, 1);
	#endif
	#endif 

			///////////////////////////////////////////////////

#if defined(DEBUG_NORMALS)
			return float4(((tnormal.x + tnormal.y) * 3).xxx, 1);
#endif

			///////////////////////////////////////////////////

		//	fixed4 color = fixed4(0, 0, 0, 1);
/*#if !defined(REFLECTION_NONE)

#include "EModules Water Model Reflection.cginc"

#endif*/
#include "EModules Water Model Texture.cginc"
#ifdef WATER_DOWNSAMPLING_HARD
}
else { return tex2D(_FrameBuffer, fb_wcoord); }if (cond) {
#endif
			///////////////////////////////////////////////////


			///////////////////////////////////////////////////

	#include "EModules Water Model Refraction.cginc"
	#if defined(APPLY_REF)
			fixed3 apr01 = refractionColor;
#if defined(APPLY_REF_FOG)
			apr01 = lerp(apr01, unionFog,unionFog.b);
#endif

			//fixed apamount05 = _RefrTopAmount * (1 - lerped_refr) + _RefrDeepAmount * lerped_refr;
			apr01 *= lerp(_RefrTopAmount, _RefrDeepAmount, lerped_refr);


#if defined(REFRACTION_DEBUG_RGB)
			return float4(apr01.rgb, 1);
#endif


			fixed apr05 = 0;
#if defined(USE_REFRACTION_BLEND_FRESNEL) 
			apr05 += ((
#if !defined(SKIP_REFRACTION_BLEND_FRESNEL_INVERCE) 
				1 -
#endif
				wViewDir.y) * _RefractionBLendAmount);
			//return float4(apr05.rrr, 1);

#if defined(DEBUG_REFR_BLEND)
			return apr05;
#endif

			fixed3 apr15 = tex * _ReplaceColor;
			color.rgb = lerp(apr01, apr15, apr05);
#else
			color.rgb = apr01;
#endif


		
	#else
			color.rgb = tex * _ReplaceColor;
	#endif

			///////////////////////////////////////////////////

			color.rgb *= _MainTexColor.rgb;

			///////////////////////////////////////////////////


			///////////////////////////////////////////////////

			fixed fresnelFac = 1;
			fixed specularLight = 0;
	#include "EModules Water Model Lighting.cginc"
			color.rgb += specularLight;

	#if !defined(SKIP_LIGHTING) && !defined(MINIMUM_MODE)
			color.rgb += lightLight;
	#endif

			///////////////////////////////////////////////////

			//fixed4 foamColor = fixed4(0, 0, 0, 1);

	#if defined(SHORE_WAVES) && !defined(DEPTH_NONE)

	#include "EModules Water Model Shore.cginc"

	#endif

			///////////////////////////////////////////////////

	#include "EModules Water Model PostProcess.cginc"

			///////////////////////////////////////////////////


			return color;
#ifdef WATER_DOWNSAMPLING_HARD
}
return 1;
#endif
#ifdef WATER_DOWNSAMPLING
}
else { return tex2D(_FrameBuffer, fb_wcoord); }
return 1;
#endif
			//POST_PROCESS(i, color, foamColor, zdepth);
					   //color.rgb = zdepth / 20;

}//!frag

/*
// only compile Shader for platforms that can potentially
// do it (currently gles,gles3,metal)
#pragma only_renderers framebufferfetch


void frag(in v2f i, inout half4 ocol : SV_Target)// : SV_Target
{
	fixed sp = ((i.pos.x + _FrameRate) + _FrameRate) % 8;
	if (sp < 4) { 
		ocol = ocol;
		return; 
	}
	ocol = _frag(i);
}*/

