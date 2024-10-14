using ArcticFoxEngine.Nodes;
using CoolClassLibrary;

namespace ArcticFoxEngine.Demos.RenderingStressTest {
	public class RenderingStressTestNode : Node {

		public RenderingStressTestNode() {
			name = "Rendering Stress Test";

			int numObjectPerDim = 12;

			Log.Info("Testing scene with " + (numObjectPerDim * numObjectPerDim * numObjectPerDim) + " cubes");

			int currentObject = 0;

			CreateChild<CameraController>();

			Node cubeStack = CreateChild<BaseNode>("Cube Stack");
			cubeStack.transform.localPosition = new Vector3(0f, 0f, 30f);

			float maxDim = (numObjectPerDim - 1) * 2f + 1f;


			for (int x = 0; x < numObjectPerDim; x++) {
				for (int y = 0; y < numObjectPerDim; y++) {
					for (int z = 0; z < numObjectPerDim; z++) {
						Node newObj = cubeStack.CreateChild<BaseNode>("Object #" + currentObject);
						newObj.transform.localPosition = new Vector3(x * 2f - maxDim / 2f + 0.5f, y * 2f - maxDim / 2f + 0.5f, z * 2f - maxDim / 2f + 0.5f);
						newObj.CreateChild<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
						currentObject++;
					}
				}
			}

			Enable();
		}


	}
}
