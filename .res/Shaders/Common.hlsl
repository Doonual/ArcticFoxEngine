struct Vertex {
	
	// The SV_Position and COLOR tell the shader that these
	// are the values to be passed into the rasteriser stage
	float4 position : SV_Position;
	float4 color : COLOR;
	float2 uv : TEXCOORD;
	
};


cbuffer RenderInfo : register(b0) {
	float4x4 cameraProjectionMatrix;
	int screenWidth;
	int screenHeight;
	float aspectRatio;
};

cbuffer ObjectInfo : register(b1) {
	float4x4 transformMatrix;
};

Texture2D g_texture : register(t0);
SamplerState g_sampler : register(s0);