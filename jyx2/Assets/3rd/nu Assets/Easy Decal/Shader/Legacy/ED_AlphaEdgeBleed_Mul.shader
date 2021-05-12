// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Easy Decal/Legacy/Alpha Edge Bleed Multiply" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Factor ("Fade Out", Range(1,2)) = 1
	}
	
	Category 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend Zero SrcColor
		Offset -1,-1
		Cull Back 
		Lighting Off 
		ZWrite Off 

		SubShader   
		{
			Pass 
			{
				 CGPROGRAM
				 
				 #pragma vertex vert  
				 #pragma fragment frag 
				 #pragma multi_compile_fog


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
					fixed4 color : COLOR;

					UNITY_FOG_COORDS(1)
				 };
 
				 vertexOutput vert(vertexInput input) 
				 {
					vertexOutput output;
 
					output.pos =  UnityObjectToClipPos(input.vertex);
					output.uv = TRANSFORM_TEX(input.uv, _MainTex);
					output.color = input.color;

					UNITY_TRANSFER_FOG(output, output.pos);

					return output;
				 }
 
				 float4 frag(vertexOutput input) : COLOR 
				 {
					fixed4 color;

					//Bleed threshold
					if(input.color.a < 0.999)
					{
						input.color = input.color * float4(1.0, 1.0, 1.0, _Factor);
					}

					half4 c = tex2D(_MainTex, input.uv) * input.color;

					color = lerp(half4(1,1,1,1), c, c.a);

					UNITY_APPLY_FOG_COLOR(input.fogCoord, color, fixed4(1,1,1,1));



					return color;
				 }
 
				 ENDCG  

			}
		} 
	}
}