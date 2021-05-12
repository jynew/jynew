Shader "Beautify/Beautify" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OverlayTex ("Overlay (RGB)", 2D) = "black" {}
		//_DoFTex("DoF (RGBA)", 2D) = "black" {}
		_DoFTex("DoF (RGBA)", any) = "black" {}
		_Sharpen ("Sharpen Data", Vector) = (2.5, 0.035, 0.5)
		_ColorBoost ("Color Boost Data", Vector) = (1.1, 1.1, 0.08, 0)
		_Dither ("Dither Data", Vector) = (5, 0, 0, 1.0)
		_FXColor ("FXColor Color", Color) = (1,1,1,0)
		_TintColor ("Tint Color Color", Color) = (1,1,1,0)
		_Vignetting ("Vignetting", Color) = (0.3,0.3,0.3,0.05)
		_VignettingAspectRatio ("Vignetting Aspect Ratio", Float) = 1.0
		_VignettingMask ("Mask Texture (A)", 2D) = "white" {}
		_Frame("Frame Data", Vector) = (50,50,50,0)
		_FrameMask ("Mask Texture (A)", 2D) = "white" {}
		_Outline("Outline", Color) = (0,0,0,0.8)
		_Dirt("Dirt Data", Vector) = (0.5,0.5,0.5,0.5)
		_Bloom("Bloom Data", Vector) = (0.5,0,0)
		_BloomTex("BloomTex (RGBA)", any) = "black" {}
		//_BloomTex("BloomTex (RGBA)", 2D) = "black" {}
		_BloomWeights("Bloom Weights", Vector) = (0.35,0.55,0.7,0.8)
		_BloomWeights2("Bloom Weights 2", Vector) = (0.35,0.55,0.7,0.8)
		_ScreenLum("Luminance Tex (RGBA)", any) = "black" {}
		//_ScreenLum("Luminance Tex (RGBA)", 2D) = "black" {}
		_CompareParams("Compare Params", Vector) = (0.785398175, 0.001, 0, 0)
		_AFTint ("Anamorphic Flares Tint", Color) = (1,1,1,0.5)
		_BokehData("Bokeh Data", Vector) = (10,1,0,1)
		_DepthTexture("Depth (RGBA)", 2D) = "black" {}
		_DofExclusionTexture("DoF Exclusion (R)", 2D) = "white" {}
		_BokehData2("Bokeh Data 2", Vector) = (1000.0, 4, 0, 0)
		_BokehData3("Bokeh Data 3", Float) = 1000.0
		_EyeAdaptation("Eye Adaptation Data", Vector) = (0.1, 2, 0.7, 1)
		_Purkinje ("Purkinje Data", Vector) = (1.0, 0.15, 0)
		_BloomDepthTreshold("Bloom Depth Threshold", Float) = 1.0
		_FlareTex("Sun Flare texture", 2D) = "black" {}
		_SunPos("SunFlares Sun Screen Position", Vector) = (0.5, 0.5, 0, 0)
		_SunData("SunFlares Sun Data", Vector) = (0.1, 0.05, 3.5, 0.13)
		_SunCoronaRays1("SunFlares Corona Rays 1 Data", Vector) = (0.02, 12, 0.001, 0)
		_SunCoronaRays2("SunFlares Corona Rays 2 Data", Vector) = (0.05, 12, 0.1, 0)
		_SunGhosts1("SunFlares Ghosts 1 Data", Vector) = (0, 0.03, 0.6, 0.06)
		_SunGhosts2("SunFlares Ghosts 2 Data", Vector) = (0, 0.1, 0.2, 0.03)
		_SunGhosts3("SunFlares Ghosts 3 Data", Vector) = (0, 0.25, 0.4, 0.025)
		_SunGhosts4("SunFlares Ghosts 4 Data", Vector) = (0, 0.5, 0.3, 0.04)
		_SunTint("Sun Flare Tint Color", Color) = (1,1,1)
		_SunHalo("SunFlares Halo Data", Vector) = (0.22, 15.1415, 1.0)
		_CompareTex ("Compare Image (RGB)", any) = "black" {}
//		_CompareTex ("Compare Image (RGB)", 2D) = "black" {}
		_BlurScale ("Blur Scale", Float) = 1.0
		_LUTTex ("Lut Texture (RGB)", 2D) = "white" {}
	}

