using ArcticFoxEngine.Debug;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiDebugLogWindow : GuiWindow {

		public override string name => "Debug Log";

		public ImGuiDebugLogWindow(params string[] menuGroups) : base(menuGroups) { }

		public override void Render() {
			ImGui.ShowDebugLogWindow(ref open);
		}
	}
}
