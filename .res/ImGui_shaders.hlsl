

cbuffer vertexBuffer : register(b0) {
	float4x4 ProjectionMatrix;
};

struct VS_INPUT {
	float2 pos : POSITION;
	float4 col : COLOR0;
	float2 uv : TEXCOORD0;
};

struct PS_INPUT {
	float4 pos : SV_POSITION;
	float4 col : COLOR0;
	float2 uv : TEXCOORD0;
};

PS_INPUT Vertex_Main(VS_INPUT input) {
	PS_INPUT output;
	output.pos = mul(ProjectionMatrix, float4(input.pos.xy, 0.f, 1.f));
	output.col = input.col;
	output.uv = input.uv;
	return output;
}



Texture2D g_texture : register(t0);
SamplerState g_sampler : register(s0);

float4 Pixel_Main(PS_INPUT input) : SV_Target {
	return input.col * g_texture.Sample(g_sampler, input.uv);
}