using ArcticFoxEngine.Debug;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiDemoWindow : GuiWindow {

		public override string name => "Demo";

		public ImGuiDemoWindow(params string[] menuGroups) : base(menuGroups) { }

		public override void Render() {
			ImGui.ShowDemoWindow(ref open);
		}
	}
}
