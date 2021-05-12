#define ORTO_PROJ_DIVIDER 100
#define Pi 3.1415926535897932384626433832795


#if  !defined(ULTRA_FAST_MODE) && !defined(MINIMUM_MODE)

#define SCSPDF 1.4

#if defined(WAVES_MAP_CROSS)
#define UVS(sourceuv, scrolldir, scrollsprrd, result) \
	result = float4( \
		sourceuv.x + scrolldir.x	* scrollsprrd.x + _FracTimeFull * _AnimMove.x, \
		sourceuv.y + scrolldir.y	* scrollsprrd.y + _FracTimeFull * _AnimMove.y, \
		sourceuv.y+0.5 - scrolldir.z	* scrollsprrd.x /SCSPDF+ _FracTimeFull * _AnimMove.y, \
		sourceuv.x+0.5 - scrolldir.w	* scrollsprrd.y/SCSPDF + _FracTimeFull * _AnimMove.x \
	);
#else
#define UVS(sourceuv, scrolldir, scrollsprrd, result) \
	result = float4( \
		sourceuv.x +scrolldir.x		* scrollsprrd.x + _FracTimeFull * _AnimMove.x, \
		sourceuv.y +scrolldir.y		* scrollsprrd.y + _FracTimeFull * _AnimMove.y, \
		sourceuv.x+0.5 -scrolldir.z		* scrollsprrd.x/SCSPDF + _FracTimeFull * _AnimMove.x, \
		sourceuv.y+0.5 -scrolldir.w     * scrollsprrd.y/SCSPDF + _FracTimeFull * _AnimMove.y \
	);
#endif
#if defined(WAVES_MAP_CROSS)
#define UVS2X(sourceuv, scrolldir, scrollsprrd, result) \
	result = float2( \
		sourceuv.x + scrolldir.x	* scrollsprrd.x +_FracTimeFull *  _AnimMove.x, \
		sourceuv.y + scrolldir.y	* scrollsprrd.y +_FracTimeFull *  _AnimMove.y \
	);
#else
#define UVS2X(sourceuv, scrolldir, scrollsprrd, result) \
	result = float2( \
		sourceuv.x +scrolldir.x		* scrollsprrd.x +_FracTimeFull *  _AnimMove.x, \
		sourceuv.y +scrolldir.y		* scrollsprrd.y +_FracTimeFull *  _AnimMove.y \
	);
#endif
#if defined(WAVES_MAP_CROSS)
#define UVS2Z(sourceuv, scrolldir, scrollsprrd, result) \
	result = float2( \
		sourceuv.y+0.5 - scrolldir.z	* scrollsprrd.x/SCSPDF + _FracTimeFull * _AnimMove.y, \
		sourceuv.x+0.5 - scrolldir.w	* scrollsprrd.y/SCSPDF  +_FracTimeFull *  _AnimMove.x\
	);
#else
#define UVS2Z(sourceuv, scrolldir, scrollsprrd, result) \
	result = float2( \
		sourceuv.x +0.5-scrolldir.z		* scrollsprrd.x/SCSPDF +_FracTimeFull *  _AnimMove.x, \
		sourceuv.y +0.5-scrolldir.w     * scrollsprrd.y/SCSPDF +_FracTimeFull *  _AnimMove.y \
	);
#endif






fixed GET_WAVES(v2f i, fixed dir1, fixed TILE, fixed TS, fixed2 UV)
{
	fixed n;
	fixed time = _Time.y * TS;


	fixed T_OFF = (cos(time + (UV.x + UV.y / 2) * TILE) * 0.5 + 0.5) * 0.4 + 0.8;
	fixed dir2 = dir1 * T_OFF * 0.1;

	n = sin(dir2 + time * 2 + (UV.x + UV.y) * TILE * TS / 2) / 2 + 0.5;
	n = 1 - n * n / 1.2;
	//	fixed = 

	//n *= sin(dir1*1.2 + _Time.w * 3 ) / 2 + 0.5;
	fixed m = sin(dir1 * 2 + time * 2) / 2 + 0.3;

	return 1 - n * m;
}




fixed GET_NOISE(v2f i, fixed _tile, fixed speed, fixed2 UV) {

	fixed tile = 50 * _tile;
	fixed tile2 = 50 * _tile;

	fixed2 uv = UV * tile2;
	fixed dir1 = uv.x + uv.y / 2;
	uv *= 1.4;
	fixed dir2 = uv.x + uv.y / 2.1;
	fixed dir3 = uv.x + uv.y / 2.3;
	uv *= 1.4;
	fixed dirS = uv.x * 3 + uv.y / 1.9;

	fixed w1 = GET_WAVES(i, dir1, tile, 1 * speed, UV);
	fixed w2 = GET_WAVES(i, dir2, tile*.2, 1.2 * speed, UV);
	fixed w3 = GET_WAVES(i, dir3, tile * 3, 0.8 * speed, UV);

	fixed S = sin(dirS*0.3 + _Time.y * 3 * speed) / 2 + 1.3;

	//return S;
	return w1 * w2 * w3 * S / 1.2;

}

