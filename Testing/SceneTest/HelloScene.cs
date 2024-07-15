using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticFoxEngine.Components;
using CoolClassLibrary;

namespace ArcticFoxEngine.Testing.SceneTest {
	public class HelloScene {

		public static void RunTest() {

			Log.Info("Starting Engine");
			Engine.init = Init;
			Engine.Run(1280, 720);

		}

		public static void Init() {

			Scene mainScene = new Scene();

			GameObject mainObj = new GameObject("Camera");
			mainObj.AddComponent(new Camera(95f, Camera.ProjectionType.Perspective));
			mainObj.AddComponent(new CameraController());
			mainScene.Instantiate(mainObj);
			mainObj.transform.position = Vector3.Back * 3f;

			GameObject cube = new GameObject("Cube");
			MeshFilter cubeMesh = new MeshFilter();
			
			cube.AddComponent(cubeMesh);
			mainScene.Instantiate(cube);
			cubeMesh.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			mainScene.SetActiveScene();

			


		}

	}
}
