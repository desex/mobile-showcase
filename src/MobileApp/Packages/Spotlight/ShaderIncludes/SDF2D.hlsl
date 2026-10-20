// https://iquilezles.org/articles/distfunctions2d/

#ifndef SDF_2D
#define SDF_2D

// COMBINATIONS
inline float SdfMerge(in float firstShape, in float secondShape)
{
    return min(firstShape, secondShape);
}

inline float SdfIntersect(in float firstShape, in float secondShape)
{
    return max(firstShape, secondShape);
}

inline float SdfSubtract(in float firstShape, in float secondShape)
{
    return SdfIntersect(firstShape, -secondShape);
}

inline float SdfRoundMerge(float firstShape, float secondShape, float radius)
{
    float2 intersectionSpace = float2(firstShape - radius, secondShape - radius);
    intersectionSpace = min(intersectionSpace, 0);
    float insideDistance = -length(intersectionSpace);
    float simpleUnion = SdfMerge(firstShape, secondShape);
    float outsideDistance = max(simpleUnion, radius);
    return insideDistance + outsideDistance;
}

// SHAPES
float SdfRectangle(in float2 position, in float2 halfSize)
{
    float2 componentWiseEdgeDistance = abs(position) - halfSize;
    float outsideDistance = length(max(componentWiseEdgeDistance, 0));
    float insideDistance = min(max(componentWiseEdgeDistance.x, componentWiseEdgeDistance.y), 0);
    return outsideDistance + insideDistance;
}

#endif