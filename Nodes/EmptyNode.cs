using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Nodes {
	public class EmptyNode : Node {

		internal override string debugName => "BaseNode";
		internal override string nodeIconPath => ".res/NodeIcons/EmptyNode.png";

		public EmptyNode() : base() {

			SetName("Empty");
			Enable();
		}

	}
}
