using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;

namespace ArcticFoxEngine.Demos.ChildTest {
	public class CubeWheel : Node {

		float radius;
		float angle;
		float omega;

		public CubeWheel() {

			name = "Cube Wheel";

			radius = 7f;
			angle = MathUtil.RandomFloat(0f, MathF.PI * 2);
			omega = MathUtil.RandomFloat(0.1f, 0.4f);

			Enable();
		}

		public void Stop() {
			omega = 0f;
			angle = 0f;
		}

		public override void Update() {

			transform.localRotation = Quaternion.RotationYawPitchRoll(0f, 0f, angle);

			angle += omega * Profiler.deltaTime;
			angle %= MathF.PI * 2f;

		}


		public void Propagate(int count, float radiusUpdate) {

			radius = radiusUpdate;

			MeshRenderer cubeChild = CreateChild<MeshRenderer>("Cube");
			cubeChild.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			cubeChild.transform.localPosition.y = radiusUpdate / 2f;
			cubeChild.transform.localScale = new Vector3(radiusUpdate / 10f, radiusUpdate, radiusUpdate / 10f);

			if (count <= 0) { return; }

			CubeWheel cw = CreateChild<CubeWheel>();
			cw.transform.localPosition.y = radiusUpdate;
			cw.Propagate(count - 1, MathUtil.RandomFloat(radius * 0.4f, radius * 0.7f));

		}

		public override void DrawInspector() {
			base.DrawInspector();
			ImGui.SliderFloat("Radius", ref radius, 1.9f, 6f);
			ImGui.SliderFloat("Angle", ref angle, 0f, MathF.PI * 2);
			ImGui.SliderFloat("Omega", ref omega, 0.004f, 0.01f);

			if (ImGui.Button("Adopt Camera") == true) {
				Log.Warn("Not implemented yet");
				//gameObject.scene.mainCamera.gameObject.SetParent(gameObject);
			}

		}

	}
}
