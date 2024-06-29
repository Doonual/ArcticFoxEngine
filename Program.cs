using SharpDX.Windows;

namespace Engine {

	internal static class Program {

		[STAThread]
		static void Main() {

			RenderForm form = new RenderForm("Hello Triangle") {
				Width = 1280,
				Height = 800
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