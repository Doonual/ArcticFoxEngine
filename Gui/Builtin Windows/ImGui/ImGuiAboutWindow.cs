using ArcticFoxEngine.Debug;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiAboutWindow : GuiWindow {

		public override string name => "About";

		public override void Render() {
			ImGui.ShowAboutWindow(ref open);
		}
	}
}
