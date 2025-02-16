using ArcticFoxEngine.Demos.ChildTest;
using ArcticFoxEngine.Demos.LightingTest;
using ArcticFoxEngine.Demos.RenderingStressTest;
using ArcticFoxEngine.Demos.SceneTest;
using ArcticFoxEngine.Gui.Builtin_Windows;
using ArcticFoxEngine.ImGuiIntegration;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using SharpDX.Direct3D12;
using SharpDX.Windows;

namespace ArcticFoxEngine.Gui {
	public static class GuiManager {

		private static Texture renderTexture;
		private static DescriptorHeap rtvDescHeap;
		private static Texture depthTexture;
		private static DescriptorHeap dsvDescHeap;


		private static bool isOpen;

		private static List<GuiWindow> windows;
		private static List<CustomWindow> temporaryWindows;
		private static List<GuiOverlay> overlays;
		private static List<Type> demoNodes;

		private static Vector4 menuSubtitleCol = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);


		internal static void Init(RenderForm form) {

			// Create render texture and accompanying descriptor heap
			renderTexture = new Texture(Screen.width, Screen.height, flags: ResourceFlags.AllowRenderTarget);
			DescriptorHeapDescription rtvDescHeapDescription = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Flags = DescriptorHeapFlags.None,
				Type = DescriptorHeapType.RenderTargetView,
			};
			rtvDescHeap = Graphics.device.CreateDescriptorHeap(rtvDescHeapDescription);
			Graphics.device.CreateRenderTargetView(renderTexture.resource, null, rtvDescHeap.CPUDescriptorHandleForHeapStart);

