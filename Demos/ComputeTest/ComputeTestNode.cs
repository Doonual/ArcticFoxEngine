using ArcticFoxEngine.Compute;
using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using ArcticFoxEngine.Nodes;
using ArcticFoxEngine.Render;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Demos.ComputeTest {
	public class ComputeTestNode : Node {



		public ComputeTestNode() {

			ComputeBufferAgentsNode exampleNode = new ComputeBufferAgentsNode();
			exampleNode.SetParent(this);

		}

		public override void Render() {


		}

		public override void Update() {


		}

		public override void DrawInspector() {

		}

		

	}
}
