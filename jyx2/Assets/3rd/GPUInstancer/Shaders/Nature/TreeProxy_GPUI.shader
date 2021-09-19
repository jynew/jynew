Shader "Hidden/GPUInstancer/Nature/TreeProxy"
{
	SubShader
	{
		Tags 
		{ 
			"Queue"="Transparent" 
			"RenderType"="Transparent"
			//"ForceNoShadowCasting"="True"
			//"IgnoreProjector"="True"
			//"LightMode"="Always" 
		}
		LOD 100
		
		//ZTest Always
        Cull Off
        //ZWrite Off
        //Fog { Mode off }
        //Blend Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			void vert () { }
			
			void frag ()
			{
				discard;
			}
			ENDCG
		}
	}
}

