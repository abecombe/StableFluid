#ifndef CS_GRID_PARAMS_SAMPLING_HLSL
#define CS_GRID_PARAMS_SAMPLING_HLSL

float2 SampleVelocity(RWTexture2D<float2> velocityTex, int2 coord, int2 resolution)
{
    float2 multiplier = float2(1.0, 1.0);

    if (coord.x < 0 || coord.x >= resolution.x)
    {
        multiplier.x = -1.0;
    }
    if (coord.y < 0 || coord.y >= resolution.y)
    {
        multiplier.y = -1.0;
    }

    return multiplier * velocityTex[clamp(coord, 0, resolution - 1)];
}

float2 SampleVelocity(sampler2D velocityTex, float2 uv)
{
    return tex2Dlod(velocityTex, float4(uv, 0.0, 0.0)).xy;
}

float SamplePressure(const RWTexture2D<float> pressureTex, int2 coord, int2 resolution)
{
    return pressureTex[clamp(coord, 0, resolution - 1)];
}


#endif /* CS_GRID_PARAMS_SAMPLING_HLSL */