using ArcticFoxEngine.Debug;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiDebugLogWindow : GuiWindow {

		public override string name => "Debug Log";

		public override void Render() {
			ImGui.ShowDebugLogWindow(ref open);
		}
	}
}
