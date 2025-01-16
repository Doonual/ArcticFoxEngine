using ArcticFoxEngine.ImGuiIntegration;
using ImGuiNET;
using SharpDX.DirectInput;

namespace ArcticFoxEngine.Input.Devices {
	internal static class MouseInputDevice {

		private static bool initialised = false;
		internal static List<Action<MouseUpdate>> deviceUpdate;
		internal static Mouse mouse;

		internal static void Init() {
			if (initialised == true) { return; }

			deviceUpdate = new List<Action<MouseUpdate>>();

			InputManager.AddInputDevice(UpdateDevice);

			mouse = new Mouse(InputManager.directInput);
			mouse.Properties.BufferSize = 128;
			mouse.Acquire();

			initialised = true;
		}

		private static void UpdateDevice() {
			if (initialised == false) { return; }

			MouseUpdate[] updates = mouse.GetBufferedData();
			for (int i = 0; i < updates.Length; i++) {
				ImGuiInput.UpdateMouse(updates[i]);
				if (InputManager.imGuiBlocksInput == true && ImGui.GetIO().WantCaptureKeyboard == true) { continue; }
				for (int n = 0; n < deviceUpdate.Count; n++) {
					deviceUpdate[n](updates[i]);
				}

			}
		}

	}
}
