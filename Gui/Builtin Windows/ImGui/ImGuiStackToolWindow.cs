using ArcticFoxEngine.Debug;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiStackToolWindow : GuiWindow {

		public override string name => "Stack Tool";

		public ImGuiStackToolWindow(params string[] menuGroups) : base(menuGroups) { }

		public override void Render() {
			ImGui.ShowStackToolWindow(ref open);
		}
	}
}
