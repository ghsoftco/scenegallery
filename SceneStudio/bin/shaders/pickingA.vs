uniform float4x4 WorldViewProj;

struct VS_OUTPUT
{
    float4 Position         : POSITION;
};

VS_OUTPUT VShaderEntry( in float3 Position : POSITION )
{
    VS_OUTPUT Output;
    
    Output.Position = mul( float4( Position.x, Position.y, Position.z, 1.0f ), WorldViewProj );
    
    return Output;
}
