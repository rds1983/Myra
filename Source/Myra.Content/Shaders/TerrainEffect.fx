#include "Macros.fxh"

#ifdef TEXTURE

DECLARE_TEXTURE(_textureLayer0, 0);
DECLARE_TEXTURE(_textureLayer1, 1);
DECLARE_TEXTURE(_textureLayer2, 2);
DECLARE_TEXTURE(_textureLayer3, 3);
DECLARE_TEXTURE(_textureLayer4, 4);
DECLARE_TEXTURE(_textureBlendMap, 5);

#endif

BEGIN_CONSTANTS

#ifdef LIGHTNING

float3 _lightDir;
float3 _lightColor;

#endif

float4 _diffuseColor;
float3 _dirToSun;
float _scale;

MATRIX_CONSTANTS

float4x4 _worldViewProj;
float3x3 _worldInverseTranspose;

END_CONSTANTS

struct VSInput
{
    float4 Position : SV_POSITION;
    float3 Normal   : NORMAL;
    float2 TexCoord : TexCoord0;
};

// Define a vertex shader output structure; that is, a structure
// that defines the data we output from the vertex shader.  Here,
// we only output a 4D vector in homogeneous clip space.  The
// semantic \": POSITION0\" tells Direct3D that the data returned
// in this data member is a vertex position.
struct VSOutput
{
    float4 Position : POSITION0;
	float2 TexCoord: TexCoord0;
	float2 BlendCoord: TexCoord1;
    float4 Color: COLOR0;
};

// Define the vertex shader program.  The parameter posL 
// corresponds to a data member in the vertex structure.
// Specifically, it corresponds to the data member in the 
// vertex structure with usage D3DDECLUSAGE_POSITION and 
// index 0 (as specified by the vertex declaration).
VSOutput VertexShaderFunction(VSInput input)
{
    VSOutput output = (VSOutput)0;

    output.Position = mul(input.Position, _worldViewProj);
	
#ifdef TEXTURE
	// Pass on texture coordinates to be interpolated in rasterization.
	output.TexCoord = input.TexCoord * _scale;
	output.BlendCoord = input.TexCoord;
#endif

#ifdef LIGHTNING
	float s = max(dot(_dirToSun, input.Normal), 0.0f);
	s = saturate(s + 0.1f);
	output.Color = float4(s, s, s, 1.0f) * _diffuseColor;
#endif

    return output;
}

// Define the pixel shader program.  Just return a 4D color
// vector (i.e., first component red, second component green,
// third component blue, fourth component alpha).  Here we
// specify black to color the lines black. 
float4 PixelShaderFunction(VSOutput input) : COLOR
{
#ifdef TEXTURE
	float3 c0 = SAMPLE_TEXTURE(_textureLayer0, input.TexCoord).rgb;
	float3 c1 = SAMPLE_TEXTURE(_textureLayer1, input.TexCoord).rgb;
	float3 c2 = SAMPLE_TEXTURE(_textureLayer2, input.TexCoord).rgb;
	float3 c3 = SAMPLE_TEXTURE(_textureLayer3, input.TexCoord).rgb;
	float3 c4 = SAMPLE_TEXTURE(_textureLayer4, input.TexCoord).rgb;
	float4 b = SAMPLE_TEXTURE(_textureBlendMap, input.BlendCoord).rgba;

	float3 c = c0;
	c = lerp(c, c1, b.r);
	c = lerp(c, c2, b.g);
	c = lerp(c, c3, b.b);
	c = lerp(c, c4, b.a);

    float4 color = float4(c, 1.0f);
#else
	float4 color = float4(1, 1, 1, 1);
#endif

#ifdef LIGHTNING
	return color * input.Color;
#else
	return color * _diffuseColor;
#endif
}

TECHNIQUE(Default, VertexShaderFunction, PixelShaderFunction);