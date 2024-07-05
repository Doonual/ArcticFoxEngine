using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input {
	public abstract class AxisBinding : InputBinding {

		internal float axisCurrent;
		internal float axisPrev;

		internal Action<float> activeChangedEvent;

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

		internal override void BufferValues() {

			axisPrev = axisCurrent;
			axisCurrent = axisActive;
		}

		internal override void NextFrame_() {
			NextFrame();
		}

		public float GetValue() {
			return axisPrev;
		}
		public float GetDelta() {
			return axisCurrent - axisPrev;
		}

		protected abstract void NextFrame();

	}
}