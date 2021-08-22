// Simplified Additive Particle shader. Differences from regular Additive Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

// Cartoon FX difference:
// - uses Alpha8 monochrome textures to save up on texture memory size

Shader "Cartoon FX/Mobile Particles Additive Alpha8"
{
Properties
{
	_MainTex ("Particle Texture (Alpha8)", 2D) = "white" {}
}

Category
{
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	BindChannels
	{
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader
	{
		Pass
		{
			SetTexture [_MainTex]
			{
				combine primary, texture * primary
			}
		}
	}
}
}