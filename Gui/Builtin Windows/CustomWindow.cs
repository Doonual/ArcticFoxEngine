using ArcticFoxEngine.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class CustomWindow : GuiWindow {
		
		public override string name => windowName;

		public override void Render() {
			renderFunc();
		}

		public CustomWindow(string name, Action renderFunc) : base("") {

			windowName = name;
			this.renderFunc = renderFunc;

		}

		public string windowName;
		public Action renderFunc;

	}
}
