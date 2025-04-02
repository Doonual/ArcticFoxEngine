using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {

	[GuiWindowOptions("ImGui/Stack Tool")]
	internal class ImGuiStackToolWindow : GuiWindow {

		public override void Render() {
			ImGui.ShowStackToolWindow(ref open);
		}

	}
}
