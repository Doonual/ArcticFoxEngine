#include "Common.hlsl"
#define PI2 6.28318

cbuffer ViewportInfo : register(b2) {
	
	float2 viewCenter;
	float zoom;
	
};




float4 Pixel_Main(Vertex input) : SV_TARGET {
	
	float2 originalPos = input.uv - float2(0.5f, 0.5f);
	
	originalPos *= zoom;
	originalPos -= viewCenter;
	
	
	float2 pos = float2(0.0, 0.0);
	float depth = 0.0;
	bool escaped = false;
	
	int maxIterations = 100;
	for (int i = 0; i < maxIterations; i++) {
		
		float2 tempPos = float2(pos.x * pos.x - pos.y * pos.y, 2 * pos.x * pos.y);
		tempPos += originalPos;
		pos = tempPos;
		
		if (pos.x * pos.x + pos.y * pos.y > 4.0) {
			i = 1000;
			escaped = true;
		}
		
		depth += 0.1;
		
	}
	
	float3 a = float3(0.561, 0.572, 0.578);
	float3 b = float3(-0.211, 0.136, 0.078);
	float3 c = float3(-0.627, -0.734, -0.650);
	float3 d = float3(0.550, 0.504, 0.672);
	
	float seededOffset = (a % maxIterations) / maxIterations;
	
	float3 col = a + b * cos(PI2 * (c * (depth + seededOffset) + d));
	if (escaped == false) {
		col = float3(0.0, 0.0, 0.0);
	}
	
	return float4(col, 0.0);
	
}