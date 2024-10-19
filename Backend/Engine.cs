using ArcticFoxEngine.Debug;
using ArcticFoxEngine.Debug.Commands;
using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using SharpDX.Windows;

namespace ArcticFoxEngine {
	public static class Engine {

		private static bool disposed = true;

		internal static RenderForm form;
		public static Action init;
		private static RenderLoop loop;

		static ButtonBinding exitButton;
		static ButtonBinding toggleDebugButton;

		/// <summary>
		/// Runs ArcticFoxEngine
		/// </summary>
		/// <param name="width">The width of the window</param>
		/// <param name="height">The height of the window</param>
		/// <param name="title">The title of the window</param>
		/// <param name="iconPath">The path to the icon the window will use</param>
		public static void Run(int width, int height, string title = "Arctic Fox", string iconPath = ".res/icon.ico") {
			if (disposed == false) { Log.Warn("Cannot run ArcticFoxEngine, already running"); return; }
			disposed = false;

			#region Create the main window

			try {
				form = new RenderForm(title) {
					Width = width + 16,
					Height = height + 39,
					Icon = new Icon(iconPath),
					FormBorderStyle = FormBorderStyle.None,
				};
				form.BackColor = new Color(0, 0, 0);

				form.Width = 1920;
				form.Height = 1080;
				form.Location = new Point(0, 0);
				Log.Success("Created window");
			}
			catch (Exception e) {
				Log.Error("Create window failed");
				Log.Raw(e);
			}

			CommandController.Init(new List<Command>() {
				new HelpCommand(),
				new AddObjectCommand(),
			});


			#endregion
			#region Setup rendering

			Graphics.Init(form);
			Upload.Init();
			Rendering.Rendering.Init();
			Screen.InitScreen(form);
			InputManager.InitInput();

			GuiManager.Init(form);
			Log.Success("Engine initialisation complete");

			Log.Raw("");

			#endregion

			exitButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.Escape);
			toggleDebugButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.F1);


			if (init != null) { init(); } // Run the main init code


			// Main game loop
			using (loop = new RenderLoop(form)) {
				while (loop != null && loop.NextFrame()) {


					Profiler.FrameBegin();


					Profiler.MetricBegin("Input update");
					InputManager.NextFrame();
					InputManager.GetInputDeviceUpdates();
					Profiler.MetricEnd();

					Profiler.MetricBegin("Scene update");
					if (Node.rootNode != null) {

						Profiler.MetricBegin("Node update");
						Node.rootNode.UpdateEvent();
						Profiler.MetricEnd();

						Profiler.MetricBegin("Render");
						Node.rootNode.RenderEvent();
						Profiler.MetricEnd();

					}
					Profiler.MetricEnd();


					if (toggleDebugButton.GetButtonDown() == true) { GuiManager.ToggleGUI(); }

					Profiler.FrameEnd();

					GuiManager.UpdateImGui();
					if (exitButton.GetButton() == true) {
						Stop();
						break;
					}

					Graphics.WaitForCmdList();
					Graphics.Buffer();
					form.Show();
				}
			}

			Graphics.WaitForCmdList();
			Graphics.Dispose();
			Rendering.Rendering.Dispose();

			GuiManager.CloseGUI();

		}

		/// <summary>
		/// Closes ArcticFoxEngine
		/// </summary>
		public static void Stop() {
			if (disposed == true) { Log.Warn("Cannot stop ArcticFoxEngine, not running"); return; }
			disposed = true;

			loop.Dispose();
			loop = null;
			CommandController.Stop();
			GuiManager.Dispose();
		}

	}
}
