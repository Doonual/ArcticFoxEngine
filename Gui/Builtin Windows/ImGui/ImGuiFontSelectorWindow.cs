using ArcticFoxEngine.Debug;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiFontSelectorWindow : GuiWindow {

		public override string name => "Font Selector";

		public override void Render() {
			ImGui.Begin("Font Selector", ref open);
			ImGui.ShowFontSelector("Font Selector");
			ImGui.End();
		}
	}
}
