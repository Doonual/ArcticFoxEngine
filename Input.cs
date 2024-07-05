using CoolClassLibrary;
using SharpDX.RawInput;

namespace ArcticFoxEngine {
	public static class Input {

		// New mouse input is accumulated in the activeMouseVector vector
		// until the end of the frame. Then it is passed into currentMouseVector
		// then activeMouseVector is cleared
		private static Vector2 currentMouseVector;
		private static Vector2 activeMouseVector;
		private static bool mouseButtonLeft;

		internal static void InitInput() {

			Device.RegisterDevice(SharpDX.Multimedia.UsagePage.Generic, SharpDX.Multimedia.UsageId.GenericMouse, DeviceFlags.None);
			Device.MouseInput += Device_MouseInput;

			currentMouseVector = Vector2.zero;

		}

		public static Vector2 GetMouseVector() {
			return currentMouseVector;
		}
		public static bool GetMouseButton() {
			return mouseButtonLeft;
		}

		private static void Device_MouseInput(object sender, MouseInputEventArgs e) {
			activeMouseVector += new Vector2(e.X, e.Y);

			if (e.ButtonFlags == MouseButtonFlags.LeftButtonDown) {
				mouseButtonLeft = true;
			}
			if (e.ButtonFlags == MouseButtonFlags.LeftButtonUp) {
				mouseButtonLeft = false;
			}
			
		}

		internal static void NextFrame() {

			currentMouseVector = activeMouseVector;
			activeMouseVector = Vector2.zero;

		}

	}
}
