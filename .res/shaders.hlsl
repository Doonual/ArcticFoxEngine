cbuffer RenderInfo : register(b0) {
	
	float4x4 cameraProjectionMatrix;

	int screenWidth;
	int screenHeight;
	float aspectRatio;
	
};
cbuffer ObjectInfo : register(b1) {
	float4x4 transformMatrix;
};

struct Pixel_Input {
	
	// The SV_Position and COLOR tell the shader that these
	// are the values to be passed into the rasteriser stage
	float4 position : SV_Position;
	float4 color : COLOR0;
	
};

Pixel_Input Vertex_Main(float3 position : POSITION, float4 color : COLOR0) {
	
	Pixel_Input result;

	result.position = mul(cameraProjectionMatrix, mul(transformMatrix, float4(position, 1.0)));
	result.color = color;
	
	return result;
	
}

float4 Pixel_Main(Pixel_Input input) : SV_TARGET {

	return input.color;
	
}