#endif



#if !defined(SKIP_BLEND_ANIMATION) && !defined (SHADER_API_GLES) && !defined(MINIMUM_MODE)
void BLEND_ANIMATION(inout v2f o)
{
	//fixed2 uv_offset = fixed2(0.3125, 0.3125) / 2;
	fixed anim_time = _Frac01Time_d8_mBlendAnimSpeed * 64;
	fixed index1 = floor(anim_time) * 4;
	fixed index2 = floor(anim_time + 0.25) * 4;
	fixed index3 = floor(anim_time + 0.5) * 4;
	fixed index4 = floor(anim_time + 0.75) * 4;
	anim_time = frac(anim_time);

	fixed l1 = saturate(abs(0.5 - anim_time) * 2);
	fixed l2 = saturate(abs(0.25 - anim_time) * 2) - saturate(anim_time - 0.75) * 2;


#if defined(SMOOTH_BLEND_ANIMATION)

	fixed l3 = 1 - (l1)*(l1);
	fixed l4 = 1 - (l2)*(l2);
	l1 = 1 - (1 - l1)*(1 - l1);
	l2 = 1 - (1 - l2)*(1 - l2);
	fixed sum = (l1 + l2 + l3 + l4) / 2;
	l1 /= sum;
	l2 /= sum;
	l3 /= sum;
	l4 /= sum;
#else
	fixed l3 = 1 - l1;
	fixed l4 = 1 - l2;
#endif
	
	fixed uv_offset = 0.3125 / 2;
	o.blend_index = fixed4(index1 + 2, index2 + 3, index3, index4 + 1) * uv_offset;
	o.blend_time = fixed4(l1, l2, l3, l4);
}

#endif


#if defined(WAVES_GERSTNER)
/*fixed3 mister_gerstner(fixed2 xzVtx)
{
	fixed3 nrml = fixed3(0, 0, 0);

	nrml.x -=
		_GDirectionAB.x * (_GAmplitude.x * _GFrequency.x) *
		cos(_GFrequency.x * dot(_GDirectionAB, xzVtx) + _GSpeed.x * _Time.x);

	nrml.z -=
		_GDirectionAB.y * (_GAmplitude.y * _GFrequency.y) *
		cos(_GFrequency.y * dot(_GDirectionAB, xzVtx) + _GSpeed.y * _Time.x);

	return nrml;
}*/

/*fixed3 mister_gerstner(fixed2 uv)
{
	fixed3 normal = fixed3(0, 1, 0);
	fixed numWaves = 4;
	[unroll]
	for (float i = 0; i < numWaves; i++)
	{
		fixed2 dir = lerp(_GDirectionAB.xy, _GDirectionAB.zw, i / (numWaves - 1));

		fixed wi = 2 / _GFrequency[i];
		fixed WA = wi * _GAmplitude[i];
		fixed phi = _GSpeed[i] * wi;
		fixed rad = wi * dot(dir, uv) + phi * _Time.x;
		fixed Qi = _GSteepness[i] / (_GAmplitude[i] * wi * numWaves);
		normal.xz -= normalize(dir) * WA * cos(rad);
		normal.y -= Qi * WA * sin(rad);
	}
	return normalize(normal);
}*/
/*
fixed3 mister_gerstner(fixed2 xzVtx)
{

	fixed3 result = fixed3(0, 0, 0);

	fixed3 normal = fixed3(0, 1, 0);
	fixed numWaves = 2;
	[unroll]
	for (float i = 0; i < numWaves; i++)
	{
		fixed3 nrml = fixed3(0, 2.0, 0);

		fixed4 AB = _GFrequency.xxyy * _GAmplitude.xxyy * _GDirectionAB.xyzw;
		fixed4 CD = _GFrequency.zzww * _GAmplitude.zzww * _GDirectionCD.xyzw;

		fixed4 dotABCD = _GFrequency.xyzw * fixed4(dot(_GDirectionAB.xy, xzVtx), dot(_GDirectionAB.zw, xzVtx), dot(_GDirectionCD.xy, xzVtx), dot(_GDirectionCD.zw, xzVtx));
		fixed4 TIME = _Time.yyyy * _GSpeed;

		fixed4 COS = cos(dotABCD + TIME);

		nrml.x -= dot(COS, fixed4(AB.xz, CD.xz));
		nrml.z -= dot(COS, fixed4(AB.yw, CD.yw));

		result += normalize(nrml);

		fixed M = 1.1;
		_GAmplitude *= M;
		 _GFrequency *= M;
		 _GSteepness *= M;
		 _GSpeed *= M;
		 _GDirectionAB = lerp(_GDirectionAB.xyzw, _GDirectionAB.wxyz ,0.3);
		 _GDirectionCD = lerp(_GDirectionCD.xyzw , _GDirectionCD.wxyz , 0.3);
		 //_GDirectionCD.xyzw = _GDirectionCD.wxyz;
		// _GDirectionAB *= 1.5;
		// _GDirectionCD *= 1.5;
	}

	result.xz *= _BumpAmount;
	return normalize(result);
}*/
#endif



