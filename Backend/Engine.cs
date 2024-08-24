using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Debug;
using ArcticFoxEngine.Debug.Commands;
using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using ArcticFoxEngine;
using CoolClassLibrary;
using SharpDX.Windows;

namespace ArcticFoxEngine {
	public static class Engine {

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

			CommandController.Init(new List<Command>() {
				new HelpCommand(),
				new AddObjectCommand(),
			});
			

			#region Create the main window

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

			#endregion
			#region Setup rendering

			try {
				Graphics.SetupRenderer(form);
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
					Scene.PerformSceneSwap();
					InputManager.GetInputDeviceUpdates();

					
					if (Scene.activeScene != null) {
						Scene.activeScene.NewFrame();
						GPU_Render.Render(Scene.activeScene.mainCamera, Scene.activeScene.mainGeometry);
					}

					if (exitButton.GetButton() == true) { Stop(); }
					if (toggleDebugButton.GetButtonDown() == true) { DebugManager.ToggleGUI(); }

					Profiler.FrameEnd();

					DebugManager.UpdateImGui();

					Graphics.Buffer();
					Graphics.WaitForPreviousFrame();
					InputManager.NextFrame();

				}
			}
			
			Graphics.Dispose();
			DebugManager.CloseGUI();

		}
		
		/// <summary>
		/// Closes ArcticFoxEngine
		/// </summary>
		public static void Stop() {
			loop.Dispose();
			loop = null;
			CommandController.Stop();
			DebugManager.Dispose();
		}

	}
}
