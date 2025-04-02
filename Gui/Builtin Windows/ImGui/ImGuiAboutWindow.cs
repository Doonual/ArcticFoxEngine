using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {

	[GuiWindowOptions("ImGui/About")]
	internal class ImGuiAboutWindow : GuiWindow {
		
		public override void Render() {
			ImGui.ShowAboutWindow(ref open);
		}
	}
}
