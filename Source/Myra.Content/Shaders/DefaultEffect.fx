//-----------------------------------------------------------------------------
// BasicEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "Macros.fxh"

#ifdef TEXTURE

DECLARE_TEXTURE(_texture, 0);

#endif

BEGIN_CONSTANTS

#ifdef LIGHTNING

float3 _lightDir;
float3 _lightColor;
float _specularPower = 16;

#endif

float3 _eyePosition;
float4 _diffuseColor;

MATRIX_CONSTANTS

float4x4 _world;
float4x4 _worldViewProj;
float3x3 _worldInverseTranspose;

END_CONSTANTS

struct VSInput
{
    float4 Position : SV_POSITION;
    float3 Normal   : NORMAL;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position : SV_POSITION;
#ifdef TEXTURE	
    float2 TexCoord : TEXCOORD0;
#endif

#ifdef LIGHTNING
	float3 WorldPosition: TEXCOORD1;
	float3 WorldNormal: TEXCOORD2;
#endif
};

VSOutput VertexShaderFunction(VSInput input)
{
    VSOutput output = (VSOutput)0;

    output.Position = mul(input.Position, _worldViewProj);
	
#ifdef TEXTURE
	output.TexCoord = input.TexCoord;
#endif

#ifdef LIGHTNING
	output.WorldPosition = mul(input.Position, _world).xyz;
	output.WorldNormal = normalize(mul(input.Normal, _worldInverseTranspose));
#endif

    return output;
}

float4 PixelShaderFunction(VSOutput input) : SV_Target0
{
#ifdef TEXTURE
    float4 color = SAMPLE_TEXTURE(_texture, input.TexCoord);
#else
	float4 color = float4(1, 1, 1, 1);
#endif

#ifdef LIGHTNING
	float3 result = float3(0, 0, 0);

	float3 normal = input.WorldNormal;
	// Diffuse part
	float diffuseFactor = saturate(dot(normal, -_lightDir));
	
	if (diffuseFactor > 0.0f)
	{
		float3 eyeVector = normalize(_eyePosition - input.WorldPosition);

		// Using Blinn-Phong shading model
		float3 halfWay = normalize(eyeVector - _lightDir);
		float halfDotView = max(0.0, dot(halfWay, normal));
		float specFactor = pow(halfDotView, _specularPower);
		
		result += diffuseFactor * _lightColor * _diffuseColor.rgb;
		result += specFactor * _lightColor * _diffuseColor.rgb;
	}
	
	return color * float4(result, 1);
#else
	return color * _diffuseColor;
#endif
}

TECHNIQUE(Default, VertexShaderFunction, PixelShaderFunction);