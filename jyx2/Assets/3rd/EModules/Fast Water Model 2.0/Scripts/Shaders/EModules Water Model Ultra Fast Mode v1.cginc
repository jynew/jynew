
struct appdata {
	float4 vertex : POSITION;
	fixed3 normal : NORMAL;
	fixed4 color : COLOR;
	fixed4 tangent : TANGENT;
	float4 texcoord : TEXCOORD0;
};

struct v2f {
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0; // xy - world ; zw - object
	float4 uvscroll : TEXCOORD1;
	half4 screen : TEXCOORD2;
	fixed4 wPos : TEXCOORD3; //xyz - wpos // w - distance to camera
	fixed4 VER_TEX : TEXCOORD4;// xyz - face tangent view ; w - local vertex y pos 
	fixed4 helpers : TEXCOORD5; //xy - inputnormals ; x - vertex alpha ; w - up sun factor
	fixed4 vfoam : TEXCOORD6; //xy - low distortion ; z - stretched uv foam ; w - flat spec
	fixed4 fogCoord : TEXCOORD7;//x - fog ; y - ... ; zw - low distortion main tex
/*#if !defined(SKIP_FOG)
#ifdef ORTO_CAMERA_ON
	fixed4 fogCoord : TEXCOORD7;
#else
	UNITY_FOG_COORDS(7)
#endif
#endif*/
		//fixed4 detileuv : TEXCOORD1;
		//fixed2 texcoord : TEXCOORD3;

		//fixed4 tspace0 : TEXCOORD4; // tangent.x, bitangent.x, normal.x, localnormal
		//fixed4 tspace1 : TEXCOORD5; // tangent.y, bitangent.y, normal.y, localnormal
		//fixed4 tspace2 : TEXCOORD6; // tangent.z, bitangent.z, normal.z, localnormal

#if defined(REFRACTION_GRABPASS)

		fixed4 grabPos : TEXCOORD8;
#endif


#if !defined(SKIP_BLEND_ANIMATION) && !defined(SHADER_API_GLES)
	fixed4 blend_index : TEXCOORD9;
	fixed4 blend_time : TEXCOORD10;
#endif

#if defined(HAS_USE_SHADOWS)
	SHADOW_COORDS(11)
#endif

#if defined(USE_VERTEX_OUTPUT_STEREO)
		UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
#endif

};

#include "EModules Water Model Utils.cginc"





float2 INIT_COORDS(inout appdata v, inout v2f o) {

	o.wPos = mul(unity_ObjectToWorld, v.vertex);

	o.uv.zw = 1 - (v.vertex.xz - _VertexSize.xy) /( _VertexSize.zw );

	float TX = -(o.wPos.x / 1000 * _MainTex_ST.x + _MainTex_ST.z);
	float TY = -(o.wPos.z / 1000 * _MainTex_ST.y + _MainTex_ST.w);


#if defined(HAS_ROTATION)
	float a_x = cos(_MainTexAngle);
	float a_y = sin(_MainTexAngle);

	//o.detileuv.z = a_x;
	//o.detileuv.w = a_y;

	return fixed2(a_x, a_y) * TX + fixed2(-a_y, a_x) * TY;
#else
	return fixed2(TX, TY);
#endif
}


