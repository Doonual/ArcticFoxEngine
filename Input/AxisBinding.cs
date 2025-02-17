namespace ArcticFoxEngine.Input {
	public abstract class AxisBinding : InputBinding {

		internal float axisCurrent;
		internal float axisPrev;

		internal Action<float> activeChangedEvent = (float a) => { };


		protected float axisActive {
			get {
				return axisActive_;
			}
			set {
				activeChangedEvent(value);
				axisActive_ = value;
			}
		}
		private float axisActive_;


		public float GetValue() {
			return axisPrev;
		}
		public float GetDelta() {
			return axisCurrent - axisPrev;
		}

		internal override void BufferValues() {

			if (MainWindow.form.Focused == false) {
				axisActive = 0f;
			}

			axisPrev = axisCurrent;
			axisCurrent = axisActive;
		}
		internal override void NextFrame_() {
			NextFrame();
		}
		protected abstract void NextFrame();

	}
}