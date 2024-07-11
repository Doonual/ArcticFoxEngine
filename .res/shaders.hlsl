cbuffer ConstantBuffer : register(b0) {
	
	float4x4 projectionMatrix;

	int screenWidth;
	int screenHeight;
	float aspectRatio;
	
};

struct Pixel_Input {
	
	float4 position : SV_Position;
	float4 color : COLOR;
	
};

Pixel_Input Vertex_Main(float4 position : POSITION, float4 color : COLOR) {
	
	Pixel_Input result;
	
	result.position = mul(projectionMatrix, position);
	result.color = color;
	
	return result;
	
}

float4 Pixel_Main(Pixel_Input input) : SV_TARGET {

	return input.color;
	
}