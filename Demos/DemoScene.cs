using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Testing {
	public abstract class DemoScene {

		internal abstract string name { get; }

		internal abstract Scene LoadScene();
		internal abstract void UnloadScene();

	}
}
