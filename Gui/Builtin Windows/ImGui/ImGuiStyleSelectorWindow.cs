using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {

	[GuiWindowOptions("ImGui/Style Selector")]
	internal class ImGuiStyleSelectorWindow : GuiWindow {

		public override void Render() {
			ImGui.Begin("Style Selector", ref open);
			ImGui.ShowStyleSelector("Style selector");
			ImGui.End();
		}

	}
}