Subshader {	

  Pass { // 0
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertCompare
      #pragma fragment fragCompare
      #pragma target 3.0
      #include "Beautify.cginc"
      ENDCG
  }
 
  Pass { // 1
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragBeautify
      #pragma target 3.0
#pragma multi_compile __ BEAUTIFY_DALTONIZE BEAUTIFY_LUT BEAUTIFY_NIGHT_VISION BEAUTIFY_THERMAL_VISION
#pragma multi_compile __ BEAUTIFY_DEPTH_OF_FIELD BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
#pragma multi_compile __ BEAUTIFY_OUTLINE
#pragma multi_compile __ BEAUTIFY_DIRT
#pragma multi_compile __ BEAUTIFY_BLOOM
#pragma multi_compile __ BEAUTIFY_EYE_ADAPTATION
#pragma multi_compile __ BEAUTIFY_TONEMAP_ACES
#pragma multi_compile __ BEAUTIFY_PURKINJE
// Edited by Shader Control: #pragma multi_compile __ BEAUTIFY_VIGNETTING BEAUTIFY_VIGNETTING_MASK
#pragma multi_compile __ BEAUTIFY_VIGNETTING 
// Edited by Shader Control: #pragma multi_compile __ BEAUTIFY_FRAME BEAUTIFY_FRAME_MASK
#pragma multi_compile __ BEAUTIFY_FRAME 
#pragma multi_compile __ UNITY_COLORSPACE_GAMMA


      #include "Beautify.cginc"
      ENDCG
  }
  
  Pass { // 2
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertLum
      #pragma fragment fragLum
      #pragma fragmentoption ARB_precision_hint_fastest
#pragma multi_compile __ BEAUTIFY_BLOOM_USE_DEPTH
#pragma multi_compile __ BEAUTIFY_BLOOM_USE_LAYER
#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #include "BeautifyLum.cginc"
      ENDCG
  }  
  
  
  Pass { // 3
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragDebugBloom
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyLum.cginc"
      ENDCG
  }   
  
  Pass { // 4
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertBlurH
      #pragma fragment fragBlur
      #pragma fragmentoption ARB_precision_hint_fastest
	  #pragma target 3.0
      #include "BeautifyLum.cginc"
      ENDCG
  }    
  
      
  Pass { // 5
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertBlurV
      #pragma fragment fragBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyLum.cginc"
      ENDCG
  }    

            
  Pass { // 6
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragBloomCompose
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyLum.cginc"
      ENDCG
  }   
  
  Pass { // 7
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragResample
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyLum.cginc"
      ENDCG
  }   
    
  Pass { // 8
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  Blend One One
	  
      CGPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragResample
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyLum.cginc"
      ENDCG
  } 


  Pass { // 9
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertCrossLum
      #pragma fragment fragLumAntiflicker
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
#pragma multi_compile __ BEAUTIFY_BLOOM_USE_DEPTH
#pragma multi_compile __ BEAUTIFY_BLOOM_USE_LAYER
#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #include "BeautifyLum.cginc"
      ENDCG
  }  

  Pass { // 10
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  Blend One One

      CGPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragResampleAF
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyLum.cginc"
      ENDCG
  } 

  Pass { // 11
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  Blend One One

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragCopy
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyLum.cginc"
      ENDCG
  } 

  Pass { // 12 DoF CoC
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragCoC
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
#pragma multi_compile __ BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #include "BeautifyDoF.cginc"
      ENDCG
  } 
 
  Pass { // 13 DoF CoC Debug
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragCoCDebug
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
#pragma multi_compile __ BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
      #include "BeautifyDoF.cginc"
      ENDCG
  } 
 
  Pass { // 14 DoF Blur
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyDoF.cginc"
      ENDCG
  }    
  
 Pass { // 15 Compute Screen Lum
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragScreenLum
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #include "BeautifyEA.cginc"
      ENDCG
  }      
  
  Pass { // 16 Reduce Screen Lum
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragReduceScreenLum
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyEA.cginc"
      ENDCG
  }  
  
  Pass { // 17 Blend Screen Lum
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  Blend SrcAlpha OneMinusSrcAlpha
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragBlendScreenLum
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyEA.cginc"
      ENDCG
  }    
      
  Pass { // 18 Simple Blend
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
//	  Blend SrcAlpha OneMinusSrcAlpha
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragBlend
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyEA.cginc"
      ENDCG
  }  
 
  Pass { // 19 DoF Blur wo/Bokeh
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragBlurNoBokeh
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyDoF.cginc"
      ENDCG
  }    
  
  Pass { // 20 Sun Flares
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragSF
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifySF.cginc"
      ENDCG
  }
  
    Pass { // 21 Sun Flares Additive
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragSFAdditive
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifySF.cginc"
      ENDCG
  }

  Pass { // 22  Raw copy used in single blits with Single Pass Instanced
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragCopy
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyLum.cginc"
      ENDCG
  } 


Pass { // 23 DoF Blur CoC
      ZTest Always Cull Off ZWrite Off
      Fog { Mode Off }
      
      CGPROGRAM
      #pragma vertex vertBlurH
      #pragma fragment fragBlurCoC
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyDoF.cginc"
      ENDCG
  }    

Pass { // 24 DoF Blur CoC
      ZTest Always Cull Off ZWrite Off
      Fog { Mode Off }
      
      CGPROGRAM
      #pragma vertex vertBlurV
      #pragma fragment fragBlurCoC
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #include "BeautifyDoF.cginc"
      ENDCG
  }    
  
 }
FallBack Off
}
