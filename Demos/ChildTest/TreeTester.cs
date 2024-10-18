

using ArcticFoxEngine.Nodes;
using CoolClassLibrary;

namespace ArcticFoxEngine.Demos.ChildTest {
	public class TreeTester : Node {

		public TreeTester() {
			Enable();
		}

		public void GenerateTree(int power) {

			if (power <= 0) { return; }

			int children = MathUtil.RandomInt(1, (int)MathF.Ceiling((float)power / 1.3f));
			for (int i = 0; i < children; i ++) {
				TreeTester child = CreateChild<TreeTester>();
				child.GenerateTree(power - children);
			}

		}

	}
}
