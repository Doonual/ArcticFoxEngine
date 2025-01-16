using ArcticFoxEngine.ImGuiIntegration;
using ArcticFoxEngine.Input.Devices;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input.Bindings {

	public class MousePositionInput : Axis2DBinding {

		

		public MousePositionInput() {

		}


		protected override void NextFrame() {
			POINT cursorPos;
			User32.GetCursorPos(out cursorPos);
			axis2DActive = new Vector2(cursorPos.X, cursorPos.Y);

		}
	}

}
