using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			Vertex[] vertexData = new Vertex[] {
				new Vertex() {Position=new Vector3(-1f, -1f, -1f), Color = new Color(0.0f, 0.0f, 0.0f)},
				new Vertex() {Position=new Vector3(1f, -1f, -1f), Color = new Color(1.0f, 0.0f, 0.0f)},
				new Vertex() {Position=new Vector3(-1f, 1f, -1f), Color = new Color(0.0f, 1.0f, 0.0f)},
				new Vertex() {Position=new Vector3(1f, 1f, -1f), Color = new Color(1.0f, 1.0f, 0.0f)},
				new Vertex() {Position=new Vector3(-1f, -1f, 1f), Color = new Color(0.0f, 0.0f, 1.0f)},
				new Vertex() {Position=new Vector3(1f, -1f, 1f), Color = new Color(1.0f, 0.0f, 1.0f)},
				new Vertex() {Position=new Vector3(-1f, 1f, 1f), Color = new Color(0.0f, 1.0f, 1.0f)},
				new Vertex() {Position=new Vector3(1f, 1f, 1f), Color = new Color(1.0f, 1.0f, 1.0f)},
			};
			int[] indexData = new int[] {
				// Z+ Face
				0, 2, 1,
				2, 3, 1,
				4, 6, 0,
				6, 2, 0,
				5, 7, 4,
				7, 6, 4,
				1, 3, 5,
				3, 7, 5,
				2, 6, 3,
				6, 7, 3,
				0, 1, 5,
				5, 4, 0

			};
			mainScene.mainGeometry = new GeometryInfo(vertexData, indexData);

			GameObject mainObj = new GameObject("Camera");
			mainObj.AddComponent(new Camera(95f, Camera.ProjectionType.Perspective));
			mainObj.AddComponent(new CameraController());
			mainScene.Instantiate(mainObj);
			mainObj.transform.position = Vector3.Back * 3f;


			mainScene.SetActiveScene();

			


		}

	}
}
