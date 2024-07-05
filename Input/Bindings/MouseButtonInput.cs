using ArcticFoxEngine.Input.Devices;
using CoolClassLibrary;
using SharpDX.RawInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input.Bindings {
	public class MouseButtonInput : ButtonBinding {


		MouseButton mouseButton;
		public enum MouseButton {

			Left,
			Right,
			Middle,
			Button1,
			Button2,
			Button3,
			Button4,
			Button5

		}

		public MouseButtonInput(MouseButton mouseButton) {
			MouseInputDevice.Init();
			MouseInputDevice.deviceUpdate.Add(MouseUpdate);
			this.mouseButton = mouseButton;
		}

		private void MouseUpdate(MouseInputEventArgs e) {
			
			switch (mouseButton) {

				case MouseButton.Left:
				if (e.ButtonFlags == MouseButtonFlags.LeftButtonDown) {
					inputButton = true;
				}
				if (e.ButtonFlags == MouseButtonFlags.LeftButtonUp) {
					inputButton = false;
				}
				break;

				case MouseButton.Right:
				if (e.ButtonFlags == MouseButtonFlags.RightButtonDown) {
					inputButton = true;
				}
				if (e.ButtonFlags == MouseButtonFlags.RightButtonUp) {
					inputButton = false;
				}
				break;

				case MouseButton.Middle:
				if (e.ButtonFlags == MouseButtonFlags.MiddleButtonDown) {
					inputButton = true;
				}
				if (e.ButtonFlags == MouseButtonFlags.MiddleButtonUp) {
					inputButton = false;
				}
				break;

				case MouseButton.Button1:
				if (e.ButtonFlags == MouseButtonFlags.Button1Down) {
					inputButton = true;
				}
				if (e.ButtonFlags == MouseButtonFlags.Button1Up) {
					inputButton = false;
				}
				break;

				case MouseButton.Button2:
				if (e.ButtonFlags == MouseButtonFlags.Button2Down) {
					inputButton = true;
				}
				if (e.ButtonFlags == MouseButtonFlags.Button2Up) {
					inputButton = false;
				}
				break;

				case MouseButton.Button3:
				if (e.ButtonFlags == MouseButtonFlags.Button3Down) {
					inputButton = true;
				}
				if (e.ButtonFlags == MouseButtonFlags.Button3Up) {
					inputButton = false;
				}
				break;

				case MouseButton.Button4:
				if (e.ButtonFlags == MouseButtonFlags.Button4Down) {
					inputButton = true;
				}
				if (e.ButtonFlags == MouseButtonFlags.Button4Up) {
					inputButton = false;
				}
				break;

				case MouseButton.Button5:
				if (e.ButtonFlags == MouseButtonFlags.Button5Down) {
					inputButton = true;
				}
				if (e.ButtonFlags == MouseButtonFlags.Button5Up) {
					inputButton = false;
				}
				break;


			}
			
			
		}

		protected override void NextFrame() {}

	}
}
