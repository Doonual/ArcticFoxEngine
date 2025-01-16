using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using CoolClassLibrary;
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
		bool lockRoll;

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
			lockRoll = true;

			Enable();
		}



		public override void Update() {

			if (ImGui.GetIO().WantTextInput == true) { return; }

			#region Translation

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

			if (increaseSpeed.GetButton() == true) {
				speed *= 1.2f;
			}
			if (decreaseSpeed.GetButton() == true) {
				speed /= 1.2f;
			}

			#endregion

			#region Rotation

			if (lockRoll == false) {

				if (rollRight.GetButton() == true) {
					transform.localRotation *= Quaternion.RotationYawPitchRoll(0f, 0f, -0.01f);
				}
				if (rollLeft.GetButton() == true) {
					transform.localRotation *= Quaternion.RotationYawPitchRoll(0f, 0f, 0.01f);
				}

				if (lookHold.GetButton() == true) {
					transform.localRotation *= Quaternion.RotationYawPitchRoll(lookVector.GetValue().x * 0.002f, lookVector.GetValue().y * 0.002f, 0f);
				}

			}

			if (lockRoll == true) {

				if (lookHold.GetButton() == true) {
					transform.localRotation = Quaternion.RotationAxis(Vector3.Up, lookVector.GetValue().x * 0.002f) * transform.localRotation;
					transform.localRotation = Quaternion.RotationAxis(transform.Right, lookVector.GetValue().y * 0.002f) * transform.localRotation;
				}

				Vector3 flatRightVector = transform.Right;
				flatRightVector.y = 0f;
				flatRightVector = flatRightVector.Normalize();

				Vector3 rotationAxis = Vector3.Cross(flatRightVector, transform.Right);
				transform.localRotation = Quaternion.RotationAxis(rotationAxis, -MathF.Asin(rotationAxis.Length())) * transform.localRotation;

			}

			#endregion

			
		}

		Vector3 rotationAxis = Vector3.Zero;
		public override void DrawInspector() {
			base.DrawInspector();
			ImGui.SliderFloat("Speed", ref speed, 0f, 1000f, "%.6f", ImGuiSliderFlags.Logarithmic);
			ImGui.Checkbox("Lock roll", ref lockRoll);


		}

	}
}
