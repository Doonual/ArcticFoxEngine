using ImGuiNET;
using ClickableTransparentOverlay;

namespace ArcticFoxEngine.Debug {
	internal class DebugScene : DebugWindow {
		
		internal override string name => "Scene";
		internal override void Render() {

			if (Scene.activeScene != null) {
				Scene.activeScene.DebugEvent();
			}
			
		}
	}
}
