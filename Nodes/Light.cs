
using ArcticFoxEngine.Rendering;
using CoolClassLibrary;
using ImGuiNET;

namespace ArcticFoxEngine.Nodes {

	public class Light : Node {

		internal override string nodeIconPath => ".res/NodeIcons/Light.png";
		internal override string nodeIconPath32 => ".res/NodeIcons/Light32.png";

		LightingSystem mainLightingSystem;
		public Color colour;
		public float strength;

		public Light() {
			name = "Light";

			colour = new Color(255, 255, 255);
			strength = 1f;
			
			mainLightingSystem = SearchNodeTreeUp<LightingSystem>(true);

			Enable();
		}

		internal LitRenderPipeline.LightData GetLightData() {


			LitRenderPipeline.LightData lightData = new LitRenderPipeline.LightData();
			
			lightData.pos = (Vector4)transform.worldPosition;
			lightData.col = new Vector3(colour.r / 255f, colour.g / 255f, colour.b / 255f);
			lightData.strength = strength;
			return lightData;

		}

		public override void OnEnable() {
			mainLightingSystem.AddLight(this);
		}
		public override void OnDisable() {
			mainLightingSystem.RemoveLight(this);
		}

		public override void Debug() {

			System.Numerics.Vector3 colVec = new System.Numerics.Vector3(colour.r / 255f, colour.g / 255f, colour.b / 255f);
			ImGuiExtras.ItemWidthForText("Light col"); ImGui.ColorEdit3("Light col", ref colVec);
			colour = new Color(colVec.X, colVec.Y, colVec.Z);

			ImGuiExtras.ItemWidthForText("Light col"); ImGui.SliderFloat("Strength", ref strength, 0f, 5f);

		}


	}
}
