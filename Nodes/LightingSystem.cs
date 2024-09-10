
namespace ArcticFoxEngine.Nodes {

	public class LightingSystem : Node {

		internal override string nodeIconPath => ".res/NodeIcons/LightManager.png";

		public LightingSystem() {
			name = "Lighting System";

			Enable();
		}

	}
}
