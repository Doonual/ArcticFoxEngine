using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiUserGuideWindow : GuiWindow {

		public override string name => "User Guide";

		public ImGuiUserGuideWindow(params string[] menuGroups) : base(menuGroups) { }

		public override void Render() {
			ImGui.Begin("User Guide", ref open);
			ImGui.ShowUserGuide();
			ImGui.End();
		}
	}
}
