#include "../Common.hlsl"
#define PI2 6.28318

cbuffer ViewportInfo : register(b2) {
	
	float2 viewCenter;
	float zoom;
	int iterations;
	bool doublePrecision;

	
};




float4 Pixel_Main(Vertex input) : SV_TARGET {
	
	
	
	
	if (doublePrecision == false) {
		

		float2 originalPos = input.uv - float2(0.5, 0.5);
		
		originalPos *= zoom;
		originalPos -= viewCenter;
		
		float2 pos = float2(0.0, 0.0);
		float depth = 0.0;
		bool escaped = false;
	
		for (int i = 0; i < iterations; i++) {
		
			float2 tempPos = float2(pos.x * pos.x - pos.y * pos.y, 2 * pos.x * pos.y);
			tempPos += originalPos;
			pos = tempPos;
		
			if (pos.x * pos.x + pos.y * pos.y > 4.0) {
				i = iterations;
				escaped = true;
			}
		
			depth += 0.1;
		
		}

		float3 a = float3(0.561, 0.572, 0.578);
		float3 b = float3(-0.211, 0.136, 0.078);
		float3 c = float3(-0.627, -0.734, -0.650);
		float3 d = float3(0.550, 0.504, 0.672);
	
		float3 col = a + b * cos(PI2 * (c * (depth) + d));
		if (escaped == false) {
			col = float3(0.0, 0.0, 0.0);
		}
		
		return float4(col, 0.0);
		
	}
	else {
		


		double originalPosx = (double) (input.uv.x - 0.5);
		originalPosx *= (double) zoom;
		originalPosx -= (double) viewCenter.x;
		
		double originalPosy = (double) (input.uv.y - 0.5);
		originalPosy *= (double) zoom;
		originalPosy -= (double) viewCenter.y;
		
		double posx = 0.0;
		double posy = 0.0;
		float depth = 0.0;
		bool escaped = false;
	
		for (int i = 0; i < iterations; i++) {
			
			double tempPosx = (posx * posx) - (posy * posy);
			double tempPosy = 2 * posx * posy;
			

			tempPosx += originalPosx;
			tempPosy += originalPosy;
			posx = tempPosx;
			posy = tempPosy;
		
			if (posx * posx + posy * posy > (double) 4.0) {
				i = iterations;
				escaped = true;
			}
		
			depth += 0.1;
		
		}
		

		float3 a = float3(0.561, 0.572, 0.578);
		float3 b = float3(-0.211, 0.136, 0.078);
		float3 c = float3(-0.627, -0.734, -0.650);
		float3 d = float3(0.550, 0.504, 0.672);

		float3 col = a + b * cos(PI2 * (c * (depth) + d));
		if (escaped == false) {
			col = float3(0.0, 0.0, 0.0);
		}
		
		return float4(col, 0.0);

		
	}
	
	
	
	
}