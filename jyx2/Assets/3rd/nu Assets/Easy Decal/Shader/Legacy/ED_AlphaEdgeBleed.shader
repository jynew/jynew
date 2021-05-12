// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Easy Decal/Legacy/Alpha Edge Bleed" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Factor ("Fade Out", Range(1,2)) = 1
	}
	
	Category 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off
		Offset -1,-1
		Alphatest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha 
		Fog { Color(0,0,0,0) }
		Lighting Off
		Cull Back 

		SubShader   
		{
			Pass 
			{
				 CGPROGRAM
				 
				 #pragma vertex vert  
				 #pragma fragment frag 

				 #include "UnityCG.cginc"

				 uniform sampler2D _MainTex;
				 uniform float4 _MainTex_ST; 
				 float _Factor;

 
				 struct vertexInput 
				 {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					fixed4 color : COLOR;
				 };
				 struct vertexOutput 
				 {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 posInObjectCoords : TEXCOORD1;
					fixed4 color : COLOR;
				 };
 
				 vertexOutput vert(vertexInput input) 
				 {
					vertexOutput output;
 
					output.pos =  UnityObjectToClipPos(input.vertex);
					output.posInObjectCoords = input.vertex; 
					output.uv = TRANSFORM_TEX(input.uv, _MainTex);
					output.color = input.color;

					return output;
				 }
 
				 float4 frag(vertexOutput input) : COLOR 
				 {
					//Bleed threshold
					if(input.color.a < 0.999)
					{
						input.color = input.color * float4(1.0, 1.0, 1.0, _Factor);
					}

					float4 c = tex2D(_MainTex, input.uv) * input.color;

					return  c;
				 }
 
				 ENDCG  

			}
		} 
	}
}