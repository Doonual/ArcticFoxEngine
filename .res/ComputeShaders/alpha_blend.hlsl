RWTexture2D<float4> underTexture : register(u0);
RWTexture2D<float4> overTexture : register(u1);
RWTexture2D<float4> resultTexture : register(u2);

[numthreads(8, 8, 1)]
void main(uint3 dispatchThreadID : SV_DispatchThreadID) {
	
	float4 overCol = overTexture[dispatchThreadID.xy];
	float4 underCol = underTexture[dispatchThreadID.xy];
	
	
	float resultAlpha = overCol.a + underCol.a * (1.0 - overCol.a);
	float3 resultCol = overCol.rgb * overCol.a + underCol.rgb * underCol.a * (1.0 - overCol.a);
	resultCol /= resultAlpha;
	
	resultTexture[dispatchThreadID.xy] = float4(resultCol, resultAlpha);
	
}