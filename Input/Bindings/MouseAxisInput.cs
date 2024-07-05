using ArcticFoxEngine.Input.Devices;
using CoolClassLibrary;
using SharpDX.RawInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input.Bindings {
	public class MouseAxisInput : AxisBinding {

		MouseAxis axis;

		public enum MouseAxis {

			x,
			y

		}
		public MouseAxisInput(MouseAxis axis) {
			MouseInputDevice.Init();
			MouseInputDevice.deviceUpdate.Add(MouseUpdate);
			this.axis = axis;

		}

		private void MouseUpdate(MouseInputEventArgs args) {

			if (axis == MouseAxis.x) {
				axisActive += args.X;
			}
			if (axis == MouseAxis.y) {
				axisActive += args.Y;
			}

		}

		protected override void NextFrame() {
			axisActive = 0f;
		}
	}
}
