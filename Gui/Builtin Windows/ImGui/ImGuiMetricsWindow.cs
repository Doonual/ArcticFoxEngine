using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {

	[GuiWindowOptions("ImGui/Metrics")]
	internal class ImGuiMetricsWindow : GuiWindow {

		public override void Render() {
			ImGui.ShowMetricsWindow(ref open);
		}

	}
}
