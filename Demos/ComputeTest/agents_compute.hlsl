struct Agent {
	
	float2 position;
	float2 velocity;
	float3 color;
	
};

RWStructuredBuffer<Agent> agentBuffer : register(u0);
RWTexture2D<float4> agentMap : register(u1);
RWTexture2D<float4> renderTexture : register(u2);

[numthreads(8, 1, 1)]
void UpdateAgents(uint3 id : SV_DispatchThreadID) {
	
	uint numAgents;
	uint stride;
	agentBuffer.GetDimensions(numAgents, stride);
	
	if (id.x >= numAgents) {
		return;
	}
	
	float aspectRatio = 16.0 / 9.0;
	
	Agent agent = agentBuffer[id.x];
	
	if (agent.position.x < 0.0) {
		agent.velocity.x = abs(agent.velocity.x);
		agent.position.x = -agent.position.x;
	}
	if (agent.position.x > 1.0 * aspectRatio) {
		agent.velocity.x = -abs(agent.velocity.x);
		agent.position.x = 2.0 * aspectRatio - agent.position.x;
	}
	if (agent.position.y < 0.0) {
		agent.velocity.y = abs(agent.velocity.y);
		agent.position.y = -agent.position.y;
	}
	if (agent.position.y > 1.0) {
		agent.velocity.y = -abs(agent.velocity.y);
		agent.position.y = 2.0 - agent.position.y;

	}
	
	agent.position += agent.velocity;
	agentBuffer[id.x] = agent;
	
	int2 mapUV = agent.position * 1080;
	agentMap[mapUV] += float4(agent.color + 0.2.xxx, 1.0) * 0.5;
	
}

[numthreads(8, 8, 1)]
void UpdateMap(uint3 id : SV_DispatchThreadID) {
	
	
	float4 average = 0.0;
	for (int i = -1; i <= 1; i++) {
		for (int n = -1; n <= 1; n++) {
			average += agentMap[id.xy + int2(i, n)];
		}
	}
	average /= 9.0;
	
	
	agentMap[id.xy] = max(average * 0.99, float4(0.0, 0.0, 0.0, 1.0));
	
	renderTexture[id.xy] = clamp(agentMap[id.xy], float4(0.0, 0.0, 0.0, 0.0), float4(1.0, 1.0, 1.0, 1.0));
	
}