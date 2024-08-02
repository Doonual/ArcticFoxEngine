using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using System.Windows.Forms;

namespace ArcticFoxEngine {
	public class Scene {

		internal static Scene activeScene;

		internal List<GameObject> objects;	// Top level objects here only
		public GeometryResources mainGeometry;

		public Scene() {
			objects = new List<GameObject>();
			mainGeometry = new GeometryResources();
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
		private bool addingNewObj = false;
		private string newObjString = "";
		internal void DebugEvent() {

			if (expandedChildren == null) {
				expandedChildren = new List<GameObject>();
			}

			ImGui.SeparatorText("Objects: " + objects.Count);

			#region Object Tree

			// This scares me
			Action<GameObject> inspectObjectTree = null;
			inspectObjectTree = (GameObject obj) => {

				ImGui.PushID(obj.GetHashCode());

				#region Object enabled checkbox

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

				#endregion
				#region Expand children arrow button

				bool expandCurrent = false;
				if (obj.GetChildCount() > 0) {

					ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(0f, 0f));

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

					ImGui.SameLine();
					ImGui.PopStyleVar();

				}
				else {
					/*
					ImGui.PushStyleColor(ImGuiCol.Button,			new System.Numerics.Vector4(0f, 0f,0f, 0f));
					ImGui.PushStyleColor(ImGuiCol.ButtonHovered,	new System.Numerics.Vector4(0f, 0f, 0f, 0f));
					ImGui.PushStyleColor(ImGuiCol.ButtonActive,		new System.Numerics.Vector4(0f, 0f, 0f, 0f));

					ImGui.ArrowButton(obj.GetHashCode() + " show children", ImGuiDir.None);

					ImGui.PopStyleColor();
					ImGui.PopStyleColor();
					ImGui.PopStyleColor();
					*/
				}

				

				#endregion

				if (debugObjectSelected == obj) {
					ImGui.PushStyleColor(ImGuiCol.Button,			new System.Numerics.Vector4(0.2588f / 2f, 0.5882f / 2f, 0.9804f / 2f, 0.4f));
					ImGui.PushStyleColor(ImGuiCol.ButtonHovered,	new System.Numerics.Vector4(0.2588f / 2f, 0.5882f / 2f, 0.9804f / 2f, 0.4f));
					ImGui.PushStyleColor(ImGuiCol.ButtonActive,		new System.Numerics.Vector4(0.2588f / 2f, 0.5882f / 2f, 0.9804f / 2f, 0.4f));
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

			#endregion
			ImGui.Separator();

			#region "Add New" and "Delete Current" buttons

			bool addNewObject = false;
			bool deleteObject = false;
			if (addingNewObj == false) {
				
				ImGui.Columns(2);
				addingNewObj = ImGui.Button("Add New", new System.Numerics.Vector2(-0.0001f, 0f));
				ImGui.NextColumn();

				if (debugObjectSelected == null) { ImGui.BeginDisabled(); }
				deleteObject = ImGui.Button("Delete Current", new System.Numerics.Vector2(-0.0001f, 0f));
				if (debugObjectSelected == null) { ImGui.EndDisabled(); }
				ImGui.Columns();

				if (addingNewObj == true) {
					newObjString = "";
				}

			}
			else {
				ImGui.Columns(2);
				ImGui.PushItemWidth(-1f);
				ImGui.InputText("", ref newObjString, 128);
				ImGui.PopItemWidth();

				ImGui.NextColumn();
				if (ImGui.Button("Create") == true) {
					addNewObject = true;
					addingNewObj = false;
				}
				ImGui.SameLine();
				if (ImGui.Button("Cancel") == true) {
					addingNewObj = false;
				}

			}

			#endregion

			ImGui.EndChild();

			ImGui.BeginChild("Object components inspector", new System.Numerics.Vector2(-1f, 500f), true);
			if (debugObjectSelected != null) {
				debugObjectSelected.DebugEvent();
			}
			else {
				ImGui.Text("Select an object to inspect");
			}
			
			ImGui.EndChild();

			if (addNewObject == true) {
				InstantiateObject(newObjString);
			}
			if (deleteObject == true) {
				DestroyObject(debugObjectSelected);
				debugObjectSelected = null;
			}

		}

		private bool disposed = false;
		public void Dispose() {
			if (disposed == true) { return; }
			disposed = true;
			mainGeometry.Dispose();
		}
		~Scene() {
			Dispose();
		}

	}
}
