using ArcticFoxEngine.Gui.Builtin_Windows;
using ArcticFoxEngine.ImGuiIntegration;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using SixLabors.ImageSharp.PixelFormats;
using System.Windows.Forms;

namespace ArcticFoxEngine.Gui {

	[GuiWindowOptions("Scene")]
	public class SceneWindow : GuiWindow {

		internal static Node selectedNode = null;
		internal static NodeInspectorGui selectedNodeGui;

		public override void Render() {

			ImGui.Begin("Scene", ref open);
			RenderNodeTree();
			ImGui.End();
			
		}

		private void RenderNodeTree() {

			// Draw node tree
			ImGui.SeparatorText("Node tree");

			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0f, 0f));
			ImGui.BeginChild("Node tree view child", new Vector2(-1f, 400f), true);


			ImGuiTableFlags tableFlags = ImGuiTableFlags.RowBg | ImGuiTableFlags.NoBordersInBody;
			if (ImGui.BeginTable("Node tree view table", 1, tableFlags) == true) {

				
				ImGui.TableSetupColumn("Node");
				//ImGui.TableSetupColumn("Enabled");

				if (Node.rootNode != null) {
					ImGui.PushID(Node.rootNode.GetHashCode() + "draw node tree");
					Node childSelected = Node.rootNode.DrawNodeTreeGui(true);
					if (childSelected != null) {
						selectedNodeGui = new NodeInspectorGui(childSelected);
						selectedNode = childSelected;
					}
					ImGui.PopID();
				}
				ImGui.EndTable();
			}

			ImGui.EndChild();
			ImGui.PopStyleVar();

			// Draw node inspector
			ImGui.SeparatorText("Node inspector");
			if (selectedNodeGui != null) {
				ImGui.PushID(selectedNode.GetHashCode() + " draw event");
				selectedNodeGui.DrawNodeInspector(false);
				ImGui.PopID();
			}
			else {
				ImGui.Text("Select a node to insepct it");
			}

		}

		
	}
}
