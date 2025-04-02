struct RawVertex {
	
	// The SV_Position and COLOR tell the shader that these
	// are the values to be passed into the rasteriser stage
	float4 position : SV_Position;
	float4 color : COLOR;
	float2 uv : TEXCOORD;
	float4 normal : NORMAL;
	
};

cbuffer projMatrixCBuffer : register(b0) {
	float4x4 projMatrix;
}
cbuffer cameraTransformMatrixCBuffer : register(b1) {
	float4x4 camTfMatrix;
}
cbuffer LightingWorld : register(b2) {
	float3 sunDir;
	float sunStrength;
	float ambientLight;
};
cbuffer SkyboxInfo : register(b3) {
	
	float3 skyTopCol;
	float3 skyBottomCol;
	float3 groundTopCol;
	float3 groundBottomCol;
	
	float sunSharpness;
	float horizonSharpness;
	
};

float3 GetSkyCol(float3 lookVector) {
	
	float normalisedSunDot = (dot(lookVector, sunDir) + 1) / 2;
	
	float3 skyCol = lerp(skyBottomCol.rgb, skyTopCol.rgb, normalisedSunDot);
	
	if (lookVector.y < 0) {
		skyCol = skyCol * (1.5 * lookVector.y + 1);
		skyCol = 0.0;
	}
	
	return skyCol;
	
	
	
}
float3 GetSunCol(float3 lookVector) {
	
	float sunDot = 1 - max(0, dot(lookVector, -sunDir));
	
	float a = sunSharpness;
	float c = a * a - a;
	float b = c / -a - 1;
	
	float sunCol = c / (sunDot - a) - b;
	
	return sunCol.xxx;
	
}
float3 GetHorizonCol(float3 lookVector) {
	
	float sharpnessVal = horizonSharpness;
	
	float skyHeight = abs(lookVector.y);
	
	float a = sharpnessVal;
	float c = a * a - a;
	float b = c / -a - 1;
	
	float horizonVal = c / (skyHeight - a) - b;
	
	return horizonVal.xxx;
	
}
float3 GetGroundCol(float3 lookVector) {
	
	if (lookVector.y > 0) {
		return 0.0.xxx;
	}
	
	return lerp(groundBottomCol, groundTopCol, (lookVector.y + 1));
	
}

float4 Pixel_Main(RawVertex input) : SV_Target {
	
	float2 uv = ((input.uv) - float2(0.5, 0.5)) * 2;
	float4 localCamPos = float4(uv, 1.0, 1.0);
	
	float4x4 cameraRotMatrix = camTfMatrix;
	cameraRotMatrix._14_24_34 = 0.0.xxx;
	
	
	float4 cameraPoint = mul(projMatrix, localCamPos);
	float4 worldPoint = mul(cameraRotMatrix, cameraPoint);
	float3 lookVector = normalize(worldPoint);
	

	float3 skyCol = GetSkyCol(lookVector);
	float3 sunCol = GetSunCol(lookVector);
	float3 horizonCol = GetHorizonCol(lookVector);
	float3 groundCol = GetGroundCol(lookVector);
	
	return float4(skyCol + sunCol + horizonCol + groundCol, 1.0);
	
}
