using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input.Bindings {
	public class GenericAxis2DInput : Axis2DBinding {

		AxisBinding xAxis;
		AxisBinding yAxis;

		// Because each Axis must be created before the Axis2D, their NextFrame methods are always called before this one

		public GenericAxis2DInput(AxisBinding xAxis, AxisBinding yAxis) {

			this.xAxis = xAxis;
			this.yAxis = yAxis;

			this.xAxis.activeChangedEvent = AxisUpdatedX;
			this.yAxis.activeChangedEvent = AxisUpdatedY;

		}

		private void AxisUpdatedX(float value) {
			axis2DActive.x = value;
		}
		private void AxisUpdatedY(float value) {
			axis2DActive.y = value;
		}

		protected override void NextFrame() {
			
		}

	}
}
