using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Debug;
using ArcticFoxEngine.Debug.Commands;
using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using ArcticFoxEngine;
using CoolClassLibrary;
using SharpDX.Windows;
using SharpDX.DXGI;

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

			try {
				Graphics.Init(form);

				GPU_Upload.Init();
				Backend.Render.GPU_Render.Init(width, height);
				Screen.InitScreen(form);
				InputManager.InitInput();

				DebugManager.Init(form);
				Log.Success("Engine initialisation complete");
			}
			catch (Exception e) {
				Log.Error("Failed to initialise engine");
				Log.Raw(e);
			}
			Log.Raw("");

			#endregion

			exitButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.Escape);
			toggleDebugButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.F1);


			if (init != null) {	init();	} // Run the main init code

			
			// Main game loop
			using (loop = new RenderLoop(form)) {
				while (loop != null && loop.NextFrame()) {


					Profiler.FrameBegin();

					Profiler.MetricBegin("Input update");
					InputManager.NextFrame();
					InputManager.GetInputDeviceUpdates();
					Profiler.MetricEnd();

					Profiler.MetricBegin("Scene update");
					Scene.PerformSceneSwap();
					if (Scene.activeScene != null) {
						Scene.activeScene.Update();
					}
					Profiler.MetricEnd();

					
					if (toggleDebugButton.GetButtonDown() == true) { DebugManager.ToggleGUI(); }

					Profiler.FrameEnd();

					DebugManager.UpdateImGui();
					if (exitButton.GetButton() == true) { Stop(); }

					
					Graphics.Buffer();
					form.Show();
				}
			}
			
			Graphics.Dispose();
			Backend.Render.GPU_Render.Dispose();

			DebugManager.CloseGUI();

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
			DebugManager.Dispose();
		}

	}
}
