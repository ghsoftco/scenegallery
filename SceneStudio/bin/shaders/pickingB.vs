uniform float4x4 WorldViewProj;

struct VS_OUTPUT
{
    float4 Position         : POSITION;
    float2 UV                : TEXCOORD0;
    float4 TriID            : TEXCOORD1;
};

VS_OUTPUT VShaderEntry( in float3 Position : POSITION,
                        in float2 UV : TEXCOORD0,
                        in float4 TriID : COLOR0 )
{
    VS_OUTPUT Output;
    
    Output.Position = mul( float4( Position.x, Position.y, Position.z, 1.0f ), WorldViewProj );
    Output.UV = UV;
    Output.TriID = TriID;
    
    return Output;
}
