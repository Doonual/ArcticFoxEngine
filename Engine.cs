using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcticFoxEngine {
	public static class Engine {

		private static RenderForm form;

		public static void Create(int width, int height, string title = "Arctic Fox", string iconPath = "res/icon.ico") {

			form = new RenderForm(title) {
				Width = width + 16,
				Height = height + 39,
				Icon = new Icon(iconPath),
			};
			form.Show();

			Graphics.Initialise(form);

			using (RenderLoop loop = new RenderLoop(form)) {
				while (loop.NextFrame()) {
					MainEventLoop();
				}
			}
			
			Graphics.Dispose();

		}

		private static void MainEventLoop() {

			Graphics.Render();
			Graphics.WaitForPreviousFrame();

		}

	}
}
