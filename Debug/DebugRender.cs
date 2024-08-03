using ImGuiNET;

namespace ArcticFoxEngine.Debug {
	internal class DebugRender : DebugWindow {

		bool showRenderWindow;

		internal DebugRender() {
			showRenderWindow = true;
		}

		internal override string name => "Render";

		internal override void Render() {

			if (showRenderWindow == true) {
				ImGui.Begin("Render", ref showRenderWindow, ImGuiWindowFlags.None);
			}

		}
	}
}
