using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Nodes {
	public class Cube : Node {

		internal override string nodeIconPath => ".res/NodeIcons/BaseNode.png";

		public Cube() {
			name = "Cube";

			CreateChild<Transform>();
			CreateChild<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));

			Enable();
		}

	}
}
