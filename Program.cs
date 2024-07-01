using SharpDX.Windows;
using CoolClassLibrary;

namespace Engine {

	internal static class Program {

		[STAThread]
		static void Main() {

			//Log.Init("Arctic Fox Engine", "Doonual", DateTime.Now);


			Log.Info("Starting Engine");
			RenderForm form = new RenderForm("Hello Triangle") {
				Width = 1280 + 16,
				Height = 720 + 39,
				Icon = new Icon("icon.ico"),
			};
			form.Show();

			using (HelloTriangle app = new HelloTriangle()) {

				app.Initialise(form);

				using (RenderLoop loop = new RenderLoop(form)) {

					while (loop.NextFrame()) {

						app.Update();
						app.Render();

					}

				}

			}

		}
	}
}