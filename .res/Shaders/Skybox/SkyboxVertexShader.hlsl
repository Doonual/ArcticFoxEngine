
struct RawVertex {
	
	// The SV_Position and COLOR tell the shader that these
	// are the values to be passed into the rasteriser stage
	float4 position : SV_Position;
	float4 color : COLOR;
	float2 uv : TEXCOORD;
	float4 normal : NORMAL;
	
};

RawVertex Vertex_Main(RawVertex vertIn) {
	vertIn.position.z = 1.0;
	return vertIn;
}