using ArcticFoxEngine.Debug;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiUserGuideWindow : GuiWindow {

		public override string name => "User Guide";

		public override void Render() {
			ImGui.Begin("User Guide", ref open);
			ImGui.ShowUserGuide();
			ImGui.End();
		}
	}
}
