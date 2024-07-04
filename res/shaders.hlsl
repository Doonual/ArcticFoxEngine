cbuffer ConstantBuffer : register(b0) {
	
	float4x4 projectionMatrix;
	float4x4 worldToCameraMatrix;

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
	
	float4 vertexPos = mul(worldToCameraMatrix, position * float4(1.0, 1.0, -1.0, 1.0));
	result.position = mul(projectionMatrix, vertexPos);
	//result.position.z += 1.0;
	//result.position.z /= 2.0;
	//result.position.w = 1.0;
	//result.position.w = result.position.z;
	result.color = color;
	
	return result;
	
}

#define PI2 6.28318
float4 Pixel_Main(Pixel_Input input) : SV_TARGET {

	float2 uv = input.position.xy / float2(screenWidth, screenHeight);
	int xCell = (int) floor(uv.x * 4);
	int yCell = (int) floor(uv.y * 4);
	
	float amnt = abs(projectionMatrix[xCell][yCell]);
	
	return input.color;
	if (projectionMatrix[xCell][yCell] > 0) {
		return float4(0.0, amnt, 0.0, 0.0);
	}
	else {
		return float4(amnt, 0.0, 0.0, 0.0);
	}
	
	
	
}