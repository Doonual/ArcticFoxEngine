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

		private static List<GuiWindow> imguiWindows;
		private static List<GuiWindow> builtinWindows;
		private static List<GuiWindow> userWindows;

		private static List<GuiOverlay> overlays;

		private static List<Type> demoNodes;

		private static Vector4 menuSubtitleCol = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);

		internal static void Init(RenderForm form) {

			RenderImGui.Init(1920, 1080);
			ImGuiInput.Init(form.Handle);
			
			isOpen = false;

			imguiWindows = new List<GuiWindow>() {
				new ImGuiAboutWindow(),
				new ImGuiDebugLogWindow(),
				new ImGuiDemoWindow(),
				new ImGuiFontSelectorWindow(),
				new ImGuiMetricsWindow(),
				new ImGuiStackToolWindow(),
				new ImGuiStyleEditorWindow(),
				new ImGuiStyleSelectorWindow(),
				new ImGuiUserGuideWindow(),
			};

			builtinWindows = new List<GuiWindow>() {
				new LogWindow(),
				new DebugMeshBuffers(),
				new PerformanceWindow(),
				new RenderWindow(),
				new SceneWindow(),
			};
			userWindows = new List<GuiWindow>();

			overlays = new List<GuiOverlay>() {
				new NodeGizmosOverlay(),
			};
			overlays[0].open = true;
			LoadWindowOptions();

			demoNodes = new List<Type>() {
				typeof(CubeSpin),
				typeof(ChildTestNode),
				typeof(RenderingStressTestNode),
				typeof(LightingTestNode),
			};


			Log.ListenToLog(GetDebugWindow<LogWindow>().LogEvent);
			Log.ListenToLogColor(GetDebugWindow<LogWindow>().LogColorEvent);
			LoadWindowOptions();

		}

		internal static T GetDebugWindow<T>() where T : GuiWindow {

			for (int i = 0; i < builtinWindows.Count; i++) {
				if (builtinWindows[i].GetType() == typeof(T)) {
					return (T)builtinWindows[i];
				}
			}
			return null;

		}
		public static void RegisterDebugWindow(GuiWindow debugWindow) {
			userWindows.Add(debugWindow);
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

		internal static void Render() {

			if (ImGui.BeginMainMenuBar() == true) {

				// Scene menu options
				if (ImGui.BeginMenu("Scene") == true) {
					ImGui.PushStyleColor(ImGuiCol.Text, menuSubtitleCol);
					ImGui.Text("Demos");
					ImGui.PopStyleColor();
					for (int i = 0; i < demoNodes.Count; i++) {
						if (ImGui.MenuItem(demoNodes[i].Name) == true) {
							if (Node.rootNode != null) {
								Node.rootNode.DisposeEvent();
							}
							Node newNode = (Node)Activator.CreateInstance(demoNodes[i]);
							Node.SetRootNode(newNode);
						}
					}

					ImGui.EndMenu();
				}


				// Window menu options
				if (ImGui.BeginMenu("Window") == true) {

					if (ImGui.MenuItem("Save window layout") == true) {
						SaveWindowOptions();
					}
					if (ImGui.MenuItem("Load window layout") == true) {
						LoadWindowOptions();
					}

					// ImGui Demo windows
					ImGui.PushStyleColor(ImGuiCol.Text, menuSubtitleCol);
					ImGui.SeparatorText("Builtin");
					ImGui.PopStyleColor();

					if (ImGui.BeginMenu("Demos") == true) {
						for (int i = 0; i < imguiWindows.Count; i++) {
							if (ImGui.MenuItem(imguiWindows[i].name, null, ref imguiWindows[i].open) == true) {
								SaveWindowOptions();
							}
						}
						ImGui.EndMenu();
					}
					

					
					if (ImGui.BeginMenu("Arctic Fox Engine") == true) {
						for (int i = 0; i < builtinWindows.Count; i++) {
							if (ImGui.MenuItem(builtinWindows[i].name, null, ref builtinWindows[i].open) == true) {
								SaveWindowOptions();
							}
						}
						ImGui.EndMenu();
					}

					ImGui.PushStyleColor(ImGuiCol.Text, menuSubtitleCol);
					ImGui.SeparatorText("User");
					ImGui.PopStyleColor();
					for (int i = 0; i < userWindows.Count; i++) {
						if (ImGui.MenuItem(userWindows[i].name, null, ref userWindows[i].open) == true) {
							SaveWindowOptions();
						}
					}
	
					


					ImGui.EndMenu();
				}

				// Overlay menu options
				if (ImGui.BeginMenu("Overlay") == true) {
					for (int i = 0; i < overlays.Count; i ++) {
						ImGui.MenuItem(overlays[i].name, null, ref overlays[i].open);
					}
					ImGui.EndMenu();
				}
				

			}
			ImGui.EndMenuBar();


			// Render builtin windows
			for (int i = 0; i < imguiWindows.Count; i++) {
				if (imguiWindows[i].open == true) {
					// We can skip the ImGUi.begin here because the imgui windows already have the ImGui.Begin();
					//ImGui.Begin(imguiWindows[i].name + "##" + imguiWindows[i].GetHashCode(), ref builtinWindows[i].open);
					imguiWindows[i].Render();
					//ImGui.End();
				}
			}
			for (int i = 0; i < builtinWindows.Count; i++) {
				if (builtinWindows[i].open == true) {
					ImGui.Begin(builtinWindows[i].name + "##" + builtinWindows[i].GetHashCode(), ref builtinWindows[i].open);
					builtinWindows[i].Render();
					ImGui.End();
				}
			}
			for (int i = 0; i < userWindows.Count; i++) {
				if (userWindows[i].open == true) {
					ImGui.Begin(userWindows[i].name + "##" + userWindows[i].GetHashCode(), ref userWindows[i].open);
					userWindows[i].Render();
					ImGui.End();
				}
			}
			for (int i = 0; i < overlays.Count; i++) {
				if (overlays[i].open == true) {
					overlays[i].Render();
				}
			}

		}

		private static void SaveWindowOptions() {

			byte saveValue = 0;
			for (int i = 0; i < builtinWindows.Count; i++) {
				saveValue <<= 1;
				saveValue += (byte)(builtinWindows[i].open == true ? 1 : 0);
			}

			File.WriteAllBytes("debugconfig", new byte[] { saveValue });
			ImGui.SaveIniSettingsToDisk("imgui.ini");

		}
		private static void LoadWindowOptions() {

			if (File.Exists("debugconfig") == false) {
				return;
			}
			byte saveValue = File.ReadAllBytes("debugconfig")[0];
			for (int i = builtinWindows.Count - 1; i >= 0; i--) {
				if ((saveValue & 1) == 1) {
					builtinWindows[i].open = true;
				}
				else {
					builtinWindows[i].open = false;
				}
				saveValue >>= 1;
			}
			ImGui.LoadIniSettingsFromDisk("imgui.ini");

		}

		public static void Dispose() {
			RenderImGui.Dispose();
		}

	}
}
