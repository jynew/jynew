// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Canvas shader utils. Version 1.3
// Copyright (c) by Sycoforge

#define RGB_ 0
#define RGBA_ 1
#define R_ 2
#define G_ 3
#define B_ 4
#define A_ 5
#define Grayscale_ 6

struct appdata_t 
{
	float4 vertex : POSITION;
	float2 texcoord : TEXCOORD0;
};

struct v2f 
{
	float4 vertex : SV_POSITION;
	half2 texcoord : TEXCOORD0;
};

sampler2D _MainTex;
float4 _MainTex_ST;
float4 _MainTex_TexelSize;
half _OffsetX;
half _OffsetY;
half _Tile;
half _ClampPreview;

float4 _SeamColor;

			
v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
	return o;
}

inline half2 TransformUV(float2 uv)
{
	half2 offset = half2(-_OffsetX, -_OffsetY);
	//half2 offset = half2(-_OffsetX, _OffsetY);
	//half2 offset = half2(_OffsetX, _OffsetY);
	half2 uv_ = (uv + offset) * _Tile;

	return uv_;
}

//Returns 1 when inside specified rectangle, otherwise 0.
// rectangle: x: xMin, y: yMin, z: xMax, w: yMax
fixed InsideRect(fixed2 uv, fixed4 rectangle)
{
	fixed inside = uv.x >= rectangle.x && uv.x <= rectangle.z && uv.y >= rectangle.y && uv.y <= rectangle.w;

	return inside;
}

//Returns 1 on the first pixel of the specified rectangle, otherwise 0.
// rectangle: x: xMin, y: yMin, z: xMax, w: yMax
fixed2 RectBorder(fixed2 uv, fixed4 rectangle, fixed2 texelSize)
{
	fixed cUp = (uv.y - texelSize.y) < rectangle.y && uv.y >= rectangle.y;
	fixed cDown = (uv.y + texelSize.y) > rectangle.w && uv.y <= rectangle.w;

	fixed cLeft = (uv.x - texelSize.x) < rectangle.x && uv.x >= rectangle.x;
	fixed cRight = (uv.x + texelSize.x) > rectangle.z && uv.x <= rectangle.z;
					
	fixed vertical = cUp + cDown;
	fixed horizontal = cLeft + cRight;

	return fixed2(vertical, horizontal);
}

//Returns 1 on the first pixel of the specified rectangle, otherwise 0.
// rectangle: x: xMin, y: yMin, z: xMax, w: yMax
fixed RectBorderClampedCombined(fixed2 uv, fixed4 rectangle, fixed2 texelSize)
{
	fixed2 b = RectBorder(uv, rectangle, texelSize);
	return (b.x + b.y) * InsideRect(uv, rectangle);
}

fixed3 RectBorderClamped(fixed2 uv, fixed4 rectangle, fixed2 texelSize)
{
	fixed2 b = RectBorder(uv, rectangle, texelSize);
	return fixed3(b.x, b.y, InsideRect(uv, rectangle));
}

inline float4 ClampClipSpace(half2 uv, float4 color)
{
	float2 uv_ = uv;
	bool valid = (uv_.x >= 0 && uv_.x <= 1) && (uv_.y >= 0 && uv_.y <= 1);

	float4 white = float4(1.0f, 1.0f, 1.0f, 0.0f);

	color = (valid && _ClampPreview) || !_ClampPreview ? color : white;

	return color;	
}

float4 DrawSeam(half2 uv, float4 color)
{
	int2 ab = (int2)uv;
	fixed2 uvc = (uv - ab);
	uvc.x = ab.x == 0 ? uvc.x : abs(uv.x - ab.x);
	uvc.y = ab.y == 0 ? uvc.y : abs(uv.y - ab.y);


	fixed2 scale = _MainTex_TexelSize.xy * _Tile;


	if((uvc.x < scale.x && uvc.x > 0) || (uvc.y < scale.y && uvc.y > 0))
	{
		color = _SeamColor.a * _SeamColor + (1.0 - _SeamColor.a) * color;
	}

	return color;
}
			
