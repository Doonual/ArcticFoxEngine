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

		public Scene scene {
			get;
			internal set;
		}
		public bool enabled { get; private set; }

		private List<Component> components;

		public Transform transform { get { return (Transform)components[0]; } }

		public string name;

		internal GameObject(string name) {
			components = new List<Component>();
			AddComponent<Transform>();
			this.name = name;
			this.enabled = true;
		}

		internal void UpdateComponents() {
			for (int i = 0; i < components.Count; i++) {
				components[i].Update();
			}
		}
		internal void DebugComponents() {

			//ImGui.SeparatorText("Object #" + GetHashCode());
			ImGui.Text("Name: " + name);
			ImGui.Text("Components (" + components.Count + ")");

			ImGui.Separator();
			for (int i = 0; i < components.Count; i++) {

				ImGui.PushID(components[i].GetHashCode() );

				if (components[i].GetType() == typeof(Transform)) {
					ImGui.BeginDisabled();
				}
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
		internal void RenderComponents() {
			for (int i = 0; i < components.Count; i++) {
				components[i].OnRender();
			}
		}

		public void Enable() {
			if (enabled == true) { return; }
			enabled = true;
			for (int i = 0; i < components.Count; i ++) {
				components[i].ObjectEnable();
			}
		}
		public void Disable() {
			if (enabled == false) { return; }
			enabled = false;
			for (int i = 0; i < components.Count; i++) {
				components[i].ObjectDisable();
			}
		}

		public T GetComponent<T>() where T : Component, new() {

			for (int i = 0; i < components.Count; i ++) {

				if (components[i].GetType() == typeof(T)) {
					return (T)components[i];
				}

			}

			return null;

		}
		public T AddComponent<T>() where T : Component, new() {
			T newComp = new T();
			components.Add(newComp);
			newComp.gameObject = this;
			newComp.dependantEnabled = enabled;
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

	}
}
