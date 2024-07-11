using ArcticFoxEngine.Debug;
using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using ArcticFoxEngine.Input.Devices;
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

		#region Input Bindings

		static ButtonBinding moveForward;
		static ButtonBinding moveRight;
		static ButtonBinding moveBack;
		static ButtonBinding moveLeft;
		static ButtonBinding moveUp;
		static ButtonBinding moveDown;
		static ButtonBinding rollRight;
		static ButtonBinding rollLeft;
		static Axis2DBinding lookVector;
		static ButtonBinding lookHold;

		static ButtonBinding toggleDebug;

		#endregion

		static Camera mainCamera;
		static GeometryInfo mainGeometry;

		private static void Setup() {

			mainCamera = new Camera(95f, Camera.ProjectionType.Perspective);
			mainCamera.transform.position = Vector3.Back * 2f;

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

			#region Input Bindings

			moveForward = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.W);
			moveRight = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.D);
			moveBack = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.S);
			moveLeft = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.A);
			moveUp = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.Space);
			moveDown = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.C);
			rollRight = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.E);
			rollLeft = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.Q);
			lookVector = new GenericAxis2DInput(new MouseAxisInput(MouseAxisInput.MouseAxis.x), new MouseAxisInput(MouseAxisInput.MouseAxis.y));
			lookHold = new MouseButtonInput(MouseButtonInput.MouseButton.Right);

			toggleDebug = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.F1);

			#endregion

		}


		private static void Update() {

			#region Camera Controls

			if (moveForward.GetButton() == true) {
				mainCamera.transform.position += mainCamera.transform.Forward * 0.02f;
			}
			if (moveRight.GetButton() == true) {
				mainCamera.transform.position += mainCamera.transform.Right * 0.02f;
			}
			if (moveBack.GetButton() == true) {
				mainCamera.transform.position += mainCamera.transform.Back * 0.02f;
			}
			if (moveLeft.GetButton() == true) {
				mainCamera.transform.position += mainCamera.transform.Left * 0.02f;
			}

			if (moveUp.GetButton() == true) {
				mainCamera.transform.position += mainCamera.transform.Up * 0.02f;
			}
			if (moveDown.GetButton() == true) {
				mainCamera.transform.position += mainCamera.transform.Down * 0.02f;
			}

			if (rollRight.GetButton() == true) {
				mainCamera.transform.rotation *= Quaternion.RotationYawPitchRoll(0f, 0f, -0.01f);
			}
			if (rollLeft.GetButton() == true) {
				mainCamera.transform.rotation *= Quaternion.RotationYawPitchRoll(0f, 0f, 0.01f);
			}
			if (lookHold.GetButton() == true) {
				mainCamera.transform.rotation *= Quaternion.RotationYawPitchRoll(lookVector.GetValue().x * 0.002f, lookVector.GetValue().y * 0.002f, 0f);
			}
			

			#endregion
			if (toggleDebug.GetButtonDown() == true) {
				if (DebugManager.isOpen == true) {
					DebugManager.Close();
				}
				else {
					DebugManager.Open(mainCamera);
				}

			}

			Command.ExecuteMainRender(mainCamera, mainGeometry);

		}


	}
}