v2f vert(appdata v)
{
	const float4 v4_one_2 = float4(-1, 1, 1, -1);
	const float3 v3_one = float3(1, 1, 1);
	const float4 v4_one = float4(1, 1, 1, 1);



	v2f o;
	UNITY_INITIALIZE_OUTPUT(v2f, o);


#if defined(USE_VERTEX_OUTPUT_STEREO)
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	//UNITY_SETUP_INSTANCE_ID(i); frag
#endif

	float2 texcoord = INIT_COORDS(v, o);

#if !defined(SKIP_BLEND_ANIMATION) && !defined(SHADER_API_GLES)
	BLEND_ANIMATION(o);
#endif

#if defined(SIN_OFFSET)
	texcoord.y += sin(texcoord.x *_sinFriq) * _sinAmount / 10;
#endif

	fixed3 n = v.normal;
#if !defined(SKIP_3DVERTEX_ANIMATION)
	VERTEX_MOVER(v, o, texcoord, n);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.wPos = mul(unity_ObjectToWorld, v.vertex);
	o.uv.zw = 1 - (v.vertex.xz - _VertexSize.xy) / (_VertexSize.zw);
	//texcoord = INIT_COORDS(v, o);

#else 
	o.pos = UnityObjectToClipPos(v.vertex);
	o.VER_TEX.w = 0.2;
#endif

#if defined(HAS_USE_SHADOWS)
	TRANSFER_SHADOW(o)
#endif

	o.wPos.w = distance(_WorldSpaceCameraPos.xyz, o.wPos.xyz);

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



	o.uv.xy  *= _WaterTextureTiling.xy;

#if defined(USE_VERTEX_OUTPUT_STEREO)
	COMPUTE_EYEDEPTH(o.screen.z);
#endif
#if defined(USE_LIGHTMAPS)
	//o._utils.zw = v.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
#if defined(REFRACTION_GRABPASS)
	o.grabPos = ComputeGrabScreenPos(o.pos);
#endif


	TANGENT_SPACE_ROTATION; // Unity macro that creates a rotation matrix called "rotation" that transforms vectors from object to tangent space.
	o.VER_TEX.xyz = normalize(mul(rotation, ObjSpaceViewDir(v.vertex))) ; // Get tangent space view dir from object space view dir.
	o.VER_TEX.xy = o.VER_TEX.xy / _VertexSize.zw * 150 / _ObjectScale.xz;
	o.VER_TEX.xy = o.VER_TEX.xy / (o.VER_TEX.z* 0.8 + 0.2);
	//	o.VER_TEX.z = sqrt(1 - o.VER_TEX.x * o.VER_TEX.x - o.VER_TEX.y * o.VER_TEX.y);
	//o.helpers.z = saturate(v.color.a * 100 - 5);
	o.helpers.z = v.color.a;
	//fixed3 wViewDir = normalize(UnityWorldSpaceViewDir(o.wPos));
	//o.valpha.yzw = normalize((-(_LightDir)+(wViewDir)));
	//o.wViewDir = normalize(UnityWorldSpaceViewDir(o.wPos));


	//o.tspace.xy = fixed2(cos(_ObjecAngle), sin(_ObjecAngle));

	/*fixed3 wNormal = UnityObjectToWorldNormal(n);

	fixed3 wTangent = v.tangent.xyz;

	fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
	fixed3 wBitangent = cross(wNormal, wTangent) * tangentSign;


	o.tspace0.xyz = fixed3(wTangent.x, wBitangent.x, wNormal.x);
	o.tspace1.xyz = fixed3(wTangent.y, wBitangent.y, wNormal.y);
	o.tspace2.xyz = fixed3(wTangent.z, wBitangent.z, wNormal.z);

	o.tspace0.w = n.x;
	o.tspace1.w = n.y;
	o.tspace2.w = n.z;*/
	o.helpers.xy = n.xz;

	/*	fixed SCSPDF_DIV = 1.4;

#if defined(WAVES_MAP_CROSS)
	fixed4 UVS_DIR = (fixed4(
		(_FracTimeX / 2 + _CosTime.x * 0.02)	* _WaterTextureTiling.z,
		(_FracTimeX / 2 + _SinTime.x * 0.03)	* _WaterTextureTiling.w ,
		(_FracTimeX / 2 + _SinTime.x * 0.05 + 0.5)	* _WaterTextureTiling.z  / SCSPDF_DIV,
		(_FracTimeX / 2 + _CosTime.x * 0.04 + 0.5)* _WaterTextureTiling.w  / SCSPDF_DIV
	)1);
#else
	fixed4 UVS_DIR = (fixed4(\
		(_FracTimeX / 2 + _CosTime.x * 0.04)	* _WaterTextureTiling.z ,
		(_FracTimeX / 2 + _SinTime.x * 0.05)	* _WaterTextureTiling.w ,
		(_FracTimeX / 2 + _CosTime.x * 0.02 + 0.5)	* _WaterTextureTiling.z  / SCSPDF_DIV,
		(_FracTimeX / 2 + _SinTime.x * 0.03 + 0.5) * _WaterTextureTiling.w  / SCSPDF_DIV
	));
#endif*/
	// _FracTimeFull * _AnimMove.x
	// _FracTimeFull * _AnimMove.y

	

#if defined(WAVES_MAP_CROSS)
	o.uvscroll = float4(
	dot(float3(	o.uv.x , _Frac_UVS_DIR.x , _Frac_WaterTextureTilingTime_m_AnimMove.x), v3_one),
	dot(float3(	o.uv.y , _Frac_UVS_DIR.y , _Frac_WaterTextureTilingTime_m_AnimMove.y), v3_one),
	dot(float4(	o.uv.y , 0.5 , _Frac_UVS_DIR.z , _Frac_WaterTextureTilingTime_m_AnimMove.y), v4_one_2),
	dot(float4(	o.uv.x , 0.5 , _Frac_UVS_DIR.w , _Frac_WaterTextureTilingTime_m_AnimMove.x), v4_one_2)
		);
#else
	o.uvscroll = float4(
		dot(float3(o.uv.x, _Frac_UVS_DIR.x, _Frac_WaterTextureTilingTime_m_AnimMove.x), v3_one),
		dot(float3(o.uv.y, _Frac_UVS_DIR.y, _Frac_WaterTextureTilingTime_m_AnimMove.y), v3_one),
		dot(float4(o.uv.x, 0.5, _Frac_UVS_DIR.z, _Frac_WaterTextureTilingTime_m_AnimMove.x), v4_one_2),
		dot(float4(o.uv.y , 0.5 , _Frac_UVS_DIR.w , _Frac_WaterTextureTilingTime_m_AnimMove.y), v4_one_2)
		);
#endif
/*#if defined(WAVES_MAP_CROSS)
	o.uvscroll = float4(
		o.uv.x + _Frac_UVS_DIR.x + _Frac_WaterTextureTilingTime_m_AnimMove.x,
		o.uv.y + _Frac_UVS_DIR.y + _Frac_WaterTextureTilingTime_m_AnimMove.y,
		-o.uv.y+0.5 + _Frac_UVS_DIR.z - _Frac_WaterTextureTilingTime_m_AnimMove.y,
		-o.uv.x+0.5 + _Frac_UVS_DIR.w - _Frac_WaterTextureTilingTime_m_AnimMove.x
	);
#else
	o.uvscroll = float4(
		o.uv.x + _Frac_UVS_DIR.x + _Frac_WaterTextureTilingTime_m_AnimMove.x,
		o.uv.y + _Frac_UVS_DIR.y + _Frac_WaterTextureTilingTime_m_AnimMove.y,
		-o.uv.x+0.5 + _Frac_UVS_DIR.z - _Frac_WaterTextureTilingTime_m_AnimMove.x,
		-o.uv.y+0.5 + _Frac_UVS_DIR.w - _Frac_WaterTextureTilingTime_m_AnimMove.y
	);
#endif*/


	/*fixed4 UVS_DIR = o.uvscroll;
	fixed2 UVS_SPD = fixed2(_WaterTextureTiling.z, _WaterTextureTiling.w);
	fixed4 UVSCROLL;
	UVS(o.uv, UVS_DIR, UVS_SPD, UVSCROLL)
		o.uvscroll = (UVSCROLL);*/

#if defined(DETILE_LQ)
	MYFIXED DX = (sin((o.uv.x + o.uv.y)* _DetileFriq + _Frac2PITime)) * _DetileAmount;
	MYFIXED DY = (sin(o.uv.y * _DetileFriq + _Frac2PITime)) * _DetileAmount;
	o.uv.x += DX;
	o.uv.y += DY;
	o.uvscroll.x += DX;
	o.uvscroll.y += DY;
#if defined(DETILE_SAME_DIR)
	o.uvscroll.z -= DX;
	o.uvscroll.w -= DY;
#else
	o.uvscroll.z += DX;
	o.uvscroll.w += DY;
#endif
#endif





#if defined(SHORE_WAVES) && !defined(DEPTH_NONE)  

#if (defined(SHORE_UNWRAP_STRETCH_1) || defined(SHORE_UNWRAP_STRETCH_2)) && !defined(MINIMUM_MODE)
	/*fixed point_uvw = v.vertex.xz;
	fixed uvw = distance(point_uvw, v.vertex.xz);
	point_uvw.x += _VertexSize.z;
	fixed uvw += distance(point_uvw, v.vertex.xz);
	(v.vertex.xz - _VertexSize.xy)  _VertexSize.z;*/
	fixed3 point_uvw = _VertexSize;
	//point_uvw.y += _VertexSize.w;
	//o.vfoam.z = 0; //v.vertex.x - _VertexSize.x + v.vertex.z - _VertexSize.y + distance(point_uvw, v.vertex);
	//o.vfoam.z += distance(point_uvw, v.vertex);
	point_uvw.x += _VertexSize.z;
	//o.vfoam.z += distance(point_uvw, v.vertex);
	point_uvw.z += _VertexSize.w;
	//o.vfoam.z += distance(point_uvw, v.vertex);
	point_uvw.x -= _VertexSize.z;
	//o.vfoam.z += distance(point_uvw, v.vertex);
	//o.vfoam.z /= 3;
	

#if defined(SHORE_UNWRAP_STRETCH_DOT) || defined(SHORE_UNWRAP_STRETCH_OFFSETDOT)
	fixed sincor = 4 * sin(length((v.vertex - point_uvw) / 4));
#if defined(SHORE_UNWRAP_STRETCH_OFFSETDOT)
	fixed dot1 = dot(normalize(v.vertex.xz-_VertexSize.xy*2), fixed2(0, 1));
	fixed dot2 = dot(normalize(v.vertex.xz-_VertexSize.xy*2), fixed2(1, 0));
	fixed dot3 = dot(normalize(v.vertex.xz-_VertexSize.xy*2), fixed2(1, 1));
#else
	fixed dot1 = dot(normalize(v.vertex.xz), fixed2(0, 1));
	fixed dot2 = dot(normalize(v.vertex.xz), fixed2(1, 0));
	fixed dot3 = dot(normalize(v.vertex.xz), fixed2(1, 1));
#endif
	dot1 = abs(dot1);
	dot2 = abs(dot2);
	dot3 = abs(dot3);
	//dot1 += dot3;
	//dot2 += dot3;
	//o.vfoam.z = (v.vertex.x - _VertexSize.x) * dot1+ (v.vertex.z - _VertexSize.y) * dot2+ sincor;
	o.vfoam.z = dot(fixed3( (v.vertex.x - _VertexSize.x) * dot1 , (v.vertex.z - _VertexSize.y) * dot2 , sincor), v3_one);
#else 
	fixed sincor = 4 * sin(length((v.vertex - point_uvw) / 4));
	fixed d1 = length(v.vertex.xz - _VertexSize.xy);
	//fixed d2 = length(v.vertex.xz - _VertexSize.xy + _VertexSize.zw);

	//o.vfoam.z = v.vertex.x + v.vertex.z + sincor + d1 - _VertexSize.x - _VertexSize.y;
	o.vfoam.z = dot(fixed4( v.vertex.x , v.vertex.z  , sincor , d1), v4_one) - _VertexSize.x - _VertexSize.y;
#endif
#endif //defined(SHORE_UNWRAP_STRETCH_1) || defined(SHORE_UNWRAP_STRETCH_2)


#endif

#if !defined(SKIP_FLAT_SPECULAR)
	o.vfoam.w = normalize( lerp(fixed3(0.1, 0.28, 0.55), fixed3(0.1, 0.98, 0.05), _Light_FlatSpecTopDir));
#endif

	//MYFLOAT2 LowDistUv = o.uv.xy * _LOW_DISTOR_Tile + _Frac2PITime * floor(_LOW_DISTOR_Speed);
	MYFLOAT2 LowDistUv2 = texcoord.xy *20 * _LOW_DISTOR_Tile + _Frac2PITime * floor(_LOW_DISTOR_Speed);
	o.vfoam.x = (sin(LowDistUv2.x))*0.1;
	o.vfoam.y = (cos(LowDistUv2.y))*0.1;
	/*o.vfoam.x += cos(LowDistUv2.x / 35 + o.vfoam.x * 5)*0.1;
	o.vfoam.y += sin(LowDistUv2.y / 53 + o.vfoam.y * 5)*0.1;*/

#if !defined(SKIP_FOG)
#ifdef ORTO_CAMERA_ON
	//o.fogCoord = v.vertex;
	fixed3 eyePos = UnityObjectToViewPos(v.vertex);
	o.fogCoord.x = length(eyePos);
#else
	UNITY_TRANSFER_FOG(o, o.pos);
#endif
#endif

#if defined(USE_BLENDANIMATED_MAINTEX) && !defined(SKIP_MAIN_TEXTURE)
	MYFLOAT2 LowDistUv = texcoord.xy * _LOW_DISTOR_MAINTEX_Tile + _Frac2PITime * floor(_LOW_DISTOR_MAINTEX_Speed * 2);
	o.fogCoord.z = (sin(LowDistUv.x))*0.1;
	o.fogCoord.w = (cos(LowDistUv.y))*0.1;
#endif
	//o.vfoam.x = (sin(o.uv.x * _LOW_DISTOR_Tile + _Frac2PITime * floor(_LOW_DISTOR_Speed)))*0.1;
	//o.vfoam.y = (cos(o.uv.y * _LOW_DISTOR_Tile + _Frac2PITime * floor(_LOW_DISTOR_Speed)))*0.1;

	//o.fogCoord.xyzw = 0;

#if !defined(SKIP_LIGHTING) ||  !defined(SKIP_SPECULAR) ||  !defined(SKIP_FLAT_SPECULAR)
	o.helpers.w = (1 - dot(fixed3(0, 1, 0), -_LightDir))*0.65 + 0.35;
#endif


	/*fixed4 loduv;
	loduv.xy = o.uv.xy / 8;
		loduv.zw = 1.0;
	fixed msk = tex2Dlod(_MainTex, loduv).r;


	fixed fr = frac((_FracTimeXX) * 4 + msk * 2 + o.uv.x + o.uv.y) * 2;
	fixed time = abs(1 - fr);
	o.valpha.z = time;*/
	//time = frac(1 +  time * time *time);
	//	



	return o;
}//!vert














	// , UNITY_VPOS_TYPE screenPos : VPOS
