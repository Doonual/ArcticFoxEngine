using ArcticFoxEngine.Components;
using ArcticFoxEngine.Debug.Commands;
using ArcticFoxEngine.Testing.SceneTest;
using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Testing.ChildTest {
	public static class ChildTest {

		public static void RunTest() {

			Log.Info("Starting Engine");
			CommandController.Init(new List<Command>() {
				new HelpCommand(),
				new AddObjectCommand(),
			});

			Engine.init = Init;
			Engine.Run(1920, 1080, "ArcticFox - ChildTest");

		}

		public static void Init() {

			Scene mainScene = new Scene();
			Scene.LoadScene(mainScene);

			GameObject cameraObj = GameObject.Instantiate("Camera");
			cameraObj.AddComponent<Camera>();
			cameraObj.AddComponent<CameraController>();
			cameraObj.transform.position = new Vector3(0f, 2f, -15f);

			#region Block tower

			GameObject blockPlatform = GameObject.Instantiate("Block tower platform");
			blockPlatform.transform.position = new Vector3(0f, -0.5f, 0f);
			blockPlatform.transform.scale = new Vector3(5f, 1f, 5f);
			blockPlatform.AddComponent<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));

			GameObject blockTowerA = GameObject.Instantiate("Block Tower A");
			blockTowerA.transform.position = new Vector3(0f, 0.5f, 0f);
			blockTowerA.AddComponent<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));

			GameObject blockTowerB = blockTowerA.InstantiateChild("Block Tower B");
			blockTowerB.AddComponent<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			blockTowerB.transform.position = new Vector3(0f, 2f, 0f);

			GameObject blockTowerC = blockTowerB.InstantiateChild("Block Tower C");
			blockTowerC.AddComponent<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			blockTowerC.transform.position = new Vector3(0f, 2f, 0f);

			#endregion
			#region Cube Cube

			GameObject cubePlatform = GameObject.Instantiate("Cube cube platform");
			cubePlatform.transform.position = new Vector3(-7f, -0.5f, 1f);
			cubePlatform.transform.scale = new Vector3(7f, 1f, 7f);
			cubePlatform.AddComponent<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));

			GameObject cubeParent = GameObject.Instantiate("Cube Cube");
			cubeParent.transform.position = new Vector3(-7f, 4f, 1f);
			Vector3[] positions = new Vector3[] {
				new Vector3(-2f, -2f, -2f),
				new Vector3(2f, -2f, -2f),
				new Vector3(-2f, 2f, -2f),
				new Vector3(2f, 2f, -2f),
				new Vector3(-2f, -2f, 2f),
				new Vector3(2f, -2f, 2f),
				new Vector3(-2f, 2f, 2f),
				new Vector3(2f, 2f, 2f),
			};
			for (int i = 0; i < positions.Length; i ++) {
				GameObject cubeObj = GameObject.Instantiate("Cube #" + i, cubeParent);
				cubeObj.transform.position = positions[i];
				cubeObj.AddComponent<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			}
			cubeParent.AddComponent<CubeRoller>();

			#endregion
			#region Cube Wheel

			float cubeWheelSize = 8f;

			GameObject cubeWheelPlatform = GameObject.Instantiate("Cube wheel platform");
			cubeWheelPlatform.transform.position = new Vector3(cubeWheelSize / 2f + 3.5f, -0.5f, (cubeWheelSize / 2f) - 2.5f);
			cubeWheelPlatform.transform.scale = new Vector3(cubeWheelSize, 1f, cubeWheelSize);
			cubeWheelPlatform.AddComponent<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));

			GameObject cubeWheel = GameObject.Instantiate("Cube Wheel");
			cubeWheel.transform.position = new Vector3(cubeWheelSize / 2f + 3.5f, -0.5f, (cubeWheelSize / 2f) - 2.5f);
			GameObject originalWheel = cubeWheel.InstantiateChild("Original wheel");
			originalWheel.AddComponent<MeshRenderer>();
			CubeWheel cubeWheelComp = originalWheel.AddComponent<CubeWheel>();
			cubeWheelComp.Stop();
			cubeWheelComp.Propagate(5, 8f);
			

			#endregion


		}

	}
}
