using ArcticFoxEngine.Debug;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiStyleEditorWindow : GuiWindow {

		public override string name => "Style Editor";

		public override void Render() {
			ImGui.Begin("Style Editor", ref open);
			ImGui.ShowStyleEditor();
			ImGui.End();
		}
	}
}
