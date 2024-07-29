using ImGuiNET;
using ClickableTransparentOverlay;

namespace ArcticFoxEngine.Debug {
	public static class DebugRender {

		static bool showImGuiDemo;

		internal static void Render() {
			ImGui.Begin("Render");
			showImGuiDemo ^= ImGui.RadioButton("Show ImGui Demo", showImGuiDemo);

			if (showImGuiDemo == true) {
				ImGui.ShowDemoWindow();
			}

		}
	}
}
