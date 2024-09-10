#include "../Common.hlsl"

Texture2D mainTex : register(t0);
sampler mainSampelr : register(s0);

float4 Pixel_Main(Vertex input) : SV_TARGET {

	float4 outCol = input.color * mainTex.Sample(mainSampelr, input.uv);
	return outCol;
	
}