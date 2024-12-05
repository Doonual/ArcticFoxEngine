#include "../Common.hlsl"

Texture2D mainTex : register(t0);
sampler mainSampler : register(s0);

float4 Pixel_Main(Vertex input) : SV_TARGET {

	float4 outCol = input.color * mainTex.Sample(mainSampler, input.uv);
	return outCol;
	
}