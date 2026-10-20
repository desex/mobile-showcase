#ifndef SMOOTH_VORONOI_INCLUDED
#define SMOOTH_VORONOI_INCLUDED

float2 hash2_float(float2 p)
{
    p = float2
    (
        dot(p, float2(127.1, 311.7)),
        dot(p, float2(269.5, 183.3))
    );
    return frac(sin(p) * 43758.5453);
}

void SmoothVoronoi_float(float2 uv, float angleOffset, float smoothness, out float result)
{
    float2 n = floor(uv);
    float2 f = frac(uv);
    smoothness = max(smoothness, 1e-3);

    result = 8.0;
    for (int j = -2; j <= 2; j++)
    {
        for (int i = -2; i <= 2; i++)
        {
            float2 g = float2(i, j);
            float2 o = hash2_float(n + g);
            o = 0.5 + 0.5 * sin(6.28318530 * o + angleOffset);
            
            float d = length(g - f + o);
            
            float h = smoothstep(-1.0, 1.0, (result - d) / smoothness);
            result = lerp(result, d, h) - h * (1.0 - h) * smoothness / (1.0 + 3.0 * smoothness);
        }
    }
}

half2 hash2_half(half2 p)
{
    p = half2
    (
        dot(p, half2(127.1, 311.7)),
        dot(p, half2(269.5, 183.3))
    );
    return frac(sin(p) * 43758.5453);
}

void SmoothVoronoi_half(half2 uv, half angleOffset, half smoothness, out half result)
{
    half2 n = floor(uv);
    half2 f = frac(uv);
    smoothness = max(smoothness, 1e-3);

    result = 8.0;
    for (int j = -2; j <= 2; j++)
    {
        for (int i = -2; i <= 2; i++)
        {
            half2 g = half2(i, j);
            half2 o = hash2_half(n + g);
            o = 0.5 + 0.5 * sin(6.28318530 * o + angleOffset);
            
            half d = length(g - f + o);
            
            half h = smoothstep(-1.0, 1.0, (result - d) / smoothness);
            result = lerp(result, d, h) - h * (1.0 - h) * smoothness / (1.0 + 3.0 * smoothness);
        }
    }
}
#endif
