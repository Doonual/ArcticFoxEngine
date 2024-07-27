using ImGuiNET;
using ClickableTransparentOverlay;

namespace ArcticFoxEngine.Debug {
	public static class DebugDemo {

		static bool showImGuiDemo;

		internal static void Render() {
			ImGui.Begin("Settings");
			showImGuiDemo ^= ImGui.RadioButton("Show ImGui Demo", showImGuiDemo);

			if (showImGuiDemo == true) {
				ImGui.ShowDemoWindow();
			}

		}
	}
}
