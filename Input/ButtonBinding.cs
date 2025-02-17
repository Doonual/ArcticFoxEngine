namespace ArcticFoxEngine.Input {
	public abstract class ButtonBinding : InputBinding {

		internal bool buttonDownCurrent;
		internal bool buttonDownPrev;

		protected bool inputButton;


		public bool GetButtonDown() {
			return buttonDownCurrent && !buttonDownPrev;
		}
		public bool GetButtonUp() {
			return !buttonDownCurrent && buttonDownPrev;
		}
		public bool GetButton() {
			return buttonDownCurrent;
		}

		internal override void BufferValues() {

			if (MainWindow.form.Focused == false) {
				inputButton = false;
			}

			buttonDownPrev = buttonDownCurrent;
			buttonDownCurrent = inputButton;

		}
		internal override void NextFrame_() {
			NextFrame();
		}
		protected abstract void NextFrame();


		


	}
}