using ImGuiNET;
using ClickableTransparentOverlay;

namespace ArcticFoxEngine.Debug {
	public static class DebugScene {

		internal static void Render() {

			if (Scene.activeScene != null) {
				Scene.activeScene.DebugEvent();
			}
			
		}
	}
}
