using ImGuiNET;
using ClickableTransparentOverlay;
using CoolClassLibrary;
using ArcticFoxEngine.Backend;

namespace ArcticFoxEngine.Debug {
	public static class DebugPerformance {

		static double msMax = 0.0;
		static double msMaxView = 0.0;

		static double msMin = 0.0;
		static double msMinView = 0.0;

		static float[] fpsVals;
		static int fpsValPos;

		static int logFrame;
		static double interpolateMs;

		static DebugPerformance() {
			fpsVals = new float[500];
			fpsValPos = 0;

			logFrame = 0;
		}

		internal static void UpdateVals() {

			

			double ms = Profiler.GetFrameTime() * 1000;
			interpolateMs = Math.Max(interpolateMs, ms);

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


			fpsVals[fpsValPos] = (float)interpolateMs;
			fpsVals[fpsValPos + fpsVals.Length / 2] = (float)interpolateMs;

			logFrame++;
			logFrame %= 10;
			if (logFrame != 0) { return; }

			fpsValPos += 1;
			fpsValPos %= fpsVals.Length / 2;
			interpolateMs = 0.0;

		}

		internal static void Render() {

			double ms = Profiler.GetFrameTime() * 1000;

			ImGui.Begin("Performance");

			ImGuiTableFlags flags = ImGuiTableFlags.Borders;
			ImGui.BeginTable("Performance Table", 4, flags);


			ImGui.TableSetupColumn("");
			ImGui.TableSetupColumn("Worst");
			ImGui.TableSetupColumn("Best");
			ImGui.TableSetupColumn("Current");
			ImGui.TableHeadersRow();
			ImGui.TableNextColumn();

			
			ImGui.Text("ms");
			ImGui.TableNextColumn();
			ImGui.Text(msMaxView.ToString("F"));
			ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.8f, 0, 0, 0.25f)));
			ImGui.TableNextColumn();
			ImGui.Text(msMinView.ToString("F"));
			ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0f, 0.8f, 0, 0.25f)));
			ImGui.TableNextColumn();
			ImGui.Text(ms.ToString("F"));

			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.Text("fps");
			ImGui.TableNextColumn();
			ImGui.Text((1000.0 / msMaxView).ToString("F"));
			ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.8f, 0, 0, 0.25f)));
			ImGui.TableNextColumn();
			ImGui.Text((1000.0 / msMinView).ToString("F"));
			ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0f, 0.8f, 0, 0.25f)));
			ImGui.TableNextColumn();
			ImGui.Text((1000.0 / ms).ToString("F"));

			
			
			
			ImGui.EndTable();

			int numSamples = (int)ImGui.GetColumnWidth();

			ImGui.PlotLines("FPS", ref fpsVals[fpsValPos + 1], fpsVals.Length / 2, 0, "", 0f, 1000f / 40f, new System.Numerics.Vector2(-30f, 150f));

			ImGui.End();
			
		}


	}
}
