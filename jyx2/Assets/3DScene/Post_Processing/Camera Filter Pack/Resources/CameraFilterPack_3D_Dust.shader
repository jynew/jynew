// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////
Shader "CameraFilterPack/3D_Dust" {
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_TimeX("Time", Range(0.0, 1.0)) = 1.0
		_Distortion("_Distortion", Range(0.0, 1.00)) = 1.0
		_ScreenResolution("_ScreenResolution", Vector) = (0.,0.,0.,0.)
		_ColorRGB("_ColorRGB", Color) = (1,1,1,1)

	}
		SubShader
	{
		Pass
	{
Cull Off ZWrite Off ZTest Always
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0
#include "UnityCG.cginc"


		uniform sampler2D _MainTex;
		uniform float _Visualize;
	uniform float _TimeX;
	uniform float _Distortion;
	uniform float4 _ScreenResolution;
	uniform float4 _ColorRGB;
	uniform float _Near;
	uniform float _Far;
	uniform float _FarCamera;
	uniform sampler2D _CameraDepthTexture;
	uniform float _FixDistance;
	uniform float _LightIntensity;
	uniform float _FadeShield;
	uniform float2 _MainTex_TexelSize;
	uniform float _Value;
	uniform float _Value2;
	uniform float _Value3;
	uniform float _Value4;
	uniform float _TestX;
	uniform float _TestY;
	uniform float _TestZ;

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float2 texcoord  : TEXCOORD0;
		float4 vertex   : SV_POSITION;
		float4 color : COLOR;
		float3 wpos : TEXCOORD1;
		float3 vpos : TEXCOORD2;
		float4 projPos : TEXCOORD3;
	};

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color;
		OUT.projPos = ComputeScreenPos(OUT.vertex);
		float3 worldPos = mul(unity_ObjectToWorld, OUT.vertex).xyz;
		OUT.vpos = OUT.vertex.xyz;
		OUT.wpos = worldPos;
		return OUT;
	}



#define time _Time*.5

	float march(float depth)
	{
		float precis = 0.002;
		float h = precis*2.0;
		float d = 0.;
		for (int i = 0; i<100; i++)
		{
			if (abs(h)<precis || d>30) break;
			d += h;
			d += h;
			float res = depth;
			h = res;
		}
		return d;
	}

	float tri(in float x) { return abs(frac(x) - .5); }
	float3 tri3(in float3 p) { return float3(tri(p.z + tri(p.y*1.)), tri(p.z + tri(p.x*1.)), tri(p.y + tri(p.x*1.))); }

	float2x2 m2 = float2x2(0.970, 0.242, -0.242, 0.970);

	float triNoise3d(in float3 p, in float spd)
	{
		float z = 1.4;
		float rz = 0.;
		float3 bp = p;
		for (float i = 0.; i <= 3.; i++)
		{
			float3 dg = tri3(bp*2.);
			p += (dg + time*spd);

			bp *= 1.8;
			z *= 1.5;
			p *= 1.2;

			rz += (tri(p.z + tri(p.x + tri(p.y)))) / z;
			bp += 0.14;
		}
		return rz;
	}

	float fogmap(in float3 p, in float d)
	{
		p.x += time*1.5;
		p.z += sin(p.x*.5);
		return triNoise3d(p*2.2 / (d + 20.), 0.2)*(1. - smoothstep(0., .7, p.y));
	}

	float3 fog(in float3 col, in float3 ro, in float3 rd, in float mt)
	{
		float d = 0.5;
		for (int i = 0; i<7; i++)
		{
			float3  pos = ro + rd*d;
			float rz = fogmap(pos, d);
			float grd = clamp((rz - fogmap(pos + .8 - float(i)*0.1, d))*3., 0.1, 1.);
			float3 col2 = float3(1 - grd, 1 - grd, 1 - grd);
			col = lerp(col, col2, clamp(rz*smoothstep(d - 0.4, d + 2. + d*.75, mt), 0., 1.));
			d *= 1.5 + 0.3;

		}
		return col;
	}



	inline float3 UnityWorldSpaceViewDir2(in float3 worldPos)
	{
		float3 wpos;
		wpos.x = unity_CameraToWorld[0][1];
		wpos.y = unity_CameraToWorld[1][1];
		wpos.z = unity_CameraToWorld[2][1];
		return wpos - worldPos;
	}

	float3 rotx(float3 p, float a) 
	{
		float s = sin(a), c = cos(a);
		return float3(p.x, c*p.y - s*p.z, s*p.y + c*p.z);
	}
	
	float3 roty(float3 p, float a) 
	{
		float s = sin(a), c = cos(a);
		return float3(c*p.x + s*p.z, p.y, -s*p.x + c*p.z);
	}

	float3 rotz(float3 p, float a) 
	{
		float s = sin(a), c = cos(a);
		return float3(c*p.x - s*p.y, s*p.x + c*p.y, p.z);
	}

	half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
		float2 uv = uvst.xy;
#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1 - uv.y;
#endif

		float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
		depth /= _FixDistance * 10;
		float ss = smoothstep(_Near, saturate(_Near * 4 + _Far * 4), depth);
		depth = ss;
		
		if (_Visualize == 1) return depth;

		float4 txt = tex2D(_MainTex, uv);
		float2 p = uvst.xy - 0.5;
		p.x *= 4;

		float3 mo = float3(0, 0, 0);
		float3 ro = float3(0, 0, 0);
		float3 ri = float3(1, 0, 0);

		ro.x += _WorldSpaceCameraPos.z;
		ro.y += _WorldSpaceCameraPos.y;
		ro.z += _WorldSpaceCameraPos.x;

		_TestX /= 512;
		_TestY /= 512;
		_TestZ /= 512;

		ri.x += _TestX;
		ri.y += _TestY;
		ri.z += _TestZ;

		float3 mi = float3(0, 0, 0);
		mi = rotx(mo, 0.15 + _TestX*3.14*2.);
		mi = roty(mo, 1.5 + _TestY*3.14*2.);
		mi = rotz(mo, 1.5 + _TestZ*3.14*2.);
		 
		float3 ww = normalize(UnityWorldSpaceViewDir2(i.wpos));

		float3 eyedir = normalize(ri);
		float3 rightdir = normalize(mi);
		float3 updir = normalize(cross(rightdir, eyedir));
		float3 rd = normalize((p.x*rightdir + p.y*updir)*1. + eyedir);
		float rz = march(depth);

		float3 col = float3(0, 0, 0);

		col = fog(col, ro, rd, rz);
		txt += float4(col, 1.0);

		return txt;
	}

		ENDCG
	}

	}
}