using ClickableTransparentOverlay;
using CoolClassLibrary;
using ImGuiNET;

namespace ArcticFoxEngine.Debug {
	public class DebugManager : Overlay {

		static DebugManager debugManager;
		static ImGuiIOPtr ioPtr;
		private static List<DebugWindow> windows;
		private static bool showDemo;
		public static bool isOpen {
			get;
			private set;
		}

		static DebugManager() {
			isOpen = false;
			windows = new List<DebugWindow>() {
				new DebugLog(),
				new DebugMeshBuffers(),
				new DebugPerformance(),
				new DebugRender(),
				new DebugScene(),
			};
			LoadWindowOptions();
		}

		internal static T GetDebugWindow<T>() where T : DebugWindow {

			for (int i = 0; i < windows.Count; i++) {
				if (windows[i].GetType() == typeof(T)) {
					return (T)windows[i];
				}
			}
			return null;

		}

		internal static void InitImGui() {

			// This shouldnt work, but it does
			ImGui.CreateContext();
			ioPtr = ImGui.GetIO();
			ioPtr.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

			ImGui.StyleColorsDark();

			Log.ListenToLog(GetDebugWindow<DebugLog>().LogEvent);
			Log.ListenToLogColor(GetDebugWindow<DebugLog>().LogColorEvent);

		}

		public static void OpenGUI() {

			if (isOpen == true) { return; }

			isOpen = true;

			debugManager = new DebugManager();
			debugManager.Start();

			LoadWindowOptions();


		}
		public static void CloseGUI() {

			if (isOpen == false) { return; }

			isOpen = false;
			debugManager.Close();
			debugManager.Dispose();

		}

		protected override void Render() {

			if (ImGui.BeginMainMenuBar() == true) {
				if (ImGui.BeginMenu("Window") == true) {

					ImGui.Checkbox("Show ImGui Demo", ref showDemo);
					ImGui.Separator();

					for (int i = 0; i < windows.Count; i++) {
						if (ImGui.MenuItem(windows[i].name, null, ref windows[i].open) == true) {
							SaveWindowOptions();
						}
					}

				}
			}
			ImGui.EndMenuBar();

			if (showDemo == true) {
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

		internal static string FormatHex(int hexValue) {

			string hexString = hexValue.ToString("X");
			while (hexString.Length < 7) {
				hexString = "0" + hexString;
			}
			hexString = "0x" + hexString;
			return hexString;

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

	}
}
