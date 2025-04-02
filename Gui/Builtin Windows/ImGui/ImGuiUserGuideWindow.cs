using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {

	[GuiWindowOptions("ImGui/User Guide")]
	internal class ImGuiUserGuideWindow : GuiWindow {

		public override void Render() {
			ImGui.Begin("User Guide", ref open);
			ImGui.ShowUserGuide();
			ImGui.End();
		}

	}
}
