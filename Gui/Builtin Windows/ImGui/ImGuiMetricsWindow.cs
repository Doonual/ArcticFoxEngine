using ArcticFoxEngine.Debug;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiMetricsWindow : GuiWindow {

		public override string name => "Metrics";

		public override void Render() {
			ImGui.ShowMetricsWindow(ref open);
		}
	}
}
