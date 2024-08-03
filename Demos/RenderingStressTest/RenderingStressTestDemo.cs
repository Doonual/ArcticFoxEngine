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

			int numObjectPerDim = 8;
			int currentObject = 0;
			
			mainScene = new Scene();
			GameObject cameraObj = mainScene.InstantiateObject("Camera");
			cameraObj.AddComponent<Camera>();
			cameraObj.AddComponent<CameraController>();

			Log.Info("Testing scene with " + (numObjectPerDim * numObjectPerDim * numObjectPerDim) + "cubes");
			for (int x = 0; x < numObjectPerDim; x ++) {
				for (int y = 0; y < numObjectPerDim; y++) {
					for (int z = 0; z < numObjectPerDim; z++) {
						GameObject newObj = mainScene.InstantiateObject("Object #" + currentObject);
						newObj.transform.position = new Vector3(x * 2f, y * 2f, z * 2f);
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
