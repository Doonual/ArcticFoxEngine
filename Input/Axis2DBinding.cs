using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input {
	public abstract class Axis2DBinding : InputBinding {

		internal Vector2 axis2DCurrent;
		internal Vector2 axis2DPrev;

		protected Vector2 axis2DActive;

		public Axis2DBinding() {
			axis2DCurrent = Vector2.zero;
			axis2DPrev = Vector2.zero;
			axis2DActive = Vector2.zero;
		}

		internal override void BufferValues() {

			if (Engine.form.Focused == false) {
				axis2DActive = Vector2.zero;
			}

			axis2DPrev = axis2DCurrent;
			axis2DCurrent = axis2DActive;

			

		}

		internal override void NextFrame_() {
			NextFrame();
		}

		public Vector2 GetValue() {
			return axis2DCurrent;
		}
		public Vector2 GetDelta() {
			return axis2DCurrent - axis2DPrev;
		}

		protected abstract void NextFrame();

	}
}