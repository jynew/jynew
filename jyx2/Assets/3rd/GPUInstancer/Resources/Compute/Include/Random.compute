#ifndef __random_hlsl_
#define __random_hlsl_

static const float hashScale1 = 0.1031;
static const float3 hashScale2 = float3(0.1031, 0.1030, 0.0973);

float randomFloat(float p)
{
    float3 p3 = frac(abs(float3(p, p, p)) * hashScale1);
    p3 += dot(p3, p3.yzx + 19.19);
    return frac((p3.x + p3.y) * p3.z);
}

float2 randomFloat2(float2 p)
{
    float3 p3 = frac(abs(p.xyx) * hashScale2);
    p3 += dot(p3, p3.yzx + 19.19);
    return frac((p3.xx + p3.yz) * p3.zy);
}

#endif