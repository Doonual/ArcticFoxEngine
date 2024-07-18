using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace ArcticFoxEngine {
	public abstract class Component {

		public GameObject gameObject {
			get;
			internal set;
		}

		internal virtual string debugName => "";
		internal virtual string debugDescription => "";

		public virtual void Start() {

		}
		public virtual void Update() {

		}
		public virtual void OnRender() {

		}
		public virtual void Debug() {

			ImGui.SeparatorText(debugName + " Component");
			ImGui.TextWrapped(debugDescription);
			ImGui.Separator();

		}

	}
}
