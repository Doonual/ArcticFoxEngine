using ArcticFoxEngine.Input.Devices;
using CoolClassLibrary;
using SharpDX.DirectInput;

namespace ArcticFoxEngine.Input.Bindings {
	public class MouseAxisInput : AxisBinding {

		MouseAxis axis;

		public enum MouseAxis {

			x = 0,
			y = 4

		}
		public MouseAxisInput(MouseAxis axis) {
			MouseInputDevice.Init();
			MouseInputDevice.deviceUpdate.Add(MouseUpdate);
			this.axis = axis;

		}

		private void MouseUpdate(MouseUpdate args) {

			if (((int)axis) == ((int)args.Offset)) {
				axisActive += args.Value;
			}

		}

		protected override void NextFrame() {
			axisActive = 0f;
		}
	}
}
