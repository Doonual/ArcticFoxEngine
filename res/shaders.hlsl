cbuffer ConstantBuffer : register(b0) {
	
	int screenWidth;
	int screenHeight;
	float aspectRatio;
	int anotherField;
	float4x4 cameraInfo;
	
};

struct Pixel_Input {
	
	float4 position : SV_Position;
	float4 color : COLOR;
	
};

Pixel_Input Vertex_Main(float4 position : POSITION, float4 color : COLOR) {
	
	Pixel_Input result;
	
	result.position = position;
	//result.position.x /= aspectRatio;
	
	result.color = color;
	
	return result;
	
}

#define PI2 6.28318


float4 Pixel_Main(Pixel_Input input) : SV_TARGET {

	int xCell = floor(input.position.x / screenWidth * 4);
	int yCell = floor(input.position.y / screenHeight * 4);

	return float4(cameraInfo[xCell][yCell].xxx, 0.0);
	
	
}