using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Debug;
using ArcticFoxEngine.Input;
using SharpDX.Windows;

namespace ArcticFoxEngine {
	public static class Engine {

		private static RenderForm form;
		public static Action init;

		private static RenderLoop loop;

		public static void Run(int width, int height, string title = "Arctic Fox", string iconPath = ".res/icon.ico") {

			

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

			Graphics.SetupRenderer(form);
			DebugManager.InitImGui();

			init();

			using (loop = new RenderLoop(form)) {
				while (loop != null && loop.NextFrame()) {

					long timestamp;
					Command.mainRenderCommandQueue.GetClockCalibration(out timestamp, out _);
					GPU_Profiler.GpuTimestampFrameStart(timestamp);

					InputManager.GetInputDeviceUpdates();

					if (Scene.activeScene != null) {
						Scene.activeScene.NewFrame();
					}

					Graphics.Buffer();


					Command.mainRenderCommandQueue.GetClockCalibration(out timestamp, out _);
					GPU_Profiler.GpuTimestampFrameEnd(timestamp);
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
		}

	}
}
