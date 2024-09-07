using ArcticFoxEngine.Nodes;
using ArcticFoxEngine.Testing;
using ArcticFoxEngine.Testing.SceneTest;
using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Demos.RenderingStressTest {
	public class RenderingStressTestNode : Node {
		

		public RenderingStressTestNode() : base() {

			int numObjectPerDim = 12;

			Log.Info("Testing scene with " + (numObjectPerDim * numObjectPerDim * numObjectPerDim) + " cubes");

			int currentObject = 0;
			
			CreateChild<CameraController>();

			Node cubeStack = CreateChild<EmptyNode>("Cube Stack");
			cubeStack.CreateChild<Transform>();
			cubeStack.transformChild.position = new Vector3(0f, 0f, 30f);

			float maxDim = (numObjectPerDim - 1) * 2f + 1f;

			
			for (int x = 0; x < numObjectPerDim; x ++) {
				for (int y = 0; y < numObjectPerDim; y++) {
					for (int z = 0; z < numObjectPerDim; z++) {
						Node newObj = cubeStack.CreateChild<EmptyNode>("Object #" + currentObject);
						newObj.CreateChild<Transform>();
						newObj.transformChild.position = new Vector3(x * 2f - maxDim / 2f + 0.5f, y * 2f - maxDim / 2f + 0.5f, z * 2f - maxDim / 2f + 0.5f);
						newObj.CreateChild<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
						currentObject++;
					}
				}
			}


			SetName("Rendering Stress Test");
			Enable();

		}


	}
}
