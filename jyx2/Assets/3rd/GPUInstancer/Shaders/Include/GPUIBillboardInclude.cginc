#ifndef GPU_INSTANCER_BILLBOARD_INCLUDED
#define GPU_INSTANCER_BILLBOARD_INCLUDED

void GPUIBillboardVertex(inout float4 vertex, inout float3 normal, inout float4 tangent, inout float3 tangentWorld, inout float3 bitangentWorld,
                         inout float3 normalWorld, inout float4 texcoord, inout float2 atlasUV, float frameCount)
{
    
    float3 billboardCameraPos = _WorldSpaceCameraPos;
    
    // If shadowcaster, account for billboard:
    //#if defined(UNITY_PASS_SHADOWCASTER)
        // _WorldSpaceLightPos0.w is 0 for directional lights only. Use light vector instead of cam vector if that is the case:
        // billboardCameraPos = _WorldSpaceCameraPos * (1 - _WorldSpaceLightPos0.w) + (_WorldSpaceLightPos0.w * _WorldSpaceLightPos0.xyz);
    //#endif

    float3 billboardCameraDir = normalize(mul((float3x3) unity_WorldToObject, billboardCameraPos - unity_ObjectToWorld._m03_m13_m23));

    // get current camera angle in radians
    float angle = atan2(-billboardCameraDir.z, billboardCameraDir.x);
    
    // calculate current frame index and set uvs
    float frameIndex = round((angle - UNITY_PI / 2) / (UNITY_TWO_PI / frameCount));
    atlasUV = texcoord.xy * float2(1 / frameCount, 1) + float2((1 / frameCount) * frameIndex, 0);

    // calculate camera vectors
    float3 up = float3(0, 1, 0);
    float3 forward = -normalize(UNITY_MATRIX_V._m20_m21_m22);

    #ifdef _BILLBOARDFACECAMPOS_ON
        float3 right = normalize(cross(float3(0, 1, 0), unity_ObjectToWorld._m03_m13_m23 - billboardCameraPos));
    #else
        float3 right = normalize(UNITY_MATRIX_V._m00_m01_m02);
	    // adjust rotation matrix if camera is upside down
        right *= sign(normalize(UNITY_MATRIX_V._m10_m11_m12).y);
    #endif	

	// create camera rotation matrix
    float4x4 rotationMatrix = float4x4(right, 0, up, 0, forward, 0, 0, 0, 0, 1);
    
	// rotate to camera lookAt
    vertex.x *= length(unity_ObjectToWorld._m00_m10_m20);
    vertex.y *= length(unity_ObjectToWorld._m01_m11_m21);
    vertex.z *= length(unity_ObjectToWorld._m02_m12_m22);
    vertex = mul(vertex, rotationMatrix);
			
	// account for world position
    vertex.xyz += unity_ObjectToWorld._m03_m13_m23;
			
	// ignore initial object rotation for the billboard
    vertex = mul(unity_WorldToObject, vertex);
    vertex.y += (1 / frameCount * unity_ObjectToWorld[3].y + unity_ObjectToWorld[3].z) * length(unity_ObjectToWorld._m01_m11_m21);
    
    // adjust normal and tangents
    normal = -billboardCameraDir;
    tangent.xyz = normalize(cross(up, normal));
    tangent.w = -1;

    // interpolate normal and tangents to surface func
    normalWorld = normal;
    tangentWorld = tangent.xyz;
    bitangentWorld = cross(normalWorld, tangentWorld) * (tangent.w * unity_WorldTransformParams.w);
}

half4 GPUIBillboardNormals(sampler2D _NormalAtlas, float2 uvAtlas, float frameCount, float3 tangentWorld, float3 bitangentWorld, float3 normalWorld)
{
    // read Normal Atlas
    half4 normalTexture = tex2D(_NormalAtlas, uvAtlas);

    // remap normalTexture back to [-1, 1]
    half3 unpackedNormalTexture = (normalTexture.xyz * 2.0 - 1.0);

    // modify normal map with the billboard world vectors
    float3 normal = normalize(mul(half3x3(tangentWorld, bitangentWorld, normalWorld), unpackedNormalTexture));

    // calculate depth
    half depth = max(normalTexture.w - 0.35, 0);

    return half4(normal, depth);
}

void CalculateHueVariation(half4 hueColor, inout half4 originalColor)
{
    float3 worldPosActual = unity_ObjectToWorld._m03_m13_m23;
    float hueVariationAmount = frac(worldPosActual.x + worldPosActual.y + worldPosActual.z);
    hueVariationAmount = saturate(hueVariationAmount * hueColor.a);
    half3 shiftedColor = lerp(originalColor.rgb, hueColor.rgb, hueVariationAmount);
    
    half maxBase = max(originalColor.r, max(originalColor.g, originalColor.b));
    half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));
    maxBase /= newMaxBase;
    maxBase = maxBase * 0.5f + 0.5f;

    shiftedColor.rgb *= maxBase;
    originalColor.rgb = saturate(shiftedColor);
}

// Does not calculate worldPos in the surface function
void CalculateHueVariationVulkan(half4 hueColor, inout half4 originalColor, float3 worldPos)
{
    float hueVariationAmount = frac(worldPos.x + worldPos.y + worldPos.z);
    
	// Vulkan surface shader worldpos fix 
	if (hueVariationAmount > 0.999999){
		hueVariationAmount = 0;
	}

    hueVariationAmount = saturate(hueVariationAmount * hueColor.a);
    half3 shiftedColor = lerp(originalColor.rgb, hueColor.rgb, hueVariationAmount);
    
    half maxBase = max(originalColor.r, max(originalColor.g, originalColor.b));
    half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));
    maxBase /= newMaxBase;
    maxBase = maxBase * 0.5f + 0.5f;

    shiftedColor.rgb *= maxBase;
    originalColor.rgb = saturate(shiftedColor);
}

inline float LinearToGammaExact(float value)
{
    if (value <= 0.0F)
        return 0.0F;
    else if (value <= 0.0031308F)
        return 12.92F * value;
    else if (value < 1.0F)
        return 1.055F * pow(value, 0.4166667F) - 0.055F;
    else
        return pow(value, 0.45454545F);
}

// uses the exact (and more expansive) version of LinearToGammaSpace. Used only for baking.
inline half3 LinearToGamma(half3 linRGB)
{
    linRGB = max(linRGB, half3(0.h, 0.h, 0.h));
	
    // Exact version of the LineatToGammeSpace from UnityCG.cginc:
    return half3(LinearToGammaExact(linRGB.r), LinearToGammaExact(linRGB.g), LinearToGammaExact(linRGB.b));
}

#endif