using ArcticFoxEngine.Gui;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	internal class CustomWindow : GuiWindow {
		
		public override string name => windowName;
		public string windowName;
		public Action renderFunc;


		float defaultWidth = 400f;

		internal float lastWindowHeight;
		public override void Render() {

			renderFunc();

			float currentWindowHeight = ImGui.GetCursorPosY() + ImGui.GetStyle().WindowPadding.Y;
			SetSize(new Vector2(defaultWidth, currentWindowHeight));
		}

		public CustomWindow(string name, Action renderFunc) : base("") {

			windowName = name;
			this.renderFunc = renderFunc;
			SetPosition(ImGui.GetMousePos() - new System.Numerics.Vector2(defaultWidth / 2f, 0f));

		}
		

		

	}
}
