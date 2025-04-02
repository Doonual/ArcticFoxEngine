using ArcticFoxEngine.Demos.ChildTest;
using ArcticFoxEngine.Demos.LightingTest;
using ArcticFoxEngine.Demos.RenderingStressTest;
using ArcticFoxEngine.Demos.SceneTest;
using ArcticFoxEngine.Gui.Builtin_Windows;
using ArcticFoxEngine.Gui.Tools;
using ArcticFoxEngine.ImGuiIntegration;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using SharpDX.Direct3D12;
using SharpDX.Windows;
using System.Reflection;


namespace ArcticFoxEngine.Gui {
	public static class GuiManager {


		private static bool isOpen;

		private static List<Type> possibleWindowsList;
		private static List<GuiWindow> windows;
		private static List<GuiOverlay> overlays;


		private static List<Type> demoNodes;
		

		private static Vector4 menuSubtitleCol = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);


		internal static void Init(RenderForm form) {

			// Create render texture and accompanying descriptor heap
			RenderImGui.Init(1920, 1080);
			ImGuiInput.Init(form.Handle);
			
			isOpen = false;

			possibleWindowsList = new List<Type>() {
				typeof(ImGuiAboutWindow),
				typeof(ImGuiDebugLogWindow),
				typeof(ImGuiDemoWindow),
				typeof(ImGuiFontSelectorWindow),
				typeof(ImGuiMetricsWindow),
				typeof(ImGuiStackToolWindow),
				typeof(ImGuiStyleEditorWindow),
				typeof(ImGuiStyleSelectorWindow),
				typeof(ImGuiUserGuideWindow),
				typeof(LogWindow),
				typeof(PerformanceWindow),
				typeof(SceneWindow),
				typeof(TextureInspectorWindow),

			};
			possibleWindowsList = SortGuiWindowsAlphabetically(possibleWindowsList);

			windows = new List<GuiWindow>();

			overlays = new List<GuiOverlay>() {
				new NodeIconsOverlay(),
			};


			demoNodes = new List<Type>() {
				typeof(CubeSpin),
				typeof(ChildTestNode),
				typeof(RenderingStressTestNode),
				typeof(LightingTestNode),
			};


			//Log.ListenToLog(GetDebugWindow<LogWindow>().LogEvent);
			//Log.ListenToLogColor(GetDebugWindow<LogWindow>().LogColorEvent);

			AppDomain.CurrentDomain.ProcessExit += (System.Object sender, EventArgs e) => { Dispose(); };

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
		
		public static void OpenGuiWindow(Type guiWindowType) {

			if (GetAllowMultipleOfGuiWindow(guiWindowType) == false) {

				for (int i = 0; i < windows.Count; i ++) {
					if (windows[i].GetType() == guiWindowType) {
						return;
					}
				}

			}

			GuiWindow newWindow = (GuiWindow)Activator.CreateInstance(guiWindowType);
			windows.Add(newWindow);

			SaveWindowOptions();

		}
		public static void OpenGuiWindow(GuiWindow guiWindow) {
			windows.Add(guiWindow);

			SaveWindowOptions();
		}

		public static void CloseGuiWindow(Type guiWindowType) {

			for (int i = windows.Count - 1; i >= 0; i --) {
				if (windows[i].GetType() == guiWindowType) {
					windows.RemoveAt(i);
				}
			}

			SaveWindowOptions();

		}
		public static void CloseGuiWindow(GuiWindow guiWindow) {
			windows.Remove(guiWindow);

			SaveWindowOptions();

		}

		internal static void UpdateImGui() {

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
						windows.Clear();
					}


					DrawWindowMenu(possibleWindowsList, 0);

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
			for (int i = windows.Count - 1; i >= 0; i--) {

				if (windows[i].open == false) {
					CloseGuiWindow(windows[i]);
					continue;
				}

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
			
			

			for (int i = 0; i < overlays.Count; i++) {
				if (overlays[i].open == true) {
					overlays[i].Render();
				}
			}

		}

		private static void DrawWindowMenu(List<Type> optionsToRender, int currentNameSplitDepth) {

			List<Type> currentOptionsToRender = new List<Type>();
			currentOptionsToRender.AddRange(optionsToRender);

			while (currentOptionsToRender.Count > 0) {

				string[] currentWindowNameSplit = GetNameOfGuiWindow(currentOptionsToRender[0]).Split('/');

				if (currentNameSplitDepth >= currentWindowNameSplit.Length - 1) {

					// Check whether we should include a tick for the windows that can only be instantiated once
					if (GetAllowMultipleOfGuiWindow(currentOptionsToRender[0]) == false) {
						bool enabled = false;
						for (int i = 0; i < windows.Count; i ++) {
							if (windows[i].GetType() == currentOptionsToRender[0]) {
								enabled = true;
							}
						}
						bool changed = ImGui.MenuItem(currentWindowNameSplit[currentWindowNameSplit.Length - 1], null, ref enabled);
						if (changed == true && enabled == true) {
							OpenGuiWindow(currentOptionsToRender[0]);
						}
						if (changed == true && enabled == false) {
							CloseGuiWindow(currentOptionsToRender[0]);
						}

					}
					else {
						if (ImGui.MenuItem(currentWindowNameSplit[currentWindowNameSplit.Length - 1]) == true) {
							OpenGuiWindow(currentOptionsToRender[0]);
						}
					}

					

					currentOptionsToRender.RemoveAt(0);
					continue;
				}




				string nextGroupName = currentWindowNameSplit[currentNameSplitDepth];
				List<Type> optionsToRenderInNextGroup = new List<Type>();
				while (currentOptionsToRender.Count != 0 && GetNameOfGuiWindow(currentOptionsToRender[0]).Split('/')[currentNameSplitDepth] == nextGroupName) {
					optionsToRenderInNextGroup.Add(currentOptionsToRender[0]);
					currentOptionsToRender.RemoveAt(0);
				}

				if (ImGui.BeginMenu(nextGroupName) == true) {
					DrawWindowMenu(optionsToRenderInNextGroup, currentNameSplitDepth + 1);
					ImGui.EndMenu();
				}
				


			}


		}
		
		private static List<Type> SortGuiWindowsAlphabetically(List<Type> guiWindows) {


			Type[] guiWindowsArray = guiWindows.ToArray();
			string[] names = new string[guiWindows.Count];
			long[] sortingKeys = new long[guiWindows.Count];
			for (int i = 0; i < guiWindows.Count; i++) {

				
				string nameOfWindow = GetNameOfGuiWindow(guiWindowsArray[i]);
				names[i] = nameOfWindow;

				long currentKey = 0;
				for (int n = 0; n < nameOfWindow.Length; n++) {
					currentKey += (long)nameOfWindow[n];
					currentKey *= 256;
				}

				sortingKeys[i] = currentKey;

			}

			Array.Sort(names, guiWindowsArray);

			return guiWindowsArray.ToList();

		}
		private static string GetNameOfGuiWindow(Type t) {

			GuiWindowOptionsAttribute nameAttribute = (GuiWindowOptionsAttribute)Attribute.GetCustomAttribute(t, typeof(GuiWindowOptionsAttribute));
			if (nameAttribute != null) {
				return nameAttribute.name;
			}

			return t.Name;

		}
		private static bool GetAllowMultipleOfGuiWindow(Type t) {

			GuiWindowOptionsAttribute guiWindowAttribute = (GuiWindowOptionsAttribute)Attribute.GetCustomAttribute(t, typeof(GuiWindowOptionsAttribute));
			if (guiWindowAttribute != null) {
				return guiWindowAttribute.allowMultipleWindows;
			}
			return false;

		}

		private static void SaveWindowOptions() {

			JObject configJson = new JObject();
			JArray windowOptions = new JArray();
			JArray overlayOptions = new JArray();

			for (int i = 0; i < windows.Count; i ++) {
				JObject currentWindowOption = new JObject();
				currentWindowOption.Put("name", GetNameOfGuiWindow(windows[i].GetType()));
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

			
			windows.Clear();

			JObject configJson = new JObject(File.ReadAllText("gui.json"));
			JArray windowOptions = configJson.GrabArray("windows");
			for (int i = 0; i < windowOptions.Count; i ++) {

				JObject currentWindowOption = windowOptions[i];
				string name = currentWindowOption.Grab("name");
				
				// Open target window
				for (int n = 0; n < possibleWindowsList.Count; n ++) {
					if (GetNameOfGuiWindow(possibleWindowsList[n]) == name) {
						OpenGuiWindow((GuiWindow)Activator.CreateInstance(possibleWindowsList[i]));
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
