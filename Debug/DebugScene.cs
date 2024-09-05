using ImGuiNET;
using ArcticFoxEngine;
using ArcticFoxEngine.Backend;
using SixLabors.ImageSharp.PixelFormats;

namespace ArcticFoxEngine.Debug {
	internal class DebugScene : DebugWindow {

		Node selectedNode = null;

		internal static IntPtr testTexId;

		public DebugScene() {

			testTexId = GPU_RenderImGui.CreateImageTexture(SixLabors.ImageSharp.Image.Load<Rgba32>(".res/Textures/tiger.png"), SharpDX.DXGI.Format.R8G8B8A8_UNorm);

		}

		internal override string name => "Scene";
		internal override void Render() {

			ImGuiTableFlags tableFlags = ImGuiTableFlags.RowBg | ImGuiTableFlags.NoBordersInBody;

			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0f, 0f));
			ImGui.BeginChild("Node tree view child", new Vector2(-1f, 400f), true);
			
			if (ImGui.BeginTable("Node tree view table", 2, tableFlags) == true) {

				ImGui.TableSetupColumn("Node", ImGuiTableColumnFlags.None, 16f);
				ImGui.TableSetupColumn("Enabled");

				if (Node.rootNode != null) {
					Node childSelected = Node.rootNode.DebugNodeTree();
					if (childSelected != null) {
						selectedNode = childSelected;
					}
				}

				ImGui.EndTable();


			}
		
			ImGui.EndChild();
			ImGui.PopStyleVar();

			if (selectedNode != null) {
				selectedNode.DebugEvent(true);
			}
			else {
				ImGui.Text("Select a node to insepct it");
			}
			
			

		}
	}
}
