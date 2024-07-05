using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using CoolClassLibrary;

namespace ArcticFoxEngine {
	public static class MainApp {

		public static void RunTest() {

#if DEBUG
			Log.Init("Arctic Fox Engine", "Doonual", DateTime.Now);
#endif

			Log.Info("Starting Engine");
			Engine.setup = Setup;
			Engine.update = Update;
			Engine.Run(1280, 720);

		}

		static ButtonBinding panButton;
		static Axis2DBinding panVector;

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

			panButton = new MouseButtonInput(MouseButtonInput.MouseButton.Left);
			panVector = new GenericAxis2DInput(new MouseAxisInput(MouseAxisInput.MouseAxis.x), new MouseAxisInput(MouseAxisInput.MouseAxis.y));

		}

		static Vector2 mouseVector;

		private static void Update() {

			if (panButton.GetButton() == true) {
				mouseVector = panVector.GetValue() * 0.002f;
			}


			mouseVector *= 0.985f;

			Quaternion rotateDelta = Quaternion.RotationYawPitchRoll(mouseVector.x, mouseVector.y, 0f);
			mainCamera.transform.rotation *= rotateDelta;
			mainCamera.transform.position = mainCamera.transform.Back * 3f;

			Command.ExecuteMainRender(mainCamera, mainGeometry);

		}


	}
}
