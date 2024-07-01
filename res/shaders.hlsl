#if !defined(__INTELLISENSE__) // Disable error squigly

#include "res/RootSignature.hlsl"

#endif

struct Pixel_Input {
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

[RootSignature(ROOTSIG)]
Pixel_Input Vertex_Main(float4 position : POSITION, float4 color : COLOR) {
	
	Pixel_Input result;
	result.position = position;
	result.color = color;
	return result;
	
}

#define PI2 6.28318

[RootSignature(ROOTSIG)]
float4 Pixel_Main(Pixel_Input input) : SV_TARGET {
	
	float2 originalPos = input.position / 720.0;
	originalPos -= float2(0.5 * 1280.0 / 720.0, 0.5);
	
	originalPos.x -= 0.25;
	originalPos *= 2.5;
	
	float2 pos = float2(0.0, 0.0);
	float depth = 0.0;
	
	for (int i = 0; i < 1000; i++) {
		
		float2 tempPos = float2(pos.x * pos.x - pos.y * pos.y, 2 * pos.x * pos.y);
		tempPos += originalPos;
		pos = tempPos;
		
		if (pos.x * pos.x + pos.y * pos.y > 4.0) {
			i = 1000;
		}
		
		depth += 0.1;
		
	}
	
	float3 a = float3(0.561, 0.572, 0.578);
	float3 b = float3(-0.211, 0.136, 0.078);
	float3 c = float3(-0.627, -0.734, -0.650);
	float3 d = float3(0.550, 0.504, 0.672);
	
	float3 col = a + b * cos(PI2 * (c * depth + d));
	
	return float4(col, 0.0);
	
}