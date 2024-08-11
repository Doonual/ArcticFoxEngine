using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Debug;
using ArcticFoxEngine.Debug.Commands;
using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using ClickableTransparentOverlay;
using CoolClassLibrary;
using SharpDX.Windows;

namespace ArcticFoxEngine {
	public static class Engine {

		private static RenderForm form;
		public static Action init;
		private static RenderLoop loop;

		static ButtonBinding exitButton;
		static ButtonBinding toggleDebugButton;

		public static void Run(int width, int height, string title = "Arctic Fox", string iconPath = ".res/icon.ico") {

			CommandController.Init(new List<Command>() {
				new HelpCommand(),
				new AddObjectCommand(),
			});
			DebugManager.Init();

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
				Log.Success("Engine initialisation complete");
			}
			catch (Exception e) {
				Log.Error("Failed to initialise engine");
				Log.Raw(e);
			}

			#endregion
			
			Log.Raw("");
			

			exitButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.Escape);
			toggleDebugButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.F1);

			if (init != null) {
				init();
			}

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
					
					if (toggleDebugButton.GetButtonDown() == true) {
						if (DebugManager.isOpen == true) {
							DebugManager.CloseGUI();
						}
						else {
							DebugManager.OpenGUI();
						}
					}

					Profiler.FrameEnd();

					
					

					Graphics.Buffer();

					
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
