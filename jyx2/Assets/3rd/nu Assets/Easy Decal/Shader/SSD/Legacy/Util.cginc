//-----------------------------------------------------
// Common screen space decal includes. Version 0.8 [Beta]
// Copyright (c) 2017 by Sycoforge
//-----------------------------------------------------

#ifndef SSD_SYCO_UTIL_CG_INCLUDED
#define SSD_SYCO_UTIL_CG_INCLUDED

#define V_RIGHT float3(1.0f, 0.0f, 0.0f)
#define V_UP float3(0.0f, 1.0f, 0.0f)
#define V_FORWARD float3(0.0f, 0.0f, 1.0f)

float DistanceFromPlane (float3 pos, float4 plane)
{
	return dot (float4(pos, 1.0f), plane);

}

// Returns true if triangle with given 3 world positions is outside of camera's view frustum.
// threshold is distance outside of frustum that is still considered to be inside (i.e. max displacement)
bool IsInsideFrustum (float3x3 tri, float threshold)
{    
	bool4 planeTest;
	
	// left
	planeTest.x = (( DistanceFromPlane(tri[0], unity_CameraWorldClipPlanes[0]) > -threshold) ? 1.0f : 0.0f ) +
				  (( DistanceFromPlane(tri[1], unity_CameraWorldClipPlanes[0]) > -threshold) ? 1.0f : 0.0f ) +
				  (( DistanceFromPlane(tri[2], unity_CameraWorldClipPlanes[0]) > -threshold) ? 1.0f : 0.0f );
	// right
	planeTest.y = (( DistanceFromPlane(tri[0], unity_CameraWorldClipPlanes[1]) > -threshold) ? 1.0f : 0.0f ) +
				  (( DistanceFromPlane(tri[1], unity_CameraWorldClipPlanes[1]) > -threshold) ? 1.0f : 0.0f ) +
				  (( DistanceFromPlane(tri[2], unity_CameraWorldClipPlanes[1]) > -threshold) ? 1.0f : 0.0f );
	// top
	planeTest.z = (( DistanceFromPlane(tri[0], unity_CameraWorldClipPlanes[2]) > -threshold) ? 1.0f : 0.0f ) +
				  (( DistanceFromPlane(tri[1], unity_CameraWorldClipPlanes[2]) > -threshold) ? 1.0f : 0.0f ) +
				  (( DistanceFromPlane(tri[2], unity_CameraWorldClipPlanes[2]) > -threshold) ? 1.0f : 0.0f );
	// bottom
	planeTest.w = (( DistanceFromPlane(tri[0], unity_CameraWorldClipPlanes[3]) > -threshold) ? 1.0f : 0.0f ) +
				  (( DistanceFromPlane(tri[1], unity_CameraWorldClipPlanes[3]) > -threshold) ? 1.0f : 0.0f ) +
				  (( DistanceFromPlane(tri[2], unity_CameraWorldClipPlanes[3]) > -threshold) ? 1.0f : 0.0f );
		
	// has to pass all 4 plane tests to be visible
	return !all (planeTest);
}

inline float4x4 inverseMat(float4x4 mat)
{
	float s0 = mat[0][0] * mat[1][1] - mat[1][0] * mat[0][1];
	float s1 = mat[0][0] * mat[1][2] - mat[1][0] * mat[0][2];
	float s2 = mat[0][0] * mat[1][3] - mat[1][0] * mat[0][3];
	float s3 = mat[0][1] * mat[1][2] - mat[1][1] * mat[0][2];
	float s4 = mat[0][1] * mat[1][3] - mat[1][1] * mat[0][3];
	float s5 = mat[0][2] * mat[1][3] - mat[1][2] * mat[0][3];

	float c5 = mat[2][2] * mat[3][3] - mat[3][2] * mat[2][3];
	float c4 = mat[2][1] * mat[3][3] - mat[3][1] * mat[2][3];
	float c3 = mat[2][1] * mat[3][2] - mat[3][1] * mat[2][2];
	float c2 = mat[2][0] * mat[3][3] - mat[3][0] * mat[2][3];
	float c1 = mat[2][0] * mat[3][2] - mat[3][0] * mat[2][2];
	float c0 = mat[2][0] * mat[3][1] - mat[3][0] * mat[2][1];

	float d = (s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0);

	d = d <= 0.0f ? 1.0f : d;

	float invdet = 1.0f / d;

	float4x4 b;

	b[0][0] = ( mat[1][1] * c5 - mat[1][2] * c4 + mat[1][3] * c3) * invdet;
	b[0][1] = (-mat[0][1] * c5 + mat[0][2] * c4 - mat[0][3] * c3) * invdet;
	b[0][2] = ( mat[3][1] * s5 - mat[3][2] * s4 + mat[3][3] * s3) * invdet;
	b[0][3] = (-mat[2][1] * s5 + mat[2][2] * s4 - mat[2][3] * s3) * invdet;

	b[1][0] = (-mat[1][0] * c5 + mat[1][2] * c2 - mat[1][3] * c1) * invdet;
	b[1][1] = ( mat[0][0] * c5 - mat[0][2] * c2 + mat[0][3] * c1) * invdet;
	b[1][2] = (-mat[3][0] * s5 + mat[3][2] * s2 - mat[3][3] * s1) * invdet;
	b[1][3] = ( mat[2][0] * s5 - mat[2][2] * s2 + mat[2][3] * s1) * invdet;

	b[2][0] = ( mat[1][0] * c4 - mat[1][1] * c2 + mat[1][3] * c0) * invdet;
	b[2][1] = (-mat[0][0] * c4 + mat[0][1] * c2 - mat[0][3] * c0) * invdet;
	b[2][2] = ( mat[3][0] * s4 - mat[3][1] * s2 + mat[3][3] * s0) * invdet;
	b[2][3] = (-mat[2][0] * s4 + mat[2][1] * s2 - mat[2][3] * s0) * invdet;

	b[3][0] = (-mat[1][0] * c3 + mat[1][1] * c1 - mat[1][2] * c0) * invdet;
	b[3][1] = ( mat[0][0] * c3 - mat[0][1] * c1 + mat[0][2] * c0) * invdet;
	b[3][2] = (-mat[3][0] * s3 + mat[3][1] * s1 - mat[3][2] * s0) * invdet;
	b[3][3] = ( mat[2][0] * s3 - mat[2][1] * s1 + mat[2][2] * s0) * invdet;

	return b;
}

#endif // SSD_SYCO_UTIL_CG_INCLUDED