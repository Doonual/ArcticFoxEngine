using ArcticFoxEngine.Debug;
using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Testing.SceneTest {
	public class CameraController : Node {

		internal override string nodeIconPath => ".res/NodeIcons/Camera.png";

		#region Input Bindings

		ButtonBinding moveForward;
		ButtonBinding moveRight;
		ButtonBinding moveBack;
		ButtonBinding moveLeft;
		ButtonBinding moveUp;
		ButtonBinding moveDown;
		ButtonBinding rollRight;
		ButtonBinding rollLeft;
		Axis2DBinding lookVector;
		ButtonBinding lookHold;

		ButtonBinding increaseSpeed;
		ButtonBinding decreaseSpeed;

		#endregion
		float speed;

		Transform tf;

		public CameraController() {

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

			increaseSpeed = new MouseButtonInput(MouseButtonInput.MouseButton.WheelUp);
			decreaseSpeed = new MouseButtonInput(MouseButtonInput.MouseButton.WheelDown);

			speed = 1f;


			tf = CreateChild<Transform>();
			CreateChild<Camera>();

			SetName("Camera Controller");
			Enable();

		}



		public override void Update() {
			#region Camera Controls

			if (moveForward.GetButton() == true) {
				tf.position += tf.Forward * 0.02f * speed;
			}
			if (moveRight.GetButton() == true) {
				tf.position += tf.Right * 0.02f * speed;
			}
			if (moveBack.GetButton() == true) {
				tf.position += tf.Back * 0.02f * speed;
			}
			if (moveLeft.GetButton() == true) {
				tf.position += tf.Left * 0.02f * speed;
			}

			if (moveUp.GetButton() == true) {
				tf.position += tf.Up * 0.02f * speed;
			}
			if (moveDown.GetButton() == true) {
				tf.position += tf.Down * 0.02f * speed;
			}

			if (rollRight.GetButton() == true) {
				tf.rotation *= Quaternion.RotationYawPitchRoll(0f, 0f, -0.01f);
			}
			if (rollLeft.GetButton() == true) {
				tf.rotation *= Quaternion.RotationYawPitchRoll(0f, 0f, 0.01f);
			}
			if (lookHold.GetButton() == true) {
				tf.rotation *= Quaternion.RotationYawPitchRoll(lookVector.GetValue().x * 0.002f, lookVector.GetValue().y * 0.002f, 0f);
			}

			if (increaseSpeed.GetButton() == true) {
				speed *= 1.2f;
			}
			if (decreaseSpeed.GetButton() == true) {
				speed /= 1.2f;
			}

			#endregion
		}
		
		public override void Debug() {
			base.Debug();
			ImGui.SliderFloat("Speed", ref speed, 0f, 1000f, null, ImGuiSliderFlags.Logarithmic);
		}

	}
}
