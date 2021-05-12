#ifdef WATER_DOWNSAMPLING
half2 fb_wcoord = (i.screen.xy / i.screen.w);
half RES = 16;
half hRES = RES / 2;
fixed fb_Yof = floor(frac((fb_wcoord.y * _ScreenParams.y) / RES)*hRES)*hRES;
fixed fb_sp = frac((fb_wcoord.x * _ScreenParams.x + fb_Yof + _FrameRate) / RES) - DOWNSAMPLING_SAMPLE_SIZE;
//if (fb_sp < 0) 	return tex2D(_FrameBuffer, fb_wcoord);
fixed cond = saturate(fb_sp);
//#define cond fb_sp < 0
if (cond) {
#endif

#ifdef WATER_DOWNSAMPLING_HARD
half2 fb_wcoord = (i.screen.xy / i.screen.w);
	half RES = 16;
	half hRES = RES / 2;
	fixed fb_Yof = floor(frac((fb_wcoord.y * _ScreenParams.y) / RES)*hRES)*hRES;
	fixed fb_sp = frac((fb_wcoord.x * _ScreenParams.x + fb_Yof + _FrameRate) / RES) - DOWNSAMPLING_SAMPLE_SIZE;
	//if (fb_sp < 0) 	return tex2D(_FrameBuffer, fb_wcoord);
	fixed cond = saturate(fb_sp);
	//#define cond fb_sp < 0
	if (cond) {	
#endif