			// Create depth texture and accompanying descriptor heap
			depthTexture = new Texture(Screen.width, Screen.height, format: SharpDX.DXGI.Format.D32_Float, flags: ResourceFlags.AllowDepthStencil, initialState: ResourceStates.DepthWrite);
			DescriptorHeapDescription dsvDescHeapDescription = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Flags = DescriptorHeapFlags.None,
				Type = DescriptorHeapType.DepthStencilView,
			};
			dsvDescHeap = Graphics.device.CreateDescriptorHeap(dsvDescHeapDescription);


			DepthStencilViewDescription dsvDescription = new DepthStencilViewDescription() {
				Flags = DepthStencilViewFlags.None,
				Format = SharpDX.DXGI.Format.D32_Float,
			};

			Graphics.device.CreateDepthStencilView(depthTexture.resource, null, dsvDescHeap.CPUDescriptorHandleForHeapStart);

			RenderImGui.Init(1920, 1080);
			ImGuiInput.Init(form.Handle);
			
			isOpen = false;

			windows = new List<GuiWindow>() {
				new ImGuiAboutWindow("ImGui"),
				new ImGuiDebugLogWindow("ImGui"),
				new ImGuiDemoWindow("ImGui"),
				new ImGuiFontSelectorWindow("ImGui"),
				new ImGuiMetricsWindow("ImGui"),
				new ImGuiStackToolWindow("ImGui"),
				new ImGuiStyleEditorWindow("ImGui"),
				new ImGuiStyleSelectorWindow("ImGui"),
				new ImGuiUserGuideWindow("ImGui"),

				new LogWindow(),
				new PerformanceWindow(),
				new SceneWindow(),

			};
			temporaryWindows = new List<CustomWindow>();

			overlays = new List<GuiOverlay>() {
				new NodeIconsOverlay(),
			};

			demoNodes = new List<Type>() {
				typeof(CubeSpin),
				typeof(ChildTestNode),
				typeof(RenderingStressTestNode),
				typeof(LightingTestNode),
			};


			Log.ListenToLog(GetDebugWindow<LogWindow>().LogEvent);
			Log.ListenToLogColor(GetDebugWindow<LogWindow>().LogColorEvent);

			AppDomain.CurrentDomain.ProcessExit += (System.Object sender, EventArgs e) => { Dispose(); };

		}

		internal static T GetDebugWindow<T>() where T : GuiWindow {

			for (int i = 0; i < windows.Count; i++) {
				if (windows[i].GetType() == typeof(T)) {
					return (T)windows[i];
				}
			}
			return null;

		}
		public static void AddGuiWindow(GuiWindow newWindow) {
			windows.Add(newWindow);
		}
		public static void OpenWindow(string name, Action renderFunc) {
			CustomWindow newWindow = new CustomWindow(name, renderFunc);
			newWindow.open = true;
			temporaryWindows.Add(newWindow);
		}

		public static bool IsGuiOpen() {
			return isOpen;
		}
		public static void OpenGUI() {
			isOpen = true;
		}
		public static void CloseGUI() {
			isOpen = false;
		}
		public static void ToggleGUI() {
			if (isOpen == false) {
				OpenGUI();
			}
			else {
				CloseGUI();
			}
		}

		public static void UpdateImGui() {

			if (isOpen == false) { return; }
			RenderImGui.Render();
			Graphics.Blit(RenderImGui.renderTexture, Graphics.GetActiveResource());

		}

		private static bool firstOpen = true;
		internal static void Render() {

			if (firstOpen == true) {
				LoadWindowOptions();
				firstOpen = false;
			}

			if (ImGui.BeginMainMenuBar() == true) {

				if (ImGui.BeginMenu("Engine") == true) {

					ImGui.PushStyleColor(ImGuiCol.Text, menuSubtitleCol);
					ImGui.Text("Update");
					ImGui.PopStyleColor();

					ImGui.MenuItem("Update loop", null, ref Engine.deubgRunMainLoop);
					ImGui.MenuItem("Update loop once", null, ref Engine.debugRunMainLoopOnce);

					ImGui.Separator();

					ImGui.PushStyleColor(ImGuiCol.Text, menuSubtitleCol);
					ImGui.Text("Root Node");
					ImGui.PopStyleColor();

					if (ImGui.BeginMenu("Load root node") == true) {

						for (int i = 0; i < demoNodes.Count; i++) {
							if (ImGui.MenuItem(demoNodes[i].Name) == true) {
								Node newNode = (Node)Activator.CreateInstance(demoNodes[i]);
								Node.SetRootNode(newNode);
							}
						}

						ImGui.EndMenu();
					}

					ImGui.Separator();
					if (ImGui.MenuItem("Exit", "ESC") == true) {
						Engine.Stop();
					}

					ImGui.EndMenu();
				}


				// View menu options
				if (ImGui.BeginMenu("View") == true) {

					ImGui.PushStyleColor(ImGuiCol.Text, menuSubtitleCol);
					ImGui.Text("Window");
					ImGui.PopStyleColor();

					
					if (ImGui.MenuItem("Close all windows") == true) {
						for (int i = 0; i < windows.Count; i ++) {
							windows[i].open = false;
						}
					}



					List<GuiWindow> windowOptionList = new List<GuiWindow>();
					windowOptionList.AddRange(windows);
					DrawWindowMenu(windowOptionList, 0);

					ImGui.Separator();

					ImGui.PushStyleColor(ImGuiCol.Text, menuSubtitleCol);
					ImGui.Text("Overlay");
					ImGui.PopStyleColor();

					if (ImGui.MenuItem("Close all overlays") == true) {
						for (int i = 0; i < overlays.Count; i++) {
							overlays[i].open = false;
						}
					}


					for (int i = 0; i < overlays.Count; i++) {
						ImGui.MenuItem(overlays[i].name, null, ref overlays[i].open);
					}

					ImGui.EndMenu();
				}

			

				ImGui.EndMenuBar();

			}
			


			// Render builtin windows
			for (int i = 0; i < windows.Count; i++) {
				if (windows[i].open == true) {


					if (windows[i].setWindowPos.x >= 0f) {
						ImGui.SetNextWindowPos(windows[i].setWindowPos);
						windows[i].setWindowPos = new Vector2(-1f, -1f);
					}
					if (windows[i].setWindowSize.x >= 0f) {
						ImGui.SetNextWindowSize(windows[i].setWindowSize);
						windows[i].setWindowSize = new Vector2(-1f, -1f);
					}

					windows[i].Render();
				}
			}
			
			// Render temporary windows
			for (int i = temporaryWindows.Count - 1; i >= 0; i--) {
				if (temporaryWindows[i].open == true) {

					if (temporaryWindows[i].setWindowPos.x >= 0f) {
						ImGui.SetNextWindowPos(temporaryWindows[i].setWindowPos); 
						temporaryWindows[i].setWindowPos = new Vector2(-1f, -1f);
					}
					if (temporaryWindows[i].setWindowSize.x >= 0f) {
						ImGui.SetNextWindowSize(temporaryWindows[i].setWindowSize);
						temporaryWindows[i].setWindowSize = new Vector2(-1f, -1f);
					}

					ImGui.Begin(temporaryWindows[i].name + "##" + temporaryWindows[i].GetHashCode(), ref temporaryWindows[i].open, ImGuiWindowFlags.None);
					temporaryWindows[i].Render();
					ImGui.End();
				}
				else {
					temporaryWindows.RemoveAt(i);
				}
			}


			for (int i = 0; i < overlays.Count; i++) {
				if (overlays[i].open == true) {
					overlays[i].Render();
				}
			}

		}

		private static void DrawWindowMenu(List<GuiWindow> optionsToRender, int groupStartIndex) {

			// Figure out what windows to render without putting them in groups
			List<GuiWindow> noGroupWindows = new List<GuiWindow>();
			for (int i = 0; i < optionsToRender.Count; i ++) {

				if (optionsToRender[i].menuGroups.Length == groupStartIndex) {
					// There are no more menu groups to process, draw the option here
					noGroupWindows.Add(optionsToRender[i]);
					optionsToRender.RemoveAt(i);
					i--;
				}
			}

			if (optionsToRender.Count != 0) {

				List<GuiWindow> currentOptions = new List<GuiWindow>();
				string firstGroupName = optionsToRender[0].menuGroups[groupStartIndex]; // Record the group name of the 1st GuiWindow

				// Loop through all the GuiWindows and record all of them that have the same group name of the 1st one
				for (int i = 0; i < optionsToRender.Count; i++) {
					if (optionsToRender[i].menuGroups[groupStartIndex] == firstGroupName) {
						currentOptions.Add(optionsToRender[i]);
					}
				}

				// Remove all the recorded GuiWindows from the render list
				for (int i = 0; i < currentOptions.Count; i++) {
					optionsToRender.Remove(currentOptions[i]);
				}

				if (ImGui.BeginMenu(firstGroupName) == true) {

					DrawWindowMenu(currentOptions, groupStartIndex + 1);
					ImGui.EndMenu();
				}

				DrawWindowMenu(optionsToRender, groupStartIndex);

			}
			
			// Render the window option without groups
			for (int i = 0; i < noGroupWindows.Count; i ++) {
				ImGui.MenuItem(noGroupWindows[i].name, null, ref noGroupWindows[i].open);
			}

		}

		private static void SaveWindowOptions() {

			JObject configJson = new JObject();
			JArray windowOptions = new JArray();
			JArray overlayOptions = new JArray();

			for (int i = 0; i < windows.Count; i ++) {
				JObject currentWindowOption = new JObject();
				currentWindowOption.Put("name", windows[i].name);
				currentWindowOption.Put("open", windows[i].open);
				windowOptions.Add(currentWindowOption);
			}
			configJson.Put("windows", windowOptions);

			for (int i = 0; i < overlays.Count; i ++) {
				JObject currentOverlayOption = new JObject();
				currentOverlayOption.Put("name", overlays[i].name);
				currentOverlayOption.Put("open", overlays[i].open);
				overlayOptions.Add(currentOverlayOption);
			}
			configJson.Put("overlays", overlayOptions);

			configJson.Put("imgui", ImGui.SaveIniSettingsToMemory());

			File.WriteAllText("gui.json", configJson.ToString());

		}
		private static void LoadWindowOptions() {

			if (File.Exists("gui.json") == false) {
				return;
			}

			JObject configJson = new JObject(File.ReadAllText("gui.json"));

			JArray windowOptions = configJson.GrabArray("windows");
			for (int i = 0; i < windowOptions.Count; i ++) {

				JObject currentWindowOption = windowOptions[i];
				string name = currentWindowOption.Grab("name");
				bool open = bool.Parse(currentWindowOption.Grab("open"));

				for (int n = 0; n < windows.Count; n ++) {
					if (windows[n].name == name) {
						windows[n].open = open;
						break;
					}
				}

			}

			JArray overlayOptions = configJson.GrabArray("overlays");
			for (int i = 0; i < overlayOptions.Count; i++) {

				JObject currentOverlayOption = overlayOptions[i];
				string name = currentOverlayOption.Grab("name");
				bool open = bool.Parse(currentOverlayOption.Grab("open"));

				for (int n = 0; n < overlays.Count; n++) {
					if (overlays[n].name == name) {
						overlays[n].open = open;
						break;
					}
				}

			}

			string imguiConfig = configJson.Grab("imgui");
			ImGui.LoadIniSettingsFromMemory(imguiConfig);

		}

		static bool disposed = false;
		public static void Dispose() {
			if (disposed == true) { return; }
			disposed = true;

			if (firstOpen == false) {
				SaveWindowOptions();
			}
			
			RenderImGui.Dispose();
		}


	}
}
