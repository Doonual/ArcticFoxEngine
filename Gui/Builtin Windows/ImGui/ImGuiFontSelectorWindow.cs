using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {

	[GuiWindowOptions("ImGui/Font Selector")]
	internal class ImGuiFontSelectorWindow : GuiWindow {

		public override void Render() {
			ImGui.Begin("Font Selector", ref open);
			ImGui.ShowFontSelector("Font Selector");
			ImGui.End();
		}
	}
}
