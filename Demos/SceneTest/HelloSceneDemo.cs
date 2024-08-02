using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticFoxEngine.Components;
using ArcticFoxEngine.Debug.Commands;
using CoolClassLibrary;

namespace ArcticFoxEngine.Testing.SceneTest {
	public class HelloSceneDemo : DemoScene {

		Scene mainScene;

		internal override string name => "Hello Scene";

		internal override Scene LoadScene() {

			mainScene = new Scene();
			Scene.LoadScene(mainScene);

			GameObject cameraObj = mainScene.InstantiateObject("Camera");
			Camera mainCam = cameraObj.AddComponent<Camera>();
			cameraObj.AddComponent<CameraController>();
			cameraObj.transform.position = Vector3.Back * 25f;

			GameObject cubeSpinMaster = mainScene.InstantiateObject("Cube Spin");
			cubeSpinMaster.AddComponent<CubeSpin>();

			return mainScene;

		}

		internal override void UnloadScene() {
			mainScene.Dispose();
		}
	}
}
