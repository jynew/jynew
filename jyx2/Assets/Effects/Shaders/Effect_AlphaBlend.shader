Shader "MaShen/Effect/Legacy/AlphaBlend" {
	Properties {
		_TintColor ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Particle Texture", 2D) = "white" {}
	}
	
	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		SubShader {
			Pass {
			
				SetTexture [_MainTex]
				{
					constantColor [_TintColor]
					combine Constant * primary
				}
				
				SetTexture [_MainTex]
				{
					combine texture * previous DOUBLE
				}
			}
		}
	}
}
