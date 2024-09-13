#include "../Common.hlsl"

cbuffer LightingWorld : register(b2) {
	float3 sunDir;
	float ambientLight;
};

cbuffer MaterialInfo : register(b3) {
	float normalStrength;
	float textureScale;
}

Texture2D mainTex : register(t0);
Texture2D normalTex : register(t1);
sampler defaultSampler : register(s0);

float4 Pixel_Main(Vertex input) : SV_Target {
	
	
	float3 binormal = -cross(input.normal.xyz, input.tangent.xyz);
	float2 normalOffset = (normalTex.Sample(defaultSampler, input.uv * textureScale).xy - float2(0.5, 0.5)) * 2.0;
	float3 normalVal = input.normal.xyz + (normalOffset.x * input.tangent.xyz + normalOffset.y * binormal) * normalStrength;
	
	float3 unlitCol = input.color * mainTex.Sample(defaultSampler, input.uv * textureScale);
	
	float lightingVal = max(0, dot(normalVal, -sunDir)) + ambientLight;
	float3 outCol = unlitCol * lightingVal;
	

	return float4(outCol, 0.0);
	
}