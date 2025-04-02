using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {

	[GuiWindowOptions("ImGui/Demo")]
	internal class ImGuiDemoWindow : GuiWindow {

		public override void Render() {
			ImGui.ShowDemoWindow(ref open);
		}
	}
}
