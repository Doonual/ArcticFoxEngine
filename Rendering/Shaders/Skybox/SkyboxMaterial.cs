using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Rendering {

	public class SkyboxMaterial : Material {

		SkyboxShader.SkyboxInfo skyboxInfo;
		private ConstBuffer<SkyboxShader.SkyboxInfo> skyboxInfoBuffer;


		public SkyboxMaterial() {

			skyboxInfo = new SkyboxShader.SkyboxInfo() {
				skyTopCol = new Vector3(111f / 255f, 180f / 255f, 235f / 255f),
				skyBottomCol = new Vector3(246f / 255f, 243f / 255f, 232f / 255f),
				groundTopCol = new Vector3(116f / 255f, 98f / 255f, 81f / 255f),
				groundBottomCol = new Vector3(55f / 255f, 40f / 255f, 32f / 255f),
				sunStrength = -0.00002f,
				horizonSharpness = -0.0034f,
			};
			skyboxInfoBuffer = new ConstBuffer<SkyboxShader.SkyboxInfo>(1);
			skyboxInfoBuffer.Write(skyboxInfo, 0);

		}

		public override void BindResources(Shader shader) {
			SkyboxShader skyShader = (SkyboxShader)shader;
			skyShader.skyBoxInfo.SetData(skyboxInfoBuffer, 0);
		}

		public override void Debug() {

			bool changed = false;

			changed |= ImGui.ColorEdit3("Sky top col", ref skyboxInfo.skyTopCol);
			changed |= ImGui.ColorEdit3("Sky bottom col", ref skyboxInfo.skyBottomCol);

			changed |= ImGui.ColorEdit3("Ground top col", ref skyboxInfo.groundTopCol);
			changed |= ImGui.ColorEdit3("Ground bottom col", ref skyboxInfo.groundBottomCol);

			changed |= ImGui.SliderFloat("Sun strength", ref skyboxInfo.sunStrength, -0.001f, 0f, "%.6f");
			changed |= ImGui.SliderFloat("Horizon sharpness", ref skyboxInfo.horizonSharpness, -0.1f, 0f, "%.4f");

		
			if (changed == true) {
				skyboxInfoBuffer.Write(skyboxInfo, 0);
			}

		}
	}

}
