using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input.Devices {
	internal static class MouseInputDevice {

		private static bool initialised = false;
		internal static List<Action<MouseUpdate>> deviceUpdate;
		internal static Mouse mouse;

		internal static void Init() {
			if (initialised == true) { return; }
			initialised = true;
			deviceUpdate = new List<Action<MouseUpdate>>();

			InputManager.AddInputDevice(UpdateDevice);

			mouse = new Mouse(InputManager.directInput);
			mouse.Properties.BufferSize = 128;
			mouse.Acquire();

		}

		private static void UpdateDevice() {
			MouseUpdate[] updates = mouse.GetBufferedData();
			for (int i = 0; i < updates.Length; i++) {
				for (int n = 0; n < deviceUpdate.Count; n++) {
					deviceUpdate[n](updates[i]);
				}
			}
		}

	}
}
