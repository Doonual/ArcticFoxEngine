using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ArcticFoxEngine.Rendering;

namespace ArcticFoxEngine.Demos.RenderingStressTest {
	public class RenderingStressTestNode : Node {

		public RenderingStressTestNode() {
			name = "Rendering Stress Test";


			CreateChild<LightingSystem>();
			MeshRenderer skybox = CreateChild<MeshRenderer>("skybox");
			skybox.SetShader<SkyboxShader>();
			skybox.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Quad));
			for (int i = 0; i < skybox.mesh.vertices.Length; i++) {
				skybox.mesh.vertices[i].position.y = skybox.mesh.vertices[i].position.z;
				skybox.mesh.vertices[i].position.z = 0.5f;
			}

			int numObjectPerDim = 12;

			Log.Info("Testing scene with " + (numObjectPerDim * numObjectPerDim * numObjectPerDim) + " cubes");

			int currentObject = 0;

			CreateChild<CameraController>().CreateChild<Camera>();

			Node cubeStack = CreateChild<BaseNode>("Cube Stack");
			cubeStack.transform.localPosition = new Vector3(0f, 0f, 30f);

			float maxDim = (numObjectPerDim - 1) * 2f + 1f;

			

			for (int x = 0; x < numObjectPerDim; x++) {
				for (int y = 0; y < numObjectPerDim; y++) {
					for (int z = 0; z < numObjectPerDim; z++) {
						MeshRenderer newObj = cubeStack.CreateChild<MeshRenderer>("Object #" + currentObject);
						newObj.transform.localPosition = new Vector3(x * 2f - maxDim / 2f + 0.5f, y * 2f - maxDim / 2f + 0.5f, z * 2f - maxDim / 2f + 0.5f);
						newObj.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
						newObj.SetShader<LitShader>();
						currentObject++;
					}
				}
			}

			Enable();
		}


	}
}
