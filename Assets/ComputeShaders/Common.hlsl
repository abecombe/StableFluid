#ifndef CS_COMMON_HLSL
#define CS_COMMON_HLSL

inline float3 hsv2rgb(float3 hsv)
{
    return ((clamp(abs(frac(hsv.r + float3(0, 2, 1) / 3.) * 6. - 3.) - 1., 0., 1.) - 1.) * hsv.g + 1.) * hsv.b;
}

inline float map(float value, float inputMin, float inputMax, float outputMin, float outputMax)
{
    float perc = (clamp(value, inputMin, inputMax) - inputMin) / (inputMax - inputMin);
    return perc * (outputMax - outputMin) + outputMin;
}

#endif /* CS_COMMON_HLSL */