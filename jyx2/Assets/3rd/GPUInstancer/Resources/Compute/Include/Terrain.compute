#ifndef __terrain_hlsl_
#define __terrain_hlsl_

// Bilinear Interpolation
float Blerp(float c00, float c10, float c01, float c11, float tx, float ty)
{
    return lerp(lerp(c00, c10, tx), lerp(c01, c11, tx), ty);
}

// Get height for specified coordinates
float SampleHeight(float2 p, float leftBottomH, float leftTopH, float rightBottomH, float rightTopH)
{
    return Blerp(leftBottomH, rightBottomH, leftTopH, rightTopH, p.x, p.y); 
}

// Get normal vector from specified heighmap data
float3 ComputeNormals(float leftBottomH, float leftTopH, float rightBottomH, float scale)
{
    float3 P = float3(0, leftBottomH * scale, 0);
    float3 Q = float3(0, leftTopH * scale, 1);
    float3 R = float3(1, rightBottomH * scale, 0);
    
    return normalize(cross(Q - R, R - P));
}
#endif