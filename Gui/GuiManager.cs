using ArcticFoxEngine.Demos.ChildTest;
using ArcticFoxEngine.Demos.LightingTest;
using ArcticFoxEngine.Demos.RenderingStressTest;
using ArcticFoxEngine.Demos.SceneTest;
using ArcticFoxEngine.Gui.Builtin_Windows;
using ArcticFoxEngine.ImGuiIntegration;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using SharpDX.Windows;

namespace ArcticFoxEngine.Debug {
	public static class GuiManager {

		private static bool isOpen;

		private static List<GuiWindow> windows;
		private static List<CustomWindow> temporaryWindows;

		private static List<GuiOverlay> overlays;

		private static List<Type> demoNodes;

		private static Vector4 menuSubtitleCol = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);

		internal static void Init(RenderForm form) {

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
				new NodeGizmosOverlay(),
			};

			demoNodes = new List<Type>() {
				typeof(CubeSpin),
				typeof(ChildTestNode),
				typeof(RenderingStressTestNode),
				typeof(LightingTestNode),
			};


			Log.ListenToLog(GetDebugWindow<LogWindow>().LogEvent);
			Log.ListenToLogColor(GetDebugWindow<LogWindow>().LogColorEvent);

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

		public static void OpenGUI() {

			LoadWindowOptions();

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
			RenderImGui.Render(Graphics.renderTargets[Graphics.frameIndex], Graphics.rtvHeap, Graphics.dsvHeap);

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
					// We can skip the ImGUi.begin here because the imgui windows already have the ImGui.Begin();
					//ImGui.Begin(imguiWindows[i].name + "##" + imguiWindows[i].GetHashCode(), ref builtinWindows[i].open);
					windows[i].Render();
					//ImGui.End();
				}
			}
			for (int i = temporaryWindows.Count - 1; i >= 0; i--) {
				if (temporaryWindows[i].open == true) {
					ImGui.Begin(temporaryWindows[i].name + "##" + temporaryWindows[i].GetHashCode(), ref temporaryWindows[i].open ,ImGuiWindowFlags.None);
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

			JObject windowJson = new JObject();
			JArray windowOptions = new JArray();

			for (int i = 0; i < windows.Count; i ++) {
				JObject currentWindowOption = new JObject();
				currentWindowOption.Put("name", windows[i].name);
				currentWindowOption.Put("open", windows[i].open);
				windowOptions.Add(currentWindowOption);
			}
			windowJson.Put("windows", windowOptions);

			File.WriteAllText("windowconfig.json", windowJson.ToString());
			ImGui.SaveIniSettingsToDisk("imgui.ini");

		}
		private static void LoadWindowOptions() {

			if (File.Exists("windowconfig.json") == false) {
				return;
			}
			JObject windowJson = new JObject(File.ReadAllText("windowconfig.json"));
			JArray windowOptions = windowJson.GrabArray("windows");

			for (int i = 0; i < windowOptions.Count; i ++) {

				JObject currentWindowOption = windowOptions[i];
				string name = currentWindowOption.Grab("name");
				bool open = currentWindowOption.Grab("open") == "True";

				for (int n = 0; n < windows.Count; n ++) {
					if (windows[n].name == name) {
						windows[n].open = open;
						break;
					}
				}

			}

			


			ImGui.LoadIniSettingsFromDisk("imgui.ini");

		}

		public static void Dispose() {
			RenderImGui.Dispose();
		}

	}
}
