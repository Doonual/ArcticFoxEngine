using ArcticFoxEngine.Components;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Testing.ChildTest {
	public class CubeWheel : Component {

		float radius;
		float angle;
		float omega;

		GameObject meshRenderObj;

		public override void Start() {

			radius = 7f;
			angle = MathUtil.RandomFloat(0f, MathF.PI * 2);
			omega = MathUtil.RandomFloat(0.001f, 0.004f);

			meshRenderObj = gameObject.InstantiateChild("Mesh renderer obj");
			meshRenderObj.AddComponent<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			meshRenderObj.transform.scale = new Vector3(radius, radius / 10f, radius / 10f);
			meshRenderObj.transform.position.x = -radius / 2f;


		}


		public override void Update() {

			transform.rotation = Quaternion.RotationYawPitchRoll(0f, 0f, angle);
			transform.position = transform.Right * radius;

			angle += omega;
			angle %= MathF.PI * 2f;

		}

		public void Stop() {
			omega = 0f;
			angle = MathF.PI / 2f;
		}

		public void Propagate(int count, float radiusUpdate) {

			radius = radiusUpdate;
			meshRenderObj.transform.scale = new Vector3(radius, radius / 10f, radius / 10f);
			meshRenderObj.transform.position.x = -radius / 2f;

			if (count <= 0) { return; }

			GameObject nextCube = gameObject.InstantiateChild("Count: " + count);
			nextCube.AddComponent<MeshRenderer>();
			CubeWheel cw = nextCube.AddComponent<CubeWheel>();
			cw.Propagate(count - 1, MathUtil.RandomFloat(radius * 0.4f, radius * 0.7f));

		}

		public override void Debug() {
			base.Debug();
			ImGui.SliderFloat("Radius", ref radius, 1.9f, 6f);
			ImGui.SliderFloat("Angle", ref angle, 0f, MathF.PI * 2);
			ImGui.SliderFloat("Omega", ref omega, 0.004f, 0.01f);
		}

	}
}
