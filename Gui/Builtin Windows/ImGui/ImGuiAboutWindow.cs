using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiAboutWindow : GuiWindow {

		public override string name => "About";

		public ImGuiAboutWindow(params string[] menuGroups) : base(menuGroups) { }

		public override void Render() {
			ImGui.ShowAboutWindow(ref open);
		}
	}
}
