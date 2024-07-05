using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input {
	public abstract class ButtonBinding : InputBinding {

		internal bool buttonDownCurrent;
		internal bool buttonDownPrev;

		protected bool inputButton;

		internal override void BufferValues() {

			buttonDownPrev = buttonDownCurrent;
			buttonDownCurrent = inputButton;

		}

		internal override void NextFrame_() {
			NextFrame();
		}

		public bool GetButtonDown() {
			return buttonDownCurrent && !buttonDownPrev;
		}
		public bool GetButtonUp() {
			return !buttonDownCurrent && buttonDownPrev;
		}
		public bool GetButton() {
			return buttonDownCurrent;
		}

		protected abstract void NextFrame();


	}
}