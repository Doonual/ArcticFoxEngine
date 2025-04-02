using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Gui {

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class GuiWindowOptionsAttribute : Attribute {

		public string name;
		public bool allowMultipleWindows;

		public GuiWindowOptionsAttribute(string name, bool allowMultipleWindows = false) {
			this.name = name;
			this.allowMultipleWindows = allowMultipleWindows;
		}

	}
}
