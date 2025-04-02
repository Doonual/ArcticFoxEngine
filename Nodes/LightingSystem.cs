
using ArcticFoxEngine.Render;
using CoolClassLibrary;
using ImGuiNET;

namespace ArcticFoxEngine.Nodes {

	public class LightingSystem : Node {

		internal override string nodeIconPath => ".res/NodeIcons/LightManager.png";
		internal override string nodeIconPath32 => ".res/NodeIcons/LightManager32.png";

		LitShader.LightingWorld lightingWorld;
		List<Light> lights;

		public LightingSystem() {
			name = "Lighting System";

			lightingWorld = new LitShader.LightingWorld();
			lights = new List<Light>();

			Enable();
		}

		public void AddLight(Light light) {
			if (lights.Count >= 16) {
				Log.Warn("Failed to add light to lighting system, max capacity reached");
				return;
			}
			if (lights.Contains(light) == true) {
				Log.Warn("Failed to add light to lighting system, light already added");
				return;
			}
			
			lights.Add(light);
		}
		public void RemoveLight(Light light) {
			if (lights.Contains(light) == false) {
				Log.Warn("Failed to remove light from lighting system, light not added");
				return;
			}
			lights.Remove(light);
		}

		public override void Update() {

			LitShader.SetLightingInfo(lightingWorld);

			for (int i = 0; i < lights.Count(); i++) {
				LitShader.SetLightData(lights[i].GetLightData(), i);
			}

		}

		public override void DrawInspector() {


			ImGui.DragFloat3("Sun direction", ref lightingWorld.sunDir, 0.01f);
			if (lightingWorld.sunDir.x == float.NaN || lightingWorld.sunDir.y == float.NaN || lightingWorld.sunDir.z == float.NaN) {
				lightingWorld.sunDir = new Vector3(1f, 0f, 0f);
			}
			lightingWorld.sunDir = lightingWorld.sunDir.Normalize();

			ImGui.SliderFloat("Sun strength", ref lightingWorld.sunStrength, 0f, 1f);
			ImGui.SliderFloat("Ambient light", ref lightingWorld.ambientLight, 0f, 1f);

		}
		

	}
}
