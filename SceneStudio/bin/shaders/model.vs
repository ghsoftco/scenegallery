
uniform float4x4 World;          // World transformation
uniform float4x4 WorldViewProj;  // World * View * Projection transformation
//uniform float ZShift;

struct VS_OUTPUT
{
    float4 Position         : POSITION;   // vertex position
    float4 TexCoord         : TEXCOORD0;  // vertex interpolation value
    float4 Normal           : TEXCOORD1;  // vertex normal
};

VS_OUTPUT VShaderEntry( in float3 Position : POSITION,
                        in float3 Normal   : NORMAL,
                        in float4 TexCoord : TEXCOORD0 )
{
    VS_OUTPUT Output;
    
    Output.Position = mul( float4( Position.x, Position.y, Position.z, 1.0f ), WorldViewProj );
    //Output.Position.z += ZShift;
    Output.TexCoord = TexCoord;

    Output.Normal = mul( float4( Normal.x, Normal.y, Normal.z, 0.0f ), World );
    
    return Output;
}
