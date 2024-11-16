using ArcticFoxEngine.Debug;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiMetricsWindow : GuiWindow {

		public override string name => "Metrics";

		public ImGuiMetricsWindow(params string[] menuGroups) : base(menuGroups) { }

		public override void Render() {
			ImGui.ShowMetricsWindow(ref open);
		}
	}
}
