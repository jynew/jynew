Shader "Beautify/BeautifyBasic" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Sharpen ("Sharpen Data", Vector) = (2.5, 0.035, 0.5)
		_ColorBoost ("Color Boost Data", Vector) = (1.1, 1.1, 0.08, 0)
		_CompareTex ("Compare Image (RGB)", 2D) = "black" {}
	}

Subshader {	

  Pass { // 0
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertCompare
      #pragma fragment fragCompareFast
	  #pragma fragmentoption ARB_precision_hint_fastest      
      #include "BeautifyBasic.cginc"
      ENDCG
  }
 
  Pass { // 1
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragBeautifyFast
	  #pragma fragmentoption ARB_precision_hint_fastest      
	  #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #include "BeautifyBasic.cginc"
      ENDCG
  }

}
FallBack Off
}
