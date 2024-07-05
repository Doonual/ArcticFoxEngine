using SharpDX.RawInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input.Devices {
	internal static class ControllerInputDevice {

		private static bool initialised = false;
		internal static List<Action<RawInputEventArgs>> deviceUpdate;

		internal static void Init() {
			if (initialised == true) { return; }
			initialised = true;

			deviceUpdate = new List<Action<RawInputEventArgs>>();

			Device.RegisterDevice(SharpDX.Multimedia.UsagePage.Generic, SharpDX.Multimedia.UsageId.GenericGamepad, DeviceFlags.None);
			Device.RawInput += Device_RawInput;

		}

		private static void Device_RawInput(object sender, RawInputEventArgs e) {
			for (int i = 0; i < deviceUpdate.Count; i++) {
				deviceUpdate[i](e);
			}
		}

	}
}
