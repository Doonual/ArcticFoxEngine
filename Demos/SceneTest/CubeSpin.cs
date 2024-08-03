using ArcticFoxEngine.Components;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Testing.SceneTest {
	public class CubeSpin : Component {

		List<GameObject> xSpin;
		List<GameObject> ySpin;
		List<GameObject> zSpin;
		int numPerRing = 8;
		bool animate;
		float speed;
		float t;
		float ringSize = 14f;
		

		public override void Start() {

			xSpin = new List<GameObject>();
			ySpin = new List<GameObject>();
			zSpin = new List<GameObject>();



			for (int i = 0; i < numPerRing; i++) {

				Mesh cubeMesh = Mesh.CreatePrimitive(Mesh.Primitive.Cube);
				for (int v = 0; v < cubeMesh.vertices.Length; v ++) {
					cubeMesh.vertices[v].Color = new Vector4(1f, 1f, 1f, 1f);
					if (v >= 4) {
						cubeMesh.vertices[v].Color = new Vector4(1f, 0f, 0f, 1f);
					}
					
				}

				GameObject xObj = gameObject.scene.InstantiateObject("X" + (i + 1));
				xObj.SetParent(gameObject);
				xObj.AddComponent<MeshRenderer>().SetMesh(cubeMesh);
				xSpin.Add(xObj);

			}

			for (int i = 0; i < numPerRing; i++) {

				Mesh cubeMesh = Mesh.CreatePrimitive(Mesh.Primitive.Cube);
				for (int v = 0; v < cubeMesh.vertices.Length; v++) {
					cubeMesh.vertices[v].Color = new Vector4(1f, 1f, 1f, 1f);
					if (v >= 4) {
						cubeMesh.vertices[v].Color = new Vector4(0f, 1f, 0f, 1f);
					}
				}

				GameObject yObj = gameObject.scene.InstantiateObject("Y" + (i + 1));
				yObj.SetParent(gameObject);
				yObj.AddComponent<MeshRenderer>().SetMesh(cubeMesh);
				ySpin.Add(yObj);

			}

			for (int i = 0; i < numPerRing; i++) {

				Mesh cubeMesh = Mesh.CreatePrimitive(Mesh.Primitive.Cube);
				for (int v = 0; v < cubeMesh.vertices.Length; v++) {
					cubeMesh.vertices[v].Color = new Vector4(1f, 1f, 1f, 1f);
					if (v % 4 < 2) {
						cubeMesh.vertices[v].Color = new Vector4(0f, 0f, 1f, 1f);
					}
				}

				GameObject zObj = gameObject.scene.InstantiateObject("Z" + (i + 1));
				zObj.SetParent(gameObject);
				zObj.AddComponent<MeshRenderer>().SetMesh(cubeMesh);
				zSpin.Add(zObj);

			}

			animate = true;
			speed = 0.5f;
			t = 0f;


		}

		public override void Update() {
			
			if (animate == true) {
				t += speed * 0.001f;
			}
			t %= 1;

			for (int i = 0; i < xSpin.Count; i ++) {

				float proportion = (float)i / xSpin.Count;
				GameObject obj = xSpin[i];
				obj.transform.rotation = Quaternion.RotationYawPitchRoll(0f, ((proportion + t) % 1) * MathF.PI * 2f, 0f);
				obj.transform.position = obj.transform.Back * ringSize;

			}

			for (int i = 0; i < ySpin.Count; i++) {

				float proportion = (float)i / ySpin.Count;
				GameObject obj = ySpin[i];
				obj.transform.rotation = Quaternion.RotationYawPitchRoll(((proportion + t + (1f / 24f)) % 1) * MathF.PI * 2f, 0f, 0f);
				obj.transform.position = obj.transform.Back * ringSize;

			}

			for (int i = 0; i < zSpin.Count; i++) {

				float proportion = (float)i / zSpin.Count;
				GameObject obj = zSpin[i];
				obj.transform.rotation = Quaternion.RotationYawPitchRoll(0f, 0f, ((proportion + t + (2f / 24f)) % 1) * MathF.PI * 2f);
				obj.transform.position = obj.transform.Up * ringSize;

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
