using ArcticFoxEngine.Input.Devices;
using CoolClassLibrary;
using SharpDX.RawInput;
using Swan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input.Bindings {
	internal class ControllerButtonInput : ButtonBinding {
		
		public ControllerButtonInput() {
			ControllerInputDevice.Init();

			ControllerInputDevice.deviceUpdate.Add(DeviceData);
		}

		private void DeviceData(RawInputEventArgs args) {
			Log.Info("Data: " + args.ToJson());
		}

		protected override void NextFrame() {

		}
	}
}
