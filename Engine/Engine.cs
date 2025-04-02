using ArcticFoxEngine.Gui;
using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using SharpDX.Windows;

namespace ArcticFoxEngine {
	public static class Engine {

		private static bool disposed = true;

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
		public static void Run(int width, int height, string title = "Arctic Fox Engine", string iconPath = ".res/icon.ico") {
			if (disposed == false) { Log.Warn("Cannot run ArcticFoxEngine, already running"); return; }
			disposed = false;

			MainWindow.CreateWindow(width, height, title, iconPath);


			bool debug = false;
			#if DEBUG
			debug = true;
			#endif
			Graphics.Init(MainWindow.form, debug);
			InputManager.Init();
			Upload.Init();
			
			Render.Rendering.Init();
			GuiManager.Init(MainWindow.form);

			Log.Success("Engine initialisation complete");
			Log.Raw("");


			exitButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.Escape, ignoreImGui: true);
			toggleGuiButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.F1, ignoreImGui: true);


			if (init != null) { init(); } // Run the main init code


			// Main game loop
			using (loop = new RenderLoop(MainWindow.form)) {
				while (loop != null && loop.NextFrame()) {

					Profiler.FrameBegin();

					// Input update
					Profiler.MetricBegin("Input update");
					if (deubgRunMainLoop == true || debugRunMainLoopOnce == true) {
						InputManager.NextFrame();
					}
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
					GuiManager.UpdateImGui();

					Graphics.WaitForDirectCommandQueue();
					Graphics.Buffer();
					MainWindow.form.Show();

					// Check for exit button
					if (exitButton.GetButton() == true) { Stop(); }
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
			GuiManager.Dispose();
			Graphics.Dispose();
			Render.Rendering.Dispose();
			GuiManager.CloseGUI();

			Environment.Exit(0);

		}

	}
}
