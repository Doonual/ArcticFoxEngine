using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiStyleEditorWindow : GuiWindow {

		public override string name => "Style Editor";

		public ImGuiStyleEditorWindow(params string[] menuGroups) : base(menuGroups) { }

		public override void Render() {
			ImGui.Begin("Style Editor", ref open);
			ImGui.ShowStyleEditor();
			ImGui.End();
		}
	}
}
