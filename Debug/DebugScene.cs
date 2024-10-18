using ArcticFoxEngine.ImGuiIntegration;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using SixLabors.ImageSharp.PixelFormats;
using System.Windows.Forms;

namespace ArcticFoxEngine.Debug {
	internal class DebugScene : DebugWindow {

		internal static Node selectedNode = null;
		internal IntPtr testTexId;

		Camera sceneCam = null;

		public DebugScene() {

			testTexId = RenderImGui.CreateImageTexture(SixLabors.ImageSharp.Image.Load<Rgba32>(".res/Textures/tiger.png"), SharpDX.DXGI.Format.R8G8B8A8_UNorm);

		}


		internal override string name => "Scene";
		internal override void Render() {

			RenderNodeTree();
			
		}

		private void RenderNodeTree() {

			ImGui.SeparatorText("Node tree");
			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0f, 0f));
			ImGui.BeginChild("Node tree view child", new Vector2(-1f, 400f), true);
			ImGuiTableFlags tableFlags = ImGuiTableFlags.RowBg | ImGuiTableFlags.NoBordersInBody;
			if (ImGui.BeginTable("Node tree view table", 2, tableFlags) == true) {
				ImGui.TableSetupColumn("Node", ImGuiTableColumnFlags.None, 16f);
				ImGui.TableSetupColumn("Enabled");
				if (Node.rootNode != null) {
					ImGui.PushID(Node.rootNode.GetHashCode() + "debug node tree");
					Node childSelected = Node.rootNode.DebugNodeTree(true);
					if (childSelected != null) {
						selectedNode = childSelected;
					}
					ImGui.PopID();
				}
				ImGui.EndTable();
			}

			ImGui.EndChild();
			ImGui.PopStyleVar();


			ImGui.SeparatorText("Node inspector");
			if (selectedNode != null) {
				ImGui.PushID(selectedNode.GetHashCode() + " debug event");
				selectedNode.DebugEvent(true);
				ImGui.PopID();
			}
			else {
				ImGui.Text("Select a node to insepct it");
			}

		}

		
	}
}
