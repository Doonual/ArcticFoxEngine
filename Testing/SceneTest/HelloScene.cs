using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticFoxEngine.Components;
using ArcticFoxEngine.Debug.Commands;
using CoolClassLibrary;

namespace ArcticFoxEngine.Testing.SceneTest {
	public class HelloScene {

		public static void RunTest() {

			Log.Info("Starting Engine");
			CommandController.Init(new List<Command>() {
				new HelpCommand(),
				new AddObjectCommand(),
			});

			Engine.init = Init;
			Engine.Run(1920, 1080);

		}

		public static void Init() {

			Scene mainScene = new Scene();
			mainScene.SetActiveScene();

			

			GameObject cameraObj = mainScene.InstantiateObject("Camera");
			Camera mainCam = cameraObj.AddComponent<Camera>();
			cameraObj.AddComponent<CameraController>();
			cameraObj.AddComponent<CubeSpin>();
			cameraObj.transform.position = Vector3.Back * 12f;


			


		}

	}
}
