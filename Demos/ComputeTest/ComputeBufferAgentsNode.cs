using ArcticFoxEngine.Compute;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Demos.ComputeTest {
	public class ComputeBufferAgentsNode : Node {

		struct Agent {
			public Vector2 position;
			public Vector2 velocity;
			public Vector3 color;
		}

		ComputeShader agentsCompute;

		int numAgents = 1000000;
		StructuredBuffer<Agent> agentBuffer;
		Texture agentsMap;

		public ComputeBufferAgentsNode() {

			agentsCompute = new ComputeShader("Demos/ComputeTest/agents_compute.hlsl", "UpdateAgents", "UpdateMap");

			agentBuffer = new StructuredBuffer<Agent>(numAgents, SharpDX.Direct3D12.ResourceFlags.AllowUnorderedAccess);
			Agent[] agentData = new Agent[numAgents];
			for (int i = 0; i < numAgents; i ++) {
				Agent newAgent = new Agent();
				newAgent.position = new Vector2(0.5f * MainWindow.aspectRatio, 0.5f);
				newAgent.velocity = Vector2.Angle(MathUtil.Lerp(i / (float)numAgents, 0, MathF.PI * 2f), 0.0005f);

				Color col = Color.FromHSV((int)MathUtil.Lerp(i / (float)numAgents, 0, 360), 255, 255);
				newAgent.color = new Vector3(col.r, col.g, col.b);

				agentData[i] = newAgent;
			}
			agentBuffer.Write(agentData, 0);

			agentsMap = new Texture(MainWindow.width, MainWindow.height, format: Format.R32G32B32A32_Float, flags: SharpDX.Direct3D12.ResourceFlags.AllowUnorderedAccess);


		}

		public override void Update() {

			agentsCompute.SetBuffer("agentBuffer", agentBuffer);
			agentsCompute.SetTexture("agentMap", agentsMap);
			agentsCompute.SetTexture("renderTexture", Graphics.mainTexture);

			for (int i = 0; i < 2; i ++) {
				agentsCompute.Dispatch("UpdateAgents", (int)MathF.Ceiling(numAgents / 8f), 1, 1);
				agentsCompute.Dispatch("UpdateMap", (int)MathF.Ceiling(MainWindow.width / 8f), (int)MathF.Ceiling(MainWindow.height / 8f), 1);
			}
			

			Graphics.WaitForComputeCommandQueue();

		}

		public override void Render() {
			//Graphics.BlitTexture(agentsMap, Graphics.mainTexture);
		}

	}
}
