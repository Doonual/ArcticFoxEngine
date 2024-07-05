using CoolClassLibrary;

namespace ArcticFoxEngine {
	public static class Test {

		public static void RunTest() {

#if DEBUG
			Log.Init("Arctic Fox Engine", "Doonual", DateTime.Now);
#endif

			Log.Info("Starting Engine");

			Engine.setup = Setup;
			Engine.update = Update;
			Engine.Run(1280, 720);

		}

		static Camera mainCamera;
		static GeometryInfo mainGeometry;

		private static void Setup() {

			mainCamera = new Camera(95f, Camera.ProjectionType.Perspective);

			mainCamera.transform.position = mainCamera.transform.Back * 3f;

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
			mainGeometry = new GeometryInfo(vertexData, indexData);


		}

		static Vector2 mouseVector;

		private static void Update() {


			if (Input.GetMouseButton() == true) {
				mouseVector.x = Input.GetMouseVector().x * 0.001f;
				mouseVector.y = Input.GetMouseVector().y * 0.001f;
			}
			

			mouseVector *= 0.985f;

			Quaternion rotateDelta = Quaternion.RotationYawPitchRoll(mouseVector.x, mouseVector.y, 0f);
			mainCamera.transform.rotation *= rotateDelta;
			mainCamera.transform.position = mainCamera.transform.Back * 3f;

			Command.ExecuteMainRender(mainCamera, mainGeometry);

		}


	}
}
