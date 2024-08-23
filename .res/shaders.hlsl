/*
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
	float4 color : COLOR;
	
};

Pixel_Input Vertex_Main(float4 position : POSITION, float4 color : COLOR) {
	
	Pixel_Input result;
	
	
	result.position = mul(cameraProjectionMatrix, mul(transformMatrix, position));
	result.color = color;
	
	return result;
	
}

float4 Pixel_Main(Pixel_Input input) : SV_TARGET {

	float4 outCol = input.color;
	return outCol;
	
}
*/

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



//sampler sampler0;
//Texture2D texture0;

float4 Pixel_Main(PS_INPUT input) : SV_Target {
	return input.col;
	//return input.col * texture0.Sample(sampler0, input.uv);
}