using ArcticFoxEngine.Components;
using ArcticFoxEngine.Testing;
using ArcticFoxEngine.Testing.SceneTest;
using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Demos.RenderingStressTest {
	public class RenderingStressTestDemo : DemoScene {
		
		internal override string name => "Rendering Stress Test";
		Scene mainScene;

		internal override Scene LoadScene() {

			


			int numObjectPerDim = 12;

			Log.Info("Testing scene with " + (numObjectPerDim * numObjectPerDim * numObjectPerDim) + " cubes");

			int currentObject = 0;
			
			mainScene = new Scene();
			GameObject cameraObj = mainScene.InstantiateObject("Camera");
			cameraObj.AddComponent<Camera>();
			cameraObj.AddComponent<CameraController>();

			GameObject cubeStack = mainScene.InstantiateObject("Cube Stack");
			cubeStack.transform.position = new Vector3(0f, 0f, 30f);

			float maxDim = (numObjectPerDim - 1) * 2f + 1f;

			
			for (int x = 0; x < numObjectPerDim; x ++) {
				for (int y = 0; y < numObjectPerDim; y++) {
					for (int z = 0; z < numObjectPerDim; z++) {
						GameObject newObj = cubeStack.InstantiateChild("Object #" + currentObject);
						newObj.transform.position = new Vector3(x * 2f - maxDim / 2f + 0.5f, y * 2f - maxDim / 2f + 0.5f, z * 2f - maxDim / 2f + 0.5f);
						newObj.AddComponent<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
						currentObject++;
					}
				}
			}



			return mainScene;

		}

		internal override void UnloadScene() {
			mainScene.Dispose();
		}
	}
}
