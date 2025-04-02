using ArcticFoxEngine.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Gui.Builtin_Windows {

	internal class NodeInspectorWindow : GuiWindow {

		NodeInspectorGui nodeInspector;

		public NodeInspectorWindow(Node targetNode) {

			nodeInspector = new NodeInspectorGui(targetNode);

		}


		public override void Render() {
			nodeInspector.DrawNodeInspector(true);
		}

	}

}
