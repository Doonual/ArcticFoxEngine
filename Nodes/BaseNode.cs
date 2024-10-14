namespace ArcticFoxEngine.Nodes {
	public class BaseNode : Node {

		internal override string nodeIconPath => ".res/NodeIcons/BaseNode.png";
		internal override string nodeIconPath32 => ".res/NodeIcons/BaseNode32.png";

		public BaseNode() {

			Enable();
		}

	}
}
