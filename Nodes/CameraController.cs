using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using ImGuiNET;

namespace ArcticFoxEngine.Nodes {
	public class CameraController : Node {

		internal override string nodeIconPath => ".res/NodeIcons/Camera.png";
		internal override string nodeIconPath32 => ".res/NodeIcons/Camera32.png";

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

		public CameraController() {
			name = "Camera Controller";

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

			Enable();
		}



		public override void Update() {

			if (ImGui.GetIO().WantTextInput == true) { return; }

			#region Camera Controls

			if (moveForward.GetButton() == true) {
				transform.localPosition += transform.Forward * 0.02f * speed;
			}
			if (moveRight.GetButton() == true) {
				transform.localPosition += transform.Right * 0.02f * speed;
			}
			if (moveBack.GetButton() == true) {
				transform.localPosition += transform.Back * 0.02f * speed;
			}
			if (moveLeft.GetButton() == true) {
				transform.localPosition += transform.Left * 0.02f * speed;
			}

			if (moveUp.GetButton() == true) {
				transform.localPosition += transform.Up * 0.02f * speed;
			}
			if (moveDown.GetButton() == true) {
				transform.localPosition += transform.Down * 0.02f * speed;
			}

			if (rollRight.GetButton() == true) {
				transform.localRotation *= Quaternion.RotationYawPitchRoll(0f, 0f, -0.01f);
			}
			if (rollLeft.GetButton() == true) {
				transform.localRotation *= Quaternion.RotationYawPitchRoll(0f, 0f, 0.01f);
			}
			if (lookHold.GetButton() == true) {
				transform.localRotation *= Quaternion.RotationYawPitchRoll(lookVector.GetValue().x * 0.002f, lookVector.GetValue().y * 0.002f, 0f);
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
