using ImGuiNET;
using ClickableTransparentOverlay;

namespace ArcticFoxEngine.Debug {
	public static class DebugScene {

		static int selectedObject = 0;

		internal static void Render() {

			ImGui.Begin("Scene");
			if (Scene.activeScene != null) {
				Scene.activeScene.DebugEvent();
			}
			ImGui.End();
			
		}
	}
}
