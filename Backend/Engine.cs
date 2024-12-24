using ArcticFoxEngine.Gui;
using ArcticFoxEngine.Gui.Commands;
using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using SharpDX.Windows;

namespace ArcticFoxEngine {
	public static class Engine {

		private static bool disposed = true;

		internal static RenderForm form;
		private static RenderLoop loop;

		// Setup function. Run before any of the loop.
		public static Action init;

		// Gui and exit buttons
		private static ButtonBinding exitButton;
		private static ButtonBinding toggleGuiButton;

		internal static bool deubgRunMainLoop = true;
		internal static bool debugRunMainLoopOnce = false;

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
			Screen.Init(form);
			InputManager.Init();

			GuiManager.Init(form);
			Log.Success("Engine initialisation complete");

			Log.Raw("");

			#endregion

			exitButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.Escape);
			toggleGuiButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.F1);


			if (init != null) { init(); } // Run the main init code


			// Main game loop
			using (loop = new RenderLoop(form)) {
				while (loop != null && loop.NextFrame()) {

					Profiler.FrameBegin();

					// Input update
					Profiler.MetricBegin("Input update");
					InputManager.NextFrame();
					InputManager.GetInputDeviceUpdates();
					Profiler.MetricEnd();

					if (deubgRunMainLoop == true || debugRunMainLoopOnce == true) {

						// Scene update
						Profiler.MetricBegin("Scene update");
						if (Node.rootNode != null) {

							Profiler.MetricBegin("Node update");
							Node.rootNode.UpdateEvent();
							Profiler.MetricEnd();

							Profiler.MetricBegin("Render");
							Node.rootNode.RenderEvent();
							Profiler.MetricEnd();

						}

						debugRunMainLoopOnce = false;
					}

					Profiler.MetricEnd();
					Profiler.FrameEnd();


					// Check for debug button
					if (toggleGuiButton.GetButtonDown() == true) { GuiManager.ToggleGUI(); }

					// Check for exit button
					if (exitButton.GetButton() == true) { Stop(); }



					GuiManager.UpdateImGui();

					Graphics.WaitForDirectCommandQueue();
					Graphics.Buffer();


					form.Show();
				}
			}


			Stop();

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
			Graphics.Dispose();
			Rendering.Rendering.Dispose();
			GuiManager.CloseGUI();

			Environment.Exit(0);

		}

	}
}
