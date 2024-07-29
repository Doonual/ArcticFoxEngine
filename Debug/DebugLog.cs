using CoolClassLibrary;
using ImGuiNET;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Debug {
	internal static class DebugLog {

		private static List<string> messages;
		private static string cmdInput;
		private static bool scrollToBottom;
		private static bool pauseOutput;

		static DebugLog() {
			messages = new List<string>();
			messages.Add("");
			cmdInput = "";
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
				colString = "0000FF";
				break;

				case ConsoleColor.DarkGreen:
				colString = "00FF00";
				break;

				case ConsoleColor.DarkCyan:
				colString = "00FFFF";
				break;

				case ConsoleColor.DarkRed:
				colString = "FF0000";
				break;

				case ConsoleColor.DarkMagenta:
				colString = "FF00FF";
				break;

				case ConsoleColor.DarkYellow:
				colString = "FFFF00";
				break;

				case ConsoleColor.Gray:
				colString = "7F7F7F";
				break;

				case ConsoleColor.DarkGray:
				colString = "404040";
				break;

				case ConsoleColor.Blue:
				colString = "7F7FFF";
				break;

				case ConsoleColor.Green:
				colString = "7FFF7F";
				break;

				case ConsoleColor.Cyan:
				colString = "7FFFFF";
				break;

				case ConsoleColor.Red:
				colString = "FF7F7F";
				break;

				case ConsoleColor.Magenta:
				colString = "FF7FFF";
				break;

				case ConsoleColor.Yellow:
				colString = "FFFF7F";
				break;

				case ConsoleColor.White:
				colString = "FFFFFF";
				break;

			}

			messages[messages.Count - 1] += "!" + colString;

		}

		internal static void Render() {

			ImGui.Begin("Log");

			if (ImGui.Button("Clear") == true) {
				messages.Clear();
				messages.Add("");
			}
			ImGui.SameLine();
			pauseOutput ^= ImGui.Checkbox("Pause output", ref pauseOutput);

			ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0.16f, 0.16f, 0.16f, 0.54f));
			ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1f);
			ImGui.BeginChildFrame((uint)"DebugLog messages child".GetHashCode(), new System.Numerics.Vector2(-1f, 200f));

			if (scrollToBottom == true) {
				scrollToBottom = false;
				ImGui.SetScrollHereY(-ImGui.GetScrollMaxY());
			}
			

			for (int i = 0; i < messages.Count; i++) {

				string[] cols = messages[i].Split('!');


				for (int n = 0; n < cols.Length; n++) {

					if (cols[n].Length >= 6) {
						System.Drawing.Color? col = MathUtil.ParseColor(cols[n].Substring(0, 6));
						if (col != null) {
							ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(col.Value.R / 255f, col.Value.G / 255f, col.Value.B / 255f, 1f));
						}

					}
					if (cols[n].Length > 6) {


						ImGui.TextWrapped(cols[n].Substring(6));
						ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(-1f, 0f));
						ImGui.SameLine();
						ImGui.PopStyleVar(1);
					}

				}

				if (i != messages.Count - 1) {
					ImGui.NewLine();
				}
				

			}

			ImGui.EndChild();
			ImGui.PopStyleColor();
			ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(1f, 1f, 1f, 1f));
			ImGui.PopStyleVar();


			ImGui.PushItemWidth(-45f);
			
			bool sendCommand = ImGui.InputText("", ref cmdInput, 64u, ImGuiInputTextFlags.EnterReturnsTrue);
			ImGui.PopItemWidth();
			ImGui.SameLine();
			
			if (ImGui.Button("Send") == true || sendCommand == true) {

				CommandController.ExecuteCommand(cmdInput);
				cmdInput = "";
				
				

			}	


		}

	}
}
