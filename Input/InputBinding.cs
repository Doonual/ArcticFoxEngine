using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Input {
	public abstract class InputBinding {

		internal abstract void BufferValues();
		internal abstract void NextFrame_();

		public InputBinding() {
			InputManager.AddBinding(this);
		}

		~InputBinding() {
			InputManager.RemoveBinding(this);
		}

	}
}
