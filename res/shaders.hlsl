cbuffer ConstantBuffer : register(b0) {
	
	int screenWidth;
	int screenHeight;
	float4x4 cameraInfo;
	
};

struct Pixel_Input {
	
	float4 position : SV_Position;
	float4 color : COLOR;
	
};

Pixel_Input Vertex_Main(float4 position : POSITION, float4 color : COLOR) {
	
	Pixel_Input result;
	result.position = position;
	result.position.x *= screenHeight;
	result.position.x /= screenWidth;
	
	result.color = color;
	return result;
	
}

#define PI2 6.28318


float4 Pixel_Main(Pixel_Input input) : SV_TARGET {
	
	return input.color;
	
}