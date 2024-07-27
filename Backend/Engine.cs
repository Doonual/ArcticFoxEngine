using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Debug;
using ArcticFoxEngine.Input;
using CoolClassLibrary;
using SharpDX.Windows;

namespace ArcticFoxEngine {
	public static class Engine {

		private static RenderForm form;
		public static Action init;

		private static RenderLoop loop;

		public static void Run(int width, int height, string title = "Arctic Fox", string iconPath = ".res/icon.ico") {


			// Create the main window
			try {
				form = new RenderForm(title) {
					Width = width + 16,
					Height = height + 39,
					Icon = new Icon(iconPath),
					FormBorderStyle = FormBorderStyle.None,
				};
				form.Show();
				form.Width = 1920;
				form.Height = 1080;
				form.Location = new Point(0, 0);
				Log.Success("Created window");
			}
			catch (Exception e) {
				Log.Error("Create window failed");
				Log.Raw(e);
			}
			

			try {
				Graphics.SetupRenderer(form);
				DebugManager.InitImGui();
				Log.Success("Engine initialisation complete");
			}
			catch (Exception e) {
				Log.Error("Failed to initialise engine");
				Log.Raw(e);
			}
			Log.Raw("");

			
			init();


			// Main game loop
			using (loop = new RenderLoop(form)) {
				while (loop != null && loop.NextFrame()) {

					long timestamp;
					GPU_Render.cmdQueue.GetClockCalibration(out timestamp, out _);
					Profiler.GpuTimestampFrameStart(timestamp);

					InputManager.GetInputDeviceUpdates();

					if (Scene.activeScene != null) {
						Scene.activeScene.NewFrame();
					}

					Graphics.Buffer();


					GPU_Render.cmdQueue.GetClockCalibration(out timestamp, out _);
					Profiler.GpuTimestampFrameEnd(timestamp);
					Graphics.WaitForPreviousFrame();

					

					InputManager.NextFrame();

				}
			}
			
			Graphics.Dispose();
			DebugManager.CloseGUI();

		}
		public static void Stop() {
			loop.Dispose();
			loop = null;
			CommandController.Stop();
		}

	}
}
