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


		private static bool renderWindowOpen = true;
		private static bool sceneWindowOpen = true;
		private static bool performanceWindowOpen = false;
		private static bool meshWindowOpen = false;
		private static bool logWindowOpen = true;
		protected override void Render() {

			ImGuiWindowFlags flags = ImGuiWindowFlags.None;
			flags |= ImGuiWindowFlags.AlwaysAutoResize;
			flags |= ImGuiWindowFlags.NoTitleBar;

			ImGui.SetNextWindowPos(new System.Numerics.Vector2(0f, 0f));
			ImGui.Begin("Menu buttons", flags);
			if (ImGui.BeginMenu("Window") == true) {
				renderWindowOpen |= ImGui.MenuItem("Render") == true;
				sceneWindowOpen |= ImGui.MenuItem("Scene") == true;
				performanceWindowOpen |= ImGui.MenuItem("Performance") == true;
				meshWindowOpen |= ImGui.MenuItem("Mesh") == true;
				logWindowOpen |= ImGui.MenuItem("Log") == true;
			}

			ImGui.EndMenu();
			ImGui.End();

			if (renderWindowOpen == true) {
				ImGui.Begin("Render", ref renderWindowOpen);
				DebugRender.Render();
				ImGui.End();
			}
			if (sceneWindowOpen == true) {
				ImGui.Begin("Scene", ref sceneWindowOpen);
				DebugScene.Render();
				ImGui.End();
			}
			if (performanceWindowOpen == true) {
				ImGui.Begin("Performance", ref performanceWindowOpen);
				DebugPerformance.Render();
				ImGui.End();
			}
			if (meshWindowOpen == true) {
				ImGui.Begin("Mesh buffer data debugger", ref meshWindowOpen);
				DebugMeshBuffers.Render();
				ImGui.End();
			}
			if (logWindowOpen == true) {
				ImGui.Begin("Log", ref logWindowOpen);
				DebugLog.Render();
				ImGui.End();
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
	}
}
