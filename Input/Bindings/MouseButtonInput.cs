using ArcticFoxEngine.Input.Devices;
using CoolClassLibrary;
using ImGuiNET;
using SharpDX.DirectInput;
using Swan;

namespace ArcticFoxEngine.Input.Bindings {
	public class MouseButtonInput : ButtonBinding {

		bool ignoreImGui;
		MouseButton mouseButton;
		public enum MouseButton {

			Left = 12,
			Right = 13,
			Middle = 14,
			Mouse4 = 15,
			Mouse5 = 16,
			WheelUp = 8,
			WheelDown = 7,

		}

		public MouseButtonInput(MouseButton mouseButton, bool ignoreImGui = false) {
			MouseInputDevice.Init();
			MouseInputDevice.deviceUpdate.Add(MouseUpdate);
			this.mouseButton = mouseButton;
			this.ignoreImGui = ignoreImGui;
		}

		private void MouseUpdate(MouseUpdate e) {

			if (((int)e.Offset) != ((int)mouseButton) || mouseButton == MouseButton.WheelUp || mouseButton == MouseButton.WheelDown) {
				return;
			}

			if (ImGui.GetIO().WantCaptureMouse == true && ignoreImGui == false) {
				inputButton = false;
				return;
			}

			if (mouseButton == MouseButton.WheelUp) {
				inputButton |= e.Value == 120;
				return;
			}
			if (mouseButton == MouseButton.WheelDown) {
				inputButton |= e.Value == -120;
				return;
			}

			

			inputButton = e.Value == 128;
			


			
		}

		protected override void NextFrame() {
			
			if (mouseButton == MouseButton.WheelDown || mouseButton == MouseButton.WheelUp) {
				inputButton = false;
			}

		}

	}
}
