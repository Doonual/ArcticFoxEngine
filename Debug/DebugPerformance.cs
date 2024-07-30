using ImGuiNET;
using ClickableTransparentOverlay;
using CoolClassLibrary;
using ArcticFoxEngine.Backend;
using static System.Net.Mime.MediaTypeNames;

namespace ArcticFoxEngine.Debug {
	public static class DebugPerformance {

		static double msMax = 0.0;
		static double msMaxView = 0.0;

		static double msMin = 0.0;
		static double msMinView = 0.0;

		static float[] fpsVals;
		static int numElements;
		static float msLine;


		static DebugPerformance() {

			fpsVals = new float[2000];
			numElements = fpsVals.Length;
			msLine = 16.67f;

		}

		internal static void UpdateVals() {

			double ms = Profiler.GetFrameTime() * 1000;

			if (ms > msMax) {
				msMax = ms;
				msMaxView = ms;
			}

			msMax = (msMax - ms) * 0.994 + ms;

			if (ms < msMin) {
				msMin = ms;
				msMinView = ms;
			}
			msMin = (msMin - ms) * 0.994 + ms;

			for (int i = 0; i < fpsVals.Length - 1; i++) {
				fpsVals[i] = fpsVals[i + 1];
			}
			fpsVals[fpsVals.Length - 1] = (float)ms;


		}


		internal static void Render() {

			double ms = Profiler.GetFrameTime() * 1000;

			ImGuiTableFlags flags = ImGuiTableFlags.Borders;
			ImGui.BeginTable("Performance Table", 3, flags);

			ImGui.TableSetupColumn("Worst");
			ImGui.TableSetupColumn("Current");
			ImGui.TableSetupColumn("Best");
			ImGui.TableHeadersRow();
			ImGui.TableNextColumn();

			ImGui.Text(msMaxView.ToString("F") + " ms");
			ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.8f, 0, 0, 0.25f)));
			ImGui.TableNextColumn();
			ImGui.Text(ms.ToString("F") + " ms");
			ImGui.TableNextColumn();
			ImGui.Text(msMinView.ToString("F") + " ms");
			ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0f, 0.8f, 0, 0.25f)));
			
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.Text((1000.0 / msMaxView).ToString("F") + " fps");
			ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.8f, 0, 0, 0.25f)));
			ImGui.TableNextColumn();
			ImGui.Text((1000.0 / ms).ToString("F") + " fps");
			ImGui.TableNextColumn();
			ImGui.Text((1000.0 / msMinView).ToString("F") + " fps");
			ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0f, 0.8f, 0, 0.25f)));
			
			ImGui.EndTable();

			float preWidth = ImGui.GetColumnWidth();
			ImGui.Columns(2);
			ImGui.SetColumnWidth(0, preWidth - 120);

			float plotMinMs = 0f;
			float plotMaxMs = 40f;
			float[] testSquished = SquishPlot(fpsVals, numElements);
			System.Numerics.Vector2 histCurStart = ImGui.GetCursorPos();

			int histWidth = (int)ImGui.GetCursorPosX();
			ImGui.PlotHistogram("", ref testSquished[0], numElements, 0, "", plotMinMs, plotMaxMs, new System.Numerics.Vector2(-1f, 150f));
			ImGui.SameLine();
			
			histWidth = (int)(ImGui.GetCursorPosX() - histWidth - ImGui.GetStyle().FramePadding.X * 2 - ImGui.GetStyle().ItemSpacing.X);
			numElements = histWidth;
			ImGui.NewLine();
			System.Numerics.Vector2 histCurEnd = ImGui.GetCursorPos();

			ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0f, 0f, 0f, 0f));
			float[] lines = new float[] { msLine, msLine };
			ImGui.SetCursorPos(histCurStart);
			ImGui.PlotLines("", ref lines[0], 2, 0, "", plotMinMs, plotMaxMs, new System.Numerics.Vector2(-1f, 150f));
			ImGui.PopStyleColor();

			ImGui.NextColumn();

			float[] msCheckVals = new float[] {8.3f, 16.7f, 33.3f};
			for (int i = 0; i < msCheckVals.Length; i ++) {
				float buttonHeight = MathUtil.Map(msCheckVals[i], plotMinMs, plotMaxMs, histCurEnd.Y - 8, histCurStart.Y);
				ImGui.SetCursorPos(new System.Numerics.Vector2(ImGui.GetCursorPosX(), buttonHeight - 8));
				if (ImGui.Button(msCheckVals[i] + " ms | " + MathF.Round(1000f / msCheckVals[i]) + " FPS") == true) {
					msLine = msCheckVals[i];
				}
			}

			ImGui.Columns();
			
		}

		private static float[] SquishPlot(float[] inVals, int targetNumVals) {

			float[] outVals = new float[targetNumVals];
			for (int i = 0; i < outVals.Length; i++) {
				outVals[i] = 0f;
			}
			float averageCount = (float)inVals.Length / targetNumVals;
			int currentDestIndex = 0;
			float destRemaining = averageCount;
			for (int i = 0; i < inVals.Length; i++) {

				float srcRemaining = 1f;

				while (srcRemaining > 0f && currentDestIndex < targetNumVals) {
					if (destRemaining > 0f) {

						float amntTransfer = MathF.Min(srcRemaining, destRemaining);
						outVals[currentDestIndex] += inVals[i] * amntTransfer / averageCount;
						srcRemaining -= amntTransfer;
						destRemaining -= amntTransfer;
					}
					else {
						currentDestIndex += 1;
						destRemaining = averageCount;
					}
				}
			}
			return outVals;

		}


	}
}
