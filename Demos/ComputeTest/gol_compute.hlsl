RWTexture2D<float4> mainTex : register(u0);

float3 SampleLooped(int2 uv, int width, int height) {
	if (uv.x < 0) {
		uv.x += width;
	}
	if (uv.y < 0) {
		uv.y += height;
	}
	
	uv.x %= width;
	uv.y %= height;
	
	return mainTex[uv].rgb;

}

[numthreads(8, 8, 1)]
void main(uint3 dispatchThreadID : SV_DispatchThreadID) {
	
	int2 id = dispatchThreadID.xy;
	
	int width;
	int height;
	mainTex.GetDimensions(width, height);
	
	int numAliveNeighbours = 0;
	for (int i = -1; i <= 1; i ++) {
		for (int n = -1; n <= 1; n++) {
			if (i == 0 && n == 0) { continue; }
			
			if (SampleLooped(id + int2(i, n), width, height).r > 0.5) {
				numAliveNeighbours += 1;
			}
			
		}
	}
	
	if (mainTex[id].r > 0.5) {
		if (numAliveNeighbours != 2 && numAliveNeighbours!= 3) {
			mainTex[id] = float4(0.0, 0.0, 0.0, 1.0);
		}
	}
	else {
		if (numAliveNeighbours == 3) {
			mainTex[id] = float4(1.0, 1.0, 1.0, 1.0);
		}
	}

}

