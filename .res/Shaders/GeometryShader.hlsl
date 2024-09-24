#include "Common.hlsl"


[maxvertexcount(6)]
void Geometry_Main(triangle RawVertex input[3] : SV_Position, inout TriangleStream<Vertex> outputStream) {
	

	float3 tangent;
	
	if (abs(input[2].uv.y - input[0].uv.y) > 0.1) {
		float t = (input[1].uv.y - input[0].uv.y) / (input[2].uv.y - input[0].uv.y);
		tangent = -(t * (input[2].position - input[0].position) + input[0].position - input[1].position);
	}
	else {
		float t = (input[2].uv.y - input[0].uv.y) / (input[1].uv.y - input[0].uv.y);
		tangent = -(t * (input[1].position - input[0].position) + input[0].position - input[2].position);
	}
	

	for (int i = 0; i < 3; i++) {
		
		Vertex outVert;
		outVert.color = input[i].color;
		outVert.position = input[i].position;
		outVert.uv = input[i].uv;
		outVert.normal = input[i].normal;
		outVert.tangent = float4(tangent, 0.0);
		
		outVert.world_position = mul(transformMatrix, input[i].position);
		outVert.position = mul(cameraProjectionMatrix, outVert.world_position);
		outputStream.Append(outVert);
		
		
	}
	
	
	
}