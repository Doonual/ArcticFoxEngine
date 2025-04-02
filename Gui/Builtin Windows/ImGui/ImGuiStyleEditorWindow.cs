using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {

	[GuiWindowOptions("ImGui/Style Editor")]
	internal class ImGuiStyleEditorWindow : GuiWindow {

		public override void Render() {
			ImGui.Begin("Style Editor", ref open);
			ImGui.ShowStyleEditor();
			ImGui.End();
		}

	}
}
