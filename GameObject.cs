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

		private List<Component> components;
		public Transform transform {
			get {
				return GetComponent<Transform>();
			}
		}

		public string name;

		public GameObject(string name) {
			components = new List<Component>();
			components.Add(new Transform());
			this.name = name;
		}
		public GameObject() {
			components = new List<Component>();
			components.Add(new Transform());
			name = "Unnamed";
		}

		internal void StartComponents() {
			for (int i = 0; i < components.Count; i ++) {
				components[i].Start();
			}
		}
		internal void UpdateComponents() {
			for (int i = 0; i < components.Count; i++) {
				components[i].Update();
			}
		}
		internal void DebugComponents() {
			for (int i = 0; i < components.Count; i++) {
				if (ImGui.CollapsingHeader("Component " + i + ": " + components[i].GetType().Name) == true) {
					ImGui.Indent();
					components[i].Debug();
					ImGui.Unindent();
				}
			}
		}
		internal void RenderComponents() {
			for (int i = 0; i < components.Count; i++) {
				components[i].OnRender();
			}
		}

		public T GetComponent<T>() where T : Component {

			for (int i = 0; i < components.Count; i ++) {

				if (components[i].GetType() == typeof(T)) {
					return (T)components[i];
				}

			}

			return null;

		}
		public void AddComponent(Component comp) {
			components.Add(comp);
			comp.gameObject = this;
		}
		public void RemoveComponent(Component comp) {
			components.Remove(comp);
		}
		public void RemoveComponent<T>() where T : Component {
			components.Remove(GetComponent<T>());
		}

	}
}