void APP_FOG(fixed ff, inout fixed4 color) {
	UNITY_APPLY_FOG(saturate(ff), color);
}


fixed3 postrize(fixed3 c, fixed s)
{
	return floor(c*s) / (s - 1);
}

#if defined(ADVANCE_PC)
#define TEX2DGRAD(s,uv) tex2Dgrad(s,uv,0,0)
#else
#if !defined (SHADER_API_GLES)
//#define TEX2DGRAD(s,uv) tex2Dgrad(s,uv,0,0)
#define TEX2DGRAD(s,uv) tex2D(s,uv)
#else
#define TEX2DGRAD(s,uv) tex2D(s,uv)
#endif
#endif



//#if defined(HAS_BAKED_DEPTH)
fixed4 _internal_rawZ;
fixed GET_BAKED_Z(fixed2 zUV) {

#if defined(BAKED_DEPTH_EXTERNAL_TEXTURE)
	_internal_rawZ = TEX2DGRAD(_BakedData, zUV);
#else
	_internal_rawZ = TEX2DGRAD(_BakedData_temp, zUV);
#endif
	return dot(_internal_rawZ.bgr, fixed3(128, 32, 8));
}
fixed GET_BAKED_Z_WITHOUTWRITE(fixed2 zUV) {

#if defined(BAKED_DEPTH_EXTERNAL_TEXTURE)
	fixed3 _rawZ = TEX2DGRAD(_BakedData, zUV).rgb;
#else
	fixed3 _rawZ = TEX2DGRAD(_BakedData_temp, zUV).rgb;
#endif
	return dot(_rawZ.bgr, fixed3(128, 32, 8));
}
//#endif
/*fixed rawzdepth = (rawZ.b * 4 + rawZ.g);
rawzdepth = (rawzdepth * 4 + rawZ.r);
return rawzdepth * 8;*/

fixed GET_SHORE(fixed2 zUV) {
#if defined(HAS_CAMERA_DEPTH)
#if defined(BAKED_DEPTH_EXTERNAL_TEXTURE)
	return TEX2DGRAD(_BakedData, zUV).a;
#else
	return TEX2DGRAD(_BakedData_temp, zUV).a;
#endif
#else

#if defined(BAKED_DEPTH_EXTERNAL_TEXTURE)
	return _internal_rawZ.a;
#else
	return _internal_rawZ.a;
#endif
#endif
}

#if defined(HAS_CAMERA_DEPTH)

fixed GET_Z(in v2f i, fixed4 UV) {
	/*#if defined(FOAM_FINE_REFRACTIOND_DOSTORT)
	fixed restore_zdepth = LinearEyeDepth(RESTORE_TCDPRJ) - i.screen.w;
	#endif*/
	/*#if defined(SKIP_CALCULATE_HEIGHT_DEPTH) && defined(FALSE)
				return LinearEyeDepth(tcdprj) - i.pos.w;
				//return LinearEyeDepth(tcdprj)*_ProjectionParams.z - _ProjectionParams.y;
				//return tcdprj;
	#else*/

#ifdef ORTO_CAMERA_ON
	//fixed wD = i.wPos.y - _WorldSpaceCameraPos.y;
	//fixed direction = wD / i.pos.w;

	//fixed ZP = (i.pos.z);
	fixed ZP = (i.screen.z);
	fixed ld = (SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, (i.screen)));
	/*fixed4 grabPos = ComputeGrabScreenPos(i.pos);
	fixed ld = Linear01Depth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, (grabPos)));
	float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen)));
	float partZ = i.screen.z;
	return ((sceneZ - partZ) - _MyNearClipPlane)/ (_MyFarClipPlane - _MyNearClipPlane);*/
	//return ZP;
	//return ld*10;
#if !defined(UNITY_REVERSED_Z)
			//ZP = 1.0f - ZP;
	ZP = (1.0f - ZP) / 2;
	//return (ZP) * 10;
	ld = 1 - ld;
#endif
	return (ZP - ld)  * (_MyFarClipPlane - _MyNearClipPlane); //* 2


	/*fixed3 wD = i.wPos - _WorldSpaceCameraPos;
	//fixed ld = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen)).r));
	fixed worldspace = normalize(wD).y * ld + _WorldSpaceCameraPos.y;
	//fixed worldspace = wD / (i.screen.w) * ld + _WorldSpaceCameraPos.y;
	return (i.wPos.y - worldspace);*/



	/*fixed SD = Linear01Depth(tex2Dgrad(_CameraDepthTexture, UV, 0, 0).r);
//fixed SD = Linear01Depth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UV));
//fixed SD = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UV));

fixed zpos = (ZP - SD) * (_MyFarClipPlane);
//fixed zpos = (ZP - SD) * (_MyFarClipPlane - _MyNearClipPlane);
//fixed zpos = ( i.pos.z - UNITY_SAMPLE_DEPTH(tcdprj))*(_MyFarClipPlane - _MyNearClipPlane);
return ZP;*/
//tcdprj = tex2D(_CameraDepthTexture, (i.screen)).r;
//tcdprj = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen)).r;
/*tcdprj = 1 - (UNITY_SAMPLE_DEPTH(tcdprj));
fixed ZPOS = 1 - i.pos.z;
return  (tcdprj - ZPOS) * _MyFarClipPlane;*/

