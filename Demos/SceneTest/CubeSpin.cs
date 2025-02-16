using ArcticFoxEngine.Nodes;
using ArcticFoxEngine.Rendering;
using ImGuiNET;

namespace ArcticFoxEngine.Demos.SceneTest {
	public class CubeSpin : Node {

		Texture mainTex;

		List<Node> xSpin;
		List<Node> ySpin;
		List<Node> zSpin;
		int numPerRing = 8;
		bool animate;
		float speed;
		float t;
		float ringSize = 14f;


		public CubeSpin() {
			name = "Cube Spin";

			mainTex = new Texture(".res/Textures/uv_512.png");

			CameraController cameraController = CreateChild<CameraController>();
			cameraController.CreateChild<Camera>();

			xSpin = new List<Node>();
			ySpin = new List<Node>();
			zSpin = new List<Node>();

			Node xRing = CreateChild<BaseNode>("X Ring");
			Node yRing = CreateChild<BaseNode>("Y Ring");
			Node zRing = CreateChild<BaseNode>("Z Ring");

			for (int i = 0; i < numPerRing; i++) {

				Mesh cubeMesh = Mesh.CreatePrimitive(Mesh.Primitive.Cube);
				for (int v = 0; v < cubeMesh.vertices.Length; v++) {
					cubeMesh.vertices[v].color = new Vector4(1f, 1f, 1f, 1f);
					if (v >= 4) {
						cubeMesh.vertices[v].color = new Vector4(1f, 0f, 0f, 1f);
					}

				}

				MeshRenderer xObj = xRing.CreateChild<MeshRenderer>("X" + (i + 1));
				xObj.SetMesh(cubeMesh);
				((UnlitMaterial)xObj.material).mainTex = mainTex;
				xSpin.Add(xObj);

			}
			for (int i = 0; i < numPerRing; i++) {

				Mesh cubeMesh = Mesh.CreatePrimitive(Mesh.Primitive.Cube);
				for (int v = 0; v < cubeMesh.vertices.Length; v++) {
					cubeMesh.vertices[v].color = new Vector4(1f, 1f, 1f, 1f);
					if (v >= 4) {
						cubeMesh.vertices[v].color = new Vector4(0f, 1f, 0f, 1f);
					}
				}

				MeshRenderer yObj = yRing.CreateChild<MeshRenderer>("Y" + (i + 1));
				yObj.SetMesh(cubeMesh);
				((UnlitMaterial)yObj.material).mainTex = mainTex;
				ySpin.Add(yObj);

			}
			for (int i = 0; i < numPerRing; i++) {

				Mesh cubeMesh = Mesh.CreatePrimitive(Mesh.Primitive.Cube);
				for (int v = 0; v < cubeMesh.vertices.Length; v++) {
					cubeMesh.vertices[v].color = new Vector4(1f, 1f, 1f, 1f);
					if (v % 4 < 2) {
						cubeMesh.vertices[v].color = new Vector4(0f, 0f, 1f, 1f);
					}
				}

				MeshRenderer zObj = zRing.CreateChild<MeshRenderer>("Z" + (i + 1));
				zObj.SetMesh(cubeMesh);
				((UnlitMaterial)zObj.material).mainTex = mainTex;
				zSpin.Add(zObj);

			}

			animate = true;
			speed = 0.5f;
			t = 0f;


			Enable();
		}

		public override void Update() {

			if (animate == true) {
				t += speed * (float)Profiler.deltaTime * 0.3f;
			}
			t %= 1;

			for (int i = 0; i < xSpin.Count; i++) {

				float proportion = (float)i / xSpin.Count;
				Node obj = xSpin[i];
				obj.transform.localRotation = Quaternion.RotationYawPitchRoll(0f, ((proportion + t) % 1) * MathF.PI * 2f, 0f);
				obj.transform.localPosition = obj.transform.Back * ringSize;

			}

			for (int i = 0; i < ySpin.Count; i++) {

				float proportion = (float)i / ySpin.Count;
				Node obj = ySpin[i];
				obj.transform.localRotation = Quaternion.RotationYawPitchRoll(((proportion + t + (1f / 24f)) % 1) * MathF.PI * 2f, 0f, 0f);
				obj.transform.localPosition = obj.transform.Back * ringSize;

			}

			for (int i = 0; i < zSpin.Count; i++) {

				float proportion = (float)i / zSpin.Count;
				Node obj = zSpin[i];
				obj.transform.localRotation = Quaternion.RotationYawPitchRoll(0f, 0f, ((proportion + t + (2f / 24f)) % 1) * MathF.PI * 2f);
				obj.transform.localPosition = obj.transform.Up * ringSize;

			}

		}

		public override void DrawInspector() {
			base.DrawInspector();
			animate ^= ImGui.Checkbox("Animate", ref animate);

			if (animate == false) { ImGui.BeginDisabled(); }
			ImGui.SliderFloat("Speed", ref speed, -1f, 1f);
			if (animate == false) { ImGui.EndDisabled(); }
			ImGui.SliderFloat("T", ref t, 0f, 1f);

		}

	}
}
