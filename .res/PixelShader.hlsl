#include "Common.hlsl"

float4 Pixel_Main(Vertex input) : SV_TARGET {

	float4 outCol = input.color * g_texture.Sample(g_sampler, input.uv);
	return outCol;
	
}