/*fixed3 direction = (i.wPos - _WorldSpaceCameraPos).y;
fixed zver = direction * zpos + _WorldSpaceCameraPos.y;
return  (i.wPos.y - zver) / 200;*/
/*fixed3 vp = mul(UNITY_MATRIX_VP, i.wPos);
if (i.screen.x < 0.5) return ZPOS;
return  (tcdprj - ZPOS )* _MyFarClipPlane ;*/
/*

tcdprj = tcdprj * (_MyFarClipPlane - _MyNearClipPlane) + _MyNearClipPlane;
ZPOS = ZPOS * (_MyFarClipPlane - _MyNearClipPlane) + _MyNearClipPlane;

tcdprj *= direction;
ZPOS *= direction;

tcdprj += _WorldSpaceCameraPos.y;
ZPOS += _WorldSpaceCameraPos.y;

return  (i.wPos.y - ZPOS);

if (i.screen.x < 0.5) return tcdprj;
//return  (distance(i.wPos , _WorldSpaceCameraPos) - _MyNearClipPlane)/ _MyFarClipPlane;
return  1 - i.pos.z;
fixed ld = tcdprj;
return tcdprj ;
//fixed3 direction = normalize(i.wPos - _WorldSpaceCameraPos);
fixed worldspace = direction * ld + _WorldSpaceCameraPos.y;
return (i.wPos.y - worldspace);*/

//return ld;
//fixed ld = tcdprj * (_MyFarClipPlane - _MyNearClipPlane) + _MyNearClipPlane;
//fixed ld = (tcdprj * ( _MyFarClipPlane - _MyNearClipPlane));
//	return tcdprj * ;
	//fixed3 worldspace = direction * ld + _WorldSpaceCameraPos;
	//return (worldspace.y - i.wPos.y );
	/*if (tcdprj == i.pos.z) return 0;
	return 1;
	return tcdprj - i.pos.z;*/
#else

	/*#ifdef ORTO_CAMERA_ON
				fixed tcdprj = tex2Dgrad(_CameraDepthTexture, UV, 0, 0).r;
	#else
				fixed tcdprj = tex2Dproj(_CameraDepthTexture, UV).r;
	#endif*/

#if defined(ZDEPTH_CALCANGLE) && !defined(ULTRA_FAST_MODE)
	fixed3 ld = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UV));
	fixed3 wD = i.wPos.xyz - _WorldSpaceCameraPos;
	fixed3 worldspace = wD / (i.screen.w) * ld + _WorldSpaceCameraPos;
	fixed wViewDir = -wD / i.wPos.w;
	fixed lerped = (1 - saturate(dot(fixed3(0, 1, 0), wViewDir) *1.5)) * _ZDistanceCalcOffset;
	//return lerped;
	return (distance(i.wPos.xyz, worldspace) * (lerped)+(i.wPos.y - worldspace.y) * 2.5 * (1 - lerped));
#else

	fixed ld = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UV));
	//return ;


	fixed wD = i.wPos.y - _WorldSpaceCameraPos.y;
	fixed worldspace = wD / (i.screen.w) * ld + _WorldSpaceCameraPos.y;
#if defined(ZDEPTH_CALCANGLE)
	fixed wViewDir = -wD / i.wPos.w;
	fixed lerped = (1 - saturate(dot(fixed3(0, 1, 0), wViewDir) *1.5)) * _ZDistanceCalcOffset;
	return (((ld - i.screen.w)) * (lerped)+(i.wPos.y - worldspace) * 2.5 * (1 - lerped));
#else
		return (i.wPos.y - worldspace) * 2.5;
#endif
		
#endif

	//fixed ld = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen)).r));
	//fixed3 worldspace = normalize(wD) * ld + _WorldSpaceCameraPos;
	//float Z = saturate((i.wPos.y - worldspace.y) / _DeepFalloff - _DeepOffset);
	//float4 color = half4(Z, Z, Z, 1) * _MainTexColor;
	//float3 WP = (i.wPos + worldspace) / 2;


	//fixed rawDepth = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen)));

	//tcdprj = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen)).r;
	//tcdprj = Linear01Depth(UNITY_SAMPLE_DEPTH(tcdprj)) * 10;
	//refrUv.y += (dot(i.wViewDir, worldNormal)) * 10;

	/*fixed wD = i.wPos.y - _WorldSpaceCameraPos.y;
	fixed ld = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tcdprj));
	fixed worldspace = wD / (i.screen.w) * ld + _WorldSpaceCameraPos.y;

	return (i.wPos.y - worldspace);*/
#endif


	/*#if defined(FOAM_FINE_REFRACTIOND_DOSTORT)
	fixed ld2 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(RESTORE_TCDPRJ));
	fixed worldspace2 = direction * ld2 + _WorldSpaceCameraPos.y;
	fixed restore_zdepth = (i.wPos.y - worldspace2);
	#endif*/

	// color.rgb = float4(frac((worldspace / 10)), 1.0f);

