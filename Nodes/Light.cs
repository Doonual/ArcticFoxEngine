
namespace ArcticFoxEngine.Nodes {

	public class Light : Node {

		internal override string nodeIconPath => ".res/NodeIcons/Light.png";

		public enum Type {
			point,
			directional
		}
		public Type lightType;
		public Color colour;

		public Light() {
			name = "Light";

			lightType = Type.point;
			colour = new Color(255, 255, 255);

			Enable();
		}

	}
}
