using ArcticFoxEngine.Gui;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class ImGuiStyleSelectorWindow : GuiWindow {

		public override string name => "Style Selector";

		public ImGuiStyleSelectorWindow(params string[] menuGroups) : base(menuGroups) { }

		public override void Render() {
			ImGui.Begin("Style Selector", ref open);
			ImGui.ShowStyleSelector("Style selector");
			ImGui.End();
		}
	}
}
