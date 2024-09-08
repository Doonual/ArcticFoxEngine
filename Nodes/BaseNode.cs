using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Nodes {
	public class BaseNode : Node {

		internal override string nodeIconPath => ".res/NodeIcons/BaseNode.png";

		public BaseNode() {

			Enable();
		}

	}
}
