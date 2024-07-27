using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace ArcticFoxEngine {
	public class Scene {

		internal static Scene activeScene;

		private List<GameObject> objects;

		public GeometryInfo mainGeometry;

		public Scene() {
			objects = new List<GameObject>();
			mainGeometry = new GeometryInfo();
		}

		
		public GameObject InstantiateObject(string name) {
			GameObject newGo = new GameObject(name);
			objects.Add(newGo);
			newGo.scene = this;
			return newGo;
		}
		public GameObject InstantiateObject() {
			return InstantiateObject("Object #" + objects.Count);
		}

		public void SetActiveScene() {
			activeScene = this;
		}

		internal void NewFrame() {
			
			for (int i = 0; i < objects.Count; i ++) {
				objects[i].UpdateComponents();
			}

			for (int i = 0; i < objects.Count; i++) {
				objects[i].RenderComponents();
			}

		}

		private int selectedObjectIndex = 0;
		internal void DebugEvent() {

			ImGui.SeparatorText("Objects: " + objects.Count);

			string[] objectNames = new string[objects.Count];
			for (int i = 0; i < objects.Count; i ++) {
				objectNames[i] = objects[i].name;
			}
			ImGui.Combo("Object", ref selectedObjectIndex, objectNames, objects.Count);

			ImGui.BeginChild(1, new System.Numerics.Vector2(-1f, 500f), true);
			objects[selectedObjectIndex].DebugComponents();
			ImGui.EndChild();

		}

		public GameObject[] GetObjects() {
			return objects.ToArray();
		}

	}
}
