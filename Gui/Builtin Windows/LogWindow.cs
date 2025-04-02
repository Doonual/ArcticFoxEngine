using CoolClassLibrary;
using ImGuiNET;

namespace ArcticFoxEngine.Gui {

	[GuiWindowOptions("Log")]
	internal class LogWindow : GuiWindow {

		static private List<string> messages;
		static private string cmdInput;
		static private bool scrollToBottom;
		static private bool pauseOutput;


		public LogWindow() {
			messages = new List<string>();
			messages.Add("");
			cmdInput = "";
		}

		static LogWindow() {
			Log.ListenToLog(LogEvent);
			Log.ListenToLogColor(LogColorEvent);
		}
		internal static void LogEvent(string text) {

			if (pauseOutput == true) { return; }

			for (int i = 0; i < text.Length; i++) {
				if (text[i] != '\n') {
					messages[messages.Count - 1] += text[i];
				}
				else {

					messages.Add("");
				}
			}
			scrollToBottom = true;

		}
		internal static void LogColorEvent(ConsoleColor col) {

			if (pauseOutput == true) { return; }

			string colString = "";
			switch (col) {

				case ConsoleColor.Black:
				colString = "000000";
				break;

				case ConsoleColor.DarkBlue:
				colString = "00007F";
				break;

				case ConsoleColor.DarkGreen:
				colString = "007F00";
				break;

				case ConsoleColor.DarkCyan:
				colString = "007F7F";
				break;

				case ConsoleColor.DarkRed:
				colString = "7F0000";
				break;

				case ConsoleColor.DarkMagenta:
				colString = "7F007F";
				break;

				case ConsoleColor.DarkYellow:
				colString = "7F7F00";
				break;

				case ConsoleColor.Gray:
				colString = "7F7F7F";
				break;

				case ConsoleColor.DarkGray:
				colString = "404040";
				break;

				case ConsoleColor.Blue:
				colString = "0000FF";
				break;

				case ConsoleColor.Green:
				colString = "00FF00";
				break;

				case ConsoleColor.Cyan:
				colString = "00FFFF";
				break;

				case ConsoleColor.Red:
				colString = "FF0000";
				break;

				case ConsoleColor.Magenta:
				colString = "FF00FF";
				break;

				case ConsoleColor.Yellow:
				colString = "FFFF00";
				break;

				case ConsoleColor.White:
				colString = "FFFFFF";
				break;

			}

			messages[messages.Count - 1] += "!" + colString;

		}

		public override void Render() {

			ImGui.Begin("Log", ref open);
			if (ImGui.Button("Clear") == true) {
				messages.Clear();
				messages.Add("");
			}
			ImGui.SameLine();
			pauseOutput ^= ImGui.Checkbox("Pause output", ref pauseOutput);

			ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.16f, 0.16f, 0.16f, 0.54f));
			ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1f);
			ImGui.BeginChildFrame((uint)"DebugLog messages child".GetHashCode(), new Vector2(-1f, 200f));
			ImGui.PopStyleVar();
			ImGui.PopStyleColor();

			if (scrollToBottom == true) {
				scrollToBottom = false;
				ImGui.SetScrollHereY(-ImGui.GetScrollMaxY());
			}

			for (int i = 0; i < messages.Count; i++) {

				string[] cols = messages[i].Split('!');


				for (int n = 0; n < cols.Length; n++) {

					bool pushedCol = false;
					if (cols[n].Length >= 6) {
						System.Drawing.Color? col = MathUtil.ParseColor(cols[n].Substring(0, 6));
						if (col != null) {
							ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(col.Value.R / 255f, col.Value.G / 255f, col.Value.B / 255f, 1f));
							pushedCol = true;
						}

					}
					if (cols[n].Length > 6) {
						ImGui.TextWrapped(cols[n].Substring(6));
						ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(-1f, 0f));
						ImGui.SameLine();
						ImGui.PopStyleVar();

					}

					if (pushedCol == true) {
						ImGui.PopStyleColor();
					}

				}

				if (i != messages.Count - 1) {
					ImGui.NewLine();
				}


			}

			ImGui.EndChildFrame();


			ImGui.PushItemWidth(-45f);


			bool sendCommand = ImGui.InputText("", ref cmdInput, 64u, ImGuiInputTextFlags.EnterReturnsTrue);
			if (sendCommand == true) {
				ImGui.SetKeyboardFocusHere(-1);
			}
			ImGui.PopItemWidth();
			ImGui.SameLine();

			if (ImGui.Button("Send") == true || sendCommand == true) {

				CommandController.ExecuteCommand(cmdInput);
				cmdInput = "";
			}
			
			ImGui.End();


		}

	}
}
