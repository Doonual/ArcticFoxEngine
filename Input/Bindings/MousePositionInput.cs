using ArcticFoxEngine.Gui;
using ArcticFoxEngine.ImGuiIntegration;
using ArcticFoxEngine.Input.Devices;
using ImGuiNET;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input.Bindings {

	public class MousePositionInput : Axis2DBinding {

		bool ignoreImGui;

		public MousePositionInput(bool ignoreImGui = false) {
			this.ignoreImGui = ignoreImGui;
		}


		protected override void NextFrame() {

			if (ImGui.GetIO().WantCaptureMouse == true && ignoreImGui == false && false) {
				axis2DActive = Vector2.zero;
				return;
			}

			POINT cursorPos;
			User32.GetCursorPos(out cursorPos);
			axis2DActive = new Vector2(cursorPos.X, cursorPos.Y);

		}
	}

}
