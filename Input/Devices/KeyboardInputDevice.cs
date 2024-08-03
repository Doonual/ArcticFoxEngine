using CoolClassLibrary;
using SharpDX.DirectInput;
using Swan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input.Devices {
	internal static class KeyboardInputDevice {

		static bool initialised = false;
		internal static Keyboard keyboard;
		internal static List<Action<KeyboardUpdate>> deviceUpdate;

		internal static void Init() {
			if (initialised == true) { return; }
			deviceUpdate = new List<Action<KeyboardUpdate>>();

			InputManager.AddInputDevice(UpdateDevice);

			keyboard = new Keyboard(InputManager.directInput);
			keyboard.Properties.BufferSize = 128;
			keyboard.Acquire();

			initialised = true;
		}

		private static void UpdateDevice() {
			if (initialised == false) { return; }
			KeyboardUpdate[] updates = keyboard.GetBufferedData();
			for (int i = 0; i < updates.Length; i ++) {
				for (int n = 0; n < deviceUpdate.Count; n ++) {
					deviceUpdate[n](updates[i]);
				}
			}

		}


	}
}
