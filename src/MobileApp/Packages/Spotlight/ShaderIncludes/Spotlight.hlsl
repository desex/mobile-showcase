#ifndef SPOTLIGHT_INCLUDED
#define SPOTLIGHT_INCLUDED

#include "SDF2D.hlsl"

#define SPOTLIGHT_MAX_CUTOUTS 16

float4 _SpotlightCutoutRects[SPOTLIGHT_MAX_CUTOUTS];
int _SpotlightCutoutsCount;
float _SpotlightCutoutsSmoothness;
float _SpotlightCutoutsRadius;
float _SpotlightAspect;

float spotlightMask(const in float2 uv)
{
    float result = 1.0;
                
    for (int i = 0; i < _SpotlightCutoutsCount && i < SPOTLIGHT_MAX_CUTOUTS; i++)
    {
        const float4 cutout = _SpotlightCutoutRects[i];
        const float2 cutoutSize = float2(cutout.z - cutout.x, cutout.w - cutout.y);
        const float2 cutoutHalfSize = cutoutSize * 0.5;
        const float2 cutoutCenter = cutout.xy + cutoutHalfSize;
        
        const float2 rectanglePosition = (uv - cutoutCenter) / float2(1, _SpotlightAspect);
        
        const float2 halfSize = cutoutHalfSize / float2(1, _SpotlightAspect);
        const float maxDepth = min(halfSize.x, halfSize.y);
        const float fade = _SpotlightCutoutsSmoothness * maxDepth;
        const float radius = min(max(_SpotlightCutoutsRadius, fade), maxDepth);
        const float rectangle = SdfRectangle(rectanglePosition, halfSize - radius) - radius;
        result = min(result, smoothstep(-max(fade, 1e-5), 0, rectangle));
    }

    return result;
}

void GetSpotlightMask_float(float2 uv, out float value)
{
    value = spotlightMask(uv);
}

#endif
