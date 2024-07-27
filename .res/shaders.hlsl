cbuffer ConstantBuffer : register(b0) {
	
	float4x4 cameraProjectionMatrix;

	int screenWidth;
	int screenHeight;
	float aspectRatio;
	
};
cbuffer ObjectInfo : register(b1) {
	
	float4x4 transformMatrix;
	
};

struct Pixel_Input {
	
	float4 position : SV_Position;
	float4 color : COLOR;
	
};

Pixel_Input Vertex_Main(float4 position : POSITION, float4 color : COLOR) {
	
	Pixel_Input result;
	
	result.position = mul(cameraProjectionMatrix, mul(transformMatrix, position));
	result.color = color;
	
	return result;
	
}

float4 Pixel_Main(Pixel_Input input) : SV_TARGET {

	return input.color;
	
}