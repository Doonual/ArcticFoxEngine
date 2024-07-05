using SharpDX.RawInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input.Devices {
	internal static class MouseInputDevice {

		private static bool initialised = false;
		internal static List<Action<MouseInputEventArgs>> deviceUpdate;

		internal static void Init() {
			if (initialised == true) { return; }
			initialised = true;

			deviceUpdate = new List<Action<MouseInputEventArgs>>();

			Device.RegisterDevice(SharpDX.Multimedia.UsagePage.Generic, SharpDX.Multimedia.UsageId.GenericMouse, DeviceFlags.None);
			Device.MouseInput += Device_MouseInput;

		}

		private static void Device_MouseInput(object sender, MouseInputEventArgs e) {
			for (int i = 0; i < deviceUpdate.Count; i ++) {
				deviceUpdate[i](e);
			}
		}
	}
}
