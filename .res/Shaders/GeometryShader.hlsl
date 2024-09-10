#include "Common.hlsl"


[maxvertexcount(6)]
void Geometry_Main(triangle Vertex input[3] : SV_Position, inout TriangleStream<Vertex> outputStream) {
	


	for (int i = 0; i < 3; i++) {
		
		Vertex outVert;
		outVert.color = input[i].color;
		outVert.position = input[i].position;
		outVert.uv = input[i].uv;
		outVert.normal = input[i].normal;
		
		outVert.position = mul(cameraProjectionMatrix, mul(transformMatrix, input[i].position));
		outputStream.Append(outVert);
		
		
	}
	
	
	
}