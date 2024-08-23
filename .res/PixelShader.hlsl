Texture2D g_texture : register(t0);
SamplerState g_sampler : register(s0);


struct Pixel_Input {
	
	// The SV_Position and COLOR tell the shader that these
	// are the values to be passed into the rasteriser stage
	float4 position : SV_Position;
	float4 color : COLOR;
	float2 uv : TEXCOORD;
	
};


float4 Pixel_Main(Pixel_Input input) : SV_TARGET {

	float4 outCol = g_texture.Sample(g_sampler, input.uv);
	return outCol;
	
}