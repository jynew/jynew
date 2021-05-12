// Upgrade NOTE: replaced 'defined MAINTEX_HAS_MOVE' with 'defined (MAINTEX_HAS_MOVE)'


#if defined(SKIP_MAIN_TEXTURE)
//fixed3 tex = _ReplaceColor;
 tex = fixed3(1, 1, 1);
#define SKIP_TEX = 1;
#else/////////////////// USE_TEX

#if defined(TEXTURE_CHANNEL_RGB)
#define MAINT_SAMPLE(t,u,v) fixed3 v = tex2D(t, u).rgb
#elif defined(TEXTURE_CHANNEL_G)
#define MAINT_SAMPLE(t,u,v) fixed v = tex2D(t, u).g
#elif defined(TEXTURE_CHANNEL_B)
#define MAINT_SAMPLE(t,u,v) fixed v = tex2D(t, u).b
#else
#define MAINT_SAMPLE(t,u,v) fixed v = tex2D(t, u).r
#endif


/*half2 maintuv = (DETILEUV
#if defined (MAINTEX_HAS_MOVE)
	+_FracTimeX * MAIN_TEX_Move
#endif
	) * MAIN_TEX_Tile;


half2 maintuv1 = (UVSCROLL.xy + _FracTimeX * (
#if defined (MAINTEX_HAS_MOVE)
	MAIN_TEX_Move +
#endif
	MAIN_TEX_CA_Speed)) * MAIN_TEX_Tile;
half2 maintuv2 = (UVSCROLL.zw + _FracTimeX * (
#if defined (MAINTEX_HAS_MOVE)
	MAIN_TEX_Move
#endif
	- MAIN_TEX_CA_Speed)) * MAIN_TEX_Tile;*/

#if defined(MINIMUM_MODE)
MYFLOAT2 UVSDIST = i.vfoam.xy * MAIN_TEX_LQDistortion;
#else
MYFLOAT _UVSDIST = tnormal_TEXEL * MAIN_TEX_Distortion ;
#if defined(MAIN_TEX_ADDDISTORTION_THAN_MOVE)
_UVSDIST += _UVSDIST * i.vfoam.x * MAIN_TEX_ADDDISTORTION_THAN_MOVE_Amount;
#endif
MYFLOAT2 UVSDIST = _UVSDIST - i.vfoam.xy * MAIN_TEX_LQDistortion;
//MYFLOAT2 UVSDIST = _UVSDIST - i.fogCoord.zw * MAIN_TEX_LQDistortion;

#endif

#define SINGLEUV MYFLOAT2 maintuv = (DETILEUV ) * MAIN_TEX_Tile+ _FracMAIN_TEX_TileTime_mMAIN_TEX_Move  + UVSDIST

#define DOUBLEUVS \
MYFLOAT2 maintuv1 = (UVSCROLL.xy + UVSDIST) * MAIN_TEX_Tile + _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed;\
	MYFLOAT2 maintuv2 = (UVSCROLL.zw - UVSDIST) * MAIN_TEX_Tile + _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed


#if !defined(MINIMUM_MODE)
#if defined(USE_BLENDANIMATED_MAINTEX)

	SINGLEUV;

//maintuv /= 2;

#if defined(TEXTURE_CHANNEL_RGB)
	fixed3 _msk = tex2D(_MainTex, maintuv).rgb;
	fixed msk = (_msk.r + _msk.g + _msk.b) / 3;
#elif defined(TEXTURE_CHANNEL_G)
	fixed msk = tex2D(_MainTex, maintuv).g;
#elif defined(TEXTURE_CHANNEL_B)
	fixed msk = tex2D(_MainTex, maintuv).b;
#else
	fixed msk = tex2D(_MainTex, maintuv ).r;
#endif
//return  (i.uv.x + i.uv.y)*10;
	/*fixed uvxf = frac(i.uv.x*3) *2 - 1;
	fixed uvyf = frac(i.uv.y*3) *2 - 1;
fixed fr = frac(_Frac01Time_MAIN_TEX_BA_Speed + ( msk  - uvxf * uvyf) ) * 2;*/
	fixed uvxf = (maintuv.x);
	//fixed uvyf = (maintuv.y);
	//fixed uvxf = abs(frac(i.uv.x*2) - 0.5) * 2;
	fixed uvyf = abs(frac (maintuv.y * 2)-0.5)  ;
	fixed fr = frac(_Frac01Time_MAIN_TEX_BA_Speed + i.fogCoord.z + i.fogCoord.w) * 2;
	//return fr;
	fixed time = abs(1 - fr);
	//time = frac(1 +  time * time *time);
	//	
	
	MAINT_SAMPLE(_MainTex, maintuv + 0.5 , val);
	val = saturate(val * MAIN_TEX_Multy + MAIN_TEX_MixOffset);
	fixed text1 = val;
	fixed text2 = 1 - val;
	fixed result = lerp(text1, text2, (time));
	
	val =( msk+0.1) * result ;

//return float4(val.rrr, 1);
//return float4(result.rrr, 1);
/*fixed asd = 1 - (abs(i.VER_TEX.w  + 0.3) - 0.1);
asd *= asd * 2;
//return asd;
asd = saturate(asd);
val = val *  asd;*/
//(i.VER_TEX.w + 0.8) *
//tex = fixed3(0.5, 0.5, 0.5);

#elif defined(USE_CROSSANIMATED_MAINTEX)
	//half2 mainspeed = _FracTimeX * MAIN_TEX_CA_Speed;
	DOUBLEUVS;
MAINT_SAMPLE(_MainTex, maintuv1, val);
MAINT_SAMPLE(_MainTex, maintuv2, val2);
val = (val + val2) / 3.5;
//val /= 1.75;
#elif defined(USE_CROSSANIMATED4X_MAINTEX)
	//half2 mainspeed = half2(_FracTimeX, _FracTimeX) * MAIN_TEX_CA_Speed;
	DOUBLEUVS;

MAINT_SAMPLE(_MainTex, maintuv1, val);
MAINT_SAMPLE(_MainTex, maintuv2, val2);
MAINT_SAMPLE(_MainTex, maintuv1.yx, val3);
MAINT_SAMPLE(_MainTex, maintuv2.yx, val4);
val = (val + val2 + val3 + val4) / 6;
//val = min(val, min( val2 , min( val3 ,val4))); 
//val /= 1.5;
#else
	SINGLEUV;
MAINT_SAMPLE(_MainTex, maintuv, val);
val /= 2;
#endif
#else
SINGLEUV;
MAINT_SAMPLE(_MainTex, maintuv, val);
#endif

val *= MAIN_TEX_Amount;
#if !defined(SKIP_MAINTEX_VHEIGHT) && !defined(SKIP_MAIN_TEXTURE)
val *= saturate(i.VER_TEX.w + MAINTEX_VHEIGHT_Offset)*MAINTEX_VHEIGHT_Amount; // saturate
#endif

#if defined(_APPLY_REFR_TO_TEX_DISSOLVE_FAST) && defined(HAS_REFRACTION)
//FIXEDAngleVector *= saturate(1 - lerped_refr);
val *= APS;
#endif

#if defined(CLAMP_BRIGHT)
val = saturate(val + MAIN_TEX_Bright);
#else
val = (val) + MAIN_TEX_Bright;
#endif
 tex = val;




#if defined(DEBUG_TEXTURE)
return float4(tex * _ReplaceColor, 1);
#endif

#endif//


#if !defined(SKIP_TEX)
//tex *= 2;
#endif