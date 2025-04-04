using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Gui.Builtin_Windows {

	internal class NodeInspectorWindow : GuiWindow {

		NodeInspectorGui nodeInspector;
		Node targetNode;

		public NodeInspectorWindow(Node targetNode) {

			this.targetNode = targetNode;
			nodeInspector = new NodeInspectorGui(targetNode);

		}


		public override void Render() {
			ImGui.Begin(targetNode.name + " (" + targetNode.GetHashCode() + ") - Inspector##" + GetHashCode(), ref open);
			nodeInspector.DrawNodeInspector(true);
			ImGui.End();
		}

	}

}