//#endif
}

#endif



/*static const fixed2 random_h = fixed2(12.9898, 78.233);
		fixed random(fixed2 input) {
			return frac(sin(dot(input, random_h))* 43758.5453123);
		}*/






		//VERTEX
#if !defined(SKIP_3DVERTEX_ANIMATION)

void VERTEX_MOVER(inout appdata v, inout v2f o, in float2 texcoord, inout fixed3 n) {

	half4 vetrex = v.vertex;

#if defined(VERTEX_ANIMATION_BORDER_FADE)&& !defined(MINIMUM_MODE)
	/*#if defined(HAS_CAMERA_DEPTH) && !defined(WRONG_DEPTH)
				half4 SCRP = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
	#ifdef ORTO_CAMERA_ON//////////////////////////////////////
				half beforetcdprj = tex2D(_CameraDepthTexture, SCRP).r;
	#else//////////////////////////////////////////////////////
				half beforetcdprj = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(SCRP)).r;
	#endif//////////////////////////////////////////////////////
				half vetexMask = saturate(GET_Z(o, beforetcdprj));
	#else*/


	half3 rawZ;
#if defined(BAKED_DEPTH_EXTERNAL_TEXTURE)
	rawZ = tex2Dlod(_BakedData, fixed4(o.uv.zw, 1.0, 1.0)).a;
#else
	rawZ = tex2Dlod(_BakedData_temp, fixed4(o.uv.zw, 1.0, 1.0)).a;
#endif
	//float rawzdepth = (rawZ.b * 4 + rawZ.g);
	//rawzdepth = (rawzdepth * 4 + rawZ.r) * 8;

	//rawzdepth = rawZ;
	//half _3Dwaves_BORDER_FACE = 0;
	fixed vetexMask = saturate(rawZ +  _3Dwaves_BORDER_FACE);
	//vetexMask = 1 - (1 - vetexMask) * _BAKED_DEPTH_EXTERNAL_TEXTURE_Amount;
	//half vetexMask = rawZ.r + rawZ.g * 255

	//#endif
#elif (defined(VERTEX_ANIMATION_BORDER_CLAMPXZ) || defined(VERTEX_ANIMATION_BORDER_CLAMPXYZ))&& !defined(MINIMUM_MODE)
	fixed2 sum = max(0, abs(o.uv.zw - 0.5) - 0.45) / 0.05;
	fixed vetexMask = 1 - saturate(max(sum.x, sum.y));
#else
	fixed vetexMask = 1;
#endif

	fixed PI2 = Pi * 2;

	//half wavetime = _Time.w * 4 * _3DWavesSpeed / PI2;
	float2 wavetime = _FracWTime_m4_m3DWavesSpeed_dPI2;

#if defined(HAS_ROTATION)
	//wavetime = rNX * wavetime.x + rNY * wavetime.y;
	float2 wt = _3DWavesTile.xy;
#else
	float2 wt = _3DWavesTile.xy;
#endif


	/*#if defined(HAS_WAVES_ROTATION)
				half waves_a_x = cos(_WavesDirAngle);
				half waves_a_y = sin(_WavesDirAngle);
				half waves_b_x = -waves_a_y;
				half waves_b_y = waves_a_x;
				half2 wavesCoord = half2(waves_a_x, waves_a_y) * texcoord.x + half2(waves_b_x, waves_b_y)*texcoord.y;
				wt = half2(waves_a_x, waves_a_y) * wt.x + half2(waves_b_x, waves_b_y)*wt.y;
	#else*/
	float2 wavesCoord = texcoord.xy / PI2;

	#if defined(HAS_WAVES_DETILE)
	
	//float sincor = 4 * sin(length((v.vertex - point_uvw) / 4));
	float2 tp = v.vertex.xz - _VertexSize.xy;
	float p1 = length(tp);
	float p2 = length(tp + _VertexSize.wz);
	const float4 v4_one3 = float4(1, 1, -1, -1);
	float tsum = dot(float4(wavesCoord.x, wavesCoord.y , p1 , p2), v4_one3);
	float DTS = (_VERTEX_ANIM_DETILESPEED  *( _Time.x * tsum)) % PI2;
	/*
		float2 DT2 = float2 ( sin((wavesCoord.y + wavesCoord.x /4) * _VERTEX_ANIM_DETILEFRIQ * 100 + DTS),
			cos((wavesCoord.x + wavesCoord.y / 4) * _VERTEX_ANIM_DETILEFRIQ * 100 - DTS))* _VERTEX_ANIM_DETILEAMOUNT;*/
	float2 source = (wavesCoord.yx * _VERTEX_ANIM_DETILEFRIQ * 100)%PI2 ;
	//float2 DT2 = float2(sin(wavesCoord.x *_VERTEX_ANIM_DETILEFRIQ * 100), cos(wavesCoord.y *_VERTEX_ANIM_DETILEFRIQ * 100));
	float2 rotA = float2(cos(DTS), sin(DTS)) ;
	float2 rotB = float2(-rotA.y, rotA.x);
	//float offset = _VERTEX_ANIM_DETILE_YOFFSET;
	//offset = -p1 / _VertexSize.w;
	//float2 DT2a = clamp((rotA * (sin(source.x)) + rotB * (cos(source.y)))* _VERTEX_ANIM_DETILEAMOUNT, _VERTEX_ANIM_DETILE_YOFFSET.x, _VERTEX_ANIM_DETILE_YOFFSET.y) ;
	//float2 DT2b = clamp((rotA * cos(source.x) + rotB * (sin(source.y) ) )* _VERTEX_ANIM_DETILEAMOUNT, _VERTEX_ANIM_DETILE_YOFFSET.x, _VERTEX_ANIM_DETILE_YOFFSET.y) ;
	float2 DT2a = (rotA * sin(source.x) + rotB * cos(source.y))* _VERTEX_ANIM_DETILEAMOUNT;
	float2 DT2b = (rotA * cos(source.x) + rotB * sin(source.y))* _VERTEX_ANIM_DETILEAMOUNT;
	float2 DT2 = (DT2a + DT2b);
	//wavesCoord.x += DX2 * _VERTEX_ANIM_DETILEAMOUNT;
	//wavesCoord.y += DY2 * _VERTEX_ANIM_DETILEAMOUNT;
#endif


#if defined(HAS_WAVES_ROTATION)
	/*float waves_a_x = cos(_WavesDirAngle);
	float waves_a_y = sin(_WavesDirAngle);
	float waves_b_x = -waves_a_y;
	float waves_b_y = waves_a_x;
	wavetime = float2(waves_a_x, waves_a_y) * wavetime.x + float2(waves_b_x, waves_b_y)*wavetime.y;*/
	//wt = half2(waves_a_x, waves_a_y) * wt.x + half2(waves_b_x, waves_b_y)*wt.y;
#endif
	//#endif
				//half coordY = sin(texcoord.y *_3DWavesTile.x / 10 * _3DWavesTileZ + wavetime) * _3DWavesTileZAm;
				//half coordY = texcoord.y;
		float2 wavesCoordPreComp = frac(wavesCoord * wt + wavetime)*PI2;
/*#if defined(DETILE_LQ) || defined(DETILE_HQ)
		wavesCoordPreComp += wt * DT2*PI2;
#endif*/

	float waveX = (wavesCoordPreComp.x);
	float waveY = (wavesCoordPreComp.y);
	// waveX += waveX * DT2.x;
	// waveY += waveY * DT2.y;
	
#if defined(HAS_WAVES_DETILE)
	//waveX += sin( DT2.x);
	//waveY += cos( DT2.y);
	//waveX = (sin(waveX+ DT2.x+2.5));
	//waveY = cos(waveY*abs( DT2.y));
	fixed2 v_detile = float2( sin(waveY + (DT2.x)) + (DT2.x) , cos(waveX + (DT2.x)) + (DT2.y) );
	fixed2 clampVal = fixed2(min(5, _VERTEX_ANIM_DETILE_YOFFSET.x), max(-1, _VERTEX_ANIM_DETILE_YOFFSET.y));
	v_detile = clamp(v_detile, clampVal.x, clampVal.y);
	v_detile -= _VERTEX_ANIM_DETILE_YOFFSET.z;
	waveX = sin(waveX) + v_detile.x;
	waveY = cos(waveY) + v_detile.y;
	//waveX = sin(waveX);
	//waveY = cos(waveY);
#else
	waveX = sin(waveX);
	waveY = cos(waveY);
#endif


#if defined(HAS_WAVES_ROTATION)
	float waves_a_x = cos(_WavesDirAngle);
	float waves_a_y = sin(_WavesDirAngle);
	wavesCoord = float2(waves_a_x, waves_a_y) * waveX + float2(-waves_a_y, waves_a_x)*waveY;
	waveX = wavesCoord.x;
	waveY = wavesCoord.y;
	//wt = half2(waves_a_x, waves_a_y) * wt.x + half2(waves_b_x, waves_b_y)*wt.y;
#endif
	waveX = (waveX);
	waveY = (waveY);
	//waveX += waveX * DT2F.x;
	//waveY += waveY * DT2F.y;

	float MMX = max(vetexMask, 0.0);
	float yoffset = (waveY * _3DWavesHeight ) / _ObjectScale.y;
#if !defined(VERTEX_ANIMATION_BORDER_CLAMPXZ)&& !defined(MINIMUM_MODE)
	yoffset *= MMX;
#endif
	MMX = _3DWavesWind * MMX;
	float difX = waveY * MMX / _ObjectScale.x * _3DWavesWind;
	float difY = waveX * MMX / _ObjectScale.z * _3DWavesWind;
	vetrex.x += difX;
	vetrex.z += difY;
	vetrex.y += yoffset  / 3;



	o.vfoam.x = difX;
	o.vfoam.y = difY;

	o.VER_TEX.w = vetrex.y;
	//o._utils.y = yoffset;
	//o._utils.x = max(0, waveX * vetexMask);
	//o._utils.z = max(0, waveY * vetexMask);

	/*if (posUV.x != 0)
	pos.y += yoffset;*/


	//NORMALS
#if defined(WAW3D_NORMAL_CALCULATION)  && !defined(MINIMUM_MODE)

			//n.y += (o._utils.x - _3DWavesHeight /10) / 10 * _3dwanamnt;
			//n.x -= (yoffset - _3DWavesHeight / 10) / 40 * _3dwanamnt;
	half ddd = _3dwanamnt/1000 / _3DWavesWind;
	n.z -= (difY) * ddd;
	n.x -= (difX) * ddd;
	//n.z += (difX) / 1000 * _3dwanamnt;
	//n.y *= waveY / 5;
	n = normalize(n);
	//v.normal = n;

#endif


	v.vertex = vetrex;
	//o.pos = UnityObjectToClipPos(vetrex);
}



