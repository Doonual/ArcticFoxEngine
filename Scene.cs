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

		// Top level objects here only
		internal List<GameObject> objects;

		public GeometryInfo mainGeometry;

		public Scene() {
			objects = new List<GameObject>();
			mainGeometry = new GeometryInfo();
		}


		public GameObject InstantiateObject(string name = "", GameObject parent = null) {
			
			if (name == "") {
				name = "Object #" + objects.Count;
			}
			
			GameObject newGo = new GameObject(name);
			objects.Add(newGo);
			newGo.scene = this;
			newGo.SetParent(parent);
			return newGo;
		}
		public void DestroyObject(GameObject obj) {

			obj.SetParent(null);
			objects.Remove(obj);
			
			obj.Disable();

		}
		public GameObject[] GetObjects() {
			return objects.ToArray();
		}


		public static void LoadScene(Scene scene) {
			activeScene = scene;
		}

		internal void NewFrame() {
			
			for (int i = 0; i < objects.Count; i ++) {
				objects[i].UpdateEvent();
			}

			for (int i = 0; i < objects.Count; i++) {
				objects[i].RenderEvent();
			}

		}

		private GameObject debugObjectSelected;
		private List<GameObject> expandedChildren;
		internal void DebugEvent() {

			if (expandedChildren == null) {
				expandedChildren = new List<GameObject>();
			}

			ImGui.SeparatorText("Objects: " + objects.Count);

			string[] objectNames = new string[objects.Count];
			for (int i = 0; i < objects.Count; i ++) {
				objectNames[i] = objects[i].name;
			}

			// This scares me
			Action<GameObject> inspectObjectTree = null;
			inspectObjectTree = (GameObject obj) => {

				ImGui.PushID(obj.GetHashCode());

				bool objectEnabled = obj.enabled;
				
				if (ImGui.Checkbox("", ref objectEnabled) == true) {
					if (obj.enabled == true) {
						obj.Disable();
					}
					else {
						obj.Enable();
					}
				}

				
				ImGui.SameLine();


				

				bool expandCurrent = false;
				if (obj.GetChildCount() > 0) {
					expandCurrent = expandedChildren.Contains(obj);
					ImGuiDir arrowDirection = ImGuiDir.Right;
					if (expandCurrent == true) {
						arrowDirection = ImGuiDir.Down;
					}
					
					if (ImGui.ArrowButton(obj.GetHashCode() + " show children", arrowDirection) == true) {
						if (expandCurrent == true) {
							expandedChildren.Remove(obj);
						}
						else {
							expandedChildren.Add(obj);
						}
					}

					ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(0f, 0f));
					ImGui.SameLine();
					ImGui.PopStyleVar();
				}

				if (debugObjectSelected == obj) {
					ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4(230f / 255f, 179f / 255f, 0f, 1f));
					ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new System.Numerics.Vector4(230f / 255f, 179f / 255f, 0f, 1f));
					ImGui.PushStyleColor(ImGuiCol.ButtonActive, new System.Numerics.Vector4(230f / 255f, 179f / 255f, 0f, 1f));
				}


				ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0f, 0.5f));

				if (obj.globalEnabled == false) {
					ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.4f, 0.4f, 0.4f, 1f));
				}

				if (ImGui.Button(obj.name, new System.Numerics.Vector2(-1f, 0f)) == true) {
					debugObjectSelected = obj;
				}
				if (obj.globalEnabled == false) {
					ImGui.PopStyleColor();
				}
				ImGui.PopStyleVar();

				if (debugObjectSelected == obj) {
					ImGui.PopStyleColor();
					ImGui.PopStyleColor();
					ImGui.PopStyleColor();
				}

				if (expandCurrent == true) {
					ImGui.Indent();
					for (int i = 0; i < obj.GetChildCount(); i ++) {
						inspectObjectTree(obj.GetChild(i));
					}
					ImGui.Unindent();
				}

				ImGui.PopID();



			};
			ImGui.Text("Object Tree:");
			
			ImGui.BeginChild((uint)GetHashCode(), new System.Numerics.Vector2(-1f, 200f), true);
			for (int i = 0; i < objects.Count; i ++) {
				inspectObjectTree(objects[i]);
			}
			ImGui.EndChild();

			ImGui.Columns(2);
			bool addNewObject = ImGui.Button("Add New", new System.Numerics.Vector2(-0.0001f, 0f));
			ImGui.NextColumn();

			if (debugObjectSelected == null) { ImGui.BeginDisabled(); }
			bool deleteObject = ImGui.Button("Delete current", new System.Numerics.Vector2(-0.0001f, 0f));
			if (debugObjectSelected == null) { ImGui.EndDisabled(); }
			ImGui.Columns();

			ImGui.BeginChild("Object components inspector", new System.Numerics.Vector2(-1f, 500f), true);
			if (debugObjectSelected != null) {
				debugObjectSelected.DebugEvent();
			}
			else {
				ImGui.Text("Select an object to inspect");
			}
			
			ImGui.EndChild();

			if (addNewObject == true) {
				InstantiateObject();
			}
			if (deleteObject == true) {
				DestroyObject(debugObjectSelected);
				debugObjectSelected = null;
			}

		}

		
	}
}
