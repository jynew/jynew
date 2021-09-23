	Shader "GPUInstancer/PaintTextureAdvance"{

	Properties{ _MainTex("Texture", any) = "" {} }

	SubShader{
			
		ZTest Always Cull Off ZWrite Off

		CGINCLUDE
			#include "UnityCG.cginc"
			#include "Packages/com.unity.terrain-tools/Shaders/TerrainTools.hlsl"


			sampler2D _MainTex;
			float4 _MainTex_TexelSize;      // 1/width, 1/height, width, height

			sampler2D _BrushTex;
			sampler2D _FilterTex;

			float4 _BrushParams;
			#define BRUSH_STRENGTH      (_BrushParams[0])
			#define BRUSH_TARGETHEIGHT  (_BrushParams[1])
			
			struct appdata_t {
				float4 vertex : POSITION;
				float2 pcUV : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 pcUV : TEXCOORD0;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.pcUV = v.pcUV;
				return o;
			}
		ENDCG

		Pass    // 0 paint splat alphamap
		{
			Name "Paint splat texture"

			CGPROGRAM
#include "UnityCG.cginc"
#include "./../../Shaders/Include/GPUInstancerInclude.cginc"
#pragma instancing_options procedural:setupGPUI
#pragma multi_compile_instancing
			#pragma vertex vert
			#pragma fragment PaintSplatAlphamap

			float ApplyBrush(float height, float brushStrength)
			{
				float targetHeight = 1.0f;
				if (targetHeight > height)
				{
					height += brushStrength;
					height = height < targetHeight ? height : targetHeight;
				}
				else
				{
					height -= brushStrength;
					height = height > targetHeight ? height : targetHeight;
				}
				return height;
			}

			float4 PaintSplatAlphamap(v2f i) : SV_Target
			{
				float2 brushUV = PaintContextUVToBrushUV(i.pcUV);
				float2 normalUV = PaintContextUVToBrushUV(i.pcUV);

				// out of bounds multiplier
				float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;

				float brushStrength = BRUSH_STRENGTH * oob * UnpackHeightmap(tex2D(_FilterTex, i.pcUV)) * UnpackHeightmap(tex2D(_BrushTex, brushUV.xy));
				float splatMap = tex2D(_MainTex, i.pcUV).r;
				float targetAlpha = BRUSH_TARGETHEIGHT;

				return lerp(splatMap, targetAlpha, brushStrength);
			}

			ENDCG
		}
	}
	Fallback Off
}