fixed4 frag(v2f i) : SV_Target
{
	fixed3 tex;
	fixed zdepth;
	fixed raw_zdepth;
fixed2 tnormal_GRAB;
fixed2 tnormal_TEXEL;
fixed3 tnormal;
		fixed3 worldNormal;
fixed3 wViewDir;
fixed2 DFACTOR;	
fixed rd;
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

		/*i.uv.x += (sin(i.uv.x * 5 + i.uv.y * 5)) / _WaterTextureTiling.x / 2;
		i.uv.y += (sin(i.uv.y * 5)) / _WaterTextureTiling.y / 2;*/
		

		///////////////////////////////////////////////////

		//i.vfoam.x = (sin(i.uv.x * 20 + _Frac2PITime * 8))*0.1;
		//i.vfoam.y = (cos(i.uv.y * 20 + _Frac2PITime * 8))*0.1;
		///////////////////////////////////////////////////

#if defined(SKIP_BLEND_ANIMATION) || defined(SHADER_API_GLES)
#if defined(USE_4X_BUMP)
		fixed4 bump1A = (tex2D(_BumpMap, UVSCROLL.wx));
		fixed3 tnormal1A = UnpackNormal(bump1A);
		fixed4 bump2A = (tex2D(_BumpMap, UVSCROLL.yz));
		fixed3 tnormal2A = UnpackNormal(bump2A);
#endif

		fixed4 bump1 = (tex2D(_BumpMap, UVSCROLL.xy));
		fixed3 tnormal1 = UnpackNormal(bump1);
		fixed4 bump2 = (tex2D(_BumpMap, UVSCROLL.zw));
		fixed3 tnormal2 = UnpackNormal(bump2);

#if defined(USE_4X_BUMP)
		//tnormal1 = (tnormal1 + tnormal1A)* 0.5;
		tnormal1 = lerp(tnormal1, tnormal1A, 0.5);
		//tnormal2 = (tnormal2 + tnormal2A)* 0.5;
		tnormal2 = lerp(tnormal2, tnormal2A, 0.5);
#endif

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
#endif


#if !defined(SKIP_WNN)
		//tnormal1 = normalize(tnormal1);
		//tnormal2 = normalize(tnormal2);
#endif

		//return 1-sum/50;
		//fixed3 avt2 = (tnormal1 + tnormal2) * 0.5;
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
		//return av15;
			//av15 = saturate(av15);
	
		tnormal.xy *= _BumpAmount;



/*#if defined(UF_AMOUNTMASK)
		float2 amtile = DETILEUV * _UF_NMASK_Tile + _UF_NMASK_offset;
		fixed amm = tex2D(_UF_NMASK_Texture, amtile).r;
		fixed amount_mask = saturate(amm * _UF_NMASK_Contrast - _UF_NMASK_Contrast  / 2 + _UF_NMASK_Brightnes);
		//fixed amount_mask = min(_AMOUNTMASK_max, (tex2D(_UF_AMountMaskTexture, DETILEUV * _AMOUNTMASK_Tile + _AMOUNTMASK_offset).r * _AMOUNTMASK_Amount + _AMOUNTMASK_min));
#if defined(AMOUNT_MASK_DEBUG)
		return float4(amount_mask.rrr, 1);
#endif
		tnormal.xy *= amount_mask;
#endif*/
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
		fixed amount_mask = saturate(amm * _UF_NMASK_Contrast - _UF_NMASK_Contrast / 2 + _UF_NMASK_Brightnes);
#if defined(AMOUNT_MASK_DEBUG)
		return float4(amount_mask.rrr, 1);
#endif
		tnormal.xy *= amount_mask;
#endif



		//fixed3 inputNormal = fixed3(i.tspace0.w, i.tspace2.w, i.tspace1.w);
		//tnormal.xy += inputNormal.xy;

		//tnormal = normalize(tnormal);
		//fixed3 rawtnormal = normalize(tnormal);
		 tnormal_TEXEL = tnormal.xy / _VertexSize.zw / _ObjectScale.xz * 2 ;
		 tnormal_GRAB = tnormal.xy;

		///////////////////////////////////////////////////



		///////////////////////////////////////////////////

//#define WVIEW normalize( i.wPos.xyz - _WorldSpaceCameraPos.xyz)
		 wViewDir = (_WorldSpaceCameraPos.xyz - i.wPos.xyz) / i.wPos.w;

		//worldNormal.xz = tnormal.x * i.tspace.xy + tnormal.y * fixed2(-i.tspace.y, i.tspace.x);
		worldNormal.xz = tnormal.xy + i.helpers.xy;
		worldNormal.y = tnormal.z;
		/*	fixed3 worldNormal;
			worldNormal.x = dot(i.tspace0.xyz, tnormal);
			worldNormal.y = dot(i.tspace1.xyz, tnormal);
			worldNormal.z = dot(i.tspace2.xyz, tnormal);*/
			//fixed2 tspace = fixed2(1, 0);
	#if !defined(SKIP_WNN)
			worldNormal = normalize(worldNormal);
	#endif
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
#if defined(FIX_DISTORTION)
			fixed before_zdepth_raw = GET_Z(i, UV);
#endif

#if defined(USE_ZD_DISTOR)
			fixed dv01 = _RefrDistortionZ * 30;
			UV.xy += tnormal_TEXEL * dv01;
#endif

#if defined(FIX_DISTORTION)
			 zdepth = max(before_zdepth_raw, GET_Z(i, UV));
#else
			 zdepth = GET_Z(i, UV);
#endif
			 raw_zdepth = zdepth;



	#else
			fixed2 UV = i.uv.zw;
/*#if defined(FIX_DISTORTION)
			fixed before_zdepth_raw = GET_BAKED_Z(i, UV);
#endif*/
#if defined(USE_ZD_DISTOR)
			fixed dv01 = _RefrDistortionZ * 0.3;
			UV.xy += tnormal_TEXEL * dv01;
#endif

			 zdepth = GET_BAKED_Z(UV);
			 raw_zdepth = zdepth;

	#if !defined(SKIP_Z_CALC_DEPTH_FACTOR)
			/*fixed av10 = saturate(zdepth / _RefrDeepFactor) / (i.VER_TEX.z* 0.8 + 0.2);
			fixed2 DFACTOR = i.VER_TEX.xy * av10;*/
			fixed av10 = saturate(zdepth / _RefrDeepFactor);
			 DFACTOR = i.VER_TEX.xy * av10;
#define HAS_DEPTH_FACTOR = 1;
			zdepth = GET_BAKED_Z_WITHOUTWRITE(UV - DFACTOR);
	#endif

	#endif


/*#if defined(HAS_CAMERA_DEPTH)
			MYFIXED zdepth = GET_Z(i, (UV));
#if defined(HAS_REFRACTION) && !defined(SKIP_ZDEPTH_REFRACTION_DISTORTIONCORRECTION)
			zdepth = max(before_zdepth_raw, zdepth);
#endif
#else

#if defined(HAS_REFRACTION) && !defined(SKIP_ZDEPTH_REFRACTION_DISTORTIONCORRECTION)
			MYFIXED zdepth = max(before_zdepth_raw, GET_BAKED_Z(UV));
#else
			MYFIXED zdepth = GET_BAKED_Z(UV);
#endif

#endif*/



	#if defined(DEGUB_Z)
			return fixed4(zdepth / 10, zdepth / 10, zdepth / 10, 1);
	#endif
	#endif 


			///////////////////////////////////////////////////

#if defined(HAS_Z_AFFECT_BUMP)
			fixed zaff = saturate((zdepth - _BumpZFade) / _BumpZOffset + _BumpZFade);
#if defined(INVERT_Z_AFFECT_BUMP)
			zaff = 1 - zaff;
#endif
			tnormal.xy *= zaff;
			tnormal_GRAB.xy *= zaff;
			tnormal_TEXEL.xy *= zaff;
			//worldNormal.y += (worldNormal.x + worldNormal.z) * zaff * 3;
			worldNormal.xz *= zaff;
			worldNormal = normalize(worldNormal);
#endif
#if defined(DEBUG_NORMALS)
			return float4(((tnormal.x + tnormal.y) * 3).xxx,1);
#endif


			///////////////////////////////////////////////////
//#ifdef WATER_DOWNSAMPLING

//#endif
#include "EModules Water Model Refraction.cginc"
#include "EModules Water Model Texture.cginc"

			///////////////////////////////////////////////////

#ifdef WATER_DOWNSAMPLING_HARD
}else {return tex2D(_FrameBuffer, fb_wcoord);}if (cond){
#endif
 //{//128 instructions


			///////////////////////////////////////////////////

	#if defined(APPLY_REF)
			fixed3 apr01 = refractionColor;
#if defined(APPLY_REF_FOG)
			apr01 = lerp(apr01, unionFog,unionFog.b);
#endif

			//fixed apamount05 =  _RefrTopAmount + (_RefrDeepAmount - _RefrTopAmount) * lerped_refr;
			apr01 *= lerp(_RefrTopAmount, _RefrDeepAmount, lerped_refr);


#if defined(REFRACTION_DEBUG_RGB)
			return float4(apr01.rgb, 1);
#endif

#if !defined(SKIP_TEX)
			fixed apr05 = (tex.b / 2 + tex.b * MAIN_TEX_Contrast * abs(tnormal.x));
			//if (apr05 < 0.1) return 1;

			//fixed apr05 = tex.b ;
#else
			fixed apr05 = 0;
#endif

#if defined(USE_REFRACTION_BLEND_FRESNEL) 
			apr05 += ((
#if !defined(SKIP_REFRACTION_BLEND_FRESNEL_INVERCE) 
				1 -
#endif
				wViewDir.y) * _RefractionBLendAmount);

#if defined(DEBUG_REFR_BLEND)
			return apr05;
#endif
#endif
			//return float4(_ReplaceColor.rgb, 1);
			fixed3 apr15 = tex * _ReplaceColor;
			color.rgb = lerp(apr01, apr15, apr05);

			//apr05 = (apr05); //#saturate to fix
			//return float4(apr05.rrr, 1);
#if !defined(REFLECTION_NONE)
//#include "EModules Water Model Reflection.cginc"
		//	apr15 *= reflectionColor;
#endif


	#else
			color.rgb = tex * _ReplaceColor;
#endif

			///////////////////////////////////////////////////

			color.rgb *= _MainTexColor.rgb;
			///////////////////////////////////////////////////

	#if !defined(REFLECTION_NONE)

	#include "EModules Water Model Reflection.cginc"

	#endif


			///////////////////////////////////////////////////

	#if defined(SHORE_WAVES) && !defined(DEPTH_NONE)

	#include "EModules Water Model Shore.cginc"

	#endif
			fixed fresnelFac = 1;
			fixed specularLight = 0;
	#include "EModules Water Model Lighting.cginc"
			color.rgb +=  specularLight;
	#if !defined(SKIP_LIGHTING) 
			color.rgb += lightLight;
	#endif

			///////////////////////////////////////////////////

			//fixed4 foamColor = fixed4(0, 0, 0, 1);


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

