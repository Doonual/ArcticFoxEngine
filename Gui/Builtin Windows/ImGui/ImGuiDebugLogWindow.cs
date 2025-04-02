using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	[GuiWindowOptions("ImGui/Debug Log")]
	internal class ImGuiDebugLogWindow : GuiWindow {

		public override void Render() {
			ImGui.ShowDebugLogWindow(ref open);
		}
	}
}
