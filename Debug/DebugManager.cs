using ArcticFoxEngine.Demos.ChildTest;
using ArcticFoxEngine.Demos.LightingTest;
using ArcticFoxEngine.Demos.RenderingStressTest;
using ArcticFoxEngine.Demos.SceneTest;
using ArcticFoxEngine.ImGuiIntegration;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using SharpDX.Windows;

namespace ArcticFoxEngine.Debug {
	public static class DebugManager {

		private static bool isOpen;
		private static List<DebugWindow> windows;
		private static List<DebugOverlay> overlays;
		private static bool showImGuiDemo;

		private static List<Type> demoNodes;

		internal static void Init(RenderForm form) {


			RenderImGui.Init(1920, 1080);
			ImGuiInput.Init(form.Handle);
			ImGui.LoadIniSettingsFromDisk("imgui.ini");

			isOpen = false;
			windows = new List<DebugWindow>() {
				new DebugLog(),
				new DebugMeshBuffers(),
				new DebugPerformance(),
				new DebugRender(),
				new DebugScene(),
			};
			overlays = new List<DebugOverlay>() {
				new DebugNodeGizmos(),
			};
			overlays[0].open = true;
			LoadWindowOptions();

			demoNodes = new List<Type>() {
				typeof(CubeSpin),
				typeof(ChildTestNode),
				typeof(RenderingStressTestNode),
				typeof(LightingTestNode),
			};


			Log.ListenToLog(GetDebugWindow<DebugLog>().LogEvent);
			Log.ListenToLogColor(GetDebugWindow<DebugLog>().LogColorEvent);

		}

		internal static T GetDebugWindow<T>() where T : DebugWindow {

			for (int i = 0; i < windows.Count; i++) {
				if (windows[i].GetType() == typeof(T)) {
					return (T)windows[i];
				}
			}
			return null;

		}

		public static void OpenGUI() {

			LoadWindowOptions();

			isOpen = true;

		}
		public static void CloseGUI() {
			SaveWindowOptions();
			ImGui.SaveIniSettingsToDisk("imgui.ini");
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
					ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 1.0f));
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

					ImGui.Checkbox("Show ImGui Demo", ref showImGuiDemo);
					ImGui.Separator();

					for (int i = 0; i < windows.Count; i++) {
						if (ImGui.MenuItem(windows[i].name, null, ref windows[i].open) == true) {
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

			if (showImGuiDemo == true) {
				ImGui.ShowDemoWindow();
			}
			for (int i = 0; i < windows.Count; i++) {
				if (windows[i].open == true) {
					ImGui.Begin(windows[i].name + "##" + windows[i].GetHashCode(), ref windows[i].open);
					windows[i].Render();
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
			for (int i = 0; i < windows.Count; i++) {
				saveValue <<= 1;
				saveValue += (byte)(windows[i].open == true ? 1 : 0);
			}

			File.WriteAllBytes("debugconfig", new byte[] { saveValue });

		}
		private static void LoadWindowOptions() {

			if (File.Exists("debugconfig") == false) {
				return;
			}
			byte saveValue = File.ReadAllBytes("debugconfig")[0];
			for (int i = windows.Count - 1; i >= 0; i--) {
				if ((saveValue & 1) == 1) {
					windows[i].open = true;
				}
				else {
					windows[i].open = false;
				}
				saveValue >>= 1;
			}

		}

		public static void Dispose() {
			RenderImGui.Dispose();
		}

	}
}