#endif




/*



#if !defined(SKIP_3DVERTEX_ANIMATION) && !defined(SKIP_3DVERTEX_HEIGHT_COLORIZE)
		//foamColor.rgb = min(0.7, pow(i._utils.y + 0.95, 32))  * unpackedFoam;
		//foamColor.rgb += saturate(i._utils.y * 5 * unpackedFoam * pow((i._utils.x), 1));
		color.rgb += saturate(i._utils.y * 5 * color.rgb * _3DWavesYFoamAmount * i._utils.x);
#endif

#if defined(USE_LIGHTMAPS)
		color.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i._utils.zw));
#endif


#if defined(HAS_NOISED_GLARE)
		//color.rgb += max(foamColor, NOISE_GLARE* color.rgb * color.rgb) + min(0, foamColor);
		//color.rgb += lerp(NOISE_GLARE* color.rgb * color.rgb, (foamColor), min(1, foamL));


#if !defined(SKIP_FOAM)
		fixed noise_lerp = min(1, foamL);
#if defined(NOISE_BLEND_3)
		color.rgb += lerp(NOISE_GLARE* color.rgb* color.rgb , (foamColor), noise_lerp);
#elif defined(NOISE_BLEND_2)
		color.rgb += lerp(NOISE_GLARE* color.rgb, (foamColor), noise_lerp);
#else
		color.rgb += lerp(NOISE_GLARE, (foamColor), noise_lerp);
#endif
#else
		color.rgb += NOISE_GLARE;
#endif
		//color.rgb += NOISE_GLARE * color.rgb * color.rgb * (1- saturate(foamL)) ;
		//color.rgb += lerp((NOISE_GLARE* saturate(color.rgb* color.rgb)), (foamColor), foamL);
		//color.rgb += foamColor.rgb +  min(10,max(-10,NOISE_GLARE * color.rgb* color.rgb));
		//NOISE_GLARE = NOISE_GLARE * color.rgb * color.rgb;
		//color.rgb += (foamColor - NOISE_GLARE) * foamL + (N);
#else

#if defined(FOAM_BLEND_LUM)
		color.rgb = lerp(color.rgb, foamColor.rgb, saturate((foamColor.x + foamColor.y + foamColor.z) / 3 - _FoamBlendOffset) );
#else
		color.rgb += foamColor;
#endif
		//;
#endif



		color.a = foamColor.a;
#if defined(TRANSPARENT_LUMINOSITY)
		color.a *= lerp(1 - saturate((tnormal.x + tnormal.y) * 5 )- 1 + _TransparencyLuminosity, 1, 1 - fresnelFac);
#endif
#if defined(TRANSPARENT_POW)
#if !defined(TRANSPARENT_POW_INV)
		//color.a *= saturate((pow((color.rgb - (2 - _TransparencyPow * 3)), 1)));
		color.a *= saturate(((color.rgb - (2 - _TransparencyPow * 3))));
#else
		//color.a *= saturate(pow(abs(1 - color.rgb - (3 - (1 - _TransparencyPow) * 3)), 1)) + specularLight;
		color.a *= saturate(abs(1- color.rgb - (3 - (1 - _TransparencyPow) * 3))) + specularLight ;
#endif
		//color.a *= saturate((pow(abs(color.rgb - (2 - _TransparencyPow * 3)), 1))) + specularLight * (1 - saturate( _TransparencyPow * 10 - 5));
#endif
#if defined(TRANSPARENT_SIMPLE)
		color.a *= _TransparencySimple;
#endif

		//
		 //color.rgb = max(color.rgb, color.a);

		//return fixed4(color.a,0,0, 1);
		//RIM
		//return fixed4(color.rgb, 1);
#if defined(RIM)


#if defined(SKIP_FRESNEL_CALCULATION)

		fixed dott = dot(fixed3(0,1,0), tnormal);
		fixed rim = 0.98 - (dott);
		//rim = min(0.95, rim);
		//rim = max(0.6, rim);
		fixed minus =  saturate(pow(rim, RIM_Minus)) * color.b;
		//fixed3 plus = saturate(pow(rim, RIM_Plus)) * color.rgb;
		//color.rgb -= minus;
		//color.rgb += plus;


		dott = saturate(pow (dot(normalize(i.wViewDir), worldNormal), RIM_Plus));



		fixed diff = dott * minus;
#else
		fixed diff = (1 - fresnelFac * RIM_Plus + RIM_Minus);
#endif

#if defined(RIM_INVERSE)
		diff = 1 - diff;
#endif

		fixed3 ramp = tex2Dgrad(_RimGradient, saturate(fixed2(diff, 0)),0,0).rgb;
		color.rgb *= ramp;
		//
#endif
		//RIM



#if defined(USE_SURFACE_GRADS)

		//color.rgb = color.rgb*surface_grad;
		color.rgb = color.rgb+surface_grad;

#endif



		//SHADOW
#if defined(USE_SHADOWS)
		color.rgb *= (SHADOW_ATTENUATION(i) / 2 + 0.5);
		//return SHADOW_ATTENUATION(i);
#endif
		//color.rgb += i.ambient;

#if !defined(SKIP_AMBIENT_COLOR)
		//color.rgb = color.rgb * i.ambient + color.rgb;
		color *= UNITY_LIGHTMODEL_AMBIENT;
#endif


		//return float4(UNITY_LIGHTMODEL_AMBIENT.rrr, 1);



#if defined(POSTRIZE)
		color.rgb = postrize(color.rgb, POSTRIZE_Colors);
		//color.b = floor(color.b * POSTRIZE_Colors) / (POSTRIZE_Colors - 1);
#endif



#if defined(USE_OUTPUT_GRADIENT)
	
#if defined(GRAD_5)
fixed POS = 0.85;
#elif defined(GRAD_4)
fixed POS = 0.65;
#elif defined(GRAD_3)
fixed POS = 0.5;
#elif defined(GRAD_2)
fixed POS = 0.35;
#else
fixed POS = 0.15;
#endif
fixed3 grad = tex2Dgrad(_GradTexture, fixed2(saturate((color.r + color.g + color.b) / 3), POS), 0, 0);
// color.rgb = grad;


#if defined(USE_OUTPUT_BLEND_1)
color.rgb = max(color.rgb, (grad - color.rgb) * _OutGradBlend* saturate(zdepth / _OutGradZ) + color.rgb);
#elif defined(USE_OUTPUT_BLEND_3)
color.rgb = (grad - color.rgb) * _OutGradBlend + color.rgb;
#else
color.rgb = (grad - color.rgb) * _OutGradBlend * saturate(zdepth / _OutGradZ) + color.rgb;
#endif


#if defined(USE_OUTPUT_SHADOWS)
fixed NdotL = dot(worldNormal, fixed3(i.tspace0.z, i.tspace1.z, i.tspace2.z));
//fixed NdotL = dot(worldNormal, fixed3(0,1,0)) ;
NdotL = 1 - pow(NdotL, 4048);
// NdotL =  pow( NdotL , 16);
color.rgb -= (NdotL / 10) * _OutShadowsAmount;
#endif

#endif
//color.b = floor(color.b * 6) / (6 - 1);

color.a *= i.helpers.z;
//color.a *= max(0,i.helpers.z);


#if defined (USE_LUT)
fixed3 uvw = (color.rgb);

#if !UNITY_COLORSPACE_GAMMA
uvw = LinearToGammaSpace(uvw);
#endif

#if defined(FIX_OVEREXPO)
fixed3 maxvalue = saturate(uvw - 1);
uvw = saturate(uvw);
#endif

fixed3 scaleOffset = _Lut2D_params.xyz;

uvw.z *= scaleOffset.z;
fixed shift = floor(uvw.z);
uvw.xy = uvw.xy * scaleOffset.z * scaleOffset.xy + scaleOffset.xy * 0.5;
uvw.x += shift * scaleOffset.y;
uvw.y = saturate(uvw.y);
uvw.xyz = lerp(tex2Dgrad(_Lut2D, uvw.xy, 0, 0).rgb, tex2Dgrad(_Lut2D, uvw.xy + fixed2(scaleOffset.y, 0), 0, 0).rgb, uvw.z - shift);

#if defined(FIX_OVEREXPO)
uvw += maxvalue;
#endif

#if !UNITY_COLORSPACE_GAMMA
uvw = GammaToLinearSpace(uvw);
#endif
//color.rgb = lerp(color.rgb, uvw, _Lut2D_ST.w);
//color.rgb = uvw;



color.rgb = lerp(color.rgb, uvw, _LutAmount);
#endif


#if !defined(SKIP_FOG)
#ifdef ORTO_CAMERA_ON
fixed3 eyePos = UnityObjectToViewPos(i.fogCoord);
fixed fogCoord = length(eyePos);
UNITY_CALC_FOG_FACTOR_RAW(fogCoord);
APP_FOG(unityFogFactor, color);
#else
//color.rgb = lerp(unity_FogColor.rgb, color.rgb, i.fogCoord);
UNITY_APPLY_FOG(i.fogCoord, color);
#endif
#endif

*/