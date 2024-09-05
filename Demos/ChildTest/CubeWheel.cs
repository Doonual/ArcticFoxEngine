using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Testing.ChildTest {
	public class CubeWheel : Node {

		float radius;
		float angle;
		float omega;

		public CubeWheel() {
			radius = 7f;
			angle = MathUtil.RandomFloat(0f, MathF.PI * 2);
			omega = MathUtil.RandomFloat(0.1f, 0.4f);

			CreateChild<Transform>();

			SetName("Cube Wheel");
			Enable();
		}

		public void Stop() {
			omega = 0f;
			angle = 0f;
		}

		public override void Update() {

			transformChild.rotation = Quaternion.RotationYawPitchRoll(0f, 0f, angle);

			angle += omega * Profiler.deltaTime;
			angle %= MathF.PI * 2f;

		}


		public void Propagate(int count, float radiusUpdate) {

			radius = radiusUpdate;

			Node cubeChild = CreateChild<EmptyNode>("Cube");
			cubeChild.CreateChild<Transform>();
			cubeChild.CreateChild<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			cubeChild.transformChild.position.y = radiusUpdate / 2f;
			cubeChild.transformChild.scale = new Vector3(radiusUpdate / 10f, radiusUpdate, radiusUpdate / 10f);

			if (count <= 0) { return; }

			CubeWheel cw = CreateChild<CubeWheel>();
			cw.transformChild.position.y = radiusUpdate;
			cw.Propagate(count - 1, MathUtil.RandomFloat(radius * 0.4f, radius * 0.7f));

		}

		public override void Debug() {
			base.Debug();
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
