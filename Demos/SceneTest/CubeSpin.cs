using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Testing.SceneTest {
	public class CubeSpin : Node {

		List<Node> xSpin;
		List<Node> ySpin;
		List<Node> zSpin;
		int numPerRing = 8;
		bool animate;
		float speed;
		float t;
		float ringSize = 14f;
		

		public CubeSpin() : base() {

			Node cameraObj = CreateChild<EmptyNode>("Main Camera");
			cameraObj.CreateChild<Transform>();
			Camera mainCam = cameraObj.CreateChild<Camera>();
			cameraObj.CreateChild<CameraController>();
			cameraObj.GetChild<Transform>().position = Vector3.Back * 25f;


			xSpin = new List<Node>();
			ySpin = new List<Node>();
			zSpin = new List<Node>();

			Node xRing = CreateChild<EmptyNode>("X Ring");
			xRing.CreateChild<Transform>();
			Node yRing = CreateChild<EmptyNode>("Y Ring");
			yRing.CreateChild<Transform>();
			Node zRing = CreateChild<EmptyNode>("Z Ring");
			zRing.CreateChild<Transform>();

			for (int i = 0; i < numPerRing; i++) {

				Mesh cubeMesh = Mesh.CreatePrimitive(Mesh.Primitive.Cube);
				for (int v = 0; v < cubeMesh.vertices.Length; v ++) {
					cubeMesh.vertices[v].color = new Vector4(1f, 1f, 1f, 1f);
					if (v >= 4) {
						cubeMesh.vertices[v].color = new Vector4(1f, 0f, 0f, 1f);
					}
					
				}

				Node xObj = xRing.CreateChild<EmptyNode>("X" + (i + 1));
				xObj.CreateChild<Transform>();
				xObj.CreateChild<MeshRenderer>().SetMesh(cubeMesh);
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

				Node yObj = yRing.CreateChild<EmptyNode>("Y" + (i + 1));
				yObj.CreateChild<Transform>();
				yObj.CreateChild<MeshRenderer>().SetMesh(cubeMesh);
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

				Node zObj = zRing.CreateChild<EmptyNode>("Z" + (i + 1));
				zObj.CreateChild<Transform>();
				zObj.CreateChild<MeshRenderer>().SetMesh(cubeMesh);
				zSpin.Add(zObj);

			}

			animate = true;
			speed = 0.5f;
			t = 0f;

			SetName("Cube Spin");
			Enable();
		}

		public override void Update() {
			
			if (animate == true) {
				t += speed * (float)Profiler.deltaTime * 0.3f;
			}
			t %= 1;

			for (int i = 0; i < xSpin.Count; i ++) {

				float proportion = (float)i / xSpin.Count;
				Node obj = xSpin[i];
				obj.GetChild<Transform>().rotation = Quaternion.RotationYawPitchRoll(0f, ((proportion + t) % 1) * MathF.PI * 2f, 0f);
				obj.GetChild<Transform>().position = obj.GetChild<Transform>().Back * ringSize;

			}

			for (int i = 0; i < ySpin.Count; i++) {

				float proportion = (float)i / ySpin.Count;
				Node obj = ySpin[i];
				obj.GetChild<Transform>().rotation = Quaternion.RotationYawPitchRoll(((proportion + t + (1f / 24f)) % 1) * MathF.PI * 2f, 0f, 0f);
				obj.GetChild<Transform>().position = obj.GetChild<Transform>().Back * ringSize;

			}

			for (int i = 0; i < zSpin.Count; i++) {

				float proportion = (float)i / zSpin.Count;
				Node obj = zSpin[i];
				obj.GetChild<Transform>().rotation = Quaternion.RotationYawPitchRoll(0f, 0f, ((proportion + t + (2f / 24f)) % 1) * MathF.PI * 2f);
				obj.GetChild<Transform>().position = obj.GetChild<Transform>().Up * ringSize;

			}

		}

		public override void Debug() {
			base.Debug();
			animate ^= ImGui.Checkbox("Animate", ref animate);

			if (animate == false) { ImGui.BeginDisabled(); }
			ImGui.SliderFloat("Speed", ref speed, -1f, 1f);
			if (animate == false) { ImGui.EndDisabled(); }
			ImGui.SliderFloat("T", ref t, 0f, 1f);

		}

	}
}
