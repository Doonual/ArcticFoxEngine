using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Debug {
	internal abstract class DebugWindow {

		internal abstract string name { get; }
		internal bool open = false;
		internal abstract void Render();

	}
}
