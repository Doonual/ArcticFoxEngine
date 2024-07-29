using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoolClassLibrary;
using ImGuiNET;

namespace ArcticFoxEngine {
	public abstract class Component {

		public GameObject gameObject { get;	internal set; }
		public Transform transform { get { return gameObject.transform;	} }
		public bool enabled { get; private set; } // Whether the component is enabled or disabled independant of the object
		internal bool dependantEnabled; // Overally enabled state of the component, if the object is disabled, this is disabled

		internal Component() {
			enabled = false;
		}

		internal virtual string debugName => "";
		internal virtual string debugDescription => "";
		internal virtual Type[] dependencies => new Type[0];

		public virtual void OnEnable() { }
		public virtual void OnDisable() { }
		public virtual void Start() { }
		public virtual void Update() { }
		public virtual void OnRender() { }


		internal void ObjectEnable() {
			if (dependantEnabled == true) { return; }
			dependantEnabled = true;
			if (enabled == true) {
				OnEnable();
			}
		}
		internal void ObjectDisable() {
			if (dependantEnabled == false) { return; }
			dependantEnabled = false;
			if (enabled == true) {
				OnDisable();
			}
		}

		public void Enable() {
			if (enabled == true) { return;	}
			enabled = true;
			if (dependantEnabled == true) {
				OnEnable();
			}
		}
		public void Disable() {
			if (enabled == false || GetType() == typeof(Transform)) { return; }
			enabled = false;
			if (dependantEnabled == true) {
				OnDisable();
			}
		}


		public virtual void Debug() {

			string debugNameActual = debugName;

			if (debugNameActual == "") {
				debugNameActual = GetType().Name;
			}

			ImGui.SeparatorText(debugNameActual + " Component");
			if (debugDescription != "") {
				ImGui.TextWrapped(debugDescription);
			}
			
			

		}

	}
}
