
using ArcticFoxEngine.Rendering;
using CoolClassLibrary;
using ImGuiNET;

namespace ArcticFoxEngine.Nodes {

	public class LightingSystem : Node {

		internal override string nodeIconPath => ".res/NodeIcons/LightManager.png";
		LitRenderPipeline.LightingWorld lightingWorld;

		Transform sunTransform;

		public LightingSystem() {
			name = "Lighting System";

			Node fakeSun = CreateChild<BaseNode>("Sun");
			sunTransform = fakeSun.CreateChild<Transform>();
			MeshRenderer sunRenderer = fakeSun.CreateChild<MeshRenderer>();
			sunRenderer.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));


			lightingWorld = new LitRenderPipeline.LightingWorld();

			Enable();
		}

		public override void Update() {

			sunTransform.position = lightingWorld.sunDir * -50f;
			sunTransform.scale = new Vector3(10f, 10f, 10f);
			LitRenderPipeline.SetLightingInfo(lightingWorld);
		}

		public override void Debug() {

			System.Numerics.Vector3 sunDirSys = lightingWorld.sunDir;
			ImGui.DragFloat3("Sun direction", ref sunDirSys, 0.01f);
			if (sunDirSys.X == float.NaN || sunDirSys.Y == float.NaN || sunDirSys.Z == float.NaN) {
				sunDirSys = new System.Numerics.Vector3(1f, 0f, 0f);
			}
			lightingWorld.sunDir = sunDirSys;
			lightingWorld.sunDir = lightingWorld.sunDir.Normalize();

			ImGui.SliderFloat("Ambient light", ref lightingWorld.ambientLight, 0f, 1f);

		}

	}
}
