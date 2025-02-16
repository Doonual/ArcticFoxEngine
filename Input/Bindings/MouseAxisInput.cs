using ArcticFoxEngine.Gui;
using ArcticFoxEngine.Input.Devices;
using CoolClassLibrary;
using ImGuiNET;
using SharpDX.DirectInput;

namespace ArcticFoxEngine.Input.Bindings {
	public class MouseAxisInput : AxisBinding {

		bool ignoreImGui;
		MouseAxis axis;

		public enum MouseAxis {

			x = 0,
			y = 4

		}
		public MouseAxisInput(MouseAxis axis, bool ignoreImGui = false) {
			MouseInputDevice.Init();
			MouseInputDevice.deviceUpdate.Add(MouseUpdate);
			this.axis = axis;
			this.ignoreImGui = ignoreImGui;
		}

		private void MouseUpdate(MouseUpdate args) {

			if (((int)axis) != ((int)args.Offset)) {
				return;
			}

			if (ImGui.GetIO().WantCaptureMouse == true && ignoreImGui == false) {
				axisActive = 0f;
				return;
			}

			axisActive += args.Value;

		}

		protected override void NextFrame() {
			axisActive = 0f;
		}
	}
}
