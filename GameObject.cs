using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Debug;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine {
	public class GameObject {

		// Object Hierarchy
		public Scene scene { get; internal set; }		// Scene the object is contained in
		public GameObject parent { get; private set; }	// When null, indicates it is in the scene root
		private List<GameObject> children;

		// Object properties
		private List<Component> components;
		public string name;
		public bool enabled { get; private set; }
		private bool dependantEnabled;
		public bool globalEnabled { get { return enabled && dependantEnabled; } }

		public Transform transform { get { return (Transform)components[0]; } }	// Shortcut to the objects transform

		internal GameObject(string name) {
			components = new List<Component>();
			AddComponent<Transform>();
			this.name = name;

			

			parent = null;
			enabled = true;
			dependantEnabled = true;
			children = new List<GameObject>();

		}
		public GameObject InstantiateChild(string name = "") {
			return scene.InstantiateObject(name, this);
		}

		#region Parenting

		public void SetParent(GameObject parent) {
			if (this.parent == parent) { return; }

			// If we were parented to another object, remove this object from the previous parent's children
			if (this.parent != null) {
				this.parent.children.Remove(this);
			}

			this.parent = parent;
			if (parent == null) {
				// If we are setting the parent back to the scene root, add the object back to the scene root
				scene.objects.Add(this);
				dependantEnabled = true;
			}
			else {
				// Otherwise
				scene.objects.Remove(this);
				parent.children.Add(this);
				dependantEnabled = parent.enabled && parent.dependantEnabled;
			}

		}
		public int GetChildCount() {
			return children.Count;
		}
		public GameObject GetChild(int index) {
			return children[index];
		}

		#endregion
		#region Enabling

		internal void ParentEnable() {
			if (dependantEnabled == true) { return; }
			dependantEnabled = true;
			if (enabled == true) {
				for (int i = 0; i < components.Count; i++) {
					components[i].ObjectEnable();
				}
				for (int i = 0; i < children.Count; i++) {
					children[i].ParentEnable();
				}
			}
		}
		internal void ParentDisable() {
			if (dependantEnabled == false) { return; }
			dependantEnabled = false;
			if (enabled == true) {
				for (int i = 0; i < components.Count; i++) {
					components[i].ObjectDisable();
				}
				for (int i = 0; i < children.Count; i++) {
					children[i].ParentDisable();
				}
			}
		}
		public void Enable() {
			// Skip if both enabled and dependantEnabled is true, nothing to do
			if (enabled == true && dependantEnabled == true) { return; }
			enabled = true;
			if (dependantEnabled == true) {
				for (int i = 0; i < components.Count; i++) {
					components[i].ObjectEnable();
				}
				for (int i = 0; i < children.Count; i++) {
					children[i].ParentEnable();
				}
			}
		}
		public void Disable() {
			// Skip if either enabled or dependantEnabled is false, nothing to do
			if (enabled == false || dependantEnabled == false) {
				// We cannot guarantee enabled is false here, set it false just in case
				enabled = false;
				return;
			}
			enabled = false;
			if (dependantEnabled == true) {
				for (int i = 0; i < components.Count; i++) {
					components[i].ObjectDisable();
				}
				for (int i = 0; i < children.Count; i++) {
					children[i].ParentDisable();
				}
			}
		}

		#endregion
		#region Component management

		public T GetComponent<T>() where T : Component, new() {

			for (int i = 0; i < components.Count; i++) {

				if (components[i].GetType() == typeof(T)) {
					return (T)components[i];
				}

			}

			return null;

		}
		public Type GetComponent(Type type) {

			for (int i = 0; i < components.Count; i++) {
				if (components[i].GetType() == type) {
					return (components[i].GetType());
				}
			}
			return null;
		}
		public T AddComponent<T>() where T : Component, new() {
			T newComp = new T();
			components.Add(newComp);
			newComp.gameObject = this;
			newComp.dependantEnabled = enabled;

			// Check for dependencies
			bool dependencyProblem = false;
			for (int i = 0; i < newComp.dependencies.Length; i++) {
				if (GetComponent(newComp.dependencies[i]) == null) {
					Log.Warn("Component " + newComp.GetType().Name + " was added to object " + name + " with missing dependency " + newComp.dependencies[i].Name);
					Log.Warn("Adding default " + newComp.dependencies[i].Name);
					AddComponent(newComp.dependencies[i]);
					dependencyProblem = true;
				}
			}
			if (dependencyProblem == true) {
				Log.Raw(newComp.GetType().Name + " depends on the following components");
				for (int i = 0; i < newComp.dependencies.Length; i++) {
					Log.Raw(" - " + newComp.dependencies[i].Name);

				}
			}


			newComp.Start();
			newComp.Enable();
			return newComp;
		}
		public Component AddComponent(Type t) {

			Component newComp = (Component)Activator.CreateInstance(t);
			components.Add(newComp);
			newComp.gameObject = this;
			newComp.dependantEnabled = enabled;

			// Check for dependencies
			bool dependencyProblem = false;
			for (int i = 0; i < newComp.dependencies.Length; i++) {
				if (GetComponent(newComp.dependencies[i]) == null) {
					Log.Warn("Component " + newComp.GetType().Name + " was added to object " + name + " with missing dependency " + newComp.dependencies[i].Name);
					Log.Warn("Adding default " + newComp.dependencies[i].Name);
					AddComponent(newComp.dependencies[i]);
					dependencyProblem = true;
				}
			}
			if (dependencyProblem == true) {
				Log.Raw(newComp.GetType().Name + " depends on the following components");
				for (int i = 0; i < newComp.dependencies.Length; i++) {
					Log.Raw(" - " + newComp.dependencies[i].Name);
				}
			}


			newComp.Start();
			newComp.Enable();
			return newComp;

		}
		public void RemoveComponent(Component comp) {
			comp.Disable();
			components.Remove(comp);
		}
		public void RemoveComponent<T>() where T : Component, new() {
			RemoveComponent(GetComponent<T>());
		}

		#endregion

		#region Component events

		internal void UpdateEvent() {
			if (enabled == false) { return; }
			for (int i = 0; i < components.Count; i++) {
				if (components[i].enabled == false) { continue; }
				components[i].Update();
			}
			for (int i = 0; i < children.Count; i ++) {
				children[i].UpdateEvent();
			}
		}
		internal void RenderEvent() {
			if (enabled == false) { return; }
			for (int i = 0; i < components.Count; i++) {
				if (components[i].enabled == false) { return; }
				components[i].OnRender();
			}
			for (int i = 0; i < children.Count; i++) {
				children[i].RenderEvent();
			}
		}
		internal void DebugEvent() {

			ImGui.InputText("Name", ref name, 128);
			ImGui.Text("Components (" + components.Count + ")");

			ImGui.Separator();
			for (int i = 0; i < components.Count; i++) {

				ImGui.PushID(components[i].GetHashCode());

				// If the current component is the Transform component
				if (components[i].GetType() == typeof(Transform)) {
					ImGui.BeginDisabled();
				}

				// Checkbox for enabling and disabling components
				bool componentEnabled = components[i].enabled;
				if (ImGui.Checkbox("", ref componentEnabled) == true) {
					if (components[i].enabled == true) {
						components[i].Disable();
					}
					else {
						components[i].Enable();
					}
				}

				ImGui.SameLine();


				if (components[i].GetType() == typeof(Transform)) {
					ImGui.EndDisabled();
				}

				if (ImGui.CollapsingHeader(components[i].GetType().Name) == true) {
					ImGui.Indent();
					components[i].Debug();
					ImGui.Unindent();
				}

				ImGui.PopID();

			}



		}

		#endregion

		
		
	}
}
