using ClickableTransparentOverlay;
using CoolClassLibrary;
using ImGuiNET;

namespace ArcticFoxEngine.Debug {
	public class DebugManager : Overlay {

		static DebugManager debugManager;
		static ImGuiIOPtr ioPtr;

		public static bool isOpen {
			get;
			private set;
		}
		
		static DebugManager() {
			isOpen = false;
		}

		internal static void InitImGui() {

			// This shouldnt work, but it does
			ImGui.CreateContext();
			ioPtr = ImGui.GetIO();
			ioPtr.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

			ImGui.StyleColorsDark();

			Log.ListenToLog(DebugLog.LogEvent);
			Log.ListenToLogColor(DebugLog.LogColorEvent);

		}

		public static void OpenGUI() {

			if (isOpen == true) { return; }

			isOpen = true;

			debugManager = new DebugManager();
			debugManager.Start();


		}
		public static void CloseGUI() {

			if (isOpen == false) { return; }

			isOpen = false;
			debugManager.Close();
			debugManager.Dispose();

		}

		protected override void Render() {

			DebugRender.Render();
			DebugScene.Render();
			DebugPerformance.Render();
			DebugMeshBuffers.Render();
			DebugLog.Render();

		}


		internal static string FormatHex(int hexValue) {

			string hexString = hexValue.ToString("X");
			while (hexString.Length < 7) {
				hexString = "0" + hexString;
			}
			hexString = "0x" + hexString;
			return hexString;

		}
	}
}
