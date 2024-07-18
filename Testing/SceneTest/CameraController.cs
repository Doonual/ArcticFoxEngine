using ArcticFoxEngine.Debug;
using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Testing.SceneTest {
	public class CameraController : Component {

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
		static ButtonBinding exitButton;

		#endregion

		Camera mainCamera;

		public override void Start() {
			mainCamera = gameObject.GetComponent<Camera>();

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
			exitButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.Escape);

		}

		public override void Update() {

			#region Camera Controls

			if (moveForward.GetButton() == true) {
				gameObject.transform.position += gameObject.transform.Forward * 0.02f;
			}
			if (moveRight.GetButton() == true) {
				gameObject.transform.position += gameObject.transform.Right * 0.02f;
			}
			if (moveBack.GetButton() == true) {
				gameObject.transform.position += gameObject.transform.Back * 0.02f;
			}
			if (moveLeft.GetButton() == true) {
				gameObject.transform.position += gameObject.transform.Left * 0.02f;
			}

			if (moveUp.GetButton() == true) {
				gameObject.transform.position += gameObject.transform.Up * 0.02f;
			}
			if (moveDown.GetButton() == true) {
				gameObject.transform.position += gameObject.transform.Down * 0.02f;
			}

			if (rollRight.GetButton() == true) {
				gameObject.transform.rotation *= Quaternion.RotationYawPitchRoll(0f, 0f, -0.01f);
			}
			if (rollLeft.GetButton() == true) {
				gameObject.transform.rotation *= Quaternion.RotationYawPitchRoll(0f, 0f, 0.01f);
			}
			if (lookHold.GetButton() == true) {
				gameObject.transform.rotation *= Quaternion.RotationYawPitchRoll(lookVector.GetValue().x * 0.002f, lookVector.GetValue().y * 0.002f, 0f);
			}


			#endregion
			if (toggleDebug.GetButtonDown() == true) {
				if (DebugManager.isOpen == true) {
					DebugManager.CloseGUI();
				}
				else {
					DebugManager.OpenGUI();
				}

			}
			if (exitButton.GetButton() == true) {
				Engine.Stop();
			}


		}


	}
}
