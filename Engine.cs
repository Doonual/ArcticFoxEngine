using ArcticFoxEngine.Input;
using SharpDX.Windows;

namespace ArcticFoxEngine {
	public static class Engine {

		private static RenderForm form;

		public static Action setup;
		public static Action update;

		public static void Run(int width, int height, string title = "Arctic Fox", string iconPath = ".res/icon.ico") {

			form = new RenderForm(title) {
				Width = width + 16,
				Height = height + 39,
				Icon = new Icon(iconPath),
			};
			form.Show();

			Graphics.SetupRenderer(form);

			if (setup != null) {
				setup();
			}

			using (RenderLoop loop = new RenderLoop(form)) {
				while (loop.NextFrame()) {
					
					if (update != null) {
						update();
					}

					Graphics.Buffer();
					
					Graphics.WaitForPreviousFrame();
					InputManager.NextFrame();
				}
			}
			
			Graphics.Dispose();

		}

	}
}
