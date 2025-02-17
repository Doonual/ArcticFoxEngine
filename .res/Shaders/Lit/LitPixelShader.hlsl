#include "../Common.hlsl"

cbuffer LightingWorld : register(b2) {
	float3 sunDir;
	float sunStrength;
	float ambientLight;
};

cbuffer MaterialInfo : register(b3) {
	float normalStrength;
	float textureScale;
}

struct LightData {
	float4 pos;
	float3 col;
	float strength;
	
};

StructuredBuffer<LightData> lightData : register(t0);

Texture2D mainTex : register(t1);
Texture2D normalTex : register(t2);



sampler defaultSampler : register(s0);

float4 Pixel_Main(Vertex input) : SV_Target {
	

	float3 binormal = -cross(input.normal.xyz, input.tangent.xyz);
	float2 normalOffset = (normalTex.Sample(defaultSampler, input.uv * textureScale).xy - float2(0.5, 0.5)) * 2.0;
	float3 normalVal = input.normal.xyz + (normalOffset.x * input.tangent.xyz + normalOffset.y * binormal) * normalStrength;
	
	float3 unlitCol = input.color * mainTex.Sample(defaultSampler, input.uv * textureScale);
	

	
	float lightVal = 0.0;
	float3 outCol = float3(0.0, 0.0, 0.0);
	
	// Ambient light
	lightVal = ambientLight;
	
	outCol += unlitCol * lightVal;
	
	// Sun light
	lightVal = max(0, dot(normalVal, -sunDir)) * sunStrength;
	outCol += unlitCol * lightVal;
	
	for (int i = 0; i < 16; i++) {
		
		float lightStrength = (length(lightData[i].pos - input.world_position) / lightData[i].strength) + 1.0;
		lightStrength = lightStrength * lightStrength;
		lightStrength = 1.0 / lightStrength;
		
		lightVal = max(0, dot(normalVal, normalize(lightData[i].pos.xyz - input.world_position.xyz)));
		lightVal *= lightStrength * lightStrength;
		outCol += lightVal * lightData[i].col * 255 * unlitCol;
		
	}
	
	

	return float4(outCol, 1.0);
	
}