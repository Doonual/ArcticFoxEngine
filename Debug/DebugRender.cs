using ImGuiNET;
using ClickableTransparentOverlay;
using CoolClassLibrary;

namespace ArcticFoxEngine.Debug {
	public static class DebugRender {

		static bool showImGuiDemo;
		static bool showRenderWindow;

		static DebugRender() {
			showRenderWindow = true;
		}

		internal static void Render() {

			if (showRenderWindow == true) {
				ImGui.Begin("Render", ref showRenderWindow, ImGuiWindowFlags.None);

				showImGuiDemo ^= ImGui.RadioButton("Show ImGui Demo", showImGuiDemo);

				if (showImGuiDemo == true) {
					ImGui.ShowDemoWindow();
				}
			}
			

		}
	}
}
