using ImGuiNET;

namespace ArcticFoxEngine.Debug {
	internal class DebugRender : DebugWindow {

		internal static bool updateObjectInfo = true;

		internal override string name => "Render";

		internal override void Render() {

			ImGui.Checkbox("Update Object Info", ref updateObjectInfo);
			

		}
	}
}
