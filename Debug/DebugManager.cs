using ArcticFoxEngine.Testing;
using ArcticFoxEngine.Testing.SceneTest;
using ArcticFoxEngine.Testing.ChildTest;
using ArcticFoxEngine.Demos.RenderingStressTest;
using ArcticFoxEngine;
using CoolClassLibrary;
using ImGuiNET;
using System.Windows.Forms;
using SharpDX.Windows;

namespace ArcticFoxEngine.Debug {
	public static class DebugManager {

		private static bool isOpen;
		private static List<DebugWindow> windows;
		private static bool showImGuiDemo;

		private static List<DemoScene> demoScenes;

		internal static void Init(RenderForm form) {

			
			GPU_RenderImGui.Init(1920, 1080);
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
			LoadWindowOptions();
			
			demoScenes = new List<DemoScene>() {
				new HelloSceneDemo(),
				new ChildTestDemo(),
				new RenderingStressTestDemo(),
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
			GPU_RenderImGui.Render(Graphics.renderTargets[Graphics.frameIndex], Graphics.rtvHeap, Graphics.dsvHeap);

		}

		internal static void Render() {

			if (ImGui.BeginMainMenuBar() == true) {
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
				
				if (ImGui.BeginMenu("Scene") == true) {
					ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 1.0f));
					ImGui.Text("Demos");
					ImGui.PopStyleColor();
					for (int i = 0; i < demoScenes.Count; i ++) {
						if (ImGui.MenuItem(demoScenes[i].name) == true) {
							Scene.LoadDemoScene(demoScenes[i]);
						}
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
					ImGui.Begin(windows[i].name, ref windows[i].open);
					windows[i].Render();
					ImGui.End();
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
			GPU_RenderImGui.Dispose();
		}

	}
}
